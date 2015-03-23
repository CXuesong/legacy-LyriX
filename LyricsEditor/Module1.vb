Imports DocumentViewModel.Managers
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Friend Module Module1
    Public WithEvents DM As DocumentManager

    Private Sub DM_ViewCreated(sender As Object, e As DocumentViewModel.ViewEventArgs) Handles DM.ViewCreated
        If TypeOf e.View Is Window Then
            Dim wnd = DirectCast(e.View, Window)
            '显示窗口
            wnd.Show()
        End If
    End Sub

    <Extension()>
    Public Function FindAncestor(Of T As DependencyObject)(source As DependencyObject) As T
        If source Is Nothing Then
            Throw New ArgumentNullException("source")
        End If
        Dim CurrentObject = source
        Do Until CurrentObject Is Nothing
            CurrentObject = VisualTreeHelper.GetParent(CurrentObject)
            If TypeOf CurrentObject Is T Then
                Return DirectCast(CurrentObject, T)
            End If
        Loop
        Return Nothing
    End Function

    Public Const MSGBOX_PROMPT_LIMIT = 100
    Public Const NAVIGATION_PROMPT_LIMIT = 20
    Public Function LimitStringLength(str As String, length As Integer) As String
        If Len(str) > length Then
            Return Left(str, length) & "..."
        Else
            Return str
        End If
    End Function
End Module

''' <summary>
''' 包含了此程序可能会用到的各种命令。
''' </summary>
Public Class EditorCommands
    Private Shared Resource As New System.Resources.ResourceManager(GetType(EditorCommands))

    Public Shared Insert As RoutedUICommand = CreateCommand("Insert", New KeyGesture(Key.I, ModifierKeys.Control))
    Public Shared NewWindow As RoutedUICommand = CreateCommand("NewWindow")
    Public Shared About As RoutedUICommand = CreateCommand("About")
    Public Shared [Exit] As RoutedUICommand = CreateCommand("Exit")
    Public Shared CheckDocument As RoutedUICommand = CreateCommand("CheckDocument")

    Private Shared Function CreateCommand(name As String, Optional gesture As InputGesture = Nothing) As RoutedUICommand
        Return New RoutedUICommand(If(Resource.GetString(name), ""),
                                   name, GetType(EditorCommands),
                                 If(gesture Is Nothing, Nothing, New InputGestureCollection({gesture})))
    End Function

    Private Sub New()

    End Sub

    Shared Sub New()
        CommandManager.RegisterClassInputBinding(GetType(DataGrid),
                                                 New InputBinding(Insert, Insert.InputGestures(0)))
    End Sub
End Class

''' <summary>
''' 表示程序内部使用的剪切版。
''' </summary>
Public Class InternalClipboard
    Private Shared m_Data As Object

    ''' <summary>
    ''' 获取/设置剪切版的内容。
    ''' </summary>
    Public Shared Property Data As Object
        Get
            Return m_Data
        End Get
        Set(value As Object)
            m_Data = value
        End Set
    End Property

    Public Shared Sub Clear()
        m_Data = Nothing
    End Sub

    Private Sub New()

    End Sub
End Class