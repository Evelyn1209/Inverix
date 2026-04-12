<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InverixForm.aspx.cs" Inherits="Inverix.Views.InverixForm" OnPreRender="Page_PreRender" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Inverix - Gestion Financiera</title>
<link rel="icon" type="image/x-icon" href="../Images/iconlogo.png" />
    <link href="../Styles/InverixStyle.css" rel="stylesheet" />

</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <!-- ================= LOGIN ================= usuario: admin  password: admin123 -->
        <asp:Panel ID="pnlLogin" runat="server" CssClass="login-panel">

            <div class="login-container">

                <asp:Image ID="imgLogoLogin"
      runat="server"
    ImageUrl="../Images/logo.png"
    CssClass="logo-login" />

                <h1 class="app-title">INVERIX</h1>
                <h3 class="app-subtitle">
                    Sistema de Gestion de Cotizaciones 
                </h3>

                <hr class="divider" /> 


                <div class="form-group">
                    <asp:Label ID="lblUsuario" runat="server" 
                        Text="Usuario:" CssClass="form-label"></asp:Label>

                    <asp:TextBox ID="txtUsuario" runat="server" 
                        CssClass="form-input" autocomplete="off" oninput="document.getElementById('lblMensaje').innerText = '';"></asp:TextBox>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblClave" runat="server" 
                        Text="Contrase;a:" CssClass="form-label"></asp:Label>

                    <asp:TextBox ID="txtClave" runat="server" 
                        TextMode="Password" CssClass="form-input" autocomplete="new-password" oninput="document.getElementById('lblMensaje').innerText = '';"></asp:TextBox>
                </div>

                <asp:Button ID="btnLogin" runat="server"
                    Text="Iniciar Sesion"
                    CssClass="btn-primary"
                    OnClick="btnLogin_Click" />

                <asp:Label ID="lblMensaje" runat="server" 
                    CssClass="error-message"></asp:Label>

            </div>
        </asp:Panel>


        <!-- ================= SISTEMA PRINCIPAL ================= -->
        <asp:Panel ID="pnlSistema" runat="server" 
            Visible="false" CssClass="system-panel">

            <!-- HEADER -->
            <div class="header">

                <div class="header-left">
<div class="header-brand">

    <asp:Image ID="imgLogoHeader"
        runat="server"
        ImageUrl="../Images/iconlogo.png"
        CssClass="logo-header" />

    <h1 class="app-title">INVERIX</h1>

</div>                </div>

                <div class="header-right">
                    <asp:Label ID="lblUsuarioLogueado" 
                        runat="server" 
                        Text="Administrador"
                        CssClass="user-label">
                    </asp:Label>
                </div>

            </div>

            <asp:Label ID="lblMensajeSistema" runat="server" 
                CssClass="error-message" style="display:block; margin: 10px 20px; padding: 10px; border-radius: 4px; background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; text-align: center; font-weight: bold;" Visible="false"></asp:Label>

            <hr class="divider" />

            <!-- CONTENEDOR GENERAL -->
            <div class="main-container">

                <!-- SIDEBAR -->
                <div class="sidebar">

                    <h3 class="menu-title">Inicio</h3>
                    <asp:LinkButton ID="lnkInicio" runat="server" CssClass="menu-link" OnClick="lnkInicio_Click">Inicio</asp:LinkButton>

                    <h3 class="menu-title">Administraci&oacute;n</h3>
                    <asp:LinkButton ID="lnkUsuarios" runat="server" CssClass="menu-link" OnClick="lnkUsuarios_Click">Usuarios</asp:LinkButton>
                    <asp:LinkButton ID="lnkRoles" runat="server" CssClass="menu-link" OnClick="lnkRoles_Click">Roles</asp:LinkButton>

                    <h3 class="menu-title">Finanzas</h3>
                    <asp:LinkButton ID="lnkMonedas" runat="server" CssClass="menu-link" OnClick="lnkMonedas_Click">Monedas</asp:LinkButton>
                    <asp:LinkButton ID="lnkProductos" runat="server" CssClass="menu-link" OnClick="lnkProductos_Click">Productos</asp:LinkButton>
                    <asp:LinkButton ID="lnkTasas" runat="server" CssClass="menu-link" OnClick="lnkTasas_Click">Tasas</asp:LinkButton>
                    <asp:LinkButton ID="lnkPlazos" runat="server" CssClass="menu-link" OnClick="lnkPlazos_Click">Plazos</asp:LinkButton>

                    <h3 class="menu-title">Clientes</h3>
                    <asp:LinkButton ID="lnkCotizantes" runat="server" CssClass="menu-link" OnClick="lnkCotizantes_Click">Cotizantes</asp:LinkButton>

                    <h3 class="menu-title">Cotizaciones</h3>
                    <asp:LinkButton ID="lnkNuevaCotizacion" runat="server" CssClass="menu-link" OnClick="lnkNuevaCotizacion_Click">Nueva Cotizacion</asp:LinkButton>
                    <asp:LinkButton ID="lnkListaCotizaciones" runat="server" CssClass="menu-link" OnClick="lnkListaCotizaciones_Click">Mis Cotizaciones</asp:LinkButton>

                    <div class="sidebar-footer">
                        <asp:Button ID="btnCerrarSesion" runat="server"
                            Text="Cerrar Sesi&oacute;n"
                            CssClass="btn-logout"
                            OnClick="btnCerrarSesion_Click" />
                    </div>

                </div>

                <!-- CONTENIDO CENTRAL -->
                <div class="content">

                    <asp:Panel ID="pnlInicio" runat="server">

    <h2 class="welcome-title">
        Bienvenido a INVERIX
    </h2>

    <asp:Label ID="Label1" runat="server"
        CssClass="content-text"
        Text="INVERIX es una plataforma inteligente dise&ntilde;ada para la gesti&oacute;n eficiente de cotizaciones e inversiones financieras.">
    </asp:Label>

    <asp:Label ID="Label2" runat="server"
        CssClass="content-text"
        Text="Desde este sistema podr&aacute; administrar productos financieros, configurar tasas y plazos, generar cotizaciones y consultar reportes estrat&eacute;gicos.">
    </asp:Label>

