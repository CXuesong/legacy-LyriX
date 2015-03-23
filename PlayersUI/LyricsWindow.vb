
''' <summary>
''' 封装了一个
''' </summary>
Public Class LyricsWindow
    Implements IDisposable

    'FUTURE 实现此功能
    ''' <summary>
    ''' 在歌词面板要求跳转至某一位置时引发。
    ''' </summary>
    ''' <remarks>此功能尚未实现</remarks>
    Public Event SeekPosition(sender As Object, e As Players.ObjectModel.PositionChangedEventArgs)

    ''' <summary>
    ''' 在 <see cref="Visible" /> 发生变化时引发。
    ''' </summary>
    Public Event VisibleChanged(sender As Object, e As EventArgs)

    '此处不需要包含界面元素的详细设置，把它交给 PlayersUI 内置的“选项”对话框
    Private m_Player As SingleTrackLyricsPlayer
    Private m_Document As Compiled.LyricsDocument
    Private WithEvents m_Window As Controls.LyricsPanelWindow
    Private m_IsDisposing As Boolean
    Private m_IsDisposed As Boolean

    ''' <summary>
    ''' 获取当前使用的播放器。
    ''' </summary>
    Public ReadOnly Property Player As Players.ObjectModel.LyricsPlayer
        Get
            Return m_Player
        End Get
    End Property

    ''' <summary>
    ''' 获取当前使用的歌词面板。
    ''' </summary>
    Public ReadOnly Property Window As Window
        Get
            Return m_Window
        End Get
    End Property

    ''' <summary>
    ''' 获取当前打开的歌词文挡。
    ''' </summary>
    Public ReadOnly Property Document As Compiled.LyricsDocument
        Get
            Return m_Document
        End Get
    End Property

    ''' <summary>
    ''' 打开一个现存的歌词文件，并重置当前播放器。
    ''' </summary>
    Public Sub OpenDocument(fileName As String)
        Using fs As New IO.FileStream(fileName, IO.FileMode.Open, IO.FileAccess.Read)
            OpenDocument(fs)
        End Using
    End Sub

    Public Sub OpenDocument(stream As IO.Stream)
        Dim Package As New Document.LyriXPackage(stream)
        OpenDocument(Package)
    End Sub

    Public Sub OpenDocument(package As Document.LyriXPackage)
        CheckDisposed()
        Dim Compiler As New Compilers.LyriXCompiler
        m_Document = Compiler.Compile(package)
        For Each EachOutput In Compiler.Output
            If EachOutput.IsWarning Then
                Trace.TraceWarning(EachOutput.ToString)
            ElseIf EachOutput.IsInformation Then
                Trace.TraceInformation(EachOutput.ToString)
            Else
                Trace.WriteLine(EachOutput.ToString)
            End If
        Next
        m_Player.Document = m_Document
    End Sub

    ''' <summary>
    ''' 关闭当前打开的歌词文件。
    ''' </summary>
    ''' <remarks>此方法适用于清除歌词播放器的内容。如果要另行打开新的歌词文件，请直接使用 <see cref="OpenDocument" />。</remarks>
    Public Sub CloseDocument()
        CheckDisposed()
        m_Player.Document = Nothing
        m_Player.Version = Nothing
        m_Player.Position = Nothing
    End Sub

    ''' <summary>
    ''' 设置音乐的当前播放位置。
    ''' </summary>
    Public Sub SetMusicPosition(position As TimeSpan)
        CheckDisposed()
        m_Player.Position = position
    End Sub

    Public Sub Show()
        CheckDisposed()
        m_Window.Show()
    End Sub

    Public Sub Hide()
        CheckDisposed()
        m_Window.Hide()
    End Sub

    ''' <summary>
    ''' 激活歌词面板窗口。
    ''' </summary>
    Public Sub Activate()
        CheckDisposed()
        m_Window.Activate()
    End Sub

    ''' <summary>
    ''' 获取/设置歌词面板窗口的可见性。
    ''' </summary>
    Public Property Visible As Boolean
        Get
            CheckDisposed()
            Return m_Window.Visibility = Visibility.Visible
        End Get
        Set(value As Boolean)
            CheckDisposed()
            m_Window.Visibility = If(value, Visibility.Visible, Visibility.Hidden)
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置歌词面板窗口的标题。
    ''' </summary>
    Public Property Title As String
        Get
            CheckDisposed()
            Return m_Window.Title
        End Get
        Set(value As String)
            CheckDisposed()
            m_Window.Title = value
        End Set
    End Property

    Public ReadOnly Property IsDisposed As Boolean
        Get
            Return m_IsDisposed
        End Get
    End Property

    ''' <summary>
    ''' 引发 <see cref="SeekPosition" />。
    ''' </summary>
    Protected Overridable Sub OnSeekPosition(e As Players.ObjectModel.PositionChangedEventArgs)
        RaiseEvent SeekPosition(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="VisibleChanged" />。
    ''' </summary>
    Protected Overridable Sub OnVisibleChanged()
        RaiseEvent VisibleChanged(Me, EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' 检查当前的 <see cref="IsDisposed" />，如果为 <c>true</c>，则引发异常。
    ''' </summary>
    Protected Sub CheckDisposed()
        If m_IsDisposed Then
            Throw New ObjectDisposedException(GetType(LyricsWindow).Name)
        End If
    End Sub

    Public Sub New()
        m_Player = New SingleTrackLyricsPlayer
        m_Window = New Controls.LyricsPanelWindow With {.ShowActivated = False}
        m_Window.lyricsPanel.Player = m_Player
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not m_IsDisposed Then
            m_IsDisposing = True
            If disposing Then
                '释放托管状态(托管对象)。
                m_Window.Close()
                m_Window = Nothing
                m_Document = Nothing
                m_Player = Nothing
            End If
            '释放非托管资源(非托管对象)并重写下面的 Finalize()。
            '将大型字段设置为 null。
            m_IsDisposed = True
        End If
    End Sub

    '仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    Private Sub m_Window_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles m_Window.Closing
        If Not m_IsDisposing Then
            '隐藏窗口，而非关闭
            e.Cancel = True
            m_Window.Hide()
        End If
    End Sub

    Private Sub m_Window_IsVisibleChanged(sender As Object, e As System.Windows.DependencyPropertyChangedEventArgs) Handles m_Window.IsVisibleChanged
        OnVisibleChanged()
    End Sub
End Class
