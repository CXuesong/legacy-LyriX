﻿'------------------------------------------------------------------------------
' <auto-generated>
'     此代码由工具生成。
'     运行时版本:4.0.30319.239
'
'     对此文件的更改可能会导致不正确的行为，并且如果
'     重新生成代码，这些更改将会丢失。
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources.Ind
    
    '此类是由 StronglyTypedResourceBuilder
    '类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    '若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    '(以 /str 作为命令选项)，或重新生成 VS 项目。
    '''<summary>
    '''  一个强类型的资源类，用于查找本地化的字符串等。
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Class Prompts
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  返回此类使用的缓存的 ResourceManager 实例。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("LyriX.Prompts", GetType(Prompts).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  使用此强类型资源类，为所有资源查找
        '''  重写当前线程的 CurrentUICulture 属性。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  查找类似 {1}（CV {0}） 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property ArtistC() As String
            Get
                Return ResourceManager.GetString("ArtistC", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}（{1}） 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property AuthorC() As String
            Get
                Return ResourceManager.GetString("AuthorC", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 LyriX 歌词文挡（*.lrcx）|*.lrcx 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property LyriXFileFilter() As String
            Get
                Return ResourceManager.GetString("LyriXFileFilter", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}（{1}） 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property MusicInfoA() As String
            Get
                Return ResourceManager.GetString("MusicInfoA", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}（{1} #{2:d}） 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property MusicInfoAT() As String
            Get
                Return ResourceManager.GetString("MusicInfoAT", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0} 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property MusicInfoIL() As String
            Get
                Return ResourceManager.GetString("MusicInfoIL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}（{1}） 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property MusicInfoILA() As String
            Get
                Return ResourceManager.GetString("MusicInfoILA", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}（{1} #{2:d}） 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property MusicInfoILAT() As String
            Get
                Return ResourceManager.GetString("MusicInfoILAT", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}
        '''源：{1}({2})。 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property ReaderOutput() As String
            Get
                Return ResourceManager.GetString("ReaderOutput", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}({1}) 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property SegmentA() As String
            Get
                Return ResourceManager.GetString("SegmentA", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}[{2}] 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property SegmentAL() As String
            Get
                Return ResourceManager.GetString("SegmentAL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}[{1}] 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property SegmentL() As String
            Get
                Return ResourceManager.GetString("SegmentL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 {0}：{1} 的本地化字符串。
        '''</summary>
        Friend Shared ReadOnly Property Version() As String
            Get
                Return ResourceManager.GetString("Version", resourceCulture)
            End Get
        End Property
    End Class
End Namespace