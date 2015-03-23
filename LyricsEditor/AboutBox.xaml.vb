Class AboutBox

    Private Sub Window_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        ProductNameLabel.Text = My.Application.Info.ProductName
        VersionLabel.Text = My.Application.Info.Version.ToString
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        Me.DialogResult = True
    End Sub
End Class
