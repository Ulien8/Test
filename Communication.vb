Imports System.Diagnostics

Public Class Communication
    Implements IDisposable

#Region "API宣言"
    ''' <summary>
    ''' 1 つまたは複数のウィンドウへ、指定されたメッセージを送信します。この関数は、指定されたウィンドウのウィンドウプロシージャを呼び出し、そのウィンドウプロシージャがメッセージを処理し終わった後で、制御を返します。
    ''' </summary>
    ''' <param name="hWnd">送信先ウィンドウのハンドル</param>
    ''' <param name="MSG">メッセージ</param>
    ''' <param name="wParam">メッセージの最初のパラメータ</param>
    ''' <param name="lParam">コピーするデータの構造体</param>
    ''' <returns>ウィンドウのハンドル</returns>
    Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
        (ByVal hWnd As IntPtr, ByVal MSG As Integer, ByVal wParam As Integer, ByRef lParam As COPYDATASTRUCT) As IntPtr
    Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
        (ByVal hWnd As IntPtr, ByVal MSG As Integer, ByVal wParam As Integer, ByRef lParam As Boolean) As IntPtr
#End Region

#Region "定数"
    Public Enum MSG
        SendDesktopReference = &H10000
        ChangeCommander = &H20000
    End Enum
#End Region

#Region "構造体"
    ''' <summary>
    ''' 送信データ構造体
    ''' </summary>
    Public Structure COPYDATASTRUCT
        Dim dwData As Int32     '送信するビット値
        Dim cbData As Int32     'lpDataのバイト数
        Dim lpData As List(Of Desktop)    '送信するデータへのポインタ(0も可能)
    End Structure
#End Region

#Region "IDisposable"
    Public Sub Dispose()
        Dispose(True)

        GC.SuppressFinalize(Me)
    End Sub

    Public Overridable Sub Dispose(ByVal disposing As Boolean)

        For Each dsk As Desktop In Me.Desk.ToArray
            dsk.Dispose()
        Next

    End Sub

    Private Sub IDisposable_Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
#End Region


#Region "プロパティ"
    ''' <summary>
    ''' 現在の操作対象かどうかを取得、設定します
    ''' </summary>
    Private _commander As Boolean
    Public Property Commander() As Boolean
        Get
            Return _commander
        End Get
        Set(ByVal value As Boolean)
            _commander = value
        End Set
    End Property

    ''' <summary>
    ''' Desktopリストを取得、設定します
    ''' </summary>
    Private _desk As List(Of Desktop)
    Public Property Desk() As List(Of Desktop)
        Get
            Return _desk
        End Get
        Set(ByVal value As List(Of Desktop))
            _desk = value
        End Set
    End Property
#End Region

#Region "フィールド"
    ''' <summary>
    ''' 破棄されたか
    ''' </summary>
    Private _disposed As Boolean
#End Region

#Region "コンストラクタ"

    ''' <summary>
    ''' コンストラクタ。生成時に操作対象かを判定します
    ''' </summary>
    ''' <param name="createDesktop">ウィンドウを作成するか。省略した場合はFalse</param>
    Sub New(Optional ByVal createDesktop As Boolean = False)
        ' ぼっちか
        If IsAlone() Then
            Me.Commander = True
        Else
            Me.Commander = False
        End If
        _disposed = False
        If createDesktop Then
            Dim dsk As New Desktop()
            dsk.Create("Test")
            dsk.Prepare()
            Me.Desk.Add(dsk)
        End If
    End Sub


#End Region

    ''' <summary>
    ''' 同じプロセス名が存在しないかどうか
    ''' </summary>
    ''' <returns></returns>
    Private Function IsAlone() As Boolean
        ' 名前のプロセスが見つからなかったらおわり
        Dim otherProcesses() As Process = Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)
        If otherProcesses Is Nothing Then
            ' なかった
            Return True
        End If
        ' あった
        Return False
    End Function

    ''' <summary>
    ''' 司令塔と化します
    ''' </summary>
    Public Sub ImCommander()
        If IsAlone() Then
            Return
        End If

        Dim otherProcesses() As Process = SearchProcess(Process.GetCurrentProcess.ProcessName)
        If otherProcesses Is Nothing Then
            Return
        End If

        For Each oP As Process In otherProcesses
            Dim sendResult As Boolean = SendMessage(oP.Handle, MSG.ChangeCommander, 0, False)
        Next
    End Sub

    ''' <summary>
    ''' Desktopのオブジェクト参照を設定します
    ''' </summary>
    ''' <param name="desk"></param>
    Public Sub SetReference(ByRef desk As List(Of Desktop))
        If Me.Desk Is Nothing Then

        End If
        Me.Desk = desk
    End Sub

    ''' <summary>
    ''' プロセスを探し、自身のDesktopオブジェクトを参照させます
    ''' </summary>
    Public Sub ReferenceOther()
        If IsAlone() Then
            Return
        End If

        Dim cds As COPYDATASTRUCT
        cds.dwData = 0
        cds.lpData = Me.Desk
        cds.cbData = Len(cds.lpData)

        Dim otherProcesses() As Process = SearchProcess(Process.GetCurrentProcess.ProcessName)
        If otherProcesses Is Nothing Then
            Return
        End If

        For Each oP As Process In otherProcesses
            Dim sendResult As Boolean = SendMessage(oP.Handle, MSG.SendDesktopReference, 0, cds)
        Next
    End Sub

    ''' <summary>
    ''' 指定した名前のプロセスを探し取得します
    ''' </summary>
    ''' <param name="prcName"></param>
    ''' <returns></returns>
    Private Function SearchProcess(ByVal prcName As String) As Process()
        If IsAlone() Then
            Return Nothing
        End If
        Return Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)
    End Function


End Class
