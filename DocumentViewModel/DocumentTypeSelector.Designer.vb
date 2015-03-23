<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DocumentTypeSelector
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DocumentTypeSelector))
        Me.DocumentTypeList = New System.Windows.Forms.ListView
        Me.DocumentTypeLargeImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.DocumentTypeSmallImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.OKButton = New System.Windows.Forms.Button
        Me.CancelBtn = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.ToolStrip2 = New System.Windows.Forms.ToolStrip
        Me.ViewModeSelector = New System.Windows.Forms.ToolStripDropDownButton
        Me.ViewLargeIcon = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewSmallIcon = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewList = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewDetails = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewTile = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewThumb = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.DeleteFileButton = New System.Windows.Forms.ToolStripButton
        Me.Label2 = New System.Windows.Forms.Label
        Me.FileList = New System.Windows.Forms.ListView
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStrip2.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DocumentTypeList
        '
        Me.DocumentTypeList.AllowColumnReorder = True
        Me.DocumentTypeList.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.DocumentTypeList.FullRowSelect = True
        Me.DocumentTypeList.HideSelection = False
        Me.DocumentTypeList.LargeImageList = Me.DocumentTypeLargeImageList
        Me.DocumentTypeList.Location = New System.Drawing.Point(0, 31)
        Me.DocumentTypeList.MultiSelect = False
        Me.DocumentTypeList.Name = "DocumentTypeList"
        Me.DocumentTypeList.Size = New System.Drawing.Size(370, 284)
        Me.DocumentTypeList.SmallImageList = Me.DocumentTypeSmallImageList
        Me.DocumentTypeList.TabIndex = 2
        Me.DocumentTypeList.UseCompatibleStateImageBehavior = False
        Me.DocumentTypeList.View = System.Windows.Forms.View.Details
        '
        'DocumentTypeLargeImageList
        '
        Me.DocumentTypeLargeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.DocumentTypeLargeImageList.ImageSize = New System.Drawing.Size(32, 32)
        Me.DocumentTypeLargeImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'DocumentTypeSmallImageList
        '
        Me.DocumentTypeSmallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.DocumentTypeSmallImageList.ImageSize = New System.Drawing.Size(16, 16)
        Me.DocumentTypeSmallImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'OKButton
        '
        Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.OKButton.Location = New System.Drawing.Point(426, 333)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(75, 23)
        Me.OKButton.TabIndex = 1
        Me.OKButton.Text = "确定"
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'CancelBtn
        '
        Me.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelBtn.Location = New System.Drawing.Point(507, 333)
        Me.CancelBtn.Name = "CancelBtn"
        Me.CancelBtn.Size = New System.Drawing.Size(75, 23)
        Me.CancelBtn.TabIndex = 2
        Me.CancelBtn.Text = "取消"
        Me.CancelBtn.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(143, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "请选择一个文档类型(&T)："
        '
        'ToolStrip2
        '
        Me.ToolStrip2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ToolStrip2.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewModeSelector})
        Me.ToolStrip2.Location = New System.Drawing.Point(286, 3)
        Me.ToolStrip2.Name = "ToolStrip2"
        Me.ToolStrip2.Size = New System.Drawing.Size(62, 25)
        Me.ToolStrip2.TabIndex = 0
        '
        'ViewModeSelector
        '
        Me.ViewModeSelector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ViewModeSelector.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewLargeIcon, Me.ViewSmallIcon, Me.ViewList, Me.ViewDetails, Me.ViewTile, Me.ViewThumb})
        Me.ViewModeSelector.Image = CType(resources.GetObject("ViewModeSelector.Image"), System.Drawing.Image)
        Me.ViewModeSelector.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewModeSelector.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewModeSelector.Name = "ViewModeSelector"
        Me.ViewModeSelector.Size = New System.Drawing.Size(28, 22)
        Me.ViewModeSelector.Text = "查看"
        '
        'ViewLargeIcon
        '
        Me.ViewLargeIcon.Image = CType(resources.GetObject("ViewLargeIcon.Image"), System.Drawing.Image)
        Me.ViewLargeIcon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewLargeIcon.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewLargeIcon.Name = "ViewLargeIcon"
        Me.ViewLargeIcon.Size = New System.Drawing.Size(152, 22)
        Me.ViewLargeIcon.Text = "大图标(&L)"
        '
        'ViewSmallIcon
        '
        Me.ViewSmallIcon.Image = CType(resources.GetObject("ViewSmallIcon.Image"), System.Drawing.Image)
        Me.ViewSmallIcon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewSmallIcon.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewSmallIcon.Name = "ViewSmallIcon"
        Me.ViewSmallIcon.Size = New System.Drawing.Size(152, 22)
        Me.ViewSmallIcon.Text = "小图标(&S)"
        '
        'ViewList
        '
        Me.ViewList.Image = CType(resources.GetObject("ViewList.Image"), System.Drawing.Image)
        Me.ViewList.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewList.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewList.Name = "ViewList"
        Me.ViewList.Size = New System.Drawing.Size(152, 22)
        Me.ViewList.Text = "列表(&I)"
        '
        'ViewDetails
        '
        Me.ViewDetails.Image = CType(resources.GetObject("ViewDetails.Image"), System.Drawing.Image)
        Me.ViewDetails.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewDetails.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewDetails.Name = "ViewDetails"
        Me.ViewDetails.Size = New System.Drawing.Size(152, 22)
        Me.ViewDetails.Text = "详细信息(&D)"
        '
        'ViewTile
        '
        Me.ViewTile.Image = CType(resources.GetObject("ViewTile.Image"), System.Drawing.Image)
        Me.ViewTile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewTile.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewTile.Name = "ViewTile"
        Me.ViewTile.Size = New System.Drawing.Size(152, 22)
        Me.ViewTile.Text = "平铺(&T)"
        '
        'ViewThumb
        '
        Me.ViewThumb.Enabled = False
        Me.ViewThumb.Image = CType(resources.GetObject("ViewThumb.Image"), System.Drawing.Image)
        Me.ViewThumb.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewThumb.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewThumb.Name = "ViewThumb"
        Me.ViewThumb.Size = New System.Drawing.Size(152, 22)
        Me.ViewThumb.Text = "缩略图(&H)"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Location = New System.Drawing.Point(12, 12)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ToolStrip1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.FileList)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ToolStrip2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.DocumentTypeList)
        Me.SplitContainer1.Size = New System.Drawing.Size(570, 315)
        Me.SplitContainer1.SplitterDistance = 196
        Me.SplitContainer1.TabIndex = 0
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteFileButton})
        Me.ToolStrip1.Location = New System.Drawing.Point(156, 3)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(26, 25)
        Me.ToolStrip1.TabIndex = 0
        '
        'DeleteFileButton
        '
        Me.DeleteFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.DeleteFileButton.Image = CType(resources.GetObject("DeleteFileButton.Image"), System.Drawing.Image)
        Me.DeleteFileButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.DeleteFileButton.Name = "DeleteFileButton"
        Me.DeleteFileButton.Size = New System.Drawing.Size(23, 22)
        Me.DeleteFileButton.Text = "删除"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 10)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "文件列表(&F)："
        '
        'FileList
        '
        Me.FileList.AllowColumnReorder = True
        Me.FileList.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.FileList.FullRowSelect = True
        Me.FileList.HideSelection = False
        Me.FileList.Location = New System.Drawing.Point(0, 31)
        Me.FileList.Name = "FileList"
        Me.FileList.Size = New System.Drawing.Size(196, 284)
        Me.FileList.TabIndex = 2
        Me.FileList.UseCompatibleStateImageBehavior = False
        Me.FileList.View = System.Windows.Forms.View.Details
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Image = CType(resources.GetObject("ToolStripMenuItem1.Image"), System.Drawing.Image)
        Me.ToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Red
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(136, 22)
        Me.ToolStripMenuItem1.Text = "大图标(&L)"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Image = CType(resources.GetObject("ToolStripMenuItem2.Image"), System.Drawing.Image)
        Me.ToolStripMenuItem2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Red
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(136, 22)
        Me.ToolStripMenuItem2.Text = "小图标(&S)"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Image = CType(resources.GetObject("ToolStripMenuItem3.Image"), System.Drawing.Image)
        Me.ToolStripMenuItem3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItem3.ImageTransparentColor = System.Drawing.Color.Red
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(136, 22)
        Me.ToolStripMenuItem3.Text = "列表(&I)"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Image = CType(resources.GetObject("ToolStripMenuItem4.Image"), System.Drawing.Image)
        Me.ToolStripMenuItem4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItem4.ImageTransparentColor = System.Drawing.Color.Red
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(136, 22)
        Me.ToolStripMenuItem4.Text = "详细信息(&D)"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Image = CType(resources.GetObject("ToolStripMenuItem5.Image"), System.Drawing.Image)
        Me.ToolStripMenuItem5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItem5.ImageTransparentColor = System.Drawing.Color.Red
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(136, 22)
        Me.ToolStripMenuItem5.Text = "平铺(&T)"
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Enabled = False
        Me.ToolStripMenuItem6.Image = CType(resources.GetObject("ToolStripMenuItem6.Image"), System.Drawing.Image)
        Me.ToolStripMenuItem6.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItem6.ImageTransparentColor = System.Drawing.Color.Red
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(136, 22)
        Me.ToolStripMenuItem6.Text = "缩略图(&H)"
        '
        'DocumentTypeSelector
        '
        Me.AcceptButton = Me.OKButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CancelBtn
        Me.ClientSize = New System.Drawing.Size(594, 368)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.CancelBtn)
        Me.Controls.Add(Me.OKButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DocumentTypeSelector"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "选择文档类型"
        Me.ToolStrip2.ResumeLayout(False)
        Me.ToolStrip2.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents DocumentTypeList As System.Windows.Forms.ListView
    Private WithEvents OKButton As System.Windows.Forms.Button
    Private WithEvents CancelBtn As System.Windows.Forms.Button
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents ToolStrip2 As System.Windows.Forms.ToolStrip
    Private WithEvents ViewModeSelector As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents ViewLargeIcon As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewSmallIcon As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewList As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewDetails As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewTile As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewThumb As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents DocumentTypeLargeImageList As System.Windows.Forms.ImageList
    Private WithEvents DocumentTypeSmallImageList As System.Windows.Forms.ImageList
    Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents FileList As System.Windows.Forms.ListView
    Private WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripMenuItem5 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripMenuItem6 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents DeleteFileButton As System.Windows.Forms.ToolStripButton
End Class
