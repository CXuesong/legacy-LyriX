
Imports DocumentViewModel.Managers

''' <summary>
''' 提示用户选择文档模版的对话框。
''' </summary>
Friend Class DocumentTypeSelector
    Private m_DocumentTypes As IEnumerable(Of Type)
    Private m_OpenArguments As IEnumerable(Of DocumentOpenArguments)
    Private m_SelectedIndex As Integer = -1
    Private IsPreset As Boolean     '标志，表示当前是否正在预设选项，而不是由用户设定

    ''' <summary>
    ''' 获取/设置要让用户进行选择的文档类型的列表。
    ''' </summary>
    Public Property DocumentTypes() As IEnumerable(Of Type)
        Get
            Return m_DocumentTypes
        End Get
        Set(ByVal value As IEnumerable(Of Type))
            m_DocumentTypes = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置要显示的待打开的 IO 上下文列表。（如果此项被设置则忽略 <see cref="SelectedDocumentType" /> 项）
    ''' </summary>
    ''' <remarks>指定列表以及其中保存的对象指针在全程不会变化，但内容将会因用户的选择而改变。</remarks>
    Friend Property OpenArguments() As IEnumerable(Of DocumentOpenArguments)
        Get
            Return m_OpenArguments
        End Get
        Set(ByVal value As IEnumerable(Of DocumentOpenArguments))
            m_OpenArguments = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置选择的文档类型。
    ''' </summary>
    Public ReadOnly Property SelectedDocumentType() As Type
        Get
            Return If(m_SelectedIndex >= 0, m_DocumentTypes(m_SelectedIndex), Nothing)
        End Get
    End Property

    Public Property SelectedDocumentIndex() As Integer
        Get
            Return m_SelectedIndex
        End Get
        Set(ByVal value As Integer)
            m_SelectedIndex = value
        End Set
    End Property

    Private Sub FillFileItem(ByVal FileItem As ListViewItem)
        '为了提高效率，此处不做 tag 类型检查
        Dim CurIOCnt = DirectCast(FileItem.Tag, DocumentOpenArguments)
        FileItem.SubItems.Clear()
        FileItem.Text = CurIOCnt.IOContext.Name
        FileItem.SubItems.Add(If(CurIOCnt.OpenWith Is Nothing, Nothing, DocumentInformation.GetInfo(CurIOCnt.OpenWith).DocumentName), SystemColors.GrayText, Nothing, Nothing)
        FileItem.SubItems.Add(CurIOCnt.IOContext.ToString).ForeColor = SystemColors.GrayText
    End Sub

    Private Sub DocumentTypeSelector_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '显示文档类型
        DocumentTypeList.Columns.Add("文档名称", 100)
        DocumentTypeList.Columns.Add("支持的文件类型", 240)
        DocumentTypeList.Columns.Add("内部名称", 140)
        If m_DocumentTypes IsNot Nothing Then
            For I = 0 To m_DocumentTypes.Count - 1
                Dim EachInfo = DocumentInformation.GetInfo(m_DocumentTypes(I))
                DocumentTypeLargeImageList.Images.Add(If(EachInfo.Icon, DocumentInformation.DefaultIcon))
                DocumentTypeSmallImageList.Images.Add(If(EachInfo.Icon, DocumentInformation.DefaultIcon))
                Dim NewItem = DocumentTypeList.Items.Add(EachInfo.DocumentName, DocumentTypeLargeImageList.Images.Count - 1)
                NewItem.Tag = I
                NewItem.SubItems.Add(EachInfo.FileTypes.ToDescription).ForeColor = SystemColors.GrayText
                NewItem.SubItems.Add(m_DocumentTypes(I).Name).ForeColor = SystemColors.GrayText
            Next
        End If
        ViewDetails.PerformClick()

        '显示文档列表
        If OpenArguments IsNot Nothing AndAlso OpenArguments.Any Then
            '填充
            FileList.Columns.Add("名称", 100)
            FileList.Columns.Add("打开方式", 100)
            FileList.Columns.Add("上下文", 250)
            If m_OpenArguments IsNot Nothing Then
                For Each EachIOContext In m_OpenArguments
                    If EachIOContext.IOContext IsNot Nothing Then
                        Dim NewItem As New ListViewItem()
                        NewItem.Tag = EachIOContext
                        FillFileItem(NewItem)
                        FileList.Items.Add(NewItem)
                    End If
                Next
            End If

            If FileList.Items.Count > 0 Then FileList.Items(0).Selected = True
        Else
            SplitContainer1.Panel1Collapsed = True
            If DocumentTypeList.Items.Count > 0 Then
                DocumentTypeList.Items(0).Selected = True
            Else
                OKButton.Enabled = False
            End If
        End If
    End Sub

    Private Sub DocumentTypeList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DocumentTypeList.SelectedIndexChanged
        If Not IsPreset Then
            '设置选区（对于未指定 IOContexts 数组的情况）
            m_SelectedIndex = If(DocumentTypeList.SelectedItems.Count > 0, CInt(DocumentTypeList.SelectedItems(0).Tag), -1)
            If m_OpenArguments IsNot Nothing AndAlso FileList.SelectedItems.Count > 0 Then
                '设置选中文件的打开方式
                For Each EachItem As ListViewItem In FileList.SelectedItems
                    DirectCast(EachItem.Tag, DocumentOpenArguments).OpenWith = SelectedDocumentType
                    FillFileItem(EachItem)
                Next
            End If
            '（对于仅选取文档类型）仅当有选择时才能确定
            OKButton.Enabled = m_OpenArguments IsNot Nothing OrElse SelectedDocumentType IsNot Nothing
        End If
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
        DocumentTypeList.View = NewView
    End Sub

    Private Sub DocumentTypeList_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DocumentTypeList.DoubleClick
        If m_OpenArguments Is Nothing Then OKButton.PerformClick()
    End Sub

    Private Sub FileList_ItemSelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ListViewItemSelectionChangedEventArgs) Handles FileList.ItemSelectionChanged
        If e.IsSelected Then
            '正在选中项
            Dim CurDocumentType = DirectCast(e.Item.Tag, DocumentOpenArguments).OpenWith
            If DocumentTypeList.SelectedItems.Count = 0 Then
                '查找并显示选中的模板
                For Each EachItem As ListViewItem In DocumentTypeList.Items
                    If CurDocumentType Is EachItem.Tag Then
                        IsPreset = True
                        EachItem.Selected = True
                        IsPreset = False
                        Exit For
                    End If
                Next
            ElseIf CurDocumentType IsNot DocumentTypeList.SelectedItems(0).Tag Then
                IsPreset = True
                DocumentTypeList.SelectedItems.Clear()
                IsPreset = False
            End If
        ElseIf DocumentTypeList.SelectedItems.Count > 0 Then
            IsPreset = True
            DocumentTypeList.SelectedItems.Clear()
            IsPreset = False
        End If
        DeleteFileButton.Enabled = FileList.SelectedItems.Count > 0
    End Sub

    Private Sub DeleteFileButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteFileButton.Click
        '清除选定项的打开方式
        For Each EachItem As ListViewItem In FileList.SelectedItems
            Dim EachOW = CType(EachItem.Tag, DocumentOpenArguments)
            EachOW.OpenWith = Nothing
            EachItem.Tag = EachOW
            FillFileItem(EachItem)
            EachItem.Selected = False
            EachItem.Selected = True
        Next
    End Sub
End Class