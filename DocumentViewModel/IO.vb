Option Compare Text
Imports System.IO

''' <summary>
''' 用于描述文档流的打开模式
''' </summary>
<Flags()> Public Enum StreamMode
    None = 0
    ''' <summary>
    ''' 表示流用于打开
    ''' </summary>
    ForOpen = 1
    ''' <summary>
    ''' 表示流用于保存，这将清除当前流中的任何内容
    ''' </summary>
    ForSave = 3
    ''' <summary>
    ''' 表示流以独占方式打开（具体实现依派生类而定）
    ''' </summary>
    Exclusive = 4
End Enum

''' <summary>
''' 描述文档输入/输出上下文的基类
''' </summary>
Public MustInherit Class IOContext
    Implements IEquatable(Of IOContext)

    ''' <summary>
    ''' 获取用于描述当前上下文的标题
    ''' </summary>
    Public MustOverride ReadOnly Property Name() As String

    ''' <summary>
    ''' 获取被支持的流模式。
    ''' </summary>
    Public MustOverride ReadOnly Property SupportedMode() As StreamMode

    ''' <summary>
    ''' 判断指定的流模式是否被支持。
    ''' </summary>
    Public Function IsModeSupported(ByVal testValue As StreamMode) As Boolean
        Return (testValue And SupportedMode) <> Nothing
    End Function
    ''' <summary>
    ''' 在当前的上下文中使用默认途径打开一个流。
    ''' </summary>
    ''' <exception cref="ArgumentOutOfRangeException">枚举值超出合法范围。</exception>
    ''' <remarks>此方法仅使用默认途径打开流。如果可以，您可以调用派生类的相关方法。同时，在使用完毕后需要手动调用 <see cref="Stream.Close" /> 以释放资源。</remarks>
    Public Function OpenStream(ByVal Mode As StreamMode) As Stream
        '对输入进行检查
        If IsModeSupported(Mode) = False Then Throw New ArgumentOutOfRangeException("Mode", Mode, "模式不支持。")
        Return OnOpenStream(Mode)
    End Function

    ''' <summary>
    ''' 待重写，在当前的 IO 上下文中使用默认途径打开一个流
    ''' </summary>
    ''' <remarks>此函数被调用时，调用方（此类）已经对参数进行了原则性的检查。</remarks>
    Protected MustOverride Function OnOpenStream(ByVal Mode As StreamMode) As Stream

    ''' <summary>
    ''' 返回当前 IO 上下文的描述。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return MyBase.ToString
    End Function

    Public Shared Operator =(ByVal x As IOContext, ByVal y As IOContext) As Boolean
        If x Is y Then
            Return True
        Else
            If x IsNot Nothing AndAlso y IsNot Nothing Then
                Return x.Equals(y)
            Else
                Return False
            End If
        End If
    End Operator

    Public Shared Operator <>(ByVal x As IOContext, ByVal y As IOContext) As Boolean
        Return Not x = y
    End Operator
    ''' <summary>
    ''' 返回一个值，表示此实例是否与指定的对象相等。
    ''' </summary>
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return TypeOf obj Is IOContext AndAlso Equals(DirectCast(obj, IOContext))
    End Function

    ''' <summary>
    ''' 返回一个值，表示此实例是否与指定的 <see cref="IOContext" /> 相等。
    ''' </summary>
    Public Overloads Function Equals(ByVal other As IOContext) As Boolean Implements System.IEquatable(Of IOContext).Equals
        Return EqualsCore(other)
    End Function

    ''' <summary>
    ''' 默认实现：比较此实例于指定实例的类型是否相同。待实现：比较内容并返回比较结果。
    ''' </summary>
    Protected Overridable Function EqualsCore(ByVal other As IOContext) As Boolean
        Return Me.GetType Is other.GetType
    End Function
End Class

