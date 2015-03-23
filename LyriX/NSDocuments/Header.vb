Imports <xmlns="LyriX/2011/package/header.xsd">
Imports LyriX.Document.ObjectModel

Namespace Document
    ''' <summary>
    ''' 表示 LyriX 包的基本信息。（LyriX/header.xml）
    ''' </summary>
    Public Class Header
        Inherits XPackagePartContainer

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return GetXmlNamespace().GetName("header")
            End Get
        End Property

        '元素名称（XName）
        Friend Shared ReadOnly XNHead As XName = GetXmlNamespace().GetName("head"),
            XNApplication As XName = GetXmlNamespace().GetName("application"),
            XNAuthor As XName = GetXmlNamespace().GetName("author"),
            XNRevision As XName = GetXmlNamespace().GetName("revision"),
            XNComments As XName = GetXmlNamespace().GetName("comments"),
            XNLanguage As XName = GetXmlNamespace().GetName("language")
        '属性名称
        Private Shared ReadOnly XNVersion As XName = "version",
            XNContact As XName = "contact"

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
            Set(ByVal value As String)
                m_ApplicationName = value
                OnContainerDataChanged("ApplicationName")
            End Set
        End Property

        '''<summary>
        '''创建此部分的应用程序版本。
        '''</summary>
        Public Property ApplicationVersion As String
            Get
                Return m_ApplicationVersion
            End Get
            Set(ByVal value As String)
                m_ApplicationVersion = value
                OnContainerDataChanged("ApplicationVersion")
            End Set
        End Property

        '''<summary>
        '''作者的姓名。
        '''</summary>
        Public Property AuthorName As String
            Get
                Return m_AuthorName
            End Get
            Set(ByVal value As String)
                m_AuthorName = value
                OnContainerDataChanged("AuthorName")
            End Set
        End Property

        '''<summary>
        '''作者的联系信息。
        '''</summary>
        Public Property AuthorContact As String
            Get
                Return m_AuthorContact
            End Get
            Set(ByVal value As String)
                m_AuthorContact = value
                OnContainerDataChanged("AuthorContact")
            End Set
        End Property

        '''<summary>
        '''此文件的修订次数。
        '''</summary>
        Public Property Revision As Integer
            Get
                Return m_Revision
            End Get
            Set(ByVal value As Integer)
                m_Revision = value
                OnContainerDataChanged("Revision")
            End Set
        End Property

        '''<summary>
        '''此文件作者留下的注释。
        '''</summary>
        Public Property Comments As String
            Get
                Return m_Comments
            End Get
            Set(ByVal value As String)
                m_Comments = value
                OnContainerDataChanged("Comments")
            End Set
        End Property

        '''<summary>
        '''此文档使用的默认语言。用于确定歌词与歌词信息的语言默认值。
        '''</summary>
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(ByVal value As String)
                m_Language = value
                OnContainerDataChanged("Language")
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(", ", m_Language, m_ApplicationName, m_ApplicationVersion, m_AuthorName, m_AuthorContact)
        End Function

        Public Overrides Function ToXDocument() As System.Xml.Linq.XDocument
            Dim doc = MyBase.ToXDocument
            Dim Package = Me.FindPackage
            With doc.Root
                Dim baseElement As XElement
                If m_ApplicationName IsNot Nothing Then
                    baseElement = New XElement(XNApplication, m_ApplicationName)
                    baseElement.SetAttributeValue(XNVersion, m_ApplicationVersion)
                    .Add(baseElement)
                End If
                If m_AuthorName IsNot Nothing Then
                    baseElement = New XElement(XNAuthor, m_AuthorName)
                    baseElement.SetAttributeValue(XNContact, m_AuthorContact)
                    .Add(baseElement)
                End If
                .SetElementValue(XNRevision, m_Revision)
                .SetElementValue(XNComments, m_Comments)
                .SetElementValue(XNLanguage, m_Language)
            End With
            Return doc
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XDocument)
            MyBase.New(dataSource)
            If dataSource IsNot Nothing Then
                Dim body = dataSource.Root
                If body IsNot Nothing Then
                    With body
                        Dim baseElement As XElement
                        baseElement = .Element(XNApplication)
                        If baseElement IsNot Nothing Then
                            m_ApplicationName = CStr(baseElement)
                            m_ApplicationVersion = CStr(baseElement.Attribute(XNVersion))
                        End If
                        baseElement = .Element(XNAuthor)
                        If baseElement IsNot Nothing Then
                            m_AuthorName = CStr(baseElement)
                            m_AuthorContact = CStr(baseElement.Attribute(XNContact))
                        End If
                        m_Revision = CType(.Element(XNRevision), Integer?).GetValueOrDefault
                        m_Comments = CStr(.Element(XNComments))
                        m_Language = CStr(.Element(XNLanguage))

                    End With
                End If
            End If
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class
End Namespace