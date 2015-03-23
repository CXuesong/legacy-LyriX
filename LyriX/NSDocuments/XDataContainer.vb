Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports <xmlns:com="LyriX/2011/package/common.xsd">

Namespace Document.ObjectModel
    ''' <summary>
    ''' 表示一个包含 Xml 数据的对象。
    ''' </summary>
    ''' <remarks>在实现时，需要添加强类型化属性与相关方法。</remarks>
    Public MustInherit Class XDataContainer
        Inherits DataContainer

        '元素名称（XName）
        Friend Shared ReadOnly XNTags As XName = GetXmlNamespace(com).GetName("tags"),
            XNImports As XName = GetXmlNamespace(com).GetName("imports")
        '属性名称
        Friend Shared ReadOnly XNNamespace As XName = "ns"

        Private m_XTags As XTagsCollection

        ''' <summary>
        ''' 将此对象的内容写入 <see cref="XContainer" /> 并返回。
        ''' </summary>
        Public MustOverride Function ToXContainer() As XContainer

        ''' <summary>
        ''' 获取此对象在保存为 Xml 时附加的属性列表。
        ''' </summary>
        Public ReadOnly Property XTags As XTagsCollection
            Get
                Return m_XTags
            End Get
        End Property

        ''' <summary>
        ''' 将此对象的内容写入指定的 Xml 容器。
        ''' </summary>
        ''' <param name="parent">写入的目标</param>
        ''' <exception cref="ArgumentNullException"><paramref name="parent" /> 为 <c>null</c>。</exception>
        Public Sub Save(ByVal parent As XContainer)
            If parent Is Nothing Then
                Throw New ArgumentNullException("parent")
            Else
                parent.Add(ToXContainer)
            End If
        End Sub

        ''' <summary>
        ''' 通过使用 <see cref="ToXContainer" />，构造当前实例的深层副本。
        ''' </summary>
        Public Overrides Function Clone() As DataContainer
            Return DirectCast(Activator.CreateInstance(Me.GetType, Me.ToXContainer), DataContainer)
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Friend Sub New(ByVal dataSource As XContainer)
            Dim tempXTags = Enumerable.Empty(Of XAttribute)()
            If dataSource IsNot Nothing Then
                If dataSource.Document IsNot Nothing Then
                    '导入 Tag Namespace
                    Dim baseElement = dataSource.Document.Root.Element(XNTags)
                    If baseElement IsNot Nothing Then
                        Dim TagsNamespaces = baseElement.Annotation(Of List(Of XNamespace))()
                        If TagsNamespaces Is Nothing Then
                            '缓存导入的命名空间
                            TagsNamespaces = New List(Of XNamespace)
                            baseElement.AddAnnotation(TagsNamespaces)
                            For Each EachImports In baseElement.Elements(XNImports)
                                '不允许导入空命名空间，因为空命名空间被 LyriX 占用
                                Dim nsName = CStr(EachImports.Attribute(XNNamespace))
                                If nsName <> Nothing Then
                                    TagsNamespaces.Add(nsName)
                                End If
                            Next
                        End If
                        '导入 Tags
                        If TypeOf dataSource Is XDocument Then
                            baseElement = DirectCast(dataSource, XDocument).Root
                        ElseIf TypeOf dataSource Is XElement Then
                            baseElement = DirectCast(dataSource, XElement)
                        End If
                        'XNamespace 对象必须确保已原子化，
                        '即如果两个 XNamespace 对象具有完全相同的 URI，则这两个对象将共用同一实例。
                        tempXTags = From EachTag In baseElement.Attributes
                                Where TagsNamespaces.Contains(EachTag.Name.Namespace)
                    End If
                End If
            End If
            m_XTags = New XTagsCollection(tempXTags)
            ObserveCollection(m_XTags)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Friend Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个以 <see cref="XElement" /> 及其子级为数据的对象。
    ''' </summary>
    Public MustInherit Class XElementContainer
        Inherits XDataContainer

        ''' <summary>
        ''' 获取此 Xml 元素的完全限定名。
        ''' </summary>
        ''' <remarks>使用此属性有利于构造基础文档。</remarks>
        Protected MustOverride ReadOnly Property RootName As XName

        ''' <summary>
        ''' 将此对象的内容写入 <see cref="XElement" /> 并返回。
        ''' </summary>
        Public Overridable Function ToXElement() As XElement
            Dim element As New XElement(RootName)
            element.Add(XTags)
            Return element
        End Function

        ''' <summary>
        ''' 基础结构。将此对象的内容写入 <see cref="XContainer" /> 并返回。
        ''' </summary>
        ''' <remarks>此方法的实现返回 <see cref="ToXElement" /> 的值。为了在继承时减少歧义，故封闭了此方法的重写。</remarks>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public NotOverridable Overrides Function ToXContainer() As System.Xml.Linq.XContainer
            Return ToXElement()
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XElement)
            MyBase.New(dataSource)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个以 LyriX 包的部分的 Xml 文档作为数据来源的对象。
    ''' </summary>
    Public MustInherit Class XPackagePartContainer
        Inherits XDataContainer

        ''' <summary>
        ''' 获取此 Xml 文档的根元素的完全限定名。
        ''' </summary>
        ''' <remarks>使用此属性有利于构造基础文档。</remarks>
        Protected MustOverride ReadOnly Property RootName As XName

        ''' <summary>
        ''' 将此文档写入包的指定位置。
        ''' </summary>
        ''' <exception cref="ArgumentNullException"><paramref name="package" /> 为 <c>null</c>。</exception>
        Public Sub WritePackage(ByVal package As Package, ByVal uri As Uri)
            If package Is Nothing Then
                Throw New ArgumentNullException("package")
            Else
                package.DeletePart(uri)
                Using xs = package.CreatePart(uri, Net.Mime.MediaTypeNames.Text.Xml).GetStream
                    Me.ToXDocument.Save(xs, SaveOptions.OmitDuplicateNamespaces)
                End Using
            End If
        End Sub

        ''' <summary>
        ''' 将此对象的内容写入 <see cref="XDocument" /> 并返回。
        ''' </summary>
        ''' <remarks>默认实现：使用已有的信息构造一个 <see cref="XDocument" />，其编码为 UTF-8，使用 <see cref="RootName" /> 构造一个空的根元素，并附上所有的 <see cref="XTags" />。</remarks>
        Public Overridable Function ToXDocument() As XDocument
            Dim doc As New XDocument(New XDeclaration("1.0", "utf-8", "yes"), New XElement(RootName))
            '导入 XTags 命名空间
            Dim NamespaceIndex As Integer = 0
            Dim tagsNode As New XElement(XNTags)
            For Each EachNamespace In Me.Descendants(Of XDataContainer).SelectMany(Function(ec) ec.XTags).Select(Function(et) et.Name.Namespace).Distinct
                If EachNamespace <> XNamespace.None Then
                    tagsNode.Add(New XElement(XNImports,
                                              New XAttribute(XNNamespace, EachNamespace.NamespaceName)))
                    '导入命名空间，防止文件臃肿
                    doc.Root.Add(New XAttribute(XNamespace.Xmlns.GetName("t" & NamespaceIndex), EachNamespace))
                    NamespaceIndex += 1
                Else
                    Trace.TraceWarning("发现空命名空间的 XTag，不能被保存。")
                End If
            Next
            doc.Root.Add(XTags)
            doc.Root.Add(tagsNode)
            Return doc
        End Function

        ''' <summary>
        ''' 基础结构。将此对象的内容写入 <see cref="XContainer" /> 并返回。
        ''' </summary>
        ''' <remarks>此方法的实现返回 <see cref="ToXDocument" /> 的值。为了在继承时减少歧义，故封闭了此方法的重写。</remarks>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public NotOverridable Overrides Function ToXContainer() As System.Xml.Linq.XContainer
            Return ToXDocument()
        End Function

        ''' <summary>
        ''' 使用指定的数据源进行初始化。
        ''' </summary>
        ''' <param name="dataSource">包含初始化数据的数据源。如果为 <c>null</c>，则表示构造一个空的实例。</param>
        Public Sub New(ByVal dataSource As XDocument)
            MyBase.New(dataSource)
        End Sub

        ''' <summary>
        ''' 构造一个空的实例。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub
    End Class

    ''' <summary>
    ''' 表示在文档保存为 Xml 时附加的属性列表。
    ''' </summary>
    ''' <remarks>此集合不允许 <c>null</c> 项，且添加的属性必须有非空的命名空间。</remarks>
    Public Class XTagsCollection
        Inherits ObservableCollection(Of XAttribute)

        Protected Overrides Sub InsertItem(index As Integer, item As XAttribute)
            If item Is Nothing Then
                Throw New ArgumentNullException("item")
            ElseIf item.Name.Namespace = XNamespace.None Then
                Throw New ArgumentException("item")
            Else
                MyBase.InsertItem(index, item)
            End If
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As XAttribute)
            If item Is Nothing Then
                Throw New ArgumentNullException("item")
            ElseIf item.Name.Namespace = XNamespace.None Then
                Throw New ArgumentException("item")
            Else
                MyBase.SetItem(index, item)
            End If
        End Sub

        ''' <summary>
        ''' 按指定 Xml 名称获取此对象在保存为 Xml 时附加的属性。
        ''' </summary>
        ''' <exception cref="ArgumentNullException"><paramref name="name" /> 为 <c>null</c>。</exception>
        Public Function GetAttribute(name As XName) As XAttribute
            If name Is Nothing Then
                Throw New ArgumentNullException
            Else
                Return Aggregate EachTag In MyBase.Items
                       Where EachTag IsNot Nothing AndAlso EachTag.Name = name
                       Into FirstOrDefault()
            End If
        End Function

        ''' <summary>
        ''' 按指定 Xml 名称设置此对象在保存为 Xml 时附加的属性。
        ''' </summary>
        ''' <remarks>将 <paramref name="value" /> 设置为 <c>null</c> 会导致移除属性。</remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="name" /> 为 <c>null</c>。</exception>
        Public Sub SetAttributeValue(name As XName, value As Object)
            If name Is Nothing Then
                Throw New ArgumentNullException
            Else
                For I = 0 To MyBase.Items.Count - 1
                    Dim eachItem = MyBase.Items(I)
                    If eachItem.Name = name Then
                        If value Is Nothing Then
                            MyBase.Items.RemoveAt(I)
                            OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Remove, eachItem))
                        Else
                            MyBase.Items(I).SetValue(value)
                        End If
                        Return
                    End If
                Next
                If value IsNot Nothing Then
                    Dim newItem As New XAttribute(name, value)
                    MyBase.Items.Add(newItem)
                    OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(Specialized.NotifyCollectionChangedAction.Add, newItem))
                End If
            End If
        End Sub

        Friend Sub New(list As IList(Of XAttribute))
            MyBase.New(list)
        End Sub

        Friend Sub New(list As IEnumerable(Of XAttribute))
            MyBase.New(list)
        End Sub

        Friend Sub New()
            MyBase.New()
        End Sub
    End Class
End Namespace