</asp:Panel>

                   

                <!-- ================= PRODUCTOS ================= -->

<asp:Panel ID="pnlProductos" runat="server" Visible="false">

    <div class="card">

        <div class="card-header">
            <h3 class="card-title">Control de Productos</h3>

            <asp:Button ID="btnNuevoProducto"
                runat="server"
                Text="+ Nuevo Producto"
                CssClass="btn-primary"
                OnClick="btnNuevoProducto_Click" />
        </div>

        <!-- Mensaje de éxito/error local para Productos (se auto-oculta con JS) -->
        <div style="margin-bottom:10px;">
            <asp:Label ID="lblMensajeProducto" runat="server" Visible="false"
                style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold;"></asp:Label>
        </div>

        <script type="text/javascript">
            function autoOcultarMensajeProducto() {
                var lbl = document.getElementById('<%= lblMensajeProducto.ClientID %>');
                if (lbl && lbl.innerText.trim() !== '') {
                    setTimeout(function () {
                        lbl.style.transition = 'opacity 0.8s';
                        lbl.style.opacity = '0';
                        setTimeout(function () { lbl.style.display = 'none'; }, 800);
                    }, 3000);
                }
            }
            window.addEventListener('load', autoOcultarMensajeProducto);
        </script>

        <asp:GridView ID="gvProductos"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="table-modern"
            DataKeyNames="IdProducto"
            OnRowEditing="gvProductos_RowEditing"
            OnRowDeleting="gvProductos_RowDeleting">

            <Columns>

                <asp:BoundField DataField="IdProducto"
                    HeaderText="ID" />

                <asp:BoundField DataField="Nombre"
                    HeaderText="Nombre" />

                <asp:BoundField DataField="Codigo"
                    HeaderText="Codigo" />

                <asp:TemplateField HeaderText="Moneda">
                    <ItemTemplate>
                        <%# Eval("NombreMoneda") %> 
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class='<%# Convert.ToBoolean(Eval("Estado")) ? "badge-active" : "badge-inactive" %>'>
                            <%# Convert.ToBoolean(Eval("Estado")) ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

  <asp:BoundField DataField="UsuarioCreacion" HeaderText="Creado Por" />
  <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creaci&oacute;n" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
  <asp:BoundField DataField="UsuarioModificacion" HeaderText="Modificado Por" />
  <asp:BoundField DataField="FechaModificacion" HeaderText="Fecha Modificacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />

  <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="&#9998;" DeleteText="&#128465;" ControlStyle-CssClass="btn-action" />

            </Columns>

        </asp:GridView>

        <!-- Formulario Crear/Editar -->
        <asp:Panel ID="pnlFormProducto" runat="server" Visible="false" CssClass="form-container" style="margin-top:20px; padding:15px; border:1px solid #ddd; background-color:#f9f9f9;">
            <h4 id="lblTituloFormularioProducto" runat="server" style="color:#1693A5; margin-top:0;">Nuevo Producto</h4>
            
            <asp:HiddenField ID="hfIdProducto" runat="server" />
            
            <div class="form-group">
                <label>Codigo:</label>
                <asp:TextBox ID="txtCodigoProducto" runat="server" CssClass="form-input"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Nombre del Producto:</label>
                <asp:TextBox ID="txtNombreProducto" runat="server" CssClass="form-input"></asp:TextBox>
            </div>

            <div class="form-group" id="divEstadoProducto" runat="server" visible="false">
    <label>Estado Activo:</label>
    <asp:CheckBox ID="chkEstadoProducto" runat="server" Checked="true" />
