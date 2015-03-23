Class LocalizedPackagePartsEditor

    Private Sub Hyperlinks_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Select Case CStr(DirectCast(sender, FrameworkContentElement).Tag)
            Case "i"
                EditDataContainer(DataSource.MusicInfo)
            Case "l"
                EditDataContainer(DataSource.Lyrics)
            Case "al"
                DataSource.SynchronizeChildren(Document.ObjectModel.ChildrenSynchronizationMode.All)
            Case "ad"
                DataSource.SynchronizeChildren(Document.ObjectModel.ChildrenSynchronizationMode.AddNew)
            Case "re"
                DataSource.SynchronizeChildren(Document.ObjectModel.ChildrenSynchronizationMode.RemoveInvalid)
        End Select
        DirectCast(sender, Hyperlink).Foreground = Brushes.Chocolate
    End Sub
End Class
