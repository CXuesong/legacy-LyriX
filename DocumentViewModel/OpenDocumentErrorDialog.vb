Public Class OpenDocumentErrorDialog

    Private m_Arguments As Managers.DocumentOpenArguments
    Private m_Exception As Exception
    Private m_DocumentTypes As IEnumerable(Of Type)

    Public Property Arguments() As Managers.DocumentOpenArguments
        Get
            Return m_Arguments
        End Get
        Set(ByVal value As Managers.DocumentOpenArguments)
            m_Arguments = value
        End Set
    End Property

    Public Property Exception() As Exception
        Get
            Return m_Exception
        End Get
        Set(ByVal value As Exception)
            m_Exception = value
        End Set
    End Property

    Public Property DocumentTypes() As IEnumerable(Of Type)
        Get
            Return m_DocumentTypes
        End Get
        Set(ByVal value As IEnumerable(Of Type))
            m_DocumentTypes = value
        End Set
    End Property

    Private Sub OpenDocumentDialogError_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim LinkText() As String
        LinkText = Split(String.Format(Prompts.OpenDocumentError, m_Arguments.IOContext.ToString, If(m_Arguments.OpenWith Is Nothing, "-", DocumentInformation.GetInfo(m_Arguments.OpenWith).DocumentName), If(m_Exception Is Nothing, "-", vbNullChar & m_Exception.Message & vbNullChar)), vbNullChar, 3)
        PromptLabel.Text = String.Join(Nothing, LinkText)
        If LinkText.Length = 3 Then
            PromptLabel.LinkArea = New LinkArea(Len(LinkText(0)), Len(LinkText(1)))
            ExceptionDetail.Text = m_Exception.ToString
        End If
        OpenWithButton.Enabled = m_DocumentTypes IsNot Nothing AndAlso m_DocumentTypes.Any
        PromptIcon.Image = SystemIcons.Exclamation.ToBitmap
    End Sub

    Private Sub OpenWithButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenWithButton.Click
        Using Selector As New DocumentTypeSelector
            Selector.DocumentTypes = m_DocumentTypes
            Selector.OpenArguments = New Managers.DocumentOpenArguments() {m_Arguments}
            If Selector.ShowDialog = Forms.DialogResult.OK Then
                Me.DialogResult = Forms.DialogResult.Retry
            End If
        End Using
    End Sub

    Private Sub PromptLabel_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles PromptLabel.LinkClicked
        ExceptionDetail.Visible = True
    End Sub
End Class