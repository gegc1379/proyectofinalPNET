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
            Case Else
                Dim lbl As New Label With {.Text = $"Has seleccionado: {opcionSeleccionada}", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 50, 60), .AutoSize = True, .Location = New Point(50, 50)}
                PanelContenido.Controls.Add(lbl)
        End Select
    End Sub

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
        ' Intenta conectar y cargar datos reales; si falla, mantiene valores por defecto
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
End Class
