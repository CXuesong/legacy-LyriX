Imports System.ComponentModel

Namespace Controls
    '关系
    'LyricsPanel
    '\--- LyricsLine
    <TemplatePart(Name:="LyricsPanel", Type:=GetType(Panel))>
    Public Class LyricsPanel
        Inherits Control

        'Private Shared ReadOnly UpdatePanelCallback As PropertyChangedCallback =
        '    Sub(d, e) DirectCast(d, LyricsPanel).UpdatePanel()

        ''' <summary>
        ''' 标识 <see cref="LineItems" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly LineItemsProperty As DependencyProperty = LyricsLinePresenter.LineItemsProperty.AddOwner(
            GetType(LyricsPanel),
            New FrameworkPropertyMetadata(LineItemStyleCollection.Default))
        ''' <summary>
        ''' 标识 <see cref="TextAlignment" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly TextAlignmentProperty As DependencyProperty = LyricsLinePresenter.TextAlignmentProperty.AddOwner(
            GetType(LyricsPanel),
            New FrameworkPropertyMetadata(TextAlignment.Center,
                                          FrameworkPropertyMetadataOptions.AffectsArrange))
        ' ''' <summary>
        ' ''' 标识 <see cref="VerticalLineAlignment" /> 依赖项属性。
        ' ''' </summary>
        'Public Shared ReadOnly VerticalLineAlignmentProperty As DependencyProperty = TileBrush.AlignmentYProperty.AddOwner(
        '    GetType(LyricsPanel),
        '    New FrameworkPropertyMetadata(AlignmentY.Center,
        '                                  FrameworkPropertyMetadataOptions.AffectsArrange,
        '                                  UpdatePanelCallback))
        ''' <summary>
        ''' 标识 <see cref="Orientation" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly OrientationProperty As DependencyProperty = StackPanel.OrientationProperty.AddOwner(
            GetType(LyricsPanel),
            New FrameworkPropertyMetadata(Orientation.Vertical,
                                          FrameworkPropertyMetadataOptions.AffectsArrange))


        ''' <summary>
        ''' 在 <see cref="Player" /> 变化时发生。
        ''' </summary>
        Public Event PlayerChanged(sender As Object, e As EventArgs)

        Private m_PanelItems As New Collections.ObjectModel.ObservableCollection(Of LyricsPanelItem)
        Private m_s_PanelItems As New Collections.ObjectModel.ReadOnlyObservableCollection(Of LyricsPanelItem)(m_PanelItems)
        Private WithEvents m_Player As Players.SingleTrackLyricsPlayer
        Private PanelList As ItemsControl

        '性能：
        '请不要只是为了绑定数据而将 CLR 对象转换为 XML。

        Dim LyricsLineCollection As New List(Of LyricsLine)

        ''' <summary>
        ''' 获取/设置此歌词面板使用的歌词播放器。
        ''' </summary>
        <Bindable(True)>
        Public Property Player As Players.ObjectModel.LyricsPlayer
            Get
                Return m_Player
            End Get
            Set(value As Players.ObjectModel.LyricsPlayer)
                'FUTURE 支持其他类型的
                m_Player = DirectCast(value, SingleTrackLyricsPlayer)
                OnPlayerChanged()
                UpdatePanel()
            End Set
        End Property

        ''' <summary>
        ''' 获取此歌词面板的面板项。
        ''' </summary>
        <Bindable(True)>
        Public ReadOnly Property PanelItems As Collections.ObjectModel.ReadOnlyObservableCollection(Of LyricsPanelItem)
            Get
                Return m_s_PanelItems
            End Get
        End Property

        ' ''' <summary>
        ' ''' 获取/设置内容的水平对齐方式。
        ' ''' </summary>
        'Public Property LineAlignment() As AlignmentX
        '    Get
        '        Return DirectCast(Me.GetValue(LineAlignmentProperty), AlignmentX)
        '    End Get
        '    Set(value As AlignmentX)
        '        Me.SetValue(LineAlignmentProperty, value)
        '    End Set
        'End Property

        ''' <summary>
        ''' 获取/设置内容的水平对齐方式。
        ''' </summary>
        <Bindable(True)>
        Public Property TextAlignment() As TextAlignment
            Get
                Return DirectCast(Me.GetValue(TextAlignmentProperty), TextAlignment)
            End Get
            Set(value As TextAlignment)
                Me.SetValue(TextAlignmentProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置在每一歌词行中要显示的项及其格式。
        ''' </summary>
        <Bindable(True)>
        Public Property LineItems As LineItemStyleCollection
            Get
                Return DirectCast(Me.GetValue(LineItemsProperty), LineItemStyleCollection)
            End Get
            Set(value As LineItemStyleCollection)
                Me.SetValue(LineItemsProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置歌词行的排列方向。
        ''' </summary>
        <Bindable(True)>
        Public Property Orientation As Orientation
            Get
                Return DirectCast(Me.GetValue(OrientationProperty), Orientation)
            End Get
            Set(value As Orientation)
                Me.SetValue(OrientationProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 引发一个 <see cref="PlayerChanged" /> 事件。
        ''' </summary>
        Protected Overridable Overloads Sub OnPlayerChanged()
            RaiseEvent PlayerChanged(Me, EventArgs.Empty)
        End Sub

        Public Overrides Sub OnApplyTemplate()
            If Me.Template Is Nothing Then
                PanelList = Nothing
            Else
                PanelList = TryCast(Me.Template.FindName("PanelList", Me), ItemsControl)
            End If
            MyBase.OnApplyTemplate()
        End Sub

        ''' <summary>
        ''' 获取指定行在 <see cref="LyricsPanel" /> 中的位置与尺寸。
        ''' </summary>
        ''' <param name="lineIndex">指定行的索引。</param>
        Public Function GetLineRect(lineIndex As Integer) As Rect
            If PanelList IsNot Nothing Then
                '获取行项对应的行
                Dim LineItem = TryCast(PanelList.ItemContainerGenerator.ContainerFromItem(LyricsLineCollection(lineIndex)), FrameworkElement)
                If LineItem IsNot Nothing Then
                    Dim Origin = LineItem.TranslatePoint(New Point(0, 0), Me)
                    Return New Rect(Origin.X, Origin.Y, LineItem.ActualWidth, LineItem.ActualHeight)
                End If
            End If
            Return Rect.Empty
        End Function

        Private Sub UpdatePanel()
            m_PanelItems.Clear()
            LyricsLineCollection.Clear()
            If DesignerProperties.GetIsInDesignMode(Me) Then
                'Samples
                m_PanelItems.Add(New MusicInfoHint(Samples.CompiledDocumentSample.MusicInfo))
                m_PanelItems.Add(New LyricsVersionHint(Samples.CompiledVersionSamples(0)))
                m_PanelItems.Add(New LyricsArtistsHint(Samples.CompiledTrackSamples(0).Artists))
                For I = 0 To 3
                    m_PanelItems.Add(New LyricsLine(Samples.CompiledLineSamples(I)) With {.SpanIndex = 0, .SegmentIndex = 0, .SegmentProgress = 1})
                Next
            ElseIf m_Player IsNot Nothing AndAlso m_Player.Version IsNot Nothing Then
                'MusicInfo
                If m_Player.Document IsNot Nothing Then
                    m_PanelItems.Add(New MusicInfoHint(m_Player.Document.MusicInfo))
                End If
                'Version
                m_PanelItems.Add(New LyricsVersionHint(m_Player.Version))
                Dim PreviousArtists As IList(Of Compiled.ArtistBase) = {}
                For Each EachLine In m_Player.Lines
                    'ArtistsHint
                    If EachLine.Artists.Count <> PreviousArtists.Count OrElse
                        Aggregate EachArtist In EachLine.Artists Into Any(Not PreviousArtists.Contains(EachArtist)) Then
                        PreviousArtists = EachLine.Artists
                        m_PanelItems.Add(New LyricsArtistsHint(PreviousArtists))
                    End If
                    'Line
                    Dim NewLine = New LyricsLine(EachLine)
                    LyricsLineCollection.Add(NewLine)
                    m_PanelItems.Add(NewLine)
                Next
            End If
        End Sub

        Public Sub New()

        End Sub

        Shared Sub New()
            '将 ControlTemplate 放入任何特定于主题的资源词典文件中时，
            '都必须为控件创建静态构造函数，
            '并对 DefaultStyleKey 调用 OverrideMetadata(Type, PropertyMetadata) 方法
            '
            '此 OverrideMetadata 调用通知系统该元素希望提供不同于其基类的样式。
            '此样式定义在 themes\generic.xaml 中
            DefaultStyleKeyProperty.OverrideMetadata(GetType(LyricsPanel), New FrameworkPropertyMetadata(GetType(LyricsPanel)))
        End Sub

        Private Sub m_Player_CurrentSegmentChanged(sender As Object, e As CurrentSegmentChangedEventArgs) Handles m_Player.CurrentSegmentChanged
            If e.OldLineIndex = m_Player.LineIndex AndAlso e.OldSpanIndex <> m_Player.SpanIndex AndAlso e.OldSpanIndex.CurrentExists AndAlso e.OldSegmentIndex.CurrentExists Then
                '行内段圆满
                LyricsLineCollection(m_Player.LineIndex.Current).SetProgress(e.OldSpanIndex.Current, e.OldSegmentIndex.Current, 1)
            ElseIf e.OldLineIndex.IsEmpty Then
                '如果上一个行索引为空，只能表明这是第一次播放，正在初始化
                If m_Player.LineIndex.PreviousExists Then
                    '这种情况可能发生在中途换版本时
                    '从前往后
                    For I = 0 To m_Player.LineIndex.Previous
                        LyricsLineCollection(I).FillProgress()
                    Next
                End If
            Else
                Dim Index1 As Integer, Index2 As Integer
                Dim BeginToEnd As Boolean
                If e.OldLineIndex < m_Player.LineIndex Then
                    '从前往后
                    Index1 = If(e.OldLineIndex.CurrentExists, e.OldLineIndex.Current, e.OldLineIndex.Next)
                    Index2 = m_Player.LineIndex.Previous
                    BeginToEnd = True
                Else
                    '从后往前
                    Index1 = If(e.OldLineIndex.CurrentExists, e.OldLineIndex.Current, e.OldLineIndex.Previous)
                    Index2 = m_Player.LineIndex.Next
                    BeginToEnd = False
                End If
                For I = Index1 To Index2 Step If(BeginToEnd, 1, -1)
                    '行圆满
                    If BeginToEnd Then
                        LyricsLineCollection(I).FillProgress()
                    Else
                        LyricsLineCollection(I).ClearProgress()
                    End If
                Next
            End If
        End Sub

        Private Sub m_Player_PositionChanged(sender As Object, e As ObjectModel.PositionChangedEventArgs) Handles m_Player.PositionChanged
            '改变当前进程
            If m_Player.LineIndex.CurrentExists AndAlso m_Player.SpanIndex.CurrentExists Then
                LyricsLineCollection(m_Player.LineIndex.Current).SetProgress(m_Player.SpanIndex.Current, m_Player.SegmentIndex.Current, m_Player.SegmentProgress)
            End If
        End Sub

        Private Sub m_Player_VersionChanged(sender As Object, e As System.EventArgs) Handles m_Player.VersionChanged
            UpdatePanel()
        End Sub
    End Class
End Namespace