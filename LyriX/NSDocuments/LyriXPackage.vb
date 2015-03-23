Imports LyriX.Document.ObjectModel
Imports System.ComponentModel

Namespace Document
    ''' <summary>
    ''' 用于读取 LyriX 包所包含的信息。
    ''' </summary>
    Public NotInheritable Class LyriXPackage
        Inherits DataContainer

        ''' <summary>
        ''' LyriX 包的默认小写后缀名“lrcx”，不包括点。
        ''' </summary>
        Public Const FileExt = "lrcx"

        Friend Const PNHeader = "/header.xml"
        Friend Const PNMusicInfo = "/musicInfo.xml"
        Friend Const PNLyrics = "/lyrics.xml"

        Private Shared ReadOnly PUHeader As New Uri(PNHeader, UriKind.Relative)
        Private Shared ReadOnly PUMusicInfo As New Uri(PNMusicInfo, UriKind.Relative)
        Private Shared ReadOnly PULyrics As New Uri(PNLyrics, UriKind.Relative)

        Private m_Header As Header
        Private m_MusicInfo As MusicInfo
        Private m_Lyrics As Lyrics
        Private m_LocalizedParts As LocalizedPackagePartsCollection

        ''' <summary>
        ''' 获取可用于文件对话框的 LyriX 包的筛选器字符串，格式为“...|...”。
        ''' </summary>
        Public Shared ReadOnly Property FileFilter As String
            Get
                Return Prompts.LyriXFileFilter
            End Get
        End Property

        ''' <summary>
        ''' 基础结构。获取包中的子级。
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Overrides ReadOnly Iterator Property Children As IEnumerable(Of DataContainer)
            Get
                Yield m_Header
                Yield m_MusicInfo
                Yield m_Lyrics
                Yield m_LocalizedParts
            End Get
        End Property

        ''' <summary>
        ''' 获取/设置 LyriX 包的基本信息。
        ''' </summary>
        ''' <exception cref="ArgumentNullException">试图将值设为 <c>null</c>。</exception>
        Public Property Header As Header
            Get
                Return m_Header
            End Get
            Set(ByVal value As Header)
                If value Is Nothing Then
                    Throw New ArgumentNullException("value")
                Else
                    m_Lyrics.Detach()
                    value.Attach(Me)
                    m_Header = value
                    OnContainerDataChanged("Header")
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置 LyriX 包的音乐信息。
        ''' </summary>
        ''' <exception cref="ArgumentNullException">试图将值设为 <c>null</c>。</exception>
        Public Property MusicInfo As MusicInfo
            Get
                Return m_MusicInfo
            End Get
            Set(ByVal value As MusicInfo)
                If value Is Nothing Then
                    Throw New ArgumentNullException("value")
                Else
                    m_MusicInfo.Detach()
                    value.Attach(Me)
                    m_MusicInfo = value
                    OnContainerDataChanged("MusicInfo")
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置 LyriX 包的歌词信息。
        ''' </summary>
        ''' <exception cref="ArgumentNullException">试图将值设为 <c>null</c>。</exception>
        Public Property Lyrics As Lyrics
            Get
                Return m_Lyrics
            End Get
            Set(ByVal value As Lyrics)
                If value Is Nothing Then
                    Throw New ArgumentNullException("value")
                Else
                    m_Lyrics.Detach()
                    value.Attach(Me)
                    m_Lyrics = value
                    OnContainerDataChanged("Lyrics")
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置 LyriX 包中与语言相关的部分列表。
        ''' </summary>
        Public ReadOnly Property LocalizedParts As LocalizedPackagePartsCollection
            Get
                Return m_LocalizedParts
            End Get
        End Property

        ''' <summary>
        ''' 将 LyriX 信息保存到指定包。
        ''' </summary>
        ''' <param name="package">一个空包。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="package" /> 为 <c>null</c>。</exception>
        Public Sub SavePackage(ByVal package As Package)
            If package Is Nothing Then
                Throw New ArgumentNullException("package")
            Else
                m_Header.WritePackage(package, PUHeader)
                m_MusicInfo.WritePackage(package, PUMusicInfo)
                m_Lyrics.WritePackage(package, PULyrics)
                m_LocalizedParts.WritePackage(package)
            End If
        End Sub

        ''' <summary>
        ''' 将 LyriX 信息保存到指定流。
        ''' </summary>
        ''' <param name="stream">保存的目标。</param>
        Public Sub SavePackage(ByVal stream As IO.Stream)
            Using pk = Package.Open(stream, IO.FileMode.Create)
                SavePackage(pk)
            End Using
        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        Private Sub Init(Optional ByVal package As Package = Nothing)
            '加载包
            If package Is Nothing Then
                '默认值
                m_Header = New Header
                m_MusicInfo = New MusicInfo
                m_Lyrics = New Lyrics
                m_LocalizedParts = New LocalizedPackagePartsCollection
            Else
                With package
                    '根目录
                    m_Header = .ReadPackagePart(PUHeader, Function(doc) New Header(doc))
                    m_MusicInfo = .ReadPackagePart(PUMusicInfo, Function(doc) New MusicInfo(doc))
                    m_Lyrics = .ReadPackagePart(PULyrics, Function(doc) New Lyrics(doc))
                    '本地化
                    m_LocalizedParts = New LocalizedPackagePartsCollection(package)
                End With
            End If
            m_Header.Attach(Me)
            m_MusicInfo.Attach(Me)
            m_Lyrics.Attach(Me)
            m_LocalizedParts.Attach(Me)
        End Sub

        ''' <summary>
        ''' 从包中载入 LyriX 信息。
        ''' </summary>
        ''' <exception cref="IO.FileFormatException">XML 文档格式不正确。</exception>
        ''' <exception cref="IO.IOException">无法打开指定的部分。</exception>
        Public Sub New(ByVal package As Package)
            Init(package)
        End Sub

        ''' <summary>
        ''' 从文件中载入 LyriX 信息。
        ''' </summary>
        ''' <exception cref="IO.FileFormatException">XML 文档格式不正确。</exception>
        ''' <exception cref="IO.IOException">无法打开指定的部分。</exception>
        Public Sub New(ByVal path As String)
            If path Is Nothing Then
                Init()
            Else
                Using pk = Package.Open(path, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                    Init(pk)
                End Using
            End If
        End Sub

        ''' <summary>
        ''' 从流中载入 LyriX 信息。
        ''' </summary>
        ''' <exception cref="IO.FileFormatException">XML 文档格式不正确。</exception>
        ''' <exception cref="IO.IOException">无法访问指定的流，或是打开包的指定部分。</exception>
        Public Sub New(ByVal stream As IO.Stream)
            If stream Is Nothing Then
                Init()
            Else
                Using pk = Package.Open(stream)
                    Init(pk)
                End Using
            End If
        End Sub

        ''' <summary>
        ''' 初始化一个空的 <see cref="LyriXPackage" />。
        ''' </summary>
        Public Sub New()
            Init(Nothing)
        End Sub

        Protected Sub New(prevInstance As LyriXPackage)
            Debug.Assert(prevInstance IsNot Nothing)
            With prevInstance
                m_Header = DirectCast(.m_Header.Clone, Document.Header)
                m_MusicInfo = DirectCast(.m_MusicInfo.Clone, Document.MusicInfo)
                m_Lyrics = DirectCast(.m_Lyrics.Clone, Document.Lyrics)
                m_LocalizedParts = DirectCast(.m_LocalizedParts.Clone, Document.LocalizedPackagePartsCollection)
            End With
        End Sub
    End Class
End Namespace