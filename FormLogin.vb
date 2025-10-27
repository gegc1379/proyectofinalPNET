Imports MySql.Data.MySqlClient

Public Class FormLogin

    Const Placeholder_Usu As String = "👤  Correo"
    Const Placeholder_Pass As String = "🔒  Contraseña"

    Private Sub FormLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Placeholder(txtUsuario, Placeholder_Usu)
        Placeholder(txtContraseña, Placeholder_Pass)

    End Sub
    Private Sub btnEntrar_Click(sender As Object, e As EventArgs) Handles btnEntrar.Click
        Dim correo As String = If(txtUsuario.Text.Trim() = Placeholder_Usu, "", txtUsuario.Text.Trim())
        Dim contraseña As String = If(txtContraseña.Text.Trim() = Placeholder_Pass, "", txtContraseña.Text.Trim())

        If String.IsNullOrEmpty(correo) OrElse String.IsNullOrEmpty(contraseña) Then
            MessageBox.Show("Por favor, ingrese correo y contraseña.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim conn As MySqlConnection = ModuloConexion.GetConexion()
        If conn Is Nothing Then Return

        Try
            ' Obtener el perfil (Tipo) del usuario si las credenciales son correctas
            Dim query As String = "SELECT `Tipo` FROM usuarios WHERE Correo = @correo AND Contraseña = @pass LIMIT 1"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@correo", correo)
                cmd.Parameters.AddWithValue("@pass", contraseña)

                Dim objTipo = cmd.ExecuteScalar()

                If objTipo IsNot Nothing AndAlso Not Convert.IsDBNull(objTipo) Then
                    Dim tipo As String = objTipo.ToString()
                    Dim menuPrincipal As New FormMenuPrincipal(tipo, correo)
                    menuPrincipal.Show()
                    Me.Hide()
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

    Private Sub Placeholder(ByVal txt As TextBox, ByVal texto As String)
        If String.IsNullOrWhiteSpace(txt.Text) Then
            txt.Text = texto
            txt.ForeColor = Color.Gray
            If txt.Name = "txtContraseña" Then
                txt.PasswordChar = CChar(vbNullChar)
            End If
        End If
    End Sub

    Private Sub QuitarPlaceholder(ByVal txt As TextBox, ByVal texto As String)
        If txt.Text = texto Then
            txt.Text = ""
            txt.ForeColor = Color.Black
            If txt.Name = "txtContraseña" Then
                txt.PasswordChar = CChar("*")
            End If
        End If
    End Sub

    Private Sub txtUsuario_Enter(sender As Object, e As EventArgs) Handles txtUsuario.Enter
        QuitarPlaceholder(txtUsuario, Placeholder_Usu)
    End Sub

    Private Sub txtUsuario_Leave(sender As Object, e As EventArgs) Handles txtUsuario.Leave
        Placeholder(txtUsuario, Placeholder_Usu)
    End Sub

    Private Sub txtContraseña_Enter(sender As Object, e As EventArgs) Handles txtContraseña.Enter
        QuitarPlaceholder(txtContraseña, Placeholder_Pass)
    End Sub

    Private Sub txtContraseña_Leave(sender As Object, e As EventArgs) Handles txtContraseña.Leave
        Placeholder(txtContraseña, Placeholder_Pass)
    End Sub

End Class
