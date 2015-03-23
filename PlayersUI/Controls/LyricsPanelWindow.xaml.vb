Imports System.Windows.Threading

Namespace Controls
    Class LyricsPanelWindow
        Const CurrentLinePosition = 0.5
        Const AutoScrollDelta = 0.5         '自动滚动的步长

        Private WithEvents m_CurrentPlayer As Players.SingleTrackLyricsPlayer
        Private ScrollLineRect As Rect
        Private m_AutoScroll As Boolean

        '优化性能
        Private Sub ScrollH(offset As Double)
            If Math.Abs(LyricsViewer.HorizontalOffset - offset) >= AutoScrollDelta Then
                LyricsViewer.ScrollToHorizontalOffset(offset)
            End If
        End Sub

        Private Sub ScrollV(offset As Double)
            If Math.Abs(LyricsViewer.VerticalOffset - offset) >= AutoScrollDelta Then
                LyricsViewer.ScrollToVerticalOffset(offset)
            End If
        End Sub

        Private Sub ApplyAlignments() Handles TextAlignmentLeftButton.Checked, TextAlignmentCenterButton.Checked, TextAlignmentRightButton.Checked, TextAlignmentJustifyButton.Checked, LineAlignmentRightButton.Checked, LineAlignmentCenterButton.Checked, LineAlignmentLeftButton.Checked
            '应用对齐按钮的作用
            If Not Me.IsLoaded Then Return
            '阻止在 InitializeComponment 时因部分按钮被选中而引发的调用
            '此时 LyricsPanel 为 null
            If TextAlignmentLeftButton.IsChecked Then
                LyricsPanel.TextAlignment = TextAlignment.Left
            ElseIf TextAlignmentCenterButton.IsChecked Then
                LyricsPanel.TextAlignment = TextAlignment.Center
            ElseIf TextAlignmentRightButton.IsChecked Then
                LyricsPanel.TextAlignment = TextAlignment.Right
            ElseIf TextAlignmentJustifyButton.IsChecked Then
                LyricsPanel.TextAlignment = TextAlignment.Justify
            End If
            If LyricsPanel.Orientation = Orientation.Vertical Then
                LyricsPanel.VerticalContentAlignment = Windows.VerticalAlignment.Center
                If LineAlignmentLeftButton.IsChecked Then
                    LyricsPanel.HorizontalContentAlignment = Windows.HorizontalAlignment.Left
                ElseIf LineAlignmentCenterButton.IsChecked Then
                    LyricsPanel.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
                ElseIf LineAlignmentRightButton.IsChecked Then
                    LyricsPanel.HorizontalContentAlignment = Windows.HorizontalAlignment.Right
                End If
            Else
                LyricsPanel.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
                If LineAlignmentLeftButton.IsChecked Then
                    LyricsPanel.VerticalContentAlignment = Windows.VerticalAlignment.Top
                ElseIf LineAlignmentCenterButton.IsChecked Then
                    LyricsPanel.VerticalContentAlignment = Windows.VerticalAlignment.Center
                ElseIf LineAlignmentRightButton.IsChecked Then
                    LyricsPanel.VerticalContentAlignment = Windows.VerticalAlignment.Bottom
                End If
            End If
            'LyricsPanel.HorizontalAlignment = LyricsPanel.HorizontalContentAlignment
            'LyricsPanel.VerticalAlignment = LyricsPanel.VerticalContentAlignment
        End Sub

        Private Sub ApplyOrientation() Handles OrientationButton.Click
            If Not Me.IsLoaded Then Return
            '阻止在 InitializeComponment 时因部分按钮被选中而引发的调用
            '此时 LyricsPanel 为 null
            If LyricsPanel.Orientation = Orientation.Horizontal Then
                ApplyOrientation(Orientation.Vertical)
            Else
                ApplyOrientation(Orientation.Horizontal)
            End If
        End Sub

        Private Sub ApplyOrientation(newOrientation As Orientation)
            If Not Me.IsLoaded Then Return
            '阻止在 InitializeComponment 时因部分按钮被选中而引发的调用
            '此时 LyricsPanel 为 null
            LyricsPanel.Orientation = newOrientation
            DirectCast(Me.FindResource("ToolBarDirectionTransform"), RotateTransform).Angle =
                If(newOrientation = Orientation.Horizontal, 90, 0)
            If newOrientation = Orientation.Vertical Then
                LyricsViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                LyricsViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            Else
                LyricsViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                LyricsViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            End If
            LyricsViewer.UpdateLayout() '只有在确定没有其他挂起的无效操作时才应这样做
            UpdateLineRect()
        End Sub

        Private Sub ApplyMisc() Handles AutoScrollButton.Checked, TopMostButton.Checked, AutoScrollButton.Unchecked, TopMostButton.Unchecked
            If Not Me.IsLoaded Then Return
            '阻止在 InitializeComponment 时因部分按钮被选中而引发的调用
            '此时 LyricsPanel 为 null
            Me.Topmost = TopMostButton.IsChecked.GetValueOrDefault
            m_AutoScroll = AutoScrollButton.IsChecked.GetValueOrDefault
            ScrollLyrics()
        End Sub

        Private Sub UpdateLineRect()
            Dim LineIndex = m_CurrentPlayer.LineIndex
            If LineIndex.CurrentExists Then
                '以当前行为中心
                ScrollLineRect = LyricsPanel.GetLineRect(LineIndex.Current)
            ElseIf LineIndex.NextExists Then
                '以上一行与下一行的间隙为中心
                Dim PrevLineRect = If(LineIndex.PreviousExists, LyricsPanel.GetLineRect(LineIndex.Previous), New Rect)
                Dim NextLineRect = LyricsPanel.GetLineRect(LineIndex.Next)
                If LyricsPanel.Orientation = Orientation.Vertical Then
                    'TODO Argument Out of Range Exception
                    'Prayer.lrcx
                    ScrollLineRect = New Rect(NextLineRect.Left, PrevLineRect.Bottom, NextLineRect.Width, NextLineRect.Top - PrevLineRect.Bottom)
                Else
                    ScrollLineRect = New Rect(PrevLineRect.Right, NextLineRect.Top, NextLineRect.Left - PrevLineRect.Right, NextLineRect.Height)
                End If
            Else
                ScrollLineRect = Rect.Empty
            End If
        End Sub

        Private Sub ScrollLyrics(Optional forceScroll As Boolean = False)
            If (forceScroll OrElse m_AutoScroll) AndAlso
                Not ScrollLineRect.IsEmpty Then
                Dim LineIndex = m_CurrentPlayer.LineIndex
                Dim StepProgress = If(LineIndex.CurrentExists,
                                     m_CurrentPlayer.LineProgress,
                                     m_CurrentPlayer.StepProgress)
                If LyricsPanel.Orientation = Orientation.Vertical Then
                    ScrollV(-LyricsViewer.ActualHeight * CurrentLinePosition + ScrollLineRect.Top + ScrollLineRect.Height * StepProgress)
                Else
                    ScrollH(-LyricsViewer.ActualWidth * CurrentLinePosition + ScrollLineRect.Left + ScrollLineRect.Width * StepProgress)
                End If
            End If
        End Sub

        Private Sub lyricsPanel_PlayerChanged(sender As System.Object, e As System.EventArgs) Handles LyricsPanel.PlayerChanged
            m_CurrentPlayer = TryCast(LyricsPanel.Player, Players.SingleTrackLyricsPlayer)
        End Sub

        Private Sub m_CurrentPlayer_CurrentSegmentChanged(sender As Object, e As CurrentSegmentChangedEventArgs) Handles m_CurrentPlayer.CurrentSegmentChanged
            '虽然可以选择不滚动，但需要获取足够的信息，以便可以立即开始自动滚动
            If e.OldLineIndex <> m_CurrentPlayer.LineIndex Then
                UpdateLineRect()
                ScrollLyrics()
            End If
        End Sub

        Private Sub m_CurrentPlayer_PositionChanged(sender As Object, e As ObjectModel.PositionChangedEventArgs) Handles m_CurrentPlayer.PositionChanged
            '自动滚动
            ScrollLyrics()
        End Sub

        Private Sub LyricsPanelWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
            ApplyOrientation(Orientation.Vertical)
            ApplyAlignments()
            ApplyMisc()
        End Sub

        Private Sub LyricsViewer_SizeChanged(sender As Object, e As System.Windows.SizeChangedEventArgs) Handles LyricsViewer.SizeChanged
            '自动滚动
            ScrollLyrics()
        End Sub

        Private Sub m_CurrentPlayer_VersionChanged(sender As Object, e As System.EventArgs) Handles m_CurrentPlayer.VersionChanged
            DoEvents()
            UpdateLineRect()
            ScrollLyrics()
        End Sub
    End Class
End Namespace
