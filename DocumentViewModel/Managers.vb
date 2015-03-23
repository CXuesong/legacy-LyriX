Imports System.IO

Namespace Managers

    ''' <summary>
    ''' 用于在 <see cref="DocumentManager" /> 中为文档类型设置附加信息。
    ''' </summary>
    Public Class DocumentTypeInfo
        Private m_UntitledCount As Long

        ''' <summary>
        ''' 获取未命名文档的计数器。
        ''' </summary>
        Public ReadOnly Property UntitledCount() As Long
            Get
                Return m_UntitledCount
            End Get
        End Property

        ''' <summary>
        ''' 递增未命名文档的计数器。
        ''' </summary>
        Public Sub StepUntitiledCount()
            m_UntitledCount += 1
        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <remarks>初始化后，未命名文档的计数器设置为 0。</remarks>
        Friend Sub New()

        End Sub
    End Class

    ''' <summary>
    ''' 用于为 <see cref="DocumentManager" /> 提供打开文档时所需的参数。
    ''' </summary>
    Public Class DocumentOpenArguments
        Inherits DocumentCreationArguments
        Private m_OpenWith As Type

        ''' <summary>
        ''' 获取/设置此 <see cref="IOContext" /> 的打开方式，<c>null</c> 表示未定。
        ''' </summary>
        Public Property OpenWith() As Type
            Get
                Return m_OpenWith
            End Get
            Set(ByVal value As Type)
                m_OpenWith = value
            End Set
        End Property

        ''' <summary>
        ''' 初始化
        ''' </summary>
        ''' <param name="IOContext">设置一个 <see cref="IOContext" />，新的文档将从中打开。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="IOContext" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal IOContext As IOContext)
            Me.New(IOContext, Nothing)
        End Sub

        ''' <summary>
        ''' 初始化
        ''' </summary>
        ''' <param name="IOContext">设置一个 <see cref="IOContext" />，新的文档将从中打开。</param>
        ''' <param name="OpenWith">设置此 <see cref="IOContext" /> 的打开方式，<c>null</c> 表示未定。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="IOContext" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal IOContext As IOContext, ByVal OpenWith As Type)
            Me.New(IOContext, DocumentCreationModes.Normal, OpenWith)
        End Sub
        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="ioContext">设置将被打开的 <see cref="IOContext" />，如果为 <c>null</c>，表示将新建文档。</param>
        ''' <param name="OpenWith">设置此 <see cref="IOContext" /> 的打开方式，<c>null</c> 表示未定。</param>
        ''' <param name="Mode">设置新建或打开文档的模式。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="IOContext" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal IOContext As IOContext, ByVal Mode As DocumentCreationModes, ByVal OpenWith As Type)
            MyBase.New(IOContext, Mode)
            If IOContext Is Nothing Then
                '秋后算账~~
                Throw New ArgumentNullException("IOContext")
            Else
                m_OpenWith = OpenWith
            End If
        End Sub
    End Class

    ''' <summary>
    ''' 表示多文档操作的状态或对其的操作。
    ''' </summary>
    Public Enum DocumentProcessAction
        ''' <summary>
        ''' 操作将继续进行，如果当前操作出现错误，此项表示将重试。
        ''' </summary>
        Passthrough = 0
        ''' <summary>
        ''' 表示将取消对当前文档的操作。
        ''' </summary>
        CancelCurrent
        ''' <summary>
        ''' 表示将取消对此次任务中当前及以后的所有文档的操作。
        ''' </summary>
        CancelAll
    End Enum

    ''' <summary>
    ''' 为多文档打开的相关事件提供数据。
    ''' </summary>
    Public Class DocumentOpenEventArgs
        Private m_CurrentIndex As Integer
        Private m_TotalCount As Integer
        Private m_Action As DocumentProcessAction

        ''' <summary>
        ''' 设置/获取此次任务的状态或对其的操作。
        ''' </summary>
        Public Property Action() As DocumentProcessAction
            Get
                Return m_Action
            End Get
            Set(ByVal value As DocumentProcessAction)
                Try
                    BeforeStatusChange(value)
                Catch ex As Exception
                    Throw
                    Exit Property
                End Try
                m_Action = value
            End Set
        End Property

        ''' <summary>
        ''' 获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。
        ''' </summary>
        Public ReadOnly Property CurrentIndex() As Integer
            Get
                Return m_CurrentIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取在当前任务中所有待打开的文档数量。
        ''' </summary>
        Public ReadOnly Property TotalCount() As Integer
            Get
                Return m_TotalCount
            End Get
        End Property

        ''' <summary>
        ''' 在 <see cref="Action" /> 发生改变之前引发。
        ''' </summary>
        ''' <param name="NewValue">待设置的新值。</param>
        ''' <remarks>实现说明：不用调用此处基类的实现（因为实现为空）。</remarks>
        Protected Overridable Sub BeforeStatusChange(ByRef NewValue As DocumentProcessAction)

        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        ''' <param name="Action">设置对此次任务的默认操作。</param>
        Public Sub New(ByVal CurrentIndex As Integer, ByVal TotalCount As Integer, ByVal Action As DocumentProcessAction)
            m_CurrentIndex = CurrentIndex
            m_TotalCount = TotalCount
        End Sub
    End Class

    ''' <summary>
    ''' 为 <see cref="DocumentManager.DocumentOpening" /> 提供数据。
    ''' </summary>
    Public Class DocumentOpeningEventArgs
        Inherits DocumentOpenEventArgs

        Private m_Arguments As DocumentOpenArguments

        ''' <summary>
        ''' 获取打开此文档所使用的参数。
        ''' </summary>
        Public ReadOnly Property Arguments() As DocumentOpenArguments
            Get
                Return m_Arguments
            End Get
        End Property

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="Arguments">设置打开此文档所使用的参数。</param>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        ''' <param name="Action">设置此次任务的状态或对建议对其的操作。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Arguments" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal Arguments As DocumentOpenArguments, ByVal CurrentIndex As Integer, ByVal TotalCount As Integer, ByVal Action As DocumentProcessAction)
            MyBase.New(CurrentIndex, TotalCount, Action)
            If Arguments Is Nothing Then
                Throw New ArgumentNullException("Arguments")
            Else
                m_Arguments = Arguments
            End If
        End Sub

        ''' <summary>
        ''' 初始化，其 <see cref="Action" /> 为 <see cref="DocumentProcessAction.Passthrough" />。
        ''' </summary>
        ''' <param name="Arguments">设置打开此文档所使用的参数。</param>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Arguments" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal Arguments As DocumentOpenArguments, ByVal CurrentIndex As Integer, ByVal TotalCount As Integer)
            Me.New(Arguments, CurrentIndex, TotalCount, DocumentProcessAction.Passthrough)
        End Sub
    End Class

    ''' <summary>
    ''' 为 <see cref="DocumentManager.DocumentOpened" /> 提供数据。
    ''' </summary>
    Public Class DocumentOpenedEventArgs
        Inherits DocumentOpenEventArgs

        Private m_Document As Document

        ''' <summary>
        ''' 获取打开此文档所使用的参数。
        ''' </summary>
        Public ReadOnly Property Document() As Document
            Get
                Return m_Document
            End Get
        End Property

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="Document">设置被打开的文档。</param>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        ''' <param name="Action">设置此次任务的状态或对建议对其的操作。</param>
        Public Sub New(ByVal Document As Document, ByVal CurrentIndex As Integer, ByVal TotalCount As Integer, ByVal Action As DocumentProcessAction)
            MyBase.New(CurrentIndex, TotalCount, Action)
            m_Document = Document
        End Sub

        ''' <summary>
        ''' 初始化，其 <see cref="Action" /> 为 <see cref="DocumentProcessAction.Passthrough" />。
        ''' </summary>
        ''' <param name="Document">设置被打开的文档。</param>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        Public Sub New(ByVal Document As Document, ByVal CurrentIndex As Integer, ByVal TotalCount As Integer)
            Me.New(Document, CurrentIndex, TotalCount, DocumentProcessAction.Passthrough)
        End Sub

        Protected Overrides Sub BeforeStatusChange(ByRef NewValue As DocumentProcessAction)
            '覆水难收
            If NewValue = DocumentProcessAction.CancelCurrent Then
                Throw New InvalidOperationException()
            End If
        End Sub
    End Class

    ''' <summary>
    ''' 为 <see cref="DocumentManager.OpenDocumentError" /> 提供数据。
    ''' </summary>
    Public Class OpenDocumentErrorEventArgs
        Inherits DocumentOpenEventArgs

        Private m_Arguments As DocumentOpenArguments
        Private m_Exception As Exception

        ''' <summary>
        ''' 获取打开此文档所使用的参数。
        ''' </summary>
        Public ReadOnly Property Arguments() As DocumentOpenArguments
            Get
                Return m_Arguments
            End Get
        End Property

        ''' <summary>
        ''' 获取导致此事件引发的 <see cref="Exception" />。
        ''' </summary>
        Public ReadOnly Property Exception() As Exception
            Get
                Return m_Exception
            End Get
        End Property

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="Arguments">设置打开此文档所使用的参数。</param>
        ''' <param name="Exception">设置导致此事件引发的 <see cref="Exception" />。</param>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        ''' <param name="Action">设置此次任务的状态或对建议对其的操作。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Arguments" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal Arguments As DocumentOpenArguments, ByVal Exception As Exception, ByVal CurrentIndex As Integer, ByVal TotalCount As Integer, ByVal Action As DocumentProcessAction)
            MyBase.New(CurrentIndex, TotalCount, Action)
            If Arguments Is Nothing Then
                Throw New ArgumentNullException("Arguments")
            Else
                m_Arguments = Arguments
                m_Exception = Exception
            End If
        End Sub

        ''' <summary>
        ''' 初始化，其 <see cref="Action" /> 为 <see cref="DocumentProcessAction.Passthrough" />。
        ''' </summary>
        ''' <param name="Arguments">设置打开此文档所使用的参数。</param>
        ''' <param name="Exception">设置导致此事件引发的 <see cref="Exception" />。</param>
        ''' <param name="CurrentIndex">获取此文档在当前任务中待打开的文档列表中以 0 为下标的位置。</param>
        ''' <param name="TotalCount">在当前任务中所有待打开的文档数量。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Arguments" /> 为 <c>null</c>。</exception>
        Public Sub New(ByVal Arguments As DocumentOpenArguments, ByVal Exception As Exception, ByVal CurrentIndex As Integer, ByVal TotalCount As Integer)
            Me.New(Arguments, Exception, CurrentIndex, TotalCount, DocumentProcessAction.Passthrough)
        End Sub
    End Class

    ''' <summary>
    ''' 负责对 <see cref="Document" /> 的集合的管理以及向用户界面的公开。
    ''' </summary>
    Public Class DocumentManager
        Private m_DocumentTypes As New Dictionary(Of Type, DocumentTypeInfo)    '一堆文档类型
        Private m_Documents As New LinkedList(Of Document)    '一堆文档

        ''' <summary>
        ''' 表示一个空文档列表。
        ''' </summary>
        Protected Shared ReadOnly EmptyDocumentList As IList(Of Document) = Array.AsReadOnly(New Document() {})

        ''' <summary>
        ''' 当一个新的文档实例被创建时，触发此事件。
        ''' </summary>
        Public Event DocumentCreated(ByVal sender As Object, ByVal e As DocumentEventArgs)
        ''' <summary>
        ''' 当一个新的视图实例被创建时，触发此事件。
        ''' </summary>
        Public Event ViewCreated(ByVal sender As Object, ByVal e As ViewEventArgs)
        ''' <summary>
        ''' 当一个文档正在被打开时，触发此事件。
        ''' </summary>
        Public Event DocumentOpening(ByVal sender As Object, ByVal e As DocumentOpeningEventArgs)
        ''' <summary>
        ''' 当一个文档被打开时，触发此事件。
        ''' </summary>
        Public Event DocumentOpened(ByVal sender As Object, ByVal e As DocumentOpenedEventArgs)
        ''' <summary>
        ''' 当打开文档发生失败时，触发此事件。
        ''' </summary>
        Public Event OpenDocumentError(ByVal sender As Object, ByVal e As OpenDocumentErrorEventArgs)

        ''' <summary>
        ''' 启用应用程序的可视样式。
        ''' </summary>
        Public Shared Sub EnableVisualStyles()
            Application.EnableVisualStyles()
        End Sub

        ''' <summary>
        ''' 添加一个文档类型
        ''' </summary>
        ''' <param name="documentType">指定要添加的继承自 <see cref="Document" /> 的类的类型。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
        ''' <exception cref="ArgumentOutOfRangeException">指定的类型不是文档类。</exception>
        ''' <exception cref="ArgumentException"> 指定的文档类型已存在。</exception>
        Public Sub AddDocumentType(ByVal documentType As Type)
            If documentType IsNot Nothing Then
                If GetType(Document).IsAssignableFrom(documentType) Then
                    If Not m_DocumentTypes.ContainsKey(documentType) Then
                        m_DocumentTypes.Add(documentType, New DocumentTypeInfo)
                    Else
                        Throw New ArgumentException(String.Format(ExceptionPrompts.ItemExist, documentType.ToString), "documentType")
                    End If
                Else
                    Throw New ArgumentOutOfRangeException("documentType", documentType, String.Format(ExceptionPrompts.NotAssignableFrom, GetType(Document)))
                End If
            Else
                Throw New ArgumentNullException("documentType")
            End If
        End Sub

        ''' <summary>
        ''' 添加数个文档类型
        ''' </summary>
        ''' <param name="documentTypes">指定要添加的继承自 <see cref="Document" /> 的类的类型。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="documentTypes" /> 中的任意一项为 <c>null</c>。</exception>
        ''' <exception cref="ArgumentOutOfRangeException">指定的类型中的任意一项不是文档类。</exception>
        ''' <exception cref="ArgumentException"> 指定的文档类型中的任意一项已存在。</exception>
        Public Sub AddDocumentType(ByVal ParamArray documentTypes() As Type)
            For Each EachType In documentTypes
                AddDocumentType(EachType)
            Next
        End Sub

        ''' <summary>
        ''' 向文档列表中添加一个文档。
        ''' </summary>
        ''' <param name="document">要添加的文档。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="document" /> 为 <c>null</c>。</exception>
        ''' <exception cref="ArgumentException">在文档类型列表中找不到可以分配 <paramref name="document" /> 的类型。</exception>
        Public Sub AddDocument(ByVal document As Document)
            If document Is Nothing Then
                Throw New ArgumentNullException("document")
            ElseIf Not m_DocumentTypes.Keys.Contains(document.GetType) Then
                Throw New ArgumentException(String.Format(ExceptionPrompts.InvalidInstanceTypeInList, "document"))
            Else
                _AddDocument(document)
            End If
        End Sub

        Private Sub _AddDocument(ByVal document As Document)
            m_Documents.AddLast(document)
            AddHandler document.Closed, AddressOf ChildDocument_Closed
            '如果文档在加入列表前意外关闭，则引发 Closed 事件时，删除操作不会成功，也不会报错
            document.SetOwner(Me)
        End Sub

        ''' <summary>
        ''' 从文档列表中移除一个文档。
        ''' </summary>
        ''' <param name="document">要移除的文档。</param>
        ''' <returns>如果成功移除，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        Public Function RemoveDocument(ByVal document As Document) As Boolean
            RemoveHandler document.Closed, AddressOf ChildDocument_Closed
            Return m_Documents.Remove(document)
        End Function

        'WARNING    注意
        '此处暂不提供删除文档类型的方法（暂时用不上），
        '但在实现时需注意妥善处理已经被移除的 ManagedDocumentType（它还保留着指向此文档管理器的 Manager 属性）。

        ''' <summary>
        ''' 显示“新建”对话框（如果有多个模板）并新建文档。
        ''' </summary>
        ''' <param name="AlwaysShowDialog">指定是否总是显示对话框。</param>
        ''' <returns>从文件打开的文档；如果用户取消或文档类型列表为空，则返回 <c>null</c>。</returns>
        Public Function NewDocument(ByVal AlwaysShowDialog As Boolean) As Document
            Dim DocIndex As Integer
            If m_DocumentTypes.Count = 0 Then
                DocIndex = -1   '表示将返回 Nothing
            Else
                If AlwaysShowDialog OrElse m_DocumentTypes.Skip(1).Any Then
                    Using Selector As New DocumentTypeSelector
                        Selector.Text = Prompts.NewDocument
                        Selector.DocumentTypes = m_DocumentTypes.Keys
                        If Selector.ShowDialog = DialogResult.OK Then
                            DocIndex = Selector.SelectedDocumentIndex
                        Else
                            DocIndex = -1
                        End If
                    End Using
                End If
            End If
            If DocIndex >= 0 Then
                Dim NewDocTypeInfo = m_DocumentTypes.ElementAt(DocIndex)
                Dim NewDoc = CreateDocument(NewDocTypeInfo.Key, Nothing)
                NewDocTypeInfo.Value.StepUntitiledCount()
                With DocumentInformation.GetInfo(NewDocTypeInfo.Key)
                    If .DefaultTitle = Nothing Then
                        NewDoc.Title = Nothing
                    Else
                        NewDoc.Title = String.Format(.DefaultTitle, NewDocTypeInfo.Value.UntitledCount)
                    End If
                End With

                CreateView(NewDoc)      '为新文档创建视图

                _AddDocument(NewDoc)
                Return NewDoc
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 显示“新建”对话框（如果有多个模板）并新建文档。
        ''' </summary>
        ''' <returns>从文件打开的文档；如果用户取消或文档类型列表为空，则返回 <c>null</c>。</returns>
        Public Function NewDocument() As Document
            Return NewDocument(False)
        End Function

        ''' <summary>
        ''' 显示“打开”对话框并打开一个或多个文件。
        ''' </summary>
        Public Function OpenDocument() As IList(Of Document)
            Return OpenDocument(False)
        End Function

        ''' <summary>
        ''' 显示“打开”对话框并打开一个或多个文件。
        ''' </summary>
        ''' <param name="SingleSelection">指定是否禁止多选。</param>
        ''' <returns>从文件打开的文档；如果用户取消，则返回空数组。</returns>
        Public Function OpenDocument(ByVal SingleSelection As Boolean) As IList(Of Document)
            Return OpenDocumentCore(SingleSelection)
        End Function

        ''' <summary>
        ''' 使用指定的参数打开一个或数个文档，并在必要时显示“选择文档类型”对话框。
        ''' </summary>
        ''' <param name="Arguments">为打开文档操作提供参数，若为 <c>null</c>，则表示不打开任何文档。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Arguments" /> 为 <c>null</c>。</exception>
        ''' <remarks>在对未指定打开方式的 <see cref="IOContext" /> 进行匹配时，可能会修改 <paramref name="Arguments" /> 中项目的 <see cref="DocumentOpenArguments.IOContext" /> 属性。</remarks>
        Public Function OpenDocument(ByVal Arguments As IEnumerable(Of DocumentOpenArguments)) As IList(Of Document)
            Return OpenDocument(Arguments, False)
        End Function

        ''' <summary>
        ''' 使用指定的参数打开一个或数个文档，并在必要时显示“选择文档类型”对话框。
        ''' </summary>
        ''' <param name="Arguments">为打开文档操作提供参数，若为 <c>null</c>，则表示不打开任何文档。</param>
        ''' <param name="AlwaysShowDialog">指定是否总是显示对话框。</param>
        ''' <remarks>在对未指定打开方式的 <see cref="IOContext" /> 进行匹配时，可能会修改 <paramref name="Arguments" /> 中项目的 <see cref="DocumentOpenArguments.IOContext" /> 属性。</remarks>
        Public Function OpenDocument(ByVal Arguments As IEnumerable(Of DocumentOpenArguments), ByVal AlwaysShowDialog As Boolean) As IList(Of Document)
            Return OpenDocumentCore(Arguments, AlwaysShowDialog)
        End Function

        ''' <summary>
        ''' 使用指定的参数打开一个文档，并在必要时显示“选择文档类型”对话框。
        ''' </summary>
        ''' <param name="Arguments">为打开文档操作提供参数。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Argument" /> 为 <c>null</c>。</exception>
        ''' <remarks>在对未指定打开方式的 <see cref="IOContext" /> 进行匹配时，可能会修改 <paramref name="Arguments" /> 中项目的 <see cref="DocumentOpenArguments.IOContext" /> 属性。</remarks>
        Public Function OpenDocument(ByVal Arguments As DocumentOpenArguments) As Document
            Return OpenDocument(Arguments, False)
        End Function

        ''' <summary>
        ''' 使用指定的参数打开一个文档，并在必要时显示“选择文档类型”对话框。
        ''' </summary>
        ''' <param name="Arguments">为打开文档操作提供参数。</param>
        ''' <param name="AlwaysShowDialog">指定是否不论<see cref="IOContext" /> 的打开方式是否已指定，都显示“选择文档类型”对话框。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Argument" /> 为 <c>null</c>。</exception>
        ''' <remarks>在对未指定打开方式的 <see cref="IOContext" /> 进行匹配时，可能会修改 <paramref name="Arguments" /> 中项目的 <see cref="DocumentOpenArguments.IOContext" /> 属性。</remarks>
        Public Function OpenDocument(ByVal Arguments As DocumentOpenArguments, ByVal AlwaysShowDialog As Boolean) As Document
            Return OpenDocument(New DocumentOpenArguments() {Arguments}, AlwaysShowDialog).FirstOrDefault
        End Function

        ''' <summary>
        ''' 当需要显示“打开”对话框并打开一个或多个文件时被调用。
        ''' </summary>
        ''' <param name="SingleSelection">指定是否禁止多选。</param>
        ''' <returns>从文件打开的文档；如果用户取消，则返回空数组。</returns>
        Protected Overridable Function OpenDocumentCore(ByVal SingleSelection As Boolean) As IList(Of Document)
            Dim OFD As New OpenFileDialog
            Dim FileTypes As New List(Of FileType)  '包含了当前管理器中所有文档类型使用的文件类型
            Dim DocumentTypes As New List(Of KeyValuePair(Of Type, DocumentTypeInfo))  '与 FileTypes 保持一对一关系的文档类型
            Dim Filter As String
            For Each EachTypeInfo In m_DocumentTypes
                With DocumentInformation.GetInfo(EachTypeInfo.Key)
                    FileTypes.AddRange(.FileTypes)
                    For I = 1 To .FileTypes.Count
                        DocumentTypes.Add(EachTypeInfo)
                    Next
                End With
            Next
            If FileTypes.Count > 1 Then
                Dim MergedTypes = FileTypes.Join        '用来保存当前管理器能处理的所有文件类型（所有支持的文档）
                MergedTypes.Description = Prompts.AllSupportedDocuments
                FileTypes.Add(MergedTypes)
            End If
            FileTypes.Add(New FileType(Prompts.AllFiles))
            Filter = FileTypes.ToFilterDescription
            OFD.Filter = Filter
            OFD.FilterIndex = FileTypes.Count - 1   '所有支持的类型
            OFD.Multiselect = Not SingleSelection
            If OFD.ShowDialog() = DialogResult.OK Then
                '将文件名转换为 IO 上下文
                Dim OpenArg As New List(Of DocumentOpenArguments)
                'FilterIndex 以 1 为下限
                Dim DocTypeInfo = If(OFD.FilterIndex <= DocumentTypes.Count, DocumentTypes(OFD.FilterIndex - 1), Nothing)
                For Each EachFileName In OFD.FileNames
                    '生成参数
                    OpenArg.Add(New DocumentOpenArguments(New FileIOContext(EachFileName), If(OFD.ReadOnlyChecked, DocumentCreationModes.ReadOnly, DocumentCreationModes.Normal), DocTypeInfo.Key))
                Next
                '打开文件
                Return OpenDocumentCore(OpenArg, False)
            Else
                Return EmptyDocumentList
            End If
        End Function

        ''' <summary>
        ''' 当需要使用指定的参数打开一个或数个文档时被调用。
        ''' </summary>
        ''' <param name="Arguments">为打开文档操作提供参数，若为 <c>null</c>，则表示不打开任何文档。</param>
        ''' <param name="AlwaysShowDialog">指定是否总是显示对话框。</param>
        ''' <remarks>在对未指定打开方式的 <see cref="IOContext" /> 进行匹配时，可能会修改 <paramref name="Arguments" /> 中项目的 <see cref="DocumentOpenArguments.IOContext" /> 属性。</remarks>
        Protected Overridable Function OpenDocumentCore(ByVal Arguments As IEnumerable(Of DocumentOpenArguments), ByVal AlwaysShowDialog As Boolean) As IList(Of Document)
            If Arguments Is Nothing Then
                Return EmptyDocumentList
            Else
                '筛选不为 Nothing 的项目
                Arguments = Arguments.Where(Function(EachArg) EachArg IsNot Nothing)
                If Arguments.Any Then
                    Dim bShowDialog As Boolean = AlwaysShowDialog
                    For Each EachArgument In Arguments
                        If EachArgument.OpenWith Is Nothing Then
                            '自动匹配文档
                            EachArgument.OpenWith = MatchBestDocumentType(EachArgument.IOContext)
                            '检查是否需要显示对话框
                            If EachArgument.OpenWith Is Nothing Then bShowDialog = True
                        End If
                    Next
                    If bShowDialog Then
                        Using Selector As New DocumentTypeSelector
                            Selector.DocumentTypes = m_DocumentTypes.Keys
                            Selector.OpenArguments = Arguments
                            If Selector.ShowDialog() = DialogResult.Cancel Then
                                Return EmptyDocumentList
                            End If
                        End Using
                    End If
                    Dim DocList As New List(Of Document)
                    Dim ArgumentsCount As Integer = Arguments.Count
                    Dim CurrentIndex As Integer = -1
                    Dim EventArgs As DocumentOpenEventArgs
                    For Each EachArgument In Arguments
                        CurrentIndex += 1
