Namespace Players.ObjectModel
    ''' <summary>
    ''' 用于为 <see cref="LyricsPlayer.PositionChanged" /> 提供数据。
    ''' </summary>
    Public Class PositionChangedEventArgs
        Inherits EventArgs

        Private m_OldPosition As TimeSpan

        ''' <summary>
        ''' 获取发生改变前音乐的播放位置。
        ''' </summary>
        Public ReadOnly Property OldPosition As TimeSpan
            Get
                Return m_OldPosition
            End Get
        End Property

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="oldPosition">发生改变前音乐的播放位置。</param>
        Public Sub New(ByVal oldPosition As TimeSpan)
            m_OldPosition = oldPosition
        End Sub
    End Class

    ''' <summary>
    ''' 用于处理歌词播放的逻辑。
    ''' </summary>
    Public MustInherit Class LyricsPlayer

        ''' <summary>
        ''' 当 <see cref="Position" /> 发生变化时引发。
        ''' </summary>
        Public Event PositionChanged(ByVal sender As Object, ByVal e As PositionChangedEventArgs)

        ''' <summary>
        ''' 当 <see cref="Document" /> 发生变化时引发。
        ''' </summary>
        Public Event DocumentChanged(sender As Object, e As EventArgs)

        ''' <summary>
        ''' 当 <see cref="Version" /> 发生变化时引发。
        ''' </summary>
        Public Event VersionChanged(sender As Object, e As EventArgs)

        Private m_Document As Compiled.LyricsDocument
        Private m_Version As Compiled.Version
        Private m_Duration As TimeSpan
        Private m_Position As TimeSpan
        Private m_ForwardPlaying As Boolean

        ''' <summary>
        ''' 获取/设置要载入的歌词文档。
        ''' </summary>
        ''' <remarks>如果更改此属性的值（不为 <c>null</c>），则会将 <see cref="Version" /> 设置为 <c>null</c>，并自动重新查找匹配的 <see cref="Compiled.Version" />。</remarks>
        Public Property Document As Compiled.LyricsDocument
            Get
                Return m_Document
            End Get
            Set(value As Compiled.LyricsDocument)
                If m_Document IsNot value Then
                    m_Document = value
                    '更改了 Document
                    OnDocumentChanged()
                    '需要重置(Version)
                    If m_Document IsNot Nothing Then
                        Version = Lyrics.MatchVersion(m_Duration)
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取要载入的歌词信息。
        ''' </summary>
        Public ReadOnly Property Lyrics As Compiled.Lyrics
            Get
                Return If(m_Document Is Nothing, Nothing, m_Document.Lyrics)
            End Get
        End Property

        ''' <summary>
        ''' 获取/设置要载入的歌词版本。
        ''' </summary>
        ''' <value>一个 <see cref="Compiled.Version" />，如果 <see cref="Lyrics" /> 不为 <c>null</c>，则必须为其中所包含的一个歌词版本。</value>
        Public Property Version As Compiled.Version
            Get
                Return m_Version
            End Get
            Set(ByVal value As Compiled.Version)
                If value IsNot m_Version Then
                    If value IsNot Nothing AndAlso
                        Lyrics IsNot Nothing AndAlso
                        Lyrics.Versions.IndexOf(value) < 0 Then
                        Throw New InvalidOperationException(ExceptionPrompts.VersionNotInList)
                    End If
                    m_Version = value
                    OnVersionChanged()
                    Reset()
                End If
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置待播放的音乐的时间长度。
        ''' </summary>
        ''' <remarks>此属性将作为匹配最佳歌词版本的一个依据。</remarks>
        Public Property Duration As TimeSpan
            Get
                Return m_Duration
            End Get
            Set(ByVal value As TimeSpan)
                m_Duration = value
            End Set
        End Property

        ''' <summary>
        ''' 获取/设置音乐此时的播放位置。
        ''' </summary>
        Public Property Position As TimeSpan
            Get
                Return m_Position
            End Get
            Set(ByVal value As TimeSpan)
                If m_Position <> value Then
                    Dim e As New PositionChangedEventArgs(m_Position)
                    m_Position = value
                    OnPositionChanged(e)
                End If
            End Set
        End Property

        ''' <summary>
        ''' 引发 <see cref="PositionChanged" />。
        ''' </summary>
        Protected Overridable Sub OnPositionChanged(ByVal e As PositionChangedEventArgs)
            RaiseEvent PositionChanged(Me, e)
        End Sub

        ''' <summary>
        ''' 引发 <see cref="VersionChanged" />。
        ''' </summary>
        Protected Overridable Sub OnVersionChanged()
            RaiseEvent VersionChanged(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' 引发 <see cref="DocumentChanged" />。
        ''' </summary>
        Protected Overridable Sub OnDocumentChanged()
            RaiseEvent DocumentChanged(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' 重置当前的状态（只有状态）。
        ''' </summary>
        Protected Overridable Sub Reset()
            m_Position = TimeSpan.Zero
        End Sub

        ''' <summary>
        ''' 初始化。
        ''' </summary>
        ''' <param name="document">要载入的歌词文档，可为 <c>null</c>。</param>
        ''' <param name="duration">待播放的音乐的时间长度。</param>
        ''' <param name="version">要载入的歌词版本，如果为 <c>null</c>，则表示将尝试自动匹配。</param>
        Public Sub New(document As Compiled.LyricsDocument, duration As TimeSpan, version As Compiled.Version)
            m_Duration = duration
            Me.Document = document
            If version IsNot Nothing Then Me.Version = version
            Reset()
        End Sub
    End Class
End Namespace