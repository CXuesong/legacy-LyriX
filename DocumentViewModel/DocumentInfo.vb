''' <summary>
''' 指定了文档的匹配程度
''' </summary>
Public Enum DocumentMatchResult
    ''' <summary>
    ''' 不匹配。
    ''' </summary>
    SureUnmatch = -2
    ''' <summary>
    ''' 可能不匹配。就扩展名或文件名而言是不匹配的。
    ''' </summary>
    MayUnmatch = -1
    ''' <summary>
    ''' 既无法证明不匹配，也无法证明是匹配的（默认值）。
    ''' </summary>
    Unknown = 0
    ''' <summary>
    ''' 可能匹配。就扩展名或文件名而言是匹配的。
    ''' </summary>
    MayMatch = 1
    ''' <summary>
    ''' 肯定匹配。
    ''' </summary>
    SureMatch = 2
End Enum

''' <summary>
''' 公开一个根据 IO 上下文匹配对应的 <see cref="Document" /> 的方法。
''' </summary>
Public Interface IDocumentMatcher
    ''' <summary>
    ''' 根据 IO 上下文匹配对应的 <see cref="Document" />，并返回二者的匹配程度。
    ''' </summary>
    ''' <param name="IOContext">待匹配的 IO 上下文。</param>
    ''' <param name="DocumentType">待匹配的目标文档类型。</param>
    ''' <exception cref="ArgumentNullException"><paramref name="IOContext" /> 或 <paramref name="DocumentType" /> 为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentOutOfRangeException">指定的 <paramref name="DocumentType" /> 所对应的类型没有继承自 <see cref="Document" /></exception>
    Function Match(ByVal IOContext As IOContext, ByVal DocumentType As Type) As DocumentMatchResult
End Interface

''' <summary>
''' 为 <see cref="IDocumentMatcher" /> 提供默认实现。
''' </summary>
Public Class DocumentMatcher
    Implements IDocumentMatcher

    ''' <summary>
    ''' 获取一个默认的 <see cref="DocumentMatcher" /> 实例。
    ''' </summary>
    Public Shared ReadOnly [Default] As New DocumentMatcher

    ''' <summary>根据指定的 <see cref="IOContext" /> 匹配对应的 <see cref="Document" />，并返回二者的匹配程度。</summary>
    ''' <param name="ioContext">待匹配的 IO 上下文。</param>
    ''' <param name="documentType">待匹配的目标文档类型。</param>
    ''' <remarks>默认实现为仅匹配 <see cref="FileIOContext" /> 与指定文档类型的文件扩展名，
    ''' 若匹配，则返回 <see cref="DocumentMatchResult.MayMatch" />，
    ''' 否则返回 <see cref="DocumentMatchResult.MayUnmatch" />。
    ''' 如果指定的 <see cref="ioContext" /> 不是 <see cref="FileIOContext" />，则返回 <see cref="DocumentMatchResult.Unknown" />。</remarks>
    ''' <exception cref="ArgumentNullException"><paramref name="ioContext" /> 或 <paramref name="documentType" /> 为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentOutOfRangeException">指定的 <paramref name="documentType" /> 所对应的类型没有继承自 <see cref="Document" /></exception>
    Public Function Match(ByVal ioContext As IOContext, ByVal documentType As System.Type) As DocumentMatchResult Implements IDocumentMatcher.Match
        If ioContext Is Nothing Then
            Throw New ArgumentNullException("ioContext")
        ElseIf documentType Is Nothing Then
            Throw New ArgumentNullException("documentType")
        ElseIf Not documentType.IsSubclassOf(GetType(Document)) Then
            Throw New ArgumentOutOfRangeException(String.Format(ExceptionPrompts.InvalidBaseClass, GetType(Document)))
        Else
            Return OnMatch(ioContext, documentType)
        End If
    End Function

    ''' <summary>
    ''' 在派生类中重写时，用于匹配指定的 IO 上下文与文档类型。
    ''' </summary>
    ''' <param name="ioContext">待匹配的 IO 上下文。</param>
    ''' <param name="documentType">待匹配的目标文档类型。</param>
    ''' <remarks>在此方法被调用前，已经确保 <paramref name="ioContext" /> 不为 null，且 <paramref name="documentType" /> 所指定的类型是继承自 <see cref="Document" /> 的。</remarks>
    Protected Overridable Function OnMatch(ByVal ioContext As IOContext, ByVal documentType As System.Type) As DocumentMatchResult
        If TypeOf ioContext Is FileIOContext Then
            '如果是文件，可以通过扩展名来判断
            For Each EachType As FileType In DocumentInformation.GetInfo(documentType).FileTypes
                If EachType.MatchFileName(DirectCast(ioContext, FileIOContext).DestFile.Name) Then
                    Return DocumentMatchResult.MayMatch
                End If
            Next
            Return DocumentMatchResult.MayUnmatch
        Else
            Return DocumentMatchResult.Unknown
        End If
    End Function
End Class

''' <summary>
''' 用于表示文档的版本号。
''' </summary>
''' <remarks>
''' <para>版本号的格式如下所示：</para>
''' <para>主版本.次版本</para>
''' <para>应根据下面的约定使用这些部分：</para>
''' <list type="table">
''' <item><term>主版本</term><description>名称相同但主版本号不同的程序集不可互换。例如，这适用于对产品的大量重写，这些重写使得无法实现向后兼容性。</description></item>
''' <item><term>次版本</term><description>如果两个程序集的名称和主版本号相同，而次版本号不同，这指示显著增强，但照顾到了向后兼容性。例如，这适用于产品的修正版或完全向后兼容的新版本。</description></item>
''' </list>
''' </remarks>
<Serializable()> Public Structure DocumentVersion
    '不要指定会与核心命名空间中的任何类型发生冲突的类型名称（Version）。
    '核心命名空间是 System.* 命名空间（不包括应用程序命名空间和基础结构命名空间）。
    Implements IEquatable(Of DocumentVersion), IComparable, IComparable(Of DocumentVersion)

    Private m_Major As Integer
    Private m_Minor As Integer

    ''' <summary>
    ''' 表示一个 <see cref="DocumentVersion" />，其版本为 0.0。
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared ReadOnly Empty As DocumentVersion = New DocumentVersion(0, 0)

    ''' <summary>
    ''' 获取/设置版本的主版本号。
    ''' </summary>
    Public Property Major() As Integer
        Get
            Return m_Major
        End Get
        Set(ByVal value As Integer)
            m_Major = value
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置版本的次版本号。
    ''' </summary>
    Public Property Minor() As Integer
        Get
            Return m_Minor
        End Get
        Set(ByVal value As Integer)
            m_Minor = value
        End Set
    End Property

    ''' <summary>
    ''' 返回当前 <see cref="DocumentVersion" /> 的哈希代码。
    ''' </summary>
    Public Overrides Function GetHashCode() As Integer
        Return m_Major Xor m_Minor
    End Function

    ''' <summary>
    ''' 将当前版本转换为其等效的 <see cref="String" /> 表示形式，其格式为 主板本号.次版本号。
    ''' </summary>
    Public Overrides Function ToString() As String
        Return m_Major & "." & m_Minor
    End Function

    ''' <summary>
    ''' 将当前 <see cref="DocumentVersion" /> 对象与指定的 <see cref="DocumentVersion" /> 对象进行比较，并返回二者相对值的一个指示。
    ''' </summary>
    ''' <param name="other">要比较的版本。</param>
    ''' <returns>
    ''' <list type="table">
    ''' <listheader><term>返回值</term><description>说明</description></listheader>
    ''' <item><term>小于零</term><description>当前版本是 <paramref name="other" /> 之前的一个版本。 </description></item>
    ''' <item><term>零</term><description>当前版本是 <paramref name="other" /> 相同的版本。 </description></item>
    ''' <item><term>大于零</term><description>当前版本是 <paramref name="other" /> 之后的一个版本。 </description></item>
    ''' </list>
    ''' </returns>
    Public Function CompareTo(ByVal other As DocumentVersion) As Integer Implements System.IComparable(Of DocumentVersion).CompareTo
        If m_Major = other.m_Major Then
            Return m_Minor.CompareTo(other.m_Minor)
        Else
            Return m_Major.CompareTo(other.m_Major)
        End If
    End Function

    ''' <summary>
    ''' 将当前对象与指定的对象进行比较，并返回二者相对值的一个指示。
    ''' </summary>
    ''' <param name="obj">要比较的版本，或是 <c>null</c>。</param>
    ''' <returns>
    ''' <list type="table">
    ''' <listheader><term>返回值</term><description>说明</description></listheader>
    ''' <item><term>小于零</term><description>当前版本是 <paramref name="other" /> 之前的一个版本。 </description></item>
    ''' <item><term>零</term><description>当前版本是 <paramref name="other" /> 相同的版本。 </description></item>
    ''' <item><term>大于零</term><description><para>当前版本是 <paramref name="other" /> 之后的一个版本。 </para><para>- 或 -</para><para><paramref name="other" /> 为 <c>null</c>。</para></description></item>
    ''' </list>
    ''' </returns>
    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
        If obj Is Nothing Then
            Return 1
        ElseIf TypeOf obj Is DocumentVersion Then
            Return CompareTo(DirectCast(obj, DocumentVersion))
        Else
            Throw New ArgumentException(String.Format(ExceptionPrompts.InvalidInstanceType, GetType(DocumentVersion)), "obj")
        End If
    End Function

    ''' <summary>
    ''' 返回一个值，指示当前 <see cref="DocumentVersion" /> 对象和指定的 <see cref="DocumentVersion" /> 对象是否表示同一个值。
    ''' </summary>
    ''' <param name="other">要比较的版本。</param>
    Public Overloads Function Equals(ByVal other As DocumentVersion) As Boolean Implements System.IEquatable(Of DocumentVersion).Equals
        Return m_Major = other.m_Major AndAlso m_Minor = other.m_Minor
    End Function

    ''' <summary>
    ''' 返回一个值，指示当前 <see cref="DocumentVersion" /> 对象和指定的对象是否表示同一个值。
    ''' </summary>
    ''' <param name="obj">要比较的版本，或是 <c>null</c>。</param>
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is DocumentVersion Then
            Return Equals(DirectCast(obj, DocumentVersion))
        Else
            Return False
        End If
    End Function

    Public Shared Operator =(ByVal x As DocumentVersion, ByVal y As DocumentVersion) As Boolean
        Return x.Equals(y)
    End Operator

    Public Shared Operator <>(ByVal x As DocumentVersion, ByVal y As DocumentVersion) As Boolean
        Return Not x.Equals(y)
    End Operator

    Public Shared Operator >(ByVal x As DocumentVersion, ByVal y As DocumentVersion) As Boolean
        Return x.CompareTo(y) > 0
    End Operator

    Public Shared Operator >=(ByVal x As DocumentVersion, ByVal y As DocumentVersion) As Boolean
        Return x.CompareTo(y) >= 0
    End Operator

    Public Shared Operator <(ByVal x As DocumentVersion, ByVal y As DocumentVersion) As Boolean
        Return x.CompareTo(y) < 0
    End Operator

    Public Shared Operator <=(ByVal x As DocumentVersion, ByVal y As DocumentVersion) As Boolean
        Return x.CompareTo(y) <= 0
    End Operator

    ''' <summary>
    ''' 使用指定的主版本号值和次版本号值进行初始化。
    ''' </summary>
    ''' <param name="version">指定版本号，其格式为 主板本号.次版本号。</param>
    ''' <exception cref="ArgumentOutOfRangeException">指定的主板本号或次版本号小于零。</exception>
    ''' <exception cref="ArgumentException">指定的版本格式错误。</exception>
    Public Sub New(ByVal version As String)
        If version = Nothing Then
            m_Major = 0
            m_Minor = 0
        Else
            Dim values() As String = version.Split("."c)
            If values.Length <> 2 Then
                Throw New ArgumentException(ExceptionPrompts.InvalidFormat, "version")
            Else
                m_Major = Integer.Parse(values(0))
                m_Minor = Integer.Parse(values(1))
                If m_Major < 0 OrElse m_Minor < 0 Then
                    Throw New ArgumentOutOfRangeException("version")
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' 使用指定的主版本号值和次版本号值进行初始化。
    ''' </summary>
    ''' <param name="major">指定主要版本号。</param>
    ''' <param name="minor">指定次要版本号。</param>
    ''' <exception cref="ArgumentOutOfRangeException"><paramref name="major" /> 或 <paramref name="minor" /> 小于零。</exception>
    Public Sub New(ByVal major As Integer, ByVal minor As Integer)
        m_Major = major
        m_Minor = minor
    End Sub
End Structure

''' <summary>
''' 表示 <see cref="Document" /> 的派生类的文档信息。
''' </summary>

