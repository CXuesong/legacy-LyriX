Imports Microsoft.Win32
Imports System.Windows.Threading

Class MusicPlayerWindow
    Private WithEvents MusicTimer As New DispatcherTimer With {.Interval = TimeSpan.FromSeconds(0.02)}
    Private WithEvents LyricsPanel As New LyricsWindow
    Private PrevLyricsPath As String

    Private Sub MusicPlay() Handles PlayButton.Checked
        PlayButton.IsChecked = True
        PlayButtonImage.Source = DirectCast(FindResource("PauseImage"), ImageSource)
        MusicPlayer.Play()
        MusicTimer.Start()
    End Sub

    Private Sub MusicPause() Handles PlayButton.Unchecked
        PlayButton.IsChecked = False
        MusicPlayer.Pause()
        PlayButtonImage.Source = DirectCast(FindResource("PlayImage"), ImageSource)
        MusicTimer.Stop()
    End Sub

    Private Sub MusicStop() Handles StopButton.Click
        MusicPause()
        MusicPlayer.Stop()
        UpdatePosition()
    End Sub

    Private Sub MusicOpen(sourcePath As String, sourceTitle As String)
        MusicStop()
        MusicPlayer.Source = Nothing
        MusicPlayer.Source = New Uri(sourcePath)
        PlayButton.IsEnabled = False
        StopButton.IsEnabled = False
        Me.Title = sourceTitle
        '尝试查找歌词
        Dim dotPosition = sourcePath.LastIndexOf("."c)
        Dim temp As String
        If dotPosition >= 0 Then
            temp = sourcePath.Substring(0, dotPosition)
        Else
            temp = sourcePath
        End If
        LyricsOpen(temp & "." & Document.LyriXPackage.FileExt, False)
    End Sub

    Private Sub LyricsOpen(sourcePath As String, promptException As Boolean)
        Try
            RefreshLyrics.IsEnabled = False
            LyricsPanel.OpenDocument(sourcePath)
            LyricsButton.IsChecked = True
            RefreshLyrics.IsEnabled = True
            PrevLyricsPath = sourcePath
            LyricsPanel.SetMusicPosition(MusicPlayer.Position)
        Catch ex As Exception
            If promptException Then MsgBox(ex.ToString, vbExclamation)
        End Try
    End Sub

    Private Sub UpdatePosition()
        PositionSlider.Value = MusicPlayer.Position.TotalMilliseconds
        PositionLabel.Text = MusicPlayer.Position.ToString("h\:mm\:ss\.fff")
        LyricsPanel.SetMusicPosition(MusicPlayer.Position)
    End Sub

    Private Sub MoveLyricsPanel() Handles MyBase.LocationChanged, MyBase.SizeChanged
        '将歌词面板移动到窗口的下方
        LyricsPanel.Window.Left = Me.Left
        LyricsPanel.Window.Top = Me.Top + Me.ActualHeight
        LyricsPanel.Window.Width = Me.Width
    End Sub

    Private Sub MusicPlayer_MediaEnded(sender As Object, e As System.Windows.RoutedEventArgs) Handles MusicPlayer.MediaEnded
        '将指针移到起点
        MusicStop()
    End Sub

    Private Sub MuteButton_CheckedChanged(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MuteButton.Checked, MuteButton.Unchecked
        VolumeSlider.IsEnabled = Not MuteButton.IsChecked.GetValueOrDefault
    End Sub

    Private Sub MusicPlayer_MediaFailed(sender As Object, e As System.Windows.ExceptionRoutedEventArgs) Handles MusicPlayer.MediaFailed
        '报告异常
        MsgBox(e.ErrorException.ToString, MsgBoxStyle.Exclamation)
    End Sub

    Private Sub MusicPlayer_MediaOpened(sender As Object, e As System.Windows.RoutedEventArgs) Handles MusicPlayer.MediaOpened
        PlayButton.IsEnabled = True
        StopButton.IsEnabled = True
        MusicTitleLabel.Text = MusicPlayer.Source.LocalPath
        If MusicPlayer.NaturalDuration.HasTimeSpan Then
            PositionSlider.IsEnabled = True
            PositionSlider.Maximum = MusicPlayer.NaturalDuration.TimeSpan.TotalMilliseconds
            DurationLabel.Text = MusicPlayer.NaturalDuration.TimeSpan.ToString("g")
        Else
            PositionSlider.IsEnabled = False
            DurationLabel.Text = "N/A"
        End If
    End Sub

    Private Sub OpenButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OpenButton.Click
        Dim OFD As New OpenFileDialog With {.Filter = "所有文件|*.*", .Multiselect = False,
                                            .ValidateNames = True}
        If OFD.ShowDialog() Then
            MusicOpen(OFD.FileName, OFD.SafeFileName)
        End If
    End Sub

    Private Sub OpenLyrics_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OpenLyrics.Click
        Dim OFD As New OpenFileDialog With {.Filter = Document.LyriXPackage.FileFilter, .Multiselect = False,
                                      .ValidateNames = True}
        If OFD.ShowDialog() Then
            LyricsOpen(OFD.FileName, True)
        End If
    End Sub

    Private Sub MusicTimer_Tick(sender As Object, e As System.EventArgs) Handles MusicTimer.Tick
        UpdatePosition()
    End Sub

    Private Sub PositionSlider_ValueChanged(sender As System.Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles PositionSlider.ValueChanged
        MusicPlayer.Position = TimeSpan.FromMilliseconds(PositionSlider.Value)
        UpdatePosition()
    End Sub

    Private Sub LyricsButton_CheckedChanged(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles LyricsButton.Checked, LyricsButton.Unchecked
        LyricsPanel.Visible = LyricsButton.IsChecked.GetValueOrDefault
    End Sub

    Private Sub MusicPlayerWindow_Closed(sender As Object, e As System.EventArgs) Handles Me.Closed
        LyricsPanel.Dispose()
    End Sub

    Private Sub LyricsPanel_VisibleChanged(sender As Object, e As System.EventArgs) Handles LyricsPanel.VisibleChanged
        LyricsButton.IsChecked = LyricsPanel.Visible
    End Sub

    Private Sub Window_PreviewDrop(sender As System.Object, e As System.Windows.DragEventArgs) Handles MyBase.PreviewDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop, True) Then
            Try
                Dim FirstFile = CType(e.Data.GetData(DataFormats.FileDrop, True), String()).FirstOrDefault
                If FirstFile <> Nothing Then
                    MusicOpen(FirstFile, FirstFile)
                End If
            Catch ex As InvalidCastException
                MsgBox(ex.ToString, vbExclamation)
            End Try
        End If
    End Sub

    Private Sub RefreshLyrics_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles RefreshLyrics.Click
        LyricsOpen(PrevLyricsPath, True)
    End Sub
End Class