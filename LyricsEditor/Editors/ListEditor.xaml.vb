Imports System.ComponentModel
Imports System.Windows.Controls.Primitives

''' <summary>
''' 表示一个列表的编辑器。
''' </summary>
<DefaultEvent("ItemCreating")>
Friend Class ListEditor

    ''' <summary>
    ''' 标识 <see cref="View" /> 依赖项属性。
    ''' </summary>
    Public Shared ReadOnly ViewProperty As DependencyProperty =
        DependencyProperty.Register("View", GetType(Selector), GetType(ListEditor),
                                    New FrameworkPropertyMetadata(Sub(d, e) DirectCast(d, ListEditor).m_View = DirectCast(e.NewValue, Selector)))

    ''' <summary>
    ''' 标识 <see cref="View" /> 依赖项属性。
    ''' </summary>
    Public Shared ReadOnly ViewSourceProperty As DependencyProperty =
        DependencyProperty.Register("ViewSource", GetType(IList), GetType(ListEditor))

    ''' <summary>
    ''' 标识 <see cref="ItemCreating" /> 路由事件。
    ''' </summary>
    Public Shared ReadOnly ItemCreatingEvent As RoutedEvent =
        EventManager.RegisterRoutedEvent("ItemCreating", RoutingStrategy.Direct,
                                         GetType(ItemOperationEventHandler), GetType(ListEditor))

    ''' <summary>
    ''' 标识 <see cref="ItemEditing" /> 路由事件。
    ''' </summary>
    Public Shared ReadOnly ItemEditingEvent As RoutedEvent =
        EventManager.RegisterRoutedEvent("ItemEditing", RoutingStrategy.Direct,
                                         GetType(ItemOperationEventHandler), GetType(ListEditor))

    ''' <summary>
    ''' 标识 <see cref="ItemPasting" /> 路由事件。
    ''' </summary>
    Public Shared ReadOnly ItemPastingEvent As RoutedEvent =
        EventManager.RegisterRoutedEvent("ItemPasting", RoutingStrategy.Direct,
                                         GetType(ItemPastingEventHandler), GetType(ListEditor))

    Public Delegate Sub ItemOperationEventHandler(sender As Object, e As ItemOperationEventArgs)
    Public Delegate Sub ItemPastingEventHandler(sender As Object, e As ItemPastingEventArgs)

    ''' <summary>
    ''' 在需要创建项目时引发。
    ''' </summary>
    Public Custom Event ItemCreating As ItemOperationEventHandler
        AddHandler(value As ItemOperationEventHandler)
            Me.AddHandler(ItemCreatingEvent, value)
        End AddHandler
        RemoveHandler(value As ItemOperationEventHandler)
            Me.RemoveHandler(ItemCreatingEvent, value)
        End RemoveHandler
        RaiseEvent(sender As Object, e As ItemOperationEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    ''' <summary>
    ''' 在需要修改项目时引发。
    ''' </summary>
    Public Custom Event ItemEditing As ItemOperationEventHandler
        AddHandler(value As ItemOperationEventHandler)
            Me.AddHandler(ItemEditingEvent, value)
        End AddHandler
        RemoveHandler(value As ItemOperationEventHandler)
            Me.RemoveHandler(ItemEditingEvent, value)
        End RemoveHandler
        RaiseEvent(sender As Object, e As ItemOperationEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    ''' <summary>
    ''' 在粘贴项目时引发。
    ''' </summary>
    Public Custom Event ItemPasting As ItemPastingEventHandler
        AddHandler(value As ItemPastingEventHandler)
            Me.AddHandler(ItemPastingEvent, value)
        End AddHandler
        RemoveHandler(value As ItemPastingEventHandler)
            Me.RemoveHandler(ItemPastingEvent, value)
        End RemoveHandler
        RaiseEvent(sender As Object, e As ItemPastingEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    Private WithEvents m_View As Selector   '仅为监听事件

    ''' <summary>
    ''' 引发一个 <see cref="ItemCreating" /> 事件。
    ''' </summary>
    Protected Overridable Sub OnItemCreating(e As ItemOperationEventArgs)
        Debug.Assert(e.OriginalSource Is Me)
        Debug.Assert(e.RoutedEvent Is ItemCreatingEvent)
        RaiseEvent ItemCreating(Me, e)
    End Sub

    ''' <summary>
    ''' 引发一个 <see cref="ItemEditing" /> 事件。
    ''' </summary>
    Protected Overridable Sub OnItemEditing(e As ItemOperationEventArgs)
        Debug.Assert(e.OriginalSource Is Me)
        Debug.Assert(e.RoutedEvent Is ItemEditingEvent)
        RaiseEvent ItemEditing(Me, e)
    End Sub

    ''' <summary>
    ''' 引发一个 <see cref="ItemPasting" /> 事件。
    ''' </summary>
    Protected Overridable Sub OnItemPasting(e As ItemPastingEventArgs)
        Debug.Assert(e.OriginalSource Is Me)
        Debug.Assert(e.RoutedEvent Is ItemPastingEvent)
        RaiseEvent ItemPasting(Me, e)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    <Bindable(True)>
    Public Property View As ItemsControl
        Get
            Return CType(Me.GetValue(ViewProperty), ItemsControl)
        End Get
        Set(value As ItemsControl)
            Me.SetValue(ViewProperty, value)
        End Set
    End Property

    Private Sub EditItem() Handles EditButton.Click
        Dim item = SelectedItems(0)
        Dim index = ItemsSource.IndexOf(item)
        Dim re As New ItemOperationEventArgs(ItemEditingEvent, Me, item)
        OnItemEditing(re)
        If item IsNot re.Item Then
            '引用变化
            ItemsSource(index) = re.Item
        End If
    End Sub

    Private Sub AddItem() Handles AddButton.Click
        Dim re As New ItemOperationEventArgs(ItemCreatingEvent, Me, Nothing)
        OnItemCreating(re)
        If re.Item IsNot Nothing Then
            ItemsSource.Add(re.Item)
        End If
    End Sub

    Private Sub InsertItem() Handles InsertButton.Click
        Dim re As New ItemOperationEventArgs(ItemCreatingEvent, Me, Nothing)
        OnItemCreating(re)
        If re.Item IsNot Nothing Then
            ItemsSource.Insert(FirstSelectedIndex, re.Item)
        End If
    End Sub

    Private Function RemoveItems() As Boolean Handles RemoveButton.Click
        If MsgBox(String.Format(Prompts.RemoveItemPrompt,
                                String.Join(vbCrLf, SelectedItems.Cast(Of Object))),
                            MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            '集合在删除项目时会更改
            Do While SelectedItems.Count > 0
                ItemsSource.Remove(SelectedItems(0))
            Loop
            Return True
        End If
        Return False
    End Function

    Private Sub CopyItems(sender As Object, e As RoutedEventArgs) Handles CutButton.Click, CopyButton.Click
        Dim NewData As New ListEditorClipborad(SelectedItems.Cast(Of Object).ToArray,
                                                         ItemsSource.GetType,
                                                         sender Is CopyButton)
        If sender Is CutButton Then
            For Each EachItem In NewData.Items
                ItemsSource.Remove(SelectedItems(0))
            Next
        End If
        InternalClipboard.Data = NewData
        UpdateButtons()
    End Sub

    Private Sub PasteItems() Handles PasteButton.Click
        If TypeOf InternalClipboard.Data Is ListEditorClipborad Then
            Dim data = DirectCast(InternalClipboard.Data, ListEditorClipborad)
            If data.SourceType = ItemsSource.GetType Then
                If Not data.MakeCopy Then InternalClipboard.Clear() '移动
                Dim StartIndex = Math.Max(FirstSelectedIndex(), 0)
                For Each EachItem In data.Items.Reverse
                    Dim PasteItem = EachItem
                    If data.MakeCopy Then
                        Debug.Assert(TypeOf EachItem Is ICloneable)
                        PasteItem = DirectCast(EachItem, ICloneable).Clone
                    End If
                    Dim re As New ItemPastingEventArgs(ItemPastingEvent, Me, PasteItem, data.MakeCopy)
                    OnItemPasting(re)
                    If re.Item IsNot Nothing Then ItemsSource.Insert(StartIndex, re.Item)
                Next
            End If
        End If
        UpdateButtons()
    End Sub

    Private Sub MoveItem(FirstItemIndex As Integer, SelectionLength As Integer, NewIndex As Integer)
        'Eg.    A   B   [C   D]   E
        '       A   B   E
        If FirstItemIndex <> NewIndex Then
            '必须定型
            Dim MovingItems = Aggregate EI In Aggregate EachItem In ItemsSource
                              Skip FirstItemIndex Take SelectionLength
                              Into Reverse() Into ToList()
            For I = 1 To SelectionLength
                ItemsSource.RemoveAt(FirstItemIndex)
            Next
            For Each EachItem In MovingItems
                ItemsSource.Insert(NewIndex, EachItem)
            Next
        End If
    End Sub

    Private Function FirstSelectedIndex() As Integer
        If SelectedItems.Count = 0 Then Return -1
        Return Aggregate EachItem In SelectedItems
                   Select View.Items.IndexOf(EachItem)
                   Into Min()
    End Function

    ''' <summary>
    ''' 获取 View 中选择的项目。
    ''' </summary>
    Private ReadOnly Property SelectedItems() As IList
        Get
            If TypeOf View Is ListBox Then
                Return DirectCast(View, ListBox).SelectedItems
            ElseIf TypeOf View Is MultiSelector Then
                Return DirectCast(View, MultiSelector).SelectedItems
            Else
                Return New Object() {}
            End If
        End Get
    End Property

    ''' <summary>
    ''' 获取 View 中当前使用的视图的数据源（如果 View 的 ItemsSource 为视图，则需要设置）。
    ''' </summary>
    Public Property ViewSource As IList
        Get
            Return DirectCast(Me.GetValue(ViewSourceProperty), IList)
        End Get
        Set(value As IList)
            Me.SetValue(ViewSourceProperty, value)
        End Set
    End Property

    Private Shadows ReadOnly Property ItemsSource As IList
        Get
            If ViewSource IsNot Nothing Then
                Return ViewSource
            Else
                If View IsNot Nothing Then
                    If View.ItemsSource Is Nothing Then
                        Return View.Items
                    ElseIf TypeOf View.ItemsSource Is IList Then
                        Return DirectCast(View.ItemsSource, IList)
                    End If
                End If
                Return Nothing
            End If
        End Get
    End Property

    Private Sub m_View_MouseDoubleClick(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles m_View.MouseDoubleClick
        Dim Item = DirectCast(e.OriginalSource, DependencyObject).FindAncestor(Of ListBoxItem)()
        If Item IsNot Nothing Then
            EditItem()
        End If
    End Sub

    Private Sub UpdateButtons() Handles Me.Loaded, Me.GotFocus, m_View.SelectionChanged
        Dim items = ItemsSource
        If View IsNot Nothing AndAlso items IsNot Nothing Then
            Dim fsi = FirstSelectedIndex()
            If SelectedItems.Contains(CollectionView.NewItemPlaceholder) Then
                MoveUpButton.IsEnabled = False
                MoveDownButton.IsEnabled = False
                EditButton.IsEnabled = False
                RemoveButton.IsEnabled = False
                CutButton.IsEnabled = False
                CopyButton.IsEnabled = False
            Else
                InsertButton.IsEnabled = (fsi >= 0)
                EditButton.IsEnabled = (SelectedItems.Count = 1)
                RemoveButton.IsEnabled = (SelectedItems.Count > 0)
                SelectAllButton.IsEnabled = SelectedItems.Count < items.Count
                CutButton.IsEnabled = (SelectedItems.Count > 0)
                CopyButton.IsEnabled = (SelectedItems.Count > 0)
                PasteButton.IsEnabled = TypeOf InternalClipboard.Data Is ListEditorClipborad AndAlso
                    DirectCast(InternalClipboard.Data, ListEditorClipborad).SourceType = items.GetType
                If SelectedItems.Count > 0 Then
                    '序列包含完全相同的数据，但因为它们包含的对象具有不同的引用，该序列不会被视为相等。
                    Dim SelectionIsSequence = Aggregate EachItem In items
                                              Skip fsi
                                              Take SelectedItems.Count
                                              Into All(SelectedItems.Contains(EachItem))
                    MoveUpButton.IsEnabled = SelectionIsSequence AndAlso Not fsi <= 0
                    MoveDownButton.IsEnabled = SelectionIsSequence AndAlso Not SelectedItems.Contains(items(items.Count - 1))
                Else
                    MoveUpButton.IsEnabled = False
                    MoveDownButton.IsEnabled = False
                End If
            End If
        End If
    End Sub

    Private Sub MoveUpItems() Handles MoveUpButton.Click
        Dim PrevSelection = SelectedItems.Cast(Of Object).ToList
        MoveItem(FirstSelectedIndex, SelectedItems.Count, FirstSelectedIndex() - 1)
        SelectedItems.Clear()
        For Each EachItem In PrevSelection
            SelectedItems.Add(EachItem)
        Next
    End Sub

    Private Sub MoveDownItems() Handles MoveDownButton.Click
        Dim PrevSelection = SelectedItems.Cast(Of Object).ToList
        MoveItem(FirstSelectedIndex, SelectedItems.Count, FirstSelectedIndex() + 1)
        SelectedItems.Clear()
        For Each EachItem In PrevSelection
            SelectedItems.Add(EachItem)
        Next
    End Sub

    Private Sub SelectAllButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles SelectAllButton.Click
        If TypeOf View Is ListBox Then
            DirectCast(View, ListBox).SelectAll()
        ElseIf TypeOf View Is MultiSelector Then
            DirectCast(View, MultiSelector).SelectAll()
        End If
    End Sub

    Private Class ListEditorClipborad
        Private m_Items As Object()
        Private m_SourceType As Type
        Private m_MakeCopy As Boolean

        ''' <summary>
        ''' 指示是否应当构建 <see cref="Items" /> 中的副本。
        ''' </summary>
        Public ReadOnly Property MakeCopy As Boolean
            Get
                Return m_MakeCopy
            End Get
        End Property

        Public ReadOnly Property Items As Object()
            Get
                Return m_Items
            End Get
        End Property

        Public ReadOnly Property SourceType As Type
            Get
                Return m_SourceType
            End Get
        End Property

        Public Sub New(items As Object(), sourceType As Type, makeCopy As Boolean)
            Debug.Assert(items IsNot Nothing)
            Debug.Assert(sourceType IsNot Nothing)
            m_Items = items
            m_SourceType = sourceType
            m_MakeCopy = makeCopy
        End Sub
    End Class
End Class

Friend Class ItemOperationEventArgs
    Inherits RoutedEventArgs

    Private m_Item As Object

    ''' <summary>
    ''' 获取/设置要进行操作的项目实例。
    ''' </summary>
    Public Property Item As Object
        Get
            Return m_Item
        End Get
        Set(value As Object)
            m_Item = value
        End Set
    End Property

    Public Sub New(routedEvent As RoutedEvent, source As Object, item As Object)
        MyBase.New(routedEvent, source)
        m_Item = item
    End Sub
End Class

Friend Class ItemPastingEventArgs
    Inherits ItemOperationEventArgs

    Private m_IsCopy As Boolean

    ''' <summary>
    ''' 获取一个值，指示 <see cref="Item" /> 中的对象是否为剪切板内容的副本（复制）。
    ''' </summary>
    Public ReadOnly Property IsCopy As Boolean
        Get
            Return m_IsCopy
        End Get
    End Property

    Public Sub New(routedEvent As RoutedEvent, source As Object, item As Object, isCopy As Boolean)
        MyBase.New(routedEvent, source, item)
        m_IsCopy = isCopy
    End Sub
End Class