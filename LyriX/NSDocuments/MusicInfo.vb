Imports <xmlns="LyriX/2011/package/musicInfo.xsd">
Imports LyriX.Document.ObjectModel
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Document
    ''' <summary>
    ''' 表示 LyriX 所包含的歌曲信息。（LyriX/musicInfo.xml）
    ''' </summary>
    Public NotInheritable Class MusicInfo
        Inherits XPackagePartContainer

        '元素名称（XName）
        Friend Shared ReadOnly XNArtists As XName = GetXmlNamespace().GetName("artists"),
            XNArtist As XName = GetXmlNamespace().GetName("artist"),
            XNArtistGroup As XName = GetXmlNamespace().GetName("artistGroup"),
            XNGenres As XName = GetXmlNamespace().GetName("genres"),
            XNGenre As XName = GetXmlNamespace().GetName("genre"),
            XNTrack As XName = GetXmlNamespace().GetName("track"),
            XNReleaseYear As XName = GetXmlNamespace().GetName("releaseYear"),
            XNTitle As XName = GetXmlNamespace().GetName("title"),
            XNAlbum As XName = GetXmlNamespace().GetName("album")
        '属性名称（XName）（AttributeFromDefault = unqualified，-> namespace = null）
        Friend Shared ReadOnly XNId As XName = "id",
            XNAIds As XName = "aids",
            XNSex As XName = "sex",
            XNJobs As XName = "jobs",
            XNCharacterName As XName = "characterName"

        Private m_Artists As DataContainerCollection(Of ArtistBase),
            m_Genres As ObservableCollection(Of String),
            m_Track As Integer?,
            m_ReleaseYear As Integer?,
            m_Title As String,
            m_Album As String
        Private m_ArtistIndexManager As New IndexManager(Me, GetType(ArtistBase))

        ''' <summary>
        ''' 获取艺术家索引的管理器。
        ''' </summary>
        Public ReadOnly Property ArtistIndexManager As IndexManager
            Get
                Return m_ArtistIndexManager
            End Get
        End Property

        ''' <summary>
        ''' 参与创作的艺术家。
        ''' </summary>
        Public ReadOnly Property Artists As DataContainerCollection(Of ArtistBase)
            Get
                Return m_Artists
            End Get
        End Property

        ''' <summary>
        ''' 流派。
        ''' </summary>
        Public ReadOnly Property Genres As ObservableCollection(Of String)
            Get
                Return m_Genres
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
                'CheckReadOnly()
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
                'CheckReadOnly()
                m_Album = value
                OnContainerDataChanged("Album")
            End Set
        End Property

        ''' <summary>
        ''' 曲目编号。
        ''' </summary>
        Public Property Track As Integer?
            Get
                Return m_Track
            End Get
            Set(ByVal value As Integer?)
                'CheckReadOnly()
                m_Track = value
                OnContainerDataChanged("Track")
            End Set
        End Property

        ''' <summary>
        ''' 发行时间（年份）。
        ''' </summary>
        Public Property ReleaseYear As Integer?
            Get
                Return m_ReleaseYear
            End Get
            Set(ByVal value As Integer?)
                'CheckReadOnly()
                m_ReleaseYear = value
                OnContainerDataChanged("ReleaseYear")
            End Set
        End Property

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return GetXmlNamespace().GetName("musicInfo")
            End Get
        End Property

        ''' <summary>
        ''' 返回歌曲的专辑、标题名称与曲目编号。
        ''' </summary>
        Public Overrides Function ToString() As String
            If m_Album Is Nothing AndAlso m_Track Is Nothing Then
                Return m_Title
            ElseIf m_Track Is Nothing Then
                Return String.Format(Prompts.MusicInfoA, m_Title, m_Album)
            Else
                Return String.Format(Prompts.MusicInfoAT, m_Title, m_Album, m_Track)
            End If
        End Function

        Public Overrides Function ToXDocument() As System.Xml.Linq.XDocument
            Dim doc = MyBase.ToXDocument()
            With doc.Root
                .Add(New XElement(XNArtists, From EachArtist In m_Artists
                                            Where EachArtist IsNot Nothing
                                            Select EachArtist.ToXElement))
                .Add(New XElement(XNGenres, From EachGenere In m_Genres
                                           Where EachGenere <> Nothing
                                           Select New XElement(XNGenre, EachGenere)))
                .SetElementValue(XNTrack, m_Track)
                .SetElementValue(XNReleaseYear, m_ReleaseYear)
                .SetElementValue(XNTitle, m_Title)
                .SetElementValue(XNAlbum, m_Album)
            End With
            Return doc
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XDocument)
            MyBase.New(dataSource)
            Dim tempArtists = Enumerable.Empty(Of ArtistBase)()
            Dim tempGenres = Enumerable.Empty(Of String)()
            If dataSource IsNot Nothing Then
                Dim body = dataSource.Root
                If body IsNot Nothing Then
                    With body
                        Dim baseElement As XElement
                        baseElement = .Element(XNArtists)
                        If baseElement IsNot Nothing Then
                            tempArtists = (From EachArtist In baseElement.Elements(XNArtist)
                                               Select DirectCast(New Artist(EachArtist), ArtistBase)).Concat(
                                               From EachAG In baseElement.Elements(XNArtistGroup)
                                               Select New ArtistGroup(EachAG))
                        End If
                        baseElement = .Element(XNGenres)
                        If baseElement IsNot Nothing Then
                            tempGenres = From EachGenre In baseElement.Elements(XNGenre)
                                       Select EachGenre.Value
                        End If
                        m_Track = CType(.Element(XNTrack), Integer?)
                        m_ReleaseYear = CType(.Element(XNReleaseYear), Integer?)
                        m_Title = CStr(.Element(XNTitle))
                        m_Album = CStr(.Element(XNAlbum))
                    End With
                End If
            End If
            m_Artists = New DataContainerCollection(Of ArtistBase)(tempArtists)
            m_Genres = New ObservableCollection(Of String)(tempGenres)
            ObserveCollection(m_Artists, True)
            ObserveCollection(m_Genres)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示 LyriX 中艺术家或是群组的基本信息。
    ''' </summary>
    Public MustInherit Class ArtistBase
        Inherits XElementContainer
        Implements IIdentifiable

        Private m_Id As Integer?,
            m_Jobs As ArtistJobs,
            m_CharacterName As String,
            m_Name As String

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return MusicInfo.XNArtist
            End Get
        End Property

        ''' <summary>
        ''' 艺术家的标识符。
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
        ''' 艺术家参与此歌曲的具体工作列表。
        ''' </summary>
        Public Property Jobs As ArtistJobs
            Get
                Return m_Jobs
            End Get
            Set(ByVal value As ArtistJobs)
                m_Jobs = value
                OnContainerDataChanged("Jobs")
            End Set
        End Property

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

        Public Overrides Function ToString() As String
            Return If(m_CharacterName Is Nothing, m_Name, String.Format(Prompts.ArtistC, m_Name, m_CharacterName))
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(MusicInfo.XNId, m_Id)
                .SetAttributeValue(MusicInfo.XNJobs, EnumToXSD(m_Jobs))
                .SetAttributeValue(MusicInfo.XNCharacterName, m_CharacterName)
                'Value = Nothing -> Exception
                .Value = If(m_Name, "")
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            If dataSource IsNot Nothing Then
                With dataSource
                    'NEVER DataSource.@id
                    'CType(Nothing, Long?) -> 0
                    m_Id = CType(.Attribute(MusicInfo.XNId), Integer?)
                    m_Jobs = CType(XSDToEnum(CStr(.Attribute(MusicInfo.XNJobs)), GetType(ArtistJobs)).GetValueOrDefault, ArtistJobs)
                    m_CharacterName = CStr(.Attribute(MusicInfo.XNCharacterName))
                    m_Name = .Value
                End With
            End If
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示 LyriX 中艺术家的信息。
    ''' </summary>
    Public Class Artist
        Inherits ArtistBase

        Private m_Sex As Sex?

        ''' <summary>
        ''' 艺术家的性别。
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">指定的值不为 <c>null</c> 且不属于 <see cref="Sex" /> 中的任何一项。</exception>
        Public Property Sex As Sex?
            Get
                Return m_Sex
            End Get
            Set(ByVal value As Sex?)
                m_Sex = value
                OnContainerDataChanged("Sex")
            End Set
        End Property

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim element = MyBase.ToXElement()
            With element
                .SetAttributeValue(MusicInfo.XNSex, If(m_Sex Is Nothing, Nothing, EnumToXSD(m_Sex.Value)))
            End With
            Return element
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            If dataSource IsNot Nothing Then
                With dataSource
                    m_Sex = CType(XSDToEnum(CStr(.Attribute(MusicInfo.XNSex)), GetType(Sex)), Sex?)
                End With
            End If
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示 LyriX 中艺术家群组的信息。
    ''' </summary>
    Public Class ArtistGroup
        Inherits ArtistBase

        Private m_ArtistIds As ObservableCollection(Of Integer)

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return MusicInfo.XNArtistGroup
            End Get
        End Property

        ''' <summary>
        ''' 此艺术家群组的成员标识符列表。
        ''' </summary>
        ''' <remarks>其中的“成员”可以指艺术家，也可指艺术家群组，但要求列出的成员必须全部参与音乐创作或演出。必要时可以移去乐队组合的部分成员。</remarks>
        Public ReadOnly Property ArtistIds As ObservableCollection(Of Integer)
            Get
                Return m_ArtistIds
            End Get
        End Property

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim element = MyBase.ToXElement
            With element
                .SetAttributeValue(MusicInfo.XNAIds, String.Join(" ", m_ArtistIds))
            End With
            Return element
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            Dim tempAIds = Enumerable.Empty(Of Integer)()
            If dataSource IsNot Nothing Then
                With dataSource
                    Dim BaseValue As String
                    BaseValue = CStr(.Attribute(MusicInfo.XNAIds))
                    If BaseValue IsNot Nothing Then
                        tempAIds = From EachItem In BaseValue.Split(DirectCast(Nothing, Char()),
                                                                  StringSplitOptions.RemoveEmptyEntries)
                                                              Select CInt(EachItem)
                    End If
                End With
            End If
            m_ArtistIds = New ObservableCollection(Of Integer)(tempAIds)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class
End Namespace