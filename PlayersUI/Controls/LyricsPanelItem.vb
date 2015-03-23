Option Compare Text

Imports System.ComponentModel

Namespace Controls
    ''' <summary>
    ''' 表示歌词面板中的一项。
    ''' </summary>
    Public Class LyricsPanelItem
        Implements INotifyPropertyChanged

        ''' <summary>
        ''' 在更改属性值时发生。
        ''' </summary>
        Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Private m_DataSource As Object

        ''' <summary>
        ''' 获取/设置此项的数据源。
        ''' </summary>
        Public Property DataSource As Object
            Get
                Return m_DataSource
            End Get
            Set(value As Object)
                If m_DataSource IsNot value Then
                    m_DataSource = value
                    OnPropertyChanged("DataSource")
                End If
            End Set
        End Property

        ''' <summary>
        ''' 引发 <see cref="PropertyChanged" /> 事件。
        ''' </summary>
        ''' <param name="propertyName">发生变化的属性名称，若为 <see cref="String.Empty" /> 或为 <c>null</c>，则表示该对象上的所有属性都已更改。</param>
        Protected Overridable Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Public Sub New(dataSource As Object)
            m_DataSource = dataSource
        End Sub
    End Class

    ''' <summary>
    ''' 表示歌词面板中的一个歌词行。
    ''' </summary>
    Public Class LyricsLine
        Inherits LyricsPanelItem

        Private m_SpanIndex As Integer = -1
        Private m_SegmentIndex As Integer = -1
        Private m_SegmentProgress As Double = 0

        ''' <summary>
        ''' 获取/设置此项的数据源。
        ''' </summary>
        Public Shadows Property DataSource As Compiled.Line
            Get
                Return DirectCast(MyBase.DataSource, Compiled.Line)
            End Get
            Set(value As Compiled.Line)
                MyBase.DataSource = value
            End Set
        End Property

        Public Property SpanIndex As Integer
            Get
                Return m_SpanIndex
            End Get
            Set(value As Integer)
                If m_SpanIndex <> value Then
                    m_SpanIndex = value
                    OnPropertyChanged("SpanIndex")
                End If
            End Set
        End Property

        Public Property SegmentIndex As Integer
            Get
                Return m_SegmentIndex
            End Get
            Set(value As Integer)
                If m_SegmentIndex <> value Then
                    m_SegmentIndex = value
                    OnPropertyChanged("SegmentIndex")
                End If
            End Set
        End Property

        Public Property SegmentProgress As Double
            Get
                Return m_SegmentProgress
            End Get
            Set(value As Double)
                If m_SegmentProgress <> value Then
                    m_SegmentProgress = value
                    OnPropertyChanged("SegmentProgress")
                End If
            End Set
        End Property

        Public Sub SetProgress(spanIndex As Integer, segmentIndex As Integer, segmentProgress As Double)
            m_SpanIndex = spanIndex
            m_SegmentIndex = segmentIndex
            m_SegmentProgress = segmentProgress
            OnPropertyChanged(Nothing)
        End Sub

        Public Sub ClearProgress()
            SetProgress(0, 0, 0)
        End Sub

        Public Sub FillProgress()
            SegmentProgress = Double.PositiveInfinity
        End Sub

        Public Sub New(dataSource As Compiled.Line)
            MyBase.New(dataSource)
        End Sub
    End Class

    Public Class LyricsVersionHint
        Inherits LyricsPanelItem

        ''' <summary>
        ''' 获取/设置此项的数据源。
        ''' </summary>
        Public Shadows Property DataSource As Compiled.Version
            Get
                Return DirectCast(MyBase.DataSource, Compiled.Version)
            End Get
            Set(value As Compiled.Version)
                MyBase.DataSource = value
            End Set
        End Property

        Public Sub New(dataSource As Compiled.Version)
            MyBase.New(dataSource)
        End Sub
    End Class

    'Public Class LyricsPartHint
    '    Inherits LyricsPanelItem

    '    ''' <summary>
    '    ''' 获取/设置此项的数据源。
    '    ''' </summary>
    '    Public Shadows Property DataSource As Compiled.Track
    '        Get
    '            Return DirectCast(MyBase.DataSource, Compiled.Track)
    '        End Get
    '        Set(value As Compiled.Track)
    '            MyBase.DataSource = value
    '        End Set
    '    End Property

    '    Protected Overrides Sub OnPropertyChanged(propertyName As String)
    '        MyBase.OnPropertyChanged(propertyName)
    '        If propertyName = Nothing OrElse propertyName = "DataSource" Then
    '            OnPropertyChanged("ArtistItems")
    '        End If
    '    End Sub

    '    Public Sub New(dataSource As Compiled.Track)
    '        MyBase.New(dataSource)
    '    End Sub
    'End Class

    ''' <summary>
    ''' 用于表示艺术家列表。
    ''' </summary>
    Public Class LyricsArtistsHint
        Inherits LyricsPanelItem

        ''' <summary>
        ''' 获取/设置此项的数据源。
        ''' </summary>
        Public Shadows Property DataSource As IList(Of Compiled.ArtistBase)
            Get
                Return DirectCast(MyBase.DataSource, IList(Of Compiled.ArtistBase))
            End Get
            Set(value As IList(Of Compiled.ArtistBase))
                MyBase.DataSource = value
            End Set
        End Property

        ''' <summary>
        ''' 获取此艺术家列表的 <see cref="LyricsArtistItem" /> 封装。
        ''' </summary>
        Public ReadOnly Property ArtistItems As IEnumerable(Of LyricsArtistItem)
            Get
                If DataSource Is Nothing Then Return Enumerable.Empty(Of LyricsArtistItem)()
                Return From EachArtist In DataSource
                        Select New LyricsArtistItem(EachArtist)
            End Get
        End Property

        Public Sub New(dataSource As IList(Of Compiled.ArtistBase))
            MyBase.New(dataSource)
        End Sub
    End Class

    ''' <summary>
    ''' 表示列表中的艺术家项。
    ''' </summary>
    Public Class LyricsArtistItem
        Inherits LyricsPanelItem

        ''' <summary>
        ''' 获取/设置此项的数据源。
        ''' </summary>
        Public Shadows Property DataSource As Compiled.ArtistBase
            Get
                Return DirectCast(MyBase.DataSource, Compiled.ArtistBase)
            End Get
            Set(value As Compiled.ArtistBase)
                MyBase.DataSource = value
            End Set
        End Property

        ''' <summary>
        ''' 获取一个值，指示此艺术家是否为一个群组。
        ''' </summary>
        Public ReadOnly Property IsArtistGroup As Boolean
            Get
                Return If(DataSource Is Nothing, False, TypeOf DataSource Is Compiled.ArtistGroup)
            End Get
        End Property

        Protected Overrides Sub OnPropertyChanged(propertyName As String)
            MyBase.OnPropertyChanged(propertyName)
            If propertyName = Nothing OrElse propertyName = "DataSource" Then
                OnPropertyChanged("IsArtistGroup")
            End If
        End Sub

        Public Sub New(dataSource As Compiled.ArtistBase)
            MyBase.New(dataSource)
        End Sub
    End Class

    Public Class MusicInfoHint
        Inherits LyricsPanelItem

        ''' <summary>
        ''' 获取/设置此项的数据源。
        ''' </summary>
        Public Shadows Property DataSource As Compiled.MusicInfo
            Get
                Return DirectCast(MyBase.DataSource, Compiled.MusicInfo)
            End Get
            Set(value As Compiled.MusicInfo)
                MyBase.DataSource = value
            End Set
        End Property

        ''' <summary>
        ''' 获取独立的艺术家列表项，其中排除了已在群组中出现的艺术家。
        ''' </summary>
        Public ReadOnly Property ArtistItems As IEnumerable(Of LyricsArtistItem)
            Get
                If DataSource Is Nothing Then Return Enumerable.Empty(Of LyricsArtistItem)()
                Dim ArtistsInGroup = DataSource.Artists.SelectMany(
                    Function(EachItem) If(TypeOf EachItem Is Compiled.ArtistGroup, DirectCast(EachItem, Compiled.ArtistGroup).Artists, {}))
                Return From EachArtist In DataSource.Artists.Except(ArtistsInGroup)
                       Select New LyricsArtistItem(EachArtist)
            End Get
        End Property

        Protected Overrides Sub OnPropertyChanged(propertyName As String)
            MyBase.OnPropertyChanged(propertyName)
            If propertyName = Nothing OrElse propertyName = "DataSource" Then
                OnPropertyChanged("ArtistItems")
            End If
        End Sub

        Public Sub New(dataSource As Compiled.MusicInfo)
            MyBase.New(dataSource)
        End Sub
    End Class
End Namespace