BeginOpen:
                        '引发事件
                        EventArgs = New DocumentOpeningEventArgs(EachArgument, CurrentIndex, ArgumentsCount, DocumentProcessAction.Passthrough)
                        OnDocumentOpening(DirectCast(EventArgs, DocumentOpeningEventArgs))
                        Select Case EventArgs.Action
                            Case DocumentProcessAction.Passthrough
                            Case DocumentProcessAction.CancelCurrent
                                Continue For
                            Case DocumentProcessAction.CancelAll
                                Exit For
                        End Select
                        '打开文档
                        If EachArgument.OpenWith IsNot Nothing Then
                            Dim NewDoc As Document = Nothing
                            Try
                                NewDoc = OpenDocumentCore(EachArgument)
                                Debug.Assert(NewDoc IsNot Nothing)
                                DocList.Add(NewDoc)
                            Catch ex As Exception
                                '引发事件
                                EventArgs = New OpenDocumentErrorEventArgs(EachArgument, ex, CurrentIndex, ArgumentsCount, DocumentProcessAction.CancelCurrent)
                                OnOpenDocumentError(DirectCast(EventArgs, OpenDocumentErrorEventArgs))
                                Select Case EventArgs.Action
                                    Case DocumentProcessAction.Passthrough
                                        GoTo BeginOpen
                                    Case DocumentProcessAction.CancelCurrent
                                        Continue For
                                    Case DocumentProcessAction.CancelAll
                                        Exit For
                                End Select
                            End Try
                            '引发事件
                            EventArgs = New DocumentOpenedEventArgs(NewDoc, CurrentIndex, ArgumentsCount)
                            OnDocumentOpened(DirectCast(EventArgs, DocumentOpenedEventArgs))
                            If EventArgs.Action = DocumentProcessAction.CancelAll Then Exit For
                        End If
                    Next
                    Return DocList.AsReadOnly
                Else
                    Return EmptyDocumentList
                End If
            End If
        End Function

        ''' <summary>
        ''' 当需要根据指定参数打开一个文档时被调用。
        ''' </summary>
        ''' <param name="Arguments">打开文档的参数，如果为 <c>null</c>，则表示不打开任何文档。</param>
        ''' <returns>被打开的文档实例，如果 <paramref name="Arguments" /> 为 <c>null</c>，则返回 <c>null</c>。</returns>
        ''' <remarks>此方法不能等效于可接受 <see cref="IEnumerable(Of DocumentOpenArguments)" /> 参数的 <see cref="OpenDocumentCore" />，此方法只负责打开文档，而后者还参与各种事件的引发处理。</remarks>
        Protected Overridable Function OpenDocumentCore(ByVal Arguments As DocumentOpenArguments) As Document
            If Arguments Is Nothing Then
                Return Nothing
            Else
                Dim NewDoc = CreateDocument(Arguments.OpenWith, Arguments)
                CreateView(NewDoc)  '为新文档创建视图

                _AddDocument(NewDoc)
                Return NewDoc
            End If
        End Function

        ''' <summary>
        ''' 获取当前支持的文档类型列表。
        ''' </summary>
        Public ReadOnly Property DocumentTypes() As IEnumerable(Of Type)
            Get
                Return m_DocumentTypes.Keys
            End Get
        End Property

        ''' <summary>
        ''' 获取当前已经打开的文档列表。
        ''' </summary>
        Public ReadOnly Property Documents() As IEnumerable(Of Document)
            Get
                Return m_Documents
            End Get
        End Property

        ''' <summary>
        ''' 构造一个空的文档实例，但不添加到文档列表中。
        ''' </summary>
        ''' <param name="documentType">指定文档的类型。</param>
        ''' <param name="creationArguments">指定创建文档的参数。</param>
        ''' <returns>新的文档实例。</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
        ''' <exception cref="ArgumentOutOfRangeException">指定的 <paramref name="documentType" /> 没有继承自 <see cref="Document" />。</exception>
        Protected Function CreateDocument(ByVal documentType As Type, ByVal creationArguments As DocumentCreationArguments) As Document
            Dim NewInstance = Document.CreateDocument(documentType, creationArguments)
            OnDocumentCreated(NewInstance)
            Return NewInstance
        End Function

        '如果是给定一个视图类型，想要得到它的实例，可以去找 Activator ……

        ''' <summary>
        ''' 为指定的文档类型构造一个新的视图实例。
        ''' </summary>
        ''' <param name="documentType">指定文档的类型。</param>
        ''' <returns>新的视图实例。如果指定的文档没有默认视图类型，则返回 <c>null</c>。</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
        Public Function CreateView(ByVal documentType As Type) As IDocumentView
            If documentType Is Nothing Then
                Throw New ArgumentNullException("documentType")
            Else
                With DocumentInformation.GetInfo(documentType)
                    If .ViewType IsNot Nothing Then
                        Dim NewView = DirectCast(Activator.CreateInstance(.ViewType), IDocumentView)
                        OnViewCreated(NewView)
                        Return NewView
                    Else
                        Return Nothing
                    End If
                End With
            End If
        End Function

        ''' <summary>
        ''' 为指定文档构造一个新的视图实例，并将其加入文档列表。
        ''' </summary>
        ''' <param name="document">指定要新建视图的文档。</param>
        ''' <returns>新的视图实例。如果指定的文档没有默认视图类型，则返回 <c>null</c>。</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="document" /> 为 <c>null</c>。</exception>
        Public Function CreateView(ByVal document As Document) As IDocumentView
            If document Is Nothing Then
                Throw New ArgumentNullException("document")
            Else
                Dim NewView = CreateView(document.GetType)
                If NewView IsNot Nothing Then document.AddView(NewView)
                Return NewView
            End If
        End Function

        ''' <summary>
        ''' 遍历文档模版列表，并挑选出最佳模版。
        ''' </summary>
        ''' <param name="IOContext">要进行匹配的文档 IO 上下文。</param>
        ''' <returns>如果找到一个或多个匹配结果大于 <see cref="DocumentMatchResult.MayUnmatch" /> 的文档类型，则选取第一个结果最接近的文档类型。否则返回 <c>null</c>。</returns>
        Private Function MatchBestDocumentType(ByVal IOContext As IOContext) As Type
            Dim BestDocumentType As Type = Nothing
            Dim BestMatch As DocumentMatchResult = DocumentMatchResult.SureUnmatch
            For Each EachTypeInfo In m_DocumentTypes
                Dim LastMatch As DocumentMatchResult = DocumentInformation.MatchDocument(EachTypeInfo.Key, IOContext)
                If LastMatch > BestMatch Then
                    BestMatch = LastMatch
                    BestDocumentType = EachTypeInfo.Key
                End If
                If BestMatch = DocumentMatchResult.SureMatch Then Exit For
            Next
            If BestMatch <= DocumentMatchResult.MayUnmatch Then
                Return Nothing
            Else
                Return BestDocumentType
            End If
        End Function

        ''' <summary>
        ''' 遍历文档模版列表，并返回匹配结果
        ''' </summary>
        ''' <param name="IOContext">要进行匹配的文档 IO 上下文。</param>
        Private Function MatchAllDocumentTypes(ByVal IOContext As IOContext) As Dictionary(Of Type, DocumentMatchResult)
            Dim MatchResult As New Dictionary(Of Type, DocumentMatchResult)
            For Each EachTypeInfo In m_DocumentTypes    '遍历文档模版列表  
                MatchResult.Add(EachTypeInfo.Key, DocumentInformation.MatchDocument(EachTypeInfo.Key, IOContext))
            Next
            Return MatchResult
        End Function

        ''' <summary>
        ''' 引发 <see cref="DocumentCreated"/> 事件。
        ''' </summary>
        Protected Overridable Sub OnDocumentCreated(ByVal Document As Document)
            RaiseEvent DocumentCreated(Me, New DocumentEventArgs(Document))
        End Sub

        ''' <summary>
        ''' 引发 <see cref="ViewCreated"/> 事件。
        ''' </summary>
        Protected Overridable Sub OnViewCreated(ByVal View As IDocumentView)
            RaiseEvent ViewCreated(Me, New ViewEventArgs(View))
        End Sub

        ''' <summary>
        ''' 引发 <see cref="DocumentOpening" /> 事件。
        ''' </summary>
        Protected Overridable Sub OnDocumentOpening(ByVal e As DocumentOpeningEventArgs)
            RaiseEvent DocumentOpening(Me, e)
        End Sub

        ''' <summary>
        ''' 引发 <see cref="DocumentOpened" /> 事件。
        ''' </summary>
        Protected Overridable Sub OnDocumentOpened(ByVal e As DocumentOpenedEventArgs)
            RaiseEvent DocumentOpened(Me, e)
        End Sub

        ''' <summary>
        ''' 引发 <see cref="OpenDocumentError" /> 事件。
        ''' </summary>
        Protected Overridable Sub OnOpenDocumentError(ByVal e As OpenDocumentErrorEventArgs)
            ReportOpenDocumentError(e)
            RaiseEvent OpenDocumentError(Me, e)
        End Sub

        ''' <summary>
        ''' 当向用户报告打开文档的错误时引发此事件。
        ''' </summary>
        ''' <remarks>重写时，可以在此处使用自己的错误报告及处理方式来取代默认实现。</remarks>
        Protected Overridable Sub ReportOpenDocumentError(ByVal e As OpenDocumentErrorEventArgs)
            Using ErrDlg As New OpenDocumentErrorDialog With {.Arguments = e.Arguments, .Exception = e.Exception}
                Select Case ErrDlg.ShowDialog
                    Case DialogResult.Retry
                        e.Action = DocumentProcessAction.Passthrough
                    Case DialogResult.Ignore
                        e.Action = DocumentProcessAction.CancelCurrent
                    Case DialogResult.Cancel
                        e.Action = DocumentProcessAction.CancelAll
                End Select
            End Using
        End Sub

        Private Sub ChildDocument_Closed(ByVal sender As Object, ByVal e As EventArgs)    'Handles [By AddHandle] 
            RemoveDocument(DirectCast(sender, Document))     '从列表中移除已经关闭的文档
        End Sub
    End Class
End Namespace