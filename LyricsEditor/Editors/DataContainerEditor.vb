'WARNING DependencyProperty 貌似不能够很好地支持泛型，因此需把一个类拆开……

''' <summary>
''' 为 DataContainer 的编辑器提供基类。
''' </summary>
Public MustInherit Class DataContainerEditorBase
    Inherits Page

    Private Shared ReadOnly UpdateTitleCallback As PropertyChangedCallback =
        Sub(d, e) DirectCast(d, DataContainerEditorBase).UpdateTitle()

    ''' <summary>
    ''' 标识 <see cref="EditorTitle" /> 依赖项属性。
    ''' </summary>
    Public Shared ReadOnly EditorTitleProperty As DependencyProperty =
        DependencyProperty.Register("EditorTitle", GetType(String), GetType(DataContainerEditorBase),
                                    New FrameworkPropertyMetadata(UpdateTitleCallback))

    ''' <summary>
    ''' 标识 <see cref="DataContainerTitle" /> 依赖项属性。
    ''' </summary>
    Public Shared ReadOnly DataContainerTitleProperty As DependencyProperty =
        DependencyProperty.Register("DataContainerTitle", GetType(String), GetType(DataContainerEditorBase),
                                    New FrameworkPropertyMetadata(UpdateTitleCallback))

    ''' <summary>
    ''' 当 <see cref="DataSource" /> 或其子项的属性值发生变化时引发。
    ''' </summary>
    Public Event ContainerDataChanged As Document.ObjectModel.DataContainer.ContainerDataChangedEventHandler

    Private WithEvents m_dataSource As Document.ObjectModel.DataContainer   '仅用于监听事件

    ''' <summary>
    ''' 打开指定项目的编辑页面。
    ''' </summary>
    Protected Sub EditDataContainer(container As Document.ObjectModel.DataContainer, editorType As Type)
        Dim Owner = Me.FindAncestor(Of LyriXPackageView)()
        Debug.Assert(Owner IsNot Nothing)
        Owner.EditContainer(If(container, DataSource), editorType)
    End Sub

    ''' <summary>
    ''' 打开指定项目的编辑页面。
    ''' </summary>
    Protected Sub EditDataContainer(container As Document.ObjectModel.DataContainer, editor As DataContainerEditorBase)
        Dim Owner = Me.FindAncestor(Of LyriXPackageView)()
        Debug.Assert(Owner IsNot Nothing)
        Owner.EditContainer(If(container, DataSource), editor)
    End Sub

    ''' <summary>
    ''' 打开指定项目的编辑页面。
    ''' </summary>
    Protected Sub EditDataContainer(container As Document.ObjectModel.DataContainer)
        EditDataContainer(container, DirectCast(Nothing, Type))
    End Sub

    ''' <summary>
    ''' 打开当前项目的编辑页面。
    ''' </summary>
    Protected Sub EditDataContainer(editorType As Type)
        EditDataContainer(Nothing, editorType)
    End Sub

    ''' <summary>
    ''' 打开当前项目的编辑页面。
    ''' </summary>
    Protected Sub EditDataContainer(editor As DataContainerEditorBase)
        EditDataContainer(Nothing, editor)
    End Sub

    Protected Function GoBack() As Boolean
        If Me.NavigationService IsNot Nothing AndAlso Me.NavigationService.CanGoBack Then
            Me.NavigationService.GoBack()
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' 获取此编辑器的直接数据源。
    ''' </summary>
    Public ReadOnly Property DataSource As Document.ObjectModel.DataContainer
        Get
            Return DataContext.Container
        End Get
    End Property

    ''' <summary>
    ''' 获取此编辑器的数据上下文。
    ''' </summary>
    Public Shadows Property DataContext As EditorDataContext
        Get
            Return DirectCast(MyBase.DataContext, EditorDataContext)
        End Get
        Set(value As EditorDataContext)
            MyBase.DataContext = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置编辑器的标题。
    ''' </summary>
    Public Property EditorTitle As String
        Get
            Return CStr(Me.GetValue(EditorTitleProperty))
        End Get
        Set(value As String)
            Me.SetValue(EditorTitleProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置被编辑的对象的标题。
    ''' </summary>
    Public Property DataContainerTitle As String
        Get
            Return CStr(Me.GetValue(DataContainerTitleProperty))
        End Get
        Set(value As String)
            Me.SetValue(DataContainerTitleProperty, value)
        End Set
    End Property

    Protected Sub UpdateTitle()
        If DataContainerTitle <> Nothing AndAlso EditorTitle <> Nothing Then
            Me.Title = String.Format("{0} - {1}", LimitStringLength(DataContainerTitle, NAVIGATION_PROMPT_LIMIT), EditorTitle)
        Else
            Me.Title = EditorTitle
        End If
    End Sub

    Private Sub DataContainerEditorBase_DataContextChanged(sender As Object, e As System.Windows.DependencyPropertyChangedEventArgs) Handles Me.DataContextChanged
        '设置标题
        UpdateTitle()
        m_dataSource = DataSource
    End Sub

    Private Sub m_dataSource_ContainerDataChanged(sender As Object, e As Document.ObjectModel.ContainerDataChangedEventArgs) Handles m_dataSource.ContainerDataChanged
        RaiseEvent ContainerDataChanged(Me, e)
    End Sub

    Public Sub New()
        ''设置属性的默认值（支持 BindingGroup）
        'Me.BindingGroup = New BindingGroup
    End Sub
End Class

''' <summary>
''' 表示一个 DataContainer 的编辑器。
''' </summary>
Public Class DataContainerEditor(Of T As Document.ObjectModel.DataContainer)
    Inherits DataContainerEditorBase

    ''' <summary>
    ''' 获取此编辑器的直接数据源。
    ''' </summary>
    Public Shadows ReadOnly Property DataSource As T
        Get
            Return DirectCast(DataContext.Container, T)
        End Get
    End Property
End Class