</div>

            <div class="form-group">
                <label>Moneda:</label>
                <asp:DropDownList ID="ddlMonedaProducto" runat="server" CssClass="form-input" DataTextField="Nombre" DataValueField="IdMoneda">
                </asp:DropDownList>
            </div>

            <div style="margin-top: 15px;">
                <asp:Button ID="btnGuardarProducto" runat="server" Text="Guardar" CssClass="btn-primary" OnClick="btnGuardarProducto_Click" />
                <asp:Button ID="btnCancelarProducto" runat="server" Text="Cancelar" CssClass="btn-secondary" OnClick="btnCancelarProducto_Click" formnovalidate="true" />
            </div>
        </asp:Panel>

    </div>

</asp:Panel>

<!-- ================= USUARIOS ================= -->
<asp:Panel ID="pnlUsuarios" runat="server" Visible="false">
    <div class="card">
        <div class="card-header"><h3 class="card-title">Gestion de Usuarios</h3></div>
        <p class="content-text">Modulo para administrar el acceso al sistema.</p>
    </div>
</asp:Panel>

<!-- ================= ROLES ================= -->
<asp:Panel ID="pnlRoles" runat="server" Visible="false">
    <div class="card">
        <div class="card-header"><h3 class="card-title">Gestion de Roles</h3></div>
        <p class="content-text">Definicion de permisos y perfiles de usuario.</p>
    </div>
</asp:Panel>

<!-- ================= MONEDAS ================= -->
<asp:Panel ID="pnlMonedas" runat="server" Visible="false">
    <div class="card">
        <div class="card-header">
            <h3 class="card-title">Control de Monedas</h3>
            <asp:Button ID="btnNuevaMoneda" runat="server" Text="+ Nueva Moneda" CssClass="btn-primary" OnClick="btnNuevaMoneda_Click" />
        </div>

        <!-- Mensaje de éxito/error local (se auto-oculta con JS) -->
        <div style="margin-bottom:10px;">
            <asp:Label ID="lblMensajeMoneda" runat="server" Visible="false"
                style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold;"></asp:Label>
        </div>

        <script type="text/javascript">
            function autoOcultarMensajeMoneda() {
                var lbl = document.getElementById('<%= lblMensajeMoneda.ClientID %>');
                if (lbl && lbl.innerText.trim() !== '') {
                    setTimeout(function () {
                        lbl.style.transition = 'opacity 0.8s';
                        lbl.style.opacity = '0';
                        setTimeout(function () { lbl.style.display = 'none'; }, 800);
                    }, 3000);
                }
            }
            window.addEventListener('load', autoOcultarMensajeMoneda);
        </script>

        <asp:GridView ID="gvMonedas" runat="server" AutoGenerateColumns="False" CssClass="table-modern" DataKeyNames="IdMoneda" OnRowEditing="gvMonedas_RowEditing" OnRowDeleting="gvMonedas_RowDeleting">
            <Columns>
                <asp:BoundField DataField="IdMoneda" HeaderText="ID" />
                <asp:BoundField DataField="Codigo" HeaderText="Codigo" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Simbolo" HeaderText="S&iacute;mbolo" />
                
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class='<%# Convert.ToBoolean(Eval("Estado")) ? "badge-active" : "badge-inactive" %>'>
                            <%# Convert.ToBoolean(Eval("Estado")) ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:BoundField DataField="UsuarioCreacion" HeaderText="Creado Por" />
                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:BoundField DataField="UsuarioModificacion" HeaderText="Modificado Por" />
                <asp:BoundField DataField="FechaModificacion" HeaderText="Fecha Modificacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="&#9998;" DeleteText="&#128465;" ControlStyle-CssClass="btn-action" />
            </Columns>
        </asp:GridView>

        <!-- Formulario Crear/Editar Moneda -->
        <asp:Panel ID="pnlFormMoneda" runat="server" Visible="false" CssClass="form-container" style="margin-top:20px; padding:15px; border:1px solid #ddd; background-color:#f9f9f9;">
            <h4 id="lblTituloFormularioMoneda" runat="server" style="color:#1693A5; margin-top:0;">Nueva Moneda</h4>
            <asp:HiddenField ID="hfIdMoneda" runat="server" />
            
            <div class="form-group">
                <label>Codigo ISO (Ej. CRC):</label>
                <asp:TextBox ID="txtCodigoMoneda" runat="server" CssClass="form-input"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Nombre Completo:</label>
                <asp:TextBox ID="txtNombreMoneda" runat="server" CssClass="form-input"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Simbolo:</label>
                <asp:TextBox ID="txtSimboloMoneda" runat="server" CssClass="form-input" MaxLength="5"></asp:TextBox>
            </div>

            <div class="form-group" id="divEstadoMoneda" runat="server" visible="false">
                <label>Estado Activo:</label>
                <asp:CheckBox ID="chkEstadoMoneda" runat="server" Checked="true" />
            </div>

            <div style="margin-top: 15px;">
                <asp:Button ID="btnGuardarMoneda" runat="server" Text="Guardar" CssClass="btn-primary" OnClick="btnGuardarMoneda_Click" />
                <asp:Button ID="btnCancelarMoneda" runat="server" Text="Cancelar" CssClass="btn-secondary" OnClick="btnCancelarMoneda_Click" formnovalidate="true" />
            </div>
        </asp:Panel>
    </div>
