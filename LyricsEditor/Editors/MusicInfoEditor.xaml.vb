Class MusicInfoEditor

    Private Sub ArtistEditor_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles ArtistEditor.ItemCreating
        Dim NewArtistMenu = DirectCast(Me.FindResource("NewArtistMenu"), ContextMenu)
        NewArtistMenu.PlacementTarget = ArtistEditor
        NewArtistMenu.IsOpen = True
        Do While NewArtistMenu.IsOpen
            DoEvents()
        Loop
        Dim NewItem As Document.ArtistBase = Nothing
        For Each EachItem In NewArtistMenu.Items.OfType(Of MenuItem)()
            If EachItem.IsChecked Then
                EachItem.IsChecked = False
                Select Case CStr(EachItem.Tag)
                    Case "a"
                        NewItem = New Document.Artist
                    Case "g"
                        NewItem = New Document.ArtistGroup
                    Case Else
                        Continue For
                End Select
                Exit For
            End If
        Next
        If NewItem IsNot Nothing Then
            NewItem.Id = DataSource.ArtistIndexManager.NewIndex
            e.Item = NewItem
            EditDataContainer(NewItem)
        End If
    End Sub

    Private Sub ArtistEditor_ItemEditing(sender As Object, e As ItemOperationEventArgs) Handles ArtistEditor.ItemEditing
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub ArtistEditor_ItemPasting(sender As Object, e As ItemPastingEventArgs) Handles ArtistEditor.ItemPasting
        If e.IsCopy Then DirectCast(e.Item, Document.ArtistBase).Id = DataSource.ArtistIndexManager.NewIndex
    End Sub

    Private Sub GenreEditor_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles GenreEditor.ItemCreating
        Dim GenreDialog As New GenreDialog With {.Owner = Me.FindAncestor(Of Window)()}
        If GenreDialog.ShowDialog() Then
            e.Item = GenreDialog.Genre
        End If
    End Sub

    Private Sub GenreEditor_ItemEditing(sender As Object, e As ItemOperationEventArgs) Handles GenreEditor.ItemEditing
        Dim GenreDialog As New GenreDialog With {.Owner = Me.FindAncestor(Of Window)(),
                                                 .Genre = CStr(e.Item)}
        If GenreDialog.ShowDialog() Then
            e.Item = GenreDialog.Genre
        End If
    End Sub
End Class
