Imports DocumentViewModel
Class LyriXPackageView

    Private Sub WindowCommands_CanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub WindowCommands_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        If e.Command Is EditorCommands.Exit Then
            If DM.Documents.SaveModified() Then
                DM.Documents.CloseAll()
            End If
        ElseIf e.Command Is EditorCommands.About Then
            Dim wnd As New AboutBox
            wnd.Owner = Me
            wnd.ShowDialog()
        End If
    End Sub

    Public Sub EditContainer(container As Document.ObjectModel.DataContainer, Optional ByVal editorType As Type = Nothing)
        Debug.Assert(container IsNot Nothing)
        If TypeOf EditorFrame.Content Is DataContainerEditorBase AndAlso
            (editorType Is Nothing OrElse editorType.IsInstanceOfType(EditorFrame.Content)) AndAlso
            DirectCast(EditorFrame.Content, DataContainerEditorBase).DataSource Is container Then
            '无需更改编辑器
        Else
            If editorType Is Nothing Then
                If TypeOf container Is Document.LyriXPackage Then
                    editorType = GetType(LyriXPackageEditor)
                ElseIf TypeOf container Is Document.Header Then
                    editorType = GetType(HeaderEditor)
                ElseIf TypeOf container Is Document.MusicInfo Then
                    editorType = GetType(MusicInfoEditor)
                ElseIf TypeOf container Is Document.ArtistBase Then
                    editorType = GetType(ArtistBaseEditor)
                ElseIf TypeOf container Is Document.Lyrics Then
                    editorType = GetType(LyricsEditor)
                ElseIf TypeOf container Is Document.Version Then
                    editorType = GetType(VersionEditor)
                ElseIf TypeOf container Is Document.Track Then
                    editorType = GetType(TrackEditor)
                ElseIf TypeOf container Is Document.Line Then
                    editorType = GetType(LineEditor)
                ElseIf TypeOf container Is Document.LocalizedPackagePartsCollection Then
                    editorType = GetType(LocalizedPackagePartsCollectionEditor)
                ElseIf TypeOf container Is Document.LocalizedPackageParts Then
                    editorType = GetType(LocalizedPackagePartsEditor)
                ElseIf TypeOf container Is Document.LocalizedMusicInfo Then
                    editorType = GetType(LocalizedMusicInfoEditor)
                ElseIf TypeOf container Is Document.LocalizedLyrics Then
                    editorType = GetType(LocalizedLyricsEditor)
                Else
                    Return
                End If
            End If
            Dim NewPage = DirectCast(Activator.CreateInstance(editorType), DataContainerEditorBase)
            EditContainer(container, NewPage)
        End If
    End Sub

    Public Sub EditContainer(container As Document.ObjectModel.DataContainer, editor As DataContainerEditorBase)
        Debug.Assert(container IsNot Nothing)
        Debug.Assert(editor IsNot Nothing)
        '为页面设置数据源
        editor.DataContext = New EditorDataContext(CurrentDocument, DirectCast(container, Document.ObjectModel.DataContainer))
        EditorFrame.Navigate(editor)
    End Sub

    Private Sub PackageOutline_SelectedItemChanged(sender As System.Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object)) Handles PackageOutline.SelectedItemChanged
        If e.NewValue IsNot Nothing AndAlso e.OldValue IsNot e.NewValue Then
            'Debug.Print("{0} -> {1}", e.OldValue, e.NewValue)
            Dim SelContainer = CType(DirectCast(PackageOutline.SelectedItem, TreeViewItem).Header, Document.ObjectModel.DataContainer)
            EditContainer(SelContainer)
        End If
    End Sub

    Private Sub EditorFrame_Navigated(sender As System.Object, e As System.Windows.Navigation.NavigationEventArgs) Handles EditorFrame.Navigated
        '同步列表
        If TypeOf e.Content Is DataContainerEditorBase Then
            Dim Container = DirectCast(e.Content, DataContainerEditorBase).DataSource
            If Container IsNot Nothing Then
                Dim OutlineItem As TreeViewItem = GetContainerOutline(Container)
                If OutlineItem IsNot Nothing Then
                    OutlineItem.IsSelected = True
                    '展开树
                    Dim CurrentItem As FrameworkElement = OutlineItem
                    Do
                        CurrentItem = DirectCast(CurrentItem.Parent, FrameworkElement)
                        If TypeOf CurrentItem Is TreeViewItem Then
                            DirectCast(CurrentItem, TreeViewItem).IsExpanded = True
                        End If
                    Loop Until CurrentItem Is Nothing OrElse TypeOf CurrentItem Is TreeView
                End If
            End If
        End If
    End Sub

    Private Sub EditorFrame_Navigating(sender As Object, e As System.Windows.Navigation.NavigatingCancelEventArgs) Handles EditorFrame.Navigating
        CommitEditorChanges()
    End Sub
End Class