</asp:Panel>

<!-- ================= TASAS ================= -->
<asp:Panel ID="pnlTasas" runat="server" Visible="false">
    <div class="card">
        <div class="card-header">
            <h3 class="card-title">Control de Tasas</h3>
            <asp:Button ID="btnNuevaTasa" runat="server" Text="+ Nueva Tasa" CssClass="btn-primary" OnClick="btnNuevaTasa_Click" />
        </div>

        <!-- Mensaje de éxito/error (se auto-oculta con JS) -->
        <div style="margin-bottom:10px;">
            <asp:Label ID="lblMensajeTasas" runat="server" Visible="false"
                style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold;"></asp:Label>
        </div>

        <script type="text/javascript">
            function autoOcultarMensajeTasas() {
                var lbl = document.getElementById('<%= lblMensajeTasas.ClientID %>');
                if (lbl && lbl.innerText.trim() !== '') {
                    setTimeout(function () {
                        lbl.style.transition = 'opacity 0.8s';
                        lbl.style.opacity = '0';
                        setTimeout(function () { lbl.style.display = 'none'; }, 800);
                    }, 3000);
                }
            }
            window.addEventListener('load', autoOcultarMensajeTasas);
        </script>

        <asp:GridView ID="gvTasas" runat="server" AutoGenerateColumns="False" CssClass="table-modern" DataKeyNames="IdTasa" OnRowEditing="gvTasas_RowEditing" OnRowDeleting="gvTasas_RowDeleting">
            <Columns>
                <asp:BoundField DataField="IdTasa" HeaderText="ID" />
                
                <asp:TemplateField HeaderText="Producto">
                    <ItemTemplate>
                        <%# Eval("NombreProducto") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Plazo (Dias)">
                    <ItemTemplate>
                        <%# Eval("PlazoDias") %> dias
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Porcentaje" HeaderText="Tasa (%)" DataFormatString="{0:N2} %"/>

                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class='<%# Convert.ToBoolean(Eval("Estado")) ? "badge-active" : "badge-inactive" %>'>
                            <%# Convert.ToBoolean(Eval("Estado")) ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

               <asp:BoundField DataField="UsuarioCreacion" HeaderText="Creado Por" />
<asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
<asp:BoundField DataField="UsuarioModificacion" HeaderText="Modificado Por" />
<asp:BoundField DataField="FechaModificacion" HeaderText="Fecha Modificacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />

<asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="&#9998;" DeleteText="&#128465;" ControlStyle-CssClass="btn-action" />
            </Columns>
        </asp:GridView>

        <div id="seccionFormularioTasa"></div>
        <asp:Panel ID="pnlFormTasa" runat="server" Visible="false" CssClass="form-container" style="margin-top:20px; padding:15px; border:1px solid #ddd; background-color:#f9f9f9;">
            <h4 id="lblTituloFormularioTasa" runat="server" style="color:#003366; margin-top:0;">Nueva Tasa</h4>
            <asp:HiddenField ID="hfIdTasa" runat="server" />
            
            <div class="form-group">
                <label>Producto:</label>
                <asp:DropDownList ID="ddlProductoTasa" runat="server" CssClass="form-input" DataTextField="Nombre" DataValueField="IdProducto">
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label>Plazo:</label>
                <asp:DropDownList ID="ddlPlazoTasa" runat="server" CssClass="form-input" DataTextField="Descripcion" DataValueField="IdPlazo">
                </asp:DropDownList>
            </div>
            
            <div class="form-group">
                <label>Porcentaje de Tasa (Ej: 3.15 para 3.15%):</label>
                <asp:TextBox ID="txtPorcentajeTasa" runat="server" CssClass="form-input" placeholder="0.0000"></asp:TextBox>
            </div>

            <div class="form-group" id="divEstadoTasa" runat="server" visible="false">
                <label>Estado Activo:</label>
                <asp:CheckBox ID="chkEstadoTasa" runat="server" Checked="true" />
            </div>

            <div style="margin-top: 15px;">
                <asp:Button ID="btnGuardarTasa" runat="server" Text="Guardar" CssClass="btn-primary" OnClick="btnGuardarTasa_Click" />
                <asp:Button ID="btnCancelarTasa" runat="server" Text="Cancelar" CssClass="btn-secondary" OnClick="btnCancelarTasa_Click" formnovalidate="true" />
            </div>
        </asp:Panel>
    </div>
</asp:Panel>

<!-- ================= PLAZOS ================= -->

