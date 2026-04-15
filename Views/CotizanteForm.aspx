<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CotizanteForm.aspx.cs" Inherits="Inverix.Views.CotizanteForm" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Inverix - Portal Cotizante</title>
    <link href="../Styles/InverixStyle.css?v=2" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">

<!-- ========== HEADER ========== -->
<div class="header">
    <div class="header-brand">
        <span class="header-logo">INVERIX</span>
    </div>
    <div class="header-user">
        <asp:Label ID="lblUsuarioLogueado" runat="server" CssClass="user-name"></asp:Label>
        <asp:LinkButton ID="btnCerrarSesion" runat="server" CssClass="btn-logout" OnClick="btnCerrarSesion_Click">Cerrar Sesi&oacute;n</asp:LinkButton>
    </div>
</div>

<!-- ========== LAYOUT PRINCIPAL ========== -->
<div class="layout">

    <!-- SIDEBAR -->
    <div class="sidebar">
        <nav class="menu">
            <asp:LinkButton ID="lnkInicio" runat="server" CssClass="menu-link" OnClick="lnkInicio_Click">Inicio</asp:LinkButton>

            <h3 class="menu-title">Finanzas</h3>
            <asp:LinkButton ID="lnkFinanzas" runat="server" CssClass="menu-link" OnClick="lnkFinanzas_Click">Productos y Tasas</asp:LinkButton>

            <h3 class="menu-title">Cotizaciones</h3>
            <asp:LinkButton ID="lnkNuevaCotizacion" runat="server" CssClass="menu-link" OnClick="lnkNuevaCotizacion_Click">Nueva Cotizaci&oacute;n</asp:LinkButton>
            <asp:LinkButton ID="lnkMisCotizaciones" runat="server" CssClass="menu-link" OnClick="lnkMisCotizaciones_Click">Mis Cotizaciones</asp:LinkButton>

            <h3 class="menu-title">Reportes</h3>
            <asp:LinkButton ID="lnkReportes" runat="server" CssClass="menu-link" OnClick="lnkReportes_Click">Mis Reportes</asp:LinkButton>

            <h3 class="menu-title">Mi Cuenta</h3>
            <asp:LinkButton ID="lnkMiPerfil" runat="server" CssClass="menu-link" OnClick="lnkMiPerfil_Click">Mi Perfil</asp:LinkButton>
        </nav>
    </div>

    <!-- CONTENIDO -->
    <div class="content">

        <!-- ================= INICIO ================= -->
        <asp:Panel ID="pnlInicio" runat="server" Visible="true">
            <div class="card">
                <div class="card-header"><h3 class="card-title">Bienvenido a Inverix</h3></div>
                <div style="padding:20px;">
                    <p class="content-text">Hola, <strong><asp:Label ID="lblNombreCompleto" runat="server"></asp:Label></strong>. Desde este portal puedes consultar productos, generar cotizaciones y revisar tus reportes de inversión.</p>
                    <div style="display:flex; gap:20px; flex-wrap:wrap; margin-top:20px;">
                        <div class="card" style="flex:1; min-width:180px; text-align:center; padding:20px;">
                            <div style="font-size:2rem;">&#128196;</div>
                            <p style="font-weight:bold; color:#1693A5;">Nueva Cotización</p>
                            <asp:LinkButton ID="lnkInicioNuevaCot" runat="server" CssClass="btn-primary" OnClick="lnkNuevaCotizacion_Click">Crear</asp:LinkButton>
                        </div>
                        <div class="card" style="flex:1; min-width:180px; text-align:center; padding:20px;">
                            <div style="font-size:2rem;">&#128203;</div>
                            <p style="font-weight:bold; color:#1693A5;">Mis Cotizaciones</p>
                            <asp:LinkButton ID="lnkInicioMisCot" runat="server" CssClass="btn-primary" OnClick="lnkMisCotizaciones_Click">Ver</asp:LinkButton>
                        </div>
                        <div class="card" style="flex:1; min-width:180px; text-align:center; padding:20px;">
                            <div style="font-size:2rem;">&#128202;</div>
                            <p style="font-weight:bold; color:#1693A5;">Mis Reportes</p>
                            <asp:LinkButton ID="lnkInicioReportes" runat="server" CssClass="btn-primary" OnClick="lnkReportes_Click">Ver</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- ================= FINANZAS (SOLO LECTURA) ================= -->
        <asp:Panel ID="pnlFinanzas" runat="server" Visible="false">
            <div class="card">
                <div class="card-header"><h3 class="card-title">Productos y Tasas Disponibles</h3></div>
                <p class="content-text" style="padding:0 15px; color:#666;">Consulta los productos de inversión y sus tasas vigentes.</p>

                <h4 style="color:#1693A5; padding:0 15px;">Tasas Activas</h4>
                <asp:GridView ID="gvTasasInfo" runat="server" AutoGenerateColumns="False" CssClass="table-modern">
                    <Columns>
                        <asp:BoundField DataField="Producto" HeaderText="Producto" />
                        <asp:BoundField DataField="Moneda" HeaderText="Moneda" />
                        <asp:BoundField DataField="Plazo" HeaderText="Plazo" />
                        <asp:BoundField DataField="Tasa" HeaderText="Tasa %" DataFormatString="{0:N2}%" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- ================= NUEVA COTIZACIÓN ================= -->
        <asp:Panel ID="pnlNuevaCotizacion" runat="server" Visible="false">
            <div class="card">
                <div class="card-header"><h3 class="card-title">Nueva Cotizaci&oacute;n</h3></div>

                <asp:Label ID="lblMensajeNuevaCot" runat="server" Visible="false"
                    style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold; margin:10px 15px;"></asp:Label>

                <div style="padding:15px;">
                    <div class="form-group">
                        <label>Tasa / Producto:</label>
                        <asp:DropDownList ID="ddlTasaCot" runat="server" CssClass="form-input" DataTextField="Descripcion" DataValueField="IdTasa"></asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label>Monto a invertir:</label>
                        <asp:TextBox ID="txtMontoCot" runat="server" CssClass="form-input" placeholder="Ej: 1000000"></asp:TextBox>
                    </div>

                    <div style="margin-top:15px;">
                        <asp:Button ID="btnGenerarCotizacion" runat="server" Text="Generar Cotizaci&oacute;n" CssClass="btn-primary" OnClick="btnGenerarCotizacion_Click" />
                        <asp:Button ID="btnLimpiarCot" runat="server" Text="Limpiar" CssClass="btn-secondary" OnClick="btnLimpiarCot_Click" formnovalidate="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- ================= MIS COTIZACIONES ================= -->
        <asp:Panel ID="pnlMisCotizaciones" runat="server" Visible="false">
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Mis Cotizaciones</h3>
                    <asp:Button ID="btnImprimirMisCot" runat="server" Text="&#128438; Imprimir" CssClass="btn-secondary" OnClientClick="imprimirMisCotizaciones(); return false;" />
                </div>

                <asp:Label ID="lblMensajeMisCot" runat="server" Visible="false"
                    style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold; margin:10px 0;"></asp:Label>

                <asp:GridView ID="gvMisCotizaciones" runat="server" AutoGenerateColumns="False"
                    CssClass="table-modern" DataKeyNames="IdCotizacion" OnRowCommand="gvMisCotizaciones_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="NumeroCotizacion" HeaderText="Nro. Cotizaci&oacute;n" />
                        <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                        <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="MontoTotalNeto" HeaderText="Total Neto" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnVerDetalles" runat="server" CommandName="VerDetalles" CommandArgument='<%# Eval("IdCotizacion") %>' CssClass="btn-info" Text="Ver Detalle"></asp:LinkButton>
                                <asp:LinkButton ID="btnEditarCot" runat="server" CommandName="EditarCotizacion" CommandArgument='<%# Eval("IdCotizacion") %>' CssClass="btn-primary" Text="Editar" style="margin-left:5px;"></asp:LinkButton>
                                <asp:LinkButton ID="btnInactivarCot" runat="server" CommandName="InactivarCotizacion" CommandArgument='<%# Eval("IdCotizacion") %>' CssClass="btn-danger" Text="Inactivar" OnClientClick="return confirm('¿Está seguro de inactivar esta cotización?');" style="margin-left:5px;"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <!-- Panel Editar Cotización (Mismo estilo que el Admin) -->
                <asp:Panel ID="pnlEditarCotizacion" runat="server" Visible="false" style="margin-top:20px; border-top:2px solid #1693A5; padding-top:15px; background:#f9f9f9; padding:20px; border-radius:8px;">
                    <h4 style="color:#1693A5; margin-bottom:15px;">Modificar Cotización</h4>
                    <asp:HiddenField ID="hfIdCotizacionEditar" runat="server" />
                    
                    <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                        <div class="form-group">
                            <label>Tasa / Producto / Plazo:</label>
                            <asp:DropDownList ID="ddlTasaEdicion" runat="server" CssClass="form-input"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Nuevo Monto:</label>
                            <asp:TextBox ID="txtMontoEdicion" runat="server" CssClass="form-input" TextMode="Number" step="0.01"></asp:TextBox>
                        </div>
                    </div>

                    <div style="margin-top:20px;">
                        <asp:Button ID="btnActualizarCotizacion" runat="server" Text="Guardar Cambios" CssClass="btn-primary" OnClick="btnActualizarCotizacion_Click" />
                        <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar" CssClass="btn-secondary" OnClick="btnCancelarEdicion_Click" style="margin-left:10px;" />
                    </div>
                </asp:Panel>

                <!-- Detalles Mensuales -->
                <asp:Panel ID="pnlDetallesCot" runat="server" Visible="false" style="margin-top:20px; border-top:2px solid #ddd; padding-top:15px;">
                    <h4 style="color:#1693A5;">Detalles Mensuales</h4>
                    <asp:HiddenField ID="hfIdCotizacionCot" runat="server" />

                    <asp:GridView ID="gvDetallesMensualesCot" runat="server" AutoGenerateColumns="False"
                        CssClass="table-modern" DataKeyNames="IdCotizacion,NumeroMes"
                        ShowFooter="True"
                        OnRowDataBound="gvDetallesMensualesCot_RowDataBound"
                        OnDataBound="gvDetallesMensualesCot_DataBound">
                        <Columns>
                            <asp:BoundField DataField="NumeroMes" HeaderText="Mes" />
                            <asp:BoundField DataField="InteresBruto" HeaderText="Inter&eacute;s Bruto" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="Impuesto" HeaderText="Impuesto (13%)" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="InteresNeto" HeaderText="Inter&eacute;s Neto" DataFormatString="{0:N2}" />
                        </Columns>
                    </asp:GridView>

                    <div style="margin-top:15px;">
                        <asp:Button ID="btnCerrarDetalleCot" runat="server" Text="Cerrar Detalles" CssClass="btn-secondary" OnClick="btnCerrarDetalleCot_Click" />
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>

        <!-- ================= REPORTES ================= -->
        <asp:Panel ID="pnlReportes" runat="server" Visible="false">
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Mis Reportes</h3>
                    <div style="display:flex; gap:10px;">
                        <asp:Button ID="btnReportePDF" runat="server" Text="&#128438; Imprimir Reporte" CssClass="btn-secondary" OnClientClick="imprimirReporte(); return false;" />
                        <asp:Button ID="btnReporteExcel" runat="server" Text="Exportar Excel" CssClass="btn-secondary" OnClick="btnReporteExcel_Click" />
                    </div>
                </div>

                <!-- Filtros de Reporte -->
                <div style="display:flex; align-items:center; gap:10px; flex-wrap:wrap; margin:15px; padding:10px; background:#f9f9f9; border:1px solid #eee; border-radius:6px;">
                    <label style="font-weight:bold;">Desde:</label>
                    <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-input" TextMode="Date" style="max-width:160px;"></asp:TextBox>
                    <label style="font-weight:bold;">Hasta:</label>
                    <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-input" TextMode="Date" style="max-width:160px;"></asp:TextBox>
                    <asp:Button ID="btnFiltrarReporte" runat="server" Text="Filtrar" CssClass="btn-primary" OnClick="btnFiltrarReporte_Click" />
                    <asp:Button ID="btnVerTodoReporte" runat="server" Text="Ver Todos" CssClass="btn-secondary" OnClick="btnVerTodoReporte_Click" />
                </div>

                <asp:Label ID="lblMensajeReporte" runat="server" Visible="false"
                    style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold; margin:0 15px 10px;"></asp:Label>

                <asp:GridView ID="gvReporte" runat="server" AutoGenerateColumns="False" CssClass="table-modern" DataKeyNames="IdCotizacion">
                    <Columns>
                        <asp:BoundField DataField="NumeroCotizacion" HeaderText="Nro. Cotizaci&oacute;n" />
                        <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                        <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="MontoTotalNeto" HeaderText="Total Neto" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="UsuarioCreacion" HeaderText="Creado Por" />
                        <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creaci&oacute;n" DataFormatString="{0:dd/MM/yyyy}" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- ================= MI PERFIL ================= -->
        <asp:Panel ID="pnlMiPerfil" runat="server" Visible="false">
            <div class="card">
                <div class="card-header"><h3 class="card-title">Mi Perfil</h3></div>

                <asp:Label ID="lblMensajePerfil" runat="server" Visible="false"
                    style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold; margin:10px 15px;"></asp:Label>

                <div style="padding:15px;">
                    <h4 style="color:#1693A5;">Datos Personales</h4>
                    <table style="width:100%; border-collapse:collapse; margin-bottom:20px;">
                        <tr><td style="padding:8px; font-weight:bold; width:180px;">Nombre:</td><td><asp:Label ID="lblPerfilNombre" runat="server"></asp:Label></td></tr>
                        <tr><td style="padding:8px; font-weight:bold;">Identificaci&oacute;n:</td><td><asp:Label ID="lblPerfilIdentificacion" runat="server"></asp:Label></td></tr>
                        <tr><td style="padding:8px; font-weight:bold;">Correo:</td><td><asp:Label ID="lblPerfilCorreo" runat="server"></asp:Label></td></tr>
                        <tr><td style="padding:8px; font-weight:bold;">Tel&eacute;fono:</td><td><asp:Label ID="lblPerfilTelefono" runat="server"></asp:Label></td></tr>
                        <tr><td style="padding:8px; font-weight:bold;">Usuario:</td><td><asp:Label ID="lblPerfilUsuario" runat="server"></asp:Label></td></tr>
                    </table>

                    <h4 style="color:#1693A5; border-top:1px solid #eee; padding-top:15px;">Cambiar Contrase&ntilde;a</h4>
                    <div class="form-group">
                        <label>Contrase&ntilde;a Actual:</label>
                        <asp:TextBox ID="txtClaveActual" runat="server" CssClass="form-input" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Nueva Contrase&ntilde;a (m&iacute;nimo 6 caracteres):</label>
                        <asp:TextBox ID="txtClaveNueva" runat="server" CssClass="form-input" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Confirmar Nueva Contrase&ntilde;a:</label>
                        <asp:TextBox ID="txtClaveConfirmar" runat="server" CssClass="form-input" TextMode="Password"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnCambiarClave" runat="server" Text="Actualizar Contrase&ntilde;a" CssClass="btn-primary" OnClick="btnCambiarClave_Click" />
                </div>
            </div>
        </asp:Panel>

    </div><!-- fin .content -->
