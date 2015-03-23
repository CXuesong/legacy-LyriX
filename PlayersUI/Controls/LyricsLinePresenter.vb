#Const ApplyMask = True

Imports System.Windows.Controls.Primitives
Imports System.ComponentModel

Namespace Controls
    '关系
    'LyricsLine
    '\--- Item

    Public Class LyricsLinePresenter
        Inherits FrameworkElement

        ''' <summary>
        ''' 用于暂存语义段的具体布局信息。
        ''' </summary>
        Private Class _SegmentLayoutInfo
            Public ColumnIndex As Integer   '记录每个 Segment 在 Grid 的列索引
            Public Left As Double           '记录每个 Segment 的左边距
            Public Width As Double          '记录每个 Segment 的宽度

            ''' <summary>
            ''' 构造一个语义段布局信息。
            ''' </summary>
            Public Sub New(columnIndex As Integer)
                Me.ColumnIndex = columnIndex
            End Sub
        End Class

        Private Shared ReadOnly UpdateLineCallback As PropertyChangedCallback =
            Sub(d, e) DirectCast(d, LyricsLinePresenter).UpdatePresenter()

        ''' <summary>
        ''' 标识 <see cref="LineItems" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly LineItemsProperty As DependencyProperty = DependencyProperty.Register(
            "LineItems", GetType(LineItemStyleCollection), GetType(LyricsLinePresenter),
            New FrameworkPropertyMetadata(LineItemStyleCollection.Default,
                                          FrameworkPropertyMetadataOptions.AffectsMeasure,
                                          UpdateLineCallback))
        ''' <summary>
        ''' 标识 <see cref="TextAlignment" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly TextAlignmentProperty As DependencyProperty = TextBlock.TextAlignmentProperty.
            AddOwner(GetType(LyricsLinePresenter),
                     New FrameworkPropertyMetadata(TextAlignment.Center,
                                                   FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsRender,
                                                   UpdateLineCallback))
        ''' <summary>
        ''' 标识 <see cref="Source" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly SourceProperty As DependencyProperty =
            DependencyProperty.Register("Source", GetType(LyricsLine), GetType(LyricsLinePresenter),
                                        New FrameworkPropertyMetadata(Nothing,
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      Sub(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
                                                                          'FUTURE DISCUSS 内存泄漏？
                                                                          If e.OldValue IsNot Nothing Then
                                                                              RemoveHandler DirectCast(e.OldValue, LyricsLine).PropertyChanged, AddressOf DirectCast(d, LyricsLinePresenter).Source_PropertyChanged
                                                                          End If
                                                                          If e.NewValue IsNot Nothing Then
                                                                              AddHandler DirectCast(e.NewValue, LyricsLine).PropertyChanged, AddressOf DirectCast(d, LyricsLinePresenter).Source_PropertyChanged
                                                                          End If
                                                                          DirectCast(d, LyricsLinePresenter).UpdatePresenter()
                                                                      End Sub))

        Dim SegmentLayoutInfo As New List(Of _SegmentLayoutInfo)
        Dim SpanFirstSegmentIndex As New List(Of Integer)            '记录每个 Span 的第一个 Segment 的位置（base + Index）
        Dim LayoutGrid As New Grid With {.Width = Double.NaN,
                                         .Height = Double.NaN}  '控件的主体部分
        Dim LineBrush As New VisualBrush(LayoutGrid) With
            {.Stretch = Stretch.None,
             .AlignmentX = AlignmentX.Left,
             .AlignmentY = AlignmentY.Top}
        '.Viewbox = New Rect(0, 0, 0, 0),
        '.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox}
        ''注意：此处如果不设置 ViewboxUnits，会导致部分文本左对齐
        ''FUTURE Figure out what happened
        ' SOLUTION : VS 2012 OK
        Dim LineVisual As New DrawingVisual With {.OpacityMask = LineBrush}

        Public Property LineItems() As LineItemStyleCollection
            Get
                Return DirectCast(Me.GetValue(LineItemsProperty), LineItemStyleCollection)
            End Get
            Set(value As LineItemStyleCollection)
                Me.SetValue(LineItemsProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置内容的水平对齐方式。
        ''' </summary>
        Public Property TextAlignment() As TextAlignment
            Get
                Return DirectCast(Me.GetValue(TextAlignmentProperty), TextAlignment)
            End Get
            Set(value As TextAlignment)
                Me.SetValue(TextAlignmentProperty, value)
            End Set
        End Property

        Public Property Source As LyricsLine
            Get
                Return DirectCast(Me.GetValue(SourceProperty), LyricsLine)
            End Get
            Set(value As LyricsLine)
                Me.SetValue(SourceProperty, value)
            End Set
        End Property

        Protected Overrides ReadOnly Property VisualChildrenCount As Integer
            Get
                'Control 有且只能有一个子级
                Return 1
            End Get
        End Property

        Protected Overrides Function GetVisualChild(ByVal index As Integer) As System.Windows.Media.Visual
            Select Case index
                Case 0
                    Return LineVisual
                Case Else
                    Throw New IndexOutOfRangeException
            End Select
        End Function

        '开始定位，调整布局
        Protected Overrides Function ArrangeOverride(finalSize As System.Windows.Size) As System.Windows.Size
            Dim ClientRect = New Rect(0, 0, finalSize.Width, finalSize.Height)
            'LayoutGrid.Width = finalSize.Width
            LayoutGrid.Arrange(ClientRect)
            '继续填充 Span Info
            For Each EachSpan In SegmentLayoutInfo
                With EachSpan
                    .Left = Aggregate EachColumn In LayoutGrid.ColumnDefinitions
                                                Take .ColumnIndex
                                                Into Sum(EachColumn.ActualWidth)
                    .Width = LayoutGrid.ColumnDefinitions(.ColumnIndex).ActualWidth
                End With
            Next
            UpdateVisual(ClientRect)
            Return MyBase.ArrangeOverride(finalSize)
        End Function

        Protected Overrides Function MeasureOverride(availableSize As System.Windows.Size) As System.Windows.Size
            LayoutGrid.Measure(availableSize)
            Return LayoutGrid.DesiredSize
        End Function

        Protected Overrides Sub OnRender(drawingContext As System.Windows.Media.DrawingContext)
            MyBase.OnRender(drawingContext)
        End Sub

#Region "Line Render"
        Private Sub UpdatePresenter()
            If Source Is Nothing Then
                If DesignerProperties.GetIsInDesignMode(Me) Then
                    UpdateLine(Samples.CompiledLineSamples(0))
                Else
                    UpdateLine(Nothing)
                End If
            Else
                UpdateLine(Source.DataSource)
            End If
        End Sub

        Private Sub UpdateLine(line As Compiled.Line)
            '清理
            LayoutGrid.Children.Clear()
            LayoutGrid.RowDefinitions.Clear()
            LayoutGrid.ColumnDefinitions.Clear()
            SegmentLayoutInfo.Clear()
            SpanFirstSegmentIndex.Clear()
            '构造
            If line IsNot Nothing AndAlso LineItems IsNot Nothing AndAlso
                line.Spans.Count > 0 AndAlso LineItems.Count > 0 Then
                '将行与行索引对应
                Dim I As Integer = 0
                Dim ItemRows As New List(Of KeyValuePair(Of Integer, LineItemStyle))(LineItems.Count)
                For Each EachItem In LineItems
                    ItemRows.Add(New KeyValuePair(Of Integer, LineItemStyle)(I, EachItem))
                    LayoutGrid.RowDefinitions.Add(New RowDefinition With {.Height = GridLength.Auto})
                    I += 1
                Next
                ConstructLine(line, ItemRows)
            End If
            InvalidateMeasure() '--> UpdateVisual
        End Sub

        'Eg.
        '那些|因为年轻|犯的错
        '语义段：按每个汉字分割

        Private Sub ConstructLine(line As Compiled.Line, ItemRows As IEnumerable(Of KeyValuePair(Of Integer, LineItemStyle)))
            Debug.Assert(line IsNot Nothing AndAlso line.Spans.Count > 0)
            'Line
            '[1*]...[1*]
            '空列用于为较长的通行项提供空间
            Dim Column1 As Integer = If(TextAlignment = AlignmentX.Center OrElse
                                       TextAlignment = AlignmentX.Right,
                                       CreateColumn(New GridLength(1, GridUnitType.Star)), 0)
            Dim I As Integer = 0
            Debug.Assert(Column1 = 0)
            For Each EachSpan In line.Spans
                ConstructSpan(EachSpan, ItemRows)
                I += 1
            Next
            Dim Column2 As Integer = If(TextAlignment = AlignmentX.Left OrElse
                                       TextAlignment = AlignmentX.Center,
                                       CreateColumn(New GridLength(1, GridUnitType.Star)),
                                       CurrentColumn)
            For Each EachRow In (From ER In ItemRows Where ER.Value.AttachedTo = LineItemAttachedLevel.Line)
                CreateTextBlock(EachRow.Value, line, EachRow.Key, Column1, 1 + Column2 - Column1)
            Next
        End Sub

        Private Sub ConstructSpan(span As Compiled.Span, ItemRows As IEnumerable(Of KeyValuePair(Of Integer, LineItemStyle)))
            If span.Segments.Count > 0 Then
                Dim FirstSegmentIndex = SegmentLayoutInfo.Count
                Dim Column1 As Integer = CurrentColumn
                For Each EachSegment In span.Segments
                    ConstructSegment(EachSegment, ItemRows)
                Next
                Dim Column2 As Integer = CurrentColumn
                For Each EachRow In (From ER In ItemRows Where ER.Value.AttachedTo = LineItemAttachedLevel.Span)
                    CreateTextBlock(EachRow.Value, span, EachRow.Key, Column1, 1 + Column2 - Column1)
                Next
                SpanFirstSegmentIndex.Add(FirstSegmentIndex)
            Else
                SpanFirstSegmentIndex.Add(-1)
            End If
        End Sub

        Private Sub ConstructSegment(segment As Compiled.Segment, ItemRows As IEnumerable(Of KeyValuePair(Of Integer, LineItemStyle)))
            Dim ColumnIndex = CreateColumn()
            Dim NewInfo = New _SegmentLayoutInfo(ColumnIndex)
            For Each EachRow In (From ER In ItemRows Where ER.Value.AttachedTo = LineItemAttachedLevel.Segment)
                CreateTextBlock(EachRow.Value, segment, EachRow.Key, ColumnIndex, 1)
            Next
            SegmentLayoutInfo.Add(NewInfo)      '登记布局信息
        End Sub

        Private Function CreateColumn(Optional width As GridLength = Nothing) As Integer
            LayoutGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = width})
            Return CurrentColumn
        End Function

        ''' <summary>
        ''' 获取 LayoutGrid 当前的列位置。
        ''' </summary>
        ''' <value>-1 表示不存在。</value>
        Private ReadOnly Property CurrentColumn As Integer
            Get
                Return LayoutGrid.ColumnDefinitions.Count - 1
            End Get
        End Property

        Private Function CreateTextBlock(style As LineItemStyle, source As Compiled.ObjectModel.CompiledContainer, row As Integer, column As Integer, columnSpan As Integer) As TextBlock
            If style IsNot Nothing Then
                Dim Text = style.GetText(source)
                '省内存，省布局空间
                If Text <> Nothing Then
                    Dim NewInstance As New TextBlock With {.Foreground = Brushes.Black,
                                                           .HorizontalAlignment = Windows.HorizontalAlignment.Stretch,
                                                           .TextAlignment = style.GetTextAlignment(TextAlignment),
                                                           .VerticalAlignment = style.VerticalAlignment,
                                                           .FontFamily = style.FontFamily,
                                                           .FontSize = style.FontSize,
                                                           .FontStyle = style.FontStyle}
                    NewInstance.Inlines.Add(New Run(Text))
#If Not ApplyMask Then
                NewInstance.Background = New SolidColorBrush(Color.FromArgb(128, 200, CByte(Rnd() * 200), CByte(Rnd() * 200)))
#End If
                    With NewInstance.Margin
                        NewInstance.Margin = New Thickness(.Left, .Top, .Right, style.ItemSpacing)
                    End With
                    LayoutGrid.Children.Add(NewInstance)
                    Grid.SetRow(NewInstance, row)
                    Grid.SetColumn(NewInstance, column)
                    Grid.SetColumnSpan(NewInstance, columnSpan)
                    Return NewInstance
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 填充行的色块。
        ''' </summary>
        Private Sub UpdateVisual(Optional lineRect As Rect? = Nothing)
            'Debug.Print(Source.DataSource.ToString & vbTab & Source.SegmentProgress)
            Static previousRect As Rect? = Nothing
            previousRect = If(lineRect, previousRect)
            If previousRect Is Nothing Then
                '表明参数 lineRect 与 previousRect 都为 Nothing
                'do nothing
            Else
                '需要绘制行项
                Dim ClientRect = previousRect.Value
                Using DC = LineVisual.RenderOpen
                    If LayoutGrid.RowDefinitions.Count > 0 Then
                        Dim I As Integer = 0, currentY As Double = 0
                        Dim ProgressWidth As Double
                        If Source IsNot Nothing Then
                            With Source
                                If .SegmentProgress = Double.PositiveInfinity Then
                                    ProgressWidth = ClientRect.Width
                                ElseIf .SpanIndex >= 0 AndAlso .SegmentIndex >= 0 Then
                                    Dim SegmentInfo = SegmentLayoutInfo(SpanFirstSegmentIndex(.SpanIndex) + .SegmentIndex)
                                    ProgressWidth = SegmentInfo.Left + SegmentInfo.Width * .SegmentProgress
                                Else
                                    ProgressWidth = 0
                                End If
                            End With
                        End If
                        For Each EachItem In LineItems
                            '绘制行项
                            Dim currentHeight = LayoutGrid.RowDefinitions(I).ActualHeight
                            DC.DrawRectangle(EachItem.Foreground, Nothing, New Rect(0, currentY, ClientRect.Width, currentHeight))
                            '绘制进度
                            If ProgressWidth > 0 Then
                                DC.DrawRectangle(EachItem.HighlightForeground, Nothing, New Rect(0, currentY, If(Double.IsInfinity(ProgressWidth), ClientRect.Width, ProgressWidth), currentHeight))
                            End If
                            currentY += currentHeight
                            I += 1
                        Next
#If Not ApplyMask Then
                    DC.DrawRectangle(LineBrush, Nothing, ClientRect)
#End If
                    End If
                End Using
            End If
        End Sub
#End Region

        Public Sub New()
            Me.AddLogicalChild(LineVisual)
            Me.AddVisualChild(LineVisual)
#If Not ApplyMask Then
        LineVisual.OpacityMask = Nothing
        LayoutGrid.ShowGridLines = True
#End If
            If LineBrush.CanFreeze Then LineBrush.Freeze()
            UpdatePresenter()
        End Sub

        Shared Sub New()
            '将 ControlTemplate 放入任何特定于主题的资源词典文件中时，
            '都必须为控件创建静态构造函数，
            '并对 DefaultStyleKey 调用 OverrideMetadata(Type, PropertyMetadata) 方法
            '
            '此 OverrideMetadata 调用通知系统该元素希望提供不同于其基类的样式。
            '此样式定义在 themes\generic.xaml 中
            'DefaultStyleKeyProperty.OverrideMetadata(GetType(LyricsLine), New FrameworkPropertyMetadata(GetType(LyricsLine)))
            'DependencyProperty.Register()
        End Sub

        Private Sub Source_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)   'Handles [By AddHandler]
            If e.PropertyName = "DataSource" Then
                Me.UpdatePresenter()
            Else
                Me.UpdateVisual()
            End If
        End Sub
    End Class
End Namespace