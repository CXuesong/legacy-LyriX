Imports System.ComponentModel

Namespace Document.ObjectModel
    ''' <summary>
    ''' 表示一个包含本地化信息的 <see cref="DataContainer" />。
    ''' </summary>
    ''' <remarks>在对一个 <see cref="DataContainer" /> 进行本地化时，未本地化的对象是本地化对象的“源”。</remarks>
    Public Interface ILocalizedDataContainer

        ''' <summary>
        ''' 获取/设置本地化源的标识符。
        ''' </summary>
        Property SourceId As Integer?

        ''' <summary>
        ''' 获取包含此对象对应的未本地化的 <see cref="DataContainer" />。
        ''' </summary>
        ''' <value>此对象本地化信息的源，如果返回值为 <c>null</c>，则表示此对象没有对应的待本地化的源。</value>
        ReadOnly Property Source As DataContainer

        ''' <summary>
        ''' 从源同步本地化信息的子项。
        ''' </summary>
        ''' <param name="mode">同步模式。</param>
        ''' <exception cref="InvalidOperationException">进行同步时，<see cref="Source" /> 为 <c>null</c>。</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="mode" /> 不是有效的 <see cref="ChildrenSynchronizationMode" />。</exception>
        Sub SynchronizeChildren(mode As ChildrenSynchronizationMode)
    End Interface

    ''' <summary>
    ''' 表示在对不同 <see cref="DataContainer" /> 的子项进行同步时要进行的操作。
    ''' </summary>
    Public Enum ChildrenSynchronizationMode
        ''' <summary>
        ''' 不进行任何操作。
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 将子项列表中不存在，而源列表中存在的项目添加到此列表中。
        ''' </summary>
        AddNew = 1
        ''' <summary>
        ''' 将子项列表中指向源列表中不存在的项目的子项移除。
        ''' </summary>
        RemoveInvalid = 2
        ''' <summary>
        ''' 进行添加与移除工作。
        ''' </summary>
        All = AddNew Or RemoveInvalid
    End Enum
End Namespace
