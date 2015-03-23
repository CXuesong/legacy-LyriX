Imports System.Text

<TestClass()>
Public Class UnitTest1

    <TestMethod()>
    Public Sub TestMethod1()
        '在此处添加测试逻辑
        Dim PK As New LyriXPackage("j:\my files\visual studio 2010\Projects\LyriX\LyriX\Schema\package.zip")
        Dim Com As New LyriXCompiler
        Dim cd = Com.Compile(PK)
    End Sub

End Class
