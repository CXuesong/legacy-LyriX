Imports LyriX.Document.ObjectModel
Imports System.ComponentModel

Namespace Document
    ''' <summary>
    ''' 表示 LyriX 包中按语言标识分组的若干部分组成的列表。
    ''' </summary>
    Public NotInheritable Class LocalizedPackagePartsCollection
        Inherits DataContainer
        Implements ILocalizedDataContainer

        Private m_Items As DataContainerCollection(Of LocalizedPackageParts)

        ''' <summary>
        ''' 获取所有语言的本地化部分。
        ''' </summary>
        Public ReadOnly Property Items As DataContainerCollection(Of LocalizedPackageParts)
            Get
                Return m_Items
            End Get
        End Property

        ''' <summary>
        ''' 获取或设置与指定的语言相关联的值。
        ''' </summary>
        ''' <param name="language">要获取或设置对象的语言标识，不区分大小写。<c>null</c> 与空字符串等价。</param>
        ''' <value>指定语言对应的部分。在获取时，如果指定的语言在列表中不存在，则会返回 <c>null</c>。在设置时，如果指定的部分为 <c>null</c>，则会删除此语言项。</value>
        Default Public ReadOnly Property Item(ByVal language As String) As LocalizedPackageParts
            Get
                Return Aggregate EachItem In m_Items
                       Where String.IsNullOrEmpty(language) AndAlso String.IsNullOrEmpty(EachItem.Language) OrElse
                                String.Equals(EachItem.Language, language, StringComparison.OrdinalIgnoreCase)
                       Into FirstOrDefault()
            End Get
        End Property

        ''' <summary>
        ''' 获取与指定的语言相关联的值（并在必要时向列表中添加项）。
        ''' </summary>
        ''' <param name="language">要获取或设置对象的语言标识，不区分大小写。<c>null</c> 与空字符串等价。</param>
        ''' <value>指定语言对应的部分。在获取时，如果指定的语言在列表中不存在，则会向列表中添加指定语言的项目。</value>
        Public ReadOnly Property SafeItem(ByVal language As String) As LocalizedPackageParts
            Get
                Dim ListItem = Me.Item(language)
                If ListItem Is Nothing Then
                    Dim NewItem As New LocalizedPackageParts(language)
                    m_Items.Add(NewItem)
                    Return NewItem
                Else
                    Return ListItem
                End If
            End Get
        End Property

        ''' <summary>
        ''' 基础结构，获取包含此对象对应的未本地化的 <see cref="DataContainer" />。
        ''' </summary>
        Private ReadOnly Property [_Source] As ObjectModel.DataContainer Implements ObjectModel.ILocalizedDataContainer.Source
            Get
                Return FindPackage()
            End Get
        End Property

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="Package" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        Public ReadOnly Property Source As LyriXPackage
            Get
                Return FindPackage()
            End Get
        End Property

        ''' <summary>
        ''' 基础结构。此属性与此类无关。
        ''' </summary>
        Private Property SourceId As Integer? Implements ObjectModel.ILocalizedDataContainer.SourceId
            Get
                Return Nothing
            End Get
            Set(value As Integer?)
                Throw New NotSupportedException
            End Set
        End Property

        ''' <summary>
        ''' 从源同步本地化信息的子项。
        ''' </summary>
        ''' <param name="mode">同步模式。</param>
        ''' <remarks>此操作会同步子级的子项。</remarks>
        ''' <exception cref="InvalidOperationException">进行同步时，<see cref="Source" /> 为 <c>null</c>。</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="mode" /> 不是有效的 <see cref="ChildrenSynchronizationMode" />。</exception>
        Public Sub SynchronizeChildren(mode As ObjectModel.ChildrenSynchronizationMode) Implements ObjectModel.ILocalizedDataContainer.SynchronizeChildren
            If Source Is Nothing Then
                Throw New InvalidOperationException
            Else
                For Each EachItem In m_Items
                    EachItem.SynchronizeChildren(mode)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 将此实例的信息保存到包。
        ''' </summary>
        ''' <param name="package">保存的目标。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="package" /> 为 <c>null</c>。</exception>
        Public Sub WritePackage(ByVal package As Package)
            If package Is Nothing Then
                Throw New ArgumentNullException("package")
            Else
                For Each EachItem In m_Items
                    EachItem.WritePackage(package)
                Next
            End If
        End Sub

        Protected Sub New(prevInstance As LocalizedPackagePartsCollection)
            Debug.Assert(prevInstance IsNot Nothing)
            m_Items = New DataContainerCollection(Of LocalizedPackageParts)(From EachItem In prevInstance.m_Items
                                                                            Select DirectCast(EachItem.Clone, LocalizedPackageParts))
            ObserveCollection(m_Items, True)
        End Sub

        Public Sub New(dataSource As Package)
            m_Items = New DataContainerCollection(Of LocalizedPackageParts)
            If dataSource IsNot Nothing Then
                For Each EachPart In dataSource.GetParts
                    Dim PartName = EachPart.Uri.ToString.Split({"/"c}, StringSplitOptions.RemoveEmptyEntries)
                    If PartName.Count = 2 Then  'Eg. /zh-Hans-cn/info.xml
                        If Me.Item(PartName(0)) Is Nothing Then
                            m_Items.Add(New LocalizedPackageParts(PartName(0), dataSource))
                        End If
                    End If
                Next
            End If
            ObserveCollection(m_Items, True)
        End Sub

        Public Sub New()
            Me.New(DirectCast(Nothing, Package))
        End Sub
    End Class

    ''' <summary>
    ''' 表示 LyriX 包中按语言标识分组的若干部分。
    ''' </summary>
    Public NotInheritable Class LocalizedPackageParts
        Inherits DataContainer
        Implements ILocalizedDataContainer

        Friend Const PNMusicInfo = "/localizedMusicInfo.xml"
        Friend Const PNLyrics = "/localizedLyrics.xml"

        Private m_Language As String
        Private m_MusicInfo As LocalizedMusicInfo
        Private m_Lyrics As LocalizedLyrics

        ''' <summary>
        ''' 基础结构。获取包中的子级。
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Overrides ReadOnly Iterator Property Children As IEnumerable(Of DataContainer)
            Get
                Yield m_MusicInfo
                Yield m_Lyrics
            End Get
        End Property

        ''' <summary>
        ''' 获取/设置此部分的本地化语言。
        ''' </summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        ''' <summary>
        ''' 按照某一语言进行翻译的音乐信息。
        ''' </summary>
        ''' <exception cref="ArgumentNullException">试图将值设为 <c>null</c>。</exception>
        Public Property MusicInfo As LocalizedMusicInfo
            Get
                Return m_MusicInfo
            End Get
            Set(value As LocalizedMusicInfo)
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
        ''' 按照某一语言进行翻译的歌词。
        ''' </summary>
        ''' <exception cref="ArgumentNullException">试图将值设为 <c>null</c>。</exception>
        Public Property Lyrics As LocalizedLyrics
            Get
                Return m_Lyrics
            End Get
            Set(value As LocalizedLyrics)
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
        ''' 基础结构，获取包含此对象对应的未本地化的 <see cref="DataContainer" />。
        ''' </summary>
        Private ReadOnly Property [_Source] As ObjectModel.DataContainer Implements ObjectModel.ILocalizedDataContainer.Source
            Get
                Return FindPackage()
            End Get
        End Property

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="Package" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        Public ReadOnly Property Source As LyriXPackage
            Get
                Return FindPackage()
            End Get
        End Property

        ''' <summary>
        ''' 基础结构。此属性与此类无关。
        ''' </summary>
        Private Property SourceId As Integer? Implements ObjectModel.ILocalizedDataContainer.SourceId
            Get
                Return Nothing
            End Get
            Set(value As Integer?)
                Throw New NotSupportedException
            End Set
        End Property

        ''' <summary>
        ''' 从源同步本地化信息的子项。
        ''' </summary>
        ''' <param name="mode">同步模式。</param>
        ''' <remarks>此操作会同步子级的子项。</remarks>
        ''' <exception cref="InvalidOperationException">进行同步时，<see cref="Source" /> 为 <c>null</c>。</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="mode" /> 不是有效的 <see cref="ChildrenSynchronizationMode" />。</exception>
        Public Sub SynchronizeChildren(mode As ObjectModel.ChildrenSynchronizationMode) Implements ObjectModel.ILocalizedDataContainer.SynchronizeChildren
            If Source Is Nothing Then
                Throw New InvalidOperationException
            Else
                m_MusicInfo.SynchronizeChildren(mode)
                m_Lyrics.SynchronizeChildren(mode)
            End If
        End Sub

        ''' <summary>
        ''' 将此实例的信息保存到包。
        ''' </summary>
        ''' <param name="package">保存的目标。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="package" /> 为 <c>null</c>。</exception>
        Public Sub WritePackage(ByVal package As Package)
            If package Is Nothing Then
                Throw New ArgumentNullException("package")
            Else
                '部件 URI 必须以正斜杠分隔，不能出现两个斜杠
                Dim LanguageUri = If(m_Language = Nothing, Nothing, "/" & m_Language)
                Dim PUMusicInfo As New Uri(LanguageUri & PNMusicInfo, UriKind.Relative)
                Dim PULyrics As New Uri(LanguageUri & PNLyrics, UriKind.Relative)
                m_MusicInfo.WritePackage(package, PUMusicInfo)
                m_Lyrics.WritePackage(package, PULyrics)
            End If
        End Sub

        ''' <summary>
        ''' 从包中加载本地化部分的信息。
        ''' </summary>
        ''' <param name="language">此部分的本地化语言（如：zh-cn）。</param>
        ''' <param name="package">加载的源。</param>
        ''' <exception cref="IO.FileFormatException">XML 文档格式不正确。</exception>
        ''' <exception cref="IO.IOException">无法打开指定的部分。</exception>
        Friend Sub New(ByVal language As String, ByVal package As Package)
            '加载包
            If package Is Nothing Then
                m_MusicInfo = New LocalizedMusicInfo
                m_Lyrics = New LocalizedLyrics
            Else
                Dim LanguageUri = If(language = Nothing, Nothing, "/" & language)
                Dim PUMusicInfo As New Uri(LanguageUri & PNMusicInfo, UriKind.Relative)
                Dim PULyrics As New Uri(LanguageUri & PNLyrics, UriKind.Relative)

                With package
                    m_MusicInfo = .ReadPackagePart(PUMusicInfo, Function(doc) New LocalizedMusicInfo(doc))
                    m_Lyrics = .ReadPackagePart(PULyrics, Function(doc) New LocalizedLyrics(doc))
                End With
            End If
            m_MusicInfo.Attach(Me)
            m_Lyrics.Attach(Me)
            m_Language = language
        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="language">此部分的本地化语言（如：zh-cn）。</param>
        Friend Sub New(ByVal language As String)
            Me.New(language, Nothing)
        End Sub

        ''' <summary>
        ''' 初始化一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing, Nothing)
        End Sub

        Protected Sub New(prevInstance As LocalizedPackageParts)
            m_Language = prevInstance.m_Language
            m_MusicInfo = DirectCast(prevInstance.m_MusicInfo.Clone, LocalizedMusicInfo)
            m_Lyrics = DirectCast(prevInstance.m_Lyrics.Clone, LocalizedLyrics)
        End Sub
    End Class
End Namespace