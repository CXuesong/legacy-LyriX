Imports LyriX.Players.ObjectModel

Namespace Players
    ''' <summary>
    ''' 为 <see cref="SingleTrackLyricsPlayer.CurrentSegmentChanged" /> 提供数据。
    ''' </summary>
    Public Class CurrentSegmentChangedEventArgs
        Inherits EventArgs

        Private m_OldLineIndex As IndexChain
        Private m_OldSpanIndex As IndexChain
        Private m_OldSegmentIndex As IndexChain

        ''' <summary>
        ''' 获取发生改变前，当前行的位置。
        ''' </summary>
        ''' <value>发生改变前，当前行在列表中从零开始的位置。</value>
        Public ReadOnly Property OldLineIndex As IndexChain
            Get
                Return m_OldLineIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取发生改变前，当前段的位置。
        ''' </summary>
        ''' <value>发生改变前，当前段在当前行的段列表中从零开始的位置。</value>
        Public ReadOnly Property OldSpanIndex As IndexChain
            Get
                Return m_OldSpanIndex
            End Get
        End Property

        Public ReadOnly Property OldSegmentIndex As IndexChain
            Get
                Return m_OldSegmentIndex
            End Get
        End Property

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        Public Sub New(ByVal oldLineIndex As IndexChain, ByVal oldSpanIndex As IndexChain, oldSegmentIndex As IndexChain)
            m_OldLineIndex = oldLineIndex
            m_OldSpanIndex = oldSpanIndex
            m_OldSegmentIndex = oldSegmentIndex
        End Sub
    End Class

    ''' <summary>
    ''' 表示一个包含前一个、当前、下一个项目的索引链。
    ''' </summary>
    ''' <remarks>有时仅使用当前索引可能无法确定播放位置，例如，如果当前位置没有任何段或行项目，但在此时间之前与之后存在项目，则使用索引链即可得知。</remarks>
    Public Structure IndexChain
        Implements IEquatable(Of IndexChain), IComparable(Of IndexChain)

        Private m_Previous As Integer
        Private m_Current As Integer
        Private m_Next As Integer

        ''' <summary>
        ''' 表示一个空的索引链。
        ''' </summary>
        Public Shared ReadOnly Empty As New IndexChain(-1, -1, -1)

        ''' <summary>
        ''' 获取/设置上一个项目的索引。
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">索引链的索引顺序无效。</exception>
        Public Property Previous As Integer
            Get
                Return m_Previous
            End Get
            Set(ByVal value As Integer)
                If CheckIndices(value, m_Current, m_Next) = False Then
                    Throw New ArgumentOutOfRangeException("value")
                Else
                    m_Previous = Math.Max(-1, value)
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置当前项目的索引。
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">索引链的索引顺序无效。</exception>
        Public Property Current As Integer
            Get
                Return m_Current
            End Get
            Set(ByVal value As Integer)
                If CheckIndices(m_Previous, value, m_Next) = False Then
                    Throw New ArgumentOutOfRangeException("value")
                Else
                    m_Current = Math.Max(-1, value)
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置下一个项目的索引。
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">索引链的索引顺序无效。</exception>
        Public Property [Next] As Integer
            Get
                Return m_Next
            End Get
            Set(ByVal value As Integer)
                If CheckIndices(m_Previous, m_Current, value) = False Then
                    Throw New ArgumentOutOfRangeException("value")
                Else
                    m_Next = Math.Max(-1, value)
                End If
            End Set
        End Property

        'Public Function SetIndices(ByVal previous As Integer, ByVal current As Integer, ByVal [next] As Integer) As Boolean

        ''' <summary>
        ''' 获取一个值，指示上一个项目的索引是否存在。
        ''' </summary>
        ''' <returns>如果<see cref="Current" /> 的值小于零，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        Public ReadOnly Property PreviousExists As Boolean
            Get
                Return m_Previous >= 0
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示当前项目的索引是否存在。
        ''' </summary>
        ''' <returns>如果<see cref="Current" /> 的值小于零，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        Public ReadOnly Property CurrentExists As Boolean
            Get
                Return m_Current >= 0
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示下一个项目的索引是否存在。
        ''' </summary>
        ''' <returns>如果<see cref="Current" /> 的值小于零，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        Public ReadOnly Property NextExists As Boolean
            Get
                Return m_Next >= 0
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示当前索引链是否位于列表的首项之前。
        ''' </summary>
        ''' <returns>如果 <see cref="Previous" /> 与 <see cref="Current" /> 的值均小于零，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        Public ReadOnly Property BOF As Boolean
            Get
                Return m_Previous < 0 AndAlso m_Current < 0
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示当前索引链是否位于列表的末项之后。
        ''' </summary>
        ''' <returns>如果 <see cref="Current" /> 与 <see cref="[Next]" /> 的值均小于零，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        Public ReadOnly Property EOF As Boolean
            Get
                Return m_Next < 0 AndAlso m_Current < 0
            End Get
        End Property

        ''' <summary>
        ''' 获取一个值，指示当前索引链是否为空。
        ''' </summary>
        ''' <remarks>使索引链为空的充要条件是 <see cref="BOF" /> 与 <see cref="EOF" /> 均为 <c>true</c>。</remarks>
        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return m_Previous < 0 AndAlso m_Current < 0 AndAlso m_Next < 0
            End Get
        End Property

        ''' <summary>
        ''' 获取此索引链的文本表示形式。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return m_Previous & "," & m_Current & "," & m_Next
        End Function

        ''' <summary>
        ''' 用作特定类型的哈希函数。
        ''' </summary>
        Public Overrides Function GetHashCode() As Integer
            Return m_Previous Xor m_Current Xor m_Next
        End Function

        Private Shared Function CheckIndices(ByVal previous As Integer, ByVal current As Integer, ByVal [next] As Integer) As Boolean
            Return (previous < current AndAlso current < [next]) OrElse
                (current < 0 AndAlso (previous < [next] OrElse previous < 0 OrElse [next] < 0))
        End Function

        ''' <summary>
        ''' 指示此实例与指定对象是否相等。
        ''' </summary>
        Public Overloads Function Equals(ByVal other As IndexChain) As Boolean Implements System.IEquatable(Of IndexChain).Equals
            Return CompareTo(other) = 0
        End Function

        ''' <summary>
        ''' 指示此实例与指定对象是否相等。
        ''' </summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is IndexChain Then
                Return CompareTo(DirectCast(obj, IndexChain)) = 0
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 比较当前索引链和另一索引链的相对位置。
        ''' </summary>
        ''' <param name="other">与此对象进行比较的对象。</param>
        ''' <returns>
        ''' 一个值，指示要比较的对象的相对顺序。返回值的含义如下：
        ''' <list type="table">
        ''' <listheader><term>值</term><description>含义</description></listheader>
        ''' <item><term>小于零</term><description>此对象小于 other 参数。</description></item>
        ''' <item><term>零</term><description>此对象等于 other。</description></item>
        ''' <item><term>大于零</term><description>此对象大于 other。</description></item>
        ''' </list>
        ''' </returns>
        ''' <remarks></remarks>
        Public Function CompareTo(ByVal other As IndexChain) As Integer Implements System.IComparable(Of IndexChain).CompareTo
            If m_Current = other.m_Current AndAlso m_Previous = other.m_Previous AndAlso m_Next = other.m_Next Then
                '完全等效
                Return 0
                'STAGE1 判断是否为空，空比任何索引小
            ElseIf IsEmpty Then
                If other.IsEmpty Then
                    Return 0
                Else
                    Return -1
                End If
            ElseIf other.IsEmpty Then
                Return 1
                'this 与 other 都非空，BOF、EOF 不全为 true
                'STAGE2 判断 BOF、EOF
            ElseIf BOF Then
                If other.BOF Then
                    Return 0
                Else
                    'EOF 或 既不 BOF，也不 EOF
                    Return -1
                End If
            ElseIf EOF Then
                If other.EOF Then
                    Return 0
                Else
                    'BOF 或 既不 BOF，也不 EOF
                    Return 1
                End If
                'this 的 BOF、EOF 全为 false
            ElseIf other.BOF Then
                Return 1
            ElseIf other.EOF Then
                Return -1
                'this 与 other 都非空，BOF、EOF 全为 false
                'STAGE3 等效比较（1,-1,2 = 1.5）
            ElseIf m_Previous <= other.m_Previous AndAlso m_Next <= other.m_Next Then
                Return -1
            ElseIf m_Previous >= other.m_Previous AndAlso m_Next >= other.m_Next Then
                Return 1
            Else
                Throw New NotSupportedException
                'Return GetEquivantCurrent(Me).CompareTo(GetEquivantCurrent(other))
            End If
        End Function

        Public Shared Operator =(ByVal x As IndexChain, ByVal y As IndexChain) As Boolean
            Return x.CompareTo(y) = 0
        End Operator

        Public Shared Operator <>(ByVal x As IndexChain, ByVal y As IndexChain) As Boolean
            Return x.CompareTo(y) <> 0
        End Operator

        Public Shared Operator >(ByVal x As IndexChain, ByVal y As IndexChain) As Boolean
            Return x.CompareTo(y) > 0
        End Operator

        Public Shared Operator <(ByVal x As IndexChain, ByVal y As IndexChain) As Boolean
            Return x.CompareTo(y) < 0
        End Operator

        Public Shared Operator >=(ByVal x As IndexChain, ByVal y As IndexChain) As Boolean
            Return x.CompareTo(y) >= 0
        End Operator

        Public Shared Operator <=(ByVal x As IndexChain, ByVal y As IndexChain) As Boolean
            Return x.CompareTo(y) <= 0
        End Operator

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <exception cref="ArgumentException">索引链的索引顺序无效。</exception>
        Public Sub New(ByVal previous As Integer, ByVal current As Integer, ByVal [next] As Integer)
            If CheckIndices(previous, current, [next]) = False Then
                Throw New ArgumentException()
            Else
                m_Previous = Math.Max(-1, previous)
                m_Current = Math.Max(-1, current)
                m_Next = Math.Max(-1, [next])
            End If
        End Sub

        ''' <summary>
        ''' 初始化一个依次递增的索引链。
        ''' </summary>
        ''' <param name="current">当前索引。</param>
        Public Sub New(ByVal current As Integer)
            Me.New(current - 1, current, current + 1)
        End Sub

        ''' <summary>
        ''' 初始化一个不存在当前索引的索引链。
        ''' </summary>
        Public Sub New(ByVal previous As Integer, ByVal [next] As Integer)
            Me.New(previous, -1, [next])
        End Sub
    End Structure

    ''' <summary>
    ''' 用于处理所有部分使其在同一时间只出现一句歌词的播放逻辑。
    ''' </summary>
    Public Class SingleTrackLyricsPlayer
        Inherits LyricsPlayer

        ''' <summary>
        ''' 当前段发生变化时引发。
        ''' </summary>
        Public Event CurrentSegmentChanged(ByVal sender As Object, ByVal e As CurrentSegmentChangedEventArgs)

        Private m_Lines As Compiled.Line(),
            m_s_Lines As IList(Of Compiled.Line),
            m_LineIndex As IndexChain,
            m_SpanIndex As IndexChain,
            m_SegmentIndex As IndexChain,
            m_CurrentStepBegin As TimeSpan,
            m_CurrentStepEnd As TimeSpan

        ''' <summary>
        ''' 获取将要播放的歌词行。其项目按时间升序排列。
        ''' </summary>
        Public ReadOnly Property Lines As IList(Of Compiled.Line)
            Get
                Return m_s_Lines
            End Get
        End Property

        ''' <summary>
        ''' 获取当前行在列表中的从零开始的位置。
        ''' </summary>
        ''' <value>当前行在列表中的从零开始的位置。</value>
        Public ReadOnly Property LineIndex As IndexChain
            Get
                Return m_LineIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取当前段在当前行的段列表中的从零开始的位置。
        ''' </summary>
        ''' <value>当前段在当前行的段列表中的从零开始的位置。</value>
        Public ReadOnly Property SpanIndex As IndexChain
            Get
                Return m_SpanIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取当前语义段在当前段的语义段列表中的从零开始的位置。
        ''' </summary>
        ''' <returns>当前语义段在当前段的语义段列表中的从零开始的位置。如果不存在当前段，则为 -1。</returns>
        Public ReadOnly Property SegmentIndex As IndexChain
            Get
                Return m_SegmentIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取当前行。
        ''' </summary>
        Public ReadOnly Property CurrentLine As Compiled.Line
            Get
                Return If(m_LineIndex.CurrentExists, m_Lines(m_LineIndex.Current), Nothing)
            End Get
        End Property

        ''' <summary>
        ''' 获取当前段。
        ''' </summary>
        Public ReadOnly Property CurrentSpan As Compiled.Span
            Get
                Return If(m_LineIndex.CurrentExists AndAlso m_SpanIndex.CurrentExists, m_Lines(m_LineIndex.Current).Spans(m_SpanIndex.Current), Nothing)
            End Get
        End Property

        ''' <summary>
        ''' 获取当前语义段。
        ''' </summary>
        Public ReadOnly Property CurrentSegment As Compiled.Segment
            Get
                Return If(m_LineIndex.CurrentExists AndAlso
                          m_SpanIndex.CurrentExists AndAlso
                          m_SegmentIndex.CurrentExists,
                          m_Lines(m_LineIndex.Current).Spans(m_SpanIndex.Current).Segments(m_SegmentIndex.Current),
                          Nothing)
            End Get
        End Property

        ''' <summary>
        ''' 获取当前段或空白开始的时间。
        ''' </summary>
        Public ReadOnly Property CurrentStepBegin As TimeSpan
            Get
                Return m_CurrentStepBegin
            End Get
        End Property

        ''' <summary>
        ''' 获取当前段或空白持续的时间。
        ''' </summary>
        Public ReadOnly Property CurrentStepDuration As TimeSpan
            Get
                Return m_CurrentStepEnd - m_CurrentStepBegin
            End Get
        End Property

        ''' <summary>
        ''' 获取当前段或空白结束的时间。
        ''' </summary>
        ''' <remarks>只有到达此时间后，当前段与当前行的信息才会被更新。</remarks>
        Public ReadOnly Property CurrentStepEnd As TimeSpan
            Get
                Return m_CurrentStepEnd
            End Get
        End Property

        ''' <summary>
        ''' 获取当前时刻在当前段或空白的持续时间中所占的百分比。
        ''' </summary>
        Public ReadOnly Property StepProgress As Double
            Get
                Return (Position.Ticks - m_CurrentStepBegin.Ticks) / (m_CurrentStepEnd - m_CurrentStepBegin).Ticks
            End Get
        End Property

        ''' <summary>
        ''' 获取当前时刻在当前语义段的持续时间中所占的百分比。
        ''' </summary>
        Public ReadOnly Property SegmentProgress As Double
            Get
                Dim CSg = CurrentSegment
                If CSg Is Nothing Then
                    Return 0
                Else
                    Return (Position.Ticks - CSg.Begin.Ticks) / CSg.Duration.Ticks
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取当前时刻在当前段的持续时间中所占的百分比。
        ''' </summary>
        ''' <remarks>此属性为按语义段控制时间的近似处理提供了一个解决方案。</remarks>
        Public ReadOnly Property SpanProgress As Double
            Get
                Dim CSp = CurrentSpan
                If CSp Is Nothing Then
                    Return 0
                Else
                    Return (Position.Ticks - CSp.Begin.Ticks) / CSp.Duration.Ticks
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取当前时刻在当前行的持续时间中所占的百分比。
        ''' </summary>
        Public ReadOnly Property LineProgress As Double
            Get
                Dim CLn = CurrentLine
                If CLn Is Nothing Then
                    Return 0
                Else
                    Return (Position.Ticks - CLn.Begin.Ticks) / CLn.Duration.Ticks
                End If
            End Get
        End Property

        ''' <summary>
        ''' 跳转到指定的段。
        ''' </summary>
        ''' <param name="lineIndex">行的索引。</param>
        ''' <param name="spanIndex">段的索引。</param>
        ''' <param name="progress">要转到的时刻在指定段中的时间百分比。</param>
        Public Sub Seek(ByVal lineIndex As Integer, ByVal spanIndex As Integer, ByVal progress As Single)
            Seek(m_Lines(lineIndex).Spans(spanIndex), progress)
        End Sub

        ''' <summary>
        ''' 跳转到指定段的开始部分。
        ''' </summary>
        ''' <param name="lineIndex">行的索引。</param>
        ''' <param name="spanIndex">段的索引。</param>
        Public Sub Seek(ByVal lineIndex As Integer, ByVal spanIndex As Integer)
            Seek(m_Lines(lineIndex).Spans(spanIndex))
        End Sub

        ''' <summary>
        ''' 跳转到指定的段。
        ''' </summary>
        ''' <param name="span">要转到的段。</param>
        ''' <param name="progress">要转到的时刻在指定段中的时间百分比。</param>
        Public Sub Seek(ByVal span As Compiled.Span, ByVal progress As Single)
            '暂时为性能考虑，先不检查 span 是不是这个 Player 的
            If span Is Nothing Then
                Throw New ArgumentNullException("span")
            Else
                Position = New TimeSpan(span.Begin.Ticks + CInt(span.Duration.Ticks * progress))
            End If
        End Sub

        ''' <summary>
        ''' 跳转到指定段的开始部分。
        ''' </summary>
        ''' <param name="span">要转到的段。</param>
        Public Sub Seek(ByVal span As Compiled.Span)
            '暂时为性能考虑，先不检查 span 是不是这个 Player 的
            If span Is Nothing Then
                Throw New ArgumentNullException("span")
            Else
                Position = span.Begin
            End If
        End Sub

        Protected Overrides Sub OnPositionChanged(ByVal e As ObjectModel.PositionChangedEventArgs)
            If Position < m_CurrentStepBegin OrElse Position > m_CurrentStepEnd Then
                Dim oldLineIndex = m_LineIndex, oldSpanIndex = m_SpanIndex, oldSegmentIndex = m_SegmentIndex
                Dim oldLine = CurrentLine, oldSpan = CurrentSpan
                If oldLine Is Nothing OrElse Position < oldLine.Begin OrElse Position > oldLine.End Then
                    '需要重新确定行
                    '先设置为 EOF 的情况
                    m_LineIndex = New IndexChain(UBound(m_Lines), -1, -1)
                    For I = 0 To UBound(m_Lines)
                        Dim EachLine = m_Lines(I)
                        If Position > EachLine.End Then
                            '当前位置在此行之后
                        ElseIf Position < EachLine.Begin Then
                            '当前位置在此行之前
                            '如果是向后搜索，能到达此处说明已经越位了
                            '即当前行不存在
                            m_LineIndex = New IndexChain(I - 1, I)
                            m_CurrentStepBegin = If(m_LineIndex.BOF, TimeSpan.Zero, m_Lines(I - 1).End)
                            m_CurrentStepEnd = EachLine.Begin
                            Exit For
                        Else
                            '此行位于当前位置
                            '就是这行
                            m_LineIndex = New IndexChain(I)
                            '时间段由 span 确定
                            Exit For
                        End If
                    Next
                End If
                If m_LineIndex.EOF Then
                    'EOF
                    m_CurrentStepBegin = If(m_LineIndex.BOF, TimeSpan.Zero, m_Lines(m_LineIndex.Previous).End)
                    m_CurrentStepEnd = TimeSpan.MaxValue
                End If
                Dim newLine = CurrentLine
                If newLine Is Nothing Then
                    '当前行不存在
                    m_SpanIndex = IndexChain.Empty
                ElseIf oldSpan Is Nothing OrElse Position < oldSpan.Begin OrElse Position > oldSpan.End Then
                    '需要重新确定段
                    For I = 0 To newLine.Spans.Count - 1
                        Dim EachSpan = newLine.Spans(I)
                        If Position > EachSpan.End Then
                            '当前位置在此段之后
                        ElseIf Position < EachSpan.Begin Then
                            '当前位置在此段之前
                            '如果是向后搜索，能到达此处说明已经越位了
                            m_SpanIndex = New IndexChain(I - 1, I)
                            m_CurrentStepBegin = If(SpanIndex.BOF, TimeSpan.Zero, newLine.Spans(I - 1).End)
                            m_CurrentStepEnd = EachSpan.Begin
                            Exit For
                        Else
                            '此段位于当前位置
                            '就是这段
                            m_SpanIndex = New IndexChain(I)
                            '时间段由 segment 决定
                            Exit For
                        End If
                    Next
                End If
                Dim newSpan = CurrentSpan
                If newSpan Is Nothing Then
                    '当前段不存在
                    m_SegmentIndex = IndexChain.Empty
                Else
                    For I = 0 To newSpan.Segments.Count - 1
                        Dim EachSegment = newSpan.Segments(I)
                        If Position > EachSegment.End Then
                            '当前位置在此语义段之后
                        Else
                            'If Position < EachSegment.Begin Then
                            '    当前位置在此段之前
                            '    如果是向后搜索，能到达此处说明已经越位了
                            '    几乎不会运行到这里
                            '
                            '此段位于当前位置
                            '就是这段
                            m_SegmentIndex = New IndexChain(I)
                            m_CurrentStepBegin = EachSegment.Begin
                            m_CurrentStepEnd = EachSegment.End
                            Exit For
                        End If
                    Next
                End If
                'Debug.Print(Position.ToString & "====================" & vbCrLf & _
                '            'm_LineIndex.ToString & vbTab & m_SpanIndex.ToString & vbTab & m_SegmentIndex & vbCrLf & _
                ' vbTab & "BEG:" & m_CurrentStepBegin.ToString & vbCrLf & _
                ' vbTab & "END:" & m_CurrentStepEnd.ToString)
                OnCurrentSegmentChanged(New CurrentSegmentChangedEventArgs(oldLineIndex, oldSpanIndex, oldSegmentIndex))
            End If
            MyBase.OnPositionChanged(e)
        End Sub

        ''' <summary>
        ''' 重置当前的状态（只有状态）。
        ''' </summary>
        Protected Overrides Sub Reset()
            MyBase.Reset()
            m_LineIndex = IndexChain.Empty
            m_SpanIndex = IndexChain.Empty
            m_SegmentIndex = IndexChain.Empty
            m_CurrentStepBegin = TimeSpan.Zero
            m_CurrentStepEnd = TimeSpan.Zero
        End Sub

        ''' <summary>
        ''' 引发 <see cref="CurrentSegmentChanged" />。
        ''' </summary>
        Protected Overridable Sub OnCurrentSegmentChanged(ByVal e As CurrentSegmentChangedEventArgs)  'Overridable
            RaiseEvent CurrentSegmentChanged(Me, e)
        End Sub

        Protected Overrides Sub OnVersionChanged()
            If Me.Version Is Nothing Then
                m_Lines = {}
            Else
                With Me.Version
                    Select Case .Tracks.Count
                        Case 0
                            m_Lines = {}
                        Case 1
                            m_Lines = .Tracks(0).Lines.OrderBy(Function(EachLine) EachLine.Begin).ToArray
                        Case Else
                            '需要合并部分
                            m_Lines = .Tracks.SelectMany(Function(EachVp) EachVp.Lines).
                                OrderBy(Function(EachLine) EachLine.Begin).ToArray
                    End Select
                End With
            End If
            m_s_Lines = Array.AsReadOnly(m_Lines)
            MyBase.OnVersionChanged()
        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="document">要载入的歌词文档。</param>
        ''' <param name="duration">待播放的音乐的时间长度。</param>
        ''' <param name="version">要载入的歌词版本，如果为 <c>null</c>，则表示将尝试自动匹配。</param>
        Public Sub New(ByVal document As Compiled.LyricsDocument, duration As TimeSpan, version As Compiled.Version)
            MyBase.New(document, duration, version)
        End Sub

        ''' <summary>
        ''' 使用指定的歌词初始化并尝试自动匹配歌词版本。
        ''' </summary>
        ''' <param name="document">要载入的歌词文档。</param>
        ''' <param name="duration">待播放的音乐的时间长度。</param>
        Public Sub New(ByVal document As Compiled.LyricsDocument, duration As TimeSpan)
            Me.New(document, duration, Nothing)
        End Sub

        ''' <summary>
        ''' 使用指定的歌词初始化并尝试载入一个未指定歌曲长度的歌词版本。
        ''' </summary>
        ''' <param name="document">要载入的歌词文档。</param>
        Public Sub New(ByVal document As Compiled.LyricsDocument)
            Me.New(document, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' 使用指定的歌词版本初始化。
        ''' </summary>
        ''' <param name="version">要载入的歌词版本，如果为 <c>null</c>，则表示将尝试自动匹配。</param>
        Public Sub New(version As Compiled.Version)
            Me.New(Nothing, Nothing, version)
        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing, Nothing, Nothing)
        End Sub
    End Class
End Namespace