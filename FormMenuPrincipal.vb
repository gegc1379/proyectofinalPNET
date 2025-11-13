Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Linq

Partial Class FormMenuPrincipal

    Public Property UsuarioRol As String = String.Empty
    Public Property UsuarioCorreo As String = String.Empty

    Private panelLateralExpandido As Boolean = True
    Private ReadOnly expandedWidth As Integer = 200
    Private ReadOnly collapsedWidth As Integer = 60

    Private btnToggle As Button
    Private btnLogout As Button
    Private tt As ToolTip

    ' Color principal de la UI (se puede cambiar desde Configuración Rápida)
    Private primaryColor As Color = Color.FromArgb(0, 122, 204)

    ' Labels del dashboard para actualizar desde BD
    Private lblValOrders As Label
    Private lblValTechs As Label
    Private lblValAppointments As Label
    Private activityFlowPanel As FlowLayoutPanel
    Private rightPanelField As Panel

    ' Opciones (Item3 = emoji). Se admite '*' para indicar visible a todos
    Private opcionesMenu As New List(Of Tuple(Of String, String, String)) From {
        Tuple.Create("Inicio", "*", "🏠"),
        Tuple.Create("Empleados", "Administrador", "👥"),
        Tuple.Create("Usuarios", "Administrador", "👤"),
        Tuple.Create("Clientes", "Administrador;Vendedor;Mecanico;Aseguradora", "🧾"),
        Tuple.Create("Repuestos", "Administrador;Vendedor;Mecanico", "🔧"),
        Tuple.Create("Ventas", "Administrador;Vendedor", "💰"),
        Tuple.Create("Siniestros", "Administrador;Aseguradora", "⚠️"),
        Tuple.Create("Servicios", "Administrador;Vendedor;Mecanico", "🛠️")
    }

    Public Sub New(Optional role As String = "", Optional correo As String = Nothing)
        InitializeComponent()
        If Not String.IsNullOrEmpty(role) Then
            UsuarioRol = role
        End If
        If Not String.IsNullOrEmpty(correo) Then
            UsuarioCorreo = correo
        End If

        tt = New ToolTip() With {.AutoPopDelay = 4000, .InitialDelay = 200, .ReshowDelay = 100, .ShowAlways = True}

        ' Sync header color
        PanelTop.BackColor = Color.FromArgb(40, 50, 60)
        PanelLateral.BackColor = PanelTop.BackColor

        ' configurar tamaño mínimo para evitar ventana demasiado pequeña
        Me.MinimumSize = New Size(900, 600)

        AddHandler PanelLateral.Resize, AddressOf PanelLateral_Resize
        InicializarPanelLateral()
        MostrarDashboardInicial()
        ActualizarEncabezado()
    End Sub

    Private Sub ActualizarEncabezado()
        If Not String.IsNullOrEmpty(UsuarioCorreo) Then
            lblUsuarioTop.Text = $"👤: {UsuarioCorreo}"
        Else
            lblUsuarioTop.Text = ""
        End If
    End Sub

    Private Sub PanelLateral_Resize(sender As Object, e As EventArgs)
        If btnToggle IsNot Nothing Then btnToggle.Left = PanelLateral.Width - btnToggle.Width - 5
        If btnLogout IsNot Nothing Then
            btnLogout.Top = PanelLateral.Height - btnLogout.Height - 10
            btnLogout.Left = If(panelLateralExpandido, 10, (PanelLateral.Width - btnLogout.Width) \ 2)
        End If
    End Sub

    Private Sub InicializarPanelLateral()
        If PanelLateral Is Nothing Then Return

        PanelLateral.Controls.Clear()
        PanelLateral.BackColor = Color.FromArgb(40, 50, 60)
        PanelTop.BackColor = PanelLateral.BackColor
        PanelLateral.Width = If(panelLateralExpandido, expandedWidth, collapsedWidth)

        ' Toggle
        btnToggle = New Button With {.Name = "btnToggle", .Text = If(panelLateralExpandido, "<", ">"), .Width = 30, .Height = 30, .FlatStyle = FlatStyle.Flat, .ForeColor = Color.White, .BackColor = Color.FromArgb(30, 40, 50), .Tag = "TOGGLE", .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        btnToggle.FlatAppearance.BorderSize = 0
        AddHandler btnToggle.Click, AddressOf TogglePanelLateral
        PanelLateral.Controls.Add(btnToggle)
        btnToggle.BringToFront()
        btnToggle.Left = PanelLateral.Width - btnToggle.Width - 5
        btnToggle.Top = 8

        Dim yPos As Integer = btnToggle.Bottom + 12

        For Each opcion In opcionesMenu
            Dim roles = opcion.Item2.Split(";"c).Select(Function(s) s.Trim().ToLower()).ToArray()
            Dim visibleToUser As Boolean = roles.Contains("*") OrElse (Not String.IsNullOrEmpty(UsuarioRol) AndAlso roles.Contains(UsuarioRol.Trim().ToLower()))
            If visibleToUser Then
                Dim mostrarTexto As String = If(panelLateralExpandido, opcion.Item1, opcion.Item3)
                Dim btnWidth As Integer = If(panelLateralExpandido, PanelLateral.Width - 20, collapsedWidth - 10)
                Dim leftPos As Integer = If(panelLateralExpandido, 10, (PanelLateral.Width - btnWidth) \ 2)
                Dim align As ContentAlignment = If(panelLateralExpandido, ContentAlignment.MiddleLeft, ContentAlignment.MiddleCenter)
                Dim fontToUse As Font = If(panelLateralExpandido, New Font("Segoe UI", 11, FontStyle.Regular), New Font("Segoe UI Emoji", 14, FontStyle.Regular))

                Dim btn As New Button With {.Text = mostrarTexto, .Width = btnWidth, .Height = 40, .Left = leftPos, .Top = yPos, .FlatStyle = FlatStyle.Flat, .ForeColor = Color.White, .BackColor = Color.FromArgb(40, 50, 60), .Font = fontToUse, .Tag = opcion.Item1, .TextAlign = align}
                btn.FlatAppearance.BorderSize = 0
                AddHandler btn.MouseEnter, Sub(s, ev) btn.BackColor = Color.FromArgb(60, 70, 80)
                AddHandler btn.MouseLeave, Sub(s, ev) btn.BackColor = Color.FromArgb(40, 50, 60)
                AddHandler btn.Click, AddressOf OpcionMenu_Click
                PanelLateral.Controls.Add(btn)

                If Not panelLateralExpandido Then tt.SetToolTip(btn, opcion.Item1) Else tt.SetToolTip(btn, Nothing)

                yPos += 50
            End If
        Next

        ' Logout
        Dim logoutText As String = If(panelLateralExpandido, "Cerrar Sesión", "⎋")
        Dim logoutWidth As Integer = If(panelLateralExpandido, PanelLateral.Width - 20, collapsedWidth - 10)
        Dim logoutLeft As Integer = If(panelLateralExpandido, 10, (PanelLateral.Width - logoutWidth) \ 2)

        btnLogout = New Button With {.Name = "btnLogout", .Text = logoutText, .Width = logoutWidth, .Height = 40, .Left = logoutLeft, .Top = PanelLateral.Height - 60, .FlatStyle = FlatStyle.Flat, .ForeColor = Color.White, .BackColor = Color.FromArgb(0, 122, 204), .Font = New Font("Segoe UI", 10, FontStyle.Regular), .Tag = "LOGOUT", .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left, .TextAlign = ContentAlignment.MiddleCenter}
        btnLogout.FlatAppearance.BorderSize = 0
        AddHandler btnLogout.Click, AddressOf Logout_Click
        PanelLateral.Controls.Add(btnLogout)
        btnLogout.BringToFront()

        btnToggle.Left = PanelLateral.Width - btnToggle.Width - 5
        btnLogout.Left = If(panelLateralExpandido, 10, (PanelLateral.Width - btnLogout.Width) \ 2)
        btnLogout.Top = PanelLateral.Height - btnLogout.Height - 10
    End Sub

    Private Sub TogglePanelLateral(sender As Object, e As EventArgs)
        panelLateralExpandido = Not panelLateralExpandido
        PanelLateral.Width = If(panelLateralExpandido, expandedWidth, collapsedWidth)
        InicializarPanelLateral()
    End Sub

    Private Sub Logout_Click(sender As Object, e As EventArgs)
        Dim frmLogin As New FormLogin()
        frmLogin.Show()
        Me.Close()
    End Sub

    Private Sub OpcionMenu_Click(sender As Object, e As EventArgs)
        Dim opcionSeleccionada As String = CType(CType(sender, Button).Tag, String)
        If PanelContenido Is Nothing Then Return

        If opcionSeleccionada = "Inicio" Then
            MostrarDashboardInicial()
            Return
        End If

        PanelContenido.Controls.Clear()

        Select Case opcionSeleccionada
            Case "Clientes"
                Dim pnl As New Panel With {.Dock = DockStyle.Fill}
                Dim lbl As New Label With {.Text = "Ultra Mecánica", .Font = New Font("Segoe UI", 20, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 50, 60), .AutoSize = False, .TextAlign = ContentAlignment.MiddleCenter, .Dock = DockStyle.Top, .Height = 80}
                pnl.Controls.Add(lbl)
                PanelContenido.Controls.Add(pnl)

            Case "Repuestos"
                MostrarPanelRepuestos()

            Case "Usuarios"
                MostrarPanelUsuarios()

            Case "Ventas"
                MostrarPanelVentas()

            Case Else
                Dim lbl As New Label With {.Text = $"Has seleccionado: {opcionSeleccionada}", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 50, 60), .AutoSize = True, .Location = New Point(50, 50)}
                PanelContenido.Controls.Add(lbl)
        End Select
    End Sub


    ' *******************************
    ' *** PANEL REPUESTOS  *********
    ' *******************************

    Private Sub MostrarPanelRepuestos()

        Dim dt As New DataTable()
        Dim modoEdicion As Boolean = False
        Dim idRepuestoSeleccionado As Integer = -1

        ' Panel principal
        Dim panelPrincipal As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}

        ' Header
        Dim header As New Panel With {.Dock = DockStyle.Top, .Height = 60, .BackColor = Color.Transparent}
        Dim title As New Label With {
        .Text = "🔧 Gestión de Repuestos",
        .Font = New Font("Segoe UI", 18, FontStyle.Bold),
        .ForeColor = Color.FromArgb(40, 50, 60),
        .AutoSize = True,
        .Location = New Point(20, 15)
    }
        header.Controls.Add(title)


        Dim mainContainer As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}



        ' ** FORMULARIO Panel Repuestos
        Dim leftPanel As New Panel With {
        .Width = 380,
        .Dock = DockStyle.Left,
        .Padding = New Padding(20),
        .BackColor = Color.White
    }

        Dim yPos As Integer = 20

        ' Título del formulario
        Dim lblFormTitle As New Label With {
        .Text = "Datos del Repuesto",
        .Font = New Font("Segoe UI", 14, FontStyle.Bold),
        .ForeColor = Color.FromArgb(0, 122, 204),
        .Location = New Point(20, yPos),
        .AutoSize = True
    }
        leftPanel.Controls.Add(lblFormTitle)
        yPos += 45


        Dim lblId As New Label With {
        .Text = "ID:",
        .Location = New Point(20, yPos),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .AutoSize = True
    }
        Dim txtId As New TextBox With {
        .Location = New Point(20, yPos + 20),
        .Width = 340,
        .Font = New Font("Segoe UI", 10),
        .ReadOnly = True,
        .BackColor = Color.FromArgb(230, 230, 230),
        .Text = "Auto-generado",
        .Name = "txtIdRepuesto"
    }
        leftPanel.Controls.AddRange({lblId, txtId})
        yPos += 60

        '  Nombre
        Dim lblNombre As New Label With {
        .Text = "Nombre del Repuesto: *",
        .Location = New Point(20, yPos),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .ForeColor = Color.FromArgb(40, 50, 60),
        .AutoSize = True
    }
        Dim txtNombre As New TextBox With {
        .Location = New Point(20, yPos + 20),
        .Width = 340,
        .Font = New Font("Segoe UI", 10),
        .Name = "txtNombreRepuesto"
    }
        leftPanel.Controls.AddRange({lblNombre, txtNombre})
        yPos += 60

        '  Cantidad
        Dim lblCantidad As New Label With {
        .Text = "Cantidad en Stock: *",
        .Location = New Point(20, yPos),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .AutoSize = True
    }
        Dim txtCantidad As New TextBox With {
        .Location = New Point(20, yPos + 20),
        .Width = 340,
        .Font = New Font("Segoe UI", 10),
        .Name = "txtCantidadStock"
    }
        leftPanel.Controls.AddRange({lblCantidad, txtCantidad})
        yPos += 60

        '  Precio
        Dim lblPrecio As New Label With {
        .Text = "Precio Unitario: *",
        .Location = New Point(20, yPos),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .AutoSize = True
    }
        Dim txtPrecio As New TextBox With {
        .Location = New Point(20, yPos + 20),
        .Width = 340,
        .Font = New Font("Segoe UI", 10),
        .Name = "txtPrecioUnitario"
    }
        leftPanel.Controls.AddRange({lblPrecio, txtPrecio})
        yPos += 60

        '  Proveedor
        Dim lblProveedor As New Label With {
        .Text = "Proveedor: *",
        .Location = New Point(20, yPos),
        .Font = New Font("Segoe UI", 9, FontStyle.Bold),
        .AutoSize = True
    }
        Dim txtProveedor As New TextBox With {
        .Location = New Point(20, yPos + 20),
        .Width = 340,
        .Font = New Font("Segoe UI", 10),
        .Name = "txtProveedorRepuesto"
    }
        leftPanel.Controls.AddRange({lblProveedor, txtProveedor})
        yPos += 80

        ' Panel de botones
        Dim panelBotones As New Panel With {
        .Location = New Point(20, yPos),
        .Width = 340,
        .Height = 100,
        .BackColor = Color.Transparent
    }

        Dim btnNuevo As New Button With {
        .Text = "➕ Nuevo",
        .Width = 160,
        .Height = 38,
        .Location = New Point(0, 0),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(0, 122, 204),
        .ForeColor = Color.White,
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .Cursor = Cursors.Hand
    }
        btnNuevo.FlatAppearance.BorderSize = 0

        Dim btnGuardar As New Button With {
        .Text = "💾 Guardar",
        .Width = 160,
        .Height = 38,
        .Location = New Point(180, 0),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(40, 167, 69),
        .ForeColor = Color.White,
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .Enabled = False,
        .Cursor = Cursors.Hand
    }
        btnGuardar.FlatAppearance.BorderSize = 0

        Dim btnEditar As New Button With {
        .Text = "✏️ Editar",
        .Width = 160,
        .Height = 38,
        .Location = New Point(0, 48),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(255, 193, 7),
        .ForeColor = Color.White,
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .Enabled = False,
        .Cursor = Cursors.Hand
    }
        btnEditar.FlatAppearance.BorderSize = 0

        Dim btnEliminar As New Button With {
        .Text = "🗑️ Eliminar",
        .Width = 160,
        .Height = 38,
        .Location = New Point(180, 48),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Color.FromArgb(220, 53, 69),
        .ForeColor = Color.White,
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .Enabled = False,
        .Cursor = Cursors.Hand
    }
        btnEliminar.FlatAppearance.BorderSize = 0

        panelBotones.Controls.AddRange({btnNuevo, btnGuardar, btnEditar, btnEliminar})
        leftPanel.Controls.Add(panelBotones)



        ' ***************************************
        ' *** P.REPUESTOS, TABLA Y BÚSQUEDA ***
        ' ***************************************


        Dim rightPanel As New Panel With {
        .Dock = DockStyle.Fill,
        .Padding = New Padding(20),
        .BackColor = Color.WhiteSmoke
    }

        ' Barra de búsqueda 
        Dim panelBusqueda As New Panel With {
        .Dock = DockStyle.Top,
        .Height = 80,
        .BackColor = Color.Transparent
    }

        Dim lblBuscar As New Label With {
        .Text = "🔍 Buscar por nombre o ID:",
        .Location = New Point(0, 5),
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .AutoSize = True
    }

        Dim txtBuscar As New TextBox With {
        .Location = New Point(0, 30),
        .Width = 350,
        .Font = New Font("Segoe UI", 11),
        .Name = "txtBuscarRepuesto"
    }

        panelBusqueda.Controls.AddRange({lblBuscar, txtBuscar})
        rightPanel.Controls.Add(panelBusqueda)

        ' DataGridView
        Dim dgvRepuestos As New DataGridView With {
        .Dock = DockStyle.Fill,
        .BackgroundColor = Color.White,
        .BorderStyle = BorderStyle.None,
        .AllowUserToAddRows = False,
        .AllowUserToDeleteRows = False,
        .ReadOnly = True,
        .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        .MultiSelect = False,
        .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        .RowHeadersVisible = False,
        .EnableHeadersVisualStyles = False,
        .Name = "dgvRepuestos"
    }

        ' Estilo del header
        dgvRepuestos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204)
        dgvRepuestos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvRepuestos.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgvRepuestos.ColumnHeadersDefaultCellStyle.Padding = New Padding(5)
        dgvRepuestos.ColumnHeadersHeight = 40

        ' Estilo de filas
        dgvRepuestos.DefaultCellStyle.Font = New Font("Segoe UI", 9)
        dgvRepuestos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 180, 255)
        dgvRepuestos.DefaultCellStyle.SelectionForeColor = Color.White
        dgvRepuestos.RowTemplate.Height = 35
        dgvRepuestos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

        rightPanel.Controls.Add(dgvRepuestos)

        ' Agregar paneles al contenedor principal
        mainContainer.Controls.Add(rightPanel)
        mainContainer.Controls.Add(leftPanel)

        ' Agregar todo al panel principal
        panelPrincipal.Controls.Add(mainContainer)
        panelPrincipal.Controls.Add(header)

        PanelContenido.Controls.Add(panelPrincipal)



        ' ********************************************
        ' *** PANEL Repuestos Funciones y EVentos ****
        ' ********************************************

        ' Cargar Los Repuestos de la base de datos 
        Dim CargarRepuestos As Action = Sub()
                                            Try
                                                Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                                If conn Is Nothing Then Return

                                                Dim query As String = "SELECT RepuestoID, NombreRepuesto, CantidadStock, PrecioUnitario, Proveedor FROM repuestos ORDER BY RepuestoID"
                                                Dim da As New MySqlDataAdapter(query, conn)
                                                dt = New DataTable()
                                                da.Fill(dt)

                                                dgvRepuestos.DataSource = dt
                                                dgvRepuestos.Columns("RepuestoID").HeaderText = "ID"
                                                dgvRepuestos.Columns("NombreRepuesto").HeaderText = "Nombre"
                                                dgvRepuestos.Columns("CantidadStock").HeaderText = "Stock"
                                                dgvRepuestos.Columns("PrecioUnitario").HeaderText = "Precio"
                                                dgvRepuestos.Columns("Proveedor").HeaderText = "Proveedor"

                                                dgvRepuestos.Columns("RepuestoID").Width = 60
                                                dgvRepuestos.Columns("CantidadStock").Width = 80
                                                dgvRepuestos.Columns("PrecioUnitario").Width = 100

                                            Catch ex As Exception
                                                MessageBox.Show("Error al cargar repuestos: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            Finally
                                                ModuloConexion.Desconectar()
                                            End Try
                                        End Sub


        ' Funcion para limpiar el formulario 
        Dim LimpiarFormulario As Action = Sub()
                                              txtId.Text = "Auto-generado"
                                              txtNombre.Clear()
                                              txtCantidad.Clear()
                                              txtPrecio.Clear()
                                              txtProveedor.Clear()
                                              modoEdicion = False
                                              idRepuestoSeleccionado = -1
                                              btnGuardar.Enabled = False
                                              btnEditar.Enabled = False
                                              btnEliminar.Enabled = False
                                              txtNombre.Enabled = False
                                              txtCantidad.Enabled = False
                                              txtPrecio.Enabled = False
                                              txtProveedor.Enabled = False
                                          End Sub

        ' Validar campos del formulario 
        Dim ValidarCampos As Func(Of Boolean) = Function()
                                                    If String.IsNullOrWhiteSpace(txtNombre.Text) Then
                                                        MessageBox.Show("El nombre del repuesto es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtNombre.Focus()
                                                        Return False
                                                    End If

                                                    Dim cantidad As Integer
                                                    If Not Integer.TryParse(txtCantidad.Text, cantidad) OrElse cantidad < 0 Then
                                                        MessageBox.Show("La cantidad debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtCantidad.Focus()
                                                        Return False
                                                    End If

                                                    Dim precio As Decimal
                                                    If Not Decimal.TryParse(txtPrecio.Text, precio) OrElse precio <= 0 Then
                                                        MessageBox.Show("El precio debe ser un número positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtPrecio.Focus()
                                                        Return False
                                                    End If

                                                    If String.IsNullOrWhiteSpace(txtProveedor.Text) Then
                                                        MessageBox.Show("El proveedor es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtProveedor.Focus()
                                                        Return False
                                                    End If

                                                    Return True
                                                End Function

        ' Verificar duplicados
        Dim VerificarDuplicado As Func(Of Boolean) = Function()
                                                         Try
                                                             Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                                             If conn Is Nothing Then Return True

                                                             Dim query As String = "SELECT COUNT(*) FROM repuestos WHERE LOWER(NombreRepuesto) = LOWER(@nombre)"
                                                             If modoEdicion Then
                                                                 query &= " AND RepuestoID <> @id"
                                                             End If

                                                             Using cmd As New MySqlCommand(query, conn)
                                                                 cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim())
                                                                 If modoEdicion Then
                                                                     cmd.Parameters.AddWithValue("@id", idRepuestoSeleccionado)
                                                                 End If

                                                                 Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                                                                 Return count > 0
                                                             End Using

                                                         Catch ex As Exception
                                                             MessageBox.Show("Error al verificar duplicado: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                             Return True
                                                         Finally
                                                             ModuloConexion.Desconectar()
                                                         End Try
                                                     End Function

        ' Boron :Nuevo
        AddHandler btnNuevo.Click, Sub()
                                       LimpiarFormulario()
                                       txtNombre.Enabled = True
                                       txtCantidad.Enabled = True
                                       txtPrecio.Enabled = True
                                       txtProveedor.Enabled = True
                                       btnGuardar.Enabled = True
                                       txtNombre.Focus()
                                   End Sub

        ' Botón Guardar
        AddHandler btnGuardar.Click, Sub()
                                         If Not ValidarCampos() Then Return

                                         If VerificarDuplicado() Then
                                             MessageBox.Show("Ya existe un repuesto con ese nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                             Return
                                         End If

                                         Try
                                             Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                             If conn Is Nothing Then Return

                                             Dim query As String
                                             If modoEdicion Then
                                                 query = "UPDATE repuestos SET NombreRepuesto=@nombre, CantidadStock=@cantidad, PrecioUnitario=@precio, Proveedor=@proveedor WHERE RepuestoID=@id"
                                             Else
                                                 query = "INSERT INTO repuestos (NombreRepuesto, CantidadStock, PrecioUnitario, Proveedor) VALUES (@nombre, @cantidad, @precio, @proveedor)"
                                             End If

                                             Using cmd As New MySqlCommand(query, conn)
                                                 cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim())
                                                 cmd.Parameters.AddWithValue("@cantidad", Convert.ToInt32(txtCantidad.Text))
                                                 cmd.Parameters.AddWithValue("@precio", Convert.ToDecimal(txtPrecio.Text))
                                                 cmd.Parameters.AddWithValue("@proveedor", txtProveedor.Text.Trim())

                                                 If modoEdicion Then
                                                     cmd.Parameters.AddWithValue("@id", idRepuestoSeleccionado)
                                                 End If

                                                 cmd.ExecuteNonQuery()
                                                 MessageBox.Show(If(modoEdicion, "Repuesto actualizado correctamente.", "Repuesto agregado correctamente."), "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                 CargarRepuestos()
                                                 LimpiarFormulario()
                                             End Using

                                         Catch ex As Exception
                                             MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                         Finally
                                             ModuloConexion.Desconectar()
                                         End Try
                                     End Sub

        '  Botón Editar
        AddHandler btnEditar.Click, Sub()
                                        modoEdicion = True
                                        txtNombre.Enabled = True
                                        txtCantidad.Enabled = True
                                        txtPrecio.Enabled = True
                                        txtProveedor.Enabled = True
                                        btnGuardar.Enabled = True
                                        btnEditar.Enabled = False
                                        btnEliminar.Enabled = False
                                        txtNombre.Focus()
                                    End Sub

        '  Botón Eliminar
        AddHandler btnEliminar.Click, Sub()
                                          If MessageBox.Show("¿Está seguro de eliminar este repuesto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                                              Return
                                          End If

                                          Try
                                              Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                              If conn Is Nothing Then Return

                                              Dim query As String = "DELETE FROM repuestos WHERE RepuestoID=@id"
                                              Using cmd As New MySqlCommand(query, conn)
                                                  cmd.Parameters.AddWithValue("@id", idRepuestoSeleccionado)
                                                  cmd.ExecuteNonQuery()
                                                  MessageBox.Show("Repuesto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                  CargarRepuestos()
                                                  LimpiarFormulario()
                                              End Using

                                          Catch ex As Exception
                                              MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                          Finally
                                              ModuloConexion.Desconectar()
                                          End Try
                                      End Sub

        ' Evento: Selección en DataGridView
        AddHandler dgvRepuestos.SelectionChanged, Sub()
                                                      If dgvRepuestos.SelectedRows.Count > 0 Then
                                                          Dim row As DataGridViewRow = dgvRepuestos.SelectedRows(0)
                                                          idRepuestoSeleccionado = Convert.ToInt32(row.Cells("RepuestoID").Value)
                                                          txtId.Text = row.Cells("RepuestoID").Value.ToString()
                                                          txtNombre.Text = row.Cells("NombreRepuesto").Value.ToString()
                                                          txtCantidad.Text = row.Cells("CantidadStock").Value.ToString()
                                                          txtPrecio.Text = row.Cells("PrecioUnitario").Value.ToString()
                                                          txtProveedor.Text = row.Cells("Proveedor").Value.ToString()
                                                          btnEditar.Enabled = True
                                                          btnEliminar.Enabled = True
                                                          btnGuardar.Enabled = False
                                                          txtNombre.Enabled = False
                                                          txtCantidad.Enabled = False
                                                          txtPrecio.Enabled = False
                                                          txtProveedor.Enabled = False
                                                      End If
                                                  End Sub

        ' Evento: Búsqueda
        AddHandler txtBuscar.TextChanged, Sub()
                                              If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return

                                              Dim filtro As String = txtBuscar.Text.Trim()
                                              If String.IsNullOrEmpty(filtro) Then
                                                  dt.DefaultView.RowFilter = ""
                                              Else
                                                  dt.DefaultView.RowFilter = String.Format("NombreRepuesto LIKE '%{0}%' OR CONVERT(RepuestoID, 'System.String') LIKE '%{0}%'", filtro.Replace("'", "''"))
                                              End If
                                          End Sub

        ' Cargar datos iniciales
        CargarRepuestos()
    End Sub

    '************* FIN PANEL DE REPUESTOS ****************


    ' *******************************
    ' ****  PANEL VENTAS  ***********
    ' *******************************

    Private Sub MostrarPanelVentas()
        ' Variables para el módulo de ventas
        Dim dtRepuestos As New DataTable()
        Dim repuestoSeleccionado As Integer = -1
        Dim stockDisponible As Integer = 0

        ' Panel principal
        Dim panelPrincipal As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}

        ' Header
        Dim header As New Panel With {.Dock = DockStyle.Top, .Height = 60, .BackColor = Color.Transparent}
        Dim title As New Label With {
            .Text = "💰 Registro de Ventas",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True,
            .Location = New Point(20, 15)
        }
        header.Controls.Add(title)

        Dim mainContainer As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}

        ' ** PANEL IZQUIERDO: FORMULARIO DE VENTA **
        Dim leftPanel As New Panel With {
            .Width = 400,
            .Dock = DockStyle.Left,
            .Padding = New Padding(20),
            .BackColor = Color.White,
            .AutoScroll = True
        }

        Dim yPos As Integer = 20

        ' Título del formulario
        Dim lblFormTitle As New Label With {
            .Text = "Datos de la Venta",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 122, 204),
            .Location = New Point(20, yPos),
            .AutoSize = True
        }
        leftPanel.Controls.Add(lblFormTitle)
        yPos += 45

        ' Búsqueda de Repuesto
        Dim lblBuscarRepuesto As New Label With {
            .Text = "Por favor busca y selecciona el repuesto que necesita:",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True,
            .MaximumSize = New Size(340, 0)
        }
        Dim txtBuscarRepuesto As New TextBox With {
            .Location = New Point(20, yPos + 40),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .Name = "txtBuscarRepuesto"
        }
        leftPanel.Controls.AddRange({lblBuscarRepuesto, txtBuscarRepuesto})
        yPos += 80

        ' ComboBox para seleccionar repuesto
        Dim lblRepuesto As New Label With {
            .Text = "Repuesto: *",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim cboRepuesto As New ComboBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Name = "cboRepuesto"
        }
        leftPanel.Controls.AddRange({lblRepuesto, cboRepuesto})
        yPos += 60

        ' Nombre del Repuesto (solo lectura)
        Dim lblNombre As New Label With {
            .Text = "Nombre del Repuesto:",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtNombre As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .ReadOnly = True,
            .BackColor = Color.FromArgb(240, 240, 240),
            .Name = "txtNombreRepuesto"
        }
        leftPanel.Controls.AddRange({lblNombre, txtNombre})
        yPos += 60

        ' Stock Disponible (solo lectura)
        Dim lblStock As New Label With {
            .Text = "Unidades disponibles:",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtStock As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .ReadOnly = True,
            .BackColor = Color.FromArgb(240, 240, 240),
            .Name = "txtStockDisponible"
        }
        leftPanel.Controls.AddRange({lblStock, txtStock})
        yPos += 60

        ' Precio Unitario (solo lectura)
        Dim lblPrecio As New Label With {
            .Text = "Precio:",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtPrecio As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .ReadOnly = True,
            .BackColor = Color.FromArgb(240, 240, 240),
            .Name = "txtPrecioUnitario"
        }
        leftPanel.Controls.AddRange({lblPrecio, txtPrecio})
        yPos += 60

        ' Cantidad a Vender
        Dim lblCantidad As New Label With {
            .Text = "Cantidad: *",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtCantidad As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .Name = "txtCantidadVenta"
        }
        leftPanel.Controls.AddRange({lblCantidad, txtCantidad})
        yPos += 60

        ' Total a pagar (solo lectura)
        Dim lblTotal As New Label With {
            .Text = "Total a pagar:",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtTotal As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .ReadOnly = True,
            .BackColor = Color.FromArgb(240, 240, 240),
            .Name = "txtTotalPagar",
            .Text = "$0.00"
        }
        leftPanel.Controls.AddRange({lblTotal, txtTotal})
        yPos += 60

        ' Fecha (solo lectura)
        Dim lblFecha As New Label With {
            .Text = "Fecha:",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtFecha As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .ReadOnly = True,
            .BackColor = Color.FromArgb(240, 240, 240),
            .Name = "txtFechaVenta",
            .Text = DateTime.Now.ToString("yyyy-MM-dd")
        }
        leftPanel.Controls.AddRange({lblFecha, txtFecha})
        yPos += 70

        ' Botón Confirmar Compra
        Dim btnConfirmar As New Button With {
            .Text = "Confirmar compra",
            .Width = 340,
            .Height = 42,
            .Location = New Point(20, yPos),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(40, 167, 69),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnConfirmar.FlatAppearance.BorderSize = 0
        leftPanel.Controls.Add(btnConfirmar)

        ' ** PANEL DERECHO: TABLA DE REPUESTOS **
        Dim rightPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .BackColor = Color.WhiteSmoke
        }

        Dim panelTabla As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 80,
            .BackColor = Color.Transparent
        }

        Dim lblTablaTitle As New Label With {
            .Text = "Repuestos Disponibles",
            .Location = New Point(0, 5),
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }

        ' Etiquetas de columnas
        Dim lblColumnas As New Label With {
            .Text = "ID  /  Nombre  /  Stock  /  Precio  /  Proveedor",
            .Location = New Point(0, 35),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(100, 100, 100),
            .AutoSize = True
        }

        panelTabla.Controls.AddRange({lblTablaTitle, lblColumnas})
        rightPanel.Controls.Add(panelTabla)

        ' DataGridView de repuestos
        Dim dgvRepuestos As New DataGridView With {
            .Dock = DockStyle.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            .RowHeadersVisible = False,
            .EnableHeadersVisualStyles = False,
            .Name = "dgvRepuestos",
            .AllowUserToResizeRows = False,
            .ScrollBars = ScrollBars.Both
        }

        ' Estilo del header
        dgvRepuestos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204)
        dgvRepuestos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvRepuestos.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgvRepuestos.ColumnHeadersDefaultCellStyle.Padding = New Padding(5)
        dgvRepuestos.ColumnHeadersHeight = 40

        ' Estilo de filas
        dgvRepuestos.DefaultCellStyle.Font = New Font("Segoe UI", 9)
        dgvRepuestos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 180, 255)
        dgvRepuestos.DefaultCellStyle.SelectionForeColor = Color.White
        dgvRepuestos.RowTemplate.Height = 35
        dgvRepuestos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

        rightPanel.Controls.Add(dgvRepuestos)

        ' Agregar paneles al contenedor principal
        mainContainer.Controls.Add(rightPanel)
        mainContainer.Controls.Add(leftPanel)

        ' Agregar todo al panel principal
        panelPrincipal.Controls.Add(mainContainer)
        panelPrincipal.Controls.Add(header)

        PanelContenido.Controls.Add(panelPrincipal)

        ' ********************************************
        ' *** PANEL VENTAS: FUNCIONES Y EVENTOS ****
        ' ********************************************

        ' Función para rellenar el formulario con un repuesto
        Dim RellenarFormulario As Action(Of Integer) = Sub(idRepuesto)
                                                           Try
                                                               Dim rows = dtRepuestos.Select("RepuestoID = " & idRepuesto)
                                                               If rows.Length > 0 Then
                                                                   repuestoSeleccionado = idRepuesto
                                                                   txtNombre.Text = rows(0)("NombreRepuesto").ToString()
                                                                   stockDisponible = Convert.ToInt32(rows(0)("CantidadStock"))
                                                                   txtStock.Text = stockDisponible.ToString()
                                                                   txtPrecio.Text = "$" & Convert.ToDecimal(rows(0)("PrecioUnitario")).ToString("N2")
                                                                   txtCantidad.Clear()
                                                                   txtTotal.Text = "$0.00"

                                                                   ' Seleccionar en el ComboBox
                                                                   For i As Integer = 0 To cboRepuesto.Items.Count - 1
                                                                       Dim item = cboRepuesto.Items(i)
                                                                       If item.RepuestoID = idRepuesto Then
                                                                           cboRepuesto.SelectedIndex = i
                                                                           Exit For
                                                                       End If
                                                                   Next

                                                                   ' Seleccionar en el DataGridView
                                                                   For Each row As DataGridViewRow In dgvRepuestos.Rows
                                                                       If Convert.ToInt32(row.Cells("RepuestoID").Value) = idRepuesto Then
                                                                           row.Selected = True
                                                                           dgvRepuestos.FirstDisplayedScrollingRowIndex = row.Index
                                                                           Exit For
                                                                       End If
                                                                   Next

                                                                   txtCantidad.Focus()
                                                               End If
                                                           Catch ex As Exception
                                                               MessageBox.Show("Error al rellenar formulario: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                           End Try
                                                       End Sub

        ' Cargar repuestos disponibles
        Dim CargarRepuestos As Action = Sub()
                                            Try
                                                Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                                If conn Is Nothing Then Return

                                                Dim query As String = "SELECT RepuestoID, NombreRepuesto, CantidadStock, PrecioUnitario, Proveedor FROM repuestos WHERE CantidadStock > 0 ORDER BY NombreRepuesto"
                                                Dim da As New MySqlDataAdapter(query, conn)
                                                dtRepuestos = New DataTable()
                                                da.Fill(dtRepuestos)

                                                dgvRepuestos.DataSource = dtRepuestos

                                                ' Configurar columnas con anchos específicos
                                                If dgvRepuestos.Columns.Count > 0 Then
                                                    dgvRepuestos.Columns("RepuestoID").HeaderText = "ID"
                                                    dgvRepuestos.Columns("RepuestoID").Width = 50

                                                    dgvRepuestos.Columns("NombreRepuesto").HeaderText = "Nombre"
                                                    dgvRepuestos.Columns("NombreRepuesto").Width = 250

                                                    dgvRepuestos.Columns("CantidadStock").HeaderText = "Stock"
                                                    dgvRepuestos.Columns("CantidadStock").Width = 80

                                                    dgvRepuestos.Columns("PrecioUnitario").HeaderText = "Precio"
                                                    dgvRepuestos.Columns("PrecioUnitario").Width = 100
                                                    dgvRepuestos.Columns("PrecioUnitario").DefaultCellStyle.Format = "N2"

                                                    dgvRepuestos.Columns("Proveedor").HeaderText = "Proveedor"
                                                    dgvRepuestos.Columns("Proveedor").Width = 180
                                                End If

                                                ' Llenar ComboBox
                                                cboRepuesto.Items.Clear()
                                                cboRepuesto.DisplayMember = "NombreRepuesto"
                                                cboRepuesto.ValueMember = "RepuestoID"
                                                For Each row As DataRow In dtRepuestos.Rows
                                                    cboRepuesto.Items.Add(New With {
                                                        .RepuestoID = row("RepuestoID"),
                                                        .NombreRepuesto = row("NombreRepuesto").ToString() & " (Stock: " & row("CantidadStock").ToString() & ")"
                                                    })
                                                Next

                                            Catch ex As Exception
                                                MessageBox.Show("Error al cargar repuestos: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            Finally
                                                ModuloConexion.Desconectar()
                                            End Try
                                        End Sub

        ' Calcular total
        Dim CalcularTotal As Action = Sub()
                                          Try
                                              If String.IsNullOrWhiteSpace(txtCantidad.Text) OrElse String.IsNullOrWhiteSpace(txtPrecio.Text) Then
                                                  txtTotal.Text = "$0.00"
                                                  Return
                                              End If

                                              Dim cantidad As Decimal
                                              Dim precio As Decimal

                                              If Decimal.TryParse(txtCantidad.Text, cantidad) AndAlso Decimal.TryParse(txtPrecio.Text.Replace("$", ""), precio) Then
                                                  Dim total As Decimal = cantidad * precio
                                                  txtTotal.Text = "$" & total.ToString("N2")
                                              Else
                                                  txtTotal.Text = "$0.00"
                                              End If
                                          Catch ex As Exception
                                              txtTotal.Text = "$0.00"
                                          End Try
                                      End Sub

        ' Evento: Cambio en ComboBox de repuesto
        AddHandler cboRepuesto.SelectedIndexChanged, Sub()
                                                         Try
                                                             If cboRepuesto.SelectedIndex >= 0 Then
                                                                 Dim selectedItem = cboRepuesto.SelectedItem
                                                                 RellenarFormulario(selectedItem.RepuestoID)
                                                             End If
                                                         Catch ex As Exception
                                                             MessageBox.Show("Error al seleccionar repuesto: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                         End Try
                                                     End Sub

        ' Evento: Click en DataGridView
        AddHandler dgvRepuestos.CellClick, Sub(sender, e)
                                               Try
                                                   If e.RowIndex >= 0 AndAlso e.RowIndex < dgvRepuestos.Rows.Count Then
                                                       Dim idRepuesto As Integer = Convert.ToInt32(dgvRepuestos.Rows(e.RowIndex).Cells("RepuestoID").Value)
                                                       RellenarFormulario(idRepuesto)
                                                   End If
                                               Catch ex As Exception
                                                   MessageBox.Show("Error al seleccionar fila: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                               End Try
                                           End Sub

        ' Evento: Cambio en cantidad
        AddHandler txtCantidad.TextChanged, Sub()
                                                CalcularTotal()
                                            End Sub

        ' Evento: Búsqueda de repuesto (con auto-selección)
        AddHandler txtBuscarRepuesto.TextChanged, Sub()
                                                      If dtRepuestos Is Nothing OrElse dtRepuestos.Rows.Count = 0 Then Return

                                                      Dim filtro As String = txtBuscarRepuesto.Text.Trim()
                                                      If String.IsNullOrEmpty(filtro) Then
                                                          dtRepuestos.DefaultView.RowFilter = ""
                                                          Return
                                                      End If

                                                      ' Filtrar tabla
                                                      dtRepuestos.DefaultView.RowFilter = String.Format("NombreRepuesto LIKE '%{0}%' OR CONVERT(RepuestoID, 'System.String') LIKE '%{0}%'", filtro.Replace("'", "''"))

                                                      ' Si hay solo un resultado, seleccionarlo automáticamente
                                                      If dtRepuestos.DefaultView.Count = 1 Then
                                                          Dim idRepuesto As Integer = Convert.ToInt32(dtRepuestos.DefaultView(0)("RepuestoID"))
                                                          RellenarFormulario(idRepuesto)
                                                      ElseIf dtRepuestos.DefaultView.Count > 1 Then
                                                          ' Buscar coincidencia exacta por ID
                                                          Dim idBuscado As Integer
                                                          If Integer.TryParse(filtro, idBuscado) Then
                                                              Dim rows = dtRepuestos.Select("RepuestoID = " & idBuscado)
                                                              If rows.Length > 0 Then
                                                                  RellenarFormulario(idBuscado)
                                                              End If
                                                          End If
                                                      End If
                                                  End Sub

        ' Evento: Confirmar Compra
        AddHandler btnConfirmar.Click, Sub()
                                           ' Validaciones
                                           If repuestoSeleccionado = -1 Then
                                               MessageBox.Show("Por favor, seleccione un repuesto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                               cboRepuesto.Focus()
                                               Return
                                           End If

                                           If String.IsNullOrWhiteSpace(txtCantidad.Text) Then
                                               MessageBox.Show("Por favor, ingrese la cantidad a vender.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                               txtCantidad.Focus()
                                               Return
                                           End If

                                           Dim cantidadVenta As Integer
                                           If Not Integer.TryParse(txtCantidad.Text, cantidadVenta) OrElse cantidadVenta <= 0 Then
                                               MessageBox.Show("La cantidad debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                               txtCantidad.Focus()
                                               Return
                                           End If

                                           ' Validar stock disponible
                                           If cantidadVenta > stockDisponible Then
                                               MessageBox.Show("La cantidad solicitada (" & cantidadVenta.ToString() & ") excede el stock disponible (" & stockDisponible.ToString() & ")." & vbCrLf & vbCrLf & "Por favor, ingrese una cantidad menor o igual al stock disponible.", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                               txtCantidad.Focus()
                                               Return
                                           End If

                                           ' Confirmar venta
                                           Dim total As Decimal = Decimal.Parse(txtTotal.Text.Replace("$", ""))
                                           Dim resultado As DialogResult = MessageBox.Show(
                                               "¿Confirmar la venta?" & vbCrLf & vbCrLf &
                                               "Repuesto: " & txtNombre.Text & vbCrLf &
                                               "Cantidad: " & cantidadVenta.ToString() & vbCrLf &
                                               "Total: $" & total.ToString("N2"),
                                               "Confirmar Venta",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question)

                                           If resultado = DialogResult.No Then Return

                                           ' Procesar venta
                                           Try
                                               Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                               If conn Is Nothing Then Return

                                               ' Actualizar stock
                                               Dim queryUpdate As String = "UPDATE repuestos SET CantidadStock = CantidadStock - @cantidad WHERE RepuestoID = @id"
                                               Using cmd As New MySqlCommand(queryUpdate, conn)
                                                   cmd.Parameters.AddWithValue("@cantidad", cantidadVenta)
                                                   cmd.Parameters.AddWithValue("@id", repuestoSeleccionado)
                                                   cmd.ExecuteNonQuery()
                                               End Using

                                               MessageBox.Show("¡Venta realizada exitosamente!" & vbCrLf & vbCrLf &
                                                             "Total: $" & total.ToString("N2") & vbCrLf &
                                                             "Fecha: " & DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                                             "Venta Confirmada",
                                                             MessageBoxButtons.OK,
                                                             MessageBoxIcon.Information)

                                               ' Limpiar formulario
                                               cboRepuesto.SelectedIndex = -1
                                               txtNombre.Clear()
                                               txtStock.Clear()
                                               txtPrecio.Clear()
                                               txtCantidad.Clear()
                                               txtTotal.Text = "$0.00"
                                               txtBuscarRepuesto.Clear()
                                               repuestoSeleccionado = -1
                                               stockDisponible = 0

                                               ' Recargar datos
                                               CargarRepuestos()

                                           Catch ex As Exception
                                               MessageBox.Show("Error al procesar la venta: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                           Finally
                                               ModuloConexion.Desconectar()
                                           End Try
                                       End Sub

        ' Cargar datos iniciales
        CargarRepuestos()
    End Sub

    '************* FIN PANEL DE VENTAS ****************


    ' *******************************
    ' ****  PANEL USUARIOS  *********
    ' *******************************

    Private Sub MostrarPanelUsuarios()
        ' Variables para el módulo de usuarios
        Dim dt As New DataTable()
        Dim modoEdicion As Boolean = False
        Dim rutUsuarioSeleccionado As String = String.Empty

        ' Panel principal
        Dim panelPrincipal As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}

        ' Header
        Dim header As New Panel With {.Dock = DockStyle.Top, .Height = 60, .BackColor = Color.Transparent}
        Dim title As New Label With {
            .Text = "👤 Gestión de Usuarios",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True,
            .Location = New Point(20, 15)
        }
        header.Controls.Add(title)

        ' Contenedor principal (izquierda: formulario; derecha: tabla)
        Dim mainContainer As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}

        ' ************************************
        ' ****  PANEL USUARIOS: fORMULARIO ***
        ' ************************************

        Dim leftPanel As New Panel With {
            .Width = 380,
            .Dock = DockStyle.Left,
            .Padding = New Padding(20),
            .BackColor = Color.White
        }

        Dim yPos As Integer = 20

        ' Título del formulario
        Dim lblFormTitle As New Label With {
            .Text = "Datos del Usuario",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 122, 204),
            .Location = New Point(20, yPos),
            .AutoSize = True
        }
        leftPanel.Controls.Add(lblFormTitle)
        yPos += 45

        ' Campo RUT
        Dim lblRut As New Label With {
            .Text = "RUT: *",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtRut As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .MaxLength = 11,
            .Name = "txtRutUsuario"
        }
        leftPanel.Controls.AddRange({lblRut, txtRut})
        yPos += 60

        ' Campo Correo
        Dim lblCorreo As New Label With {
            .Text = "Correo Electrónico: *",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtCorreo As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .Name = "txtCorreoUsuario"
        }
        leftPanel.Controls.AddRange({lblCorreo, txtCorreo})
        yPos += 60

        ' Campo Contraseña
        Dim lblContraseña As New Label With {
            .Text = "Contraseña: *",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim txtContraseña As New TextBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .Name = "txtContraseñaUsuario"
        }
        leftPanel.Controls.AddRange({lblContraseña, txtContraseña})
        yPos += 60

        ' Campo Tipo (ComboBox)
        Dim lblTipo As New Label With {
            .Text = "Tipo de Usuario: *",
            .Location = New Point(20, yPos),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 60),
            .AutoSize = True
        }
        Dim cboTipo As New ComboBox With {
            .Location = New Point(20, yPos + 20),
            .Width = 340,
            .Font = New Font("Segoe UI", 10),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Name = "cboTipoUsuario"
        }
        cboTipo.Items.AddRange({"Administrador", "Vendedor", "Mecanico", "Aseguradora", "Analista", "Gerente"})
        leftPanel.Controls.AddRange({lblTipo, cboTipo})
        yPos += 80

        ' Panel de botones
        Dim panelBotones As New Panel With {
            .Location = New Point(20, yPos),
            .Width = 340,
            .Height = 100,
            .BackColor = Color.Transparent
        }

        Dim btnNuevo As New Button With {
            .Text = "➕ Nuevo",
            .Width = 160,
            .Height = 38,
            .Location = New Point(0, 0),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 122, 204),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnNuevo.FlatAppearance.BorderSize = 0

        Dim btnGuardar As New Button With {
            .Text = "💾 Guardar",
            .Width = 160,
            .Height = 38,
            .Location = New Point(180, 0),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(40, 167, 69),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Enabled = False,
            .Cursor = Cursors.Hand
        }
        btnGuardar.FlatAppearance.BorderSize = 0

        Dim btnEditar As New Button With {
            .Text = "✏️ Editar",
            .Width = 160,
            .Height = 38,
            .Location = New Point(0, 48),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(255, 193, 7),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Enabled = False,
            .Cursor = Cursors.Hand
        }
        btnEditar.FlatAppearance.BorderSize = 0

        Dim btnEliminar As New Button With {
            .Text = "🗑️ Eliminar",
            .Width = 160,
            .Height = 38,
            .Location = New Point(180, 48),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(220, 53, 69),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Enabled = False,
            .Cursor = Cursors.Hand
        }
        btnEliminar.FlatAppearance.BorderSize = 0

        panelBotones.Controls.AddRange({btnNuevo, btnGuardar, btnEditar, btnEliminar})
        leftPanel.Controls.Add(panelBotones)

        '*******************************************
        '*** PANEL DE USUARIOS: TABLA Y BÚSQUEDA ***
        '*******************************************

        Dim rightPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .BackColor = Color.WhiteSmoke
        }

        ' Barra de búsqueda
        Dim panelBusqueda As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 70,
            .BackColor = Color.Transparent
        }

        Dim lblBuscar As New Label With {
            .Text = "🔍 Buscar por RUT o Correo:",
            .Location = New Point(0, 5),
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .AutoSize = True
        }

        Dim txtBuscar As New TextBox With {
            .Location = New Point(0, 30),
            .Width = 350,
            .Font = New Font("Segoe UI", 11),
            .Name = "txtBuscarUsuario"
        }

        panelBusqueda.Controls.AddRange({lblBuscar, txtBuscar})
        rightPanel.Controls.Add(panelBusqueda)

        ' DataGridView
        Dim dgvUsuarios As New DataGridView With {
            .Dock = DockStyle.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .RowHeadersVisible = False,
            .EnableHeadersVisualStyles = False,
            .Name = "dgvUsuarios"
        }

        ' Estilo del header
        dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204)
        dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgvUsuarios.ColumnHeadersDefaultCellStyle.Padding = New Padding(5)
        dgvUsuarios.ColumnHeadersHeight = 40

        ' Estilo de filas
        dgvUsuarios.DefaultCellStyle.Font = New Font("Segoe UI", 9)
        dgvUsuarios.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 180, 255)
        dgvUsuarios.DefaultCellStyle.SelectionForeColor = Color.White
        dgvUsuarios.RowTemplate.Height = 35
        dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

        rightPanel.Controls.Add(dgvUsuarios)

        ' Agregar paneles al contenedor principal
        mainContainer.Controls.Add(rightPanel)
        mainContainer.Controls.Add(leftPanel)

        ' Agregar todo al panel principal
        panelPrincipal.Controls.Add(mainContainer)
        panelPrincipal.Controls.Add(header)

        PanelContenido.Controls.Add(panelPrincipal)

        '**********************************************
        '*** PANEL DE USUARIOS: FUNCIONES Y EVENTOS ***
        '**********************************************

        ' Cargar usuarios desde la BD
        Dim CargarUsuarios As Action = Sub()
                                           Try
                                               Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                               If conn Is Nothing Then Return

                                               Dim query As String = "SELECT Rut, Correo, Contraseña, Tipo FROM usuarios ORDER BY Rut"
                                               Dim da As New MySqlDataAdapter(query, conn)
                                               dt = New DataTable()
                                               da.Fill(dt)

                                               dgvUsuarios.DataSource = dt
                                               dgvUsuarios.Columns("Rut").HeaderText = "RUT"
                                               dgvUsuarios.Columns("Correo").HeaderText = "Correo Electrónico"
                                               dgvUsuarios.Columns("Contraseña").HeaderText = "Contraseña"
                                               dgvUsuarios.Columns("Tipo").HeaderText = "Tipo"

                                               dgvUsuarios.Columns("Rut").Width = 100
                                               dgvUsuarios.Columns("Correo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                                               dgvUsuarios.Columns("Contraseña").Width = 100
                                               dgvUsuarios.Columns("Tipo").Width = 120

                                           Catch ex As Exception
                                               MessageBox.Show("Error al cargar usuarios: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                           Finally
                                               ModuloConexion.Desconectar()
                                           End Try
                                       End Sub

        ' Limpiar formulario
        Dim LimpiarFormulario As Action = Sub()
                                              txtRut.Clear()
                                              txtCorreo.Clear()
                                              txtContraseña.Clear()
                                              cboTipo.SelectedIndex = -1
                                              modoEdicion = False
                                              rutUsuarioSeleccionado = String.Empty
                                              btnGuardar.Enabled = False
                                              btnEditar.Enabled = False
                                              btnEliminar.Enabled = False
                                              txtRut.Enabled = False
                                              txtCorreo.Enabled = False
                                              txtContraseña.Enabled = False
                                              cboTipo.Enabled = False
                                          End Sub

        ' Validar campos
        Dim ValidarCampos As Func(Of Boolean) = Function()
                                                    ' Validar RUT
                                                    If String.IsNullOrWhiteSpace(txtRut.Text) Then
                                                        MessageBox.Show("El RUT es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtRut.Focus()
                                                        Return False
                                                    End If

                                                    If txtRut.Text.Trim().Length < 8 Or txtRut.Text.Trim().Length > 11 Then
                                                        MessageBox.Show("El RUT debe tener entre 8 y 11 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtRut.Focus()
                                                        Return False
                                                    End If

                                                    ' Validar Correo
                                                    If String.IsNullOrWhiteSpace(txtCorreo.Text) Then
                                                        MessageBox.Show("El correo electrónico es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtCorreo.Focus()
                                                        Return False
                                                    End If

                                                    If Not txtCorreo.Text.Contains("@") OrElse Not txtCorreo.Text.Contains(".") Then
                                                        MessageBox.Show("Debe ingresar un correo electrónico válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtCorreo.Focus()
                                                        Return False
                                                    End If

                                                    ' Validar Contraseña
                                                    If String.IsNullOrWhiteSpace(txtContraseña.Text) Then
                                                        MessageBox.Show("La contraseña es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtContraseña.Focus()
                                                        Return False
                                                    End If

                                                    If txtContraseña.Text.Length < 6 Then
                                                        MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        txtContraseña.Focus()
                                                        Return False
                                                    End If

                                                    ' Validar Tipo
                                                    If cboTipo.SelectedIndex = -1 Then
                                                        MessageBox.Show("Debe seleccionar un tipo de usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                        cboTipo.Focus()
                                                        Return False
                                                    End If

                                                    Return True
                                                End Function

        ' Verificar RUT duplicado
        Dim VerificarDuplicadoRut As Func(Of Boolean) = Function()
                                                            Try
                                                                Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                                                If conn Is Nothing Then Return True

                                                                Dim query As String = "SELECT COUNT(*) FROM usuarios WHERE Rut = @rut"
                                                                If modoEdicion Then
                                                                    query &= " AND Rut <> @rutOriginal"
                                                                End If

                                                                Using cmd As New MySqlCommand(query, conn)
                                                                    cmd.Parameters.AddWithValue("@rut", txtRut.Text.Trim())
                                                                    If modoEdicion Then
                                                                        cmd.Parameters.AddWithValue("@rutOriginal", rutUsuarioSeleccionado)
                                                                    End If

                                                                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                                                                    Return count > 0
                                                                End Using

                                                            Catch ex As Exception
                                                                MessageBox.Show("Error al verificar RUT: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                                Return True
                                                            Finally
                                                                ModuloConexion.Desconectar()
                                                            End Try
                                                        End Function

        ' Verificar Correo duplicado
        Dim VerificarDuplicadoCorreo As Func(Of Boolean) = Function()
                                                               Try
                                                                   Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                                                   If conn Is Nothing Then Return True

                                                                   Dim query As String = "SELECT COUNT(*) FROM usuarios WHERE LOWER(Correo) = LOWER(@correo)"
                                                                   If modoEdicion Then
                                                                       query &= " AND Rut <> @rutOriginal"
                                                                   End If

                                                                   Using cmd As New MySqlCommand(query, conn)
                                                                       cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim())
                                                                       If modoEdicion Then
                                                                           cmd.Parameters.AddWithValue("@rutOriginal", rutUsuarioSeleccionado)
                                                                       End If

                                                                       Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                                                                       Return count > 0
                                                                   End Using

                                                               Catch ex As Exception
                                                                   MessageBox.Show("Error al verificar correo: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                                   Return True
                                                               Finally
                                                                   ModuloConexion.Desconectar()
                                                               End Try
                                                           End Function

        ' Evento: Botón Nuevo
        AddHandler btnNuevo.Click, Sub()
                                       LimpiarFormulario()
                                       txtRut.Enabled = True
                                       txtCorreo.Enabled = True
                                       txtContraseña.Enabled = True
                                       cboTipo.Enabled = True
                                       btnGuardar.Enabled = True
                                       txtRut.Focus()
                                   End Sub

        ' Evento: Botón Guardar
        AddHandler btnGuardar.Click, Sub()
                                         If Not ValidarCampos() Then Return

                                         If VerificarDuplicadoRut() Then
                                             MessageBox.Show("Ya existe un usuario con ese RUT.", "RUT Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                             Return
                                         End If

                                         If VerificarDuplicadoCorreo() Then
                                             MessageBox.Show("Ya existe un usuario con ese correo electrónico.", "Correo Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                             Return
                                         End If

                                         Try
                                             Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                             If conn Is Nothing Then Return

                                             Dim query As String
                                             If modoEdicion Then
                                                 query = "UPDATE usuarios SET Rut=@rut, Correo=@correo, Contraseña=@pass, Tipo=@tipo WHERE Rut=@rutOriginal"
                                             Else
                                                 query = "INSERT INTO usuarios (Rut, Correo, Contraseña, Tipo) VALUES (@rut, @correo, @pass, @tipo)"
                                             End If

                                             Using cmd As New MySqlCommand(query, conn)
                                                 cmd.Parameters.AddWithValue("@rut", txtRut.Text.Trim())
                                                 cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim())
                                                 cmd.Parameters.AddWithValue("@pass", txtContraseña.Text.Trim())
                                                 cmd.Parameters.AddWithValue("@tipo", cboTipo.SelectedItem.ToString())

                                                 If modoEdicion Then
                                                     cmd.Parameters.AddWithValue("@rutOriginal", rutUsuarioSeleccionado)
                                                 End If

                                                 cmd.ExecuteNonQuery()
                                                 MessageBox.Show(If(modoEdicion, "Usuario actualizado correctamente.", "Usuario agregado correctamente."), "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                 CargarUsuarios()
                                                 LimpiarFormulario()
                                             End Using

                                         Catch ex As Exception
                                             MessageBox.Show("Error al guardar usuario: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                         Finally
                                             ModuloConexion.Desconectar()
                                         End Try
                                     End Sub

        ' Evento: Botón Editar
        AddHandler btnEditar.Click, Sub()
                                        modoEdicion = True
                                        txtRut.Enabled = True
                                        txtCorreo.Enabled = True
                                        txtContraseña.Enabled = True
                                        cboTipo.Enabled = True
                                        btnGuardar.Enabled = True
                                        btnEditar.Enabled = False
                                        btnEliminar.Enabled = False
                                        txtRut.Focus()
                                    End Sub

        ' Evento: Botón Eliminar
        AddHandler btnEliminar.Click, Sub()
                                          If MessageBox.Show("¿Está seguro de eliminar este usuario?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                                              Return
                                          End If

                                          Try
                                              Dim conn As MySqlConnection = ModuloConexion.GetConexion()
                                              If conn Is Nothing Then Return

                                              Dim query As String = "DELETE FROM usuarios WHERE Rut=@rut"
                                              Using cmd As New MySqlCommand(query, conn)
                                                  cmd.Parameters.AddWithValue("@rut", rutUsuarioSeleccionado)
                                                  cmd.ExecuteNonQuery()
                                                  MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                  CargarUsuarios()
                                                  LimpiarFormulario()
                                              End Using

                                          Catch ex As Exception
                                              MessageBox.Show("Error al eliminar usuario: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                          Finally
                                              ModuloConexion.Desconectar()
                                          End Try
                                      End Sub

        ' Evento: Selección en DataGridView
        AddHandler dgvUsuarios.SelectionChanged, Sub()
                                                     If dgvUsuarios.SelectedRows.Count > 0 Then
                                                         Dim row As DataGridViewRow = dgvUsuarios.SelectedRows(0)
                                                         rutUsuarioSeleccionado = row.Cells("Rut").Value.ToString()
                                                         txtRut.Text = row.Cells("Rut").Value.ToString()
                                                         txtCorreo.Text = row.Cells("Correo").Value.ToString()
                                                         txtContraseña.Text = row.Cells("Contraseña").Value.ToString()
                                                         cboTipo.SelectedItem = row.Cells("Tipo").Value.ToString()
                                                         btnEditar.Enabled = True
                                                         btnEliminar.Enabled = True
                                                         btnGuardar.Enabled = False
                                                         txtRut.Enabled = False
                                                         txtCorreo.Enabled = False
                                                         txtContraseña.Enabled = False
                                                         cboTipo.Enabled = False
                                                     End If
                                                 End Sub

        ' Evento: Búsqueda
        AddHandler txtBuscar.TextChanged, Sub()
                                              If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return

                                              Dim filtro As String = txtBuscar.Text.Trim()
                                              If String.IsNullOrEmpty(filtro) Then
                                                  dt.DefaultView.RowFilter = ""
                                              Else
                                                  filtro = filtro.Replace("'", "''")
                                                  dt.DefaultView.RowFilter = String.Format("Rut LIKE '%{0}%' OR Correo LIKE '%{0}%'", filtro)
                                              End If
                                          End Sub

        ' Cargar datos iniciales
        CargarUsuarios()
    End Sub
    '************* FIN PANEL DE USUARIOS ****************

    '***********************
    '*** PANEL DE INICIO ***
    '***********************

    Private Sub MostrarDashboardInicial()
        ' Limpiar contenido previo
        PanelContenido.Controls.Clear()
        PanelContenido.BackColor = Color.WhiteSmoke

        ' Header
        Dim header As New Panel With {.Dock = DockStyle.Top, .Height = 60, .BackColor = Color.Transparent}
        Dim title As New Label With {.Text = "Ultra Mecánica", .Font = New Font("Segoe UI", 18, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 50, 60), .AutoSize = True, .Location = New Point(20, 15)}
        header.Controls.Add(title)

        ' Contenedor principal (izquierda: cards + botones; derecha: actividad reciente)
        Dim mainContainer As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.WhiteSmoke}

        ' Left: estadisticas y botones
        Dim leftPanel As New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .BackColor = Color.WhiteSmoke}

        ' Tarjetas de estadísticas: uso de FlowLayout para que se acomoden horizontalmente
        Dim statsFlow As New FlowLayoutPanel With {.Dock = DockStyle.Top, .AutoSize = True, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        statsFlow.Padding = New Padding(0)

        ' Helper para crear tarjeta
        Dim createCard As Func(Of String, Label, String, Panel) = Function(titleText, valueLabel, subText)
                                                                      Dim p As New Panel With {.Size = New Size(240, 90), .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle, .Margin = New Padding(6)}
                                                                      Dim t As New Label With {.Text = titleText, .Font = New Font("Segoe UI", 10, FontStyle.Regular), .ForeColor = Color.FromArgb(80, 80, 80), .Location = New Point(12, 8), .AutoSize = True}
                                                                      valueLabel.Font = New Font("Segoe UI", 20, FontStyle.Bold)
                                                                      valueLabel.ForeColor = primaryColor
                                                                      valueLabel.Location = New Point(12, 30)
                                                                      valueLabel.AutoSize = True
                                                                      Dim s As New Label With {.Text = subText, .Font = New Font("Segoe UI", 9, FontStyle.Regular), .ForeColor = Color.Gray, .Location = New Point(12, 60), .AutoSize = True}
                                                                      p.Controls.Add(t)
                                                                      p.Controls.Add(valueLabel)
                                                                      p.Controls.Add(s)
                                                                      Return p
                                                                  End Function

        ' Crear labels para actualización desde BD
        lblValOrders = New Label()
        lblValTechs = New Label()
        lblValAppointments = New Label()

        Dim card1 = createCard("Órdenes Activas", lblValOrders, "Última agregada: #A5732")
        Dim card2 = createCard("Técnicos Disponibles", lblValTechs, "En servicio: 3 / En pausa: 2")
        Dim card3 = createCard("Citas Próximas", lblValAppointments, "Mañana: Juan Pérez")

        ' Valores por defecto
        lblValOrders.Text = "--"
        lblValTechs.Text = "--"
        lblValAppointments.Text = "--"

        statsFlow.Controls.Add(card1)
        statsFlow.Controls.Add(card2)
        statsFlow.Controls.Add(card3)

        ' Espacio entre stats y los demás elementos
        Dim spacer As New Panel With {.Height = 12, .Dock = DockStyle.Top}

        ' Botones (colocados en la parte inferior del leftPanel)
        Dim buttonsPanel As New Panel With {.Dock = DockStyle.Bottom, .Height = 70, .BackColor = Color.Transparent}
        Dim btnReport As New Button With {.Text = "Generar Reporte", .Width = 160, .Height = 36, .Left = 10, .Top = 16, .FlatStyle = FlatStyle.Flat}
        btnReport.FlatAppearance.BorderSize = 0
        AddHandler btnReport.Click, Sub(s, ev) MessageBox.Show("Función de generación de reportes aún no implementada.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Dim btnConfig As New Button With {.Text = "Configuración Rápida", .Width = 160, .Height = 36, .Left = btnReport.Right + 12, .Top = 16, .FlatStyle = FlatStyle.Flat}
        btnConfig.FlatAppearance.BorderSize = 0
        AddHandler btnConfig.Click, AddressOf BtnConfig_Click

        ' Usar color similar al boton Cerrar Sesión para contraste
        btnReport.BackColor = Color.FromArgb(0, 122, 204)
        btnReport.ForeColor = Color.White
        btnConfig.BackColor = Color.FromArgb(0, 122, 204)
        btnConfig.ForeColor = Color.White

        buttonsPanel.Controls.Add(btnReport)
        buttonsPanel.Controls.Add(btnConfig)

        ' Agregar elementos al leftPanel en orden para correcto dock: primero tarjetas, luego espacio, luego botones
        leftPanel.Controls.Add(statsFlow)
        leftPanel.Controls.Add(spacer)
        ' buttonsPanel is Dock.Bottom so add it last
        leftPanel.Controls.Add(buttonsPanel)

        ' Right: actividad reciente
        rightPanelField = New Panel With {.Width = 340, .Dock = DockStyle.Right, .Padding = New Padding(10), .BackColor = Color.White}
        Dim rightTitle As New Label With {.Text = "Actividad Reciente", .Font = New Font("Segoe UI", 12, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 50, 60), .Dock = DockStyle.Top, .Height = 30}
        rightPanelField.Controls.Add(rightTitle)

        activityFlowPanel = New FlowLayoutPanel With {.Dock = DockStyle.Fill, .AutoScroll = True, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
        activityFlowPanel.Padding = New Padding(6)
        activityFlowPanel.HorizontalScroll.Enabled = False
        activityFlowPanel.HorizontalScroll.Visible = False

        '  de actividad con mejor aspecto
        Dim createActivityItem As Func(Of String, String, Panel) =
            Function(icon, text)

                Dim it As New Panel With {.Width = rightPanelField.Width - 24, .Height = 60, .BackColor = Color.White, .BorderStyle = BorderStyle.None, .Margin = New Padding(0, 6, 0, 6)}
                Dim pic As New Label With {.Text = icon, .Font = New Font("Segoe UI Emoji", 12, FontStyle.Regular), .AutoSize = False, .Size = New Size(28, 28), .Location = New Point(6, 16), .ForeColor = Color.Gray}
                Dim lbl As New Label With {.Text = text, .Font = New Font("Segoe UI", 9, FontStyle.Regular), .ForeColor = Color.FromArgb(70, 70, 70), .Location = New Point(44, 12), .AutoSize = False, .Size = New Size(it.Width - 50, 36)}
                it.Controls.Add(pic)
                it.Controls.Add(lbl)
                Return it
            End Function

        ' Llenar con datos (temporal) y luego intentar cargar desde BD
        activityFlowPanel.Controls.Add(createActivityItem("👤", "Hace 5 min: #A5733 asignada a Luis G."))
        activityFlowPanel.Controls.Add(createActivityItem("📦", "Hace 15 min: Nuevo cliente registrado: María R."))
        activityFlowPanel.Controls.Add(createActivityItem("🔔", "Hace 15 min: Nuevo cliente notificado: Flet buso."))
        activityFlowPanel.Controls.Add(createActivityItem("⚙️", "Hace 30 min: Stock de aceite sintético bajo."))

        rightPanelField.Controls.Add(activityFlowPanel)

        ' Ajustar anchura de items cuando el panel cambie de tamaño
        AddHandler activityFlowPanel.Resize, AddressOf ActivityFlowPanel_Resize

        ' Agregar al mainContainer
        mainContainer.Controls.Add(leftPanel)
        mainContainer.Controls.Add(rightPanelField)

        ' Agregar todo al PanelContenido
        PanelContenido.Controls.Add(mainContainer)
        PanelContenido.Controls.Add(header)

        ' Cargar datos desde BD (si disponible)
        LoadDashboardData()

        ' Aplicar colores iniciales
        ApplyPrimaryColor()
    End Sub

    Private Sub LoadDashboardData()

        Try
            Dim conn As MySqlConnection = ModuloConexion.GetConexion()
            If conn Is Nothing Then Return

            ' Consultas seguras: contar órdenes, técnicos y citas; y obtener últimas actividades
            Try
                Using cmd As New MySqlCommand("SELECT COUNT(*) FROM ordenes WHERE Estado = 'Activa'", conn)
                    Dim o = cmd.ExecuteScalar()
                    If o IsNot Nothing Then lblValOrders.Text = o.ToString()
                End Using
            Catch ex As Exception
                ' ignorar
            End Try

            Try
                Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tecnicos WHERE Disponible = 1", conn)
                    Dim t = cmd.ExecuteScalar()
                    If t IsNot Nothing Then lblValTechs.Text = t.ToString()
                End Using
            Catch ex As Exception
            End Try

            Try
                Using cmd As New MySqlCommand("SELECT COUNT(*) FROM citas WHERE Fecha >= CURDATE()", conn)
                    Dim c = cmd.ExecuteScalar()
                    If c IsNot Nothing Then lblValAppointments.Text = c.ToString()
                End Using
            Catch ex As Exception
            End Try

            ' Cargar actividades recientes si existe tabla 'actividades' con columnas Descripcion y Fecha
            Try
                activityFlowPanel.Controls.Clear()
                Using cmd As New MySqlCommand("SELECT Fecha, Descripcion, Icono FROM actividades ORDER BY Fecha DESC LIMIT 10", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Dim fecha = If(IsDBNull(rdr("Fecha")), "", Convert.ToDateTime(rdr("Fecha")).ToString("HH:mm"))
                            Dim desc = If(IsDBNull(rdr("Descripcion")), "", rdr("Descripcion").ToString())
                            Dim icon = If(Not IsDBNull(rdr("Icono")), rdr("Icono").ToString(), "🔔")
                            Dim item = CreateActivityControl(icon, String.Format("{0}: {1}", fecha, desc))
                            activityFlowPanel.Controls.Add(item)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                ' si falla, deja las actividades de ejemplo
            End Try

        Catch ex As Exception
            ' no conectó; mantener valores por defecto
        Finally
            Try
                ModuloConexion.Desconectar()
            Catch
            End Try
        End Try
    End Sub

    Private Function CreateActivityControl(icon As String, text As String) As Panel
        Dim it As New Panel With {.Width = activityFlowPanel.Width - 24, .Height = 60, .BackColor = Color.White, .BorderStyle = BorderStyle.None, .Margin = New Padding(0, 6, 0, 6)}
        Dim pic As New Label With {.Text = icon, .Font = New Font("Segoe UI Emoji", 12, FontStyle.Regular), .AutoSize = False, .Size = New Size(28, 28), .Location = New Point(6, 16), .ForeColor = Color.Gray}
        Dim lbl As New Label With {.Text = text, .Font = New Font("Segoe UI", 9, FontStyle.Regular), .ForeColor = Color.FromArgb(70, 70, 70), .Location = New Point(44, 12), .AutoSize = False, .Size = New Size(it.Width - 50, 36)}
        it.Controls.Add(pic)
        it.Controls.Add(lbl)
        Return it
    End Function

    Private Sub ApplyPrimaryColor()
        ' Actualiza colores relevantes del PanelContenido (números y botones)
        If lblValOrders IsNot Nothing Then lblValOrders.ForeColor = primaryColor
        If lblValTechs IsNot Nothing Then lblValTechs.ForeColor = primaryColor
        If lblValAppointments IsNot Nothing Then lblValAppointments.ForeColor = primaryColor

        For Each btn In PanelContenido.Controls.OfType(Of Button)()
            btn.BackColor = Color.FromArgb(240, 240, 240)
            btn.ForeColor = Color.FromArgb(40, 40, 40)
        Next
    End Sub

    Private Sub BtnConfig_Click(sender As Object, e As EventArgs)
        Using dlg As New ColorDialog()
            dlg.AllowFullOpen = True
            dlg.Color = primaryColor
            If dlg.ShowDialog() = DialogResult.OK Then
                primaryColor = dlg.Color
                ApplyPrimaryColor()
            End If
        End Using
    End Sub

    Private Sub FormMenuPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ActualizarEncabezado()
        InicializarPanelLateral()
        MostrarDashboardInicial()
    End Sub

    Private Sub lblTituloTop_Click(sender As Object, e As EventArgs) Handles lblTituloTop.Click

    End Sub

    Private Sub ActivityFlowPanel_Resize(sender As Object, e As EventArgs)
        Try
            If activityFlowPanel Is Nothing Then Return
            For Each ctrl As Control In activityFlowPanel.Controls
                ctrl.Width = Math.Max(100, activityFlowPanel.ClientSize.Width - 24)
                For Each child As Control In ctrl.Controls
                    If TypeOf child Is Label Then
                        child.Width = Math.Max(50, ctrl.Width - 50)
                    End If
                Next
            Next
        Catch
        End Try
    End Sub

    Private Sub PanelContenido_Paint(sender As Object, e As PaintEventArgs) Handles PanelContenido.Paint

    End Sub
End Class
