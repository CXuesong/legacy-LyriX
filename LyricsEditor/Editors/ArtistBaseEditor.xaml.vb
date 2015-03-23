Class ArtistBaseEditor
    Private IsUpdatingValues As Boolean

    Private Sub LoadValues() Handles Me.DataContextChanged, Me.Loaded
        If IsUpdatingValues OrElse Not IsLoaded Then Return
        IsUpdatingValues = True
        JobList.UnselectAll()
        Dim Jobs As ArtistJobs = DataSource.Jobs
        For Each EachJob As ArtistJobs In JobList.Items
            If (Jobs And EachJob) = EachJob Then
                JobList.SelectedItems.Add(EachJob)
            End If
        Next
        '选择合适的选项卡
        ArtistTab.Visibility = Windows.Visibility.Collapsed
        ArtistGroupTab.Visibility = Windows.Visibility.Collapsed
        If TypeOf DataSource Is Document.Artist Then
            ArtistTab.Visibility = Windows.Visibility.Visible
            ArtistTab.IsSelected = True
        ElseIf TypeOf DataSource Is Document.ArtistGroup Then
            ArtistGroupTab.Visibility = Windows.Visibility.Visible
            ArtistGroupTab.IsSelected = True
        End If
        If TypeOf DataSource Is Document.Artist Then
            Dim a = DirectCast(DataSource, Document.Artist)
            If a.Sex Is Nothing Then
                SexUnknownButton.IsChecked = True
            ElseIf a.Sex = Sex.Male Then
                SexMaleButton.IsChecked = True
            ElseIf a.Sex = Sex.Female Then
                SexFemaleButton.IsChecked = True
            End If
        ElseIf TypeOf DataSource Is Document.ArtistGroup Then
            Dim a = DirectCast(DataSource, Document.ArtistGroup)
            GroupMemberList.UnselectAll()
            For Each EachArtist As Document.ArtistBase In GroupMemberList.Items
                Debug.Assert(EachArtist.Id IsNot Nothing)
                If a.ArtistIds.Contains(EachArtist.Id.Value) Then
                    GroupMemberList.SelectedItems.Add(EachArtist)
                End If
            Next
        End If
        IsUpdatingValues = False
    End Sub

    Private Sub SaveValues() Handles SexUnknownButton.Checked, SexMaleButton.Checked, SexFemaleButton.Checked, GroupMemberList.LostFocus, JobList.LostFocus
        If IsUpdatingValues OrElse Not IsLoaded Then Return
        IsUpdatingValues = True
        Dim Jobs As ArtistJobs = ArtistJobs.None
        For Each EachJob As ArtistJobs In JobList.SelectedItems
            Jobs = Jobs Or EachJob
        Next
        DataSource.Jobs = Jobs
        If TypeOf DataSource Is Document.Artist Then
            Dim a = DirectCast(DataSource, Document.Artist)
            If SexUnknownButton.IsChecked Then
                a.Sex = Nothing
            ElseIf SexMaleButton.IsChecked Then
                a.Sex = Sex.Male
            ElseIf SexFemaleButton.IsChecked Then
                a.Sex = Sex.Female
            End If
        ElseIf TypeOf DataSource Is Document.ArtistGroup Then
            Dim a = DirectCast(DataSource, Document.ArtistGroup)
            a.ArtistIds.Clear()
            For Each EachArtist As Document.ArtistBase In GroupMemberList.SelectedItems
                Debug.Assert(EachArtist.Id IsNot Nothing)
                a.ArtistIds.Add(EachArtist.Id.Value)
            Next
        End If
        IsUpdatingValues = False
    End Sub

    Private Sub ArtistBaseEditor_ContainerDataChanged(sender As Object, e As Document.ObjectModel.ContainerDataChangedEventArgs) Handles Me.ContainerDataChanged
        If e.Source Is DataSource Then
            LoadValues()
        End If
    End Sub

    Private Sub LineEditor_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        '仅显示有 Id 且未被父级包括的艺术家
        GroupMemberList.Items.Filter = Function(EachArtist)
                                           If EachArtist Is DataSource Then Return False
                                           Return DirectCast(EachArtist, Document.ArtistBase).Id IsNot Nothing
                                       End Function
    End Sub
End Class