<asp:Panel ID="pnlPlazos" runat="server" Visible="false">
    <div class="card">
        <div class="card-header">
            <h3 class="card-title">Control de Plazos</h3>
            <asp:Button ID="btnNuevoPlazo" runat="server" Text="+ Nuevo Plazo" CssClass="btn-primary" OnClick="btnNuevoPlazo_Click" />
        </div>

        <!-- Mensaje de éxito/error local para Plazos (se auto-oculta con JS) -->
        <div style="margin-bottom:10px;">
            <asp:Label ID="lblMensajePlazo" runat="server" Visible="false"
                style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold;"></asp:Label>
        </div>

        <script type="text/javascript">
            function autoOcultarMensajePlazo() {
                var lbl = document.getElementById('<%= lblMensajePlazo.ClientID %>');
                if (lbl && lbl.innerText.trim() !== '') {
                    setTimeout(function () {
                        lbl.style.transition = 'opacity 0.8s';
                        lbl.style.opacity = '0';
                        setTimeout(function () { lbl.style.display = 'none'; }, 800);
                    }, 3000);
                }
            }
            window.addEventListener('load', autoOcultarMensajePlazo);
        </script>

        <asp:GridView ID="gvPlazos" runat="server" AutoGenerateColumns="False" CssClass="table-modern" DataKeyNames="IdPlazo" OnRowEditing="gvPlazos_RowEditing" OnRowDeleting="gvPlazos_RowDeleting">
            <Columns>
                <asp:BoundField DataField="IdPlazo" HeaderText="ID" />
                <asp:BoundField DataField="dias" HeaderText="Dias" />
                <asp:BoundField DataField="descripcion" HeaderText="Descripcion" />
                
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class='<%# Convert.ToBoolean(Eval("estado")) ? "badge-active" : "badge-inactive" %>'>
                            <%# Convert.ToBoolean(Eval("estado")) ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:BoundField DataField="UsuarioCreacion" HeaderText="Creado Por" />
                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:BoundField DataField="UsuarioModificacion" HeaderText="Modificado Por" />
                <asp:BoundField DataField="FechaModificacion" HeaderText="Fecha Modificacion" DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="&#9998;" DeleteText="&#128465;" ControlStyle-CssClass="btn-action" />
            </Columns>
        </asp:GridView>


        <div id="seccionFormulario"></div>
        <asp:Panel ID="pnlFormPlazo" runat="server" Visible="false" CssClass="form-container" style="margin-top:20px; padding:15px; border:1px solid #ddd; background-color:#f9f9f9;">
            <h4 id="lblTituloFormularioPlazo" runat="server" style="color:#003366; margin-top:0;">Nuevo Plazo</h4>
            <asp:HiddenField ID="hfIdPlazo" runat="server" />
            
<div class="form-group">
    <label>Cantidad de Días:</label>
    <asp:TextBox ID="txtDiasPlazo" runat="server" 
        TextMode="Number" ReadOnly="true"
        style="background-color: #e9ecef; color: #495057; cursor: not-allowed; border: 1px solid #ced4da; padding: 5px; border-radius: 4px;">
    </asp:TextBox>
</div>

<div class="form-group">
    <label>Cantidad de Meses:</label>
    <asp:TextBox ID="txtDescripcionPlazo" runat="server" CssClass="form-input" MaxLength="50">
    </asp:TextBox>
</div>

            <div class="form-group" id="divEstadoPlazo" runat="server" visible="true">
                <label>Estado Activo:</label>
                <asp:CheckBox ID="chkEstadoPlazo" runat="server" Checked="true" />
            </div>

            <div style="margin-top: 15px;">
                <asp:Button ID="btnGuardarPlazo" runat="server" Text="Guardar" CssClass="btn-primary" OnClick="btnGuardarPlazo_Click" />
                <asp:Button ID="btnCancelarPlazo" runat="server" Text="Cancelar" CssClass="btn-secondary" OnClick="btnCancelarPlazo_Click" formnovalidate="true" />
            </div>
        </asp:Panel>
    </div>
</asp:Panel>

<!-- ================= COTIZANTES ================= -->

<asp:Panel ID="pnlCotizantes" runat="server" Visible="false">
    <div class="card">
        <div class="card-header">
            <h3 class="card-title">Lista de Cotizantes</h3>
            <asp:Button ID="btnNuevoCotizante" runat="server" Text="+ Nuevo Cotizante" CssClass="btn-primary" OnClick="btnNuevoCotizante_Click" />
        </div>

        <!-- Mensaje de éxito/error local para Cotizantes (se auto-oculta con JS) -->
        <div style="margin-bottom:10px;">
            <asp:Label ID="lblMensajeCotizante" runat="server" Visible="false"
                style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold;"></asp:Label>
        </div>

        <script type="text/javascript">
            function autoOcultarMensajeCotizante() {
                var lbl = document.getElementById('<%= lblMensajeCotizante.ClientID %>');
                if (lbl && lbl.innerText.trim() !== '') {
                    setTimeout(function () {
                        lbl.style.transition = 'opacity 0.8s';
                        lbl.style.opacity = '0';
                        setTimeout(function () { lbl.style.display = 'none'; }, 800);
                    }, 3000);
                }
            }
            window.addEventListener('load', autoOcultarMensajeCotizante);
        </script>

        <asp:GridView ID="gvCotizantes" runat="server" AutoGenerateColumns="False" CssClass="table-modern" 
            DataKeyNames="IdCotizante" OnRowEditing="gvCotizantes_RowEditing" OnRowDeleting="gvCotizantes_RowDeleting">
            <Columns>
                <asp:BoundField DataField="Identificacion" HeaderText="Identificación" />
                <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre Completo" />
                
                <asp:TemplateField HeaderText="Tipo Identificación">
    <ItemTemplate>
        <%# Eval("TipoIdentificacionNombre") %> 
    </ItemTemplate>
