<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMenuPrincipal
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

    Private components As System.ComponentModel.IContainer

    Friend WithEvents PanelTop As System.Windows.Forms.Panel
    Friend WithEvents lblTituloTop As System.Windows.Forms.Label
    Friend WithEvents lblUsuarioTop As System.Windows.Forms.Label
    Friend WithEvents PanelLateral As System.Windows.Forms.Panel
    Friend WithEvents PanelContenido As System.Windows.Forms.Panel
    Friend WithEvents picLogoCentro As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PanelTop = New System.Windows.Forms.Panel()
        Me.lblTituloTop = New System.Windows.Forms.Label()
        Me.lblUsuarioTop = New System.Windows.Forms.Label()
        Me.PanelLateral = New System.Windows.Forms.Panel()
        Me.PanelContenido = New System.Windows.Forms.Panel()
        Me.picLogoCentro = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.PanelTop.SuspendLayout()
        CType(Me.picLogoCentro, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelTop
        '
        Me.PanelTop.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.PanelTop.Controls.Add(Me.PictureBox1)
        Me.PanelTop.Controls.Add(Me.lblTituloTop)
        Me.PanelTop.Controls.Add(Me.lblUsuarioTop)
        Me.PanelTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTop.Location = New System.Drawing.Point(0, 0)
        Me.PanelTop.Name = "PanelTop"
        Me.PanelTop.Size = New System.Drawing.Size(1000, 54)
        Me.PanelTop.TabIndex = 0
        '
        'lblTituloTop
        '
        Me.lblTituloTop.AutoSize = True
        Me.lblTituloTop.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTituloTop.ForeColor = System.Drawing.Color.White
        Me.lblTituloTop.Location = New System.Drawing.Point(58, 15)
        Me.lblTituloTop.Name = "lblTituloTop"
        Me.lblTituloTop.Size = New System.Drawing.Size(144, 25)
        Me.lblTituloTop.TabIndex = 0
        Me.lblTituloTop.Text = "Ultra Mecánica"
        '
        'lblUsuarioTop
        '
        Me.lblUsuarioTop.Dock = System.Windows.Forms.DockStyle.Right
        Me.lblUsuarioTop.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUsuarioTop.ForeColor = System.Drawing.Color.White
        Me.lblUsuarioTop.Location = New System.Drawing.Point(700, 0)
        Me.lblUsuarioTop.Name = "lblUsuarioTop"
        Me.lblUsuarioTop.Padding = New System.Windows.Forms.Padding(0, 20, 20, 0)
        Me.lblUsuarioTop.Size = New System.Drawing.Size(300, 54)
        Me.lblUsuarioTop.TabIndex = 1
        Me.lblUsuarioTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'PanelLateral
        '
        Me.PanelLateral.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.PanelLateral.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelLateral.Location = New System.Drawing.Point(0, 54)
        Me.PanelLateral.Name = "PanelLateral"
        Me.PanelLateral.Size = New System.Drawing.Size(200, 546)
        Me.PanelLateral.TabIndex = 1
        '
        'PanelContenido
        '
        Me.PanelContenido.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelContenido.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelContenido.Location = New System.Drawing.Point(200, 54)
        Me.PanelContenido.Name = "PanelContenido"
        Me.PanelContenido.Size = New System.Drawing.Size(800, 546)
        Me.PanelContenido.TabIndex = 2
        '
        'picLogoCentro
        '
        Me.picLogoCentro.Location = New System.Drawing.Point(0, 0)
        Me.picLogoCentro.Name = "picLogoCentro"
        Me.picLogoCentro.Size = New System.Drawing.Size(120, 120)
        Me.picLogoCentro.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogoCentro.TabIndex = 0
        Me.picLogoCentro.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.proyectofinalPNET.My.Resources.Resources.Gem_Gen_ImgLogo
        Me.PictureBox1.Location = New System.Drawing.Point(12, 7)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(40, 40)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        'FormMenuPrincipal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 600)
        Me.Controls.Add(Me.PanelContenido)
        Me.Controls.Add(Me.PanelLateral)
        Me.Controls.Add(Me.PanelTop)
        Me.Name = "FormMenuPrincipal"
        Me.Text = "FormMenuPrincipal"
        Me.PanelTop.ResumeLayout(False)
        Me.PanelTop.PerformLayout()
        CType(Me.picLogoCentro, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
End Class
