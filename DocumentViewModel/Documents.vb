Imports System.IO
Imports System.ComponentModel

''' <summary>
''' 为与 <see cref="Document" /> 相关的事件提供数据。
''' </summary>
Public Class DocumentEventArgs
    Inherits EventArgs

    Private m_Document As Document

    ''' <summary>
    ''' 获取与此事件相关的 <see cref="Document" />。
    ''' </summary>
    Public ReadOnly Property Document() As Document
        Get
            Return m_Document
        End Get
    End Property

    ''' <summary>
    ''' 初始化。
    ''' </summary>
    ''' <param name="Document">与此事件相关的 <see cref="Document" />。</param>
    Public Sub New(ByVal Document As Document)
        m_Document = Document
    End Sub
End Class

''' <summary>
''' 用于描述视图列表发生的原因。
''' </summary>
Public Enum ViewChangeReason
    ''' <summary>
    ''' 表示指定的视图被加入列表。
    ''' </summary>
    AddView = 1
    ''' <summary>
    ''' 表示指定的视图从列表中被移除。
    ''' </summary>
    RemoveView
    ''' <summary>
    ''' 表示指定的视图由于其对应的文档正在执行 <see cref="Document.Dispose" /> 进行清理而被移除。
    ''' </summary>
    Dispose
End Enum

''' <summary>
''' 为 <see cref="Document.ViewListChanged" /> 事件提供数据。
''' </summary>
Public Class ViewListChangedEventArgs
    Inherits ViewEventArgs

    Private m_Reason As ViewChangeReason

    ''' <summary>
    ''' 获取引发此事件的原因。
    ''' </summary>
    Public ReadOnly Property Reason() As ViewChangeReason
        Get
            Return m_Reason
        End Get
    End Property

    ''' <summary>
    ''' 初始化。
    ''' </summary>
    ''' <param name="View">与此事件相关的 <see cref="IDocumentView" />。</param>
    ''' <param name="Reason">引发此事件的原因。</param>
    Public Sub New(ByVal View As IDocumentView, ByVal Reason As ViewChangeReason)
        MyBase.New(View)
        m_Reason = Reason
    End Sub
End Class

''' <summary>
''' 表示新建或打开文档的模式。
''' </summary>
<Flags()> Public Enum DocumentCreationModes
    ''' <summary>
    ''' 正常模式。
    ''' </summary>
    Normal = 0
    ''' <summary>
    ''' 只读模式
    ''' </summary>
    [ReadOnly] = 1
End Enum

