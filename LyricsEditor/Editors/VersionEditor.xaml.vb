Public Class VersionEditor

    Private Sub TrackEditor_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles TrackEditor.ItemCreating
        e.Item = New Document.Track
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub TrackEditor_ItemEditing(sender As Object, e As ItemOperationEventArgs) Handles TrackEditor.ItemEditing
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub AdjustTimeLink_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles AdjustTimeLink.Click
        EditDataContainer(DataSource, GetType(VersionTimeAdjuster))
    End Sub
End Class
