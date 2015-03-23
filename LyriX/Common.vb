Imports <xmlns="LyriX/2011/package/common.xsd">

''' <summary>
''' 表示性别。
''' </summary>
Public Enum Sex
    ''' <summary>
    ''' 男性。
    ''' </summary>
    <XSDExpression("male")> Male = 0
    ''' <summary>
    ''' 女性。
    ''' </summary>
    <XSDExpression("female")> Female
End Enum

''' <summary>
''' 表示艺术家参与歌曲的具体工作。
''' </summary>
<Flags()> Public Enum ArtistJobs
    ''' <summary>
    ''' 暂无工作。
    ''' </summary>
    <XSDExpression("")> None = 0
    ''' <summary>
    ''' 作曲。
    ''' </summary>
    <XSDExpression("tuneComposition")> TuneComposition = 2
    ''' <summary>
    ''' 作词。
    ''' </summary>
    <XSDExpression("lyricsComposition")> LyricsComposition = 4
    ''' <summary>
    ''' 演奏。
    ''' </summary>
    <XSDExpression("tunePerformance")> TunePerformance = 8
    ''' <summary>
    ''' 演唱。
    ''' </summary>
    <XSDExpression("lyricsPerformance")> LyricsPerformance = 16
    ''' <summary>
    ''' 其它职务。
    ''' </summary>
    <XSDExpression("others")> Others = 32
End Enum

