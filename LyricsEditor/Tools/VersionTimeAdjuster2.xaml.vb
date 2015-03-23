Imports LyriX.Document
Imports System.Windows.Threading
Imports System.ComponentModel

Class VersionTimeAdjuster2
    Private WithEvents MusicTimer As New DispatcherTimer With {.Interval = TimeSpan.FromSeconds(0.02)}
    Private m_Lines As IList(Of Line)
    Private m_MusicSource As String
    Private m_CurrentSpan As Span
    Private m_SetVersionDuration As Boolean

    Private AdjustingItems As List(Of AdjusterListItem)

    Private NotInheritable Class AdjusterListItem
        Implements INotifyPropertyChanged

        Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Private m_SelectedSpanIndex As Integer
        Private m_Line As Line

        Public ReadOnly Property Line As Line
            Get
                Return m_Line
            End Get
        End Property

        Public Property SelectedSpanIndex As Integer
            Get
                Return m_SelectedSpanIndex
            End Get
            Set(value As Integer)
                m_SelectedSpanIndex = value
                OnPropertyChanged("SelectedSpanIndex")
            End Set
        End Property

        Protected Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Public Sub New(line As Line)
            Debug.Assert(line IsNot Nothing)
            m_Line = line
            m_SelectedSpanIndex = -1
        End Sub
    End Class

    Public Property Lines() As IList(Of Line)
        Get
            Return m_Lines
        End Get
        Set(value As IList(Of Line))
            m_Lines = value
        End Set
    End Property

    Public Property SetVersionDuration() As Boolean
        Get
            Return m_SetVersionDuration
        End Get
        Set(value As Boolean)
            m_SetVersionDuration = value
        End Set
    End Property

    Private Property CurrentSpan As Span
        Get
            Return m_CurrentSpan
        End Get
        Set(value As Span)
            If value Is Nothing Then
                m_CurrentSpan = value
            Else
                If value IsNot m_CurrentSpan Then
                    If m_CurrentSpan IsNot Nothing Then
                        Dim OldItemContainer = AdjustingSpansListOf(DirectCast(m_CurrentSpan.Parent, Line))
                        OldItemContainer.UnselectAll()
                    End If
                    m_CurrentSpan = value
                    Dim ai = AdjustingItemOf(DirectCast(value.Parent, Line))
                    Debug.Assert(ai IsNot Nothing)
                    ai.SelectedSpanIndex = ai.Line.Spans.IndexOf(value)
                    '移动焦点
                    Dim ItemContainer = AdjustingSpansListOf(DirectCast(m_CurrentSpan.Parent, Line))
                    ItemContainer.Focus()
                End If
            End If
            '更新按钮
            AdjustAllButton.IsEnabled = (m_CurrentSpan IsNot Nothing)
            AdjustBeginButton.IsEnabled = (m_CurrentSpan IsNot Nothing)
            AdjustEndButton.IsEnabled = (m_CurrentSpan IsNot Nothing)
            SkipButton.IsEnabled = (m_CurrentSpan IsNot Nothing AndAlso m_CurrentSpan IsNot AdjustingItems.Last.Line.Spans.LastOrDefault)
            MergeSpansButton.IsEnabled = (m_CurrentSpan IsNot Nothing)
            SplitSpanButton.IsEnabled = (m_CurrentSpan IsNot Nothing)
            SpanBeginTimeBox.IsEnabled = (m_CurrentSpan IsNot Nothing)
            'SpanDuraionTimeBox.IsEnabled = (m_CurrentSpan IsNot Nothing)
            SpanEndTimeBox.IsEnabled = (m_CurrentSpan IsNot Nothing)
            UpdateSplitLocator()
            UpdateMergeSelector()
            UpdateSpanTimeTextBox()
        End Set
    End Property

    Private Sub UpdateSpanTimeTextBox()
        If CurrentSpan Is Nothing Then
            SpanBeginTimeBox.Text = ""
            SpanDuraionTimeBox.Text = ""
            SpanEndTimeBox.Text = ""
        Else
            SpanBeginTimeBox.Text = CurrentSpan.Begin.ToString("g")
            SpanDuraionTimeBox.Text = CurrentSpan.Duration.ToString("g")
            SpanEndTimeBox.Text = CurrentSpan.End.ToString("g")
        End If
    End Sub

    '把当前行放在滚动视图中
    Private Sub ScrollAjustingItem()
        If CurrentSpan IsNot Nothing Then
            Dim ItemContainer = AdjustingSpansListOf(DirectCast(CurrentSpan.Parent, Line))
            Dim ContainerRect = VisualTreeHelper.GetDescendantBounds(ItemContainer)
            Dim ViewerHeight = AdjustingViewer.ActualHeight
            ItemContainer.BringIntoView(New Rect(ContainerRect.Left, ContainerRect.Top - ViewerHeight / 3, ContainerRect.Width, ContainerRect.Height + ViewerHeight / 3 * 2))
        End If
    End Sub

    ''' <summary>
    ''' 获取指定行的 
    ''' </summary>
    Private Function AdjustingItemOf(line As Line) As AdjusterListItem
        Debug.Assert(line IsNot Nothing)
        Return Aggregate EachItem In AdjustingItems
               Where EachItem.Line Is line
               Into FirstOrDefault()
    End Function

    ''' <summary>
    ''' 获取指定行在列表中的索引。
    ''' </summary>
    Private Function AdjustingIndexOf(line As Line) As Integer
        Debug.Assert(line IsNot Nothing)
        For I = 0 To AdjustingItems.Count - 1
            If AdjustingItems(I).Line Is line Then
                Return I
            End If
        Next
        Return 1
    End Function

    Private Function AdjustingSpansListOf(line As Line) As ListBox
        Dim ItemContainer = DirectCast(AdjustingList.ItemContainerGenerator.ContainerFromItem(
                AdjustingItemOf(line)), ContentPresenter)
        Return DirectCast(AdjustingList.ItemTemplate.FindName("SpansList", ItemContainer), ListBox)
    End Function

    Public Sub GoToPreviousSpan()
        If CurrentSpan IsNot Nothing Then
            Dim Line = DirectCast(CurrentSpan.Parent, Line)
            Dim SpanIndex = Line.Spans.IndexOf(CurrentSpan)
            Debug.Assert(SpanIndex >= 0)
            If SpanIndex > 0 Then
                CurrentSpan = Line.Spans(SpanIndex - 1)
            Else
                '上一行
                GoToPreviousLine()
            End If
        End If
    End Sub

    Public Sub GoToNextSpan() Handles SkipButton.Click
        If CurrentSpan IsNot Nothing Then
            Dim Line = DirectCast(CurrentSpan.Parent, Line)
            Dim SpanIndex = Line.Spans.IndexOf(CurrentSpan)
            Debug.Assert(SpanIndex >= 0)
            If SpanIndex < Line.Spans.Count - 1 Then
                CurrentSpan = Line.Spans(SpanIndex + 1)
            Else
                '下一行
                GoToNextLine()
            End If
        End If
    End Sub

    Public Sub GoToNextLine()
        If CurrentSpan IsNot Nothing Then
            Dim Line = DirectCast(CurrentSpan.Parent, Line)
            Dim LineIndex = AdjustingIndexOf(Line)
            Do While LineIndex < AdjustingItems.Count - 1
                Debug.Assert(LineIndex >= 0)
                LineIndex += 1
                Line = AdjustingItems(LineIndex).Line
                '定位到行的第一个段
                If Line.Spans.Count > 0 Then
                    CurrentSpan = Line.Spans.First
                    ScrollAjustingItem()
                    Exit Do
                End If
            Loop
        End If
    End Sub

    Public Sub GoToPreviousLine()
        If CurrentSpan IsNot Nothing Then
            Dim Line = DirectCast(CurrentSpan.Parent, Line)
            Dim LineIndex = AdjustingIndexOf(Line)
            Do While LineIndex > 0
                LineIndex -= 1
                Line = AdjustingItems(LineIndex).Line
                '定位到行的第一个段
                If Line.Spans.Count > 0 Then
                    CurrentSpan = Line.Spans.First
                    ScrollAjustingItem()
                    Exit Do
                End If
            Loop
        End If
    End Sub

    Public Property MusicSource As String
        Get
            Return m_MusicSource
        End Get
        Set(value As String)
            m_MusicSource = value
        End Set
    End Property

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

    Private Sub MusicOpen(sourcePath As String)
        MusicStop()
        MusicPlayer.Source = New Uri(sourcePath)
        PlayButton.IsEnabled = False
        StopButton.IsEnabled = False
        StartButton.IsEnabled = True
    End Sub

    Private Sub UpdatePosition()
        PositionSlider.Value = MusicPlayer.Position.TotalMilliseconds
        PositionLabel.Text = MusicPlayer.Position.ToString("h\:mm\:ss\.fff")
        'LyricsPanel.SetMusicPosition(MusicPlayer.Position)
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
        If Me.NavigationService IsNot Nothing Then Me.GoBack()
        MsgBox(e.ErrorException.ToString, MsgBoxStyle.Exclamation)
    End Sub

    Private Sub MusicPlayer_MediaOpened(sender As Object, e As System.Windows.RoutedEventArgs) Handles MusicPlayer.MediaOpened
        PlayButton.IsEnabled = True
        StopButton.IsEnabled = True
        If MusicPlayer.NaturalDuration.HasTimeSpan Then
            PositionSlider.IsEnabled = True
            PositionSlider.Maximum = MusicPlayer.NaturalDuration.TimeSpan.TotalMilliseconds
            DurationLabel.Text = MusicPlayer.NaturalDuration.TimeSpan.ToString("g")
            If m_SetVersionDuration Then DataSource.Duration = MusicPlayer.NaturalDuration.TimeSpan
        Else
            PositionSlider.IsEnabled = False
            DurationLabel.Text = "N/A"
        End If
    End Sub

    Private Sub MusicTimer_Tick(sender As Object, e As System.EventArgs) Handles MusicTimer.Tick
        UpdatePosition()
    End Sub

    Private Sub PositionSlider_ValueChanged(sender As System.Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles PositionSlider.ValueChanged
        MusicPlayer.Position = TimeSpan.FromMilliseconds(PositionSlider.Value)
        UpdatePosition()
    End Sub

    Private Sub VersionTimeAdjuster_ContainerDataChanged(sender As Object, e As Document.ObjectModel.ContainerDataChangedEventArgs) Handles Me.ContainerDataChanged
        If e.Source Is CurrentSpan Then
            UpdateSpanTimeTextBox()
        End If
    End Sub

    Private Sub Page_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        MusicOpen(m_MusicSource)
        AdjustingItems = Aggregate EachLine In m_Lines
                         Select New AdjusterListItem(EachLine)
                         Into ToList()
        AdjustingList.ItemsSource = AdjustingItems
        CurrentSpan = Nothing
        AdjustingViewer.Focus()
    End Sub

    Private Sub Page_PreviewKeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyDown
        Select Case e.Key
            Case Key.Enter
                If SMSPopup.IsOpen Then
                    If MergeSpansApplyButton.IsEnabled Then
                        MergeSpanApplyButton_ApplyMerge()
                    End If
                Else
                    Return
                End If
            Case Key.Decimal
                SpeedSlowButton.IsChecked = True
            Case Key.NumPad0
                GoToNextSpan()
            Case Key.NumPad1
                AdjustBegin()
            Case Key.NumPad2
                AdjustAll()
            Case Key.NumPad3
                AdjustEnd()
            Case Key.NumPad5
                PlayButton.IsChecked = Not PlayButton.IsChecked
            Case Key.Space
                AdjustingStart()
            Case Key.LeftCtrl, Key.RightCtrl
                SplitSpanButton.IsChecked = True
            Case Key.LeftShift, Key.RightShift
                MergeSpansButton.IsChecked = True
            Case Key.Up
                GoToPreviousLine()
            Case Key.Down
                GoToNextLine()
            Case Key.Left
                GoToPreviousSpan()
            Case Key.Right
                GoToNextSpan()
            Case Else
                Return
        End Select
        e.Handled = True
    End Sub

    Private Sub VersionTimeAdjuster2_PreviewKeyUp(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyUp
        Select Case e.Key
            Case Key.LeftCtrl, Key.RightCtrl
                SplitSpanButton.IsChecked = False
            Case Key.LeftShift, Key.RightShift
                MergeSpansButton.IsChecked = False
            Case Key.Decimal
                SpeedSlowButton.IsChecked = False
            Case Else
                Return
        End Select
        e.Handled = True
    End Sub

    Private Sub AdjustingStart() Handles StartButton.Click
        MusicPlay()
        CurrentSpan = Aggregate EachLine In Lines
                      Select EachSpan = EachLine.Spans.FirstOrDefault
                      Where EachSpan IsNot Nothing
                      Into First()
    End Sub

    Private Sub SpansList_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)
        CurrentSpan = e.AddedItems.OfType(Of Span).FirstOrDefault
    End Sub

    Private Sub AdjustBegin() Handles AdjustBeginButton.Click
        If CurrentSpan IsNot Nothing Then
            Dim PrevEnd = CurrentSpan.End
            CurrentSpan.Begin = TimeSpan.FromTicks(CInt(Math.Round(MusicPlayer.Position.Ticks / TimeSpan.TicksPerMillisecond)) * TimeSpan.TicksPerMillisecond)
            CurrentSpan.End = PrevEnd
        End If
    End Sub

    Private Sub AdjustEnd() Handles AdjustEndButton.Click
        If CurrentSpan IsNot Nothing Then
            CurrentSpan.End = TimeSpan.FromTicks(CInt(Math.Round(MusicPlayer.Position.Ticks / TimeSpan.TicksPerMillisecond)) * TimeSpan.TicksPerMillisecond)
            GoToNextSpan()
        End If
    End Sub

    Private Sub AdjustAll() Handles AdjustAllButton.Click
        AdjustEnd()
        AdjustBegin()
    End Sub

    Private Sub SpanSplitLocator_ApplySplit()
        Debug.Assert(CurrentSpan IsNot Nothing)
        Dim Line = DirectCast(CurrentSpan.Parent, Line)
        Dim index = Line.Spans.IndexOf(CurrentSpan)
        Debug.Assert(index >= 0)
        If SpanSplitLocator.SelectionStart > 0 AndAlso
            SpanSplitLocator.SelectionStart < Len(SpanSplitLocator.Text) Then
            '防止划出空段
            Dim src = CurrentSpan
            Dim dest = SplitSpan(src, SpanSplitLocator.SelectionStart, DataContext.Document.Package.Lyrics.SpanIndexManager.NewIndex, True)
            Line.Spans.Insert(index, dest(1))
            Line.Spans.Insert(index, dest(0))
            CurrentSpan = Nothing
            Line.Spans.Remove(src)
            DoEvents()
            CurrentSpan = dest(1)
        End If
    End Sub

    Private Sub MergeSpanApplyButton_ApplyMerge() Handles MergeSpansApplyButton.Click
        Dim line = DirectCast(CurrentSpan.Parent, Line)
        Dim src = SpansMergeSelector.SelectedItems.Cast(Of Document.Span)().ToList
        Dim index = Aggregate EachItem In src
                    Select line.Spans.IndexOf(EachItem)
                    Into Min()
        Debug.Assert(index >= 0)
        Dim dest = MergeSpans(src)
        line.Spans.Insert(index, dest)
        CurrentSpan = dest
        For Each EachItem In src
            line.Spans.Remove(EachItem)
        Next
        DoEvents()
        UpdateMergeSelector()
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

    Private Sub UpdateSplitLocator() Handles SplitSpanButton.Checked, SplitSpanButton.Unchecked, SplitSpanButton.IsEnabledChanged
        If SplitSpanButton.IsEnabled AndAlso SplitSpanButton.IsChecked Then
            Dim SpansList = AdjustingSpansListOf(DirectCast(CurrentSpan.Parent, Line))
            '准备切割
            SpanSplitLocator.Text = GetSpanText(m_CurrentSpan)
            Dim SpanContainer = DirectCast(SpansList.ItemContainerGenerator.ContainerFromItem(m_CurrentSpan), FrameworkElement)
            With SpanContainer.PointToScreen(New Point(0, 0))
                SSLPopup.HorizontalOffset = .X
                SSLPopup.VerticalOffset = .Y
            End With
            SSLPopup.MinWidth = SpanContainer.ActualWidth
            SSLPopup.MinHeight = SpanContainer.ActualHeight
            SSLPopup.IsOpen = True
            SpanSplitLocator.Focus()
        Else
            If SSLPopup.IsOpen = True Then
                SSLPopup.IsOpen = False
                AdjustingViewer.Focus()
            End If
        End If
    End Sub

    Private Sub UpdateMergeSelector() Handles MergeSpansButton.Checked, MergeSpansButton.Unchecked, MergeSpansButton.IsEnabledChanged
        If MergeSpansButton.IsEnabled AndAlso MergeSpansButton.IsChecked Then
            Dim SpansList = AdjustingSpansListOf(DirectCast(CurrentSpan.Parent, Line))
            '准备合并
            SpansMergeSelector.ItemsSource = DirectCast(CurrentSpan.Parent, Line).Spans
            SpansMergeSelector.UnselectAll()
            SpansMergeSelector.SelectedItems.Add(CurrentSpan)
            With SpansList.PointToScreen(New Point(0, 0))
                SMSPopup.HorizontalOffset = .X
                SMSPopup.VerticalOffset = .Y
            End With
            SMSPopup.MinWidth = SpansList.ActualWidth
            SMSPopup.MinHeight = SpansList.ActualHeight
            SMSPopup.IsOpen = True
            SpansMergeSelector.Focus()
        Else
            If SMSPopup.IsOpen = True Then
                SMSPopup.IsOpen = False
                AdjustingViewer.Focus()
            End If
        End If
    End Sub

    Private Sub SpansMergeSelector_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles SpansMergeSelector.SelectionChanged
        MergeSpansApplyButton.IsEnabled = (SpansMergeSelector.SelectedItems.Count > 1)
    End Sub

    Private Sub SpansList_PreviewMouseDoubleClick(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)
        Dim span = DirectCast(sender, ListBox).SelectedItem
        If span IsNot Nothing Then
            '自动定位
            MusicPlay()
            MusicPlayer.Position = DirectCast(DirectCast(sender, ListBox).SelectedItem, Span).Begin
        End If
    End Sub

    Private Sub SpansList_PreviewKeyDown(sender As System.Object, e As System.Windows.Input.KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim span = DirectCast(sender, ListBox).SelectedItem
            If span IsNot Nothing Then
                '自动定位
                MusicPlay()
                MusicPlayer.Position = DirectCast(DirectCast(sender, ListBox).SelectedItem, Span).Begin
            End If
        End If
    End Sub

    Private Sub SpanTimeBox_PreviewKeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles SpanBeginTimeBox.PreviewKeyDown, SpanDuraionTimeBox.PreviewKeyDown, SpanEndTimeBox.PreviewKeyDown
        If e.Key = Key.Add OrElse e.Key = Key.Subtract OrElse e.Key = Key.Enter Then
            Dim value As TimeSpan, adjustment As TimeSpan
            Dim st = DirectCast(sender, TextBox)
            If TimeSpan.TryParse(DirectCast(SpanAdjustmentBox, TextBox).Text, adjustment) Then
                If TimeSpan.TryParse(st.Text, value) Then
                    Select Case e.Key
                        Case Key.Add
                            st.Text = (value + adjustment).ToString("g")
                        Case Key.Subtract
                            st.Text = (value - adjustment).ToString("g")
                        Case Key.Enter
                            If sender Is SpanBeginTimeBox Then
                                Dim PrevEnd = CurrentSpan.End
                                CurrentSpan.Begin = value
                                CurrentSpan.End = PrevEnd
                            ElseIf sender Is SpanDuraionTimeBox Then
                                CurrentSpan.Duration = value
                            ElseIf sender Is SpanEndTimeBox Then
                                CurrentSpan.End = value
                            End If
                            AdjustingViewer.Focus()
                    End Select
                Else
                    Beep()
                End If
            Else
                SpanAdjustmentBox.Focus()
                Beep()
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub AdjustingViewer_PreviewMouseWheel(sender As System.Object, e As System.Windows.Input.MouseWheelEventArgs) Handles AdjustingViewer.PreviewMouseWheel
        AdjustingViewer.ScrollToVerticalOffset(AdjustingViewer.VerticalOffset - e.Delta)
        e.Handled = True
    End Sub

    Private Sub SpeedSlowButton_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles SpeedSlowButton.Checked, SpeedSlowButton.Unchecked
        Static PrevSpeed As Double
        SpeedRatioSlider.IsEnabled = Not SpeedSlowButton.IsChecked.GetValueOrDefault
        If SpeedSlowButton.IsChecked Then
            PrevSpeed = MusicPlayer.SpeedRatio
            MusicPlayer.SpeedRatio = 0.5
        Else
            MusicPlayer.SpeedRatio = PrevSpeed
        End If
    End Sub
End Class
