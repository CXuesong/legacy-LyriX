Imports System.Runtime.CompilerServices


Public Module DVMExtensions
    ''' <summary>
    ''' 将多个 <see cref="FileType" /> 合并。
    ''' </summary>
    ''' <param name="Source">要进行合并的。<see cref="IEnumerable(Of FileType)" /> 。</param>
    <Extension()> Public Function Join(ByVal Source As IEnumerable(Of FileType)) As FileType
        Dim RDescription As String = Nothing, RFilter As String = Nothing
        If Source.Count > 0 Then
            For Each EachType As FileType In Source
                RDescription += If(RDescription = Nothing, Nothing, ", ") & EachType.Description
                RFilter += If(RFilter = Nothing, Nothing, ";") & EachType.Filter
            Next
        End If
        Return New FileType With {.Description = RDescription, .Filter = RFilter}
    End Function

    ''' <summary>
    ''' 将多个 <see cref="IEnumerable(Of FileType)" /> 转换为可以被文件对话框识别的过滤器字符串。
    ''' </summary>
    ''' <param name="Source">要进行转换的<see cref="IEnumerable(Of FileType)" /> 。</param>
    ''' <returns>一个 String，包含了可以被文件对话框识别的过滤器。</returns>
    <Extension()> Public Function ToFilterDescription(ByVal Source As IEnumerable(Of FileType)) As String
        Dim ReturnString As String = Nothing
        If Source.Count > 0 Then
            For Each EachType As FileType In Source
                ReturnString += EachType.ToString & "|"
            Next
            ReturnString = ReturnString.Substring(0, ReturnString.Length - 1)
        End If
        Return ReturnString
    End Function

    ''' <summary>
    ''' 将多个 <see cref="IEnumerable(Of FileType)" /> 转换为可以被用户识别的描述性字符串。
    ''' </summary>
    ''' <param name="Source">要进行转换的 IEnumerable(Of FileType)。</param>
    ''' <returns>一个 String，包含了可以被文件对话框识别的过滤器。</returns>
    <Extension()> Public Function ToDescription(ByVal Source As IEnumerable(Of FileType)) As String
        Dim ReturnString As String = Nothing
        If Source.Count > 0 Then
            For Each EachType As FileType In Source
                ReturnString += EachType.Description & "（" & EachType.Filter & "）" & ";"
            Next
            ReturnString = ReturnString.Substring(0, ReturnString.Length - 1)
        End If
        Return ReturnString
    End Function

    ''' <summary>
    ''' 更新视图。
    ''' </summary>
    ''' <param name="dest">要更新的目标视图，</param>
    <Extension()> Public Sub Update(ByVal dest As IDocumentView)
        Update(dest, Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' 更新视图。
    ''' </summary>
    ''' <param name="dest">要更新的目标视图，</param>
    ''' <param name="sender">发起视图更新的视图，如果为 <c>null</c>，则表示发起者未知。</param>
    <Extension()> Public Sub Update(ByVal dest As IDocumentView, ByVal sender As IDocumentView)
        Update(dest, sender, Nothing)
    End Sub

    ''' <summary>
    ''' 更新视图。
    ''' </summary>
    ''' <param name="dest">要更新的目标视图，</param>
    ''' <param name="context">由调用方定义的附加上下文，可为 <c>null</c>。</param>
    <Extension()> Public Sub Update(ByVal dest As IDocumentView, ByVal context As Object)
        Update(dest, Nothing, context)
    End Sub

    ''' <summary>
    ''' 更新视图。
    ''' </summary>
    ''' <param name="dest">要更新的目标视图，</param>
    ''' <param name="sender">发起视图更新的视图，如果为 <c>null</c>，则表示发起者未知。</param>
    ''' <param name="context">由调用方定义的附加上下文，可为 <c>null</c>。</param>
    <Extension()> Public Sub Update(ByVal dest As IDocumentView, ByVal sender As IDocumentView, ByVal context As Object)
        dest.Update(New UpdateContext(If(sender Is Nothing, UpdateReason.Unknown, UpdateReason.View), sender, context))
    End Sub

    ''' <summary>
    ''' 根据需要显示一个“文档保存”对话框，提示用户保存指定列表中未保存的文档。
    ''' </summary>
    ''' <param name="Documents">指定了所有的文档集合，其中可能包含了为保存的文件。如果是这样，则会显示“文档保存”对话框。</param>
    ''' <returns>返回 True 表示列表中的文档已经被保存；返回 False 表示列表中的文档至少有一个尚未保存（可能被取消）。</returns>
    <Extension()> Public Function SaveModified(ByVal Documents As IEnumerable(Of Document)) As Boolean
        If Documents.Any(Function(EachDocument) EachDocument.IsModified) Then
            If Documents.Skip(1).Any Then
                Using Prompter As New DocumentListDialog
                    Prompter.DialogUsage = DocumentListUsage.SaveDocuments
                    Prompter.Documents = Documents
                    Select Case Prompter.ShowDialog
                        Case DialogResult.Cancel
                            Return False
                        Case Else
                            Return True
                    End Select
                End Using
            Else
                Return Documents.First.SaveModified
            End If
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' 在不提示的情况下，关闭列表中的所有文档。
    ''' </summary>
    ''' <param name="Documents">指定了所有的文档集合，其中可能包含了为保存的文件。如果是这样，则会显示“文档保存”对话框。</param>
    ''' <remarks><para>存在于文档列表中所有文档的 <see cref="Document.Close" /> 将被调用。</para>
    ''' <para>在关闭前，若需要提示用户保存，请使用 <see cref="SaveModified" />。</para></remarks>
    <Extension()> Public Sub CloseAll(ByVal Documents As IEnumerable(Of Document))
        '复制列表以避免参数提供的列表发生变化
        If Documents.Any Then
            For Each EachDocument In New List(Of Document)(Documents)
                If EachDocument IsNot Nothing Then EachDocument.Close()
            Next
        End If
    End Sub

    '''' <summary>
    '''' 转换文档格式（如果支持）。
    '''' </summary>
    '''' <param name="Source">待转换的文档。</param>
    '''' <param name="Destination">要转换到的目标模板，被转换的文档将会被添加进去。</param>
    '<Extension()> Public Function Convert(ByVal Source As Document, ByVal Destination As DocumentTemplate) As Document
    '    Dim tempContext As New MemoryIOContext
    '    Source.ExportDocument(tempContext)
    '    Return Destination.OpenDocument(tempContext)
    'End Function
End Module