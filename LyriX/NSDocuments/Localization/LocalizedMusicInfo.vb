Imports <xmlns="LyriX/2011/package/localizedMusicInfo.xsd">
Imports LyriX.Document.ObjectModel
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Document
    ''' <summary>
    ''' 表示按照某一语言进行翻译的音乐信息的架构。（LyriX/[.../]localizedMusicInfo.xml）
    ''' </summary>
    Public Class LocalizedMusicInfo
        Inherits XPackagePartContainer
        Implements ILocalizedDataContainer

        '元素名称（XName）
        Friend Shared ReadOnly XNArtists As XName = GetXmlNamespace().GetName("artists"),
            XNArtist As XName = GetXmlNamespace().GetName("artist"),
            XNTitle As XName = GetXmlNamespace().GetName("title"),
            XNAlbum As XName = GetXmlNamespace().GetName("album")
        '属性名称（XName）（AttributeFromDefault = unqualified，-> namespace = null）
        Friend Shared ReadOnly XNAId As XName = "aid",
            XNCharacterName As XName = "characterName"

        Private m_Artists As DataContainerCollection(Of LocalizedArtist),
            m_Title As String,
            m_Album As String

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return GetXmlNamespace().GetName("localizedMusicInfo")
            End Get
        End Property

        ''' <summary>
        ''' 参与创作的艺术家。
        ''' </summary>
        Public ReadOnly Property Artists As DataContainerCollection(Of LocalizedArtist)
            Get
                Return m_Artists
            End Get
        End Property

        ''' <summary>
        ''' 歌曲的标题。
        ''' </summary>
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(ByVal value As String)
                m_Title = value
                OnContainerDataChanged("Title")
            End Set
        End Property

        ''' <summary>
        ''' 专辑/电影名称。
        ''' </summary>
        Public Property Album As String
            Get
                Return m_Album
            End Get
            Set(ByVal value As String)
                m_Album = value
                OnContainerDataChanged("Album")
            End Set
        End Property

        ''' <summary>
        ''' 基础结构，获取包含此对象对应的未本地化的 <see cref="DataContainer" />。
        ''' </summary>
        Private ReadOnly Property [_Source] As ObjectModel.DataContainer Implements ObjectModel.ILocalizedDataContainer.Source
            Get
                Dim Package = FindPackage()
                If Package Is Nothing Then
                    Return Nothing
                Else
                    Return Package.MusicInfo
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="MusicInfo" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        Public ReadOnly Property Source As MusicInfo
            Get
                Return DirectCast(_Source, MusicInfo)
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
        ''' <remarks>此操作会同步艺术家列表。</remarks>
        ''' <exception cref="InvalidOperationException">进行同步时，<see cref="Source" /> 为 <c>null</c>。</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="mode" /> 不是有效的 <see cref="ChildrenSynchronizationMode" />。</exception>
        Public Sub SynchronizeChildren(mode As ObjectModel.ChildrenSynchronizationMode) Implements ObjectModel.ILocalizedDataContainer.SynchronizeChildren
            Dim src = Source
            If src Is Nothing Then
                Throw New InvalidOperationException
            Else
                If (mode And ChildrenSynchronizationMode.RemoveInvalid) = ChildrenSynchronizationMode.RemoveInvalid Then
                    Dim InvalidArtists As New List(Of Document.LocalizedArtist)
                    For Each EA In m_Artists
                        Dim EachArtist = EA
                        If EachArtist.SourceId Is Nothing OrElse
                            Not Aggregate EachArtist1 In src.Artists
                                Where EachArtist1.Id = EachArtist.SourceId
                                Into Any() Then
                            InvalidArtists.Add(EachArtist)
                        End If
                    Next
                    For Each EachArtist In InvalidArtists
                        m_Artists.Remove(EachArtist)
                    Next
                End If
                If (mode And ChildrenSynchronizationMode.AddNew) = ChildrenSynchronizationMode.AddNew Then
                    For Each EA In src.Artists
                        Dim EachArtist = EA
                        If EachArtist.Id IsNot Nothing AndAlso
                            Not Aggregate EachArtist1 In m_Artists
                                Where EachArtist1.SourceId = EachArtist.Id
                                Into Any() Then
                            m_Artists.Add(New Document.LocalizedArtist(EachArtist.Id))
                        End If
                    Next
                End If
            End If
        End Sub

        ''' <summary>
        ''' 返回歌曲的专辑与标题名称。
        ''' </summary>
        Public Overrides Function ToString() As String
            If m_Album = Nothing Then
                Return String.Format(Prompts.MusicInfoIL, m_Title)
            Else
                Return String.Format(Prompts.MusicInfoILA, m_Title, m_Album)
            End If
        End Function

        Public Overrides Function ToXDocument() As System.Xml.Linq.XDocument
            Dim doc = MyBase.ToXDocument()
            With doc.Root
                .SetElementValue(XNTitle, m_Title)
                .SetElementValue(XNAlbum, m_Album)
                .Add(New XElement(XNArtists, From EachArtist In m_Artists
                                             Where EachArtist IsNot Nothing
                                             Select EachArtist.ToXElement))
            End With
            Return doc
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XDocument)
            MyBase.New(dataSource)
            Dim tempArtists = Enumerable.Empty(Of LocalizedArtist)()
            If dataSource IsNot Nothing Then
                Dim body = dataSource.Root
                If body IsNot Nothing Then
                    With body
                        Dim baseElement As XElement
                        m_Title = CStr(.Element(XNTitle))
                        m_Album = CStr(.Element(XNAlbum))
                        baseElement = .Element(XNArtists)
                        If baseElement IsNot Nothing Then
                            tempArtists = From EachArtist In baseElement.Elements(XNArtist)
                                        Select New LocalizedArtist(EachArtist)
                        End If
                    End With
                End If
            End If
            m_Artists = New DataContainerCollection(Of LocalizedArtist)(tempArtists)
            ObserveCollection(m_Artists, True)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个艺术家或是群组的信息。
    ''' </summary>
    Public Class LocalizedArtist
        Inherits XElementContainer
        Implements ILocalizedDataContainer

        Private m_SourceId As Integer?,
            m_CharacterName As String,
            m_Name As String

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return LocalizedMusicInfo.XNArtist
            End Get
        End Property

        ''' <summary>
        ''' 基础结构，获取包含此对象对应的未本地化的 <see cref="DataContainer" />。
        ''' </summary>
        Private ReadOnly Property [_Source] As ObjectModel.DataContainer Implements ObjectModel.ILocalizedDataContainer.Source
            Get
                If m_SourceId Is Nothing Then Return Nothing
                Dim Package = FindPackage()
                If Package Is Nothing Then
                    Return Nothing
                Else
                    Return Aggregate EachItem In Package.MusicInfo.Artists
                           Where EachItem.Id = m_SourceId
                           Into FirstOrDefault()
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="ArtistBase" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        Public ReadOnly Property Source As ArtistBase
            Get
                Return DirectCast(_Source, ArtistBase)
            End Get
        End Property

        ''' <summary>
        ''' 获取/设置本地化源的标识符。
        ''' </summary>
        Public Property SourceId As Integer? Implements ILocalizedDataContainer.SourceId
            Get
                Return m_SourceId
            End Get
            Set(ByVal value As Integer?)
                m_SourceId = value
                OnContainerDataChanged("SourceId")
            End Set
        End Property

        ''' <summary>
        ''' 基础结构。此方法与此类无关。
        ''' </summary>
        Private Sub SynchronizeChildren(mode As ObjectModel.ChildrenSynchronizationMode) Implements ObjectModel.ILocalizedDataContainer.SynchronizeChildren
            Throw New NotSupportedException
        End Sub

        ''' <summary>
        ''' 艺术家在此歌曲中的角色名称（如果有）。
        ''' </summary>
        Public Property CharacterName As String
            Get
                Return m_CharacterName
            End Get
            Set(ByVal value As String)
                m_CharacterName = value
                OnContainerDataChanged("CharacterName")
            End Set
        End Property

        ''' <summary>
        ''' 艺术家的名称。
        ''' </summary>
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = value
                OnContainerDataChanged("Name")
            End Set
        End Property

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(LocalizedMusicInfo.XNAId, m_SourceId)
                .SetAttributeValue(LocalizedMusicInfo.XNCharacterName, m_CharacterName)
                .Value = If(m_Name, "")
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的艺术家标识符进行初始化。
        ''' </summary>
        Public Sub New(ByVal artistId? As Integer)
            m_SourceId = artistId
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            If dataSource IsNot Nothing Then
                With dataSource
                    m_SourceId = CType(.Attribute(LocalizedMusicInfo.XNAId), Integer?)
                    m_CharacterName = CStr(.Attribute(LocalizedMusicInfo.XNCharacterName))
                    m_Name = .Value
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