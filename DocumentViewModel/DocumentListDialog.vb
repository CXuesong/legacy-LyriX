''' <summary>
''' 表示此文件对话框的用途。
''' </summary>
Public Enum DocumentListUsage
    ''' <summary>
    ''' 一般用途。
    ''' </summary>
    Normal = 0
    ''' <summary>
    ''' 用作“是否保存”对话框。
    ''' </summary>
    SaveDocuments
End Enum

''' <summary>
''' 用于显示文档列表，并面向用户提供相关操作的对话框。
''' </summary>
Public Class DocumentListDialog
    Private Class DocumentListItemComparer
        Implements IComparer

        Private m_HeaderIndex As Integer
        Private m_Ascending As Boolean

        ''' <summary>
        ''' 指定要进行比较的项目的列，以零为下限。
        ''' </summary>
        Public Property HeaderIndex() As Integer
            Get
                Return m_HeaderIndex
            End Get
            Set(ByVal value As Integer)
                m_HeaderIndex = value
            End Set
        End Property

        ''' <summary>
        ''' 指定应该按照升序（递增）还是降序（递减）比较项目。
        ''' </summary>
        Public Property Ascending() As Boolean
            Get
                Return m_Ascending
            End Get
            Set(ByVal value As Boolean)
                m_Ascending = value
            End Set
        End Property

        Private Function GetColumnString(ByVal Item As ListViewItem, ByVal Index As Integer) As String
            If m_HeaderIndex > Item.SubItems.Count - 1 Then
                Return Nothing
            Else
                Return Item.SubItems(m_HeaderIndex).Text
            End If
        End Function

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim XValue = GetColumnString(DirectCast(x, ListViewItem), m_HeaderIndex)
            Dim YValue = GetColumnString(DirectCast(y, ListViewItem), m_HeaderIndex)
            If IsNumeric(XValue) Then
                If IsNumeric(YValue) Then
                    Return Math.Sign(If(m_Ascending, CDbl(XValue) - CDbl(YValue), CDbl(YValue) - CDbl(XValue)))
                Else
                    Return If(m_Ascending, 1, -1)   '认为数字比字符优先
                End If
            ElseIf IsNumeric(YValue) Then
                Return If(m_Ascending, -1, 1)   '认为数字比字符优先
            Else
                Return If(m_Ascending, 1, -1) * String.Compare(XValue, YValue)
            End If
        End Function

        Public Sub New(ByVal HeaderIndex As Integer)
            Me.New(HeaderIndex, True)
        End Sub

        Public Sub New(ByVal HeaderIndex As Integer, ByVal Ascending As Boolean)
            m_HeaderIndex = HeaderIndex
            m_Ascending = Ascending
        End Sub
    End Class

    Private m_Documents As IEnumerable(Of Document)
    Private m_VisibleDocuments As IEnumerable(Of Document)
    Private m_DocumentsStatus As IDictionary(Of Document, String)

    Private m_DialogUsage As DocumentListUsage

    Private DocumentCount As Integer
    Private VisibleDocumentCount As Integer

    ''' <summary>
    ''' 获取/设置要显示在列表中的所有文档集合。
    ''' </summary>
    Public Property Documents() As IEnumerable(Of Document)
        Get
            Return m_Documents
        End Get
        Set(ByVal value As IEnumerable(Of Document))
            m_Documents = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置在默认情况下显示在列表中的文档集合。
    ''' </summary>
    Public Property VisibleDocuments() As IEnumerable(Of Document)
        Get
            Return m_VisibleDocuments
        End Get
        Set(ByVal value As IEnumerable(Of Document))
            If m_Documents Is Nothing Then
                Throw New InvalidOperationException(String.Format(ExceptionPrompts.EmptyList, "Documents"))
            ElseIf m_Documents.Except(value).Any Then
                Throw New ArgumentOutOfRangeException(ExceptionPrompts.VisibleDocumentOutOfList)
            Else
                m_VisibleDocuments = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置自定义文档状态的集合。
    ''' </summary>
    Public Property DocumentsStatus() As IDictionary(Of Document, String)
        Get
            Return m_DocumentsStatus
        End Get
        Set(ByVal value As IDictionary(Of Document, String))
            m_DocumentsStatus = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置一个值，指示是否应将此对话框作何用途。
    ''' </summary>
    Public Property DialogUsage() As DocumentListUsage
        Get
            Return m_DialogUsage
        End Get
        Set(ByVal value As DocumentListUsage)
            m_DialogUsage = value
        End Set
    End Property

    Private Sub FillDocumentList(ByVal DocList As IEnumerable(Of Document))
        Dim DocImageIdDictionary As New Dictionary(Of Type, Integer)
        Dim DocInfoDictionary As New Dictionary(Of Type, DocumentInformation)
        '清理
        DocumentList.Items.Clear()
        DocumentLargeImageList.Images.Clear()
        DocumentSmallImageList.Images.Clear()
        If DocList IsNot Nothing Then
            '显示文档
            DocumentList.BeginUpdate()
            For Each EachDoc In DocList
                Dim EachDoc_ = EachDoc  '为 lambda 表达式而复制计数器
                If Not DocInfoDictionary.ContainsKey(EachDoc.GetType) Then
                    Dim DocInfo = DocumentInformation.GetInfo(EachDoc.GetType)
                    Dim DocIcon = If(DocInfo.Icon, DocumentInformation.DefaultIcon)
                    DocInfoDictionary.Add(EachDoc.GetType, DocInfo)
                    If DocIcon Is Nothing Then
                        DocImageIdDictionary.Add(EachDoc.GetType, -1)
                    Else
                        DocumentLargeImageList.Images.Add(DocIcon)
                        DocumentSmallImageList.Images.Add(DocIcon)
                        DocImageIdDictionary.Add(EachDoc.GetType, DocumentSmallImageList.Images.Count - 1)
                    End If
                End If
                With DocumentList.Items.Add(EachDoc.Title, DocImageIdDictionary.Item(EachDoc.GetType))
                    Dim DocStatus As String = Nothing
                    .Tag = EachDoc
                    .SubItems.Add(DocInfoDictionary.Item(EachDoc.GetType).DocumentName, SystemColors.GrayText, Nothing, Nothing)
                    If m_DocumentsStatus Is Nothing OrElse Not m_DocumentsStatus.TryGetValue(EachDoc, DocStatus) Then
                        DocStatus = If(EachDoc.IsModified, Prompts.Modified, Prompts.Normal)
                    End If
                    .SubItems.Add(DocStatus, SystemColors.GrayText, Nothing, Nothing)
                    .SubItems.Add(CStr(EachDoc.Views.Count), SystemColors.GrayText, Nothing, Nothing)
                    .SubItems.Add(If(EachDoc.IOContext Is Nothing, Nothing, EachDoc.IOContext.ToString), SystemColors.GrayText, Nothing, Nothing)
                    .Checked = True
                End With
            Next
            DocumentList.EndUpdate()
        End If
    End Sub

    Private Sub ShowDocuments(ByVal ShowAll As Boolean)
        If DocumentCount = 0 Then
            PromptLabel.Text = Prompts.NoDocument
        ElseIf Not ShowAll AndAlso VisibleDocumentCount = 0 Then
            PromptLabel.Text = String.Format(Prompts.NoVisibleDocument, DocumentCount)
        ElseIf m_DialogUsage = DocumentListUsage.SaveDocuments Then   '“保存”对话框
            PromptLabel.Text = String.Format(Prompts.SaveDocumentCount, VisibleDocumentCount)
        ElseIf ShowAll OrElse VisibleDocumentCount = DocumentCount Then
            PromptLabel.Text = String.Format(Prompts.DocumentCount, DocumentCount)
        Else
            PromptLabel.Text = String.Format(Prompts.DocumentCountEx, DocumentCount, VisibleDocumentCount)
        End If

        FillDocumentList(If(m_VisibleDocuments Is Nothing Or ShowAll, m_Documents, m_VisibleDocuments))
    End Sub

    Private Sub PreviewDocument(ByVal Document As Document)
        If Document.MainView IsNot Nothing Then Document.MainView.Preview()
    End Sub

    Private Function SaveDocuments(ByVal DocumentList As IEnumerable(Of ListViewItem)) As Boolean
        For Each EachItem In DocumentList
            EachItem.SubItems(2).Text = Prompts.Saving
            EachItem.EnsureVisible()
            PreviewDocument(DirectCast(EachItem.Tag, Document))
            If Not DirectCast(EachItem.Tag, Document).SaveModified(True) Then
                EachItem.SubItems(2).Text = Prompts.Modified
                Return False
            Else
                EachItem.SubItems(2).Text = Prompts.Normal
            End If
        Next
        Return True
    End Function

    Private Sub DocumentListDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '列标头
        DocumentList.Columns.Add(Prompts.Name, 140)
        DocumentList.Columns.Add(Prompts.Type, 80)
        DocumentList.Columns.Add(Prompts.Status, 70)
        DocumentList.Columns.Add(Prompts.ViewCount, 70)
        DocumentList.Columns.Add(Prompts.IOContext, 250)

        If m_DialogUsage = DocumentListUsage.SaveDocuments Then
            '“保存”对话框
            If m_Documents IsNot Nothing AndAlso m_VisibleDocuments Is Nothing Then
                '将显示的文档设置为未保存的文档
                m_VisibleDocuments = m_Documents.Where(Function(EachDoc) EachDoc.IsModified)
            End If
        End If

        DocumentCount = If(m_Documents Is Nothing, 0, m_Documents.Count)
        VisibleDocumentCount = If(m_VisibleDocuments Is Nothing, DocumentCount, m_VisibleDocuments.Count)

        Select Case m_DialogUsage
            Case DocumentListUsage.SaveDocuments    '“保存”对话框
                PromptLabel.Text = String.Format(Prompts.SaveDocumentCount, DocumentCount)
                SaveButton.Text = Prompts.Yes
                CloseDocButton.Text = Prompts.NoButton
                CloseButton.Text = Prompts.Cancel

                Me.AcceptButton = SaveButton

                DocumentList.CheckBoxes = True
        End Select

        If DocumentList.CheckBoxes Then ViewTile.Enabled = False '平铺视图不支持复选框

        If m_VisibleDocuments Is Nothing Then
            ShowAllDocuments.Visible = False
        Else
            ShowAllDocuments.Visible = True
        End If

        ViewDetails.PerformClick()

        ShowDocuments(False)
    End Sub

    Private Sub ViewModes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewLargeIcon.Click, ViewSmallIcon.Click, ViewList.Click, ViewDetails.Click, ViewTile.Click, ViewThumb.Click
        '清除菜单的选中状态
        ViewLargeIcon.Checked = False
        ViewSmallIcon.Checked = False
        ViewList.Checked = False
        ViewDetails.Checked = False
        ViewTile.Checked = False
        ViewThumb.Checked = False
        '判断菜单
        Dim NewView As View
        If sender Is ViewLargeIcon Then
            NewView = View.LargeIcon
            ViewLargeIcon.Checked = True
        ElseIf sender Is ViewSmallIcon Then
            NewView = View.SmallIcon
            ViewSmallIcon.Checked = True
        ElseIf sender Is ViewList Then
            NewView = View.List
            ViewList.Checked = True
        ElseIf sender Is ViewDetails Then
            NewView = View.Details
            ViewDetails.Checked = True
        ElseIf sender Is ViewTile Then
            NewView = View.Tile
            ViewTile.Checked = True
        ElseIf sender Is ViewThumb Then
            MsgBox("有图片么？", MsgBoxStyle.Question)
            ViewLargeIcon.PerformClick()
        End If
        DocumentList.View = NewView
    End Sub

    Private Sub ShowAllDocuments_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowAllDocuments.CheckedChanged
        ShowDocuments(ShowAllDocuments.Checked)
    End Sub

    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Select Case m_DialogUsage
            Case DocumentListUsage.SaveDocuments
                '保存选中的文档
                If SaveDocuments(DocumentList.CheckedItems.Cast(Of ListViewItem)) Then
                    Me.DialogResult = Forms.DialogResult.Yes
                End If
            Case Else
                SaveDocuments(DocumentList.SelectedItems.Cast(Of ListViewItem))
        End Select
    End Sub

    Private Sub CloseDocButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseDocButton.Click
        If m_DialogUsage = DocumentListUsage.SaveDocuments Then
            '保存未选中的文档
            If SaveDocuments(DocumentList.Items.Cast(Of ListViewItem).Except(DocumentList.CheckedItems.Cast(Of ListViewItem))) Then
                Me.DialogResult = Forms.DialogResult.No
            End If
        Else
            '提示保存，然后关闭文档
            Dim SelectedDocuments = DocumentList.SelectedItems.Cast(Of ListViewItem).Select(Function(EachItem) DirectCast(EachItem.Tag, Document))
            SelectedDocuments.SaveModified()
            SelectedDocuments.CloseAll()
        End If
    End Sub

    Private Sub DocumentList_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles DocumentList.ColumnClick
        If TypeOf DocumentList.ListViewItemSorter Is DocumentListItemComparer Then
            With DirectCast(DocumentList.ListViewItemSorter, DocumentListItemComparer)
                If .HeaderIndex <> e.Column Then
                    .HeaderIndex = e.Column
                    .Ascending = True
                Else
                    .Ascending = Not .Ascending
                End If
                DocumentList.Sort()
            End With
        Else
            DocumentList.ListViewItemSorter = New DocumentListItemComparer(e.Column)
        End If
    End Sub

    Private Sub DocumentList_ItemMouseHover(ByVal sender As Object, ByVal e As System.Windows.Forms.ListViewItemMouseHoverEventArgs) Handles DocumentList.ItemMouseHover
        PreviewDocument(DirectCast(e.Item.Tag, Document))
    End Sub

    Private Sub DocumentListDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Select Case m_DialogUsage
            Case DocumentListUsage.SaveDocuments
                SaveButton.Focus()
        End Select
    End Sub
End Class