''' <summary>
''' 为 <see cref="Document" /> 的派生类的实例的创建提供参数。
''' </summary>
Public Class DocumentCreationArguments
    Private m_IOContext As IOContext
    Private m_Mode As DocumentCreationModes

    ''' <summary>
    ''' 获取将被打开的 <see cref="IOContext" />，如果为 <c>null</c>，表示将新建文档。
    ''' </summary>
    Public ReadOnly Property IOContext() As IOContext
        Get
            Return m_IOContext
        End Get
    End Property

    ''' <summary>
    ''' 获取新建或打开文档的模式。
    ''' </summary>
    Public ReadOnly Property Mode() As DocumentCreationModes
        Get
            Return m_Mode
        End Get
    End Property

    ''' <summary>
    ''' 初始化。
    ''' </summary>
    ''' <param name="IOContext">设置将被打开的 <see cref="IOContext" />，如果为 <c>null</c>，表示将新建文档。</param>
    ''' <param name="Mode">设置新建或打开文档的模式。</param>
    Public Sub New(ByVal IOContext As IOContext, ByVal Mode As DocumentCreationModes)
        m_IOContext = ioContext
        m_Mode = Mode
    End Sub

    ''' <summary>
    ''' 初始化。
    ''' </summary>
    ''' <param name="IOContext">设置将被打开的 <see cref="IOContext" />，如果为 <c>null</c>，表示将新建文档。</param>
    Public Sub New(ByVal IOContext As IOContext)
        Me.New(IOContext, DocumentCreationModes.Normal)
    End Sub

    ''' <summary>
    ''' 初始化，其 <see cref="IOContext" /> 为 <c>null</c>，表示将新建文档。
    ''' </summary>
    Public Sub New()
        Me.New(Nothing)
    End Sub
End Class

''' <summary>
''' 文档基类。
''' </summary>
Public Class Document
    Implements IDisposable

    ''' <summary>
    ''' 当文档通过新建、打开等操作被创建后，在更新视图前触发此事件。
    ''' </summary>
    Public Event Created(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' 当文档将被保存时，触发此事件。
    ''' </summary>
    Public Event Saving(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' 当文档被成功保存时，触发此事件。
    ''' </summary>
    Public Event Saved(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' 在 Title 属性的值更改后发生。
    ''' </summary>
    Public Event TitleChanged(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' 在 Modified 属性的值更改后发生。
    ''' </summary>
    Public Event ModifiedChanged(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' 在 IOContext 属性的值更改后发生。
    ''' </summary>
    Public Event IOContextChanged(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' 在 Views 集合的内容更改后发生。
    ''' </summary>
    Public Event ViewListChanged(ByVal sender As Object, ByVal e As ViewListChangedEventArgs)
    ''' <summary>
    ''' 当文档关闭后，触发此事件
    ''' </summary>
    Public Event Closed(ByVal sender As Object, ByVal e As EventArgs)

    '属性的局部复制
    Private m_Title As String
    Private m_IsModified As Boolean
    Private m_ReadOnly As Boolean
    Private m_IOContext As IOContext
    Private m_Views As New LinkedList(Of IDocumentView)
    Private m_Owner As Managers.DocumentManager
    Private m_AutoClose As Boolean

    '状态标志
    Private m_IsDisposed As Boolean

    ''' <summary>
    ''' 获取/设置文档的标题。
    ''' </summary>
    <Category("Data"), DocumentDescriptionAttribute("DDocTitle")>
    Public Property Title() As String
        Get
            Return m_Title
        End Get
        Set(ByVal value As String)
            m_Title = value
            OnTitleChanged(EventArgs.Empty)
        End Set
    End Property

    ''' <summary>
    ''' 获取一个值，表示文档是否在上次保存以来已被修改。
    ''' </summary>
    <Browsable(False)>
    Public Property IsModified() As Boolean
        Get
            Return m_IsModified
        End Get
        Protected Set(ByVal value As Boolean)
            m_IsModified = value
            OnModifiedChanged(EventArgs.Empty)
        End Set
    End Property

    ''' <summary>
    ''' 将此文档的 <see cref="IsModified" /> 更改为 true。
    ''' </summary>
    Public Sub SetModified()
        IsModified = True
    End Sub

    ''' <summary>
    ''' 获取一个值，表示文档是否是只读的。
    ''' </summary>
    <Category("Behavior"), DocumentDescriptionAttribute("DDocReadOnly")> Public ReadOnly Property [ReadOnly]() As Boolean
        Get
            Return m_ReadOnly
        End Get
    End Property

    ''' <summary>
    ''' 获取一个值，表示文档是否能够自动关闭。
    ''' </summary>
    ''' <remarks>
    ''' <para>当满足以下要求时，文档会自动关闭：</para>
    ''' <list type="bullet">
    ''' <item><description><see cref="AutoClose" /> 为 <c>true</c>。</description></item>
    ''' <item><description>与此文档关联的最后一个视图被移除。</description></item>
    ''' </list>
    ''' </remarks>
    <Browsable(False)> Public Property AutoClose() As Boolean
        Get
            Return m_AutoClose
        End Get
        Set(ByVal value As Boolean)
            m_AutoClose = value
        End Set
    End Property

    ''' <summary>
    ''' 获取一个值，该值指示文档是否已经被释放。
    ''' </summary>
    <Browsable(False)> Public ReadOnly Property IsDisposed() As Boolean
        Get
            Return m_IsDisposed
        End Get
    End Property

    ''' <summary>
    ''' 获取被绑定到当前文档的主视图（仅约定）。
    ''' </summary>
    ''' <remarks>一般来说，列表中所有的视图的地位是相同的，但有时如果一定要确定一个主视图，则约定为列表中的首项。</remarks>
    <Browsable(False)> Public ReadOnly Property MainView() As IDocumentView
        Get
            Return If(m_Views.Count > 0, m_Views(0), Nothing)
        End Get
    End Property

    ''' <summary>
    ''' 获取文档当前的 IO 上下文。
    ''' </summary>
    <Category("Data"), DocumentDescriptionAttribute("DDocIOContext")> Public Property IOContext() As IOContext
        Get
            Return m_IOContext
        End Get
        Private Set(ByVal value As IOContext)
            If m_IOContext IsNot value Then
                m_IOContext = value
                OnIOContextChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    ''' <summary>
    ''' 获取或设置管理此文档的文档管理器。
    ''' </summary>
    ''' <value>一个 <see cref="Managers.DocumentManager" />，其文档列表中有此文档的实例。如果为 <c>null</c>，表示此文档未受管理。</value>
    <Browsable(False)> Public Property Owner() As Managers.DocumentManager
        Get
            Return m_Owner
        End Get
        Set(ByVal value As Managers.DocumentManager)
            If m_Owner IsNot value Then
                If m_Owner IsNot Nothing Then
                    m_Owner.RemoveDocument(Me)
                End If
                If value IsNot Nothing Then
                    value.AddDocument(Me)
                End If
            End If
        End Set
    End Property

    Friend Sub SetOwner(ByVal newOwner As Managers.DocumentManager)
        m_Owner = newOwner
    End Sub

    ''' <summary>
    ''' 在没有任何提示的情况下，完全关闭文档，并释放所有绑定到当前文档的视图。
    ''' </summary>
    ''' <remarks>此操作将调用 <c><see cref="Dispose" />(true)</c></remarks>
    Public Sub Close()
        Dispose(True)
    End Sub

    ''' <summary>
    ''' 使用当前的 IO 上下文保存文档，并在必要时显示“保存”对话框。
    ''' </summary>
    ''' <param name="ForceShowSaveDialog">如果为 True，则不论当前有没有存在 IO 上下文，一定要显示“保存”对话框（相当于“另存为”操作）。</param>
    ''' <returns>如果文档成功被保存，则返回 True；否则（如用户点了取消）返回 False。</returns>
    ''' <exception cref="ObjectDisposedException">在文档关闭后调用此方法。</exception>
    Public Function Save(ByVal ForceShowSaveDialog As Boolean) As Boolean
        If m_IsDisposed Then
            Throw GetDisposedException()
        Else
            If ForceShowSaveDialog OrElse m_IOContext Is Nothing Then
                '如果文档没有被保存过，则提示用户选择保存目标
                Using SaveDialog As New SaveFileDialog()
                    SaveDialog.Filter = DocumentInformation.GetInfo(Me.GetType).FileTypes.ToFilterDescription
                    SaveDialog.FileName = m_Title
                    If SaveDialog.ShowDialog = DialogResult.OK Then
                        Save(New FileIOContext(SaveDialog.FileName))
                        Return True
                    Else
                        Return False
                    End If
                End Using
            Else
                Save(m_IOContext)
                Return True
            End If
        End If
    End Function

    ''' <summary>
    ''' 使用当前的 IO 上下文保存文档，如果当前 IO 上下文不存在，则显示“保存”对话框。
    ''' </summary>
    ''' <returns>如果文档成功被保存，则返回 True；否则（如用户点了取消）返回 False。</returns>
    Public Function Save() As Boolean
        Return Save(False)
    End Function

    ''' <summary>
    ''' 使用指定的 IO 上下文保存文档，并替换当前文档的上下文。
    ''' </summary>
    ''' <param name="IOContext">文档 IO 的上下文信息。</param>
    ''' <exception cref="ObjectDisposedException">在文档关闭后调用此方法。</exception>
    Public Sub Save(ByVal IOContext As IOContext)
        If m_IsDisposed Then
            Throw GetDisposedException()
        Else
            OnSaving(EventArgs.Empty)
            SaveDocumentCore(IOContext, False)
            Me.IOContext = IOContext
            IsModified = False
            OnSaved(EventArgs.Empty)
        End If
    End Sub

    ''' <summary>
    ''' 使用指定的 IO 上下文导出文档，不改变当前文档的上下文以及改动标记。
    ''' </summary>
    ''' <param name="IOContext">文档 IO 的上下文信息。</param>
    Public Sub Export(ByVal IOContext As IOContext)
        SaveDocumentCore(IOContext, True)
    End Sub

    ''' <summary>
    ''' 如果文档发生更改，询问用户是否保存更改。
    ''' </summary>
    ''' <returns>返回 True 表示文档已经被保存；返回 False 表示文档尚未保存（可能被取消）。</returns>
    Public Function SaveModified() As Boolean
        Return SaveModified(False)
    End Function

    ''' <summary>
    ''' 如果文档发生更改，则保存修改。
    ''' </summary>
    ''' <param name="NoPrompt">指定是否在保存修改前提示用户。</param>
    ''' <returns>返回 True 表示文档已经被保存；返回 False 表示文档尚未保存（可能被取消）。</returns>
    Public Overridable Function SaveModified(ByVal NoPrompt As Boolean) As Boolean
        If m_IsDisposed Then
            Throw GetDisposedException()
        Else
            If m_IsModified Then
                If NoPrompt Then
                    Return Save()
                Else
                    Select Case MsgBox(String.Format(Prompts.SaveDocument, m_Title), MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                        Case MsgBoxResult.Yes
                            Return Save()
                        Case MsgBoxResult.No
                            Return True
                        Case MsgBoxResult.Cancel
                            Return False
                    End Select
                End If
            Else
                Return True
            End If
        End If
    End Function

    ''' <summary>
    ''' 返回一个 <see cref="ObjectDisposedException" />，用于引发一个由于 <see cref="Dispose" /> 已被调用而无法执行相关操作的异常。
    ''' </summary>
    Protected Function GetDisposedException() As Exception
        Return New ObjectDisposedException(Me.ToString)
    End Function

    ''' <summary>
    ''' 清除此文档的内容。
    ''' </summary>
    ''' <remarks>
    ''' <para>此方法没有默认实现，重写时不必调用。</para>
    ''' <para>重写此方法时，请不要设置 <see cref="IsModified" /> 标志。此标志在此方法调用后由调用方决定是否修改。这就意味着，外界不能通过此方法来设置 <see cref="IsModified" />。</para>
    ''' </remarks>
    Public Overridable Sub ClearContent()

    End Sub

    ''' <summary>
    ''' 在派生类中重写时，在新建文档时执行一些特殊的初始化操作。
    ''' </summary>
    ''' <remarks>
    ''' <para>此方法没有默认实现，重写时不必调用。</para>
    ''' </remarks>
    Protected Overridable Sub NewDocumentCore()

    End Sub

    ''' <summary>
    ''' 从指定的 <see cref="DocumentViewModel.IOContext" /> 中载入文档。
    ''' </summary>
    ''' <param name="IOContext">要从中载入文档内容的 <see cref="DocumentViewModel.IOContext" />，其不可能为 <c>null</c>。</param>
    ''' <remarks>
    ''' <para>此方法的默认实现是调用 <see cref="IOContext.OpenStream" /> 打开一个流，并根据上下文对文档设置一个默认标题，然后调用 <see cref="ReadDocumentStream" /> 载入文档内容。</para>
    ''' <para>重写此方法时，您可以使用自己的程序控制文档的打开，并决定是否调用 <see cref="ReadDocumentStream" />。</para>
    ''' </remarks>
    Protected Overridable Sub OpenDocumentCore(ByVal IOContext As IOContext)
        Dim ios As Stream = IOContext.OpenStream(StreamMode.ForOpen)
        Title = IOContext.Name
        Try
            ReadDocumentStream(ios)
        Catch ex As Exception
            Throw
        Finally
            ios.Close()
        End Try
    End Sub

    ''' <summary>
    ''' 向指定的 <see cref="DocumentViewModel.IOContext" /> 写入文档。
    ''' </summary>
    ''' <param name="IOContext">要写入文档内容的 <see cref="DocumentViewModel.IOContext" />，其不可能为 <c>null</c>。</param>
    ''' <param name="IsExport">表示向此 <see cref="DocumentViewModel.IOContext" /> 写入文档后，是否保留当前的 <see cref="IOContext" /> 设置（导出文档）。</param>
    ''' <remarks>
    ''' <para>此方法的默认实现是调用 <see cref="IOContext.OpenStream" /> 打开一个流，然后调用 <see cref="WriteDocumentStream" /> 写入文档内容。</para>
    ''' <para>重写此方法时，您可以使用自己的程序控制文档的保存，并决定是否调用 <see cref="WriteDocumentStream" />。</para>
    ''' <para><paramref name="IsExport" /> 仅作为一个标志，修改 <see cref="IOContext" /> 设置是由调用方完成的。</para>
    ''' </remarks>
    Protected Overridable Sub SaveDocumentCore(ByVal IOContext As IOContext, ByVal IsExport As Boolean)
        Dim ios As Stream = IOContext.OpenStream(StreamMode.ForSave)
        Try
            WriteDocumentStream(ios)
        Catch ex As Exception
            Throw
        Finally
            ios.Close()
        End Try
    End Sub

    ''' <summary>
    ''' 向指定的流写入文档。
    ''' </summary>
    ''' <param name="dest">目标流。</param>
    ''' <remarks>
    ''' <para>此方法没有默认实现，重写时不必调用。</para>
    ''' <para>在派生类中重写时，此方法是否调用取决于 <see cref="SaveDocumentCore" /> 的行为。</para>
    ''' <para>您可以在此处完成文档标题的保存工作。</para>
    ''' </remarks>
    Protected Overridable Sub WriteDocumentStream(ByVal dest As Stream)

    End Sub

    ''' <summary>
    ''' 从指定的流载入文档。
    ''' </summary>
    ''' <param name="dest">目标流。</param>
    ''' <remarks>
    ''' <para>此方法没有默认实现，重写时不必调用。</para>
    ''' <para>在派生类中重写时，此方法是否调用取决于 <see cref="OpenDocumentCore" /> 的行为。</para>
    ''' <para>您可以在此处完成文档标题的恢复工作。</para>
    ''' </remarks>
    Protected Overridable Sub ReadDocumentStream(ByVal dest As Stream)

    End Sub

    ''' <summary>
    ''' 获取被绑定到当前文档的视图列表。
    ''' </summary>
    ''' <remarks>一般来说，列表中所有的视图的地位是相同的，但有时如果一定要确定一个主视图，则约定为列表中的首项。</remarks>
    <Browsable(False)> Public ReadOnly Property Views() As IEnumerable(Of IDocumentView)
        Get
            Static EnumerableList As IEnumerable(Of IDocumentView) = m_Views.AsEnumerable
            Return EnumerableList
        End Get
    End Property

    ''' <summary>
    ''' 将一个视图绑定到当前文档。
    ''' </summary>
    ''' <param name="View">要绑定的视图。</param>
    ''' <param name="NoUpdate">指定是否在此操作完成后不自动更新视图。</param>
    ''' <exception cref="ArgumentNullException">指定的视图为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentException">指定的视图在视图列表中已存在。</exception>
    ''' <exception cref="InvalidOperationException">指定的视图已经被绑定。请先释放此视图，再进行绑定。</exception>
    Public Sub AddView(ByVal View As IDocumentView, ByVal NoUpdate As Boolean)
        If View Is Nothing Then
            Throw New ArgumentNullException("view")
        ElseIf m_Views.Contains(View) Then
            Throw New ArgumentException(String.Format(ExceptionPrompts.ItemExist, View.ToString), "view")
        ElseIf View.GetDocument IsNot Nothing Then
            Throw New InvalidOperationException(String.Format(ExceptionPrompts.ViewAttached, View.GetDocument.ToString))
        Else
            View.Attach(Me)
            m_Views.AddLast(View)
            AddHandler View.Closing, AddressOf ViewInList_Closing
            AddHandler View.Closed, AddressOf ViewInList_Closed
            If Not NoUpdate Then View.Update(New UpdateContext(UpdateReason.Initializing))
            OnViewListChanged(New ViewListChangedEventArgs(View, ViewChangeReason.AddView))
        End If
    End Sub

    ''' <summary>
    ''' 将一个视图绑定到当前文档，并在此操作完成后自动更新视图。
    ''' </summary>
    ''' <param name="View">要绑定的视图。</param>
    ''' <exception cref="ArgumentNullException">指定的视图为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentException">指定的视图在视图列表中已存在。</exception>
    ''' <exception cref="InvalidOperationException">指定的视图已经被绑定。请先释放此视图，再进行绑定。</exception>
    ''' <exception cref="ArgumentOutOfRangeException">指定的视图不支持当前的文档类型。</exception>
    Public Sub AddView(ByVal View As IDocumentView)
        AddView(View, False)
    End Sub

    ''' <summary>
    ''' 将一个绑定到当前文档的视图释放。
    ''' </summary>
    ''' <param name="View">要释放的视图。</param>
    ''' <returns>如果成功，返回 True；如果在视图列表中没有找到或释放失败，返回 False。</returns>
    ''' <remarks>（约定）如果是 MDI，则此视图在此之后应被关闭；如果是 SDI，则整个程序应被关闭。</remarks>
    Public Function RemoveView(ByVal View As IDocumentView) As Boolean
        If m_Views.Remove(View) Then
            RemoveHandler View.Closing, AddressOf ViewInList_Closing
            RemoveHandler View.Closed, AddressOf ViewInList_Closed
            View.Detach()
            '此处不再更新视图。
            OnViewListChanged(New ViewListChangedEventArgs(View, ViewChangeReason.RemoveView))
            Return True
        Else
            Return False
        End If
    End Function

    '由于此操作完成后，势必会导致 CloseDocument 的调用，因此此过程被移到 CloseDocument 过程中
    '''' <summary>
    '''' 释放所有绑定到当前文档的视图。
    '''' </summary>
    'Public Sub RemoveAllViews()

    ''' <summary>
    ''' 更新绑定到当前文档的所有视图。
    ''' </summary>
    Public Sub UpdateAllViews()
        UpdateAllViews(UpdateReason.Unknown, Nothing, Nothing, False)
    End Sub

    ''' <summary>
    ''' 更新绑定到当前文档的所有视图（除了发起者）。
    ''' </summary>
    ''' <param name="sender">发起视图更新的视图，如果为 <c>null</c>，则表示发起者未知。</param>
    ''' <param name="context">由调用方定义的附加上下文，可为 <c>null</c>。</param>
    Public Sub UpdateAllViews(ByVal sender As IDocumentView, ByVal context As Object)
        UpdateAllViews(If(sender Is Nothing, UpdateReason.Unknown, UpdateReason.View), Nothing, context, False)
    End Sub

    ''' <summary>
    ''' 更新绑定到当前文档的所有视图（除了发起者）。
    ''' </summary>
    ''' <param name="sender">发起视图更新的视图，如果为 <c>null</c>，则表示发起者未知。</param>
    ''' <param name="context">由调用方定义的附加上下文，可为 <c>null</c>。</param>
    ''' <param name="updateSender">表示是否更新发起者视图。</param>
    Public Sub UpdateAllViews(ByVal sender As IDocumentView, ByVal context As Object, ByVal updateSender As Boolean)
        UpdateAllViews(If(sender Is Nothing, UpdateReason.Unknown, UpdateReason.View), Nothing, context, updateSender)
    End Sub

    ''' <summary>
    ''' [内部调用]更新绑定到当前文档的所有视图。
    ''' </summary>
    ''' <param name="reason">发起视图更新的原因。</param>
    ''' <param name="sender">发起此次更新的视图，如果为 <c>null</c>，则表示发起者未知。</param>
    ''' <param name="context">由调用方定义的附加上下文，可为 <c>null</c>。</param>
    ''' <param name="updateSender">表示是否更新发起者视图。</param>
    Protected Sub UpdateAllViews(ByVal reason As UpdateReason, ByVal sender As IDocumentView, ByVal context As Object, ByVal updateSender As Boolean)
        For Each EachView In m_Views     '依次更新列表中的视图，排除掉 Sender
            If updateSender OrElse EachView IsNot sender Then
                EachView.Update(New UpdateContext(reason, sender, context))
            End If
        Next
    End Sub

    ''' <summary>
    ''' 引发 <see cref="Created"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnCreated(ByVal e As EventArgs)
        RaiseEvent Created(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="Saving"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnSaving(ByVal e As EventArgs)
        RaiseEvent Saving(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="Saved"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnSaved(ByVal e As EventArgs)
        RaiseEvent Saved(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="Closed"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnClosed(ByVal e As EventArgs)
        RaiseEvent Closed(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="TitleChanged"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnTitleChanged(ByVal e As EventArgs)
        RaiseEvent TitleChanged(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="ModifiedChanged"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnModifiedChanged(ByVal e As EventArgs)
        RaiseEvent ModifiedChanged(Me, e)
    End Sub
    ''' <summary>
    ''' 引发 <see cref="IOContextChanged"/> 事件。
    ''' </summary>
    ''' <remarks>此时的 IOContext 对象可能为 <c>null</c></remarks>
    Protected Overridable Sub OnIOContextChanged(ByVal e As EventArgs)
        RaiseEvent IOContextChanged(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 <see cref="ViewListChanged"/> 事件。
    ''' </summary>
    Protected Overridable Sub OnViewListChanged(ByVal e As ViewListChangedEventArgs)
        RaiseEvent ViewListChanged(Me, e)
        If m_AutoClose AndAlso m_Views.Count = 0 AndAlso e IsNot Nothing AndAlso e.Reason <> ViewChangeReason.Dispose Then
            Close()
        End If
    End Sub

    ''' <summary>
    ''' 返回包含是否被改动的标志（*）的当前文档的标题。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return m_Title & If(m_IsModified, "*", "")
    End Function

    ''' <summary>
    ''' 构造一个文档对象的实例。
    ''' </summary>
    Protected Sub New()
        'WARNING 不要在此处调用虚函数！
        'ClearContent()
        '初始化属性
        m_AutoClose = True
    End Sub

    ''' <summary>
    ''' 使用指定参数构造一个文档实例，并新建文档。
    ''' </summary>
    ''' <typeparam name="TDocument">要构造实例的继承自 <see cref="Document" /> 的类型。</typeparam>
    Public Shared Function CreateDocument(Of TDocument As Document)() As TDocument
        Return DirectCast(CreateDocument(GetType(TDocument), Nothing), TDocument)
    End Function
    ''' <summary>
    ''' 使用指定参数构造一个文档实例。
    ''' </summary>
    ''' <typeparam name="TDocument">要构造实例的继承自 <see cref="Document" /> 的类型。</typeparam>
    ''' <param name="arguments">为文档的创建提供参数，若为 <c>null</c>，则将提供默认参数，表示将新建文档。</param>
    Public Shared Function CreateDocument(Of TDocument As Document)(ByVal arguments As DocumentCreationArguments) As TDocument
        Return DirectCast(CreateDocument(GetType(TDocument), arguments), TDocument)
    End Function

    ''' <summary>
    ''' 使用指定参数构造一个文档实例，并新建文档。
    ''' </summary>
    ''' <param name="documentType">要构造实例的继承自 <see cref="Document" /> 的类型。</param>
    ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentOutOfRangeException">指定的类型不是文档类。</exception>
    Public Shared Function CreateDocument(ByVal documentType As Type) As Document
        Return CreateDocument(documentType, Nothing)
    End Function

    ''' <summary>
    ''' 使用指定参数构造一个文档实例。
    ''' </summary>
    ''' <param name="documentType">要构造实例的继承自 <see cref="Document" /> 的类型。</param>
    ''' <param name="arguments">为文档的创建提供参数，若为 <c>null</c>，则将提供默认参数，表示将新建文档。</param>
    ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentOutOfRangeException">指定的类型不是文档类。</exception>
    Public Shared Function CreateDocument(ByVal documentType As Type, ByVal arguments As DocumentCreationArguments) As Document
        If documentType Is Nothing Then
            Throw New ArgumentNullException("documentType")
        ElseIf Not GetType(Document).IsAssignableFrom(documentType) Then
            Throw New ArgumentOutOfRangeException("documentType", documentType, String.Format(ExceptionPrompts.NotAssignableFrom, GetType(Document)))
        Else
            Dim NewDoc = DirectCast(Activator.CreateInstance(documentType, True), Document)
            NewDoc.ClearContent()
            '清理文档，然后交给继承者
            If arguments Is Nothing Then
                NewDoc.m_ReadOnly = False
                NewDoc.NewDocumentCore()
                NewDoc.IOContext = Nothing
            Else
                NewDoc.m_ReadOnly = (arguments.Mode And DocumentCreationModes.ReadOnly) = DocumentCreationModes.ReadOnly
                If arguments Is Nothing OrElse arguments.IOContext Is Nothing Then
                    NewDoc.NewDocumentCore()
                Else
                    NewDoc.OpenDocumentCore(arguments.IOContext)
                End If
                NewDoc.IOContext = arguments.IOContext
            End If
            NewDoc.OnCreated(EventArgs.Empty)
            Return NewDoc
        End If
    End Function

    '此处的 Closing 和 Closed 均指视图“主动”关闭。如果是
    Private Sub ViewInList_Closed(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim senderView As IDocumentView = TryCast(sender, IDocumentView)
        If senderView IsNot Nothing AndAlso senderView.GetDocument Is Me Then
            Me.RemoveView(senderView)   '在视图关闭时释放之
        End If
    End Sub

    Private Sub ViewInList_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        'Dim senderView As IDocumentView = TryCast(sender, IDocumentView)
        If m_Views.Count = 1 Then
            If Me.SaveModified() = False Then e.Cancel = True
        End If
    End Sub

    ''' <summary>
    ''' 释放由此文档使用的资源（内存除外）。
    ''' </summary>
    ''' <param name="disposing">为 <c>true</c> 则释放托管资源和非托管资源；为 <c>false</c> 则仅释放非托管资源。</param>
    ''' <remarks>Dispose 可以由其他对象多次调用。重写 Dispose(Boolean) 时，请注意不要引用在以前调用 Dispose 时已释放的对象。</remarks>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not IsDisposed Then
            If disposing Then
                '释放托管对象

                '释放所有视图
                Do While m_Views.Any
                    '不能使用遍历，因为关闭文档后，列表被修改（项数减少）
                    RemoveView(m_Views(0))
                Loop

                IsModified = False
                m_Title = Nothing
                m_IOContext = Nothing

                '引发相关事件
                ClearContent()
                OnClosed(New EventArgs)
            End If
            '释放您非托管对象
            '将大型字段设置为 null

            m_IsDisposed = True
        End If
    End Sub

    ''' <summary>
    ''' 释放由此文档使用的所有资源。
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me) '使垃圾站不再调用此实例的 Finalize 方法
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        m_IsDisposed = True
        MyBase.Finalize()
    End Sub
End Class

Friend Class DocumentDescriptionAttribute
    Inherits DescriptionAttribute
    Private Localized As Boolean

    Public Overrides ReadOnly Property Description() As String
        Get
            If Not Localized Then
                Localized = True
                Me.DescriptionValue = Prompts.ResourceManager.GetString(MyBase.Description)
            End If
            Return MyBase.Description
        End Get
    End Property

    ''' <summary>
    ''' 初始化
    ''' </summary>
    Public Sub New(ByVal descriptionKey As String)
        MyBase.New(descriptionKey)
    End Sub
End Class