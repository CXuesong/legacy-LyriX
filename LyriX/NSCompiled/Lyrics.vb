Imports LyriX.Compiled.ObjectModel
Imports System.Collections.ObjectModel

Namespace Compiled
    ''' <summary>
    ''' 表示一个经过编译的歌词信息。
    ''' </summary>
    Public NotInheritable Class Lyrics
        Inherits CompiledDocumentPart
        '本地缓存
        Private m_Language As String,
            m_Versions As IList(Of Version) = {}

        ''' <summary>
        ''' 此元素中歌词所使用的默认语言。
        ''' </summary>
        Public ReadOnly Property Language As String
            Get
                Return m_Language
            End Get
        End Property

        ''' <summary>
        ''' 歌词的版本列表。
        ''' </summary>
        Public ReadOnly Property Versions As IList(Of Version)
            Get
                Return m_Versions
            End Get
        End Property

        Friend Sub ApplyLanguage(value As String)
            If value IsNot Nothing Then m_Language = value
        End Sub

        Friend Sub ApplyVersions(value As IEnumerable(Of Version))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing Then
                '追加列表 & 定型
                m_Versions = m_Versions.Concat(value).ToArray
            End If
        End Sub

        Protected Overrides Sub OnCompilationFinished()
            MyBase.OnCompilationFinished()
            '统计
            '密封
            m_Versions = New ReadOnlyCollection(Of Version)(m_Versions)
        End Sub

        ''' <summary>
        ''' 根据指定的歌曲时长，从 <see cref="Versions" /> 中匹配一个最佳的 <see cref="Version" />。
        ''' </summary>
        ''' <param name="musicDuration">提供歌曲的持续时间（毫秒）</param>
        ''' <returns>与指定信息最匹配的 <see cref="Version" />。如果匹配失败，或是 <see cref="Versions" /> 为空，则返回 <c>null</c>。</returns>
        Public Function MatchVersion(musicDuration As TimeSpan) As Version
            Dim BestVersion As Version = Nothing, BestMatch As Double = 1
            Dim NullVersion As Version = Nothing
            For Each EachVersion In m_Versions
                If EachVersion.Duration Is Nothing OrElse EachVersion.Deviation Is Nothing Then
                    NullVersion = EachVersion
                Else
                    Dim NewMatch = EachVersion.MatchMusicDuration(musicDuration)
                    If NewMatch < BestMatch Then
                        BestMatch = NewMatch
                        BestVersion = EachVersion
                    End If
                End If
            Next
            Return If(BestVersion, NullVersion)
        End Function

        Friend Sub New(document As LyricsDocument)
            MyBase.New(document)
            '继承属性值
            Debug.Assert(document.MusicInfo IsNot Nothing)
            m_Language = document.Header.Language
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个经过编译的歌词版本信息。
    ''' </summary>
    Public NotInheritable Class Version
        Inherits CompiledElement
        '本地缓存
        Private m_Language As String,
            m_Duration As TimeSpan?,
            m_Deviation As TimeSpan?,
            m_Tracks As IList(Of Track) = {},
            m_LyricsBegin As TimeSpan?,
            m_LyricsDuration As TimeSpan?

        ''' <summary>
        ''' 此元素中歌词所使用的默认语言。
        ''' </summary>
        Public ReadOnly Property Language As String
            Get
                Return m_Language
            End Get
        End Property

        ''' <summary>
        ''' （可选）歌曲的持续时间。
        ''' </summary>
        Public Property Duration As TimeSpan?
            Get
                Return m_Duration
            End Get
            Friend Set(value As TimeSpan?)
                m_Duration = value
            End Set
        End Property

        ''' <summary>
        ''' （可选）在匹配歌曲的持续时间时允许的最大误差时间。
        ''' </summary>
        Public Property Deviation As TimeSpan?
            Get
                Return m_Deviation
            End Get
            Friend Set(value As TimeSpan?)
                m_Deviation = value
            End Set
        End Property

        ''' <summary>
        ''' 歌词中的轨。
        ''' </summary>
        Public ReadOnly Property Tracks As IList(Of Track)
            Get
                Return m_Tracks
            End Get
        End Property

        ''' <summary>
        ''' 歌词中第一行开始的时间。
        ''' </summary>
        Public ReadOnly Property LyricsBegin As TimeSpan
            Get
                Return m_LyricsBegin.GetValueOrDefault
            End Get
        End Property

        ''' <summary>
        ''' 歌词中从第一行开始到最后一行结束持续的时间。
        ''' </summary>
        Public ReadOnly Property LyricsDuration As TimeSpan
            Get
                Return m_LyricsDuration.GetValueOrDefault
            End Get
        End Property

        ''' <summary>
        ''' 歌词所有行结束的时间。
        ''' </summary>
        Public ReadOnly Property LyricsEnd As TimeSpan
            Get
                Return (m_LyricsBegin + m_LyricsDuration).GetValueOrDefault
            End Get
        End Property

        Friend Sub ApplyLanguage(value As String)
            If value IsNot Nothing Then m_Language = value
        End Sub

        Friend Sub ApplyParts(value As IEnumerable(Of Track))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing Then
                '追加列表 & 定型
                m_Tracks = m_Tracks.Concat(value).ToArray
            End If
        End Sub

        Protected Overrides Sub OnCompilationFinished()
            MyBase.OnCompilationFinished()
            '统计
            If m_Tracks.Count > 0 Then
                '持续时间的默认值是歌词的最晚时间
                Dim NewEnd As TimeSpan = m_Tracks.First.End
                If m_LyricsBegin Is Nothing Then m_LyricsBegin = m_Tracks.First.Begin
                For Each EachTrack In m_Tracks
                    If m_LyricsBegin.Value > EachTrack.Begin Then m_LyricsBegin = EachTrack.Begin
                    If NewEnd < EachTrack.End Then NewEnd = EachTrack.End
                Next
                m_LyricsDuration = NewEnd - m_LyricsBegin
            End If
            '密封
            m_Tracks = New ReadOnlyCollection(Of Track)(m_Tracks)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Join(vbCrLf, DirectCast(m_Tracks, IEnumerable(Of Track)))
        End Function

        ''' <summary>
        ''' 根据已有的信息与提供的歌曲长度判断歌曲与此版本歌词的匹配程度。
        ''' </summary>
        ''' <param name="musicDuration">提供歌曲的持续时间（毫秒）</param>
        ''' <returns>给定的持续时间与 <see cref="Duration" /> 的差值除以 <see cref="Deviation" /> 的比值，其中 0 表示完全匹配，1 表示恰好达到 <see cref="Deviation" /> 的最高误差要求。</returns>
        ''' <exception cref="InvalidOperationException"><see cref="Duration" /> 或 <see cref="Deviation" /> 为 <c>null</c>。</exception>
        Public Function MatchMusicDuration(ByVal musicDuration As TimeSpan) As Double
            If m_Deviation Is Nothing Then
                Throw New InvalidOperationException
            Else
                Return (musicDuration - m_Duration.Value).TotalMilliseconds / m_Deviation.Value.TotalMilliseconds
            End If
        End Function

        Public Sub New(parent As Lyrics)
            MyBase.New(parent)
            '继承属性值
            m_Language = parent.Language
        End Sub
    End Class

    ''' <summary>
    ''' 一个经过编译的歌词的轨的信息。
    ''' </summary>
    Public NotInheritable Class Track
        Inherits CompiledElement

        '本地缓存
        Private m_Artists As IList(Of ArtistBase) = {},
            m_Type As PartType?,
            m_Language As String,
            m_Lines As Line() = {},
            m_s_Lines As IList(Of Line),
            m_Begin As TimeSpan?,
            m_Duration As TimeSpan?
        'm_Reference As Track

        ''' <summary>
        ''' 此轨的默认演唱者列表。
        ''' </summary>
        Public ReadOnly Property Artists As IList(Of ArtistBase)
            Get
                Return m_Artists
            End Get
        End Property

        ''' <summary>
        ''' 此轨的类型。
        ''' </summary>
        Public Property [Type] As PartType
            Get
                Return m_Type.GetValueOrDefault
            End Get
            Friend Set(ByVal value As PartType)
                m_Type = value
            End Set
        End Property

        ''' <summary>
        ''' 此轨中歌词所使用的语言。
        ''' </summary>
        Public ReadOnly Property Language As String
            Get
                Return m_Language
            End Get
        End Property

        ''' <summary>
        ''' 轨中的歌词行。
        ''' </summary>
        Public ReadOnly Property Lines As IList(Of Line)
            Get
                Return m_s_Lines
            End Get
        End Property

        ''' <summary>
        ''' 轨第一行开始的时间。
        ''' </summary>
        Public ReadOnly Property Begin As TimeSpan
            Get
                Return m_Begin.GetValueOrDefault
            End Get
        End Property

        ''' <summary>
        ''' 轨从第一行开始到最后一行结束持续的时间。
        ''' </summary>
        Public ReadOnly Property Duration As TimeSpan
            Get
                Return m_Duration.GetValueOrDefault
            End Get
        End Property

        ''' <summary>
        ''' 轨所有行结束的时间。
        ''' </summary>
        Public ReadOnly Property [End] As TimeSpan
            Get
                Return (m_Begin + m_Duration).GetValueOrDefault
            End Get
        End Property

        Friend Sub ApplyLanguage(value As String)
            If value IsNot Nothing Then m_Language = value
        End Sub

        Friend Sub ApplyArtists(value As IEnumerable(Of ArtistBase))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing Then
                '追加列表 & 定型
                m_Artists = m_Artists.Concat(value).ToArray
            End If
        End Sub

        Friend Sub ApplyLines(value As IEnumerable(Of Line))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing AndAlso value.Any Then
                '追加列表 & 定型
                m_Lines = m_Lines.Concat(value).ToArray
            End If
        End Sub

        Protected Overrides Sub OnCompilationFinished()
            MyBase.OnCompilationFinished()
            If m_Lines.Any Then
                '统计
                Dim NewEnd As TimeSpan = m_Lines.First.End
                If m_Begin Is Nothing Then m_Begin = m_Lines.First.Begin
                For Each EachLine In m_Lines
                    If m_Begin.Value > EachLine.Begin Then m_Begin = EachLine.Begin
                    If NewEnd < EachLine.End Then NewEnd = EachLine.End
                Next
                m_Duration = NewEnd - m_Begin
            Else
                m_Begin = Nothing
                m_Duration = Nothing
            End If
            '排序密封
            Array.Sort(m_Lines, Function(a, b) TimeSpan.Compare(a.Begin, b.Begin))
            m_Artists = New ReadOnlyCollection(Of ArtistBase)(m_Artists)
            m_s_Lines = New ReadOnlyCollection(Of Line)(m_Lines)
        End Sub

        ' ''' <summary>
        ' ''' 获取此轨的引用目标。
        ' ''' </summary>
        'Public Property Reference As Track
        '    Get
        '        Return m_Reference
        '    End Get
        '    Friend Set(ByVal value As Track)
        '        m_Reference = value
        '    End Set
        'End Property

        ''' <summary>
        ''' 返回此轨的所有歌词行的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return "Track:" & JoinList(m_Artists) & vbCrLf & String.Join(vbCrLf, DirectCast(m_Lines, IEnumerable(Of Line)))
        End Function

        Friend Sub New(parent As Version)
            MyBase.New(parent)
            '继承属性值
            m_Language = parent.Language
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个已编译的歌词行的信息。
    ''' </summary>
    Public NotInheritable Class Line
        Inherits CompiledElement
        Implements IReferable(Of Line)

        '本地缓存
        Private m_Id As Integer?,
            m_Language As String,
            m_Spans As Span() = {},
            m_s_Spans As IList(Of Span),
            m_LocalizedText As MultiLanguageValue(Of String) = MultiLanguageValue(Of String).Empty,
            m_Begin As TimeSpan?,
            m_Duration As TimeSpan,
            m_Artists As IList(Of ArtistBase) = {},
            m_LatinizedExist As Boolean,
            m_AlphabeticExist As Boolean,
            m_Reference As Line
        Dim ReferenceSpans As Span() = {}

        ''' <summary>
        ''' 此行的标识。
        ''' </summary>
        Public Property Id As Integer? Implements IIdentifiable.Id
            Get
                Return m_Id
            End Get
            Friend Set(ByVal value As Integer?)
                m_Id = value
            End Set
        End Property

        ''' <summary>
        ''' 此行中歌词所使用的语言。
        ''' </summary>
        Public ReadOnly Property Language As String
            Get
                Return m_Language
            End Get
        End Property

        ''' <summary>
        ''' 此行的段列表。
        ''' </summary>
        Public ReadOnly Property Spans As IList(Of Span)
            Get
                Return m_s_Spans
            End Get
        End Property

        ''' <summary>
        ''' （本地化）此行的歌词内容。
        ''' </summary>
        Public ReadOnly Property LocalizedText As MultiLanguageValue(Of String)
            Get
                Return If(m_LocalizedText, MultiLanguageValue(Of String).Empty)
            End Get
        End Property

        ''' <summary>
        ''' 行开始的时间。
        ''' </summary>
        Public ReadOnly Property Begin As TimeSpan
            Get
                Return m_Begin.Value
            End Get
        End Property

        ''' <summary>
        ''' 行持续的时间。
        ''' </summary>
        Public ReadOnly Property Duration As TimeSpan
            Get
                Return m_Duration
            End Get
        End Property

        ''' <summary>
        ''' 行结束的时间。
        ''' </summary>
        Public ReadOnly Property [End] As TimeSpan
            Get
                Return (m_Begin + m_Duration).GetValueOrDefault
            End Get
        End Property

        ''' <summary>
        ''' 此行的演唱者列表。
        ''' </summary>
        Public ReadOnly Property Artists As IList(Of ArtistBase)
            Get
                Return m_Artists
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示此行的段是否存在拉丁化表示形式。
        ''' </summary>
        Public ReadOnly Property LatinizedExist As Boolean
            Get
                Return m_LatinizedExist
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示此行的段是否存在本地化拼音表示形式。
        ''' </summary>
        Public ReadOnly Property AlphabeticExist As Boolean
            Get
                Return m_AlphabeticExist
            End Get
        End Property

        Friend Sub ApplyBegin(value As TimeSpan?)
            If value IsNot Nothing Then m_Begin = value.Value
        End Sub

        Friend Sub ApplyLanguage(value As String)
            If value IsNot Nothing Then m_Language = value
        End Sub

        Friend Sub ApplyLocalizedText(value As MultiLanguageValue(Of String))
            If value IsNot Nothing Then m_LocalizedText = value
        End Sub

        Friend Sub ApplySpans(value As IEnumerable(Of Span))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing Then
                '追加列表 & 定型
                m_Spans = m_Spans.Concat(value).ToArray
            End If
        End Sub

        Friend Sub ApplyArtists(value As IEnumerable(Of ArtistBase))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing Then
                '追加列表 & 定型
                m_Artists = m_Artists.Concat(value).ToArray
            End If
        End Sub

        Protected Overrides Sub OnCompilationFinished()
            MyBase.OnCompilationFinished()
            If m_Spans.Any Then
                '确定引用的时间问题
                If ReferenceSpans.Any AndAlso m_Begin IsNot Nothing Then
                    'Eg.
                    'ref = [00:00:05]那些因为年轻犯的错
                    'this = [00:00:10]那些因为年轻犯的错
                    Dim TimeOffset = m_Begin.Value - m_Reference.m_Begin.Value
                    For Each EachSpan In ReferenceSpans
                        EachSpan.OffsetBegin(TimeOffset)
                        EachSpan.FinishCompilation()
                    Next
                End If
                '统计
                m_LatinizedExist = False
                m_AlphabeticExist = False
                Dim NewEnd As TimeSpan = m_Spans.First.End
                If m_Begin Is Nothing Then m_Begin = m_Spans.First.Begin
                For Each EachSpan In m_Spans
                    If EachSpan.LatinizedExist Then m_LatinizedExist = True
                    If EachSpan.AlphabeticExist Then m_AlphabeticExist = True
                    If m_Begin.Value > EachSpan.Begin Then m_Begin = EachSpan.Begin
                    If NewEnd < EachSpan.End Then NewEnd = EachSpan.End
                Next
                m_Duration = NewEnd - m_Begin.Value
            Else
                m_Begin = Nothing
                m_Duration = Nothing
            End If
            '排序密封
            Array.Sort(m_Spans, Function(a, b) TimeSpan.Compare(a.Begin, b.Begin))
            m_s_Spans = New ReadOnlyCollection(Of Span)(m_Spans)
            m_Artists = New ReadOnlyCollection(Of ArtistBase)(m_Artists)
        End Sub

        ''' <summary>
        ''' 获取此行的引用目标。
        ''' </summary>
        Public ReadOnly Property Reference As Line Implements IReferable(Of LyriX.Compiled.Line).Reference
            Get
                Return m_Reference
            End Get
        End Property

        ''' <summary>
        ''' 获取此行中所有语义段中指定类型的文本，并连接起来。
        ''' </summary>
        ''' <param name="alternativeItems">指示要获取文本的类型，如果排在前面的文本为 <c>null</c>，或为 <see cref="String.Empty" />，则会尝试使用下一项所指定的文本类型。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="alternativeItems" /> 中至少有一项不是有效的 <see cref="SegmentTextItem" /> 值。</exception>
        Public Function GetText(ParamArray alternativeItems() As SegmentTextItem) As String
            Dim builder As New Text.StringBuilder
            For Each EachSegment In m_Spans
                builder.Append(EachSegment.GetText(alternativeItems))
            Next
            Return builder.ToString
        End Function

        ''' <summary>
        ''' 获取此行中所有语义段中指定类型的文本，并连接起来。
        ''' </summary>
        ''' <param name="item">指示要获取文本的类型。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="item" /> 不是有效的 <see cref="SegmentTextItem" /> 值。</exception>
        Public Function GetText(item As SegmentTextItem) As String
            Return GetText({item})
        End Function

        ''' <summary>
        ''' 返回此歌词行的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return If(m_Spans Is Nothing, "", String.Join("", DirectCast(m_Spans, IEnumerable(Of Span))))
        End Function

        Friend Sub New(parent As Track, reference As Line)
            MyBase.New(parent)
            m_Reference = reference
            If reference Is Nothing Then
                '继承属性值
                m_Language = parent.Language
                m_Artists = parent.Artists
            Else
                '引用属性值
                With reference
                    m_Language = .Language
                    m_LocalizedText = .m_LocalizedText
                    m_LatinizedExist = .m_LatinizedExist
                    m_AlphabeticExist = .m_AlphabeticExist
                    ReferenceSpans = (From EachSpan In .m_Spans
                              Select New Span(Me, EachSpan)).ToArray
                    m_Spans = ReferenceSpans
                    m_Artists = .m_Artists
                End With
            End If
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个已编译的歌词段。
    ''' </summary>
    Public NotInheritable Class Span
        Inherits CompiledElement
        Implements IReferable(Of Span)

        '本地缓存
        Private m_Id As Integer?,
            m_Begin As TimeSpan,
            m_Duration As TimeSpan,
            m_Language As String,
            m_Segments As Segment() = {},
            m_s_Segments As IList(Of Segment),
            m_LatinizedExist As Boolean,
            m_AlphabeticExist As Boolean,
            m_Length As Integer,
            m_Reference As Span
        Dim ReferenceSegments As Segment() = {}

        ''' <summary>
        ''' 此段的标识。
        ''' </summary>
        Public Property Id As Integer? Implements IIdentifiable.Id
            Get
                Return m_Id
            End Get
            Friend Set(ByVal value As Integer?)
                m_Id = value
            End Set
        End Property

        ''' <summary>
        ''' 段开始的时间。
        ''' </summary>
        Public ReadOnly Property Begin As TimeSpan
            Get
                Return m_Begin
            End Get
        End Property

        ''' <summary>
        ''' 段持续的时间。
        ''' </summary>
        Public ReadOnly Property Duration As TimeSpan
            Get
                Return m_Duration
            End Get
        End Property

        ''' <summary>
        ''' 段结束的时间。
        ''' </summary>
        Public ReadOnly Property [End] As TimeSpan
            Get
                Return m_Begin + m_Duration
            End Get
        End Property

        ''' <summary>
        ''' 此段中歌词所使用的语言。
        ''' </summary>
        Public ReadOnly Property Language As String
            Get
                Return m_Language
            End Get
        End Property

        ''' <summary>
        ''' 获取歌词段中的语义段。
        ''' </summary>
        Public ReadOnly Property Segments As IList(Of Segment)
            Get
                Return m_s_Segments
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示此段是否存在拉丁化表示形式。
        ''' </summary>
        Public ReadOnly Property LatinizedExist As Boolean
            Get
                Return m_LatinizedExist
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示此段是否存在本地化拼音表示形式。
        ''' </summary>
        Public ReadOnly Property AlphabeticExist As Boolean
            Get
                Return m_AlphabeticExist
            End Get
        End Property

        ''' <summary>
        ''' 获取此段的长度。
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                Return m_Length
            End Get
        End Property

        Friend Sub ApplyBegin(value As TimeSpan?)
            If value IsNot Nothing Then m_Begin = value.Value
        End Sub

        Friend Sub OffsetBegin(value As TimeSpan?)
            If value IsNot Nothing Then m_Begin += value.Value
        End Sub

        Friend Sub ApplyDuration(value As TimeSpan?)
            If value IsNot Nothing Then m_Duration = value.Value
        End Sub

        Friend Sub ApplyLanguage(value As String)
            If value IsNot Nothing Then m_Language = value
        End Sub

        Friend Sub ApplySegments(value As IEnumerable(Of Segment))
            '注意：value 只能被计算一次，否则可能会造成副作用
            If value IsNot Nothing Then
                '追加列表 & 定型
                m_Segments = m_Segments.Concat(value).ToArray
            End If
        End Sub

        Protected Overrides Sub OnCompilationFinished()
            MyBase.OnCompilationFinished()
            '确定语义段的时间问题
            'Segment 的 Begin、Duration 按照 Span 的时间长度按 Segment 的 Text.Length 加权插值
            '统计
            m_Length = 0
            m_LatinizedExist = False
            m_AlphabeticExist = False
            For Each EachSegment In m_Segments
                If EachSegment.Latinized <> Nothing Then m_LatinizedExist = True
                If EachSegment.Alphabetic <> Nothing Then m_AlphabeticExist = True
                m_Length += EachSegment.Length
            Next
            Dim CurrentPosition As TimeSpan = m_Begin
            For Each EachSegment In m_Segments
                'CurrentDuration 较实际值小一些
                Dim CurrentDuration = TimeSpan.FromTicks(m_Duration.Ticks * EachSegment.Length \ m_Length)
                EachSegment.ApplyBegin(CurrentPosition)
                EachSegment.ApplyDuration(CurrentDuration)
                CurrentPosition += CurrentDuration
            Next
            With m_Segments.Last
                '校准
                .ApplyDuration(.Duration + (Me.End - CurrentPosition))
                'Debug.Assert(.Duration.Ticks > 0)
                '若此段的持续时间为 0，则断言必失败
            End With
            For Each EachSegment In ReferenceSegments
                EachSegment.FinishCompilation()
            Next
            '密封
            m_s_Segments = New ReadOnlyCollection(Of Segment)(m_Segments)
        End Sub

        ''' <summary>
        ''' 获取此段的引用目标。
        ''' </summary>
        Public ReadOnly Property Reference As Span Implements IReferable(Of LyriX.Compiled.Span).Reference
            Get
                Return m_Reference
            End Get
        End Property

        ''' <summary>
        ''' 获取此段中所有语义段中指定类型的文本，并连接起来。
        ''' </summary>
        ''' <param name="alternativeItems">指示要获取文本的类型，如果排在前面的文本为 <c>null</c>，或为 <see cref="String.Empty" />，则会尝试使用下一项所指定的文本类型。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="alternativeItems" /> 中至少有一项不是有效的 <see cref="SegmentTextItem" /> 值。</exception>
        Public Function GetText(ParamArray alternativeItems() As SegmentTextItem) As String
            Dim builder As New Text.StringBuilder
            For Each EachSegment In m_Segments
                builder.Append(EachSegment.GetText(alternativeItems))
            Next
            Return builder.ToString
        End Function

        ''' <summary>
        ''' 获取此段中所有语义段中指定类型的文本，并连接起来。
        ''' </summary>
        ''' <param name="item">指示要获取文本的类型。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="item" /> 不是有效的 <see cref="SegmentTextItem" /> 值。</exception>
        Public Function GetText(item As SegmentTextItem) As String
            Return GetText({item})
        End Function

        ''' <summary>
        ''' 获取此段的所有文本。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return If(m_Segments Is Nothing, "", String.Join("", DirectCast(m_Segments, IEnumerable(Of Segment))))
        End Function

        Friend Sub New(parent As Line, reference As Span)
            MyBase.New(parent)
            m_Reference = reference
            If reference Is Nothing Then
                '继承属性值
                m_Language = parent.Language
            Else
                '引用属性值
                With reference
                    m_Language = .m_Language
                    m_Begin = .m_Begin
                    m_Duration = .m_Duration
                    m_LatinizedExist = .m_LatinizedExist
                    m_AlphabeticExist = .m_AlphabeticExist
                    ReferenceSegments = (From EachSegment In .m_Segments
                                         Select New Segment(Me, EachSegment)).ToArray
                    m_Segments = ReferenceSegments
                End With
            End If
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个已编译的语义段。
    ''' </summary>
    ''' <remarks>目前按拼音的标注需要进行人为划分，是歌词的最小部分。</remarks>
    Public NotInheritable Class Segment
        Inherits CompiledElement

        '本地缓存
        Private m_Text As String,
            m_Latinized As String,
            m_Alphabetic As String,
            m_Language As String,
            m_Begin As TimeSpan,
            m_Duration As TimeSpan

        ''' <summary>
        ''' 此段中歌词所使用的语言。
        ''' </summary>
        Public ReadOnly Property Language As String
            Get
                Return m_Language
            End Get
        End Property

        ''' <summary>
        ''' 段开始的时间。
        ''' </summary>
        Public ReadOnly Property Begin As TimeSpan
            Get
                Return m_Begin
            End Get
        End Property

        ''' <summary>
        ''' 段持续的时间。
        ''' </summary>
        Public ReadOnly Property Duration As TimeSpan
            Get
                Return m_Duration
            End Get
        End Property

        ''' <summary>
        ''' 段结束的时间。
        ''' </summary>
        Public ReadOnly Property [End] As TimeSpan
            Get
                Return m_Begin + m_Duration
            End Get
        End Property

        ''' <summary>
        ''' 获取语义段的内容。
        ''' </summary>
        Public ReadOnly Property Text As String
            Get
                Return m_Text
            End Get
        End Property

        ''' <summary>
        ''' 获取小段的拉丁化表现形式。
        ''' </summary>
        Public ReadOnly Property Latinized As String
            Get
                Return m_Latinized
            End Get
        End Property

        ''' <summary>
        ''' 获取小段的本地拼音表现形式。
        ''' </summary>
        Public ReadOnly Property Alphabetic As String
            Get
                Return m_Alphabetic
            End Get
        End Property

        ''' <summary>
        ''' 获取此语义段的长度。
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                'FUTURE IMPROVE
                Return Len(m_Text)
            End Get
        End Property

        Friend Sub ApplyBegin(value As TimeSpan?)
            If value IsNot Nothing Then m_Begin = value.Value
        End Sub

        Friend Sub ApplyDuration(value As TimeSpan?)
            If value IsNot Nothing Then m_Duration = value.Value
        End Sub

        Friend Sub ApplyLanguage(value As String)
            If value IsNot Nothing Then m_Language = value
        End Sub

        Friend Sub ApplyLatinized(value As String)
            If value IsNot Nothing Then m_Latinized = value
        End Sub

        Friend Sub ApplyAlphabetic(value As String)
            If value IsNot Nothing Then m_Alphabetic = value
        End Sub

        Friend Sub ApplyText(value As String)
            If value IsNot Nothing Then m_Text = value
        End Sub

        ''' <summary>
        ''' 获取此语义段中指定类型的文本。
        ''' </summary>
        ''' <param name="alternativeItems">指示要获取文本的类型，如果排在前面的文本为 <c>null</c>，或为 <see cref="String.Empty" />，则会尝试使用下一项所指定的文本类型。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="alternativeItems" /> 中至少有一项不是有效的 <see cref="SegmentTextItem" /> 值。</exception>
        Public Function GetText(ParamArray alternativeItems() As SegmentTextItem) As String
            If alternativeItems IsNot Nothing Then
                For Each EachItem In alternativeItems
                    Dim EachText = GetText(EachItem)
                    If EachText <> Nothing Then
                        Return EachText
                    End If
                Next
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' 获取此语义段中指定类型的文本。
        ''' </summary>
        ''' <param name="item">指示要获取文本的类型。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="item" /> 不是有效的 <see cref="SegmentTextItem" /> 值。</exception>
        Public Function GetText(item As SegmentTextItem) As String
            Select Case item
                Case SegmentTextItem.Text
                    Return m_Text
                Case SegmentTextItem.Latinized
                    Return m_Latinized
                Case SegmentTextItem.Alphabetic
                    Return m_Alphabetic
                Case Else
                    Throw New ComponentModel.InvalidEnumArgumentException("item", item, GetType(SegmentTextItem))
            End Select
        End Function

        ''' <summary>
        ''' 获取此段的所有文本与拼音。
        ''' </summary>
        Public Overrides Function ToString() As String
            If m_Latinized <> Nothing AndAlso m_Alphabetic <> Nothing Then
                Return String.Format(Prompts.SegmentAL, m_Text, m_Alphabetic, m_Latinized)
            ElseIf m_Alphabetic <> Nothing Then
                Return String.Format(Prompts.SegmentA, m_Text, m_Alphabetic)
            ElseIf m_Latinized <> Nothing Then
                Return String.Format(Prompts.SegmentL, m_Text, m_Latinized)
            Else
                Return m_Text
            End If
        End Function

        Friend Sub New(parent As Span, reference As Segment)
            MyBase.New(parent)
            If reference Is Nothing Then
                '继承属性值
                m_Language = parent.Language
            Else
                '引用属性值
                With reference
                    m_Language = .m_Language
                    m_Alphabetic = .m_Alphabetic
                    m_Latinized = .m_Latinized
                    m_Begin = .m_Begin
                    m_Duration = .m_Duration
                    m_Text = .m_Text
                End With
            End If
        End Sub
    End Class

    ''' <summary>
    ''' 表示 <see cref="Segment" /> 中的文本内容的类型，包括原文、拉丁化以及本地化拼音。
    ''' </summary>
    Public Enum SegmentTextItem
        ''' <summary>
        ''' 表示原文，即使用 <see cref="Segment.Text" />。
        ''' </summary>
        Text = 0
        ''' <summary>
        ''' 表示拉丁化，即使用 <see cref="Segment.Latinized" />。
        ''' </summary>
        Latinized
        ''' <summary>
        ''' 表示本地化拼音，即使用 <see cref="Segment.Alphabetic" />。
        ''' </summary>
        Alphabetic
    End Enum
End Namespace