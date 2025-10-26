<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLogin
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtUsuario = New System.Windows.Forms.TextBox()
        Me.txtContraseña = New System.Windows.Forms.TextBox()
        Me.btnEntrar = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel_B_Pass = New System.Windows.Forms.Panel()
        Me.Panel_B_Usu = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Panel1.SuspendLayout()
        Me.Panel_B_Pass.SuspendLayout()
        Me.Panel_B_Usu.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 13.0!)
        Me.Label2.Location = New System.Drawing.Point(0, 150)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(400, 28)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Iniciar Sesión"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtUsuario
        '
        Me.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtUsuario.Font = New System.Drawing.Font("Segoe UI", 12.5!)
        Me.txtUsuario.Location = New System.Drawing.Point(2, 2)
        Me.txtUsuario.Multiline = True
        Me.txtUsuario.Name = "txtUsuario"
        Me.txtUsuario.Size = New System.Drawing.Size(300, 25)
        Me.txtUsuario.TabIndex = 0
        '
        'txtContraseña
        '
        Me.txtContraseña.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtContraseña.Font = New System.Drawing.Font("Segoe UI", 12.5!)
        Me.txtContraseña.Location = New System.Drawing.Point(2, 2)
        Me.txtContraseña.Multiline = True
        Me.txtContraseña.Name = "txtContraseña"
        Me.txtContraseña.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtContraseña.Size = New System.Drawing.Size(300, 25)
        Me.txtContraseña.TabIndex = 1
        '
        'btnEntrar
        '
        Me.btnEntrar.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnEntrar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnEntrar.FlatAppearance.BorderSize = 0
        Me.btnEntrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue
        Me.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnEntrar.Font = New System.Drawing.Font("Segoe UI", 12.75!, System.Drawing.FontStyle.Bold)
        Me.btnEntrar.ForeColor = System.Drawing.Color.White
        Me.btnEntrar.Location = New System.Drawing.Point(75, 317)
        Me.btnEntrar.Name = "btnEntrar"
        Me.btnEntrar.Size = New System.Drawing.Size(250, 38)
        Me.btnEntrar.TabIndex = 2
        Me.btnEntrar.Text = "Entrar ➡"
        Me.btnEntrar.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Panel_B_Pass)
        Me.Panel1.Controls.Add(Me.Panel_B_Usu)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.btnEntrar)
        Me.Panel1.Location = New System.Drawing.Point(200, 90)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(400, 420)
        Me.Panel1.TabIndex = 5
        '
        'Panel_B_Pass
        '
        Me.Panel_B_Pass.BackColor = System.Drawing.Color.DodgerBlue
        Me.Panel_B_Pass.Controls.Add(Me.txtContraseña)
        Me.Panel_B_Pass.Location = New System.Drawing.Point(48, 258)
        Me.Panel_B_Pass.Name = "Panel_B_Pass"
        Me.Panel_B_Pass.Size = New System.Drawing.Size(304, 29)
        Me.Panel_B_Pass.TabIndex = 6
        '
        'Panel_B_Usu
        '
        Me.Panel_B_Usu.BackColor = System.Drawing.Color.DodgerBlue
        Me.Panel_B_Usu.Controls.Add(Me.txtUsuario)
        Me.Panel_B_Usu.Location = New System.Drawing.Point(48, 208)
        Me.Panel_B_Usu.Name = "Panel_B_Usu"
        Me.Panel_B_Usu.Size = New System.Drawing.Size(304, 29)
        Me.Panel_B_Usu.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 120)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(400, 32)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Ultra Mecánica"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.proyectofinalPNET.My.Resources.Resources.Gem_Gen_ImgLogo
        Me.PictureBox1.Location = New System.Drawing.Point(160, 30)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(80, 80)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'FormLogin
        '
        Me.AcceptButton = Me.btnEntrar
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.proyectofinalPNET.My.Resources.Resources.GemGeneratedImg
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "FormLogin"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Ultra Mecánica - Iniciar Sesión"
        Me.Panel1.ResumeLayout(False)
        Me.Panel_B_Pass.ResumeLayout(False)
        Me.Panel_B_Pass.PerformLayout()
        Me.Panel_B_Usu.ResumeLayout(False)
        Me.Panel_B_Usu.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Label2 As Label
    Friend WithEvents txtUsuario As TextBox
    Friend WithEvents txtContraseña As TextBox
    Friend WithEvents btnEntrar As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel_B_Usu As Panel
    Friend WithEvents Panel_B_Pass As Panel
End Class
