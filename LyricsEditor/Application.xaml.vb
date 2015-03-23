Imports DocumentViewModel.Managers


Class Application
    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup
        DM = New DocumentManager
        DM.AddDocumentType(GetType(LyriXPackageDocument))
        DocumentManager.EnableVisualStyles()
#If DEBUG Then
        DM.OpenDocument(New DocumentOpenArguments(New DocumentViewModel.FileIOContext("E:\My Files\Visual Studio 2010\Projects\LyriX\plot\依然在一起.lrcx")))
        DoEvents()
        'With DirectCast(Me.MainWindow, LyriXPackageView)
        '    .EditContainer(DirectCast(.GetDocument, LyriXPackageDocument).Package.Lyrics.Versions(0), GetType(VersionTimeAdjuster))
        'End With
#Else
        '新建默认文挡
        If DM.OpenDocument().Count = 0 Then
            DM.NewDocument()
        End If
#End If
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As RoutedEventArgs)
        FindAncestor(Of TextBox)(DirectCast(e.Source, DependencyObject)).DataContext = Nothing
    End Sub
End Class
