Imports System.Security.Permissions
Imports System.Windows.Threading

Namespace Utility
    Public Module WPFInteraction

        ''' <summary>
        ''' 处理当前在工作项队列的所有帧。
        ''' </summary>
        <SecurityPermissionAttribute(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
        Public Sub DoEvents()
            Dim frame As New DispatcherFrame()
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, Sub(f As DispatcherFrame) f.Continue = False, frame)
            Dispatcher.PushFrame(frame)
        End Sub
    End Module
End Namespace