Option Compare Text
Imports DocumentViewModel
Imports LyriX.Document
Imports System.ComponentModel

<DocumentInfo(GetType(LyriXPackageDocument), "1.0", ViewType:=GetType(LyriXPackageView))>
Public NotInheritable Class LyriXPackageDocument
    Inherits DocumentViewModel.Document

    ''' <summary>
    ''' 当 <see cref="Package" /> 或其子项的属性值发生变化时引发。
    ''' </summary>
    Public Event PackageDataChanged As Document.ObjectModel.DataContainer.ContainerDataChangedEventHandler

    Private WithEvents m_Package As LyriXPackage

    Public ReadOnly Property Package As LyriXPackage
        Get
            Return m_Package
        End Get
    End Property

    Protected Overrides Sub ReadDocumentStream(dest As System.IO.Stream)
        m_Package = New LyriXPackage(dest)
        SetTag(m_Package)
    End Sub

    Protected Overrides Sub WriteDocumentStream(dest As System.IO.Stream)
        m_Package.SavePackage(dest)
    End Sub

    Public Overrides Sub ClearContent()
        m_Package = New LyriXPackage
        '设置初始值
        SetTag(m_Package)
        m_Package.Header.ApplicationName = My.Application.Info.Title
        m_Package.Header.ApplicationVersion = My.Application.Info.Version.ToString
        IsModified = False
    End Sub

    Protected Overrides Sub OnModifiedChanged(e As System.EventArgs)
        If Me.IsModified = False Then
            '清除修改标记
            ClearModifiedTag(m_Package)
        End If
        MyBase.OnModifiedChanged(e)
    End Sub

    Protected Overrides Sub OnIOContextChanged(e As System.EventArgs)
        If Me.IOContext IsNot Nothing Then Me.Title = Me.IOContext.Name
    End Sub

    '方向：隧道（从上到下）
    Private Shared Sub ClearModifiedTag(container As ObjectModel.DataContainer)
        Debug.Assert(container IsNot Nothing)
        Debug.Assert(TypeOf container.Tag Is LyriXDataContainerTag)
        DirectCast(container.Tag, LyriXDataContainerTag).IsModified = False
        For Each EachItem In container.Children
            ClearModifiedTag(EachItem)
        Next
    End Sub

    '方向：冒泡（从下到上）
    Private Shared Sub SetModifiedTag(container As ObjectModel.DataContainer)
        Debug.Assert(container IsNot Nothing)
        Debug.Assert(TypeOf container.Tag Is LyriXDataContainerTag)
        DirectCast(container.Tag, LyriXDataContainerTag).IsModified = True
        If container.Parent IsNot Nothing Then SetModifiedTag(container.Parent)
    End Sub

    ''' <summary>
    ''' 为已存在的对象设置标记（如果需要的话）。
    ''' </summary>
    Private Shared Sub SetTag(container As ObjectModel.DataContainer, Optional withChildren As Boolean = True)
        Debug.Assert(container IsNot Nothing)
        If container.Tag Is Nothing Then
            container.Tag = New LyriXDataContainerTag(container)
        End If
        If withChildren Then
            For Each EachItem In container.Children
                SetTag(EachItem, True)
            Next
        End If
    End Sub

    '收集通过冒泡传到 Package 级别的更改事件
    Private Sub m_Package_ContainerDataChanged(sender As Object, e As Document.ObjectModel.ContainerDataChangedEventArgs) Handles m_Package.ContainerDataChanged
        Select Case e.PropertyName
            Case "Parent"
                '为加入列表的新项目设置标记
                SetTag(e.Source, True)
                If e.Source.Parent IsNot Nothing Then SetModifiedTag(e.Source.Parent)
                SetModified()
            Case "Tag"
                'Do NOTING
            Case Else
                SetModifiedTag(e.Source)
                SetModified()
        End Select
        RaiseEvent PackageDataChanged(sender, e)
    End Sub
End Class

''' <summary>
''' 用于设置 <see cref="Document.ObjectModel.DataContainer.Tag" />，保存适用于此程序的一些临时数据。
''' </summary>
Friend NotInheritable Class LyriXDataContainerTag
    Implements INotifyPropertyChanged

    ''' <summary>
    ''' 在更改属性值时发生。
    ''' </summary>
    Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Private m_IsModified As Boolean
    Private m_Owner As Document.ObjectModel.DataContainer

    Public Property IsModified As Boolean
        Get
            Return m_IsModified
        End Get
        Set(value As Boolean)
            m_IsModified = value
            OnPropertyChanged("IsModified")
        End Set
    End Property

    Public ReadOnly Property Owner As Document.ObjectModel.DataContainer
        Get
            Return m_Owner
        End Get
    End Property

    '绑定辅助
    Public Property Text As String
        Get
            If TypeOf m_Owner Is Span Then
                Return GetSpanText(DirectCast(m_Owner, Span))
            Else
                Return m_Owner.ToString
            End If
        End Get
        Set(value As String)
            Debug.Assert(TypeOf m_Owner Is Span)
            'If TypeOf m_Owner Is Span Then
            '自动划分语义段
            With DirectCast(m_Owner, Span).Segments
                .Clear()
                For Each EachSegment In DocumentUtility.SplitSegments(value)
                    .Add(EachSegment)
                Next
            End With
            'End If
        End Set
    End Property

    ''' <summary>
    ''' 引发 <see cref="PropertyChanged" />。
    ''' </summary>
    ''' <param name="propertyName">发生变化的属性名称，若为 <see cref="String.Empty" /> 或为 <c>null</c>，则表示该对象上的所有属性都已更改。</param>
    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Sub New(owner As Document.ObjectModel.DataContainer)
        Debug.Assert(owner IsNot Nothing)
        m_Owner = owner
        If TypeOf owner Is Document.Span Then
            AddHandler owner.ContainerDataChanged, AddressOf SpanOwner_ContainerDataChanged
        End If
    End Sub

    Private Sub SpanOwner_ContainerDataChanged(sender As Object, e As Document.ObjectModel.ContainerDataChangedEventArgs)
        If TypeOf e.Source Is Document.Segment AndAlso String.Equals(e.PropertyName, "Text", StringComparison.OrdinalIgnoreCase) OrElse
            e.Source Is Me AndAlso String.Equals(e.PropertyName, "Children", StringComparison.OrdinalIgnoreCase) Then
            OnPropertyChanged("Text")
        End If
    End Sub
End Class