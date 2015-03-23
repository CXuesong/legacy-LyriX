Imports System.ComponentModel

Namespace Controls
    ''' <summary>
    ''' 表示歌词行中项的格式信息。
    ''' </summary>
    Public MustInherit Class LineItemStyle
        Inherits Freezable

#Region "Dependency"
        ''' <summary>
        ''' 标识 <see cref="ItemSpacing" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly ItemSpacingProperty As DependencyProperty =
            DependencyProperty.Register("ItemSpacing", GetType(Double), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(2.0#))

        ''' <summary>
        ''' 标识 <see cref="Alignment" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly AlignmentProperty As DependencyProperty =
            DependencyProperty.Register("Alignment", GetType(LineItemAlignment), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(LineItemAlignment.Default),
                                        Function(value)
                                            Dim v = DirectCast(value, LineItemAlignment)
                                            Return v = LineItemAlignment.Default OrElse
                                                v = LineItemAlignment.Left OrElse
                                                v = LineItemAlignment.Center OrElse
                                                v = LineItemAlignment.Right
                                        End Function)

        ''' <summary>
        ''' 标识 <see cref="TextAlignment" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly VerticalAlignmentProperty As DependencyProperty =
            DependencyProperty.Register("VerticalAlignment", GetType(TextAlignment), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(TextAlignment.Center))
        ''' <summary>
        ''' 标识 <see cref="FontSize" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly FontSizeProperty As DependencyProperty =
            DependencyProperty.Register("FontSize", GetType(Double), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(Block.FontSizeProperty.DefaultMetadata.DefaultValue))
        ''' <summary>
        ''' 标识 <see cref="FontFamily" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly FontFamilyProperty As DependencyProperty =
            DependencyProperty.Register("FontFamily", GetType(FontFamily), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(Block.FontFamilyProperty.DefaultMetadata.DefaultValue))
        ''' <summary>
        ''' 标识 <see cref="FontStyle" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly FontStyleProperty As DependencyProperty =
            DependencyProperty.Register("FontStyle", GetType(FontStyle), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(Block.FontStyleProperty.DefaultMetadata.DefaultValue))
        ''' <summary>
        ''' 标识 <see cref="Foreground" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly ForegroundProperty As DependencyProperty =
            DependencyProperty.Register("Foreground", GetType(Brush), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(Block.ForegroundProperty.DefaultMetadata.DefaultValue))

        ''' <summary>
        ''' 标识 <see cref="HighlightForeground" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly HighlightForegroundProperty As DependencyProperty =
            DependencyProperty.Register("HighlightForeground", GetType(Brush), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(Brushes.BlueViolet))
#End Region

        ''' <summary>
        ''' 表示一个默认的 <see cref="LineItemStyle" />。
        ''' </summary>
        Public Shared ReadOnly Lyrics As New LineSegmentItemStyle() With
            {.AttachedTo = LineItemAttachedLevel.Segment,
             .TextSource = {Compiled.SegmentTextItem.Text},
             .Alignment = LineItemAlignment.Center,
             .FontSize = 24.0#,
             .Foreground = Brushes.Navy}

        ''' <summary>
        ''' 表示一个默认的 <see cref="LineItemStyle" />。
        ''' </summary>
        Public Shared ReadOnly Latinized As New LineSegmentItemStyle() With
            {.AttachedTo = LineItemAttachedLevel.Segment,
             .TextSource = {Compiled.SegmentTextItem.Latinized},
             .Alignment = LineItemAlignment.Center,
             .FontSize = 13.0#,
             .Foreground = Brushes.Navy}

        ''' <summary>
        ''' 表示一个默认的 <see cref="LineItemStyle" />。
        ''' </summary>
        Public Shared ReadOnly Alphabetic As New LineSegmentItemStyle() With
            {.AttachedTo = LineItemAttachedLevel.Segment,
             .TextSource = {Compiled.SegmentTextItem.Alphabetic},
             .Alignment = LineItemAlignment.Center,
             .FontSize = 13.0#,
             .Foreground = Brushes.Navy}

        ''' <summary>
        ''' 表示一个默认的 <see cref="LineItemStyle" />。
        ''' </summary>
        Public Shared ReadOnly Translation As New LineTranslationItemStyle With
            {.FontSize = 13.0#,
             .Foreground = Brushes.Navy}

        ''' <summary>
        ''' 获取/设置此项目后方（下方）的空白间距。
        ''' </summary>
        <Bindable(True), Category("Appearance")>
        Public Property ItemSpacing As Double
            Get
                Return CDbl(GetValue(ItemSpacingProperty))
            End Get
            Set(ByVal value As Double)
                SetValue(ItemSpacingProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置此项目的水平对齐方式。
        ''' </summary>
        <Bindable(True), Category("Text")>
        Public Property Alignment As LineItemAlignment
            Get
                Return DirectCast(GetValue(AlignmentProperty), LineItemAlignment)
            End Get
            Set(ByVal value As LineItemAlignment)
                SetValue(AlignmentProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置此项目的竖直对齐方式。
        ''' </summary>
        <Bindable(True), Category("Text")>
        Public Property VerticalAlignment As VerticalAlignment
            Get
                Return DirectCast(GetValue(VerticalAlignmentProperty), VerticalAlignment)
            End Get
            Set(ByVal value As VerticalAlignment)
                SetValue(VerticalAlignmentProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置此项目的字号。 
        ''' </summary>
        <Bindable(True), Category("Text")>
        Public Property FontSize As Double
            Get
                Return CDbl(GetValue(FontSizeProperty))
            End Get
            Set(ByVal value As Double)
                SetValue(FontSizeProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置一个用于描述此项目前景色的画笔。
        ''' </summary>
        <Bindable(True), Category("Appearance")>
        Public Property Foreground As Brush
            Get
                Return DirectCast(GetValue(ForegroundProperty), Brush)
            End Get
            Set(ByVal value As Brush)
                SetValue(ForegroundProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置歌词的首选顶级字体系列。
        ''' </summary>
        Public Property FontFamily As FontFamily
            Get
                Return DirectCast(GetValue(FontFamilyProperty), FontFamily)
            End Get
            Set(ByVal value As FontFamily)
                SetValue(FontFamilyProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置歌词的首选顶级字体系列。
        ''' </summary>
        Public Property FontStyle As FontStyle
            Get
                Return DirectCast(GetValue(FontStyleProperty), FontStyle)
            End Get
            Set(ByVal value As FontStyle)
                SetValue(FontStyleProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置一个用于描述此项目强调前景色的画笔。
        ''' </summary>
        <Bindable(True), Category("Appearance")>
        Public Property HighlightForeground As Brush
            Get
                Return DirectCast(GetValue(HighlightForegroundProperty), Brush)
            End Get
            Set(ByVal value As Brush)
                SetValue(HighlightForegroundProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置此项目的每段文本的作用范围。 
        ''' </summary>
        <Bindable(True), Category("Behavior")>
        Public MustOverride Property AttachedTo As LineItemAttachedLevel

        ''' <summary>
        ''' 根据指定的源获取与此项对应的文本内容。
        ''' </summary>
        ''' <param name="source">可以是 <see cref="Compiled.Line" />、<see cref="Compiled.Span" /> 或是 <see cref="Compiled.Segment" />。</param>
        Public Function GetText(source As Compiled.ObjectModel.CompiledContainer) As String
            Dim ActualLevel As LineItemAttachedLevel
            If source Is Nothing Then
                Return Nothing
            ElseIf TypeOf source Is Compiled.Segment Then
                ActualLevel = LineItemAttachedLevel.Segment
            ElseIf TypeOf source Is Compiled.Span Then
                ActualLevel = LineItemAttachedLevel.Span
            ElseIf TypeOf source Is Compiled.Line Then
                ActualLevel = LineItemAttachedLevel.Line
            Else
                Throw New ArgumentOutOfRangeException("source")
            End If
            If AttachedTo >= ActualLevel Then
                Return GetTextCore(source)
            Else
                Throw New ArgumentOutOfRangeException("source")
            End If
        End Function

        ''' <summary>
        ''' 根据指定的行的对齐方式与此项的对齐方式，获取此项中文本的对齐方式。
        ''' </summary>
        ''' <param name="lineAlignment">要绘制此项的行的对齐方式。</param>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="lineAlignment" /> 不是有效的 <see cref="AlignmentX" /> 值。</exception>
        Public Function GetTextAlignment(lineAlignment As TextAlignment) As TextAlignment
            Select Case Alignment
                Case LineItemAlignment.Default
                    Return lineAlignment
                Case LineItemAlignment.Left : Return TextAlignment.Left
                Case LineItemAlignment.Center : Return TextAlignment.Center
                Case LineItemAlignment.Right : Return TextAlignment.Right
                Case Else
                    Debug.Assert(False)
                    Return Nothing
            End Select
        End Function

        Protected MustOverride Function GetTextCore(source As Compiled.ObjectModel.CompiledContainer) As String

        Shared Sub New()
            If Latinized.CanFreeze Then Latinized.Freeze()
            If Alphabetic.CanFreeze Then Alphabetic.Freeze()
            If Lyrics.CanFreeze Then Lyrics.Freeze()
            If Translation.CanFreeze Then Translation.Freeze()
        End Sub
    End Class

    ''' <summary>
    ''' 表示歌词行中与语义段的内容相关的项的格式信息。
    ''' </summary>
    Public Class LineSegmentItemStyle
        Inherits LineItemStyle

        ''' <summary>
        ''' 标识 <see cref="AttachedTo" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly AttachedToProperty As DependencyProperty =
            DependencyProperty.Register("AttachedTo", GetType(LineItemAttachedLevel), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(LineItemAttachedLevel.Segment),
                                        Function(value)
                                            Dim v = DirectCast(value, LineItemAttachedLevel)
                                            Return v = LineItemAttachedLevel.Line Or
                                                v = LineItemAttachedLevel.Span Or
                                                v = LineItemAttachedLevel.Segment
                                        End Function)
        ''' <summary>
        ''' 标识 <see cref="TextSource" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly TextSourceProperty As DependencyProperty =
            DependencyProperty.Register("TextSource", GetType(Compiled.SegmentTextItem()), GetType(LineSegmentItemStyle),
                                        New PropertyMetadata(Nothing))

        ' ''' <summary>
        ' ''' 表示一个不可见的 <see cref="ItemStyle" />。
        ' ''' </summary>
        'Public Shared ReadOnly Invisible As New ItemStyle()

        ''FUTURE SNAP_TO_LYRICS property


        ''' <summary>
        ''' 获取/设置此项目的每段文本的作用范围。 
        ''' </summary>
        <Bindable(True), Category("Behavior")>
        Public Overrides Property AttachedTo As LineItemAttachedLevel
            Get
                Return DirectCast(GetValue(AttachedToProperty), LineItemAttachedLevel)
            End Get
            Set(ByVal value As LineItemAttachedLevel)
                SetValue(AttachedToProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置此项目的每段文本的作用范围。 
        ''' </summary>
        <Bindable(True), Category("Behavior")>
        Public Property TextSource As Compiled.SegmentTextItem()
            Get
                Return DirectCast(GetValue(TextSourceProperty), Compiled.SegmentTextItem())
            End Get
            Set(ByVal value As Compiled.SegmentTextItem())
                SetValue(TextSourceProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' 创建 <see cref="LineItemStyle" /> 的新实例。
        ''' </summary>
        Protected Overrides Function CreateInstanceCore() As System.Windows.Freezable
            Return New LineSegmentItemStyle
        End Function

        Protected Overrides Function GetTextCore(source As Compiled.ObjectModel.CompiledContainer) As String
            Select Case AttachedTo
                Case LineItemAttachedLevel.Line
                    Return DirectCast(source, Compiled.Line).GetText(TextSource)
                Case LineItemAttachedLevel.Span
                    Return If(TypeOf source Is Compiled.Span,
                              DirectCast(source, Compiled.Span).GetText(TextSource),
                              GetTextCore(source.Parent))
                Case LineItemAttachedLevel.Segment
                    Return If(TypeOf source Is Compiled.Segment,
                              DirectCast(source, Compiled.Segment).GetText(TextSource),
                              GetTextCore(source.Parent))
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class

    ''' <summary>
    ''' 表示歌词行中翻译项的格式信息。
    ''' </summary>
    Public Class LineTranslationItemStyle
        Inherits LineItemStyle

        ''' <summary>
        ''' 标识 <see cref="LocalizationLanguages" /> 依赖项属性。
        ''' </summary>
        Public Shared ReadOnly LocalizationLanguagesProperty As DependencyProperty = DependencyProperty.Register(
            "LocalizationLanguages", GetType(String()), GetType(LineTranslationItemStyle),
            New PropertyMetadata({Globalization.CultureInfo.CurrentCulture.Name}))

        ''' <summary>
        ''' 获取/设置在进行歌词本地化时使用的语言优先级列表。
        ''' </summary>
        Public Property LocalizationLanguages As IEnumerable(Of String)
            Get
                Return DirectCast(Me.GetValue(LocalizationLanguagesProperty), IEnumerable(Of String))
            End Get
            Set(value As IEnumerable(Of String))
                Me.SetValue(LocalizationLanguagesProperty, value)
            End Set
        End Property

        'FUTURE MERGE LocalizationLanguagesExpression

        ''' <summary>
        ''' 获取/设置在进行歌词本地化时使用的语言优先级列表，以逗号分隔。
        ''' </summary>
        ''' <remarks>此属性仅为支持 WPF 设计时输入语言优先级列表，在运行时，请使用 <see cref="LocalizationLanguages" />。</remarks>
        Public Property LocalizationLanguagesExpression As String
            Get
                If LocalizationLanguages Is Nothing Then
                    Return String.Empty
                Else
                    Return String.Join(",", LocalizationLanguages)
                End If
            End Get
            Set(value As String)
                If value = Nothing Then
                    LocalizationLanguages = Nothing
                Else
                    LocalizationLanguages = value.Split({","c}, StringSplitOptions.RemoveEmptyEntries)
                End If
            End Set
        End Property

        Protected Overrides Function GetTextCore(source As Compiled.ObjectModel.CompiledContainer) As String
            Return DirectCast(source, Compiled.Line).LocalizedText(LocalizationLanguages)
        End Function

        Public Overrides Property AttachedTo As LineItemAttachedLevel
            Get
                Return LineItemAttachedLevel.Line
            End Get
            Set(value As LineItemAttachedLevel)
                Throw New NotSupportedException
            End Set
        End Property

        Protected Overrides Function CreateInstanceCore() As System.Windows.Freezable
            Return New LineTranslationItemStyle
        End Function
    End Class

    ''' <summary>
    ''' 表示一个包含 <see cref="LineItemStyle" /> 的集合。
    ''' </summary>
    Public Class LineItemStyleCollection
        Inherits FreezableCollection(Of LineItemStyle)

        ''' <summary>
        ''' 表示一个空的、已被冻结的集合。
        ''' </summary>
        Public Shared ReadOnly Empty As New LineItemStyleCollection

        ''' <summary>
        ''' 表示一个包含默认组合的、已被冻结的集合。
        ''' </summary>
        Public Shared ReadOnly [Default] As New LineItemStyleCollection

        Shared Sub New()
            If Empty.CanFreeze Then Empty.Freeze()
            With [Default]
                .Add(LineItemStyle.Latinized)
                .Add(LineItemStyle.Alphabetic)
                .Add(LineItemStyle.Lyrics)
                .Add(LineItemStyle.Translation)
                If .CanFreeze Then .Freeze()
            End With
        End Sub
    End Class

    ''' <summary>
    ''' 指示歌词行的项对齐方式。
    ''' </summary>
    Public Enum LineItemAlignment
        ''' <summary>
        ''' 使用行指定的默认对齐方式。
        ''' </summary>
        [Default] = 0

        ''' <summary>
        ''' 左对齐。
        ''' </summary>
        Left

        ''' <summary>
        ''' 居中对齐。
        ''' </summary>
        Center

        ''' <summary>
        ''' 右对齐。
        ''' </summary>
        Right
    End Enum

    ''' <summary>
    ''' 表示项的每段文本的作用范围。
    ''' </summary>
    Public Enum LineItemAttachedLevel
        Segment = 0
        Span
        Line
    End Enum
End Namespace