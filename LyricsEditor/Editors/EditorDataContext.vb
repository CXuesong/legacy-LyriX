''' <summary>
''' 为文档编辑器提供的数据上下文。
''' </summary>
Public Class EditorDataContext
    Private m_Document As LyriXPackageDocument
    Private m_Container As Document.ObjectModel.DataContainer

    ''' <summary>
    ''' 获取当前正在编辑的文档。
    ''' </summary>
    Public ReadOnly Property Document As LyriXPackageDocument
        Get
            Return m_Document
        End Get
    End Property

    ''' <summary>
    ''' 获取当前正在编辑的文档的部分。
    ''' </summary>
    Public ReadOnly Property Container As Document.ObjectModel.DataContainer
        Get
            Return m_Container
        End Get
    End Property

    Public Sub New(document As LyriXPackageDocument, Container As Document.ObjectModel.DataContainer)
        m_Document = document
        m_Container = Container
    End Sub

    Public Sub New(document As LyriXPackageDocument)
        Me.New(document, Nothing)
    End Sub
End Class
