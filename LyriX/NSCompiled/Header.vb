Imports LyriX.Compiled.ObjectModel
Imports System.Collections.ObjectModel

Namespace Compiled
    ''' <summary>
    ''' 表示一个已经经过编译的 LyriX 文档的基本信息。
    ''' </summary>
    Public NotInheritable Class Header
        Inherits CompiledDocumentPart

        '本地缓存
        Private m_ApplicationName As String,
            m_ApplicationVersion As String,
            m_AuthorName As String,
            m_AuthorContact As String,
            m_Revision As Integer,
            m_Comments As String,
            m_Language As String

        '''<summary>
        '''创建此部分的应用程序名称。
        '''</summary>
        Public Property ApplicationName As String
            Get
                Return m_ApplicationName
            End Get
            Friend Set(ByVal value As String)
                m_ApplicationName = value
            End Set
        End Property

        '''<summary>
        '''创建此部分的应用程序版本。
        '''</summary>
        Public Property ApplicationVersion As String
            Get
                Return m_ApplicationVersion
            End Get
            Friend Set(ByVal value As String)
                m_ApplicationVersion = value
            End Set
        End Property

        '''<summary>
        '''作者的姓名。
        '''</summary>
        Public Property AuthorName As String
            Get
                Return m_AuthorName
            End Get
            Friend Set(ByVal value As String)
                m_AuthorName = value
            End Set
        End Property

        '''<summary>
        '''作者的联系信息。
        '''</summary>
        Public Property AuthorContact As String
            Get
                Return m_AuthorContact
            End Get
            Friend Set(ByVal value As String)
                m_AuthorContact = value
            End Set
        End Property

        '''<summary>
        '''此文件的修订次数。
        '''</summary>
        Public Property Revision As Integer
            Get
                Return m_Revision
            End Get
            Friend Set(ByVal value As Integer)
                m_Revision = value
            End Set
        End Property

        '''<summary>
        '''此文件作者留下的注释。
        '''</summary>
        Public Property Comments As String
            Get
                Return m_Comments
            End Get
            Friend Set(ByVal value As String)
                m_Comments = value
            End Set
        End Property

        '''<summary>
        '''此文档使用的默认语言。用于确定歌词与歌词信息的语言默认值。
        '''</summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Friend Set(ByVal value As String)
                m_Language = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return JoinList(m_Language, m_ApplicationName, m_ApplicationVersion, m_AuthorName, m_AuthorContact)
        End Function

        Friend Sub New(document As LyricsDocument)
            MyBase.New(document)
        End Sub
    End Class
End Namespace