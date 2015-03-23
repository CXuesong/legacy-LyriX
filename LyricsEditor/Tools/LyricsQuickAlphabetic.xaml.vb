Class LyricsQuickAlphabetic
    Private SegmentTemplates As List(Of Document.Segment)

    Private Sub DataContainerEditor_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        '载入所有语义段
        SegmentTemplates = Aggregate EachSegment In DataSource.Descendants(Of Document.Segment)()
                           Select ET = EachSegment.Text Distinct
                           Select New Document.Segment(ET)
                           Into ToList()
        STGrid.ItemsSource = New ListCollectionView(SegmentTemplates) With {.NewItemPlaceholderPosition = ComponentModel.NewItemPlaceholderPosition.None}
    End Sub

    Private Sub ApplyButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles ApplyButton.Click
        If MsgBox(Prompts.ApplyPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            Dim Segments = DataSource.Descendants(Of Document.Segment).ToList
            For Each ES In Segments
                Dim EachSegment = ES
                Dim Template = Aggregate EachItem In SegmentTemplates
                               Where EachItem.Text = EachSegment.Text
                               Into FirstOrDefault()
                If Template IsNot Nothing Then
                    '有可能在另一个视图中添加了一些段
                    If OverridesValuesCheck.IsChecked Then
                        If Template.Latinized IsNot Nothing Then ES.Latinized = Template.Latinized
                        If Template.Alphabetic IsNot Nothing Then ES.Alphabetic = Template.Alphabetic
                        If Template.Language IsNot Nothing Then ES.Language = Template.Language
                    Else
                        If Template.Latinized IsNot Nothing AndAlso ES.Latinized = Nothing Then ES.Latinized = Template.Latinized
                        If Template.Alphabetic IsNot Nothing AndAlso ES.Alphabetic = Nothing Then ES.Alphabetic = Template.Alphabetic
                        If Template.Language IsNot Nothing AndAlso ES.Language = Nothing Then ES.Language = Template.Language
                    End If
                End If
            Next
            Me.GoBack()
        End If
    End Sub
End Class