Public NotInheritable Class DocumentInformation
    Private m_DocumentType As Type
    Private m_DocumentName As String
    Private m_Version As DocumentVersion
    Private m_FileTypes As IEnumerable(Of FileType)
    Private m_ViewType As Type
    Private m_DefaultTitle As String
    Private m_Icon As Icon
    Private m_MatcherType As Type

    Private m_MatcherInstance As IDocumentMatcher

    ''' <summary>
    ''' 获取包含当前文档信息的文档类型。
    ''' </summary>
    Public ReadOnly Property DocumentType() As Type
        Get
            Return m_DocumentType
        End Get
    End Property

    ''' <summary>
    ''' 获取文档的默认图标。
    ''' </summary>
    ''' <value>文档的默认图标，可以用来代替 <see cref="Icon" /> 为 <c>null</c> 的文档的图标设置。</value>
    Public Shared ReadOnly Property DefaultIcon() As Icon
        Get
            Return My.Resources.Document.Icon
        End Get
    End Property

    ''' <summary>
    ''' 获取文档的名称。
    ''' </summary>
    Public ReadOnly Property DocumentName() As String
        Get
            Return m_DocumentName
        End Get
    End Property

    ''' <summary>
    ''' 获取文档类型的版本。
    ''' </summary>
    Public ReadOnly Property Version() As DocumentVersion
        Get
            Return m_Version
        End Get
    End Property

    ''' <summary>
    ''' 获取为空文档指定默认标题时使用的格式。
    ''' </summary>
    ''' <value>默认标题的格式，使用数字占位符 {0} 来表示文档数目。例如：无标题 {0}。</value>
    Public ReadOnly Property DefaultTitle() As String
        Get
            Return m_DefaultTitle
        End Get
    End Property

    ''' <summary>
    ''' 获取文档处理的所有文件的类型。
    ''' </summary>
    ''' <value>包含了此文档支持的所有文件类型的列表。</value>
    Public ReadOnly Property FileTypes() As IEnumerable(Of FileType)
        Get
            Return m_FileTypes
        End Get
    End Property

    ''' <summary>
    ''' 获取文档使用的视图对象的类型。
    ''' </summary>
    ''' <value>文档使用的实现了 <see cref="IDocumentView" /> 视图对象的类型。如果为 <c>null</c>，则表示无首选视图类型。</value>
    Public ReadOnly Property ViewType() As Type
        Get
            Return m_ViewType
        End Get
    End Property

    ''' <summary>
    ''' 获取文档的图标。
    ''' </summary>
    ''' <value>文档的图标。如果为 <c>null</c>，则表示没有特定的文档图标。</value>
    Public ReadOnly Property Icon() As Icon
        Get
            Return m_Icon
        End Get
    End Property

    ''' <summary>
    ''' 设置用于将当前文档类型与指定 <see cref="IOContext" /> 进行比较的 <see cref="IDocumentMatcher" /> 对象类型。
    ''' </summary>
    ''' <value>一个能够对 <see cref="IOContext" /> 与当前文档类型进行匹配的、具有公共空构造函数的、实现 <see cref="IDocumentMatcher" /> 的类型。如果为 <c>null</c>，则表示将使用默认设置。</value>
    Public ReadOnly Property MatcherType() As Type
        Get
            Return m_MatcherType
        End Get
    End Property

    ''' <summary>
    ''' 将当前文档类型与指定的 <see cref="IOContext" /> 匹配，并返回匹配结果。
    ''' </summary>
    ''' <param name="IOContext">指定要进行匹配的 <see cref="IOContext" />。</param>
    Public Function MatchDocument(ByVal IOContext As IOContext) As DocumentMatchResult
        '创建实例
        If m_MatcherInstance Is Nothing Then
            If m_MatcherType Is Nothing Then
                m_MatcherInstance = DocumentMatcher.Default
            Else
                m_MatcherInstance = DirectCast(Activator.CreateInstance(m_MatcherType), IDocumentMatcher)
            End If
        End If
        '进行比较
        Return m_MatcherInstance.Match(IOContext, DocumentType)
    End Function

    Private Sub New(ByVal documentType As Type, ByVal documentName As String, ByVal version As DocumentVersion, ByVal fileTypes As IEnumerable(Of FileType), ByVal viewType As Type, ByVal icon As Icon, ByVal matcherType As Type, ByVal defaultTitle As String)
        m_DocumentType = documentType
        m_DocumentName = documentName
        m_Version = version
        m_FileTypes = fileTypes
        m_ViewType = viewType
        m_Icon = icon
        m_MatcherType = matcherType
        m_DefaultTitle = defaultTitle
    End Sub

    Private Sub New(ByVal documentType As Type)
        '如果资源不存在，则会返回 Nothing，但如果资源的内容为空，则返回空字符串 ""
        Dim InfoAttr = DirectCast(Attribute.GetCustomAttribute(documentType, GetType(DocumentInfoAttribute)), DocumentInfoAttribute)
        m_DocumentType = documentType
        If InfoAttr Is Nothing Then
            Dim RM As New Resources.ResourceManager(documentType)
            m_Version = DocumentVersion.Empty
            m_ViewType = Nothing
            m_MatcherType = Nothing
            Try
                m_DocumentName = If(RM.GetString("DocumentName"), My.Resources.Document.DocumentName)
                m_FileTypes = FileType.PharseArray(RM.GetString("FileTypes"))
                m_DefaultTitle = If(RM.GetString("DefaultTitle"), My.Resources.Document.DefaultTitle)
                m_Icon = DirectCast(RM.GetObject("Icon"), Drawing.Icon)
            Catch ex As Resources.MissingManifestResourceException
                m_DocumentName = My.Resources.Document.DocumentName
                m_DefaultTitle = My.Resources.Document.DefaultTitle
                m_FileTypes = FileType.PharseArray("")
                m_Icon = Nothing
            End Try
        Else
            Dim RM As New Resources.ResourceManager(If(InfoAttr.ResourceType, documentType))
            m_Version = If(InfoAttr.Version Is Nothing, DocumentVersion.Empty, New DocumentVersion(InfoAttr.Version))
            m_ViewType = InfoAttr.ViewType
            m_MatcherType = InfoAttr.MatcherType
            Try
                m_DocumentName = If(InfoAttr.DocumentName, If(RM.GetString("DocumentName"), My.Resources.Document.DocumentName))
                m_FileTypes = FileType.PharseArray(If(InfoAttr.FileTypes, RM.GetString("FileTypes")))
                m_Icon = DirectCast(RM.GetObject("Icon"), Drawing.Icon)
                m_DefaultTitle = If(InfoAttr.DefaultTitle, If(RM.GetString("DefaultTitle"), My.Resources.Document.DefaultTitle))
            Catch ex As Resources.MissingManifestResourceException
                m_DocumentName = If(InfoAttr.DocumentName, My.Resources.Document.DocumentName)
                m_FileTypes = FileType.PharseArray(InfoAttr.FileTypes)
                m_Icon = Nothing
                m_DefaultTitle = If(InfoAttr.DefaultTitle, My.Resources.Document.DefaultTitle)
            End Try
        End If
    End Sub

    Private Shared InfoCache As New Dictionary(Of Type, DocumentInformation)

    ''' <summary>
    ''' 获取指定文档类型的信息。
    ''' </summary>
    ''' <param name="documentType">要获取文档信息的自 <see cref="Document" /> 派生的文档类的类型。</param>
    ''' <returns>文档信息。由于 <see cref="Document" /> 中已经指定了基本的文档信息，因此一般不可能为 <c>null</c>。</returns>
    ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
    ''' <exception cref="ArgumentOutOfRangeException">指定的类型不是自 Document 派生的。</exception>
    ''' <remarks>
    ''' <para>在程序中，往往不能完整的表示一个文档所需的全部信息，因此您可能需要为您的 <see cref="Document" /> 的派生类指定一个 <see cref="DocumentInfoAttribute" />，和/或是创建一个 .resource 资源。</para>
    ''' <para>在使用此方法获取信息时，将会根据指定的 <paramref name="documentType" /> 推导 .resources 文件的程序集、基名称和命名空间。此方法假定您将使用附属程序集，并希望使用默认的 ResourceSet 类。给定一个类型（如 MyCompany.MyProduct.MyDocument），此方法将在定义 MyDocument 的程序集中查找名为“MyCompany.MyProduct.MyDocument.[culture name.]resources”的 .resources 文件（在主程序集和附属程序集内）。</para>
    ''' <para>载入 .resources 文件后，将以区分大小写的方式从中搜索以下资源：</para>
    ''' <list type="table">
    ''' <item><term>DocumentName</term><description>String, 文档名称</description></item>
    ''' <item><term>FileTypes</term><description><para>String, 文档使用的文件类型，使用“|”分隔各类型的名称与过滤器，例如：</para><para>文本文档|*.txt;*.text|日志文档|*.log</para><para>请注意，不要再过滤器的名称中包含对过滤器内容的描述，如：文本文档（*.txt）|*.txt;*.text|日志文档（*.log）|*.log</para><para>在使用时，程序会决定是否自动生成括号内的文本。</para></description></item>
    ''' <item><term>Icon</term><description>Icon, 文档图标</description></item>
    ''' <item><term>DefaultTitle</term><description>String, 为空文档指定默认标题时使用的格式。使用 {0} 来表示文档数目，例如：无标题 {0}。</description></item>
    ''' </list>
    ''' <para>如果以上项目中有一项或多项未找到，则会使用默认值。需要注意的是，在查找资源时可能会引发其他异常，如在部署或安装附属程序集时出现错误，就可能引发 <see cref="IO.FileLoadException" />。</para>
    ''' <para>如果您同时为一个类型指定了 <see cref="DocumentInfoAttribute" /> 和 .resource 文件，则会优先使用 <see cref="DocumentInfoAttribute" /> 中的设置。如果其中的部分属性为 <c>null</c>，则会尝试从 .resource 文件中获取信息，如果失败，则会使用默认值。</para>
    ''' <para>请注意，空字符串（<c>""</c>）与 <c>null</c>是不同的。</para>
    ''' </remarks>
    Public Shared Function GetInfo(ByVal documentType As Type) As DocumentInformation
        If documentType Is Nothing Then
            Throw New ArgumentNullException("documentType")
        ElseIf Not GetType(Document).IsAssignableFrom(documentType) Then
            Throw New ArgumentOutOfRangeException("documentType", documentType, String.Format(ExceptionPrompts.NotAssignableFrom, GetType(Document)))
        Else
            Dim RInfo As DocumentInformation = Nothing
            If Not InfoCache.TryGetValue(documentType, RInfo) Then  '尝试返回缓存项
                '如果失败，则重新获取信息
                RInfo = New DocumentInformation(documentType)
                InfoCache.Add(documentType, RInfo)
            End If
            Return RInfo
        End If
    End Function

    ''' <summary>
    ''' 清除指定文档类型的信息缓存，以更新此文档类型的信息。
    ''' </summary>
    ''' <param name="documentType">要更新文档信息的类型。</param>
    ''' <exception cref="ArgumentNullException"><paramref name="documentType" /> 为 <c>null</c>。</exception>
    Public Shared Sub UpdateInfo(ByVal documentType As Type)
        If documentType Is Nothing Then
            Throw New ArgumentNullException("documentType")
        Else
            InfoCache.Remove(documentType)
        End If
    End Sub

    ''' <summary>
    ''' 清除所有文档类型的信息缓存，以更新所有文档类型的信息。
    ''' </summary>
    Public Shared Sub UpdateInfo()
        InfoCache.Clear()
    End Sub

    ''' <summary>
    ''' 使用当前已指定的文档类型对指定的 IO 上下文进行匹配。
    ''' </summary>
    ''' <param name="IOContext">待匹配的 IO 上下文。</param>
    ''' <param name="DocumentType">待匹配的目标文档类型。</param>
    Public Shared Function MatchDocument(ByVal documentType As Type, ByVal IOContext As IOContext) As DocumentMatchResult
        Return GetInfo(documentType).MatchDocument(IOContext)
    End Function
