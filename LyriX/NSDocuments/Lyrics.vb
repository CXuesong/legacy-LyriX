Imports <xmlns="LyriX/2011/package/lyrics.xsd">
Imports LyriX.Document.ObjectModel
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Document
    ''' <summary>
    ''' 表示 LyriX 包所包含的歌词信息。（LyriX/lyrics.xml）
    ''' </summary>
    Public NotInheritable Class Lyrics
        Inherits XPackagePartContainer

        '元素名称（XName）
        Friend Shared ReadOnly XNVersion As XName = GetXmlNamespace().GetName("version"),
            XNTrack As XName = GetXmlNamespace().GetName("track"),
            XNLineGroup As XName = GetXmlNamespace().GetName("lineGroup"),
            XNLine As XName = GetXmlNamespace().GetName("line"),
            XNSpan As XName = GetXmlNamespace().GetName("span"),
            XNSegment As XName = GetXmlNamespace().GetName("segment")
        '属性名称（XName）（AttributeFromDefault = unqualified，-> namespace = null）
        Friend Shared ReadOnly XNId As XName = "id",
            XNLanguage As XName = "language",
            XNRef As XName = "ref",
            XNBegin As XName = "begin",
            XNDuration As XName = "duration",
            XNDeviation As XName = "deviation",
            XNAIds As XName = "aids",
            XNType As XName = "type",
            XNLatinized As XName = "latinized",
            XNAlphabetic As XName = "alphabetic"

        Private m_Language As String,
            m_Versions As DataContainerCollection(Of Version)
        Private m_LineIndexManager As New IndexManager(Me, GetType(LineBase)),
            m_SpanIndexManager As New IndexManager(Me, GetType(Span))

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return GetXmlNamespace().GetName("lyrics")
            End Get
        End Property

        ''' <summary>
        ''' 此元素中歌词所使用的默认语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 歌词的版本列表。
        ''' </summary>
        Public ReadOnly Property Versions As DataContainerCollection(Of Version)
            Get
                Return m_Versions
            End Get
        End Property

        ''' <summary>
        ''' 获取行索引的管理器。
        ''' </summary>
        Public ReadOnly Property LineIndexManager As IndexManager
            Get
                Return m_LineIndexManager
            End Get
        End Property

        ''' <summary>
        ''' 获取段索引的管理器。
        ''' </summary>
        Public ReadOnly Property SpanIndexManager As IndexManager
            Get
                Return m_SpanIndexManager
            End Get
        End Property

        Public Overrides Function ToXDocument() As System.Xml.Linq.XDocument
            Dim doc = MyBase.ToXDocument()
            With doc.Root
                .SetAttributeValue(XNLanguage, m_Language)
                .Add(From EachVersion In m_Versions Where EachVersion IsNot Nothing
                     Select EachVersion.ToXElement)
            End With
            Return doc
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XDocument)
            MyBase.New(dataSource)
            Dim tempVersions = Enumerable.Empty(Of Version)()
            If dataSource IsNot Nothing Then
                Dim body = dataSource.Root
                If body IsNot Nothing Then
                    With body
                        m_Language = CStr(.Attribute(XNLanguage))
                        tempVersions = (From EachVersion In .Elements(XNVersion)
                                        Select New Version(EachVersion))
                    End With
                End If
            End If
            m_Versions = New DataContainerCollection(Of Version)(tempVersions)
            ObserveCollection(m_Versions, True)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示歌词的一个版本。
    ''' </summary>
    ''' <remarks>同一首歌可能存在不同的版本，如长版与短版。此处暂使用不同的持续时间来区分同一首歌的不同的版本。</remarks>
    Public Class Version
        Inherits XElementContainer

        Private m_Language As String,
            m_Duration As TimeSpan?,
            m_Deviation As TimeSpan?,
            m_Tracks As DataContainerCollection(Of Track)

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return Lyrics.XNVersion
            End Get
        End Property

        ''' <summary>
        ''' 歌曲的持续时间。
        ''' </summary>
        Public Property Duration As TimeSpan?
            Get
                Return m_Duration
            End Get
            Set(ByVal value As TimeSpan?)
                m_Duration = value
                OnContainerDataChanged("Duration")
            End Set
        End Property

        ''' <summary>
        ''' 在匹配歌曲的持续时间时允许的最大误差。
        ''' </summary>
        Public Property Deviation As TimeSpan?
            Get
                Return m_Deviation
            End Get
            Set(ByVal value As TimeSpan?)
                m_Deviation = value
                OnContainerDataChanged("Deviation")
            End Set
        End Property

        ''' <summary>
        ''' 此元素中歌词所使用的默认语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 歌词的部分集合。
        ''' </summary>
        Public ReadOnly Property Tracks As DataContainerCollection(Of Track)
            Get
                Return m_Tracks
            End Get
        End Property

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim element = MyBase.ToXElement()
            With element
                .SetAttributeValue(Lyrics.XNDuration, m_Duration)
                .SetAttributeValue(Lyrics.XNDeviation, m_Deviation)
                .SetAttributeValue(Lyrics.XNLanguage, m_Language)
                .Add(From EachTrack In m_Tracks Where EachTrack IsNot Nothing
                      Select (EachTrack.ToXElement))
            End With
            Return element
        End Function

        ''' <summary>
        ''' 返回所有部分的歌词内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format(Prompts.Version, m_Duration, m_Tracks.FirstOrDefault)
        End Function

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal duration As TimeSpan?, ByVal deviation As TimeSpan?, ByVal Tracks As IEnumerable(Of Track))
            m_Duration = duration
            m_Deviation = deviation
            m_Tracks = New DataContainerCollection(Of Track)(If(Tracks, Enumerable.Empty(Of Track)))
            ObserveCollection(m_Tracks)
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            Dim tempTracks = Enumerable.Empty(Of Track)()
            If dataSource IsNot Nothing Then
                With dataSource
                    m_Duration = CType(.Attribute(Lyrics.XNDuration), TimeSpan?)
                    m_Deviation = CType(.Attribute(Lyrics.XNDeviation), TimeSpan?)
                    m_Language = CStr(.Attribute(Lyrics.XNLanguage))
                    tempTracks = (From EachTrack In .Elements(Lyrics.XNTrack)
                                  Select New Track(EachTrack))
                End With
            End If
            m_Tracks = New DataContainerCollection(Of Track)(tempTracks)
            ObserveCollection(m_Tracks, True)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示歌词中的部分。
    ''' </summary>
    ''' <remarks>不同的部分一般是由演唱者的不同而人为区分开的。</remarks>
    Public Class Track
        Inherits XElementContainer
        'Implements IReferable

        Private m_ArtistIds As ObservableCollection(Of Integer),
            m_Type As PartType?,
            m_Language As String,
            m_Lines As DataContainerCollection(Of Line)
        'm_Reference As Integer?

        'm_Lines As ObservableCollection(Of LineBase)

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return Lyrics.XNTrack
            End Get
        End Property

        ''' <summary>
        ''' 此部分歌词的默认演唱者列表。
        ''' </summary>
        Public ReadOnly Property ArtistIds As ObservableCollection(Of Integer)
            Get
                Return m_ArtistIds
            End Get
        End Property

        ''' <summary>
        ''' 此部分的类型。
        ''' </summary>
        Public Property [Type] As PartType?
            Get
                Return m_Type
            End Get
            Set(ByVal value As PartType?)
                m_Type = value
            End Set
        End Property

        ''' <summary>
        ''' 此元素中歌词所使用的默认语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 按顺序获取此部分中的行与行组。
        ''' </summary>
        ''' <remarks>有关行与行组的说明，请参见 <see cref="Line" /><!-- 、<see cref="LineGroup" /> -->。</remarks>
        Public ReadOnly Property Lines As DataContainerCollection(Of Line)
            Get
                Return m_Lines
            End Get
        End Property

        'Public Property Reference As Integer? Implements ObjectModel.IReferable.Reference
        '    Get
        '        Return m_Reference
        '    End Get
        '    Set(value As Integer?)
        '        m_Reference = value
        '    End Set
        'End Property

        ''' <summary>
        ''' 返回此部分的所有歌词行的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Join(";", m_Lines)
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(Lyrics.XNAIds, String.Join(" ", m_ArtistIds))
                .SetAttributeValue(Lyrics.XNType, If(m_Type Is Nothing, Nothing, EnumToXSD(m_Type.Value)))
                .SetAttributeValue(Lyrics.XNLanguage, m_Language)
                '.SetAttributeValue(Lyrics.XNRef, m_Reference)
                .Add(From EachLine In m_Lines
                     Where EachLine IsNot Nothing
                     Select (EachLine.ToXElement))
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal ArtistIds As IEnumerable(Of Integer), ByVal [type] As PartType?, ByVal language As String, ByVal lines As IEnumerable(Of Line))
            m_ArtistIds = New ObservableCollection(Of Integer)(If(ArtistIds, Enumerable.Empty(Of Integer)))
            m_Type = [type]
            m_Language = language
            m_Lines = New DataContainerCollection(Of Line)(If(lines, Enumerable.Empty(Of Line)))
            ObserveCollection(m_Lines, True)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal ArtistIds As IEnumerable(Of Integer), ByVal [type] As PartType?, ByVal lines As IEnumerable(Of Line))
            Me.New(ArtistIds, [type], Nothing, lines)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal ArtistIds As IEnumerable(Of Integer), ByVal lines As IEnumerable(Of Line))
            Me.New(ArtistIds, Nothing, Nothing, lines)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal lines As IEnumerable(Of Line))
            Me.New(Nothing, Nothing, Nothing, lines)
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            Dim tempAIds = Enumerable.Empty(Of Integer)()
            Dim tempLines = Enumerable.Empty(Of Line)()
            If dataSource IsNot Nothing Then
                With dataSource
                    Dim BaseValue As String
                    BaseValue = CStr(.Attribute(Lyrics.XNAIds))
                    If BaseValue <> Nothing Then
                        tempAIds = From EachItem In BaseValue.Split(DirectCast(Nothing, Char()),
                                                                    StringSplitOptions.RemoveEmptyEntries)
                                                                Select CInt(EachItem)
                    End If
                    m_Type = CType(XSDToEnum(CStr(.Attribute(Lyrics.XNType)), GetType(PartType)), PartType?)
                    m_Language = CStr(.Attribute(Lyrics.XNLanguage))
                    'm_Reference = CInt(.Attribute(Lyrics.XNRef))
                    tempLines = From EachLine In .Elements(Lyrics.XNLine)
                              Select New Line(EachLine)
                    'm_Lines.AddRange(From EachLG In .Elements(Lyrics.XNLineGroup)
                    '                 Select New LineGroup(EachLG))
                End With
            End If
            m_ArtistIds = New ObservableCollection(Of Integer)(tempAIds)
            m_Lines = New DataContainerCollection(Of Line)(tempLines)
            ObserveCollection(m_Lines, True)
            ObserveCollection(m_ArtistIds)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(DirectCast(Nothing, XElement))
        End Sub
    End Class

    '注意：此类暂时保留，因为需要考虑是否需要“行组”。但在使用时，用 Line 而不是 LineBase。
    ''' <summary>
    ''' 表示歌词中的行或行组。
    ''' </summary>
    Public MustInherit Class LineBase
        Inherits XElementContainer
        Implements IReferable

        Private m_Id As Integer?,
            m_Reference As Integer?,
            m_Begin As TimeSpan?,
            m_Language As String,
            m_ArtistIds As ObservableCollection(Of Integer)

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return Lyrics.XNLine
            End Get
        End Property

        ''' <summary>
        ''' 确定行的标识，以便于后期在确定其翻译等信息。
        ''' </summary>
        Public Property Id As Integer? Implements IIdentifiable.Id
            Get
                Return m_Id
            End Get
            Set(ByVal value As Integer?)
                m_Id = value
                OnContainerDataChanged("Id")
            End Set
        End Property

        ''' <summary>
        ''' 确定此行引用目标。指定目标的信息将作为此元素内容的默认值。
        ''' </summary>
        Public Property Reference As Integer? Implements IReferable.Reference
            Get
                Return m_Reference
            End Get
            Set(ByVal value As Integer?)
                m_Reference = value
                OnContainerDataChanged("Reference")
            End Set
        End Property

        ''' <summary>
        ''' 在引用行或行组时，改变目标中按时间顺序第一个出现的段开始的时间，并将目标中的段的开始时间全部平移。
        ''' </summary>
        Public Property Begin As TimeSpan?
            Get
                Return m_Begin
            End Get
            Set(ByVal value As TimeSpan?)
                m_Begin = value
                OnContainerDataChanged("Begin")
            End Set
        End Property

        ''' <summary>
        ''' 此行中歌词所使用的语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 此行歌词的演唱者列表。
        ''' </summary>
        Public ReadOnly Property ArtistIds As ObservableCollection(Of Integer)
            Get
                Return m_ArtistIds
            End Get
        End Property

        ''' <summary>
        ''' 获取此行或行组的行列表。
        ''' </summary>
        ''' <remarks>在返回时，不会生成新的 <see cref="Line" /> 实例。</remarks>
        Public MustOverride Function GetLines() As IEnumerable(Of Line)

        ''' <summary>
        ''' 获取此行或行组的段列表。
        ''' </summary>
        ''' <remarks>在返回时，不会生成新的 <see cref="Span" /> 实例。</remarks>
        Public MustOverride Function GetSpans() As IEnumerable(Of Span)

        ''' <summary>
        ''' 返回此歌词行的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Join(Nothing, GetSpans())
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(Lyrics.XNId, m_Id)
                .SetAttributeValue(Lyrics.XNLanguage, m_Language)
                .SetAttributeValue(Lyrics.XNRef, m_Reference)
                .SetAttributeValue(Lyrics.XNBegin, m_Begin)
                If m_ArtistIds.Count > 0 Then .SetAttributeValue(Lyrics.XNAIds, String.Join(" ", m_ArtistIds))
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal language As String)
            m_Id = id
            m_Language = language
            m_ArtistIds = New ObservableCollection(Of Integer)()
            ObserveCollection(m_ArtistIds)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?)
            Me.New(id, Nothing)
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            Dim tempAIds = Enumerable.Empty(Of Integer)()
            If dataSource IsNot Nothing Then
                With dataSource
                    m_Id = CType(.Attribute(Lyrics.XNId), Integer?)
                    m_Language = CStr(.Attribute(Lyrics.XNLanguage))
                    m_Reference = CType(.Attribute(Lyrics.XNRef), Integer?)
                    m_Begin = CType(.Attribute(Lyrics.XNBegin), TimeSpan?)
                    Dim baseValue As String = CStr(.Attribute(Lyrics.XNAIds))
                    If baseValue <> Nothing Then
                        tempAIds = From EachId In baseValue.Split(DirectCast(Nothing, Char()),
                                                                  StringSplitOptions.RemoveEmptyEntries)
                                                              Select CInt(EachId)
                    End If
                End With
            End If
            m_ArtistIds = New ObservableCollection(Of Integer)(tempAIds)
            ObserveCollection(m_ArtistIds)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(DirectCast(Nothing, XElement))
        End Sub
    End Class

    ''' <summary>
    ''' 表示歌词中的一行。
    ''' </summary>
    Public Class Line
        Inherits LineBase

        Private m_Spans As DataContainerCollection(Of Span)

        ''' <summary>
        ''' 此行中的歌词段列表。
        ''' </summary>
        Public Shadows ReadOnly Property Spans As DataContainerCollection(Of Span)
            Get
                Return m_Spans
            End Get
        End Property

        ''' <summary>
        ''' 基础结构。获取此行中的行列表。
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Overrides Function GetLines() As System.Collections.Generic.IEnumerable(Of Line)
            Static RV As IEnumerable(Of Line) = {Me}
            Return RV
        End Function

        ''' <summary>
        ''' 基础结构。获取此行中的歌词段列表。
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Overrides Function GetSpans() As System.Collections.Generic.IEnumerable(Of Span)
            Return m_Spans
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .Add(From EachSpan In GetSpans()
                     Where EachSpan IsNot Nothing
                     Select (EachSpan.ToXElement))
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal language As String, ByVal spans As IEnumerable(Of Span))
            MyBase.New(id, language)
            m_Spans = New DataContainerCollection(Of Span)(If(spans, Enumerable.Empty(Of Span)))
            ObserveCollection(m_Spans, True)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal spans As IEnumerable(Of Span))
            Me.New(id, Nothing, spans)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal spans As IEnumerable(Of Span))
            Me.New(Nothing, Nothing, spans)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?)
            Me.New(id, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            Dim tempSpans = Enumerable.Empty(Of Span)()
            If dataSource IsNot Nothing Then
                With dataSource
                    tempSpans = (From EachSpan In .Elements(Lyrics.XNSpan)
                                 Select New Span(EachSpan))
                End With
            End If
            m_Spans = New DataContainerCollection(Of Span)(tempSpans)
            ObserveCollection(m_Spans, True)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(DirectCast(Nothing, XElement))
        End Sub
    End Class

    ' ''' <summary>
    ' ''' 歌词中的行组。
    ' ''' </summary>
    ' ''' <remarks>行组，即几个联系较为紧密的行，其包含的行可以通过直接加入空格而合成为一行。请注意，在翻译时，每一行是分开进行翻译的。</remarks>
    'Public Class LineGroup
    '    Inherits LineBase

    '    Private m_Lines As ObservableCollection(Of Line)

    '    Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
    '        Get
    '            Return Lyrics.XNLineGroup
    '        End Get
    '    End Property

    '    ''' <summary>
    '    ''' 按顺序获取此行组中的行。
    '    ''' </summary>
    '    Public ReadOnly Property Lines As DataContainerCollection(Of Line)
    '        Get
    '            Return m_Lines
    '        End Get
    '    End Property

    '    Public Overrides Function GetLines() As System.Collections.Generic.IEnumerable(Of Line)
    '        Return m_Lines
    '    End Function

    '    ''' <summary>
    '    ''' 获取此行组中包含的所有段。
    '    ''' </summary>
    '    Public Overrides Function GetSpans() As System.Collections.Generic.IEnumerable(Of Span)
    '        Return (From EachLine In m_Lines
    '                Where EachLine IsNot Nothing).SelectMany(
    '                Function(EachLine) EachLine.Spans
    '                    )
    '    End Function

    '    ''' <summary>
    '    ''' 返回此歌词段的所有歌词行的内容。
    '    ''' </summary>
    '    Public Overrides Function ToString() As String
    '        Return String.Join(vbCrLf, Lines)
    '    End Function

    '    Public Overrides Function ToXElement() As System.Xml.Linq.XElement
    '        Dim Element = MyBase.ToXElement
    '        With Element
    '            .Add(From EachLine In m_Lines
    '                 Where EachLine IsNot Nothing
    '                 Select (EachLine.ToXElement))
    '        End With
    '        Return Element
    '    End Function

    '    ''' <summary>
    '    ''' 使用指定的数据进行初始化。
    '    ''' </summary>
    '    Public Sub New(ByVal lines As IEnumerable(Of Line))
    '        If lines IsNot Nothing Then m_Lines.AddRange(lines)
    '    End Sub

    '    ''' <summary>
    '    ''' 使用指定的数据源进行初始化。
    '    ''' </summary>
    '    ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
    '    Public Sub New(ByVal dataSource As XElement)
    '        MyBase.New(dataSource)
    '        If dataSource IsNot Nothing Then
    '            With dataSource
    '                m_Lines.AddRange(From EachLine In .Elements(Lyrics.XNLine) Select New Line(EachLine))
    '            End With
    '        End If
    '    End Sub

    '    ''' <summary>
    '    ''' 构造一个空的实例。
    '    ''' </summary>
    '    Public Sub New()
    '        MyBase.New()
    '    End Sub
    'End Class

    ''' <summary>
    ''' 歌词中的段。
    ''' </summary>
    ''' <remarks>“段”是按停顿、语速或是语言的变化而分隔出来的的词语、短语的整体或是其中的一部分，或是标点符号。</remarks>
    Public Class Span
        Inherits XElementContainer
        Implements IReferable

        Private m_Id As Integer?,
            m_Begin As TimeSpan,
            m_Duration As TimeSpan,
            m_Language As String,
            m_Reference As Integer?,
            m_Segments As DataContainerCollection(Of Segment)

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return Lyrics.XNSpan
            End Get
        End Property

        ''' <summary>
        ''' 确定段的标识，以便于后期引用。
        ''' </summary>
        Public Property Id As Integer? Implements IIdentifiable.Id
            Get
                Return m_Id
            End Get
            Set(ByVal value As Integer?)
                m_Id = value
            End Set
        End Property

        ''' <summary>
        ''' 段开始的时间。
        ''' </summary>
        Public Property Begin As TimeSpan
            Get
                Return m_Begin
            End Get
            Set(ByVal value As TimeSpan)
                m_Begin = value
                OnContainerDataChanged("Begin")
            End Set
        End Property

        ''' <summary>
        ''' 段持续的时间。
        ''' </summary>
        Public Property Duration As TimeSpan
            Get
                Return m_Duration
            End Get
            Set(ByVal value As TimeSpan)
                m_Duration = value
                OnContainerDataChanged("Duration")
            End Set
        End Property

        ''' <summary>
        ''' 段的结束时间。
        ''' </summary>
        Public Property [End] As TimeSpan
            Get
                Return m_Begin + m_Duration
            End Get
            Set(value As TimeSpan)
                Duration = value - m_Begin
            End Set
        End Property

        ''' <summary>
        ''' 此段中歌词所使用的语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 歌词的语义段列表。
        ''' </summary>
        Public ReadOnly Property Segments As DataContainerCollection(Of Segment)
            Get
                Return m_Segments
            End Get
        End Property

        ''' <summary>
        ''' 确定此段的引用目标。指定目标的信息将作为此元素内容的默认值。
        ''' </summary>
        Public Property Reference As Integer? Implements IReferable.Reference
            Get
                Return m_Reference
            End Get
            Set(ByVal value As Integer?)
                m_Reference = value
                OnContainerDataChanged("Reference")
            End Set
        End Property

        ''' <summary>
        ''' 返回此歌词段的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Join(Nothing, m_Segments)
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(Lyrics.XNId, m_Id)
                .SetAttributeValue(Lyrics.XNRef, m_Reference)
                .SetAttributeValue(Lyrics.XNBegin, m_Begin)
                .SetAttributeValue(Lyrics.XNDuration, m_Duration)
                .SetAttributeValue(Lyrics.XNLanguage, m_Language)
                .Add(From EachSegment In m_Segments
                     Where EachSegment IsNot Nothing
                     Select (EachSegment.ToXElement))
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal language As String, begin As TimeSpan, duration As TimeSpan, ByVal segments As IEnumerable(Of Segment))
            m_Id = id
            m_Language = language
            m_Begin = begin
            m_Duration = duration
            m_Segments = New DataContainerCollection(Of Segment)(If(segments, Enumerable.Empty(Of Segment)))
            ObserveCollection(m_Segments, True)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal language As String, begin As TimeSpan, duration As TimeSpan)
            Me.New(id, language, begin, duration, Nothing)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal language As String, ByVal segments As IEnumerable(Of Segment))
            Me.New(id, language, Nothing, Nothing, segments)
        End Sub


        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?, ByVal segments As IEnumerable(Of Segment))
            Me.New(id, Nothing, Nothing, Nothing, segments)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal segments As IEnumerable(Of Segment))
            Me.New(Nothing, Nothing, Nothing, Nothing, segments)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal id As Integer?)
            Me.New(id, Nothing, Nothing, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            Dim tempSegments = Enumerable.Empty(Of Segment)()
            If dataSource IsNot Nothing Then
                With dataSource
                    m_Id = CType(.Attribute(Lyrics.XNId), Integer?)
                    m_Reference = CType(.Attribute(Lyrics.XNRef), Integer?)
                    m_Begin = CType(.Attribute(Lyrics.XNBegin), TimeSpan?).GetValueOrDefault
                    m_Duration = CType(.Attribute(Lyrics.XNDuration), TimeSpan?).GetValueOrDefault
                    m_Language = CStr(.Attribute(Lyrics.XNLanguage))
                    tempSegments = (From EachSegment In dataSource.Elements(Lyrics.XNSegment)
                                    Select New Segment(EachSegment))
                End With
            End If
            m_Segments = New DataContainerCollection(Of Segment)(tempSegments)
            ObserveCollection(m_Segments, True)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(DirectCast(Nothing, XElement))
        End Sub
    End Class

    ''' <summary>
    ''' 歌词的语义段。
    ''' </summary>
    ''' <remarks>目前按拼音的标注需要进行人为划分，是歌词的最小部分。</remarks>
    Public Class Segment
        Inherits XElementContainer

        Private m_Language As String,
            m_Latinized As String,
            m_Alphabetic As String,
            m_Text As String

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return Lyrics.XNSegment
            End Get
        End Property

        ''' <summary>
        ''' 此段中歌词所使用的语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 此行歌词拼音文字的拉丁化表示形式（如汉语拼音，或是罗马音）。
        ''' </summary>
        Public Property Latinized As String
            Get
                Return m_Latinized
            End Get
            Set(value As String)
                m_Latinized = value
                OnContainerDataChanged("Latinized")
            End Set
        End Property

        ''' <summary>
        ''' 此行歌词的本地拼音文字表示形式（如假名）。仅当 <see cref="Latinized" /> 无法包含足够的信息时使用此属性。
        ''' </summary>
        Public Property Alphabetic As String
            Get
                Return m_Alphabetic
            End Get
            Set(value As String)
                m_Alphabetic = value
                OnContainerDataChanged("Alphabetic")
            End Set
        End Property

        ''' <summary>
        ''' 歌词段的内容（歌词）。
        ''' </summary>
        Public Property Text As String
            Get
                Return m_Text
            End Get
            Set(ByVal value As String)
                m_Text = value
                OnContainerDataChanged("Text")
            End Set
        End Property

        ''' <summary>
        ''' 返回此语义段的文本。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return m_Text
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(Lyrics.XNLanguage, m_Language)
                .SetAttributeValue(Lyrics.XNLatinized, m_Latinized)
                .SetAttributeValue(Lyrics.XNAlphabetic, m_Alphabetic)
                .Value = If(m_Text, "")
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal language As String, ByVal latinized As String, ByVal alphabetic As String, ByVal text As String)
            m_Language = language
            If latinized IsNot Nothing Then m_Latinized = latinized
            If alphabetic IsNot Nothing Then m_Alphabetic = alphabetic
            m_Text = text
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal language As String, ByVal text As String)
            Me.New(language, Nothing, Nothing, text)
        End Sub

        ''' <summary>
        ''' 使用指定的数据进行初始化。
        ''' </summary>
        Public Sub New(ByVal text As String)
            Me.New(Nothing, Nothing, Nothing, text)
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            If dataSource IsNot Nothing Then
                With dataSource
                    m_Language = CStr(.Attribute(Lyrics.XNLanguage))
                    m_Latinized = CStr(.Attribute(Lyrics.XNLatinized))
                    m_Alphabetic = CStr(.Attribute(Lyrics.XNAlphabetic))
                    m_Text = .Value
                End With
            End If
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(DirectCast(Nothing, XElement))
        End Sub
    End Class
End Namespace