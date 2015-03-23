Imports LyriX.Compiled
Imports LyriX.Compiled.ObjectModel
Imports System.Collections.ObjectModel

Namespace Compiled
    ''' <summary>
    ''' 表示一个经过编译的音乐信息。
    ''' </summary>
    Public Class MusicInfo
        Inherits CompiledDocumentPart
        '本地缓存
        Private m_Album As MultiLanguageValue(Of String) = MultiLanguageValue(Of String).Empty
        Private m_Title As MultiLanguageValue(Of String) = MultiLanguageValue(Of String).Empty
        Private m_ReleaseYear As Integer?
        Private m_Track As Integer?
        Private m_s_Genres As IList(Of String) = {}
        Private m_s_Artists As IList(Of ArtistBase) = {}

        ''' <summary>
        ''' 专辑/电影名称。
        ''' </summary>
        Public Property Album As MultiLanguageValue(Of String)
            Get
                Return m_Album
            End Get
            Friend Set(ByVal value As MultiLanguageValue(Of String))
                m_Album = value.AsSealed
            End Set
        End Property

        ''' <summary>
        ''' 标题。
        ''' </summary>
        Public Property Title As MultiLanguageValue(Of String)
            Get
                Return m_Title
            End Get
            Friend Set(ByVal value As MultiLanguageValue(Of String))
                m_Title = value.AsSealed
            End Set
        End Property

        ''' <summary>
        ''' 发行时间（年份）。
        ''' </summary>
        Public Property ReleaseYear As Integer?
            Get
                Return m_ReleaseYear
            End Get
            Friend Set(ByVal value As Integer?)
                m_ReleaseYear = value
            End Set
        End Property

        ''' <summary>
        ''' 曲目编号。
        ''' </summary>
        Public Property Track As Integer?
            Get
                Return m_Track
            End Get
            Friend Set(ByVal value As Integer?)
                m_Track = value
            End Set
        End Property

        ''' <summary>
        ''' 流派。
        ''' </summary>
        Public Property Genres As IList(Of String)
            Get
                Return m_s_Genres
            End Get
            Friend Set(ByVal value As IList(Of String))
                m_s_Genres = If(value Is Nothing, Nothing, New ReadOnlyCollection(Of String)(value))
            End Set
        End Property

        ''' <summary>
        ''' 参与创作的艺术家。
        ''' </summary>
        Public Property Artists As IList(Of ArtistBase)
            Get
                Return m_s_Artists
            End Get
            Friend Set(ByVal value As IList(Of ArtistBase))
                m_s_Artists = If(value Is Nothing, Nothing, New ReadOnlyCollection(Of ArtistBase)(value))
            End Set
        End Property

        ''' <summary>
        ''' 返回歌曲的专辑、标题名称与曲目编号。
        ''' </summary>
        Public Overrides Function ToString() As String
            Debug.Assert(m_Album IsNot Nothing)
            If m_Album.IsEmpty AndAlso m_Track Is Nothing Then
                Return String.Format(Prompts.MusicInfoIL, m_Title)
            ElseIf m_Track Is Nothing Then
                Return String.Format(Prompts.MusicInfoILA, m_Title, m_Album)
            Else
                Return String.Format(Prompts.MusicInfoILAT, m_Title, m_Album, m_Track)
            End If
        End Function

        Friend Sub New(document As LyricsDocument)
            MyBase.New(document)
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个经过编译的艺术家或群组的信息。
    ''' </summary>
    Public MustInherit Class ArtistBase
        Inherits CompiledElement
        '本地缓存
        Private m_Id As Integer?
        Private m_Jobs As ArtistJobs?
        Private m_Name As MultiLanguageValue(Of String) = MultiLanguageValue(Of String).Empty
        Private m_CharacterName As MultiLanguageValue(Of String) = MultiLanguageValue(Of String).Empty

        ''' <summary>
        ''' 艺术家的标识。
        ''' </summary>
        Public Property Id As Integer?
            Get
                Return m_Id
            End Get
            Friend Set(ByVal value As Integer?)
                m_Id = value
            End Set
        End Property

        ''' <summary>
        ''' 艺术家参与此歌曲的具体工作列表。
        ''' </summary>
        Public Property Jobs As ArtistJobs?
            Get
                Return m_Jobs
            End Get
            Friend Set(ByVal value As ArtistJobs?)
                m_Jobs = value
            End Set
        End Property

        ''' <summary>
        ''' 艺术家在此歌曲中的角色名称（如果有）。
        ''' </summary>
        Public Property CharacterName As MultiLanguageValue(Of String)
            Get
                Return m_CharacterName
            End Get
            Friend Set(ByVal value As MultiLanguageValue(Of String))
                m_CharacterName = value.AsSealed
            End Set
        End Property

        ''' <summary>
        ''' 艺术家的名称。
        ''' </summary>
        Public Property Name As MultiLanguageValue(Of String)
            Get
                Return m_Name
            End Get
            Friend Set(ByVal value As MultiLanguageValue(Of String))
                m_Name = value.AsSealed
            End Set
        End Property

        ''' <summary>
        ''' 返回此艺术家的名称与角色名称。
        ''' </summary>
        Public Overrides Function ToString() As String
            Debug.Assert(m_CharacterName IsNot Nothing)
            If m_CharacterName.IsEmpty Then
                Return CStr(m_Name)
            Else
                Return String.Format(Prompts.ArtistC, m_Name, m_CharacterName)
            End If
        End Function

        Friend Sub New(parent As MusicInfo)
            MyBase.New(parent)
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个经过编译的艺术家信息。
    ''' </summary>
    Public NotInheritable Class Artist
        Inherits ArtistBase

        '本地缓存
        Private m_Sex As Sex?

        ''' <summary>
        ''' 艺术家的性别，用于后期进一步确定歌词演唱者的详情以便处理歌词。
        ''' </summary>
        Public Property Sex As Sex?
            Get
                Return m_Sex
            End Get
            Friend Set(ByVal value As Sex?)
                m_Sex = value
            End Set
        End Property

        Friend Sub New(parent As MusicInfo)
            MyBase.New(parent)
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个经过编译的艺术家群组信息。
    ''' </summary>
    Public NotInheritable Class ArtistGroup
        Inherits ArtistBase

        '本地缓存
        Private m_s_Artists As IList(Of ArtistBase) = {}

        ''' <summary>
        ''' 此艺术家群组的成员列表。
        ''' </summary>
        ''' <remarks>其中的“成员”可以指艺术家，也可指艺术家群组，但要求列出的成员必须全部参与音乐创作或演出。必要时可以移去乐队组合的部分成员。</remarks>
        Public Property Artists As IList(Of ArtistBase)
            Get
                Return m_s_Artists
            End Get
            Friend Set(ByVal value As IList(Of ArtistBase))
                m_s_Artists = If(value Is Nothing, Nothing, New ReadOnlyCollection(Of ArtistBase)(value))
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return MyBase.ToString() & If(m_s_Artists Is Nothing, Nothing, "[" & JoinList(m_s_Artists) & "]")
        End Function

        Friend Sub New(parent As MusicInfo)
            MyBase.New(parent)
        End Sub
    End Class
End Namespace
