Public Class Form1

    Private _comm As Communication

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _comm = New Communication(True)

        For Each dsk As Desktop In _comm.Desk.ToArray
            Me.cmbDesktop.Items.Add(dsk.DesktopName)
        Next
        _comm.ReferenceOther()
    End Sub

    ' desktopの参照を取らせる
    ' ComboBoxに反映
    ' Me.cmbDesktop.Items.Add()


    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case Communication.MSG.SendDesktopReference
                Dim cds As New Communication.COPYDATASTRUCT
                cds = CType(m.GetLParam(cds.GetType), Communication.COPYDATASTRUCT)
                _comm.SetReference(cds.lpData)
            Case Communication.MSG.ChangeCommander
                Dim bl As Boolean
                _comm.Commander = CType(m.GetLParam(bl.GetType), Boolean)
        End Select

        MyBase.WndProc(m)
    End Sub
End Class
