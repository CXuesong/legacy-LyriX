Imports <xmlns="LyriX/2011/package/localizedLyrics.xsd">
Imports LyriX.Document.ObjectModel
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Document
    ''' <summary>
    ''' 按照某一特定语言进行翻译的歌词。（LyriX/[.../]LocalizedLyrics.xml）
    ''' </summary>
    Public Class LocalizedLyrics
        Inherits XPackagePartContainer
        Implements ILocalizedDataContainer

        '元素名称（XName）
        Friend Shared ReadOnly XNLines As XName = GetXmlNamespace().GetName("lines"),
            XNLine As XName = GetXmlNamespace().GetName("line")
        '属性名称（XName）（AttributeFromDefault = unqualified，-> namespace = null）
        Friend Shared ReadOnly XNLId As XName = "lid"

        Private m_Lines As DataContainerCollection(Of LocalizedLine)

        ''' <summary>
        ''' 获取此歌词翻译中的行。
        ''' </summary>
        Public ReadOnly Property Lines As DataContainerCollection(Of LocalizedLine)
            Get
                Return m_Lines
            End Get
        End Property

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return GetXmlNamespace().GetName("localizedLyrics")
            End Get
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
                    Return Package.Lyrics
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="Lyrics" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        Public ReadOnly Property Source As Lyrics
            Get
                Return DirectCast(_Source, Lyrics)
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
                Throw New InvalidOperationException
            End Set
        End Property

        ''' <summary>
        ''' 从源同步本地化信息的子项。
        ''' </summary>
        ''' <param name="mode">同步模式。</param>
        ''' <remarks>此操作会同步歌词行列表。</remarks>
        ''' <exception cref="InvalidOperationException">进行同步时，<see cref="Source" /> 为 <c>null</c>。</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="mode" /> 不是有效的 <see cref="ChildrenSynchronizationMode" />。</exception>
        Public Sub SynchronizeChildren(mode As ObjectModel.ChildrenSynchronizationMode) Implements ObjectModel.ILocalizedDataContainer.SynchronizeChildren
            Dim src = Source
            If src Is Nothing Then
                Throw New InvalidOperationException
            Else
                If (mode And ChildrenSynchronizationMode.RemoveInvalid) = ChildrenSynchronizationMode.RemoveInvalid Then
                    Dim SrcDescend = src.Descendants(Of LineBase).ToList
                    Dim InvalidLines As New List(Of LocalizedLine)
                    For Each EL In m_Lines
                        Dim EachLine = EL
                        If EachLine.SourceId Is Nothing OrElse
                            Not Aggregate EachLine1 In SrcDescend
                                Where EachLine1.Id = EachLine.SourceId
                                Into Any() Then
                            InvalidLines.Add(EachLine)
                        End If
                    Next
                    For Each EachLine In InvalidLines
                        m_Lines.Remove(EachLine)
                    Next
                End If
                If (mode And ChildrenSynchronizationMode.AddNew) = ChildrenSynchronizationMode.AddNew Then
                    For Each EL In src.Descendants(Of LineBase)()
                        Dim EachLine = EL
                        If EachLine.Id IsNot Nothing AndAlso
                            Not Aggregate EachLine1 In m_Lines
                                Where EachLine1.SourceId = EachLine.Id
                                Into Any() Then
                            m_Lines.Add(New Document.LocalizedLine(EachLine.Id))
                        End If
                    Next
                End If
            End If
        End Sub

        ''' <summary>
        ''' 按顺序返回所有歌词翻译行的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Join(vbCrLf, m_Lines)
        End Function

        Public Overrides Function ToXDocument() As System.Xml.Linq.XDocument
            Dim doc = MyBase.ToXDocument()
            With doc.Root
                .Add(New XElement(XNLines, From EachLine In m_Lines
                                           Where EachLine IsNot Nothing
                                           Select EachLine.ToXElement))
            End With
            Return doc
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XDocument)
            MyBase.New(dataSource)
            Dim tempLines = Enumerable.Empty(Of LocalizedLine)()
            If dataSource IsNot Nothing Then
                Dim body = dataSource.Root
                If body IsNot Nothing Then
                    With body
                        Dim BaseElement As XElement
                        BaseElement = .Element(XNLines)
                        If BaseElement IsNot Nothing Then
                            tempLines = From EachLine In BaseElement.Elements(XNLine)
                              Select New LocalizedLine(EachLine)
                        End If
                    End With
                End If
            End If
            m_Lines = New DataContainerCollection(Of LocalizedLine)(tempLines)
            ObserveCollection(m_Lines, True)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示已翻译的歌词行。
    ''' </summary>
    Public Class LocalizedLine
        Inherits XElementContainer
        Implements ILocalizedDataContainer

        Private m_SourceId As Integer?,
            m_Text As String

        Protected Overrides ReadOnly Property RootName As System.Xml.Linq.XName
            Get
                Return LocalizedLyrics.XNLine
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
                    Return Aggregate EachItem In Package.Lyrics.Descendants(Of LineBase)()
                           Where EachItem.Id = m_SourceId
                           Into FirstOrDefault()
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="Line" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        Public ReadOnly Property Source As Line
            Get
                Return DirectCast(_Source, Line)
            End Get
        End Property

        ''' <summary>
        ''' 获取/设置本地化源的标识符。
        ''' </summary>
        Public Property SourceId As Integer? Implements ObjectModel.ILocalizedDataContainer.SourceId
            Get
                Return m_SourceId
            End Get
            Set(value As Integer?)
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
        ''' 此歌词行的内容。
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
        ''' 返回此歌词行的内容。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return Text
        End Function

        Public Overrides Function ToXElement() As System.Xml.Linq.XElement
            Dim Element = MyBase.ToXElement
            With Element
                .SetAttributeValue(LocalizedLyrics.XNLId, m_SourceId)
                .Value = If(m_Text, "")
            End With
            Return Element
        End Function

        ''' <summary>
        ''' 使用指定的行标识符进行初始化。
        ''' </summary>
        Public Sub New(ByVal lineId? As Integer)
            m_SourceId = lineId
        End Sub

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
            If dataSource IsNot Nothing Then
                With dataSource
                    m_SourceId = CType(.Attribute(LocalizedLyrics.XNLId), Integer?)
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