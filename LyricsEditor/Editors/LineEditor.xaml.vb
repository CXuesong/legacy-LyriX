Class LineEditor
    Private IsUpdatingValues As Boolean

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
        ArtistsList.Items.Filter = Function(EachArtist)
                                       Dim Id = DirectCast(EachArtist, Document.ArtistBase).Id
                                       Return Id IsNot Nothing AndAlso
                                           Not DirectCast(DataSource.Parent, Document.Track).ArtistIds.Contains(Id.Value)
                                   End Function
    End Sub

    Private Sub SpanEditor_ItemCreating(sender As System.Object, e As LyriX.LyricsEditor.ItemOperationEventArgs) Handles SpanEditor.ItemCreating
        e.Item = New Document.Span(DataContext.Document.Package.Lyrics.SpanIndexManager.NewIndex)
    End Sub

    Private Sub SegmentEditor_ItemCreating(sender As Object, e As ItemOperationEventArgs) Handles SegmentEditor.ItemCreating
        e.Item = New Document.Segment
    End Sub

    Private Sub SpanEditor_ItemPasting(sender As Object, e As ItemPastingEventArgs) Handles SpanEditor.ItemPasting
        If e.IsCopy Then DirectCast(e.Item, Document.Span).Id = DataContext.Document.Package.Lyrics.SpanIndexManager.NewIndex
    End Sub

    Private Sub MergeSpanButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MergeSpanButton.Click
        Dim src = SpanGrid.SelectedItems.OfType(Of Document.Span)().ToList
        Dim index = Aggregate EachItem In src
                    Select DataSource.Spans.IndexOf(EachItem)
                    Into Min()
        Dim dest = MergeSpans(src)
        DataSource.Spans.Insert(index, dest)
        For Each EachItem In src
            DataSource.Spans.Remove(EachItem)
        Next
        SpanGrid.UnselectAll()
        SpanGrid.SelectedItems.Add(dest)
    End Sub

    Private Sub SpanSplitLocator_ApplySplit()
        Dim index = SpanGrid.SelectedIndex
        If SpanSplitLocator.SelectionStart > 0 AndAlso SpanSplitLocator.SelectionStart < Len(SpanSplitLocator.Text) Then
            '防止划出空段
            Dim src = DirectCast(SpanGrid.SelectedItem, Document.Span)
            Dim dest = SplitSpan(src, SpanSplitLocator.SelectionStart, DataContext.Document.Package.Lyrics.SpanIndexManager.NewIndex, True)
            DataSource.Spans.Insert(index, dest(1))
            DataSource.Spans.Insert(index, dest(0))
            DataSource.Spans.Remove(src)
            SpanGrid.UnselectAll()
            SpanGrid.SelectedItems.Add(dest(1))
        End If
    End Sub

    Private Sub SpanSplitLocator_PreviewKeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles SpanSplitLocator.PreviewKeyDown
        If e.Key = Key.Enter Then
            SpanSplitLocator_ApplySplit()
            e.Handled = True
        End If
    End Sub

    Private Sub SpanSplitLocator_PreviewMouseLeftButtonUp(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles SpanSplitLocator.PreviewMouseLeftButtonUp
        SpanSplitLocator_ApplySplit()
        Mouse.Capture(Nothing)
    End Sub

    Private Sub UpdateButtons() Handles Me.Loaded, SpanGrid.SelectionChanged, SplitSpanButton.Checked, SplitSpanButton.Unchecked
        If SpanGrid.SelectedItems.Contains(CollectionView.NewItemPlaceholder) Then
            MergeSpanButton.IsEnabled = False
            SplitSpanButton.IsEnabled = False
            SpanSplitLocator.Visibility = Windows.Visibility.Collapsed
        Else
            MergeSpanButton.IsEnabled = (SpanGrid.SelectedItems.Count > 1)
            SplitSpanButton.IsEnabled = (SpanGrid.SelectedItems.Count = 1)
            SpanSplitLocator.Visibility = If(SplitSpanButton.IsEnabled AndAlso SplitSpanButton.IsChecked, Visibility.Visible, Visibility.Collapsed)
        End If
    End Sub
End Class
