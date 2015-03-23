Imports System.ComponentModel
Imports System.Collections.ObjectModel

Namespace Document.ObjectModel

    ''' <summary>
    ''' 为 <see cref="DataContainer.ContainerDataChanged" /> 提供数据。
    ''' </summary>
    Public Class ContainerDataChangedEventArgs
        Inherits EventArgs
        Private m_Source As DataContainer
        Private m_PropertyName As String

        ''' <summary>
        ''' 获取属性值发生变化的对象。
        ''' </summary>
        Public ReadOnly Property Source As DataContainer
            Get
                Return m_Source
            End Get
        End Property

        ''' <summary>
        ''' 发生变化的属性名。
        ''' </summary>
        ''' <remarks>如果是此对象的子级列表发生变化，则一定为 <c>"Children"</c>。</remarks>
        Public ReadOnly Property PropertyName As String
            Get
                Return m_PropertyName
            End Get
        End Property

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        Public Sub New(source As DataContainer, propertyName As String)
            If source Is Nothing Then
                Throw New ArgumentNullException("source")
            Else
                m_Source = source
                m_PropertyName = propertyName
            End If
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个包含数据的对象。
    ''' </summary>
    ''' <remarks>在实现时，需要添加强类型化属性与相关方法。</remarks>
    Public MustInherit Class DataContainer
        Implements INotifyPropertyChanged
        Implements ICloneable

        ''' <summary>
        ''' 在此对象或是其子对象的属性值变化时发生。
        ''' </summary>
        ''' <remarks>此事件沿 <see cref="Parent" /> 构成的树状列表中冒泡引发。</remarks>
        Public Event ContainerDataChanged(sender As Object, e As ContainerDataChangedEventArgs)

        ''' <summary>
        ''' 基础结构。在更改属性值时发生。
        ''' </summary>
        Private Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Private m_Tag As Object
        Private m_Parent As DataContainer
        Private m_ObservedCollections As New List(Of Specialized.INotifyCollectionChanged)(2)
        Private m_ChildrenCollection As IEnumerable(Of DataContainer)

        ''' <summary>
        ''' 获取/设置此对象的附加数据。
        ''' </summary>
        ''' <remarks>请注意，附加数据不属于信息集；它们不持久保留，也不由 ToString 显示。</remarks>
        Public Property Tag As Object
            Get
                Return m_Tag
            End Get
            Set(value As Object)
                m_Tag = value
                OnContainerDataChanged("Tag")
            End Set
        End Property

        '========== 树状结构 ==========
        ''' <summary>
        ''' 获取此对象的父级。
        ''' </summary>
        ''' <value>如果有父级，则返回；否则返回 <c>null</c>。</value>
        Public ReadOnly Property Parent As DataContainer
            Get
                Return m_Parent
            End Get
        End Property

        ''' <summary>
        ''' 设置新的父级。
        ''' </summary>
        Private Sub SetParent(newParent As DataContainer)
            m_Parent = newParent
            OnContainerDataChanged("Parent")
        End Sub

        ''' <summary>
        ''' 设置此对象的父级。
        ''' </summary>
        Friend Sub Attach(newParent As DataContainer)
            If m_Parent Is newParent Then
                'Do Nothing
            ElseIf m_Parent IsNot Nothing Then
                Throw New InvalidOperationException(ExceptionPrompts.ParentAlreadyExists)
            Else
                SetParent(newParent)
            End If
        End Sub

        ''' <summary>
        ''' 将此对象与其父级断开连接。
        ''' </summary>
        Friend Sub Detach()
            If m_Parent IsNot Nothing Then SetParent(Nothing)
        End Sub

        ''' <summary>
        ''' 在树状结构中查找当前实例的指定类型的父级。
        ''' </summary>
        Public Function FindAncestor(Of T As DataContainer)() As T
            Dim CurrentContainer As DataContainer = Me
            Do
                CurrentContainer = CurrentContainer.Parent
                If TypeOf CurrentContainer Is T Then
                    Return DirectCast(CurrentContainer, T)
                End If
            Loop Until CurrentContainer Is Nothing
            Return Nothing
        End Function

        ''' <summary>
        ''' 获取此对象所在的包（如果有）。
        ''' </summary>
        Public Function FindPackage() As LyriXPackage
            Return FindAncestor(Of LyriXPackage)()
        End Function

        ''' <summary>
        ''' 获取此对象以下所有层次的子级。
        ''' </summary>
        Public Function Descendants() As IEnumerable(Of DataContainer)
            If Children.Any Then
                Return Children.Concat(
                    Children.SelectMany(Function(EachChild) EachChild.Descendants))
            Else
                Return Enumerable.Empty(Of DataContainer)()
            End If
        End Function

        ''' <summary>
        ''' 获取此对象以下所有层次中指定类型的子级。
        ''' </summary>
        Public Function Descendants(Of T As DataContainer)() As IEnumerable(Of T)
            Return Me.Descendants.OfType(Of T)()
        End Function

        ''' <summary>
        ''' 获取此实例的子级列表。
        ''' </summary>
        Public Overridable ReadOnly Property Children As IEnumerable(Of DataContainer)
            Get
                Return If(m_ChildrenCollection, Enumerable.Empty(Of DataContainer)())
            End Get
        End Property

        ''' <summary>
        ''' 引发 <see cref="PropertyChanged" />。
        ''' </summary>
        ''' <param name="propertyName">发生变化的属性名称，若为 <see cref="String.Empty" /> 或为 <c>null</c>，则表示该对象上的所有属性都已更改。</param>
        Protected Overridable Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
            If propertyName <> Nothing Then OnPropertyChanged(Nothing)
        End Sub

        ''' <summary>
        ''' 引发 <see cref="ContainerDataChanged" />。
        ''' </summary>
        ''' <param name="propertyName">发生变化的属性名称，若为 <see cref="String.Empty" /> 或为 <c>null</c>，则表示该对象上的所有属性都已更改。</param>
        Protected Sub OnContainerDataChanged(propertyName As String)
            OnContainerDataChanged(New ContainerDataChangedEventArgs(Me, propertyName))
        End Sub

        ''' <summary>
        ''' 引发 <see cref="ContainerDataChanged" />。
        ''' </summary>
        ''' <param name="e">事件的数据。</param>
        Protected Overridable Sub OnContainerDataChanged(e As ContainerDataChangedEventArgs)
            RaiseEvent ContainerDataChanged(Me, e)
            If m_Parent IsNot Nothing Then
                '开始冒泡
                m_Parent.OnContainerDataChanged(e)
            End If
            '显式接口实现
            If e.PropertyName <> "Children" Then OnPropertyChanged(e.PropertyName)
        End Sub

        ''' <summary>
        ''' 监视指定集合的变化，以便于引发相关事件。
        ''' </summary>
        ''' <param name="collection">要监视的集合。</param>
        ''' <param name="isChildrenCollection">指示此集合是否将作为此实例的直接子级。</param>
        Protected Sub ObserveCollection(collection As Specialized.INotifyCollectionChanged, Optional isChildrenCollection As Boolean = False)
            Debug.Assert(collection IsNot Nothing)
            If Not m_ObservedCollections.Contains(collection) Then
                If isChildrenCollection Then
                    '直接子级集合
                    Debug.Assert(m_ChildrenCollection Is Nothing)
                    Debug.Assert(TypeOf collection Is IEnumerable(Of ObjectModel.DataContainer))
                    m_ChildrenCollection = DirectCast(collection, IEnumerable(Of ObjectModel.DataContainer))
                    For Each EachItem In m_ChildrenCollection
                        '初始化
                        EachItem.Attach(Me)
                    Next
                    AddHandler collection.CollectionChanged,
                        Sub(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
                            If e.OldItems IsNot Nothing AndAlso e.Action <> Specialized.NotifyCollectionChangedAction.Move Then
                                '为旧项断开连接
                                For Each EachItem As DataContainer In e.OldItems
                                    Debug.Assert(EachItem.m_Parent Is Me)
                                    EachItem.Detach()
                                Next
                            End If
                            If e.NewItems IsNot Nothing AndAlso e.Action <> Specialized.NotifyCollectionChangedAction.Move Then
                                '为新项设置父级
                                For Each EachItem As DataContainer In e.NewItems
                                    EachItem.Attach(Me)
                                Next
                            End If
                            OnContainerDataChanged(New ContainerDataChangedEventArgs(Me, "Children"))
                        End Sub
                Else
                    AddHandler collection.CollectionChanged, Sub(sender, e) OnContainerDataChanged(New ContainerDataChangedEventArgs(Me, "Children"))
                End If
                m_ObservedCollections.Add(collection)
            End If
        End Sub

        ''' <summary>
        ''' 基础结构。
        ''' </summary>
        Private Function [_Clone]() As Object Implements System.ICloneable.Clone
            Return Me.Clone
        End Function

        ''' <summary>
        ''' 获取当前实例的深层副本。
        ''' </summary>
        Public Overridable Function Clone() As DataContainer
            Return DirectCast(Activator.CreateInstance(Me.GetType, Me), DataContainer)
        End Function

        Friend Sub New()

        End Sub
    End Class

    ''' <summary>
    ''' 表示一个 <see cref="DataContainer" /> 组成的集合。
    ''' </summary>
    ''' <remarks>此集合不允许 <c>null</c> 项。</remarks>
    Public Class DataContainerCollection(Of T As DataContainer)
        Inherits ObservableCollection(Of T)

        Protected Overrides Sub InsertItem(index As Integer, item As T)
            If item Is Nothing Then
                Throw New ArgumentNullException("item")
            Else
                MyBase.InsertItem(index, item)
            End If
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As T)
            If item Is Nothing Then
                Throw New ArgumentNullException("item")
            Else
                MyBase.SetItem(index, item)
            End If
        End Sub

        Friend Sub New(list As IList(Of T))
            MyBase.New(list)
        End Sub

        Friend Sub New(list As IEnumerable(Of T))
            MyBase.New(list)
        End Sub

        Friend Sub New()
            MyBase.New()
        End Sub
    End Class

    ''' <summary>
    ''' 表示此元素具有一个在某一范围内唯一的标识符。
    ''' </summary>
    Public Interface IIdentifiable

        ''' <summary>
        ''' 此元素的标识符。
        ''' </summary>
        Property Id As Integer?
    End Interface

    ''' <summary>
    ''' 表示在编译时可将同类其他元素的特性作为此元素的默认值。
    ''' </summary>
    Public Interface IReferable
        Inherits IIdentifiable

        ''' <summary>
        ''' 确定此对象的引用目标。指定目标的信息将作为此元素内容的默认值。
        ''' </summary>
        Property Reference As Integer?
    End Interface
End Namespace