</asp:TemplateField>

                <asp:BoundField DataField="Correo" HeaderText="Correo" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />

                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class='<%# Convert.ToBoolean(Eval("Estado")) ? "badge-active" : "badge-inactive" %>'>
                            <%# Convert.ToBoolean(Eval("Estado")) ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="UsuarioCreacion" HeaderText="Creado Por" />
                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creación" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:BoundField DataField="UsuarioModificacion" HeaderText="Modificado Por" />
                <asp:BoundField DataField="FechaModificacion" HeaderText="Fecha Modificación" DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="&#9998;" DeleteText="&#128465;" ControlStyle-CssClass="btn-action" />
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlFormCotizante" runat="server" Visible="false" CssClass="form-container" style="margin-top:20px; padding:15px; border:1px solid #ddd; background-color:#f9f9f9;">
            <h4 id="lblTituloFormularioCotizante" runat="server" style="color:#003366; margin-top:0;">Nuevo Cotizante</h4>
            <asp:HiddenField ID="hfIdCotizante" runat="server" />
            
            <div class="form-row">
                <div class="form-group" style="flex:1; margin-right:10px;">
                    <label>Tipo Identificación:</label>
                    <asp:DropDownList ID="ddlTipoIdentificacion" runat="server" CssClass="form-input" 
    DataTextField="nombre" 
    DataValueField="id_tipo_identificacion">
</asp:DropDownList>
                </div>
                <div class="form-group" style="flex:1;">
                    <label>Número Identificación:</label>
                    <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="form-input"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-row" style="display:flex; gap:10px;">
                <div class="form-group" style="flex:1;">
                    <label>Nombre:</label>
                    <asp:TextBox ID="txtNombreCotizante" runat="server" CssClass="form-input"></asp:TextBox>
                </div>
                <div class="form-group" style="flex:1;">
                    <label>Primer Apellido:</label>
                    <asp:TextBox ID="txtApellido1" runat="server" CssClass="form-input"></asp:TextBox>
                </div>
                <div class="form-group" style="flex:1;">
                    <label>Segundo Apellido:</label>
                    <asp:TextBox ID="txtApellido2" runat="server" CssClass="form-input"></asp:TextBox>
                </div>
            </div>

            <div class="form-row" style="display:flex; gap:10px;">
                <div class="form-group" style="flex:1;">
                    <label>Correo Electrónico:</label>
                    <asp:TextBox ID="txtCorreoCotizante" runat="server" CssClass="form-input" TextMode="Email"></asp:TextBox>
                </div>
                <div class="form-group" style="flex:1;">
                    <label>Teléfono:</label>
                    <asp:TextBox ID="txtTelefonoCotizante" runat="server" CssClass="form-input"></asp:TextBox>
                </div>
            </div>

            <div class="form-group" id="divEstadoCotizante" runat="server" visible="false">
                <label>Estado Activo:</label>
                <asp:CheckBox ID="chkEstadoCotizante" runat="server" Checked="true" />
            </div>

            <div style="margin-top: 15px;">
                <asp:Button ID="btnGuardarCotizante" runat="server" Text="Guardar" CssClass="btn-primary" OnClick="btnGuardarCotizante_Click" />
                <asp:Button ID="btnCancelarCotizante" runat="server" Text="Cancelar" CssClass="btn-secondary" OnClick="btnCancelarCotizante_Click" formnovalidate="true" />
            </div>
        </asp:Panel>
    </div>
</asp:Panel>

