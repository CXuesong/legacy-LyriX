Public Class GenreDialog

    Private m_Genre As String
    Private IsSelecting As Boolean

    ''' <summary>
    ''' 获取/设置输入的流派名。
    ''' </summary>
    Public Property Genre As String
        Get
            Return m_Genre
        End Get
        Set(value As String)
            m_Genre = value
        End Set
    End Property

    Private Sub GenreBox_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles GenreBox.TextChanged
        m_Genre = GenreBox.Text
        If IsSelecting Then Return '正在使用列表框选择项目
        Dim MatchExpression = m_Genre.ToUpperInvariant
        If MatchExpression = Nothing Then
            '防止列表多余的清零动作
            If GenreList.Items.Filter IsNot Nothing Then GenreList.Items.Filter = Nothing
        Else
            Dim MatchItem = Aggregate EachItem In GenreList.Items
                            Where (TypeOf EachItem Is Xml.XmlElement AndAlso
                                   DirectCast(EachItem, Xml.XmlElement).InnerText.ToUpperInvariant = MatchExpression)
                               Into FirstOrDefault()
            '筛选包含项
            GenreList.Items.Filter = Function(Item As Object)
                                         If TypeOf Item Is Xml.XmlElement Then
                                             Return DirectCast(Item, Xml.XmlElement).InnerText.ToUpperInvariant.Contains(MatchExpression)
                                         Else
                                             Return False
                                         End If
                                     End Function
            If MatchItem IsNot Nothing Then
                GenreList.SelectedItem = MatchItem
                GenreList.ScrollIntoView(MatchItem)
            End If
        End If
    End Sub

    Private Sub GenreList_MouseDoubleClick(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles GenreList.MouseDoubleClick
        Me.DialogResult = True
    End Sub

    Private Sub GenreList_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles GenreList.SelectionChanged
        If GenreList.SelectedValue IsNot Nothing Then
            IsSelecting = True
            Dim PrevSS = GenreBox.SelectionStart, PrevSL = GenreBox.SelectionLength
            GenreBox.Text = GenreList.SelectedValue.ToString
            GenreBox.SelectionStart = PrevSS
            GenreBox.SelectionLength = PrevSL
            IsSelecting = False
        End If
    End Sub

    Private Sub ClearButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles ClearButton.Click
        GenreBox.Text = Nothing
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Private Sub GenreDialog_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        IsSelecting = True
        GenreBox.Text = m_Genre
        IsSelecting = False
    End Sub
End Class
