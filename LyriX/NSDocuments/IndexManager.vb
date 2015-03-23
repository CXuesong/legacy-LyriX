Option Compare Text

Namespace Document
    ''' <summary>
    ''' 用于管理文档中的索引。
    ''' </summary>
    Public Class IndexManager
        Private m_CurrentIndex As Integer
        Private m_MinimumIndex As Integer
        Private m_MaximumIndex As Integer
        Private WithEvents m_Owner As ObjectModel.DataContainer
        Private m_IndexedType As Type

        ''' <summary>
        ''' 获取一个新的索引号。
        ''' </summary>
        ''' <exception cref="InvalidOperationException">索引号已经全部分配完毕。</exception>
        Public Function NewIndex() As Integer
            If m_CurrentIndex >= m_MaximumIndex Then
                Throw New InvalidOperationException(ExceptionPrompts.NoIndexAvaliable)
            Else
                m_CurrentIndex += 1
                Return m_CurrentIndex
            End If
        End Function

        ''' <summary>
        ''' 获取分配索引号的最小值。
        ''' </summary>
        Public ReadOnly Property MinimumIndex As Integer
            Get
                Return m_MinimumIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取分配索引号的最大值。
        ''' </summary>
        Public ReadOnly Property MaximumIndex As Integer
            Get
                Return m_MaximumIndex
            End Get
        End Property

        ''' <summary>
        ''' 获取此索引管理器的所有者。
        ''' </summary>
        Public ReadOnly Property Owner As ObjectModel.DataContainer
            Get
                Return m_Owner
            End Get
        End Property

        ''' <summary>
        ''' 获取此索引管理器分配索引的目标对象类型。
        ''' </summary>
        Public ReadOnly Property IndexedType As Type
            Get
                Return m_IndexedType
            End Get
        End Property

        ''' <summary>
        ''' 声明指定的索引已经被占据。
        ''' </summary>
        ''' <param name="index">已经被占据的索引。</param>
        Public Sub OccupyIndex(index As Integer)
            'Debug.Print("{0} Occupy {1}", Owner, index)
            If index > m_CurrentIndex Then
                m_CurrentIndex = index + 1
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0:d} - {1:d}, {2:d}", m_MinimumIndex, m_MaximumIndex, m_CurrentIndex)
        End Function

        ''' <summary>
        ''' 使用指定的上限与下限初始化一个索引管理器。
        ''' </summary>
        ''' <exception cref="ArgumentException"><paramref name="minimumIndex" /> 大于 <paramref name="maximumIndex" />。</exception>
        ''' <exception cref="ArgumentNullException"><paramref name="owner" /> 或 <paramref name="indexedType" /> 为 <c>null</c>。</exception>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="indexedType" /> 指定的类型没有实现 <see cref="ObjectModel.IIdentifiable" /> 接口。</exception>
        Public Sub New(owner As ObjectModel.DataContainer, indexedType As Type, minimumIndex As Integer, maximumIndex As Integer)
            If owner Is Nothing Then
                Throw New ArgumentNullException("owner")
            ElseIf indexedType Is Nothing Then
                Throw New ArgumentNullException("indexedType")
            ElseIf Not GetType(ObjectModel.IIdentifiable).IsAssignableFrom(indexedType) Then
                Throw New ArgumentOutOfRangeException(String.Format(ExceptionPrompts.InterfaceUnimplemented, GetType(ObjectModel.IIdentifiable)))
            ElseIf minimumIndex > maximumIndex Then
                Throw New ArgumentException
            Else
                m_Owner = owner
                m_IndexedType = indexedType
                m_MinimumIndex = minimumIndex
                m_MaximumIndex = maximumIndex
                m_CurrentIndex = minimumIndex
            End If
        End Sub

        ''' <summary>
        ''' 初始化一个非负的索引管理器。
        ''' </summary>
        ''' <exception cref="ArgumentException"><paramref name="minimumIndex" /> 大于 <paramref name="maximumIndex" />。</exception>
        Public Sub New(owner As ObjectModel.DataContainer, indexedType As Type)
            Me.New(owner, indexedType, 0, Integer.MaxValue)
        End Sub

        '登记此项及其子项占用的索引
        Private Sub OccupyDescendent(container As ObjectModel.DataContainer)
            Debug.Assert(container IsNot Nothing)
            If IndexedType.IsAssignableFrom(container.GetType) Then
                Dim Id = DirectCast(container, ObjectModel.IIdentifiable).Id
                If Id IsNot Nothing Then OccupyIndex(Id.Value)
            End If
            For Each EachItem In container.Children
                OccupyDescendent(EachItem)
            Next
        End Sub

        Private Sub m_Owner_ContainerDataChanged(sender As Object, e As ObjectModel.ContainerDataChangedEventArgs) Handles m_Owner.ContainerDataChanged
            '登记索引
            Debug.Assert(e.Source IsNot Nothing)
            If e.PropertyName = "Parent" Then
                OccupyDescendent(e.Source)
            End If
        End Sub
    End Class
End Namespace