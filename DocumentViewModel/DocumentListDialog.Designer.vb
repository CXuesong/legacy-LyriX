<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DocumentListDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DocumentListDialog))
        Me.DocumentList = New System.Windows.Forms.ListView
        Me.DocumentLargeImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.DocumentSmallImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.SaveButton = New System.Windows.Forms.Button
        Me.CloseDocButton = New System.Windows.Forms.Button
        Me.CloseButton = New System.Windows.Forms.Button
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.ViewModeSelector = New System.Windows.Forms.ToolStripDropDownButton
        Me.ViewLargeIcon = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewSmallIcon = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewList = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewDetails = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewTile = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewThumb = New System.Windows.Forms.ToolStripMenuItem
        Me.PromptLabel = New System.Windows.Forms.Label
        Me.ShowAllDocuments = New System.Windows.Forms.CheckBox
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DocumentList
        '
        Me.DocumentList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DocumentList.FullRowSelect = True
        Me.DocumentList.HideSelection = False
        Me.DocumentList.LargeImageList = Me.DocumentLargeImageList
        Me.DocumentList.Location = New System.Drawing.Point(12, 37)
        Me.DocumentList.Name = "DocumentList"
        Me.DocumentList.Size = New System.Drawing.Size(471, 287)
        Me.DocumentList.SmallImageList = Me.DocumentSmallImageList
        Me.DocumentList.TabIndex = 2
        Me.DocumentList.UseCompatibleStateImageBehavior = False
        Me.DocumentList.View = System.Windows.Forms.View.Details
        '
        'DocumentLargeImageList
        '
        Me.DocumentLargeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit
        Me.DocumentLargeImageList.ImageSize = New System.Drawing.Size(32, 32)
        Me.DocumentLargeImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'DocumentSmallImageList
        '
        Me.DocumentSmallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit
        Me.DocumentSmallImageList.ImageSize = New System.Drawing.Size(16, 16)
        Me.DocumentSmallImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'SaveButton
        '
        Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SaveButton.Location = New System.Drawing.Point(171, 330)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(100, 24)
        Me.SaveButton.TabIndex = 4
        Me.SaveButton.Text = "保存文档(&S)"
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'CloseDocButton
        '
        Me.CloseDocButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CloseDocButton.Location = New System.Drawing.Point(277, 330)
        Me.CloseDocButton.Name = "CloseDocButton"
        Me.CloseDocButton.Size = New System.Drawing.Size(100, 24)
        Me.CloseDocButton.TabIndex = 5
        Me.CloseDocButton.Text = "关闭文档(&C)"
        Me.CloseDocButton.UseVisualStyleBackColor = True
        '
        'CloseButton
        '
        Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseButton.Location = New System.Drawing.Point(383, 330)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(100, 24)
        Me.CloseButton.TabIndex = 6
        Me.CloseButton.Text = "关闭(&L)"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewModeSelector})
        Me.ToolStrip1.Location = New System.Drawing.Point(452, 9)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(31, 25)
        Me.ToolStrip1.TabIndex = 0
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
        Me.ViewLargeIcon.Size = New System.Drawing.Size(136, 22)
        Me.ViewLargeIcon.Text = "大图标(&L)"
        '
        'ViewSmallIcon
        '
        Me.ViewSmallIcon.Image = CType(resources.GetObject("ViewSmallIcon.Image"), System.Drawing.Image)
        Me.ViewSmallIcon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewSmallIcon.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewSmallIcon.Name = "ViewSmallIcon"
        Me.ViewSmallIcon.Size = New System.Drawing.Size(136, 22)
        Me.ViewSmallIcon.Text = "小图标(&S)"
        '
        'ViewList
        '
        Me.ViewList.Image = CType(resources.GetObject("ViewList.Image"), System.Drawing.Image)
        Me.ViewList.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewList.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewList.Name = "ViewList"
        Me.ViewList.Size = New System.Drawing.Size(136, 22)
        Me.ViewList.Text = "列表(&I)"
        '
        'ViewDetails
        '
        Me.ViewDetails.Image = CType(resources.GetObject("ViewDetails.Image"), System.Drawing.Image)
        Me.ViewDetails.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewDetails.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewDetails.Name = "ViewDetails"
        Me.ViewDetails.Size = New System.Drawing.Size(136, 22)
        Me.ViewDetails.Text = "详细信息(&D)"
        '
        'ViewTile
        '
        Me.ViewTile.Image = CType(resources.GetObject("ViewTile.Image"), System.Drawing.Image)
        Me.ViewTile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewTile.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewTile.Name = "ViewTile"
        Me.ViewTile.Size = New System.Drawing.Size(136, 22)
        Me.ViewTile.Text = "平铺(&T)"
        '
        'ViewThumb
        '
        Me.ViewThumb.Enabled = False
        Me.ViewThumb.Image = CType(resources.GetObject("ViewThumb.Image"), System.Drawing.Image)
        Me.ViewThumb.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ViewThumb.ImageTransparentColor = System.Drawing.Color.Red
        Me.ViewThumb.Name = "ViewThumb"
        Me.ViewThumb.Size = New System.Drawing.Size(136, 22)
        Me.ViewThumb.Text = "缩略图(&H)"
        '
        'PromptLabel
        '
        Me.PromptLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.PromptLabel.Location = New System.Drawing.Point(12, 9)
        Me.PromptLabel.Name = "PromptLabel"
        Me.PromptLabel.Size = New System.Drawing.Size(437, 25)
        Me.PromptLabel.TabIndex = 1
        Me.PromptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ShowAllDocuments
        '
        Me.ShowAllDocuments.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ShowAllDocuments.AutoSize = True
        Me.ShowAllDocuments.Location = New System.Drawing.Point(12, 337)
        Me.ShowAllDocuments.Name = "ShowAllDocuments"
        Me.ShowAllDocuments.Size = New System.Drawing.Size(114, 16)
        Me.ShowAllDocuments.TabIndex = 3
        Me.ShowAllDocuments.Text = "显示所有文档(&A)"
        Me.ShowAllDocuments.UseVisualStyleBackColor = True
        '
        'DocumentListDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
        Me.ClientSize = New System.Drawing.Size(492, 366)
        Me.Controls.Add(Me.ShowAllDocuments)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.PromptLabel)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.DocumentList)
        Me.Controls.Add(Me.SaveButton)
        Me.Controls.Add(Me.CloseDocButton)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(200, 150)
        Me.Name = "DocumentListDialog"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "文档"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DocumentList As System.Windows.Forms.ListView
    Friend WithEvents SaveButton As System.Windows.Forms.Button
    Friend WithEvents CloseDocButton As System.Windows.Forms.Button
    Friend WithEvents CloseButton As System.Windows.Forms.Button
    Friend WithEvents DocumentSmallImageList As System.Windows.Forms.ImageList
    Private WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Private WithEvents ViewModeSelector As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents ViewLargeIcon As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewSmallIcon As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewList As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewDetails As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewTile As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ViewThumb As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PromptLabel As System.Windows.Forms.Label
    Friend WithEvents DocumentLargeImageList As System.Windows.Forms.ImageList
    Friend WithEvents ShowAllDocuments As System.Windows.Forms.CheckBox
End Class
