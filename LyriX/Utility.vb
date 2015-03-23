Imports System.Text
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.ComponentModel

Namespace Utility
    ''' <summary>
    ''' 对字符串执行固定区域性不分大小写的比较。
    ''' </summary>
    Public Class CaseInsensitiveEqualityComparer
        Inherits EqualityComparer(Of String)

        ''' <summary>
        ''' 返回一个默认的相等比较器，对字符串执行固定区域性不分大小写的比较。
        ''' </summary>
        Public Shared Shadows ReadOnly [Default] As New CaseInsensitiveEqualityComparer

        Public Overloads Overrides Function Equals(ByVal x As String, ByVal y As String) As Boolean
            Return String.Equals(x, y, StringComparison.OrdinalIgnoreCase)
        End Function

        Public Overloads Overrides Function GetHashCode(ByVal obj As String) As Integer
            If obj Is Nothing Then
                Throw New ArgumentNullException("obj")
            Else
                Return obj.ToUpperInvariant.GetHashCode
            End If
        End Function
    End Class

    ''' <summary>
    ''' 表示此枚举项在 XML 中的字符串表示。
    ''' </summary>
    <AttributeUsage(AttributeTargets.Field, AllowMultiple:=False, Inherited:=False)>
    Friend NotInheritable Class XSDExpressionAttribute
        Inherits Attribute

        Private m_Expression As String

        ''' <summary>
        ''' 获取此枚举项在 XML 中的字符串表示。
        ''' </summary>
        ''' <value>此枚举项在 XML 中的字符串表示，如果为 <c>null</c>，则表示使用此枚举项的名称。</value>
        Public ReadOnly Property Expression As String
            Get
                Return m_Expression
            End Get
        End Property

        ''' <param name="expression">此枚举项在 XML 中的字符串表示，如果为 <c>null</c>，则表示使用此枚举项的名称。</param>
        Public Sub New(ByVal expression As String)
            m_Expression = expression
        End Sub

        ''' <summary>
        ''' 使用此枚举项的名称作为其在 XML 中的字符串表示。
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub

        ''' <summary>
        ''' 获取一个枚举项的 XML 字符串表示。
        ''' </summary>
        ''' <param name="info">枚举项的信息。</param>
        Public Shared Function GetXSDExpression(ByVal info As Reflection.FieldInfo) As String
            If info Is Nothing Then
                Throw New ArgumentNullException("info")
            Else
                Dim Attr = DirectCast(Attribute.GetCustomAttribute(info, GetType(XSDExpressionAttribute)), XSDExpressionAttribute)
                If Attr Is Nothing Then
                    Return Nothing
                ElseIf Attr.m_Expression Is Nothing Then
                    Return Attr.m_Expression
                Else
                    Return Attr.m_Expression
                End If
            End If
        End Function
    End Class

    ''' <summary>
    ''' MISC
    ''' </summary>
    <HideModuleName()> Friend Module MUtility

        ''' <summary>
        ''' 将 Xml 中的常数字符串转化为与之对应的枚举项。
        ''' </summary>
        ''' <param name="xsdExpression">要转换的字符串。</param>
        ''' <param name="enumType">要返回的枚举类型。</param>
        Public Function XSDToEnum(ByVal xsdExpression As String, ByVal enumType As Type) As Long?
            If enumType Is Nothing Then
                Throw New ArgumentNullException("enumType")
            ElseIf Not enumType.IsSubclassOf(GetType([Enum])) Then
                Throw New ArgumentOutOfRangeException("enumType", String.Format(ExceptionPrompts.InvalidBaseClass, GetType(Enumerable)))
            ElseIf String.IsNullOrWhiteSpace(xsdExpression) Then
                Return Nothing
            Else
                Dim SubExpressions As String()
                Dim UnderlyingValue As Long
                If Attribute.GetCustomAttribute(enumType, GetType(FlagsAttribute)) Is Nothing Then
                    SubExpressions = {xsdExpression}
                Else
                    '按位组合的枚举，在 Xml 中以被空白分隔的表达式存在
                    SubExpressions = xsdExpression.Split(DirectCast(Nothing, Char()), StringSplitOptions.RemoveEmptyEntries)
                End If
                '按 XSDExpressionAttribute 确定值
                For Each EachExpression In SubExpressions
                    For Each EachField In enumType.GetFields(Reflection.BindingFlags.Static Or Reflection.BindingFlags.Public)
                        If XSDExpressionAttribute.GetXSDExpression(EachField) = EachExpression Then
                            UnderlyingValue = UnderlyingValue Or CLng(EachField.GetValue(Nothing))
                            Exit For
                        End If
                    Next
                Next
                Return UnderlyingValue
            End If
        End Function

        ''' <summary>
        ''' 将枚举项转化为与之对应的 Xml 中的常数字符串。
        ''' </summary>
        ''' <param name="value">要转换的枚举值。</param>
        ''' <exception cref="ArgumentOutOfRangeException">指定的枚举值无法在其枚举类型中找到匹配项。</exception>
        Public Function EnumToXSD(ByVal value As [Enum]) As String
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            Else
                Dim IsFlag = Attribute.GetCustomAttribute(value.GetType, GetType(FlagsAttribute)) IsNot Nothing
                Dim Factors As [Enum]()
                If IsFlag Then
                    'Flags
                    Factors = FactorEnum(value)
                Else
                    Factors = {value}
                End If
                Dim IsFirst As Boolean = False
                Return String.Join(" ", Factors.Select(Function(EachFactor)
                                                           Dim EachName = [Enum].GetName(value.GetType, EachFactor)
                                                           If EachName Is Nothing Then
                                                               '未找到枚举
                                                               Return CStr(Convert.ToUInt64(EachFactor))
                                                           Else
                                                               Return XSDExpressionAttribute.GetXSDExpression(
                                                                                     value.GetType.GetField(EachName,
                                                                                                            Reflection.BindingFlags.Static Or
                                                                                                            Reflection.BindingFlags.Public))
                                                           End If
                                                       End Function))
            End If
        End Function

        ''' <summary>
        ''' 分解标志枚举中的标志因子。
        ''' </summary>
        Public Function FactorEnum(ByVal flagsValue As [Enum]) As [Enum]()
            If flagsValue Is Nothing Then
                Throw New ArgumentNullException("value")
            Else
                Dim FlagValues = [Enum].GetValues(flagsValue.GetType)
                '该数组的元素按枚举常数的二进制值排序。
                Dim FlagFactors As New List(Of [Enum])(FlagValues.Length)
                Dim UnderlyingValue = Convert.ToUInt64(flagsValue)    'CULng 不可
                If UnderlyingValue > 0 Then
                    For Each EachValue As [Enum] In FlagValues
                        Dim EachUnderlyingValue = Convert.ToUInt64(EachValue)
                        If EachUnderlyingValue > 0 AndAlso (UnderlyingValue And EachUnderlyingValue) = EachUnderlyingValue Then
                            '枚举匹配
                            FlagFactors.Add(EachValue)
                            UnderlyingValue -= EachUnderlyingValue
                        End If
                    Next
                    If UnderlyingValue > 0 Then
                        '剩余一些未知项
                        FlagFactors.Add(DirectCast([Enum].ToObject(flagsValue.GetType, UnderlyingValue), [Enum]))
                    End If
                Else
                    'flagsValue = 0
                    If FlagValues.Length > 0 AndAlso Convert.ToUInt64(FlagValues.GetValue(0)) = 0 Then
                        FlagFactors.Add(DirectCast(FlagValues.GetValue(0), [Enum]))
                    End If
                End If
                Return FlagFactors.ToArray
            End If
        End Function

        ''' <summary>
        ''' 用于执行一般的包的 XML 文档部分的读取逻辑。
        ''' </summary>
        ''' <returns>如果找到了指定的部分，则返回 XDocument。否则返回 <c>null</c>。</returns>
        ''' <exception cref="IO.FileFormatException">XML 文档格式不正确。</exception>
        ''' <exception cref="IO.IOException">无法打开指定的部分。</exception>
        <Extension()> Public Function ReadPackagePart(ByVal package As Package, ByVal uri As Uri) As XDocument
            If package IsNot Nothing AndAlso package.PartExists(uri) Then
                Using xs = package.GetPart(uri).GetStream
                    Try
                        'FUTURE 是否保留空白？
                        Return XDocument.Load(xs, LoadOptions.SetLineInfo Or LoadOptions.PreserveWhitespace)
                    Catch ex As Xml.XmlException
                        Throw New IO.FileFormatException(uri, String.Format(ExceptionPrompts.InvalidXmlDocument, uri, ex.Message), ex)
                    End Try
                End Using
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 用于执行一般的包的 XML 文档部分的读取逻辑。
        ''' </summary>
        ''' <returns>不论是否找到了指定的部分，均返回对象实例。</returns>
        ''' <exception cref="IO.FileFormatException">XML 文档格式不正确。</exception>
        ''' <exception cref="IO.IOException">无法打开指定的部分。</exception>
        <Extension()> Public Function ReadPackagePart(Of T As Document.ObjectModel.XPackagePartContainer)(ByVal package As Package, ByVal uri As Uri, ByVal NewInstanceExpression As Func(Of XDocument, T)) As T
            Dim Doc = ReadPackagePart(package, uri)
            Return NewInstanceExpression(Doc)
        End Function

        ' ''' <summary>
        ' ''' 获取指定 <see cref="XContainer" /> 中指定名称的第一个元素。如果元素不存在，则会自动创建。
        ' ''' </summary>
        '<Extension()> Public Function GetElement(ByVal source As XContainer, ByVal name As XName) As XElement
        '    Dim RV As XElement = source.Element(name)
        '    If RV Is Nothing Then
        '        RV = New XElement(name)
        '        source.Add(RV)
        '    End If
        '    Return RV
        'End Function

        ''' <summary>
        ''' 使用指定区域语言设置的列表分隔符设置连接一个列表。
        ''' </summary>
        Public Function JoinList(Of T)(ByVal cultureInfo As CultureInfo, ByVal list As IEnumerable(Of T)) As String
            Return String.Join(If(cultureInfo, cultureInfo.CurrentCulture).TextInfo.ListSeparator, list)
        End Function

        ''' <summary>
        ''' 使用当前区域语言设置的列表分隔符设置连接一个列表。
        ''' </summary>
        Public Function JoinList(Of T)(ByVal list As IEnumerable(Of T)) As String
            Return JoinList(CType(Nothing, CultureInfo), list)
        End Function

        ''' <summary>
        ''' 使用当前区域语言设置的列表分隔符设置连接一个列表。
        ''' </summary>
        Public Function JoinList(Of T)(ByVal list As IList(Of T)) As String
            Return JoinList(CType(Nothing, CultureInfo), list)
        End Function

        ''' <summary>
        ''' 使用当前区域语言设置的列表分隔符设置连接一个列表。
        ''' </summary>
        Public Function JoinList(Of T)(ByVal ParamArray list() As T) As String
            Return JoinList(CType(Nothing, CultureInfo), list)
        End Function
    End Module

    ''' <summary>
    ''' 与语言相关的实用工具。
    ''' </summary>
    Public Class Locale
        Private Shared FriendlyNamesResource As New Resources.ResourceManager("LyriX.FriendlyNames", GetType(Locale).Assembly)

        ''' <summary>
        ''' 将指定的语言标识格式化为标准格式。
        ''' </summary>
        ''' <param name="languageTag">待格式化的符合 BCP47 或 UTS#35 要求的语言标识。</param>
        ''' <returns>符合 BCP47(RFC5646) 建议（如大小写建议）的语言标识。如果 <paramref name="languageTag" /> 为 <c>null</c>，则返回空字符串。</returns>
        Public Shared Function FormatTag(ByVal languageTag As String) As String
            'An implementation can reproduce this format without accessing the registry as follows.
            'All subtags, including extension and private use subtags,
            'use lowercase letters with two exceptions: 
            'two-letter and four-letter subtags that neither 
            'appear at the start of the tag nor occur after singletons.
            'Such two-letter subtags are all uppercase (as in the tags "en-CA-x-ca" or "sgn-BE-FR") 
            'and four-letter subtags are titlecase (as in the tag "az-Latn-x-latn"). 
            Dim builder As New StringBuilder(languageTag)
            Dim startIndex As Integer       '段开始的位置
            Dim afterSingleton As Boolean   '标记是否之前出现了单个字符的段（可能表示私有用途）
            With builder
                For I = 0 To .Length - 1
                    If .Chars(I) = "_"c Then
                        '将 Unicode 允许的下划线分隔转化为横杠
                        .Chars(I) = "-"c
                    End If
                    If .Chars(I) = "-"c Then
                        '分隔符
                        '小结
                        Dim segmentLength = I - startIndex      '段长
                        If segmentLength = 1 Then afterSingleton = True
                        If Not afterSingleton AndAlso startIndex > 0 Then
                            '不是第一段
                            If segmentLength = 2 Then
                                '大写（区域）
                                .Chars(I - 1) = Char.ToUpperInvariant(.Chars(I - 1))
                                .Chars(I - 2) = Char.ToUpperInvariant(.Chars(I - 2))
                            ElseIf segmentLength = 4 Then
                                '首字母大写（脚本）
                                .Chars(startIndex) = Char.ToUpperInvariant(.Chars(startIndex))
                            End If
                        End If
                        startIndex = I + 1
                    Else
                        '小写
                        .Chars(I) = Char.ToLowerInvariant(.Chars(I))
                    End If
                Next
                Return .ToString
            End With
        End Function

        ''' <summary>
        ''' 将指定的语言标记进行回退（fallback）。
        ''' </summary>
        ''' <param name="languageTag">待回退的、符合 BCP47(RFC5646) 要求（除大小写外）语言标记。</param>
        ''' <returns>符合 BCP47(RFC4647) 建议的回退后的语言标记。如果指定的语言标记为空、<c>null</c>，或已无法回退，则返回空字符串。</returns>
        Public Shared Function Fallback(ByVal languageTag As String) As String
            'In the lookup scheme, the language range is progressively truncated
            'from the end until a matching language tag is located.  Single letter
            'or digit subtags (including both the letter 'x', which introduces
            'private-use sequences, and the subtags that introduce extensions) are
            'removed at the same time as their closest trailing subtag.
            If languageTag = Nothing Then
                Return ""
            Else
                Dim LastSeparator = languageTag.LastIndexOf("-"c)
                If LastSeparator = -1 Then
                    '未找到
                    Return ""
                Else
                    '存在横杠，至少 languageTag = "-"
                    Dim RV = languageTag.Substring(0, LastSeparator)
                    If RV.Length >= 2 AndAlso RV(RV.Length - 2) = "-"c Then
                        '末端存在单个字符，继续回退
                        Return Fallback(RV)
                    ElseIf RV.Length = 1 Then
                        '单个字符，直接回退
                        Return ""
                    Else
                        Return RV
                    End If
                End If
            End If
        End Function

        ''' <summary>
        ''' 获取此 LyriX 程序集中类型的友好名称。
        ''' </summary>
        ''' <param name="type">要获取友好名称的 LyriX 程序集中的一个类型。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="type" /> 为 <c>null</c>。</exception>
        Public Shared Function GetFriendlyName(type As Type) As String
            If type Is Nothing Then
                Throw New ArgumentNullException("type")
            Else
                Return If(FriendlyNamesResource.GetString(type.Name), type.Name)
            End If
        End Function

        ''' <summary>
        ''' 获取此 LyriX 程序集中类型的友好名称。
        ''' </summary>
        ''' <param name="ref">要获取友好名称的 LyriX 程序集中的一个类型的实例。</param>
        ''' <exception cref="ArgumentNullException"><paramref name="ref" /> 为 <c>null</c>。</exception>
        Public Shared Function GetFriendlyName(ref As Object) As String
            If ref Is Nothing Then
                Throw New ArgumentNullException("ref")
            ElseIf TypeOf ref Is [Enum] Then
                Dim EnumName = [Enum].GetName(ref.GetType, ref)
                If EnumName Is Nothing Then
                    '可能是标志
                    If Attribute.GetCustomAttribute(ref.GetType, GetType(FlagsAttribute)) IsNot Nothing Then
                        'Flags
                        Return JoinList(From EachEnum In FactorEnum(DirectCast(ref, [Enum]))
                                        Select GetFriendlyName(EachEnum))
                    End If
                Else
                    '枚举，格式为 EnumType.EnumName
                    Return FriendlyNamesResource.GetString(ref.GetType.Name & "." & EnumName)
                End If
                Return ref.ToString
            Else
                Return GetFriendlyName(ref.GetType)
            End If
        End Function

        Private Sub New()

        End Sub
    End Class

    'Public Class StringArrayConverter

    ''' <summary>
    ''' 为 LyriX 文档提供样例。
    ''' </summary>
    Public Class Samples
        Private Shared SamplePackage As Document.LyriXPackage = CreatePackage()

        ''' <summary>
        ''' 获取一个 <see cref="Compiled.LyricsDocument" /> 的样例。
        ''' </summary>
        Public Shared ReadOnly CompiledDocumentSample As Compiled.LyricsDocument

        ''' <summary>
        ''' 获取一个包含 <see cref="Compiled.Line" /> 的样例。
        ''' </summary>
        Public Shared ReadOnly CompiledLineSamples As IList(Of Compiled.Line)

        Public Shared ReadOnly CompiledVersionSamples As IList(Of Compiled.Version)

        Public Shared ReadOnly CompiledTrackSamples As IList(Of Compiled.Track)

        Public Shared ReadOnly CompiledArtistSamples As IList(Of Compiled.ArtistBase)

        ''' <summary>
        ''' 构造一个样例 <see cref="Document.LyriXPackage" />。
        ''' </summary>
        ''' <returns>一个新的 <see cref="Document.LyriXPackage" />，其中包含了歌词文档的样例。</returns>
        Public Shared Function CreatePackage() As Document.LyriXPackage
            Dim packageStream As New IO.MemoryStream(My.Resources.package)
            Return New Document.LyriXPackage(packageStream)
        End Function

        Private Sub New()

        End Sub

        Shared Sub New()
            Dim compiler As New Compilers.LyriXCompiler
            CompiledDocumentSample = compiler.Compile(SamplePackage)
            CompiledArtistSamples = CompiledDocumentSample.MusicInfo.Artists
            CompiledVersionSamples = CompiledDocumentSample.Lyrics.Versions
            CompiledTrackSamples = CompiledDocumentSample.Lyrics.Versions(0).Tracks
            CompiledLineSamples = CompiledDocumentSample.Lyrics.Versions(0).Tracks(0).Lines
        End Sub
    End Class
End Namespace