Public Class TrackEditor
    Private IsUpdatingValues As Boolean

    Private Sub LineEditor_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles LineEditor.ItemCreating
        e.Item = New Document.Line With {.Id = DataContext.Document.Package.Lyrics.LineIndexManager.NewIndex}
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub LineEditor_ItemEditing(sender As Object, e As ItemOperationEventArgs) Handles LineEditor.ItemEditing
        EditDataContainer(DirectCast(e.Item, Document.ObjectModel.DataContainer))
    End Sub

    Private Sub LineEditor_ItemPasting(sender As Object, e As ItemPastingEventArgs) Handles LineEditor.ItemPasting
        If e.IsCopy Then DirectCast(e.Item, Document.LineBase).Id = DataContext.Document.Package.Lyrics.LineIndexManager.NewIndex
    End Sub

    Private Sub TrackEditor_ContainerDataChanged(sender As Object, e As Document.ObjectModel.ContainerDataChangedEventArgs) Handles Me.ContainerDataChanged
        If e.Source Is DataSource Then
            LoadValues()
        End If
    End Sub

    Private Sub LoadValues() Handles Me.DataContextChanged, Me.Loaded
        If IsUpdatingValues OrElse Not IsLoaded Then Return
        IsUpdatingValues = True
        ArtistsList.UnselectAll()
        For Each EachArtist As Document.ArtistBase In ArtistsList.Items
            Debug.Assert(EachArtist.Id IsNot Nothing)
            If DataSource.ArtistIds.Contains(EachArtist.Id.Value) Then
                ArtistsList.SelectedItems.Add(EachArtist)
            End If
        Next
        IsUpdatingValues = False
    End Sub

    Private Sub SaveValues() Handles ArtistsList.LostFocus
        If IsUpdatingValues OrElse Not IsLoaded Then Return
        IsUpdatingValues = True
        DataSource.ArtistIds.Clear()
        For Each EachArtist As Document.ArtistBase In ArtistsList.SelectedItems
            Debug.Assert(EachArtist.Id IsNot Nothing)
            DataSource.ArtistIds.Add(EachArtist.Id.Value)
        Next
        IsUpdatingValues = False
    End Sub

    Private Sub LineEditor_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        '仅显示有 Id 且未被父级包括的艺术家
        ArtistsList.Items.Filter = Function(EachArtist) DirectCast(EachArtist, Document.ArtistBase).Id IsNot Nothing
    End Sub

    Private Sub AddLinesLink_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles AddLinesLink.Click
        EditDataContainer(Me.DataSource, GetType(TrackLinesGenerator))
    End Sub
End Class
