Class LyricsEditor

    Private Sub VersionList_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles VersionList.ItemCreating
        e.Item = New Document.Version
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub VersionList_ItemEditing(sender As Object, e As ItemOperationEventArgs) Handles VersionList.ItemEditing
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub QuickAlphabeticLink_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles QuickAlphabeticLink.Click
        EditDataContainer(GetType(LyricsQuickAlphabetic))
    End Sub
End Class
