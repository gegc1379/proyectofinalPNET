Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class FormCrearCliente
    Inherits Form

    Public Property NuevoRut As String = String.Empty

    Private txtRut As TextBox
    Private txtNombre As TextBox
    Private btnGuardar As Button
    Private btnCancelar As Button

    Public Sub New()
        Me.Text = "Crear Cliente"
        Me.Size = New Drawing.Size(420, 220)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Dim lblRut As New Label With {.Text = "RUT:", .Location = New Drawing.Point(12, 15), .AutoSize = True}
        txtRut = New TextBox With {.Location = New Drawing.Point(12, 35), .Width = 370}

        Dim lblNombre As New Label With {.Text = "Nombre (opcional):", .Location = New Drawing.Point(12, 70), .AutoSize = True}
        txtNombre = New TextBox With {.Location = New Drawing.Point(12, 90), .Width = 370}

        btnGuardar = New Button With {.Text = "Guardar", .Location = New Drawing.Point(12, 130), .Width = 120, .BackColor = Drawing.Color.FromArgb(40, 167, 69), .ForeColor = Drawing.Color.White}
        btnCancelar = New Button With {.Text = "Cancelar", .Location = New Drawing.Point(140, 130), .Width = 120}

        AddHandler btnGuardar.Click, AddressOf BtnGuardar_Click
        AddHandler btnCancelar.Click, AddressOf BtnCancelar_Click

        Me.Controls.AddRange(New Control() {lblRut, txtRut, lblNombre, txtNombre, btnGuardar, btnCancelar})
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs)
        Dim rutVal As String = If(txtRut IsNot Nothing, txtRut.Text.Trim(), String.Empty)
        Dim nombreVal As String = If(txtNombre IsNot Nothing, txtNombre.Text.Trim(), String.Empty)

        If String.IsNullOrEmpty(rutVal) Then
            MessageBox.Show("El RUT es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtRut.Focus()
            Return
        End If

        Try
            Dim conn As MySqlConnection = ModuloConexion.GetConexion()
            If conn Is Nothing Then
                MessageBox.Show("No se pudo conectar a la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Verificar existencia
            Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM clientes WHERE Rut = @rut", conn)
                cmdCheck.Parameters.AddWithValue("@rut", rutVal)
                Dim existsCount As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())
                If existsCount > 0 Then
                    MessageBox.Show("Ya existe un cliente con ese RUT.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End Using

            ' Intentar insertar (columnas mínimas: Rut, Nombre) -- si la tabla requiere más columnas, la BD devolverá error
            Using cmdIns As New MySqlCommand("INSERT INTO clientes (Rut, Nombre) VALUES (@rut, @nombre)", conn)
                cmdIns.Parameters.AddWithValue("@rut", rutVal)
                cmdIns.Parameters.AddWithValue("@nombre", nombreVal)
                cmdIns.ExecuteNonQuery()
            End Using

            MessageBox.Show("Cliente creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.NuevoRut = rutVal
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Error al crear cliente: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Try
                ModuloConexion.Desconectar()
            Catch
            End Try
        End Try
    End Sub

End Class