<!-- ================= NUEVA COTIZACIÓN ================= -->
<asp:Panel ID="pnlNuevaCotizacion" runat="server" Visible="false">
    <div class="card">
        <div class="card-header"><h3 class="card-title">Generador de Cotizaciones</h3></div>
        
        <div class="form-container" style="padding:15px;">
            <div class="form-group">
                <label>Cotizante:</label>
                <asp:DropDownList ID="ddlCotizanteCotizacion" runat="server" CssClass="form-input" DataTextField="NombreCompleto" DataValueField="IdCotizante"></asp:DropDownList>
            </div>

            <div class="form-group">
                <label>Tasa Activa (Define Producto y Plazo):</label>
                <asp:DropDownList ID="ddlTasaCotizacion" runat="server" CssClass="form-input" DataTextField="PlazoDias" DataValueField="IdTasa"></asp:DropDownList>
                <small style="color:gray;">Seleccione una tasa para aplicar sus condiciones (Producto, Plazo y Porcentaje).</small>
            </div>

            <div class="form-group">
                <label>Monto a Invertir:</label>
                <asp:TextBox ID="txtMontoCotizacion" runat="server" CssClass="form-input" TextMode="Number" step="0.01"></asp:TextBox>
            </div>

            <div style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblMensajeCotizacion" runat="server" style="display:block; padding: 10px; border-radius: 4px; font-weight: bold;" Visible="false"></asp:Label>
            </div>

            <div style="margin-top: 15px;">
                <asp:Button ID="btnGenerarCotizacion" runat="server" Text="Generar Cotizaci&oacute;n" CssClass="btn-primary" OnClick="btnGenerarCotizacion_Click" />
            </div>
        </div>
    </div>
</asp:Panel>

