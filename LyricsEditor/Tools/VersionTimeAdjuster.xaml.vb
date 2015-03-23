Class VersionTimeAdjuster
    Private MusicSrc As String

    Private Sub UpdateButtons()
        OKButton.IsEnabled = MusicSrc <> Nothing AndAlso (LineList.SelectedItems.Count > 0)
    End Sub

    Private Sub DataContainerEditor_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        LineList.ItemsSource = DataSource.Descendants(Of Document.LineBase)()
        LineList.Items.Filter = Function(item) TrackList.SelectedItems.Contains(DirectCast(item, Document.LineBase).Parent)
        MusicSrc = CStr(DataSource.XTags.GetAttribute(XNMusicSource))
        MusicSrcText.Text = MusicSrc
        TrackList.SelectAll()
    End Sub

    Private Sub TrackList_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles TrackList.SelectionChanged
        LineList.Items.Filter = LineList.Items.Filter
        LineList.SelectAll()
    End Sub

    Private Sub BrowseMusicButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BrowseMusicButton.Click
        Dim OFD As New Microsoft.Win32.OpenFileDialog With {.CheckFileExists = True}
        If OFD.ShowDialog Then
            MusicSrc = OFD.FileName
            MusicSrcText.Text = MusicSrc
        End If
        UpdateButtons()
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        '保存设置
        DataSource.XTags.SetAttributeValue(XNMusicSource, MusicSrc)
        Dim Page2 As New VersionTimeAdjuster2 With {.Lines = LineList.SelectedItems.Cast(Of Document.Line).ToList,
                                                    .MusicSource = MusicSrc,
                                                    .SetVersionDuration = ApplyDurationCheckbox.IsChecked.GetValueOrDefault}
        EditDataContainer(Page2)
    End Sub

    Private Sub LineList_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles LineList.SelectionChanged
        UpdateButtons()
    End Sub
End Class
