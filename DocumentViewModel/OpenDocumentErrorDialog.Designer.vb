<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OpenDocumentErrorDialog
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
        Me.RetryButton = New System.Windows.Forms.Button()
        Me.IgnoreButton = New System.Windows.Forms.Button()
        Me.CancelAllButton = New System.Windows.Forms.Button()
        Me.OpenWithButton = New System.Windows.Forms.Button()
        Me.PromptLabel = New System.Windows.Forms.LinkLabel()
        Me.ExceptionDetail = New System.Windows.Forms.TextBox()
        Me.PromptIcon = New System.Windows.Forms.PictureBox()
        CType(Me.PromptIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RetryButton
        '
        Me.RetryButton.DialogResult = System.Windows.Forms.DialogResult.Retry
        Me.RetryButton.Location = New System.Drawing.Point(12, 112)
        Me.RetryButton.Name = "RetryButton"
        Me.RetryButton.Size = New System.Drawing.Size(100, 25)
        Me.RetryButton.TabIndex = 1
        Me.RetryButton.Text = "重试(&R)"
        Me.RetryButton.UseVisualStyleBackColor = True
        '
        'IgnoreButton
        '
        Me.IgnoreButton.DialogResult = System.Windows.Forms.DialogResult.Ignore
        Me.IgnoreButton.Location = New System.Drawing.Point(118, 112)
        Me.IgnoreButton.Name = "IgnoreButton"
        Me.IgnoreButton.Size = New System.Drawing.Size(100, 25)
        Me.IgnoreButton.TabIndex = 2
        Me.IgnoreButton.Text = "忽略(&I)"
        Me.IgnoreButton.UseVisualStyleBackColor = True
        '
        'CancelAllButton
        '
        Me.CancelAllButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelAllButton.Location = New System.Drawing.Point(224, 112)
        Me.CancelAllButton.Name = "CancelAllButton"
        Me.CancelAllButton.Size = New System.Drawing.Size(100, 25)
        Me.CancelAllButton.TabIndex = 3
        Me.CancelAllButton.Text = "全部取消(&C)"
        Me.CancelAllButton.UseVisualStyleBackColor = True
        '
        'OpenWithButton
        '
        Me.OpenWithButton.Location = New System.Drawing.Point(330, 112)
        Me.OpenWithButton.Name = "OpenWithButton"
        Me.OpenWithButton.Size = New System.Drawing.Size(100, 25)
        Me.OpenWithButton.TabIndex = 4
        Me.OpenWithButton.Text = "打开方式(&W)..."
        Me.OpenWithButton.UseVisualStyleBackColor = True
        '
        'PromptLabel
        '
        Me.PromptLabel.Location = New System.Drawing.Point(51, 10)
        Me.PromptLabel.Name = "PromptLabel"
        Me.PromptLabel.Size = New System.Drawing.Size(379, 100)
        Me.PromptLabel.TabIndex = 0
        '
        'ExceptionDetail
        '
        Me.ExceptionDetail.HideSelection = False
        Me.ExceptionDetail.Location = New System.Drawing.Point(12, 143)
        Me.ExceptionDetail.Multiline = True
        Me.ExceptionDetail.Name = "ExceptionDetail"
        Me.ExceptionDetail.ReadOnly = True
        Me.ExceptionDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.ExceptionDetail.Size = New System.Drawing.Size(420, 120)
        Me.ExceptionDetail.TabIndex = 6
        Me.ExceptionDetail.Visible = False
        '
        'PromptIcon
        '
        Me.PromptIcon.Location = New System.Drawing.Point(13, 13)
        Me.PromptIcon.Name = "PromptIcon"
        Me.PromptIcon.Size = New System.Drawing.Size(32, 32)
        Me.PromptIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PromptIcon.TabIndex = 7
        Me.PromptIcon.TabStop = False
        '
        'OpenDocumentErrorDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.IgnoreButton
        Me.ClientSize = New System.Drawing.Size(444, 268)
        Me.Controls.Add(Me.PromptIcon)
        Me.Controls.Add(Me.ExceptionDetail)
        Me.Controls.Add(Me.OpenWithButton)
        Me.Controls.Add(Me.CancelAllButton)
        Me.Controls.Add(Me.IgnoreButton)
        Me.Controls.Add(Me.RetryButton)
        Me.Controls.Add(Me.PromptLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "OpenDocumentErrorDialog"
        Me.Padding = New System.Windows.Forms.Padding(10)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "打开文档"
        CType(Me.PromptIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RetryButton As System.Windows.Forms.Button
    Friend WithEvents IgnoreButton As System.Windows.Forms.Button
    Friend WithEvents CancelAllButton As System.Windows.Forms.Button
    Friend WithEvents OpenWithButton As System.Windows.Forms.Button
    Friend WithEvents PromptLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents ExceptionDetail As System.Windows.Forms.TextBox
    Friend WithEvents PromptIcon As System.Windows.Forms.PictureBox
End Class
