Namespace Compiled
    Namespace ObjectModel
        ''' <summary>
        ''' 表示一个经过编译的 <see cref="Document.ObjectModel.DataContainer" />。
        ''' </summary>
        Public MustInherit Class CompiledContainer
            Private m_Parent As CompiledContainer
            Private m_CompilationFinished As Boolean

            ''' <summary>
            ''' 获取此对象在逻辑上的父级。
            ''' </summary>
            Public ReadOnly Property Parent As CompiledContainer
                Get
                    Return m_Parent
                End Get
            End Property

            ''' <summary>
            ''' 通知此实例编译信息已经接收完毕。
            ''' </summary>
            Friend Sub FinishCompilation()
                OnCompilationFinished()
            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <remarks>默认实现：确保此方法只被调用一次。</remarks>
            Protected Overridable Sub OnCompilationFinished()
                If m_CompilationFinished Then
                    Throw New InvalidOperationException
                End If
                m_CompilationFinished = True
            End Sub

            ''' <summary>
            ''' 初始化一个带父级的 <see cref="CompiledContainer" />。
            ''' </summary>
            ''' <param name="parent">此对象在逻辑上的父级，可以为 <c>null</c>。</param>
            Public Sub New(parent As CompiledContainer)
                m_Parent = parent
            End Sub
        End Class

        ''' <summary>
        ''' 表示一个经过编译的文档部分。
        ''' </summary>
        Public MustInherit Class CompiledDocumentPart
            Inherits CompiledContainer

            Private m_Document As LyricsDocument

            Public ReadOnly Property Document As LyricsDocument
                Get
                    Return m_Document
                End Get
            End Property

            ''' <summary>
            ''' 初始化一个 <see cref="CompiledDocumentPart" />。
            ''' </summary>
            ''' <param name="document">此对象所属的文档。</param>
            ''' <exception cref="ArgumentNullException"><paramref name="document" /> 为 <c>null</c>。</exception>
            Public Sub New(document As LyricsDocument)
                MyBase.New(Nothing)
                If document Is Nothing Then
                    Throw New ArgumentNullException("document")
                Else
                    m_Document = document
                End If
            End Sub
        End Class

        ''' <summary>
        ''' 表示一个经过编译的文档部分的某个元素。
        ''' </summary>
        Public MustInherit Class CompiledElement
            Inherits CompiledContainer

            ''' <summary>
            ''' 初始化一个带父级的 <see cref="CompiledElement" />。
            ''' </summary>
            ''' <param name="parent">此对象在逻辑上的父级。</param>
            ''' <exception cref="ArgumentNullException"><paramref name="parent" /> 为 <c>null</c>。</exception>
            Public Sub New(parent As CompiledContainer)
                MyBase.New(parent)
                If parent Is Nothing Then
                    Throw New ArgumentNullException("parent")
                End If
            End Sub
        End Class

        ''' <summary>
        ''' 表示经过编译的元素具有一个在某一范围内唯一的标识符。
        ''' </summary>
        Friend Interface IIdentifiable
            ''' <summary>
            ''' 此元素的标识符。
            ''' </summary>
            Property Id As Integer?
        End Interface

        ''' <summary>
        ''' 表示经过编译的元素在编译时可将同类其他元素的特性作为此元素的默认值。
        ''' </summary>
        Friend Interface IReferable(Of T As CompiledContainer)
            Inherits IIdentifiable

            ''' <summary>
            ''' 确定此对象的引用目标。指定目标的信息将作为此元素内容的默认值。
            ''' </summary>
            ReadOnly Property Reference As T

            ' ''' <summary>
            ' ''' 将引用目标的特性应用于此元素。
            ' ''' </summary>
            ' ''' <exception cref="InvalidOperationException">在此次调用前已经调用了一次参数不为 <c>null</c> 的 <see cref="ApplyReference" />。</exception>
            'Sub ApplyReference(reference As T)
        End Interface
    End Namespace
End Namespace