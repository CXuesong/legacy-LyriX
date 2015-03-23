Class TrackLinesGenerator

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        For Each EL In LinesTextBox.Text.Split({vbCrLf}, If(RemoveBlankLines.IsChecked,
                                                            StringSplitOptions.RemoveEmptyEntries,
                                                            StringSplitOptions.None))
            Dim EachLine = EL
            If TrimLines.IsChecked Then
                EachLine = EL.Trim
                '移除只有空格的行
                If RemoveBlankLines.IsChecked AndAlso EachLine = Nothing Then Continue For
            End If
            Dim NewSpan As New Document.Span(DocumentUtility.SplitSegments(EachLine))
            Dim NewLine As New Document.Line(DataContext.Document.Package.Lyrics.LineIndexManager.NewIndex, {NewSpan})
            DataSource.Lines.Add(NewLine)
        Next
        Me.GoBack()
    End Sub

    Private Sub DataContainerEditor_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        LinesTextBox.SelectAll()
        LinesTextBox.Focus()
    End Sub
End Class
