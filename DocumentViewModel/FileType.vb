Option Compare Text
''' <summary>
''' 用来描述文件类型，以辅助公共对话框的结构。
''' </summary>
''' <remarks>一个这样的结构的实例表示了同一种文件类型（例如：*.Txt 和 *.Text 都是文本文档）。</remarks>
Public Structure FileType
    Implements IEquatable(Of FileType)

    Private Shared ReadOnly EmptyArray() As FileType = New FileType() {}

    '保留属性值的局部变量
    Private m_Filter As String
    Private m_Description As String

    ''' <summary>
    ''' 获取/设置当前文件类型的过滤器，使用分号（;）进行分隔。
    ''' </summary>
    Public Property Filter() As String
        Get
            Return m_Filter
        End Get
        Set(ByVal value As String)
            m_Filter = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置当前文件类型的描述。
    ''' </summary>
    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property

    ''' <summary>
    ''' 返回用于文件对话框的过滤器字符串。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return String.Format("{0}（{1}）|{1}", m_Description, m_Filter)
    End Function

    ''' <summary>
    ''' 使用当前的过滤器对文件名进行匹配。
    ''' </summary>
    ''' <param name="FileName">要进行匹配的文件名称（不包括路径）。</param>
    ''' <returns>如果匹配成功，则返回 true；否则返回 false；如果当前过滤器为空，则返回 true。</returns>
    ''' <remarks></remarks>
    Public Function MatchFileName(ByVal fileName As String) As Boolean
        '为了性能考虑，因此不使用 Split 函数。
        '由于此处涉及大量字符串工作，故举例如下：
        'Eg = Test
        'Index  0   1   2   3
        'Char   T   e   s   t
        'Eg.Length = 4
        'Eg.IndexOf("s") = 2
        'Eg.IndexOf("X") = -1
        If m_Filter = Nothing Then  '确定过滤器（字符串）是否为空
            Return fileName = Nothing
        Else
            Dim FilterStartIndex As Integer = 0, FilterLength As Integer  '用于依次保存每个过滤器的在整个 Filter 字符串中的位置
            Do
                FilterLength = m_Filter.IndexOf(";", FilterStartIndex)
                If FilterLength = -1 Then FilterLength = m_Filter.Length - FilterStartIndex '如果从起始处找不到分号，则这个 Filter 字符串是最后一个了
                If fileName Like m_Filter.Substring(FilterStartIndex, FilterLength) Then Return True
                FilterStartIndex += FilterLength + 1    '通过 +1 跳过分号
            Loop Until FilterStartIndex >= m_Filter.Length
            Return False
        End If
    End Function

    ''' <summary>
    ''' 构造一个 <see cref="FileType" /> 数组。
    ''' </summary>
    ''' <param name="Args">每个元素的参数，先是描述，后是过滤器。</param>
    Public Shared Function PharseArray(ByVal ParamArray Args() As String) As FileType()
        If Args.Length > 0 Then
            Dim RV(CInt(Args.Length / 2) - 1) As FileType
            For I = RV.GetLowerBound(0) To RV.GetUpperBound(0)
                RV(I).Description = Args(I * 2)
                RV(I).Filter = Args(I * 2 + 1)
            Next
            Return RV
        Else
            Return EmptyArray
        End If
    End Function

    ''' <summary>
    ''' 构造一个 <see cref="FileType" /> 数组。
    ''' </summary>
    ''' <param name="Args">每个元素的参数，先是描述，后是过滤器，使用“|”分隔开。</param>
    Public Shared Function PharseArray(ByVal Args As String) As FileType()
        If Args = Nothing Then
            Return EmptyArray
        Else
            Return PharseArray(Args.Split("|"c))
        End If
    End Function

    Public Shared Operator =(ByVal x As FileType, ByVal y As FileType) As Boolean
        Return x.Equals(y)
    End Operator

    Public Shared Operator <>(ByVal x As FileType, ByVal y As FileType) As Boolean
        Return Not x.Equals(y)
    End Operator

    ''' <summary>
    ''' 返回一个值，表示此实例是否与指定的对象相等。
    ''' </summary>
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return TypeOf obj Is FileType AndAlso Equals(DirectCast(obj, FileType))
    End Function

    ''' <summary>
    ''' 返回一个值，表示此实例是否与指定的 <see cref="FileType" /> 相等。
    ''' </summary>
    Public Overloads Function Equals(ByVal other As FileType) As Boolean Implements System.IEquatable(Of FileType).Equals
        Return m_Filter = other.m_Filter
    End Function

    ''' <summary>
    ''' 使用指定的信息初始化一个文件类型。
    ''' </summary>
    ''' <param name="description">当前文件类型的描述。</param>
    ''' <param name="filter">当前文件类型的过滤器，使用分号（;）进行分隔。</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal description As String, ByVal filter As String)
        m_Description = description
        m_Filter = filter
    End Sub

    ''' <summary>
    ''' 使用指定的描述初始化一个“所有文件”文件类型。
    ''' </summary>
    ''' <param name="description">当前文件类型的描述。</param>
    Public Sub New(ByVal description As String)
        Me.New(Prompts.AllFiles, "*")
    End Sub
End Structure