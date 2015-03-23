''' <summary>
''' 用于指定视图更新被发起的原因。
''' </summary>
Public Enum UpdateReason
    ''' <summary>
    ''' 原因未知。
    ''' </summary>
    Unknown = 0
    ''' <summary>
    ''' 当文档通过新建、打开等操作被创建，或是此视图在被绑定到某个文档上，并被要求初始化更新。
    ''' </summary>
    Initializing
    ''' <summary>
    ''' 由某个视图发出的要求更新的命令。
    ''' </summary>
    View
End Enum

''' <summary>
''' 描述更新的发起原因、发起者，以及由调用方定义的附加上下文。
''' </summary>
''' <remarks></remarks>
Public Structure UpdateContext
    Private m_Sender As IDocumentView
    Private m_Context As Object
    Private m_Reason As UpdateReason

    ''' <summary>
    ''' 获取要求进行更新的发起者。
    ''' </summary>
    ''' <returns>如果存在发起者（视图），则返回之；否则返回 <c>null</c>。</returns>
    Public ReadOnly Property Sender() As IDocumentView
        Get
            Return m_Sender
        End Get
    End Property

    ''' <summary>
    ''' 由调用方定义的附加上下文（相当于 lHint）。
    ''' </summary>
    ''' <returns>如果存在附加上下文，则返回之；否则返回 <c>null</c>。</returns>
    Public ReadOnly Property Context() As Object
        Get
            Return m_Context
        End Get
    End Property

    ''' <summary>
    ''' 获取此次更新被发起的原因。
    ''' </summary>
    Public ReadOnly Property Reason() As UpdateReason
        Get
            Return m_Reason
        End Get
    End Property

    ''' <summary>
    ''' 使用给定的信息初始化。
    ''' </summary>
    ''' <param name="reason">此次更新被发起的原因。</param>
    ''' <param name="sender">要求进行更新的发起者。如果 <paramref name="reason" /> 为 <see cref="UpdateReason.View" />，则此项不能为 <c>null</c>。</param>
    ''' <param name="context">由调用方定义的附加上下文（相当于 lHint）。</param>
    ''' <exception cref="ArgumentNullException">当 reason 为 UpdateReason.View 时，sender 为 <c>null</c>。</exception>
    Public Sub New(ByVal reason As UpdateReason, Optional ByVal sender As IDocumentView = Nothing, Optional ByVal context As Object = Nothing)
        If reason = UpdateReason.View AndAlso sender Is Nothing Then
            Throw New ArgumentNullException("sender", "当 reason 为 UpdateReason.View 时，要求存在 sender。")
        Else
            m_Reason = reason
            m_Sender = sender
            m_Context = context
        End If
    End Sub
End Structure

'TOREALIZE 使用 CodeAccessPermission 增强 View 安全性
''' <summary>
''' 表示用于显示一个文档的视图。
''' </summary>
Public Interface IDocumentView
    Inherits IDisposable
    ''' <summary>
    ''' （<see cref="Document" /> 专用）将视图绑定到一个文档上。
    ''' </summary>
    ''' <param name="NewDocument">要绑定到的文档。</param>
    ''' <exception cref="ArgumentOutOfRangeException">不支持指定的文档。</exception>
    ''' <remarks>
    ''' 注意：请不要直接调用此方法的实现，而应使用 <see cref="Document.AddView" />。
    ''' 且 <paramref name="NewDocument" /> 不为 <c>null</c> 或是此视图不支持的文档类型
    ''' 待实现：
    ''' 声明一个模块级私有变量，用来保存文档的指针。
    ''' 调用此方法后，给变量赋值，并进行初始化（如设置标题），
    ''' 但并不需要进行内容的更新，此操作由 <see cref="Update" /> 方法完成。
    ''' </remarks>
    Sub Attach(ByVal newDocument As Document)

    ''' <summary>
    ''' （<see cref="Document" /> 专用）将视图与文档分离。
    ''' </summary>
    ''' <returns>此视图在此之前被绑定到的文档。</returns>
    ''' <remarks>
    ''' 注意：请不要直接调用此方法的实现，而应使用 <see cref="Document.RemoveView" />。
    ''' 待实现：
    ''' 调用此函数后，返回并清空文档的指针。
    ''' 由于视图离开文档后便失去存在价值，因此通常在此之后关闭视图。
    ''' </remarks>
    Function Detach() As Document

    ''' <summary>
    ''' 获取当前视图所绑定到的文档。
    ''' </summary>
    Function GetDocument() As Document

    ''' <summary>
    ''' 更新当前视图的内容。
    ''' </summary>
    ''' <param name="context">此次更新的上下文信息。</param>
    Sub Update(ByVal context As UpdateContext)

    ''' <summary>
    ''' 使视图获得焦点。
    ''' </summary>
    Sub SetFoucs()

    ''' <summary>
    ''' 就地预览视图。
    ''' </summary>
    Sub Preview()

    ''' <summary>
    ''' 视图关闭前发生。
    ''' </summary>
    Event Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
    ''' <summary>
    ''' 视图关闭后发生。
    ''' </summary>
    Event Closed(ByVal sender As Object, ByVal e As EventArgs)
End Interface

''' <summary>
''' 为与 <see cref="IDocumentView" /> 相关的事件提供数据。
''' </summary>
Public Class ViewEventArgs
    Inherits EventArgs

    Private m_View As IDocumentView

    ''' <summary>
    ''' 获取与此事件相关的 <see cref="IDocumentView" />。
    ''' </summary>
    Public ReadOnly Property View() As IDocumentView
        Get
            Return m_View
        End Get
    End Property

    ''' <summary>
    ''' 初始化。
    ''' </summary>
    ''' <param name="View">与此事件相关的 <see cref="IDocumentView" />。</param>
    Public Sub New(ByVal View As IDocumentView)
        m_View = View
    End Sub
End Class