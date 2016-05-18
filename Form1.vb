Imports System.Runtime.InteropServices

Public Class SettingForm

    Implements IDisposable

    ' 主にDesktopクラスを管理するやつ
    Private _comm As Communication

    ''' <summary>
    ''' 自分のアプリケーションパス
    ''' </summary>
    Private _exePath As String = System.IO.Path.Combine(Application.StartupPath, My.Application.Info.Title & ".exe")


    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        _comm = New Communication()

        ' 現在あるデスクトップを取得する
        For Each dskName As String In Desktop.GetDesktops
            If Desktop.Exists(dskName) Then
                Dim dsk As New Desktop
                dsk.Open(dskName)
                _comm.Add(dsk)
            End If
        Next

        ' リストに現在あるデスクトップを入れる
        Me.cmbScreenSelect.Items.AddRange(Desktop.GetDesktops)
    End Sub

    ''' <summary>
    ''' メッセージ受信
    ''' </summary>
    ''' <param name="m"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case Communication.MSG.SendDesktopReference
                ' 現在あるすべてのデスクトップを取得しろ
                _comm.SetReference()
                ListRefresh()
                Debug.WriteLine("デスクトップ確認")
            Case Communication.MSG.ChangeCommander
                ' 司令塔変更命令がきた
                Dim bl As Boolean
                _comm.Commander = CType(m.GetLParam(bl.GetType), Boolean)
                Debug.WriteLine("人事異動が発生しました")
        End Select
        MyBase.WndProc(m)
    End Sub

    ''' <summary>
    ''' 追加処理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        ' 未入力防止
        If String.IsNullOrEmpty(Me.txtScreenName.Text) Then
            MessageBox.Show("新しく作成するデスクトップの名前を入力してください", "エラー")
            Return
        End If

        ' デスクトップ追加処理
        Dim dsk As New Desktop
        dsk.Create(Me.txtScreenName.Text)
        dsk.Prepare()
        dsk.CreateProcess(_exePath)
        _comm.Add(dsk)

        ' ComboBox更新
        ListRefresh()
        ' TextBox内容削除
        Me.txtScreenName.Clear()
        ' 他プロセスにデスクトップリストの更新をさせる
        _comm.ReferenceOther()
    End Sub

    ''' <summary>
    ''' 変更処理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnChange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChange.Click
        If Me.cmbScreenSelect.SelectedIndex <= -1 Then
            MessageBox.Show("何か選択してください", "項目選択エラー")
            Return
        End If
        ' 探して表示する
        For Each dsk As Desktop In _comm.Desk.ToArray
            If Me.cmbScreenSelect.SelectedItem.ToString = dsk.DesktopName Then
                dsk.Show()
            End If
        Next
        ' ComboBox更新
        ListRefresh()
        ' 他プロセスにデスクトップリストの更新をさせる
        _comm.ReferenceOther()
    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        If Me.cmbScreenSelect.SelectedIndex < 0 Then
            MessageBox.Show("何か選択してください", "項目選択エラー")
            Return
        End If
        ' 探して削除
        For Each dsk As Desktop In _comm.Desk.ToArray
            If Me.cmbScreenSelect.SelectedItem.ToString = dsk.DesktopName And _
                Me.cmbScreenSelect.SelectedIndex.ToString <> Desktop.OpenDefaultDesktop.DesktopName Then
                ' プロセスも閉じる
                _comm.WindowCloser(dsk)

                ' 削除しようとしているデスクトップと表示しているデスクトップが一緒なら
                ' デフォルトを表示する
                If dsk.DesktopName = Desktop.GetCurrent.DesktopName Then
                    Desktop.Show(Desktop.OpenDefaultDesktop.DesktopName)
                End If
                ' デスクトップを閉じる
                dsk.Close()
            End If
        Next
        ListRefresh()
        Me.txtScreenName.Clear()
    End Sub

    Private Sub ListRefresh()
        ' リストに現在あるデスクトップを入れる
        With Me.cmbScreenSelect.Items
            .Clear()
            .AddRange(Desktop.GetDesktops)
        End With
    End Sub

    Private Sub SettingForm_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Desktop.Show(Desktop.OpenDefaultDesktop.DesktopName)
    End Sub
End Class
