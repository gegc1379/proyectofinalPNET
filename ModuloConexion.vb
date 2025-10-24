Imports MySql.Data.MySqlClient
Imports System.Data

Module ModuloConexion

    Private connectionString As String = "Server=localhost;Database=taller;User ID='root';Password='';"

    Private conexion As MySqlConnection
    Public Function GetConexion() As MySqlConnection
        Try
            If conexion Is Nothing OrElse conexion.State = ConnectionState.Closed Then
                conexion = New MySqlConnection(connectionString)
                conexion.Open()
            End If
            Return conexion
        Catch ex As MySqlException
            MessageBox.Show("Error al conectar a la base de datos: " & ex.Message, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    Public Sub Desconectar()
        Try
            If conexion IsNot Nothing AndAlso conexion.State = ConnectionState.Open Then
                conexion.Close()
            End If
        Catch ex As Exception
        End Try
    End Sub

End Module