Imports System.ComponentModel

Class LyriXPackageEditor

    Private Sub UpdateStatistics() Handles Me.DataContextChanged, Me.ContainerDataChanged
        LinesCount.Text = CStr(DataSource.Descendants(Of Document.LineBase).Count)
    End Sub

    Private Sub Hyperlinks_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Select Case CStr(DirectCast(sender, FrameworkContentElement).Tag)
            Case "h"
                EditDataContainer(DataSource.Header)
            Case "i"
                EditDataContainer(DataSource.MusicInfo)
            Case "ly"
                EditDataContainer(DataSource.Lyrics)
            Case "lo"
                EditDataContainer(DataSource.LocalizedParts)
        End Select
        DirectCast(sender, Hyperlink).Foreground = Brushes.Chocolate
    End Sub

End Class
