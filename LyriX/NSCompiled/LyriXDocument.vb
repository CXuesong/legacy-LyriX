Namespace Compiled
    ''' <summary>
    ''' 表示一个编译过的歌词文档。
    ''' </summary>
    Public Class LyricsDocument
        Private m_Header As Header
        Private m_MusicInfo As MusicInfo
        Private m_Lyrics As Lyrics

        Public ReadOnly Property Header As Header
            Get
                Return m_Header
            End Get
        End Property

        ''' <summary>
        ''' 获取 LyriX 文档中的歌曲信息。
        ''' </summary>
        Public ReadOnly Property MusicInfo As MusicInfo
            Get
                Return m_MusicInfo
            End Get
        End Property

        ''' <summary>
        ''' 获取 LyriX 文档中的歌词内容信息。
        ''' </summary>
        Public ReadOnly Property Lyrics As Lyrics
            Get
                Return m_Lyrics
            End Get
        End Property

        Friend Sub SetHeader(value As Header)
            m_Header = value
        End Sub

        Friend Sub SetMusicInfo(value As MusicInfo)
            m_MusicInfo = value
        End Sub

        Friend Sub SetLyrics(value As Lyrics)
            m_Lyrics = value
        End Sub

        ''' <summary>
        ''' 初始化一个空壳儿。
        ''' </summary>
        Friend Sub New()

        End Sub
    End Class
End Namespace