Class LocalizedPackagePartsCollectionEditor
    Private Sub SynchronizeItems(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Select Case CStr(DirectCast(sender, FrameworkContentElement).Tag)
            Case "al"
                DataSource.SynchronizeChildren(Document.ObjectModel.ChildrenSynchronizationMode.All)
            Case "ad"
                DataSource.SynchronizeChildren(Document.ObjectModel.ChildrenSynchronizationMode.AddNew)
            Case "re"
                DataSource.SynchronizeChildren(Document.ObjectModel.ChildrenSynchronizationMode.RemoveInvalid)
        End Select
        DirectCast(sender, Hyperlink).Foreground = Brushes.Chocolate
    End Sub

    Private Sub LPPEditor_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles LPPEditor.ItemCreating
        Dim NewItem As New Document.LocalizedPackageParts()
        e.Item = NewItem
        EditDataContainer(NewItem)
        '等待 NewItem 被加入列表后执行同步
        Dispatcher.BeginInvoke(DirectCast(AddressOf NewItem.SynchronizeChildren, 
                               Action(Of Document.ObjectModel.ChildrenSynchronizationMode)),
                               Document.ObjectModel.ChildrenSynchronizationMode.AddNew)
    End Sub

    Private Sub LPPEditor_ItemEditing(sender As Object, e As ItemOperationEventArgs) Handles LPPEditor.ItemEditing
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub
End Class
