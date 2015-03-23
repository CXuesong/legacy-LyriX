''' <summary>
''' 提供可为 <c>null</c> 的值与原值/字符串之间的转换。
''' </summary>
''' <!--<ValueConversion(GetType(Nullable(Of T)), GetType(String))>-->
Friend NotInheritable Class NullableValueConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing Then Return If(parameter, Prompts.NullHint)
        '格式化交给 BindingExpression
        Return value
    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        If value Is Nothing Then Return Nothing 'Nothing -> Nothing
        If TypeOf value Is String Then
            If CStr(value) = Prompts.NullHint Then Return Nothing '[未指定] -> Nothing
            If targetType IsNot GetType(String) AndAlso
                CStr(value) = Nothing Then Return Nothing 'Empty(Not String) -> Nothing
        End If
        '转换值
        If targetType Is GetType(Object) Then Return value
        Try
            'TypeConverter 方便、高效
            Dim Converter = ComponentModel.TypeDescriptor.GetConverter(targetType)
            If Converter.CanConvertFrom(value.GetType) Then
                Return Converter.ConvertFrom(value)
            End If
        Finally
        End Try
        Return DependencyProperty.UnsetValue
    End Function
End Class

''' <summary>
''' 用于将一个枚举类型转换为枚举值的列表。
''' </summary>
<ValueConversion(GetType(Type), GetType([Enum]()), ParameterType:=GetType(String))>
Friend NotInheritable Class EnumValuesConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim tValue = DirectCast(value, Type)
        If Not tValue.IsEnum Then
            Throw New ArgumentException("value")
        ElseIf String.Equals("z", CStr(parameter), StringComparison.OrdinalIgnoreCase) OrElse
            Attribute.GetCustomAttribute(tValue, GetType(FlagsAttribute)) Is Nothing Then
            '使用 Compare 避免产生 null 异常
            '包含零值
            Return [Enum].GetValues(tValue).Cast(Of [Enum]).ToArray
        Else
            '不包含零值
            Return Aggregate EachItem In [Enum].GetValues(tValue)
                   Where System.Convert.ToUInt64(EachItem) > 0
                   Select DirectCast(EachItem, [Enum])
                   Into ToArray()
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

<ValueConversion(GetType(Object), GetType(String))>
Friend NotInheritable Class FriendlyNameConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing Then Return Prompts.NullHint
        Return Utility.Locale.GetFriendlyName(value)
    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

''' <summary>
''' 将 LocalizedPackageParts 转换为本地化的完成率。
''' </summary>
<ValueConversion(GetType(Document.LocalizedPackageParts), GetType(Double))>
Friend Class LocalizationCompletionRateConverter
    Implements IValueConverter

    'FUTURE 优化
    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim lpp = TryCast(value, Document.LocalizedPackageParts)
        If lpp IsNot Nothing Then
            Dim pk = lpp.FindPackage
            If pk IsNot Nothing Then
                Dim Loc = Aggregate EachItem In lpp.Descendants
                          Where TypeOf EachItem Is Document.LocalizedArtist OrElse
                                TypeOf EachItem Is Document.LocalizedLine
                          Into Count()
                Dim Unloc = Aggregate EachItem In pk.Descendants
                            Where TypeOf EachItem Is Document.ArtistBase OrElse
                                    TypeOf EachItem Is Document.LineBase
                            Into Count()

                Return Loc / Unloc
            End If
        End If
        Return DependencyProperty.UnsetValue
    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class

' ''' <summary>
' ''' 提供段与字符串之间的转换。
' ''' </summary>
'<ValueConversion(GetType(Document.Span), GetType(String))>
'Public Class SpanStringConverter
'    Implements IValueConverter

'    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
'        If TypeOf value Is Document.Span Then
'            Return String.Join(Nothing, From EachSegment In DirectCast(value, Document.Span).Segments
'                                        Select EachSegment.Text)
'        End If
'        Return DependencyProperty.UnsetValue
'    End Function

'    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
'        If TypeOf value Is String Then
'            Return New Document.Span(SplitText(CStr(value)))
'        End If
'        Return DependencyProperty.UnsetValue
'    End Function
'End Class