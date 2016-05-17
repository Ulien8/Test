<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmbDesktop = New System.Windows.Forms.ComboBox()
        Me.btnChange = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.txtDesktopName = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'cmbDesktop
        '
        Me.cmbDesktop.FormattingEnabled = True
        Me.cmbDesktop.Location = New System.Drawing.Point(13, 13)
        Me.cmbDesktop.Name = "cmbDesktop"
        Me.cmbDesktop.Size = New System.Drawing.Size(164, 20)
        Me.cmbDesktop.TabIndex = 0
        '
        'btnChange
        '
        Me.btnChange.Location = New System.Drawing.Point(197, 11)
        Me.btnChange.Name = "btnChange"
        Me.btnChange.Size = New System.Drawing.Size(89, 23)
        Me.btnChange.TabIndex = 1
        Me.btnChange.Text = "デスクトップ変更"
        Me.btnChange.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(197, 44)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(89, 23)
        Me.btnAdd.TabIndex = 2
        Me.btnAdd.Text = "デスクトップ追加"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'txtDesktopName
        '
        Me.txtDesktopName.Location = New System.Drawing.Point(13, 47)
        Me.txtDesktopName.Name = "txtDesktopName"
        Me.txtDesktopName.Size = New System.Drawing.Size(164, 19)
        Me.txtDesktopName.TabIndex = 3
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(417, 81)
        Me.Controls.Add(Me.txtDesktopName)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.btnChange)
        Me.Controls.Add(Me.cmbDesktop)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmbDesktop As ComboBox
    Friend WithEvents btnChange As Button
    Friend WithEvents btnAdd As Button
    Friend WithEvents txtDesktopName As TextBox
End Class