''' <summary>
''' 实现了 <see cref="IOContext" />，用来表示文件 IO 上下文
''' </summary>
Public Class FileIOContext
    Inherits IOContext

    Private m_DestFile As FileInfo

    ''' <summary>
    ''' 获取目标文件
    ''' </summary>
    Public ReadOnly Property DestFile() As FileInfo
        Get
            Return m_DestFile
        End Get
    End Property

    ''' <summary>
    ''' 初始化一个 <see cref="FileIOContext" />。
    ''' </summary>
    ''' <param name="FileName">设置目标文件名，即使文件暂时不存在（用于保存），但是文件名必须合法。</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal FileName As String)
        m_DestFile = New FileInfo(FileName)
    End Sub

    Protected Overrides Function OnOpenStream(ByVal Mode As StreamMode) As System.IO.Stream
        Dim fShare As FileShare = If((Mode And StreamMode.Exclusive) = StreamMode.Exclusive, FileShare.None, FileShare.ReadWrite)
        '一定要先判断 ForSave(二进制：11)，再判断 ForOpen(二进制：01)
        If (Mode And StreamMode.ForSave) = StreamMode.ForSave Then
            '有些程序要求读/写权限
            Return m_DestFile.Open(FileMode.Create, FileAccess.ReadWrite, fShare)
        ElseIf (Mode And StreamMode.ForOpen) = StreamMode.ForOpen Then
            Return m_DestFile.Open(FileMode.Open, FileAccess.Read, fShare)
        Else
            Return Nothing
        End If
    End Function

    Protected Overrides Function EqualsCore(ByVal other As IOContext) As Boolean
        Return MyBase.EqualsCore(other) AndAlso m_DestFile.FullName = DirectCast(other, FileIOContext).m_DestFile.FullName
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return m_DestFile.Name
        End Get
    End Property

    Public Overrides ReadOnly Property SupportedMode() As StreamMode
        Get
            Return StreamMode.ForOpen Or StreamMode.ForSave Or StreamMode.Exclusive
        End Get
    End Property


    ''' <summary>
    ''' 获取当前 IO 上下文的描述，包括文件的路径。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return "文件：" & m_DestFile.FullName
    End Function
End Class

''' <summary>
'''  实现了 <see cref="IOContext" />，用来表示内存 IO 上下文。
''' </summary>
Public Class MemoryIOContext
    Inherits IOContext

    Private m_Stream As MemoryStream
    Private m_Name As String

    Public Overrides ReadOnly Property Name() As String
        Get
            Return m_Name
        End Get
    End Property

    ''' <summary>
    ''' 获取当前 IO 上下文内容的复制。
    ''' </summary>
    Public Function GetContent() As Byte()
        m_Stream.Flush()    '刷新数据（冲水）
        Return m_Stream.ToArray()
    End Function

    Protected Overrides Function OnOpenStream(ByVal Mode As StreamMode) As System.IO.Stream
        If (Mode And StreamMode.ForSave) = StreamMode.ForSave Then
            m_Stream.Close()
            m_Stream = New MemoryStream
            Return m_Stream
        ElseIf (Mode And StreamMode.ForOpen) = StreamMode.ForOpen Then
            Return New MemoryStream(m_Stream.GetBuffer, 0, CInt(m_Stream.Length), False)
        Else
            Return Nothing
        End If
    End Function

    Protected Overrides Function EqualsCore(ByVal other As IOContext) As Boolean
        '要求完全相等
        Return Me Is other
    End Function

    ''' <summary>
    ''' 初始化一个空白的 <see cref="MemoryIOContext" />。
    ''' </summary>
    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' 初始化一个具有指定内容的 <see cref="MemoryIOContext" />。
    ''' <param name="InitalContent">初始化 IO 上下文的内容，若为 <c>null</c>，则表示空。</param>
    ''' </summary>
    Public Sub New(ByVal InitalContent() As Byte)
        Me.New(InitalContent, Nothing)
    End Sub

    ''' <summary>
    ''' 初始化一个具有指定名称的 <see cref="MemoryIOContext" />。
    ''' <param name="InitalContent">初始化 IO 上下文的内容，若为 <c>null</c>，则表示空。</param>
    ''' </summary>
    Public Sub New(ByVal Name As String)
        Me.New(Nothing, Name)
    End Sub

    ''' <summary>
    ''' 初始化一个具有指定内容和名称的 <see cref="MemoryIOContext" />。
    ''' </summary>
    ''' <param name="InitalContent">初始化 IO 上下文的内容，若为 <c>null</c>，则表示空。</param>
    ''' <param name="Name">IO 上下文的名称。</param>
    Public Sub New(ByVal InitalContent() As Byte, ByVal Name As String)
        m_Stream = New MemoryStream
        If InitalContent IsNot Nothing AndAlso InitalContent.Length > 0 Then m_Stream.Write(InitalContent, 0, InitalContent.Length)
        m_Name = Name
    End Sub

    Public Overrides ReadOnly Property SupportedMode() As StreamMode
        Get
            Return StreamMode.ForOpen Or StreamMode.ForSave
        End Get
    End Property

    ''' <summary>
    ''' 获取当前 IO 上下文的描述，包括文件的路径。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return "内存 IO"
    End Function
End Class