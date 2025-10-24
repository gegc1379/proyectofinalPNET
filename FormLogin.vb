Imports MySql.Data.MySqlClient

Public Class FormLogin
    Private Sub btnIngresar_Click(sender As Object, e As EventArgs) Handles btnIngresar.Click
        Dim correo As String = txtCorreo.Text.Trim()
        Dim contraseña As String = txtContraseña.Text.Trim()

        If String.IsNullOrEmpty(correo) OrElse String.IsNullOrEmpty(contraseña) Then
            MessageBox.Show("Por favor, ingrese correo y contraseña.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim conn As MySqlConnection = ModuloConexion.GetConexion()
        If conn Is Nothing Then Return

        Try
            Dim query As String = "SELECT COUNT(*) FROM usuarios WHERE Correo = @correo AND Contraseña = @pass"

            Using cmd As New MySqlCommand(query, conn)

                cmd.Parameters.AddWithValue("@correo", correo)
                cmd.Parameters.AddWithValue("@pass", contraseña)

                Dim resultado As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                If resultado > 0 Then
                    MessageBox.Show("¡Bienvenido!", "Login Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Else
                    MessageBox.Show("Correo o contraseña incorrectos.", "Error de Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error al intentar iniciar sesión: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ModuloConexion.Desconectar()
        End Try
    End Sub
End Class