''' <summary>
''' 轨的类型。
''' </summary>
Public Enum PartType
    ''' <summary>
    ''' 主轨。
    ''' </summary>
    <XSDExpression("primary")> Primary = 0

    ''' <summary>
    ''' 伴唱轨。
    ''' </summary>
    <XSDExpression("accompany")> Accompany
End Enum

''' <summary>
''' 表示一个决定于语言的值。
''' </summary>
''' <typeparam name="TValue">值的类型。</typeparam>
Public Class MultiLanguageValue(Of TValue)
    Implements IEnumerable(Of KeyValuePair(Of String, TValue))

    ''' <summary>
    ''' 获取一个空的实例。
    ''' </summary>
    ''' <remarks>此实例使用空构造函数构造，<see cref="IsSealed" /> 为 <c>true</c>。</remarks>
    Public Shared ReadOnly Empty As New MultiLanguageValue(Of TValue)

    Private LanguageDict As Dictionary(Of String, TValue)
    Private m_IsSealed As Boolean

    Private Sub CheckSealed()
        If m_IsSealed Then Throw New InvalidOperationException(ExceptionPrompts.ObjectReadOnly)
    End Sub

    ''' <summary>
    ''' 根据指定的语言优先级获取与之对应的值。
    ''' </summary>
    ''' <param name="LanguagePriority">一个按语言优先级降序排列的语言标识列表，其第一项的优先级最高。如果为 <c>null</c>，则表示不会匹配任何内容。</param>
    ''' <returns>与指定的语言优先级最匹配的值。如果未找到，则返回 <c>null</c>。</returns>
    Default Public ReadOnly Property Value(ByVal languagePriority As IEnumerable(Of String)) As TValue
        Get
            If languagePriority IsNot Nothing AndAlso LanguageDict.Count > 0 Then
                For Each EachLanguage In languagePriority
                    Do
                        Dim RV As TValue
                        If LanguageDict.TryGetValue(EachLanguage, RV) Then
                            Return RV
                        Else
                            '失败，则回退
                            EachLanguage = Locale.Fallback(EachLanguage)
                        End If
                    Loop Until EachLanguage = Nothing
                Next
            End If
            '完全失败
            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' 获取/设置与指定的语言对应的值。
    ''' </summary>
    ''' <param name="language">要获取的值的语言，在搜索时允许回退（fallback）。</param>
    ''' <returns>与指定的语言最匹配的值。如果未找到，则返回 <c>null</c>。</returns>
    Default Public Property Value(ByVal language As String) As TValue
        Get
            Return Me.Value({language})
        End Get
        Set(ByVal value As TValue)
            CheckSealed()
            If value Is Nothing Then
                LanguageDict.Remove(language)
            Else
                LanguageDict.Item(language) = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' 获取/设置与默认语言（<c>""</c>）对应的值。
    ''' </summary>
    Public Property InvariantValue() As TValue
        Get
            Return Me.Value({""})
        End Get
        Set(ByVal value As TValue)
            CheckSealed()
            If value Is Nothing Then
                LanguageDict.Remove("")
            Else
                LanguageDict.Item("") = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' 获取此值支持的语言数目。
    ''' </summary>
    Public ReadOnly Property Count As Integer
        Get
            Return LanguageDict.Count
        End Get
    End Property

    ''' <summary>
    ''' 获取一个值，指示此实例是否为空。
    ''' </summary>
    Public ReadOnly Property IsEmpty As Boolean
        Get
            Return LanguageDict.Count = 0
        End Get
    End Property

    ''' <summary>
    ''' 获取一个值，指示此实例的值是否被密封（不能修改）。
    ''' </summary>
    Public ReadOnly Property IsSealed As Boolean
        Get
            Return m_IsSealed
        End Get
    End Property

    ''' <summary>
    ''' 锁定此实例，使其无法更改。
    ''' </summary>
    Public Sub Seal()
        m_IsSealed = True
    End Sub

    ''' <summary>
    ''' 锁定此实例，使其无法更改。
    ''' </summary>
    ''' <param name="forceNewInstance">强制构造一个只读的包装新实例，即使此实例已经被密封。</param>
    Public Function AsSealed(forceNewInstance As Boolean) As MultiLanguageValue(Of TValue)
        If forceNewInstance OrElse Not m_IsSealed Then
            Return New MultiLanguageValue(Of TValue)(Me, True)
        Else
            Return Me
        End If
    End Function

    ''' <summary>
    ''' 锁定此实例，使其无法更改。
    ''' </summary>
    Public Function AsSealed() As MultiLanguageValue(Of TValue)
        Return AsSealed(False)
    End Function

    ''' <summary>
    ''' 获取此实例的文本表现形式。
    ''' </summary>
    Public Overrides Function ToString() As String
        If LanguageDict.Count = 0 Then
            Return ""
        Else
            Dim SValue As TValue = Nothing
            If LanguageDict.TryGetValue("", SValue) Then
                Return SValue.ToString
            Else
                Return LanguageDict.First.ToString
            End If
        End If
    End Function

    ''' <summary>
    ''' 获取与默认语言（<c>""</c>）对应的值。
    ''' </summary>
    Public Shared Narrowing Operator CType(ByVal x As MultiLanguageValue(Of TValue)) As TValue
        Return x.InvariantValue
    End Operator

    ''' <summary>
    ''' 构造一个与默认语言（<c>""</c>）对应的多语言值。
    ''' </summary>
    Public Shared Widening Operator CType(ByVal x As TValue) As MultiLanguageValue(Of TValue)
        Return New MultiLanguageValue(Of TValue)(x)
    End Operator

    Public Overrides Function GetHashCode() As Integer
        Return LanguageDict.GetHashCode
    End Function

    ''' <summary>
    ''' 返回一个能循环访问语言-值对的集合枚举器。
    ''' </summary>
    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of System.Collections.Generic.KeyValuePair(Of String, TValue)) Implements System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, TValue)).GetEnumerator
        Return If(LanguageDict Is Nothing, Enumerable.Empty(Of KeyValuePair(Of String, TValue))().GetEnumerator, LanguageDict.GetEnumerator)
    End Function

    ''' <summary>
    ''' 基础结构。返回一个能循环访问语言-值对的集合枚举器。
    ''' </summary>
    Private Function [_GetEnumerator]() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    ''' <summary>
    ''' 使用已存在的值构造一个多语言值。
    ''' </summary>
    ''' <param name="value">一个包含了多语言值的实例，如果为 <c>null</c>，则会构造一个空的多语言值。</param>
    ''' <param name="seal">指示是否应构造 <paramref name="value" /> 的只读包装，而不是将其中的值复制到新实例中。</param>
    Public Sub New(ByVal value As MultiLanguageValue(Of TValue), seal As Boolean)
        If seal Then
            If value Is Nothing Then
                LanguageDict = Empty.LanguageDict
            Else
                LanguageDict = value.LanguageDict
            End If
            Me.Seal()
        Else
            If value Is Nothing Then
                LanguageDict = New Dictionary(Of String, TValue)(CaseInsensitiveEqualityComparer.Default)
            Else
                LanguageDict = New Dictionary(Of String, TValue)(value.LanguageDict, CaseInsensitiveEqualityComparer.Default)
            End If
        End If
    End Sub

    ''' <summary>
    ''' 使用已存在的值构造一个多语言值。
    ''' </summary>
    ''' <param name="value">一个包含了多语言值的实例。此实例的值将会被一一复制。</param>
    Public Sub New(ByVal value As MultiLanguageValue(Of TValue))
        Me.New(value, False)
    End Sub

    ''' <summary>
    ''' 使用一个默认语言的值构造一个多语言值。
    ''' </summary>
    ''' <param name="invariantValue">与默认语言（<c>""</c>）对应的值。</param>
    Public Sub New(ByVal invariantValue As TValue)
        Me.New(Nothing, False)
        Me.InvariantValue = invariantValue
    End Sub

    ''' <summary>
    ''' 构造一个空的多语言值。
    ''' </summary>
    Public Sub New()
        Me.New(Nothing, False)
    End Sub

    Shared Sub New()
        Empty.Seal()
    End Sub
End Class