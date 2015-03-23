Imports LyriX.Document
Imports LyriX.Document.ObjectModel

Namespace Compilers
    ''' <summary>
    ''' 表示一个用于将 LyriX 包转换为便于读取的 LyriX 文档的编译器。
    ''' </summary>
    ''' <remarks>编译器负责为所有未指定的数据确定值。</remarks>
    Public NotInheritable Class LyriXCompiler
        '对于可引用、可继承的属性的继承策略：
        '如果某一属性未被指定值，则会按照以下顺序来查找默认值：
        '1.Ref 属性指定的引用目标的对应值（不支持向后引用）
        '2.（如果 Ref 属性未指定）使用父级元素对应属性编译后的值
        '3.预定义的默认值
        '
        '对于包含引用属性的项的列表在编译时的扫描策略：
        '第一遍扫描，根据指定列表对应构造一个包含编译后的项（保存在 Destination 中）的列表，
        '               并将编译前的数据保存在 DataSource 中。
        '第二遍扫描，将 DataSource 中的所有的索引引用转换为具体的引用。

        '编译顺序：
        '   Header -> MusicInfo -> Lyrics
        'Line 与 Span 按 BeginTime 升序排序
        'Segment 的 Begin、Duration 按照 Span 的时间长度按 Segment 的 Text.Length 加权插值

        Private m_Output As New List(Of CompilerOutput),
            m_s_Output As IList(Of CompilerOutput) = m_Output.AsReadOnly,
            m_DefaultDeviation As TimeSpan = TimeSpan.FromSeconds(3)
        '运行状态
        Private Package As LyriXPackage
        Private CompiledDocument As Compiled.LyricsDocument

        ''' <summary>
        ''' 获取编译器的输出信息。
        ''' </summary>
        Public ReadOnly Property Output As IList(Of CompilerOutput)
            Get
                Return m_s_Output
            End Get
        End Property

        ''' <summary>
        ''' 清除编译器的输出信息。
        ''' </summary>
        Public Sub ClearOutput()
            m_Output.Clear()
        End Sub

        ''' <summary>
        ''' 获取/设置一个值，该值将作为编译后的歌词版本在按歌曲长度匹配时的默认允许匹配误差。
        ''' </summary>
        Public Property DefaultDeviation As TimeSpan
            Get
                Return m_DefaultDeviation
            End Get
            Set(ByVal value As TimeSpan)
                m_DefaultDeviation = value
            End Set
        End Property

#Region "实用工具"

        ''' <summary>
        ''' 根据给定的选择器获取一个包含本地化信息的值。
        ''' </summary>
        ''' <typeparam name="TResult">要返回的对象的类型。</typeparam>
        ''' <param name="FieldGetter">对象的选择器，如果找到匹配项，则应返回一个不为 <c>null</c> 的值。</param>
        ''' <returns>最匹配的本地化对象。如果未找到，则为 <c>null</c>。</returns>
        Private Function GetLocalizedField(Of TResult)(ByVal FieldGetter As Func(Of LocalizedPackageParts, TResult)) As MultiLanguageValue(Of TResult)
            Dim RV As New MultiLanguageValue(Of TResult)
            For Each EachLocalizedPart In Package.LocalizedParts.Items
                RV.Value(EachLocalizedPart.Language) = FieldGetter(EachLocalizedPart)
            Next
            Return RV
        End Function

        ''' <summary>
        ''' 根据给定的选择器获取一个包含本地化与非本地化信息的值。
        ''' </summary>
        ''' <typeparam name="TResult">要返回的对象的类型。</typeparam>
        ''' <param name="FieldGetter">对象的选择器，如果找到匹配项，则应返回一个不为 <c>null</c> 的值。</param>
        ''' <returns>最匹配的本地化对象。如果未找到，则为 <c>null</c>。</returns>
        Private Function GetMultiLanguageField(Of TResult)(ByVal DefaultValue As TResult, ByVal FieldGetter As Func(Of LocalizedPackageParts, TResult)) As MultiLanguageValue(Of TResult)
            Dim RV = GetLocalizedField(FieldGetter)
            RV.InvariantValue = DefaultValue
            Return RV
        End Function

        ' ''' <summary>
        ' ''' 根据给定的选择器获取一个包含本地化与非本地化信息的值。
        ' ''' </summary>
        ' ''' <typeparam name="TResult">要返回的对象的类型。</typeparam>
        ' ''' <param name="FieldGetter1">对象的选择器#1，其处理后的值如果不为 <c>null</c>，则将传递给 FieldGetter2。</param>
        ' ''' <param name="FieldGetter2">对象的选择器#2，如果找到匹配项，则应返回一个不为 <c>null</c> 的值。</param>
        ' ''' <returns>最匹配的本地化对象。如果未找到，则为 <c>null</c>。</returns>
        'Private Function GetMultiLanguageField(Of TResult, TResult1)(ByVal DefaultValue As TResult, ByVal FieldGetter1 As Func(Of LocalizedPackageParts, TResult1), ByVal FieldGetter2 As Func(Of TResult1, TResult)) As MultiLanguageValue(Of TResult)
        '    Return GetMultiLanguageField(DefaultValue, Function(Tracks)
        '                                                   Dim Result1 = FieldGetter1(Tracks)
        '                                                   If Result1 Is Nothing Then
        '                                                       Return Nothing
        '                                                   Else
        '                                                       Return FieldGetter2(Result1)
        '                                                   End If
        '                                               End Function)
        'End Function

        ''' <summary>
        ''' 添加输出信息。
        ''' </summary>
        Private Sub AddOutput(ByVal source As DataContainer, ByVal identity As CompilerOutputIdentity, ByVal message As String, ByVal ParamArray data() As Object)
            m_Output.Add(New CompilerOutput(source, identity, message, data))
        End Sub

        ''' <summary>
        ''' 添加输出信息。
        ''' </summary>
        Private Sub AddOutput(ByVal source As DataContainer, ByVal identity As CompilerOutputIdentity, ByVal ParamArray data() As Object)
            m_Output.Add(New CompilerOutput(source, identity, Nothing, data))
        End Sub

        ' ''' <summary>
        ' ''' 按最合适的顺序对一个包含特性引用的列表进行编译。
        ' ''' </summary>
        ' ''' <typeparam name="TSource">编译的源列表，其中为 <c>null</c> 的项与重复项会被排除。</typeparam>
        ' ''' <typeparam name="TDest"></typeparam>
        ' ''' <param name="source"></param>
        ' ''' <param name="compiler">用于执行对源列表进行编译的详情（不用设置 Reference 属性）。其中第一个参数是待编译的源项目，第二个参数是源项目引用的、已编译的项目。</param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Private Function CompileReferableList(Of TSource As {DataContainer, IReferable},
        '                                          TDest As {Compiled.ObjectModel.CompiledContainer, 
        '                                              Compiled.ObjectModel.IReferable(Of TDest)}) _
        '    (ByVal source As IEnumerable(Of TSource),
        '     ByVal compiler As Func(Of TSource, TDest, TDest)
        '     ) As Dictionary(Of TSource, TDest)
        '    '此处主要解决了引用套引用的问题
        '    '支持向后引用

        '    Dim src As New LinkedList(Of TSource)(From EachItem In source
        '                                          Distinct)  '此处不能用 Queue，因为移除的顺序取决于引用链
        '    Dim dest As New Dictionary(Of TSource, TDest)(src.Count)
        '    Do While src.Count > 0
        '        '取第一个元素
        '        '获取引用
        '        Dim FirstRef = src(0).Reference
        '        If FirstRef Is Nothing Then
        '            '没有引用，十分简单
        '            dest.Add(src(0), compiler(src(0), Nothing))
        '            src.RemoveFirst()
        '        Else
        '            '开始引用了……
        '            Dim NextRef = FirstRef  '处于 RefChain 顶端的对象的引用目标
        '            Dim RefChain As New Stack(Of TSource)
        '            '构造引用链
        '            RefChain.Push(src(0))
        '            Do
        '                '查找已编译的引用项
        '                For Each EachDest In dest.Values
        '                    If RefChain.Peek.Reference = EachDest.Id Then
        '                        '找到引用目标了
        '                        Exit Do
        '                    End If
        '                Next
        '                '没有找到，开始构造编译的实例
        '                '在源列表中查找
        '                For Each EachSrc In src
        '                    If EachSrc.Reference = NextRef Then
        '                        '找到了指定的 Id，需要构造
        '                        RefChain.Push(EachSrc)
        '                        NextRef = EachSrc.Reference
        '                        If NextRef Is Nothing Then
        '                            '到头了，不再引用
        '                            '将最顶层的
        '                            Exit Do
        '                        Else
        '                            '没到头，继续引用
        '                            Continue Do
        '                        End If
        '                    End If
        '                Next
        '                '找不到编译源
        '                '报错
        '                AddOutput(RefChain.Peek, CompilerOutputIdentity.ReferenceOutOfRange, NextRef)
        '                '错误处理
        '                '只能委屈一下最顶端的对象了……
        '                'Reference = Nothing
        '                Exit Do
        '            Loop
        '            '按照引用链的顺序进行编译
        '            Dim ReferedItem As TDest = Nothing
        '            Do
        '                '开始出栈
        '                Dim NextReferedItem = compiler(RefChain.Peek, ReferedItem)
        '                NextReferedItem.Reference = ReferedItem
        '                dest.Add(RefChain.Peek, NextReferedItem)
        '                src.Remove(RefChain.Pop)
        '                ReferedItem = NextReferedItem
        '            Loop Until RefChain.Count = 0
        '        End If
        '    Loop
        '    Return dest
        'End Function
#End Region

        Private Function CompileHeader() As Compiled.Header
            Dim Compiled As New Compiled.Header(CompiledDocument)
            With Package.Header
                '检查数据有效性
                If .Revision < 0 Then
                    AddOutput(Package.Header, CompilerOutputIdentity.RevisionOutOfRange, .Revision)
                End If
                Compiled.ApplicationName = .ApplicationName
                Compiled.ApplicationVersion = .ApplicationVersion
                Compiled.AuthorName = .AuthorName
                Compiled.AuthorContact = .AuthorContact
                Compiled.Language = .Language
                Compiled.Revision = .Revision
                Compiled.Comments = .Comments
            End With
            Compiled.FinishCompilation()
            Return Compiled
        End Function

        ''' <summary>
        ''' 编译音乐信息。
        ''' </summary>
        Private Function CompileMusicInfo() As Compiled.MusicInfo
            Dim Compiled As New Compiled.MusicInfo(CompiledDocument)
            With Package.MusicInfo
                Compiled.Album = GetMultiLanguageField(.Album, Function(Track) Track.MusicInfo.Album)
                Compiled.Title = GetMultiLanguageField(.Title, Function(Track) Track.MusicInfo.Title)
                Compiled.ReleaseYear = .ReleaseYear
                Compiled.Track = .Track
                Compiled.Genres = (From EachItem In .Genres Where EachItem <> Nothing).ToArray
                Dim CompiledArtists As New Dictionary(Of ArtistBase, Compiled.ArtistBase)(.Artists.Count)
                '扫描 1.0
                For Each EachArtist In .Artists
                    Dim CompiledArtist As Compiled.ArtistBase = Nothing
                    '设置 Artist/ArtistGroup 的个体属性
                    If TypeOf EachArtist Is Artist Then
                        Dim EA = DirectCast(EachArtist, Artist)
                        CompiledArtist = New Compiled.Artist(Compiled) With {.Sex = EA.Sex}
                    ElseIf TypeOf EachArtist Is ArtistGroup Then
                        Dim EA = DirectCast(EachArtist, ArtistGroup)
                        CompiledArtist = New Compiled.ArtistGroup(Compiled)
                    Else
                        Debug.Assert(False)
                    End If
                    '公共属性
                    With EachArtist
                        CompiledArtist.Id = .Id
                        CompiledArtist.Jobs = .Jobs
                        If .Id Is Nothing Then
                            CompiledArtist.Name = .Name
                            CompiledArtist.CharacterName = .CharacterName
                        Else
                            Dim LocalizedArtistGetter = Function(Part As LocalizedPackageParts) (From EachLA In Part.MusicInfo.Artists Where EachLA.SourceId = .Id).FirstOrDefault
                            CompiledArtist.Name = GetMultiLanguageField(
                                .Name, Function(Part)
                                           Dim LA = LocalizedArtistGetter(Part)
                                           Return If(LA Is Nothing, Nothing, LA.Name)
                                       End Function)
                            CompiledArtist.CharacterName = GetMultiLanguageField(
                                .CharacterName, Function(Part)
                                                    Dim LA = LocalizedArtistGetter(Part)
                                                    Return If(LA Is Nothing, Nothing, LA.CharacterName)
                                                End Function)
                        End If
                        CompiledArtists.Add(EachArtist, CompiledArtist)
                    End With
                Next
                '扫描 2.0
                Compiled.Artists = CompiledArtists.Select(
                    Function(EachArtist)
                        If TypeOf EachArtist.Key Is ArtistGroup Then
                            Dim EA = DirectCast(EachArtist.Key, ArtistGroup)
                            Dim CEA = DirectCast(EachArtist.Value, Compiled.ArtistGroup)
                            If EA.ArtistIds.Count > 0 Then
                                '作为 ArtistGroup，包含对 ArtistBase 的引用
                                CEA.Artists = EA.ArtistIds.Select(
                                    Function(EachId)
                                        '引用自身
                                        If EachId = EA.Id Then
                                            AddOutput(EachArtist.Key, CompilerOutputIdentity.SelfReference, EachId)
                                            Return Nothing
                                        End If
                                        Dim ReferenceDest = (From EachPair In CompiledArtists
                                                            Where (EachPair.Key.Id = EachId)).FirstOrDefault
                                        '引用目标不存在
                                        If ReferenceDest.Key Is Nothing Then
                                            AddOutput(EachArtist.Key, CompilerOutputIdentity.ReferenceOutOfRange, EachId)
                                            Return Nothing
                                        End If
                                        Return DirectCast(ReferenceDest.Value, Compiled.ArtistBase)
                                    End Function).ToArray
                            End If
                        End If
                        EachArtist.Value.FinishCompilation()
                        '返回每一项
                        Return DirectCast(EachArtist.Value, Compiled.ArtistBase)
                    End Function).ToArray
            End With
            Compiled.FinishCompilation()
            Return Compiled
        End Function

#Region "Lyrics"
        Private IdentifiedLines As New Dictionary(Of Integer, Compiled.Line)
        Private IdentifiedSpans As New Dictionary(Of Integer, Compiled.Span)

        ''' <summary>
        ''' 编译歌词信息。
        ''' </summary>
        Private Function CompileLyrics() As Compiled.Lyrics
            Dim Compiled As New Compiled.Lyrics(CompiledDocument)
            '初始化
            IdentifiedLines.Clear()
            IdentifiedSpans.Clear()
            With Package.Lyrics
                '优先设置可被继承的属性
                Compiled.ApplyLanguage(CompiledDocument.Header.Language)
                Compiled.ApplyLanguage(.Language)
                Compiled.ApplyVersions(From EachVersion In .Versions
                                       Select CompileVersion(EachVersion, Compiled))
            End With
            Compiled.FinishCompilation()
            Return Compiled
        End Function

        Private Function CompileVersion(ByVal source As Version, ByVal parent As Compiled.Lyrics) As Compiled.Version
            Dim Compiled As New Compiled.Version(parent)
            Debug.Assert(source IsNot Nothing)
            Debug.Assert(parent IsNot Nothing)
            '优先设置可被继承的属性
            Compiled.ApplyLanguage(parent.Language)
            '轨
            Compiled.ApplyParts(From EachTrack In source.Tracks
                                Select CompilePart(EachTrack, Compiled))
            Compiled.Duration = source.Duration
            Compiled.Deviation = If(source.Deviation, m_DefaultDeviation)
            Compiled.FinishCompilation()
            Return Compiled
        End Function

        Private Function CompilePart(ByVal source As Track, ByVal parent As Compiled.Version) As Compiled.Track
            Dim Compiled As New Compiled.Track(parent)
            '优先设置可被继承的属性
            Compiled.ApplyLanguage(source.Language)
            Compiled.Type = If(source.Type Is Nothing, PartType.Primary, source.Type.Value)
            Compiled.ApplyArtists(From EachArtist In CompiledDocument.MusicInfo.Artists
                                  Where EachArtist.Id IsNot Nothing AndAlso
                                  source.ArtistIds.Contains(EachArtist.Id.Value))
            Compiled.ApplyLines(From EachLine In source.Lines Select  CompileLine(EachLine, Compiled))
            Compiled.FinishCompilation()
            Return Compiled
        End Function

        ''' <summary>
        ''' 编译 Line。
        ''' </summary>
        Private Function CompileLine(ByVal source As Line, ByVal parent As Compiled.Track) As Compiled.Line
            Dim Compiled As Compiled.Line
            Dim Reference As Compiled.Line = Nothing
            '引用设置
            If source.Reference IsNot Nothing Then
                Try
                    Reference = IdentifiedLines(source.Reference.Value)
                Catch ex As KeyNotFoundException
                    AddOutput(source, CompilerOutputIdentity.ReferenceOutOfRange, source.Reference.Value)
                End Try
            End If
            Compiled = New Compiled.Line(parent, Reference)
            '优先设置可被继承的属性
            Compiled.Id = source.Id
            Compiled.ApplyBegin(source.Begin)
            Compiled.ApplyLanguage(source.Language)
            Compiled.ApplyLocalizedText(GetLocalizedField(
                    Function(Track)
                        Dim LL = (From EachLine In Track.Lyrics.Lines
                                  Where EachLine.SourceId = source.Id).FirstOrDefault
                        Return If(LL Is Nothing, Nothing, LL.Text)
                    End Function))
            Compiled.ApplySpans(From EachSpan In source.Spans Select CompileSpan(EachSpan, Compiled))
            Compiled.ApplyArtists(From EachArtist In CompiledDocument.MusicInfo.Artists
                                  Where EachArtist.Id IsNot Nothing AndAlso
                                  source.ArtistIds.Contains(EachArtist.Id.Value))
            '注册标识符
            If source.Id IsNot Nothing Then
                Try
                    IdentifiedLines.Add(source.Id.Value, Compiled)
                Catch ex As ArgumentException
                    AddOutput(source, CompilerOutputIdentity.IdentityConflict, source.Id)
                End Try
            End If
            Compiled.FinishCompilation()
            Return Compiled
        End Function

        ''' <summary>
        ''' 编译 Span。
        ''' </summary>
        Private Function CompileSpan(ByVal source As Span, ByVal parent As Compiled.Line) As Compiled.Span
            Dim Compiled As Compiled.Span
            Dim Reference As Compiled.Span = Nothing
            '引用设置
            If source.Reference IsNot Nothing Then
                Try
                    Reference = IdentifiedSpans(source.Reference.Value)
                Catch ex As KeyNotFoundException
                    AddOutput(source, CompilerOutputIdentity.ReferenceOutOfRange, source.Reference.Value)
                End Try
            End If
            Compiled = New Compiled.Span(parent, Reference)
            With Compiled
                '优先设置可被继承的属性
                .Id = source.Id
                If source.Begin.Ticks < 0 Then AddOutput(source, CompilerOutputIdentity.SpanBeginOutOfRange, source.Begin)
                If source.Duration.Ticks < 0 Then AddOutput(source, CompilerOutputIdentity.SpanDurationOutOfRange, source.Duration)
                .ApplyBegin(source.Begin)
                .ApplyDuration(source.Duration)
                .ApplyLanguage(source.Language)
                .ApplySegments(From EachSegment In source.Segments
                               Select CompileSegment(EachSegment, Compiled))
            End With
            '注册标识符
            If source.Id IsNot Nothing Then
                Try
                    IdentifiedSpans.Add(source.Id.Value, Compiled)
                Catch ex As ArgumentException
                    AddOutput(source, CompilerOutputIdentity.IdentityConflict, source.Id)
                End Try
            End If
            Compiled.FinishCompilation()
            Return Compiled
        End Function

        Private Function CompileSegment(ByVal source As Segment, ByVal parent As Compiled.Span) As Compiled.Segment
            Dim Compiled As New Compiled.Segment(parent, Nothing)
            With Compiled
                .ApplyLanguage(source.Language)
                .ApplyText(source.Text)
                .ApplyLatinized(source.Latinized)
                .ApplyAlphabetic(source.Alphabetic)
            End With
            Compiled.FinishCompilation()
            Return Compiled
        End Function
#End Region

        ''' <summary>
        ''' 编译指定的 LyriX 包。
        ''' </summary>
        Public Function Compile(ByVal source As LyriXPackage) As Compiled.LyricsDocument
            If source Is Nothing Then
                Throw New ArgumentNullException("source")
            Else
                '初始化
                Package = source
                '开始编译
                CompiledDocument = New Compiled.LyricsDocument
                CompiledDocument.SetHeader(CompileHeader)
                CompiledDocument.SetMusicInfo(CompileMusicInfo)
                CompiledDocument.SetLyrics(CompileLyrics)
                Return CompiledDocument
            End If
        End Function

        Public Sub New()

        End Sub
    End Class

    ''' <summary>
    ''' 用于标记编译器输出的信息。
    ''' </summary>
    Public Enum CompilerOutputIdentity
        'LEVEL1
        ''' <summary>
        ''' 未知。
        ''' </summary>
        Unknown = 0

        'Throw Exception
        ' ''' <summary>
        ' ''' 表示此信息是一个“错误”的标志。
        ' ''' </summary>
        '[Error] = &H1000

        ''' <summary>
        ''' 表示此信息是一个“警告”的标志。
        ''' </summary>
        Warning = &H2000
        ''' <summary>
        ''' 表示此信息是一个“消息”的标志。
        ''' </summary>
        Information = &H3000

        'LEVEL2
        ''' <summary>
        ''' 指定的值超出了允许取值的范围。
        ''' </summary>
        ValueOutOfRange = Warning Or &H100

        'LEVEL3

        ''' <summary>
        ''' 指定的标识符已存在。
        ''' </summary>
        IdentityConflict = ValueOutOfRange + 1

        ''' <summary>
        ''' 引用的标识符不存在。
        ''' </summary>
        ReferenceOutOfRange

        ''' <summary>
        ''' 引用的标识符指向自身。
        ''' </summary>
        SelfReference

        ''' <summary>
        ''' 段的开始时间为负数。
        ''' </summary>
        SpanBeginOutOfRange

        ''' <summary>
        ''' 段的持续时间为负数。
        ''' </summary>
        SpanDurationOutOfRange

        ''' <summary>
        ''' 同一轨中的段发生重叠。
        ''' </summary>
        SpanOverlap

        ''' <summary>
        ''' 同一轨中的行发生重叠。
        ''' </summary>
        LineOverlap

        ''' <summary>
        ''' 修订次数为负。
        ''' </summary>
        RevisionOutOfRange
    End Enum

    ''' <summary>
    ''' 表示读取器输出的信息。
    ''' </summary>
    Public NotInheritable Class CompilerOutput
        Private Shared MessageResource As New Resources.ResourceManager("LyriX.CompilerOutput", GetType(CompilerOutput).Assembly)

        Private m_Identity As CompilerOutputIdentity,
            m_Message As String,
            m_Data() As Object,
            m_s_Data As IList(Of Object),
            m_Source As DataContainer

        ''' <summary>
        ''' 获取输出信息的标识。
        ''' </summary>
        Public ReadOnly Property Identity As CompilerOutputIdentity
            Get
                Return m_Identity
            End Get
        End Property

        Public ReadOnly Property IsWarning As Boolean
            Get
                Return (m_Identity And CompilerOutputIdentity.Warning) = CompilerOutputIdentity.Warning
            End Get
        End Property

        Public ReadOnly Property IsInformation As Boolean
            Get
                Return (m_Identity And CompilerOutputIdentity.Information) = CompilerOutputIdentity.Information
            End Get
        End Property

        ''' <summary>
        ''' 获取输出信息的内容。
        ''' </summary>
        Public ReadOnly Property Message As String
            Get
                Return m_Message
            End Get
        End Property

        ''' <summary>
        ''' 获取读取器的附加信息，这些信息可能在 <see cref="Message" /> 中得以表现。
        ''' </summary>
        Public ReadOnly Property Data As IList(Of Object)
            Get
                Return m_s_Data
            End Get
        End Property

        ''' <summary>
        ''' 获取产生此信息的读取器。
        ''' </summary>
        Public ReadOnly Property Source As DataContainer
            Get
                Return m_Source
            End Get
        End Property

        Private Shared Function GetIdentityMessage(ByVal identity As CompilerOutputIdentity, ByVal data() As Object) As String
            Dim Msg = MessageResource.GetString([Enum].GetName(GetType(CompilerOutputIdentity), identity))
            If Msg Is Nothing Then
                Return Nothing
            Else
                Return String.Format(Msg, data)
            End If
        End Function

        ''' <summary>
        ''' 返回信息文本及产生信息的对象。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format(Prompts.ReaderOutput, m_Message, Locale.GetFriendlyName(m_Source), m_Source)
        End Function

        Friend Sub New(ByVal source As DataContainer, ByVal identity As CompilerOutputIdentity, ByVal message As String, ByVal data() As Object)
            If source Is Nothing Then
                Throw New ArgumentNullException("source")
            Else
                m_Source = source
                m_Identity = identity
                m_Data = If(data, {})
                m_Message = If(message, GetIdentityMessage(m_Identity, m_Data))
                m_s_Data = Array.AsReadOnly(m_Data)
            End If
        End Sub

        Friend Sub New(ByVal source As DataContainer, ByVal identity As CompilerOutputIdentity, ByVal data() As Object)
            Me.New(source, identity, Nothing, data)
        End Sub
    End Class
End Namespace
