Imports LyriX.Document
Imports System.Text
Imports <xmlns:let="LyriX/2011/editorTags.xsd">

Friend Module DocumentUtility

    Public ReadOnly XNSegmentTextType As XName = GetXmlNamespace(let).GetName("ct")
    Public ReadOnly XNMusicSource As XName = GetXmlNamespace(let).GetName("musicSrc")

    Private Enum SegmentTextType
        Normal = 0
        LetterDigit = 1
        OtherLetter = 2
        Separator = 3
    End Enum

    ''' <summary>
    ''' 尝试划分文本的语义段。
    ''' </summary>
    Public Function SplitSegments(text As String) As IEnumerable(Of Segment)
        If text = Nothing Then
            Return Enumerable.Empty(Of Segment)()
        Else
            Dim TextBuilder As New StringBuilder
            Dim TextSegments As New List(Of Segment)
            'FUTURE 根据语言划分
            '现行：汉字属于 OtherLetter[Lo (Letter,other)]，按字划分
            '其他的 Letter/Digit 放到一起
            Dim LastCharType = SegmentTextType.Normal
            For I = 0 To text.Length
                If I = text.Length Then GoTo FLUSH

                Dim EachChar = text.Chars(I)
                Dim CurrentCharType = SegmentTextType.Normal
                If Char.GetUnicodeCategory(EachChar) = Globalization.UnicodeCategory.OtherLetter Then
                    CurrentCharType = SegmentTextType.OtherLetter
                ElseIf Char.IsLetterOrDigit(EachChar) Then
                    CurrentCharType = SegmentTextType.LetterDigit
                ElseIf Char.IsSeparator(EachChar) Then
                    CurrentCharType = SegmentTextType.Separator
                End If
                If CurrentCharType = SegmentTextType.OtherLetter OrElse
                    CurrentCharType <> LastCharType Then
FLUSH:
                    '分隔语义段
                    If TextBuilder.Length > 0 Then
                        Dim NewSegment As New Segment(TextBuilder.ToString)
                        NewSegment.XTags.Add(New XAttribute(XNSegmentTextType, CInt(LastCharType)))
                        TextSegments.Add(NewSegment)
                        'RESET
                        TextBuilder.Clear()
                    End If
                End If
                LastCharType = CurrentCharType
                TextBuilder.Append(EachChar)
            Next
            '收尾
            Return TextSegments
        End If
    End Function

    ''' <summary>
    ''' 合并段。
    ''' </summary>
    Public Function MergeSpans(spans As IEnumerable(Of Span)) As Span
        Debug.Assert(spans IsNot Nothing)
        Dim NewSpan As New Document.Span
        Dim IsFirst As Boolean = True
        Dim newBegin As TimeSpan, newEnd As TimeSpan
        For Each EachSpan In spans
            If NewSpan.Id Is Nothing Then NewSpan.Id = EachSpan.Id
            If NewSpan.Language Is Nothing Then NewSpan.Language = EachSpan.Language
            If IsFirst Then
                newBegin = EachSpan.Begin
                newEnd = EachSpan.End
            Else
                If EachSpan.Begin < newBegin Then newBegin = EachSpan.Begin
                If EachSpan.End > newEnd Then newEnd = EachSpan.End
            End If
            For Each EachSegment In EachSpan.Segments
                NewSpan.Segments.Add(DirectCast(EachSegment.Clone, Segment))
            Next
            IsFirst = False
        Next
        NewSpan.Begin = newBegin
        NewSpan.End = newEnd
        Return NewSpan
    End Function

    Public Function GetSpanText(span As Span) As String
        Return String.Join(Nothing, From EachSegment In span.Segments
                                            Select ET = EachSegment.Text)
    End Function

    'position 以 GetSpanText 为准
    Public Function SplitSpan(span As Span, position As Integer, newSpanIndex As Integer, allowAdjustment As Boolean) As Span()
        Debug.Assert(span IsNot Nothing)
        Debug.Assert(position >= 0)
        Dim spans() As Span
        If position = 0 Then
            spans = ({New Span(newSpanIndex), DirectCast(span.Clone, Span)})
        Else
            spans = {New Span(span.Id, span.Language, span.Begin, span.Duration),
                       New Span(newSpanIndex, span.Language, span.End, TimeSpan.Zero)}
            Dim curpos As Integer = 0
            Dim NextSpanFirstSegmentIndex As Integer = -1
            '查找断点位置
            'Eg.
            '   0那1些2|因3为4年5轻|犯的错
            '   0那1些2|因3为4 5年6轻|犯的错
            For I = 0 To span.Segments.Count - 1
                Dim EachSegment = span.Segments(I)
                Dim nextpos = curpos + EachSegment.Text.Length  '算上这一段以后的位置
                Debug.Assert(curpos <= position)
                If nextpos = position Then
                    '刚好
                    spans(0).Segments.Add(DirectCast(EachSegment.Clone, Segment))
                    NextSpanFirstSegmentIndex = I + 1
                    Exit For
                ElseIf nextpos > position Then
                    '超了，要截断
                    spans(0).Segments.Add(New Segment(EachSegment.Language, EachSegment.Latinized,
                                                      EachSegment.Alphabetic, Left(EachSegment.Text, position - curpos)))
                    spans(1).Segments.Add(New Segment(EachSegment.Language, EachSegment.Latinized,
                                  EachSegment.Alphabetic, Mid(EachSegment.Text, position - curpos + 1)))
                    NextSpanFirstSegmentIndex = I + 1
                    Exit For
                Else
                    spans(0).Segments.Add(DirectCast(EachSegment.Clone, Segment))
                End If
                curpos = nextpos
            Next
            If NextSpanFirstSegmentIndex >= 0 Then
                Dim isSpaceBeginning As Boolean = True
                For I = NextSpanFirstSegmentIndex To span.Segments.Count - 1
                    Dim EachSegment = span.Segments(I)
                    '把空格放到前面一段的结束处
                    Dim segmentType = CType(CType(EachSegment.XTags.GetAttribute(XNSegmentTextType), Integer?).GetValueOrDefault, SegmentTextType)
                    If allowAdjustment AndAlso isSpaceBeginning AndAlso segmentType = SegmentTextType.Separator Then
                        spans(0).Segments.Add(DirectCast(EachSegment.Clone, Segment))
                    Else
                        isSpaceBeginning = False
                        spans(1).Segments.Add(DirectCast(EachSegment.Clone, Segment))
                    End If
                Next
            End If
        End If
        Return spans
    End Function
End Module