</div><!-- fin .layout -->

<script type="text/javascript">
    function imprimirMisCotizaciones() {
        var pw = window.open('', 'Print', 'width=900,height=700');
        pw.document.write('<html><head><title>Mis Cotizaciones - Inverix</title>');
        pw.document.write('<style>body{font-family:Arial,sans-serif;padding:20px}table{border-collapse:collapse;width:100%}th{background:#1693A5;color:#fff;padding:8px}td{border:1px solid #ddd;padding:7px}tfoot td{background:#1693A5;color:#fff;font-weight:bold}h2{color:#1693A5;border-bottom:2px solid #1693A5;padding-bottom:8px}</style></head><body>');
        pw.document.write('<h2>Mis Cotizaciones &mdash; Inverix</h2>');
        var t = document.getElementById('<%= gvMisCotizaciones.ClientID %>');
        if (t) { var c = t.cloneNode(true); c.querySelectorAll('tr > *:last-child').forEach(function(x){x.style.display='none';}); pw.document.write(c.outerHTML); }
        var d = document.getElementById('<%= pnlDetallesCot.ClientID %>');
        if (d && d.style.display !== 'none') {
            var gd = document.getElementById('<%= gvDetallesMensualesCot.ClientID %>');
            if (gd) { pw.document.write('<h4 style="color:#1693A5;margin-top:24px">Detalles Mensuales</h4>'); pw.document.write(gd.outerHTML); }
        }
        pw.document.write('<p style="margin-top:24px;font-size:11px;color:#888">Generado: ' + new Date().toLocaleString() + '</p></body></html>');
        pw.document.close(); pw.focus(); setTimeout(function(){ pw.print(); }, 400);
    }

    function imprimirReporte() {
        var pw = window.open('', 'Print', 'width=900,height=700');
        pw.document.write('<html><head><title>Reporte - Inverix</title>');
        pw.document.write('<style>body{font-family:Arial,sans-serif;padding:20px}table{border-collapse:collapse;width:100%}th{background:#1693A5;color:#fff;padding:8px}td{border:1px solid #ddd;padding:7px}h2{color:#1693A5;border-bottom:2px solid #1693A5;padding-bottom:8px}</style></head><body>');
        pw.document.write('<h2>Reporte de Cotizaciones &mdash; Inverix</h2>');
        var t = document.getElementById('<%= gvReporte.ClientID %>');
        if (t) pw.document.write(t.outerHTML);
        pw.document.write('<p style="margin-top:24px;font-size:11px;color:#888">Generado: ' + new Date().toLocaleString() + '</p></body></html>');
        pw.document.close(); pw.focus(); setTimeout(function(){ pw.print(); }, 400);
    }
</script>

</form>
</body>
</html>
