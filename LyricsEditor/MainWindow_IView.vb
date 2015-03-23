Imports LyriX.Document
Imports DocumentViewModel

Partial Class LyriXPackageView
    Implements IDocumentView

    Private disposedValue As Boolean ' 检测冗余的调用
    Private WithEvents CurrentDocument As LyriXPackageDocument
    Private OutlineDictionary As New Dictionary(Of Document.ObjectModel.DataContainer, TreeViewItem)

    Private Event ViewClosed(sender As Object, e As System.EventArgs) Implements DocumentViewModel.IDocumentView.Closed
    Private Event ViewClosing(sender As Object, e As System.ComponentModel.CancelEventArgs) Implements DocumentViewModel.IDocumentView.Closing

    Private Sub DocumentCommands_CanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub DocumentCommands_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        If e.Command Is ApplicationCommands.[New] Then
            DM.NewDocument()
        ElseIf e.Command Is ApplicationCommands.Open Then
            DM.OpenDocument()
        ElseIf e.Command Is ApplicationCommands.Save Then
            CurrentDocument.Save()
        ElseIf e.Command Is ApplicationCommands.SaveAs Then
            CurrentDocument.Save(True)
        ElseIf e.Command Is ApplicationCommands.Close Then
            CurrentDocument.Close()
        ElseIf e.Command Is EditorCommands.CheckDocument Then
            Dim cp As New Compilers.LyriXCompiler
            cp.Compile(CurrentDocument.Package)
            If cp.Output.Count = 0 Then
                MsgBox(Prompts.CheckDocumentOver, MsgBoxStyle.Information)
            Else
                MsgBox(String.Format(
                       Prompts.CheckDocumentPrompt,
                       String.Join(vbCrLf,
                                   From EachOutput In cp.Output Select ES = "- " & EachOutput.ToString)),
                           MsgBoxStyle.Information)
            End If
        ElseIf e.Command Is EditorCommands.NewWindow Then
            DM.CreateView(CurrentDocument)
        End If
    End Sub

    Public Sub Attach(newDocument As DocumentViewModel.Document) Implements DocumentViewModel.IDocumentView.Attach
        CurrentDocument = DirectCast(newDocument, LyriXPackageDocument)
    End Sub

    Public Function Detach() As DocumentViewModel.Document Implements DocumentViewModel.IDocumentView.Detach
        Dim PrevDocument = CurrentDocument
        CurrentDocument = Nothing
        Me.Close()
        Return PrevDocument
    End Function

    Public Function GetDocument() As DocumentViewModel.Document Implements DocumentViewModel.IDocumentView.GetDocument
        Return CurrentDocument
    End Function

    Public Sub Preview() Implements DocumentViewModel.IDocumentView.Preview
        Me.Activate()
    End Sub

    Public Sub SetFoucs() Implements DocumentViewModel.IDocumentView.SetFoucs
        Me.Activate()
    End Sub

    Public Sub Update(context As DocumentViewModel.UpdateContext) Implements DocumentViewModel.IDocumentView.Update
        Me.Title = CurrentDocument.ToString
        PackageOutline.Items.Clear()
        OutlineDictionary.Clear()
        PackageOutline.Items.Add(ConstructOutline(CurrentDocument.Package))
        EditContainer(CurrentDocument.Package)
    End Sub

    ''' <summary>
    ''' 从树状列表中移除指定项及其子项。
    ''' </summary>
    Private Sub RemoveContainerOutline(container As Document.ObjectModel.DataContainer)
        Debug.Assert(container IsNot Nothing)
        Dim OutlineItem As TreeViewItem = Nothing
        If OutlineDictionary.TryGetValue(container, OutlineItem) Then
            OutlineDictionary.Remove(container)
            Dim outlineParent = TryCast(OutlineItem.Parent, TreeViewItem)
            outlineParent.Items.Remove(OutlineItem)
        End If
        For Each EachItem In container.Children
            RemoveContainerOutline(EachItem)
        Next
    End Sub

    ''' <summary>
    ''' 由 DataContainer 获取其在树状列表中的项目。
    ''' </summary>
    Private Function GetContainerOutline(container As Document.ObjectModel.DataContainer) As TreeViewItem
        Dim item As TreeViewItem = Nothing
        Debug.Assert(container IsNot Nothing)
        OutlineDictionary.TryGetValue(container, item)
        Return item
    End Function

    ''' <summary>
    ''' 构造指定 DataContainer 的结构树。方向：隧道（从上到下）。
    ''' </summary>
    ''' <param name="forceConstruct">一定要构造项，而不是根据情况对某些类型的 DataContainer 返回 <c>null</c>。</param>
    Private Function ConstructOutline(container As ObjectModel.DataContainer, Optional parent As TreeViewItem = Nothing,
                                      Optional index As Integer = -1, Optional forceConstruct As Boolean = False) As TreeViewItem
        'WARNING 此处不能使用 HierarchicalDataTemplate，
        '因为在项目更新时，将列表绑定到 ListViewItem 的 ItemsSource 属性上会造成当前所选项来回移动
        Debug.Assert(container IsNot Nothing)
        Debug.Assert(TypeOf container.Tag Is LyriXDataContainerTag)
        '添加本项
        If Not forceConstruct AndAlso
            (TypeOf container Is Document.Span OrElse
            TypeOf container Is Document.LocalizedArtist OrElse
            TypeOf container Is Document.LocalizedLine) Then
            Return Nothing
        End If
        Dim Item As New TreeViewItem With {.Header = container}
        If TypeOf container Is Document.LyriXPackage OrElse
            TypeOf container Is Document.MusicInfo OrElse
            TypeOf container Is Document.Lyrics OrElse
            TypeOf container Is Document.Version OrElse
            TypeOf container Is Document.LocalizedPackagePartsCollection Then
            Item.IsExpanded = True
        End If
        If parent IsNot Nothing Then
            If index >= 0 Then
                parent.Items.Insert(index, Item)
            Else
                parent.Items.Add(Item)
            End If
        End If
        OutlineDictionary.Add(container, Item)
        '添加子项
        For Each EachItem In container.Children
            ConstructOutline(EachItem, Item)
        Next
        If TypeOf container.Children Is Specialized.INotifyCollectionChanged Then
            '监听 NotifyCollectionChanged 事件，以便重构树状列表。
            AddHandler DirectCast(container.Children, Specialized.INotifyCollectionChanged).CollectionChanged,
                Sub(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
                    Select Case e.Action
                        Case Specialized.NotifyCollectionChangedAction.Add
                            For Each EachItem As ObjectModel.DataContainer In e.NewItems
                                ConstructOutline(EachItem, Item, e.NewStartingIndex)
                            Next
                        Case Specialized.NotifyCollectionChangedAction.Remove
                            For Each EachItem As ObjectModel.DataContainer In e.OldItems
                                RemoveContainerOutline(EachItem)
                            Next
                        Case Specialized.NotifyCollectionChangedAction.Reset
                            '集合发生显著更改
                            Item.Items.Clear()
                            For Each EachItem As ObjectModel.DataContainer In container.Children
                                ConstructOutline(EachItem, Item)
                            Next
                        Case Specialized.NotifyCollectionChangedAction.Replace
                            Debug.Assert(e.OldItems.Count = e.NewItems.Count)
                            Dim I As Integer = 0
                            For Each EachItem As ObjectModel.DataContainer In e.OldItems
                                RemoveContainerOutline(EachItem)
                                ConstructOutline(EachItem, Item, e.NewStartingIndex)
                                I += 1
                            Next
                    End Select
                End Sub
        End If
        Return Item
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.Close()
            End If
            '释放非托管资源(非托管对象)并重写下面的 Finalize()。
            '将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Private Sub MainWindow_Closed(sender As Object, e As System.EventArgs) Handles Me.Closed
        RaiseEvent ViewClosed(sender, e)
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        CommitEditorChanges()
        RaiseEvent ViewClosing(sender, e)
    End Sub

    Private Sub CurrentDocument_ModifiedChanged(sender As Object, e As System.EventArgs) Handles CurrentDocument.ModifiedChanged
        Me.Title = CurrentDocument.ToString
    End Sub

    '提交编辑器焦点控件正在进行的修改
    Private Sub CommitEditorChanges()
        If TypeOf EditorFrame.Content Is DependencyObject Then
            Dim FocusedElement = FocusManager.GetFocusedElement(Me)
            If FocusedElement IsNot Nothing Then
                FocusedElement.RaiseEvent(New RoutedEventArgs(UIElement.LostFocusEvent))
            End If
        End If
    End Sub

    Private Sub CurrentDocument_Saving(sender As Object, e As System.EventArgs) Handles CurrentDocument.Saving
        CommitEditorChanges()
        If CurrentDocument.IsModified Then
            CurrentDocument.Package.Header.Revision += 1
        End If
    End Sub
End Class