End Class

''' <summary>
''' 用于为 <see cref="Document" /> 的派生类指定文档信息。
''' </summary>
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)> _
Public NotInheritable Class DocumentInfoAttribute
    Inherits Attribute

    '用于缓存由构造函数指定的参数
    Private m_ResourceType As Type
    Private m_DocumentName As String
    Private m_Version As String
    Private m_FileTypes As String
    Private m_ViewType As Type
    Private m_DefaultTitle As String
    Private m_MatcherType As Type

    ''' <summary>
    ''' 获取/设置文档的名称。
    ''' </summary>
    ''' <value>文档的名称。如果为 <c>null</c> 则表示将使用资源中指定的设置。</value>
    Public Property DocumentName() As String
        Set(ByVal value As String)
            m_DocumentName = value
        End Set
        Get
            Return m_DocumentName
        End Get
    End Property

    ''' <summary>
    ''' 获取/设置为空文档指定默认标题时使用的格式。
    ''' </summary>
    ''' <value>默认标题的格式，使用数字占位符 {0} 来表示文档数目。例如：无标题 {0}。如果为 <c>null</c>，则表示将使用资源中指定的或是默认设置。</value>
    Public Property DefaultTitle() As String
        Set(ByVal value As String)
            m_DefaultTitle = value
        End Set
        Get
            Return m_DefaultTitle
        End Get
    End Property

    ''' <summary>
    ''' 获取/设置文档处理的所有文件的类型。
    ''' </summary>
    ''' <value>文档处理的所有文件的类型的字符串表示，其语法参见 <see cref="FileType.PharseArray" />。如果为 <c>null</c>，则表示将使用资源中指定的或是默认设置。</value>
    Public Property FileTypes() As String
        Set(ByVal value As String)
            m_FileTypes = value
        End Set
        Get
            Return m_FileTypes
        End Get
    End Property

    ''' <summary>
    ''' 获取/设置文档使用的视图对象的类型。
    ''' </summary>
    ''' <value>一个实现了 <see cref="IDocumentView" /> 的视图对象的类型，如果为 <c>null</c>，则表示无默认视图类型。</value>
    Public Property ViewType() As Type
        Set(ByVal value As Type)
            m_ViewType = value
        End Set
        Get
            Return m_ViewType
        End Get
    End Property

    ''' <summary>
    ''' 获取/设置用于将当前文档类型与指定 <see cref="IOContext" /> 进行比较的 <see cref="IDocumentMatcher" /> 对象类型。
    ''' </summary>
    ''' <value>一个能够对 <see cref="IOContext" /> 与当前文档类型进行匹配的、具有公共空构造函数的、实现 <see cref="IDocumentMatcher" /> 的类型。如果为 <c>null</c>，则会使用 <see cref="DocumentMatcher.[Default]" />。</value>
    ''' <exception cref="ArgumentOutOfRangeException">指定的类型未实现 <see cref="IDocumentMatcher" /></exception>
    ''' <exception cref="MissingMethodException">在指定的类型中找不到公共空构造函数。</exception>
    Public Property MatcherType() As Type
        Set(ByVal value As Type)
            If value Is Nothing Then
                m_MatcherType = Nothing
            ElseIf Not GetType(IDocumentMatcher).IsAssignableFrom(value) Then
                Throw New ArgumentOutOfRangeException("value", value, String.Format(ExceptionPrompts.InterfaceUnimplemented, GetType(IDocumentMatcher)))
            ElseIf value.GetConstructors.Any(Function(EachConst) EachConst.GetParameters.Length = 0) = False Then
                Throw New MissingMethodException(value.Name, Reflection.ConstructorInfo.ConstructorName)
            Else
                m_MatcherType = value
            End If
        End Set
        Get
            Return m_MatcherType
        End Get
    End Property

    ''' <summary>
    ''' 获取文档版本，其格式为“主版本号.次版本号”，如果为 <c>null</c>，则表示无特定版本（0.0）。
    ''' </summary>
    Public ReadOnly Property Version() As String
        Get
            Return m_Version
        End Get
    End Property

    ''' <summary>
    ''' 获取一个类型，将以此类型在附属程序集内载入 .resources 资源。
    ''' </summary>
    ''' <value>一个类型，将以此类型在附属程序集内载入 .resources 资源。如果此属性为 <c>null</c>，则表示会使用当前的文档类型。</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ResourceType() As Type
        Get
            Return m_ResourceType
        End Get
    End Property

    ''' <summary>
    ''' 使用指定信息初始化一个文档信息类。
    ''' </summary>
    ''' <param name="resourceType">指定一个类型（一般是此文档的类型），将以此类型在附属程序集内载入 .resources 资源。如果此参数为 <c>null</c>，则会使用当前的文档类型。</param>
    ''' <param name="version">指定文档版本，其格式为“主版本号.次版本号”，如果为 <c>null</c>，则表示无特定版本（0.0）。</param>
    ''' <remarks>
    ''' <para>为了方便文档的版本管理，建议为文档类指定一个版本。</para>
    ''' </remarks>
    Public Sub New(ByVal resourceType As Type, ByVal version As String)
        m_ResourceType = resourceType
        m_Version = version
    End Sub

    ''' <summary>
    ''' 使用指定信息初始化一个文档信息类。
    ''' </summary>
    ''' <param name="resourceType">指定一个类型（一般是此文档的类型），将以此类型在附属程序集内载入 .resources 资源。如果此参数为 <c>null</c>，则会使用当前的文档类型。</param>
    ''' <remarks>
    ''' <para>为了方便文档的版本管理，建议为文档类指定一个版本。</para>
    ''' </remarks>
    Public Sub New(ByVal resourceType As Type)
        Me.New(resourceType, Nothing)
    End Sub
    ''' <summary>
    ''' 使用指定信息初始化一个文档信息类。
    ''' </summary>
    ''' <param name="version">指定文档版本，其格式为“主版本号.次版本号”，如果为 <c>null</c>，则表示无特定版本（0.0）。</param>
    ''' <remarks>
    ''' <para>为了方便文档的版本管理，建议为文档类指定一个版本。</para>
    ''' </remarks>
    Public Sub New(ByVal version As String)
        Me.New(Nothing, version)
    End Sub

    ''' <summary>
    ''' 使用默认值初始化一个文档信息类。
    ''' </summary>
    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub
End Class