<!-- ================= LISTA COTIZACIONES ================= -->
<asp:Panel ID="pnlListaCotizaciones" runat="server" Visible="false">
    <div class="card">
        <div class="card-header">
            <h3 class="card-title">Mis Cotizaciones</h3>
            <div style="display:flex; gap:10px;">
                <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" CssClass="btn-secondary" OnClick="btnExportarExcel_Click" />
                <asp:Button ID="btnExportarPDF" runat="server" Text="Exportar a PDF" CssClass="btn-secondary" OnClientClick="imprimirCotizaciones(); return false;" />
            </div>
        </div>

        <!-- Mensaje de éxito/error (se auto-oculta con JS) -->
        <div style="margin-bottom:10px;">
            <asp:Label ID="lblMensajeListaCotizaciones" runat="server" Visible="false"
                style="display:block; padding:12px 16px; border-radius:6px; font-weight:bold;"></asp:Label>
        </div>
        <script type="text/javascript">
            function autoOcultarMensajeListaCotizaciones() {
                var lbl = document.getElementById('<%= lblMensajeListaCotizaciones.ClientID %>');
                if (lbl && lbl.innerText.trim() !== '') {
                    setTimeout(function () {
                        lbl.style.transition = 'opacity 0.8s';
                        lbl.style.opacity = '0';
                        setTimeout(function () { lbl.style.display = 'none'; }, 800);
                    }, 3000);
                }
            }
            window.addEventListener('load', autoOcultarMensajeListaCotizaciones);
        </script>

        <!-- Filtro por Cotizante -->

        <div style="display:flex; align-items:center; gap:10px; margin-bottom:15px; padding:10px; background:#f9f9f9; border-radius:6px; border:1px solid #eee;">
            <label style="font-weight:bold; white-space:nowrap;">Filtrar por Cotizante:</label>
            <asp:DropDownList ID="ddlFiltroCotizante" runat="server" CssClass="form-input" style="max-width:280px;"></asp:DropDownList>
            <asp:Button ID="btnFiltrarCotizaciones" runat="server" Text="Filtrar" CssClass="btn-primary" OnClick="btnFiltrarCotizaciones_Click" />
            <asp:Button ID="btnVerTodos" runat="server" Text="Ver Todos" CssClass="btn-secondary" OnClick="btnVerTodos_Click" />
        </div>

        <script type="text/javascript">
            function imprimirCotizaciones() {
                var printContent = document.getElementById('<%= gvCotizaciones.ClientID %>');
                if (!printContent) { alert('No hay datos para imprimir'); return; }
                var windowUrl = 'about:blank';
                var printWindow = window.open(windowUrl, 'PrintWindow', 'width=800,height=600');
                
                printWindow.document.write('<html><head><title>Reporte de Cotizaciones</title>');
                printWindow.document.write('<style>');
                printWindow.document.write('body { font-family: Arial, sans-serif; padding: 20px; }');
                printWindow.document.write('table { border-collapse: collapse; width: 100%; margin-top: 20px; }');
                printWindow.document.write('th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }');
                printWindow.document.write('th { background-color: #003366; color: white; }');
                printWindow.document.write('td:last-child, th:last-child { display: none; }'); // Ocultar columna "Acción"
                printWindow.document.write('</style>');
                printWindow.document.write('</head><body>');
                printWindow.document.write('<h2 style="color:#003366; border-bottom:2px solid #003366; padding-bottom:10px;">Reporte de Cotizaciones</h2>');
                printWindow.document.write(printContent.outerHTML);
                printWindow.document.write('</body></html>');
                printWindow.document.close();
                printWindow.focus();
                // Retraso ligero para asegurar que se renderice antes de imprimir
                setTimeout(function(){ printWindow.print(); printWindow.close(); }, 250);
            }
        </script>
        
        <asp:GridView ID="gvCotizaciones" runat="server" AutoGenerateColumns="False" CssClass="table-modern" DataKeyNames="IdCotizacion" OnRowCommand="gvCotizaciones_RowCommand">
            <Columns>
                <asp:BoundField DataField="NumeroCotizacion" HeaderText="Nro Cotizaci&oacute;n" />
                <asp:BoundField DataField="NombreCotizante" HeaderText="Cotizante" />
                <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="MontoTotalNeto" HeaderText="Total Neto" DataFormatString="{0:N2}" />

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnVerDetalles" runat="server" CommandName="VerDetalles" CommandArgument='<%# Eval("IdCotizacion") %>' CssClass="btn-secondary" Text="Ver Detalles"></asp:LinkButton>
                        <asp:LinkButton ID="btnEditarCotizacion" runat="server" CommandName="EditarCotizacion" CommandArgument='<%# Eval("IdCotizacion") %>' CssClass="btn-primary" Text="Editar" style="margin-left:5px;"></asp:LinkButton>
                        <asp:LinkButton ID="btnInactivar" runat="server" CommandName="InactivarCotizacion" CommandArgument='<%# Eval("IdCotizacion") %>' CssClass="btn-danger" Text="Inactivar" OnClientClick="return confirm('&iquest;Est&aacute; seguro de inactivar esta cotizaci&oacute;n y todos sus meses?');" style="margin-left: 5px;"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Panel de Detalles de Cotización -->
        <asp:Panel ID="pnlDetallesView" runat="server" Visible="false" style="margin-top:20px; border-top:2px solid #ddd; padding-top:15px;">
            <h4 id="lblTituloDetalles" runat="server" style="color:#003366;">Detalles Mensuales</h4>
            <asp:HiddenField ID="hfIdCotizacionActual" runat="server" />
            <br/>
            <asp:GridView ID="gvDetallesCotizacion" runat="server" AutoGenerateColumns="False" CssClass="table-modern" DataKeyNames="IdCotizacion,NumeroMes">
                <Columns>
                    <asp:BoundField DataField="NumeroMes" HeaderText="Mes" />
                    <asp:BoundField DataField="InteresBruto" HeaderText="Inter&eacute;s Bruto" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Impuesto" HeaderText="Impuesto" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="InteresNeto" HeaderText="Inter&eacute;s Neto" DataFormatString="{0:N2}" />
                </Columns>
            </asp:GridView>
            <div style="margin-top: 15px;">
                <asp:Button ID="btnCerrarDetalles" runat="server" Text="Cerrar Detalles" CssClass="btn-secondary" OnClick="btnCerrarDetalles_Click" />
            </div>
        </asp:Panel>

        <!-- Panel Editar Cotización -->
        <asp:Panel ID="pnlEditarCotizacion" runat="server" Visible="false"
            style="margin-top:20px; border-top:2px solid #1693A5; padding-top:15px;">
            <h4 style="color:#1693A5;">Editar Cotizaci&oacute;n</h4>
            <asp:HiddenField ID="hfIdCotizacionEditar" runat="server" />

            <div class="form-group">
                <label>Cotizante:</label>
                <asp:DropDownList ID="ddlCotizanteEdicion" runat="server" CssClass="form-input"></asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Tasa Activa (Define Producto y Plazo):</label>
                <asp:DropDownList ID="ddlTasaEdicion" runat="server" CssClass="form-input"></asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Monto a Invertir:</label>
                <asp:TextBox ID="txtMontoEdicion" runat="server" CssClass="form-input" TextMode="Number" step="0.01"></asp:TextBox>
            </div>

            <div style="margin-top:10px;">
                <asp:Label ID="lblMensajeEdicion" runat="server" Visible="false"
                    style="display:block; padding:10px; border-radius:4px; font-weight:bold;"></asp:Label>
            </div>

            <script type="text/javascript">
                function autoOcultarMensajeEdicion() {
                    var lbl = document.getElementById('<%= lblMensajeEdicion.ClientID %>');
                    if (lbl && lbl.innerText.trim() !== '' && lbl.style.backgroundColor === 'rgb(212, 237, 218)') {
                        setTimeout(function () {
                            lbl.style.transition = 'opacity 0.8s';
                            lbl.style.opacity = '0';
                            setTimeout(function () { lbl.style.display = 'none'; }, 800);
                        }, 3000);
                    }
                }
                window.addEventListener('load', autoOcultarMensajeEdicion);
            </script>

            <div style="margin-top:15px;">
                <asp:Button ID="btnActualizarCotizacion" runat="server" Text="Actualizar Cotizaci&oacute;n"
                    CssClass="btn-primary" OnClick="btnActualizarCotizacion_Click" />
                <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar"
                    CssClass="btn-secondary" OnClick="btnCancelarEdicion_Click"
                    style="margin-left:10px;" formnovalidate="true" />
            </div>
        </asp:Panel>
    </div>
</asp:Panel>

</div>


                </div>

            

        </asp:Panel>

    </form>
</body>
</html>