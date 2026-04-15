using Inverix.Classes;
using Inverix.Controllers;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inverix.Views
{
    public partial class InverixForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usuario"] != null)
                {
                    // PROTECCIÓN DE ROL: Si es COT no puede estar en esta página (Admin)
                    if (Session["RolCodigo"] != null && Session["RolCodigo"].ToString().Trim().ToUpper() == "COT")
                    {
                        Response.Redirect("CotizanteForm.aspx");
                        return;
                    }

                    MostrarSistema();
                    pnlInicio.Visible = true;
                    pnlProductos.Visible = false;
                    Session["PanelActivo"] = "Inicio";
                }
                else
                {
                    MostrarLogin();
                }
            }
            else
            {
                // En PostBack: restaurar visibilidad de paneles sin hacer DataBind todavía
                if (Session["Usuario"] != null)
                {
                    MostrarSistema();
                    string panelActivo = Session["PanelActivo"] != null ? Session["PanelActivo"].ToString() : "Inicio";
                    OcultarTodosLosPaneles();
                    switch (panelActivo)
                    {
                        case "Monedas":  pnlMonedas.Visible = true; break;
                        case "Productos": pnlProductos.Visible = true; break;
                        case "Plazos": pnlPlazos.Visible = true; break;
                        case "Tasas": pnlTasas.Visible = true; break;
                        case "Cotizantes": pnlCotizantes.Visible = true; break;
                        case "Usuarios": CargarUsuarios(); pnlUsuarios.Visible = true; break;
                        case "Roles": CargarRoles(); pnlRoles.Visible = true; break;
                        case "NuevaCotizacion": pnlNuevaCotizacion.Visible = true; break;
                        case "ListaCotizaciones": pnlListaCotizaciones.Visible = true; break;
                        case "ReportesAdmin": pnlReportesAdmin.Visible = true; break;
                        default: pnlInicio.Visible = true; break;
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Hacer DataBind DESPUÉS de tod
            // os los eventos (guardar, editar, eliminar)
            if (IsPostBack && Session["Usuario"] != null)
            {
                string panelActivo = Session["PanelActivo"] != null ? Session["PanelActivo"].ToString() : "Inicio";
                if (panelActivo == "Monedas") CargarMonedas();
                if (panelActivo == "Productos") CargarProductos();
                if (panelActivo == "Plazos") CargarPlazos();
                if (panelActivo == "Tasas") CargarTasas();
                if (panelActivo == "Cotizantes") CargarCotizantes();
                if (panelActivo == "ListaCotizaciones")
                {
                    string filtroCotizante = Session["FiltroCotizante"] != null ? Session["FiltroCotizante"].ToString() : null;
                    string filtroNumero = Session["FiltroNumeroCotizacion"] != null ? Session["FiltroNumeroCotizacion"].ToString() : null;
                    CargarCotizaciones(filtroCotizante, filtroNumero);
                }
                if (panelActivo == "ReportesAdmin")
                {
                    CargarReporteAdmin(false);
                }


            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar estilos previos (útil si después ponemos diseño a los errores)
                lblMensaje.Text = string.Empty;

                string usuario = txtUsuario.Text.Trim();
                string clave = txtClave.Text.Trim();

                // 1. Validaciones de negocio: Campos Vacíos
                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
                {
                    lblMensaje.Text = "Por favor ingrese su usuario y contraseña.";
                    return;
                }

                // 2. Validación de Seguridad: Longitud Mínima
                if (usuario.Length < 4)
                {
                    lblMensaje.Text = "El usuario debe tener al menos 4 caracteres.";
                    return;
                }
                
                if (clave.Length < 6)
                {
                    lblMensaje.Text = "La contraseña ingresada no cumple con la longitud mínima de seguridad de 6 caracteres .";
                    return;
                }

                // 3. Validación de Seguridad: Inyección / Caracteres no permitidos
                // (Aunque usamos SP que evitan Inyección SQL, frenar caracteres especiales desde código añade una capa extra).
                if (usuario.Contains(" ") || usuario.Contains("'") || usuario.Contains("\"") || usuario.Contains(";"))
                {
                    lblMensaje.Text = "El nombre de usuario contiene caracteres de sistema no permitidos.";
                    return;
                }

                // Instanciar controlador
                Inverix.Controllers.UsuarioController userController = new Inverix.Controllers.UsuarioController();
                
                // Validar contra Base de Datos (encriptación SHA256 gestionada por el SP de SQL)
                Inverix.Classes.UsuarioClass usuarioLogueado = userController.ValidarLogin(usuario, clave);

                if (usuarioLogueado != null)
                {
                    // Limpieza de campos sensibles por seguridad al iniciar
                    txtUsuario.Text = string.Empty;
                    txtClave.Text = string.Empty;

                    Session["Usuario"] = usuarioLogueado.NombreUsuario;
                    Session["Rol"] = usuarioLogueado.Rol;
                    Session["RolCodigo"] = usuarioLogueado.RolCodigo;
                    Session["IdUsuario"] = usuarioLogueado.IdUsuario;
                    Session["IdCotizante"] = usuarioLogueado.IdCotizante;

                    // Redirigir según rol (Usando Trim() y ToUpper() por seguridad)
                    if (usuarioLogueado.RolCodigo != null && usuarioLogueado.RolCodigo.Trim().ToUpper() == "COT")
                    {
                        Response.Redirect("CotizanteForm.aspx");
                        return;
                    }

                    MostrarSistema();

                    // Mostrar Inicio después de login
                    pnlInicio.Visible = true;
                    pnlProductos.Visible = false;
                }
                else
                {
                    MostrarMensaje("Credenciales incorrectas. Verifique e intente nuevamente.", true);
                }
            }
            catch (Exception)
            {
                // Nunca mostrar errores reales de DB al usuario por temas de seguridad. 
                // Se envía un mensaje genérico.
                MostrarMensaje("Inconvenientes al conectar con el servidor. Intente más tarde.", true);
                // Si hubiera una herramienta de logs: Logs. Escribir(ex.Message);
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            MostrarLogin();
        }

        private void MostrarSistema()
        {
            pnlLogin.Visible = false;
            pnlSistema.Visible = true;

            lblMensaje.Text = string.Empty; // Limpia errores de login al entrar al sistema
            if (lblMensajeSistema != null) lblMensajeSistema.Visible = false;

            if (Session["Usuario"] != null)
            {
                lblUsuarioLogueado.Text = "Usuario: " + Session["Usuario"].ToString();
            }
        }

        private void MostrarLogin()
        {
            pnlLogin.Visible = true;
            pnlSistema.Visible = false;

            txtClave.Text = "";
            lblMensaje.Text = string.Empty; // Limpia mensajes al volver al login
            if (lblMensajeSistema != null) lblMensajeSistema.Visible = false;
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            if (lblMensajeSistema != null)
            {
                lblMensajeSistema.Text = mensaje;
                lblMensajeSistema.Visible = !string.IsNullOrEmpty(mensaje);
            }
        }

        // ================= NAVEGACIÓN =================

        private void OcultarTodosLosPaneles()
        {
            lblMensaje.Text = string.Empty; // Limpia mensajes globales
            if (lblMensajeSistema != null) lblMensajeSistema.Visible = false;
            lblMensajeMoneda.Visible = false;
            lblMensajeProducto.Visible = false;
            lblMensajePlazo.Visible = false;
            lblMensajeCotizante.Visible = false;
            lblMensajeTasas.Visible = false;
            lblMensajeListaCotizaciones.Visible = false;
            pnlInicio.Visible = false;
            pnlProductos.Visible = false;
            pnlUsuarios.Visible = false;
            pnlRoles.Visible = false;
            pnlMonedas.Visible = false;
            pnlTasas.Visible = false;
            pnlPlazos.Visible = false;
            pnlCotizantes.Visible = false;
            pnlNuevaCotizacion.Visible = false;
            pnlListaCotizaciones.Visible = false;
            if (pnlReportesAdmin != null) pnlReportesAdmin.Visible = false;
        }

        protected void lnkInicio_Click(object sender, EventArgs e)
        {
            OcultarTodosLosPaneles();
            pnlInicio.Visible = true;
        }

        protected void lnkUsuarios_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Usuarios";
            OcultarTodosLosPaneles();
            CargarUsuarios();
            pnlUsuarios.Visible = true;
        }

        protected void lnkRoles_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Roles";
            OcultarTodosLosPaneles();
            CargarRoles();
            pnlRoles.Visible = true;
        }

        protected void lnkMonedas_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Monedas";
            OcultarTodosLosPaneles();
            pnlMonedas.Visible = true;
            CargarMonedas();
        }

        protected void lnkProductos_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Productos";
            OcultarTodosLosPaneles();
            pnlProductos.Visible = true;
            CargarProductos();
        }

        protected void lnkTasas_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Tasas";
            OcultarTodosLosPaneles();
            pnlTasas.Visible = true;
            CargarTasas();

        }

        protected void lnkPlazos_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Plazos";
            OcultarTodosLosPaneles();
            pnlPlazos.Visible = true;
            CargarPlazos();

        }

        protected void lnkCotizantes_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "Cotizantes";
            OcultarTodosLosPaneles();
            pnlCotizantes.Visible = true;
            CargarCotizantes();
            CargarTiposIdEnLista();

        }

        protected void lnkNuevaCotizacion_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "NuevaCotizacion";
            OcultarTodosLosPaneles();
            pnlNuevaCotizacion.Visible = true;
            CargarCotizantesEnLista();
            CargarTasasEnLista();
            txtMontoCotizacion.Text = string.Empty;
            lblMensajeCotizacion.Visible = false;
        }

        protected void lnkListaCotizaciones_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "ListaCotizaciones";
            OcultarTodosLosPaneles();
            pnlListaCotizaciones.Visible = true;
            pnlDetallesView.Visible = false;
            CargarFiltroCotizantes();
            CargarCotizaciones();
        }

        protected void lnkReportesAdmin_Click(object sender, EventArgs e)
        {
            Session["PanelActivo"] = "ReportesAdmin";
            OcultarTodosLosPaneles();
            pnlReportesAdmin.Visible = true;
            CargarCotizantesReporte();
            CargarReporteAdmin(true);
        }

        // ================= PRODUCTOS =================

        private void CargarProductos()
        {
            try
            {
                Inverix.Controllers.ProductoController prodController = new Inverix.Controllers.ProductoController(); 
                var listaProductos = prodController.ObtenerTodos();

                gvProductos.DataSource = listaProductos;
                gvProductos.DataBind();
                
                if (listaProductos.Count == 0)
                {
                    MostrarMensaje("No se encontraron productos registrados.", false);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar productos: " + ex.Message, true);
            }
        }

        protected void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            LimpiarFormularioProducto();
            CargarMonedasEnLista();
            lblTituloFormularioProducto.InnerText = "Nuevo Producto";
            btnGuardarProducto.Text = "Crear";
            pnlFormProducto.Visible = true;
            lblMensajeProducto.Visible = false;
        }

        protected void gvProductos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true; // Evitar el modo de edición nativo del grid
            lblMensajeProducto.Visible = false;

            try
            {
                int idProducto = Convert.ToInt32(gvProductos.DataKeys[e.NewEditIndex].Value);

                // Obtener datos (se podría mejorar con un GetById, pero para mantenerlo simple filtramos la lista)
                Inverix.Controllers.ProductoController prodController = new Inverix.Controllers.ProductoController();
                var producto = prodController.ObtenerTodos().Find(p => p.IdProducto == idProducto);

                if (producto != null)
                {
                    CargarMonedasEnLista();
                    hfIdProducto.Value = producto.IdProducto.ToString();
                    txtCodigoProducto.Text = producto.Codigo;
                    txtNombreProducto.Text = producto.Nombre;
                    ddlMonedaProducto.SelectedValue = producto.IdMoneda.ToString();
                    chkEstadoProducto.Checked = producto.Estado;


                    lblTituloFormularioProducto.InnerText = "Editar Producto";
                    btnGuardarProducto.Text = "Actualizar";
                    divEstadoProducto.Visible = true;

                    pnlFormProducto.Visible = true;
                    // Hacer scroll al formulario automáticamente
                    ScriptManager.RegisterStartupScript(this, GetType(), "scrollProducto",
                        "setTimeout(function(){ document.getElementById('seccionFormularioProducto').scrollIntoView({behavior:'smooth',block:'start'}); }, 100);", true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al intentar editar: " + ex.Message, true);
            }
        }

        protected void gvProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int idProducto = Convert.ToInt32(gvProductos.DataKeys[e.RowIndex].Value);
                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                Inverix.Controllers.ProductoController prodController = new Inverix.Controllers.ProductoController();
                bool exito = prodController.Inactivar(idProducto, usuarioLogueado);

                if (exito)
                {
                    lblMensajeProducto.Text = "Producto inactivado exitosamente.";
                    lblMensajeProducto.Style["background-color"] = "#d4edda";
                    lblMensajeProducto.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeProducto.Style["color"] = "#155724";
                    lblMensajeProducto.Visible = true;
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                lblMensajeProducto.Text = "Error al inactivar el producto: " + ex.Message;
                lblMensajeProducto.Style["background-color"] = "#f8d7da";
                lblMensajeProducto.Style["color"] = "#721c24";
                lblMensajeProducto.Visible = true;
            }
        }

        protected void btnGuardarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeProducto.Visible = false;

                // Validación: Código
                if (string.IsNullOrEmpty(txtCodigoProducto.Text.Trim()))
                {
                    lblMensajeProducto.Text = "El código del producto es obligatorio.";
                    lblMensajeProducto.Style["background-color"] = "#f8d7da";
                    lblMensajeProducto.Style["color"] = "#721c24";
                    lblMensajeProducto.Visible = true;
                    return;
                }

                // Validación: Nombre
                if (string.IsNullOrEmpty(txtNombreProducto.Text.Trim()))
                {
                    lblMensajeProducto.Text = "El nombre del producto es obligatorio.";
                    lblMensajeProducto.Style["background-color"] = "#f8d7da";
                    lblMensajeProducto.Style["color"] = "#721c24";
                    lblMensajeProducto.Visible = true;
                    return;
                }

                Inverix.Classes.ProductoClass producto = new Inverix.Classes.ProductoClass
                {
                    Codigo = txtCodigoProducto.Text.Trim(),
                    Nombre = txtNombreProducto.Text.Trim(),
                    IdMoneda = Convert.ToInt32(ddlMonedaProducto.SelectedValue),
                    Estado = chkEstadoProducto.Checked
                };

                // Si el nombre viene vacío, usamos el nombre de la moneda seleccionada (Producto = Moneda)
                if (string.IsNullOrEmpty(producto.Nombre))
                {
                    producto.Nombre = ddlMonedaProducto.SelectedItem.Text;
                }

                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.ProductoController prodController = new Inverix.Controllers.ProductoController();
                bool exito = false;
                string mensajeExito = string.Empty;

                if (string.IsNullOrEmpty(hfIdProducto.Value)) // Nuevo
                {
                    exito = prodController.Crear(producto, usuarioLogueado);
                    mensajeExito = "Producto creado exitosamente.";
                }
                else // Edición
                {
                    producto.IdProducto = Convert.ToInt32(hfIdProducto.Value);
                    exito = prodController.Actualizar(producto, usuarioLogueado);
                    mensajeExito = "Producto actualizado exitosamente.";
                }

                if (exito)
                {
                    pnlFormProducto.Visible = false;
                    LimpiarFormularioProducto();
                    CargarProductos();

                    lblMensajeProducto.Text = mensajeExito;
                    lblMensajeProducto.Style["background-color"] = "#d4edda";
                    lblMensajeProducto.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeProducto.Style["color"] = "#155724";
                    lblMensajeProducto.Visible = true;
                }
                else
                {
                    lblMensajeProducto.Text = "No se pudo guardar el producto. Inténtelo de nuevo.";
                    lblMensajeProducto.Style["background-color"] = "#f8d7da";
                    lblMensajeProducto.Style["color"] = "#721c24";
                    lblMensajeProducto.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeProducto.Text = ObtenerMensajeAmigable(ex, "producto");
                lblMensajeProducto.Style["background-color"] = "#f8d7da";
                lblMensajeProducto.Style["color"] = "#721c24";
                lblMensajeProducto.Visible = true;
            }
        }

        protected void btnCancelarProducto_Click(object sender, EventArgs e)
        {
            pnlFormProducto.Visible = false;
            LimpiarFormularioProducto();
            lblMensajeProducto.Visible = false;
        }

        private void LimpiarFormularioProducto()
        {
            hfIdProducto.Value = string.Empty;
            txtCodigoProducto.Text = string.Empty;
            txtNombreProducto.Text = string.Empty;
            if (ddlMonedaProducto.Items.Count > 0)
            {
                ddlMonedaProducto.SelectedIndex = 0;
            }
        }





       

        private void CargarMonedasEnLista()
        {
            try
            {
                Inverix.Controllers.MonedaController monController = new Inverix.Controllers.MonedaController();
                var listaMonedas = monController.ObtenerTodas().FindAll(m => m.Estado); // Solo las activas
                
                ddlMonedaProducto.DataSource = listaMonedas;
                ddlMonedaProducto.DataBind();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar monedas: " + ex.Message);
            }
        }
        private void CargarMonedas()
        {
            try
            {
                Inverix.Controllers.MonedaController monController = new Inverix.Controllers.MonedaController();
                gvMonedas.DataSource = monController.ObtenerTodas();
                gvMonedas.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar monedas: " + ex.Message, true);
            }
        }

        protected void btnNuevaMoneda_Click(object sender, EventArgs e)
        {
            LimpiarFormularioMoneda();
            lblTituloFormularioMoneda.InnerText = "Nueva Moneda";
            btnGuardarMoneda.Text = "Crear";
            divEstadoMoneda.Visible = false; // Solo visibilidad de estado al editar
            chkEstadoMoneda.Checked = true;
            pnlFormMoneda.Visible = true;
            lblMensajePlazo.Visible = false;
        }

        protected void gvMonedas_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;
            lblMensajePlazo.Visible = false;

            try
            {
                int idMoneda = Convert.ToInt32(gvMonedas.DataKeys[e.NewEditIndex].Value);
                Inverix.Controllers.MonedaController monController = new Inverix.Controllers.MonedaController();
                var moneda = monController.ObtenerTodas().Find(m => m.IdMoneda == idMoneda);

                if (moneda != null)
                {
                    hfIdMoneda.Value = moneda.IdMoneda.ToString();
                    txtCodigoMoneda.Text = moneda.Codigo;
                    txtNombreMoneda.Text = moneda.Nombre;
                    txtSimboloMoneda.Text = moneda.Simbolo;
                    chkEstadoMoneda.Checked = moneda.Estado;

                    lblTituloFormularioMoneda.InnerText = "Editar Moneda";
                    btnGuardarMoneda.Text = "Actualizar";
                    divEstadoMoneda.Visible = true; // Mostrar estado al editar
                    pnlFormMoneda.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al intentar editar: " + ex.Message, true);
            }
        }

        protected void gvMonedas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int idMoneda = Convert.ToInt32(gvMonedas.DataKeys[e.RowIndex].Value);
                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                Inverix.Controllers.MonedaController monController = new Inverix.Controllers.MonedaController();
                bool exito = monController.Inactivar(idMoneda, usuarioLogueado);

                if (exito)
                {
                    MostrarMensaje("Moneda inactivada exitosamente.", false);
                    CargarMonedas();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al inactivar la moneda: " + ex.Message, true);
            }
        }

        protected void btnGuardarMoneda_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeMoneda.Visible = false;

                if (string.IsNullOrEmpty(txtCodigoMoneda.Text.Trim()) || string.IsNullOrEmpty(txtNombreMoneda.Text.Trim()) || string.IsNullOrEmpty(txtSimboloMoneda.Text.Trim()))
                {
                    lblMensajeMoneda.Text = "El código, nombre y símbolo son obligatorios.";
                    lblMensajeMoneda.Style["background-color"] = "#f8d7da";
                    lblMensajeMoneda.Style["color"] = "#721c24";
                    lblMensajeMoneda.Visible = true;
                    return;
                }

                Inverix.Classes.MonedaClass moneda = new Inverix.Classes.MonedaClass
                {
                    Codigo = txtCodigoMoneda.Text.Trim(),
                    Nombre = txtNombreMoneda.Text.Trim(),
                    Simbolo = txtSimboloMoneda.Text.Trim(),
                    Estado = chkEstadoMoneda.Checked
                };

                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.MonedaController monController = new Inverix.Controllers.MonedaController();
                bool exito = false;
                string mensajeExito = string.Empty;

                if (string.IsNullOrEmpty(hfIdMoneda.Value))
                {
                    moneda.Estado = true;
                    exito = monController.Crear(moneda, usuarioLogueado);
                    mensajeExito = "Moneda creada exitosamente.";
                }
                else
                {
                    moneda.IdMoneda = Convert.ToInt32(hfIdMoneda.Value);
                    exito = monController.Actualizar(moneda, usuarioLogueado);
                    mensajeExito = "Moneda actualizada exitosamente.";
                }

                if (exito)
                {
                    pnlFormMoneda.Visible = false; // Cerrar el formulario al guardar
                    LimpiarFormularioMoneda();
                    CargarMonedas();

                    // Mensaje verde en la parte superior de la tarjeta (JS lo oculta en 3 s)
                    lblMensajeMoneda.Text = mensajeExito;
                    lblMensajeMoneda.Style["background-color"] = "#d4edda";
                    lblMensajeMoneda.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeMoneda.Style["color"] = "#155724";
                    lblMensajeMoneda.Visible = true;
                }
                else
                {
                    lblMensajeMoneda.Text = "No se pudo guardar la moneda. Inténtelo de nuevo.";
                    lblMensajeMoneda.Style["background-color"] = "#f8d7da";
                    lblMensajeMoneda.Style["color"] = "#721c24";
                    lblMensajeMoneda.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeMoneda.Text = ObtenerMensajeAmigable(ex, "moneda");
                lblMensajeMoneda.Style["background-color"] = "#f8d7da";
                lblMensajeMoneda.Style["color"] = "#721c24";
                lblMensajeMoneda.Visible = true;
            }
        }

        protected void btnCancelarMoneda_Click(object sender, EventArgs e)
        {
            pnlFormMoneda.Visible = false;
            LimpiarFormularioMoneda();
            lblMensajePlazo.Visible = false;
        }

        private void LimpiarFormularioMoneda()
        {
            hfIdMoneda.Value = string.Empty;
            txtCodigoMoneda.Text = string.Empty;
            txtNombreMoneda.Text = string.Empty;
            txtSimboloMoneda.Text = string.Empty;
        }






        
        private void CargarPlazosEnLista()
        {
            try
            {
                Inverix.Controllers.PlazoController plaController = new Inverix.Controllers.PlazoController();
                // Filtramos solo los activos para el mantenimiento si es necesario, 
                // aunque normalmente el GridView muestra todos.
                var listaPlazos = plaController.ObtenerTodas();

                // Para un GridView, solo se usan estas dos líneas:
                gvPlazos.DataSource = listaPlazos;
                gvPlazos.DataBind();
            }
            catch (Exception ex)
            {
                // Si usas el método MostrarMensaje que vimos antes:
                MostrarMensaje("Error al listar plazos: " + ex.Message, true);
            }
        }

        private void CargarPlazos()
        {
            try
            {
                Inverix.Controllers.PlazoController plaController = new Inverix.Controllers.PlazoController();
                gvPlazos.DataSource = plaController.ObtenerTodas();
                gvPlazos.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar plazos: " + ex.Message, true);
            }
        }

        protected void btnNuevoPlazo_Click(object sender, EventArgs e)
        {
            LimpiarFormularioPlazo();
            lblTituloFormularioPlazo.InnerText = "Nuevo Plazo";
            btnGuardarPlazo.Text = "Crear";
            divEstadoPlazo.Visible = false;
            chkEstadoPlazo.Checked = true;
            pnlFormPlazo.Visible = true;
            lblMensajePlazo.Visible = false;

            // REDIRECCIÓN VISUAL: Salta a la sección del formulario
            ClientScript.RegisterStartupScript(this.GetType(), "hash", "location.hash = '#seccionFormulario';", true);
        }

        protected void gvPlazos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;
            lblMensajePlazo.Visible = false;

            try
            {
                int idPlazo = Convert.ToInt32(gvPlazos.DataKeys[e.NewEditIndex].Value);
                Inverix.Controllers.PlazoController plaController = new Inverix.Controllers.PlazoController();
                var plazo = plaController.ObtenerTodas().Find(p => p.IdPlazo == idPlazo);

                if (plazo != null)
                {
                    hfIdPlazo.Value = plazo.IdPlazo.ToString();
                    txtDiasPlazo.Text = plazo.Dias.ToString();
                    txtDescripcionPlazo.Text = plazo.Descripcion;
                    chkEstadoPlazo.Checked = plazo.Estado;

                    lblTituloFormularioPlazo.InnerText = "Editar Plazo";
                    btnGuardarPlazo.Text = "Actualizar";
                    divEstadoPlazo.Visible = true;
                    pnlFormPlazo.Visible = true;

                    // Esto le dice al navegador que se mueva al anclaje una vez termine de cargar los datos
                    ClientScript.RegisterStartupScript(this.GetType(), "hash", "location.hash = '#seccionFormulario';", true);

                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al intentar editar: " + ex.Message, true);
            }
        }

        protected void gvPlazos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int idPlazo = Convert.ToInt32(gvPlazos.DataKeys[e.RowIndex].Value);
                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                Inverix.Controllers.PlazoController plaController = new Inverix.Controllers.PlazoController();
                bool exito = plaController.Inactivar(idPlazo, usuarioLogueado);

                if (exito)
                {
                    lblMensajePlazo.Text = "Plazo inactivado exitosamente.";
                    lblMensajePlazo.Style["background-color"] = "#d4edda";
                    lblMensajePlazo.Style["border"] = "1px solid #c3e6cb";
                    lblMensajePlazo.Style["color"] = "#155724";
                    lblMensajePlazo.Visible = true;
                    CargarPlazos();
                }
            }
            catch (Exception ex)
            {
                lblMensajePlazo.Text = "Error al inactivar el plazo: " + ex.Message;
                lblMensajePlazo.Style["background-color"] = "#f8d7da";
                lblMensajePlazo.Style["color"] = "#721c24";
                lblMensajePlazo.Visible = true;
            }
        }

        protected void btnGuardarPlazo_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajePlazo.Visible = false;

                // Validar meses
                if (string.IsNullOrEmpty(txtDescripcionPlazo.Text.Trim()))
                {
                    lblMensajePlazo.Text = "La cantidad de meses es obligatoria.";
                    lblMensajePlazo.Style["background-color"] = "#f8d7da";
                    lblMensajePlazo.Style["color"] = "#721c24";
                    lblMensajePlazo.Visible = true;
                    return;
                }

                // Convertir meses a días directamente aquí
                int meses;
                if (!int.TryParse(txtDescripcionPlazo.Text.Trim(), out meses) || meses <= 0)
                {
                    lblMensajePlazo.Text = "Cantidad de meses inválida.";
                    lblMensajePlazo.Style["background-color"] = "#f8d7da";
                    lblMensajePlazo.Style["color"] = "#721c24";
                    lblMensajePlazo.Visible = true;
                    return;
                }

                int dias = meses * 30; // 1 mes = 30 días aproximado

                // Mostrar los días en el TextBox gris
                txtDiasPlazo.Text = dias.ToString();

                // Crear objeto Plazo
                Inverix.Classes.PlazoClass plazo = new Inverix.Classes.PlazoClass
                {
                    Dias = dias,
                    Descripcion = txtDescripcionPlazo.Text.Trim(),
                    Estado = chkEstadoPlazo.Checked
                };

                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.PlazoController plaController = new Inverix.Controllers.PlazoController();
                bool exito = false;
                string mensajeExito = string.Empty;

                if (string.IsNullOrEmpty(hfIdPlazo.Value)) // Nuevo
                {
                    plazo.Estado = true;
                    exito = plaController.Crear(plazo, usuarioLogueado);
                    mensajeExito = "Plazo creado exitosamente.";
                }
                else // Editar
                {
                    plazo.IdPlazo = Convert.ToInt32(hfIdPlazo.Value);
                    exito = plaController.Actualizar(plazo, usuarioLogueado);
                    mensajeExito = "Plazo actualizado exitosamente.";
                }

                if (exito)
                {
                    pnlFormPlazo.Visible = false;
                    LimpiarFormularioPlazo();
                    CargarPlazos();

                    lblMensajePlazo.Text = mensajeExito;
                    lblMensajePlazo.Style["background-color"] = "#d4edda";
                    lblMensajePlazo.Style["border"] = "1px solid #c3e6cb";
                    lblMensajePlazo.Style["color"] = "#155724";
                    lblMensajePlazo.Visible = true;
                }
                else
                {
                    lblMensajePlazo.Text = "No se pudo guardar el plazo. Inténtelo de nuevo.";
                    lblMensajePlazo.Style["background-color"] = "#f8d7da";
                    lblMensajePlazo.Style["color"] = "#721c24";
                    lblMensajePlazo.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajePlazo.Text = ObtenerMensajeAmigable(ex, "plazo");
                lblMensajePlazo.Style["background-color"] = "#f8d7da";
                lblMensajePlazo.Style["color"] = "#721c24";
                lblMensajePlazo.Visible = true;
            }
        }


        protected void btnCancelarPlazo_Click(object sender, EventArgs e)
        {
            pnlFormPlazo.Visible = false;
            LimpiarFormularioPlazo();
            lblMensajePlazo.Visible = false;
        }

        private void LimpiarFormularioPlazo()
        {
            hfIdPlazo.Value = string.Empty;
            txtDiasPlazo.Text = string.Empty;
            txtDescripcionPlazo.Text = string.Empty;
        }



        protected void txtDescripcionPlazo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Leer la cantidad de meses desde el TextBox
                int meses;
                if (int.TryParse(txtDescripcionPlazo.Text.Trim(), out meses))
                {
                    int dias = meses * 30; // Aproximación: 1 mes = 30 días
                    txtDiasPlazo.Text = dias.ToString();
                }
                else
                {
                    // Si el texto no es un número válido, dejamos vacío
                    txtDiasPlazo.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al calcular los días: " + ex.Message, true);
            }
        }



      




        // ================= TASAS =================

        private void CargarTasas()
        {
            try
            {
                Inverix.Controllers.TasaController tasaController = new Inverix.Controllers.TasaController();
                var listaTasas = tasaController.ObtenerTodas();

                gvTasas.DataSource = listaTasas;
                gvTasas.DataBind();

                if (listaTasas.Count == 0)
                {
                    MostrarMensaje("No hay tasas configuradas actualmente.", false);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar la lista de tasas: " + ex.Message, true);
            }
        }

        private void CargarCombosTasa()
        {
            try
            {
                // Cargar Productos
                Inverix.Controllers.ProductoController prodController = new Inverix.Controllers.ProductoController();
                ddlProductoTasa.DataSource = prodController.ObtenerTodos().FindAll(p => p.Estado);
                ddlProductoTasa.DataBind();
                ddlProductoTasa.Items.Insert(0, new ListItem("-- Seleccione Producto --", ""));

                // Cargar Plazos (Usando el método que ya tienes)
                Inverix.Controllers.PlazoController plazoController = new Inverix.Controllers.PlazoController();
                ddlPlazoTasa.DataSource = plazoController.ObtenerTodas().FindAll(p => p.Estado);
                ddlPlazoTasa.DataBind();
                ddlPlazoTasa.Items.Insert(0, new ListItem("-- Seleccione Plazo --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar selectores: " + ex.Message, true);
            }
        }

        protected void btnNuevaTasa_Click(object sender, EventArgs e)
        {
            LimpiarFormularioTasa();
            CargarCombosTasa();
            lblTituloFormularioTasa.InnerText = "Nueva Tasa de Interés";
            btnGuardarTasa.Text = "Crear Tasa";
            divEstadoTasa.Visible = false;
            pnlFormTasa.Visible = true;
            lblMensajePlazo.Visible = false;

            // Salto visual al formulario
            ClientScript.RegisterStartupScript(this.GetType(), "hash", "location.hash = '#seccionFormularioTasa';", true);
        }

        protected void btnGuardarTasa_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones de Selección
                if (string.IsNullOrEmpty(ddlProductoTasa.SelectedValue) || string.IsNullOrEmpty(ddlPlazoTasa.SelectedValue))
                {
                    MostrarMensaje("Debe seleccionar un Producto y un Plazo obligatoriamente.", true);
                    return;
                }

                // 2. Validaciones de Porcentaje
                decimal porcentaje;
                string valor = txtPorcentajeTasa.Text.Trim().Replace(",", ".");

                if (!decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out porcentaje))
                {
                    MostrarMensaje("El porcentaje debe ser un número válido (ej: 3.15).", true);
                    return;
                }

                if (porcentaje < 0 || porcentaje > 100)
                {
                    MostrarMensaje("La tasa debe estar entre 0 y 100 (ej: 3.15).", true);
                    return;
                }

                Inverix.Classes.TasaClass tasa = new Inverix.Classes.TasaClass
                {
                    IdProducto = Convert.ToInt32(ddlProductoTasa.SelectedValue),
                    IdPlazo = Convert.ToInt32(ddlPlazoTasa.SelectedValue),
                    Porcentaje = porcentaje,
                    Estado = chkEstadoTasa.Checked
                };

                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.TasaController tasaController = new Inverix.Controllers.TasaController();
                bool exito = false;
                string mensajeExito = string.Empty;

                if (string.IsNullOrEmpty(hfIdTasa.Value)) // Nuevo
                {
                    tasa.Estado = true;
                    exito = tasaController.Crear(tasa, usuarioLogueado);
                    mensajeExito = "Tasa configurada correctamente.";
                }
                else // Edición
                {
                    tasa.IdTasa = Convert.ToInt32(hfIdTasa.Value);
                    exito = tasaController.Actualizar(tasa, usuarioLogueado);
                    mensajeExito = "Tasa actualizada exitosamente.";
                }

                if (exito)
                {
                    pnlFormTasa.Visible = false;
                    LimpiarFormularioTasa();
                    CargarTasas();

                    lblMensajeTasas.Text = mensajeExito;
                    lblMensajeTasas.Style["background-color"] = "#d4edda";
                    lblMensajeTasas.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeTasas.Style["color"] = "#155724";
                    lblMensajeTasas.Visible = true;
                }
                else
                {
                    lblMensajeTasas.Text = "No se pudo realizar la operación. Verifique los datos.";
                    lblMensajeTasas.Style["background-color"] = "#f8d7da";
                    lblMensajeTasas.Style["color"] = "#721c24";
                    lblMensajeTasas.Visible = true;
                }
            }
            catch (Exception ex)
            {
                // Manejo de error de duplicados (PK/Unique en la base de datos)
                string errorMsg = "Error al procesar la tasa: " + ex.Message;
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate"))
                    errorMsg = "Error: Ya existe una tasa configurada para esa combinación de Producto y Plazo.";

                lblMensajeTasas.Text = errorMsg;
                lblMensajeTasas.Style["background-color"] = "#f8d7da";
                lblMensajeTasas.Style["color"] = "#721c24";
                lblMensajeTasas.Visible = true;
            }
        }

        protected void gvTasas_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;
            lblMensajeTasas.Visible = false;
            try
            {
                int idTasa = Convert.ToInt32(gvTasas.DataKeys[e.NewEditIndex].Value);
                Inverix.Controllers.TasaController tasaController = new Inverix.Controllers.TasaController();
                var tasa = tasaController.ObtenerTodas().Find(t => t.IdTasa == idTasa);

                if (tasa != null)
                {
                    CargarCombosTasa();
                    hfIdTasa.Value = tasa.IdTasa.ToString();
                    ddlProductoTasa.SelectedValue = tasa.IdProducto.ToString();
                    ddlPlazoTasa.SelectedValue = tasa.IdPlazo.ToString();
                    txtPorcentajeTasa.Text = tasa.Porcentaje.ToString("F4"); // 4 decimales
                    chkEstadoTasa.Checked = tasa.Estado;

                    ddlProductoTasa.Enabled = false;
                    ddlPlazoTasa.Enabled = false;

                    lblTituloFormularioTasa.InnerText = "Editar Configuración de Tasa";
                    btnGuardarTasa.Text = "Actualizar Tasa";
                    divEstadoTasa.Visible = true;
                    pnlFormTasa.Visible = true;

                    ClientScript.RegisterStartupScript(this.GetType(), "hash", "location.hash = '#seccionFormularioTasa';", true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar datos para edición: " + ex.Message, true);
            }
        }

        private void LimpiarFormularioTasa()
        {
            hfIdTasa.Value = string.Empty;
            txtPorcentajeTasa.Text = string.Empty;
            
            // Re-habilitar selectores (por si veníamos de una edición)
            ddlProductoTasa.Enabled = true;
            ddlPlazoTasa.Enabled = true;

            if (ddlProductoTasa.Items.Count > 0) ddlProductoTasa.SelectedIndex = 0;
            if (ddlPlazoTasa.Items.Count > 0) ddlPlazoTasa.SelectedIndex = 0;
            chkEstadoTasa.Checked = true;
        }

        protected void btnCancelarTasa_Click(object sender, EventArgs e)
        {
            pnlFormTasa.Visible = false;
            lblMensajeTasas.Visible = false;
        }



        protected void gvTasas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // 1. Obtener el ID de la tasa desde las DataKeys del GridView
                int idTasa = Convert.ToInt32(gvTasas.DataKeys[e.RowIndex].Value);
                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                // 2. Llamar al controlador para la eliminación lógica (Inactivar)
                Inverix.Controllers.TasaController tasaController = new Inverix.Controllers.TasaController();
                bool exito = tasaController.Inactivar(idTasa, usuarioLogueado);

                if (exito)
                {
                    lblMensajeTasas.Text = "La tasa ha sido inactivada exitosamente.";
                    lblMensajeTasas.Style["background-color"] = "#d4edda";
                    lblMensajeTasas.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeTasas.Style["color"] = "#155724";
                    lblMensajeTasas.Visible = true;
                    CargarTasas(); // Refrescar el GridView
                }
            }
            catch (Exception ex)
            {
                lblMensajeTasas.Text = "Error al intentar eliminar la tasa: " + ex.Message;
                lblMensajeTasas.Style["background-color"] = "#f8d7da";
                lblMensajeTasas.Style["color"] = "#721c24";
                lblMensajeTasas.Visible = true;
            }
        }








        // ================= COTIZANTES =================

        private void CargarCotizantes()
        {
            try
            {
                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                var listaCotizantes = cotController.ObtenerTodos();

                gvCotizantes.DataSource = listaCotizantes;
                gvCotizantes.DataBind();

                if (listaCotizantes.Count == 0)
                {
                    lblMensajeCotizante.Text = "No se encontraron cotizantes registrados.";
                    lblMensajeCotizante.Style["background-color"] = "#d1ecf1";
                    lblMensajeCotizante.Style["color"] = "#0c5460";
                    lblMensajeCotizante.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeCotizante.Text = "Error al cargar cotizantes: " + ex.Message;
                lblMensajeCotizante.Style["background-color"] = "#f8d7da";
                lblMensajeCotizante.Style["color"] = "#721c24";
                lblMensajeCotizante.Visible = true;
            }
        }

        protected void btnNuevoCotizante_Click(object sender, EventArgs e)
        {
            LimpiarFormularioCotizante();
            CargarTiposIdEnLista(); // Método para llenar el DropDownList de tipos de cédula
            lblTituloFormularioCotizante.InnerText = "Nuevo Cotizante";
            btnGuardarCotizante.Text = "Crear";
            pnlFormCotizante.Visible = true;
            divEstadoCotizante.Visible = false; // Ocultar estado en nuevos registros
            lblMensajeCotizante.Visible = false;
        }

        protected void gvCotizantes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;
            lblMensajeCotizante.Visible = false;

            try
            {
                int idCotizante = Convert.ToInt32(gvCotizantes.DataKeys[e.NewEditIndex].Value);
                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                var cotizante = cotController.ObtenerTodos().Find(c => c.IdCotizante == idCotizante);

                if (cotizante != null)
                {
                    CargarTiposIdEnLista();
                    hfIdCotizante.Value = cotizante.IdCotizante.ToString();
                    ddlTipoIdentificacion.SelectedValue = cotizante.IdTipoIdentificacion.ToString();
                    txtIdentificacion.Text = cotizante.Identificacion;
                    txtNombreCotizante.Text = cotizante.Nombre;
                    txtApellido1.Text = cotizante.PrimerApellido;
                    txtApellido2.Text = cotizante.SegundoApellido;
                    txtCorreoCotizante.Text = cotizante.Correo;
                    txtTelefonoCotizante.Text = cotizante.Telefono;
                    chkEstadoCotizante.Checked = cotizante.Estado;

                    lblTituloFormularioCotizante.InnerText = "Editar Cotizante";
                    btnGuardarCotizante.Text = "Actualizar";
                    divEstadoCotizante.Visible = true;
                    pnlFormCotizante.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeCotizante.Text = "Error al intentar editar: " + ex.Message;
                lblMensajeCotizante.Style["background-color"] = "#f8d7da";
                lblMensajeCotizante.Style["color"] = "#721c24";
                lblMensajeCotizante.Visible = true;
            }
        }

        protected void gvCotizantes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int idCotizante = Convert.ToInt32(gvCotizantes.DataKeys[e.RowIndex].Value);
                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                bool exito = cotController.Inactivar(idCotizante, usuarioLogueado);

                if (exito)
                {
                    lblMensajeCotizante.Text = "Cotizante inactivado exitosamente.";
                    lblMensajeCotizante.Style["background-color"] = "#d4edda";
                    lblMensajeCotizante.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeCotizante.Style["color"] = "#155724";
                    lblMensajeCotizante.Visible = true;
                    CargarCotizantes();
                }
            }
            catch (Exception ex)
            {
                lblMensajeCotizante.Text = "Error al inactivar el cotizante: " + ex.Message;
                lblMensajeCotizante.Style["background-color"] = "#f8d7da";
                lblMensajeCotizante.Style["color"] = "#721c24";
                lblMensajeCotizante.Visible = true;
            }
        }

        protected void btnGuardarCotizante_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeCotizante.Visible = false;

                string identificacion = txtIdentificacion.Text.Trim();
                string nombre = txtNombreCotizante.Text.Trim();
                string apellido1 = txtApellido1.Text.Trim();
                string apellido2 = txtApellido2.Text.Trim();
                string telefono = txtTelefonoCotizante.Text.Trim();
                int tipoId = Convert.ToInt32(ddlTipoIdentificacion.SelectedValue);

                // 1. Validación de campos obligatorios
                if (string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido1))
                {
                    MostrarErrorCotizante("La identificación, el nombre y el primer apellido son obligatorios.");
                    return;
                }

                // 2. Validación de Nombre (Solo letras y espacios)
                if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    MostrarErrorCotizante("El nombre no puede contener números ni caracteres especiales.");
                    return;
                }
                if (!Regex.IsMatch(apellido1, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    MostrarErrorCotizante("El primer apellido no puede contener números ni caracteres especiales.");
                    return;
                }
                if (!string.IsNullOrEmpty(apellido2) && !Regex.IsMatch(apellido2, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    MostrarErrorCotizante("El segundo apellido no puede contener números ni caracteres especiales.");
                    return;
                }

                // 3. Validación de Teléfono (Solo números, espacios, guiones o signo +)
                if (!string.IsNullOrEmpty(telefono) && !Regex.IsMatch(telefono, @"^[0-9+\s-]+$"))
                {
                    MostrarErrorCotizante("El teléfono solo puede contener números, espacios, guiones o el signo +.");
                    return;
                }

                // 4. Validación de Formato de Identificación (según tipo)
                if (tipoId == 1) // Cédula Física Costa Rica: 9 dígitos
                {
                    if (!Regex.IsMatch(identificacion, @"^[0-9]{9}$"))
                    {
                        MostrarErrorCotizante("La cédula física debe tener exactamente 9 dígitos, sin letras ni guiones.");
                        return;
                    }
                }
                else if (tipoId == 2) // Cédula Jurídica: 10 dígitos
                {
                    if (!Regex.IsMatch(identificacion, @"^[0-9]{10}$"))
                    {
                        MostrarErrorCotizante("La cédula jurídica debe tener exactamente 10 dígitos.");
                        return;
                    }
                }
                else if (tipoId == 3) // DIMEX: 11 o 12 dígitos
                {
                    if (!Regex.IsMatch(identificacion, @"^[0-9]{11,12}$"))
                    {
                        MostrarErrorCotizante("El DIMEX debe tener entre 11 y 12 dígitos.");
                        return;
                    }
                }

                Inverix.Classes.CotizanteClass cotizante = new Inverix.Classes.CotizanteClass
                {
                    IdTipoIdentificacion = tipoId,
                    Identificacion = identificacion,
                    Nombre = nombre,
                    PrimerApellido = apellido1,
                    SegundoApellido = apellido2,
                    Correo = txtCorreoCotizante.Text.Trim(),
                    Telefono = telefono,
                    Estado = chkEstadoCotizante.Checked
                };

                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                bool exito = false;
                string mensajeExito = string.Empty;

                if (string.IsNullOrEmpty(hfIdCotizante.Value)) // Nuevo
                {
                    exito = cotController.Crear(cotizante, usuarioLogueado);
                    mensajeExito = "Cotizante creado exitosamente.";
                }
                else // Edición
                {
                    cotizante.IdCotizante = Convert.ToInt32(hfIdCotizante.Value);
                    exito = cotController.Actualizar(cotizante, usuarioLogueado);
                    mensajeExito = "Cotizante actualizado exitosamente.";
                }

                if (exito)
                {
                    pnlFormCotizante.Visible = false;
                    LimpiarFormularioCotizante();
                    CargarCotizantes();

                    lblMensajeCotizante.Text = mensajeExito;
                    lblMensajeCotizante.Style["background-color"] = "#d4edda";
                    lblMensajeCotizante.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeCotizante.Style["color"] = "#155724";
                    lblMensajeCotizante.Visible = true;
                }
                else
                {
                    MostrarErrorCotizante("No se pudo guardar el cotizante en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                MostrarErrorCotizante("Error al guardar: " + ex.Message);
            }
        }

        private void MostrarErrorCotizante(string mensaje)
        {
            lblMensajeCotizante.Text = mensaje;
            lblMensajeCotizante.Style["background-color"] = "#f8d7da";
            lblMensajeCotizante.Style["color"] = "#721c24";
            lblMensajeCotizante.Visible = true;
        }

        protected void btnCancelarCotizante_Click(object sender, EventArgs e)
        {
            pnlFormCotizante.Visible = false;
            LimpiarFormularioCotizante();
            lblMensajeCotizante.Visible = false;
        }

        private void LimpiarFormularioCotizante()
        {
            hfIdCotizante.Value = string.Empty;
            txtIdentificacion.Text = string.Empty;
            txtNombreCotizante.Text = string.Empty;
            txtApellido1.Text = string.Empty;
            txtApellido2.Text = string.Empty;
            txtCorreoCotizante.Text = string.Empty;
            txtTelefonoCotizante.Text = string.Empty;
            if (ddlTipoIdentificacion.Items.Count > 0) ddlTipoIdentificacion.SelectedIndex = 0;
        }

        protected void gvCotizantes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GenerarAcceso")
            {
                int idCotizante = Convert.ToInt32(e.CommandArgument);
                hfGenerarAccesoIdCotizante.Value = idCotizante.ToString();
                
                txtGenUsuario.Text = string.Empty;
                txtGenClave.Text = string.Empty;
                lblMensajeGenerarAcceso.Text = string.Empty;
                
                pnlGenerarAcceso.Visible = true;
            }
        }

        protected void gvCotizantes_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.HiddenField hfIdUsuario = (System.Web.UI.WebControls.HiddenField)e.Row.FindControl("hfIdUsuarioGrid");
                System.Web.UI.WebControls.LinkButton btnGenerarAcceso = (System.Web.UI.WebControls.LinkButton)e.Row.FindControl("btnGenerarAcceso");
                
                if (hfIdUsuario != null && btnGenerarAcceso != null)
                {
                    if (!string.IsNullOrEmpty(hfIdUsuario.Value))
                    {
                        btnGenerarAcceso.Visible = false;
                    }
                }
            }
        }

        protected void btnGuardarAcceso_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtGenUsuario.Text.Trim();
                string clave = txtGenClave.Text.Trim();
                int idCotizante = Convert.ToInt32(hfGenerarAccesoIdCotizante.Value);

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
                {
                    lblMensajeGenerarAcceso.Text = "Ambos campos son requeridos.";
                    return;
                }

                if (usuario.Length < 4)
                {
                    lblMensajeGenerarAcceso.Text = "El usuario debe tener al menos 4 caracteres.";
                    return;
                }
                
                if (clave.Length < 6)
                {
                    lblMensajeGenerarAcceso.Text = "La contraseña debe tener al menos 6 caracteres.";
                    return;
                }

                if (usuario.Contains(" ") || usuario.Contains("'") || usuario.Contains("\"") || usuario.Contains(";"))
                {
                    lblMensajeGenerarAcceso.Text = "El nombre de usuario contiene caracteres no permitidos.";
                    return;
                }

                string admin = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                
                bool exito = cotController.GenerarAccesoCotizante(idCotizante, usuario, clave, admin);
                
                if (exito)
                {
                    pnlGenerarAcceso.Visible = false;
                    CargarCotizantes();
                    lblMensajeCotizante.Text = "Credenciales generadas exitosamente.";
                    lblMensajeCotizante.Style["background-color"] = "#d4edda";
                    lblMensajeCotizante.Style["color"] = "#155724";
                    lblMensajeCotizante.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeGenerarAcceso.Text = ex.Message;
            }
        }

        protected void btnCancelarAcceso_Click(object sender, EventArgs e)
        {
            pnlGenerarAcceso.Visible = false;
        }

        private void CargarTiposIdEnLista()
        {
            try
            {
                ddlTipoIdentificacion.Items.Clear();

                // Llamamos al nuevo método del controlador
                CotizanteController cotController = new CotizanteController();
                DataTable dt = cotController.ListarTiposIdentificacion();

                if (dt != null && dt.Rows.Count > 0)
                {
                    ddlTipoIdentificacion.DataSource = dt;
                    // OJO: Verifica que estos nombres de columna sean los mismos que en tu INSERT/SP
                    ddlTipoIdentificacion.DataTextField = "nombre";
                    ddlTipoIdentificacion.DataValueField = "id_tipo_identificacion";
                    ddlTipoIdentificacion.DataBind();
                }

                ddlTipoIdentificacion.Items.Insert(0, new ListItem("-- Seleccione Tipo --", "0"));
            }
            catch (Exception ex)
            {
                lblMensajeSistema.Text = ex.Message;
                lblMensajeSistema.Visible = true;
            }
        }


        // ================= COTIZACIONES =================

        private void CargarCotizantesEnLista()
        {
            try
            {
                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                var lista = cotController.ObtenerTodos().FindAll(c => c.Estado);
                ddlCotizanteCotizacion.DataSource = lista;
                ddlCotizanteCotizacion.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar cotizantes: " + ex.Message, true);
            }
        }

        private void CargarTasasEnLista()
        {
            try
            {
                Inverix.Controllers.TasaController tasaController = new Inverix.Controllers.TasaController();
                var lista = tasaController.ObtenerTodas().FindAll(t => t.Estado);
                
                // Formatear el texto a mostrar en el DropDownList para que el usuario entienda
                var fuenteDatos = new List<object>();
                foreach(var t in lista)
                {
                    fuenteDatos.Add(new {
                        IdTasa = t.IdTasa,
                        // Mostrar "Producto - Plazo (Porcentaje)"
                        TextoMostrar = $"{t.NombreProducto} - {t.PlazoDias} dias ({(t.Porcentaje * 100):0.00}%)"
                    });
                }

                ddlTasaCotizacion.DataSource = fuenteDatos;
                ddlTasaCotizacion.DataTextField = "TextoMostrar";
                ddlTasaCotizacion.DataValueField = "IdTasa";
                ddlTasaCotizacion.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar tasas activas: " + ex.Message, true);
            }
        }

        protected void btnGenerarCotizacion_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeCotizacion.Visible = false;

                if (string.IsNullOrEmpty(ddlCotizanteCotizacion.SelectedValue) || string.IsNullOrEmpty(ddlTasaCotizacion.SelectedValue))
                {
                    lblMensajeCotizacion.Text = "Debe seleccionar un cotizante y una tasa.";
                    lblMensajeCotizacion.Style["background-color"] = "#f8d7da";
                    lblMensajeCotizacion.Style["color"] = "#721c24";
                    lblMensajeCotizacion.Visible = true;
                    return;
                }

                decimal monto;
                if (!decimal.TryParse(txtMontoCotizacion.Text.Trim(), out monto) || monto <= 0)
                {
                    lblMensajeCotizacion.Text = "Ingrese un monto válido mayor a 0.";
                    lblMensajeCotizacion.Style["background-color"] = "#f8d7da";
                    lblMensajeCotizacion.Style["color"] = "#721c24";
                    lblMensajeCotizacion.Visible = true;
                    return;
                }

                int idCotizante = Convert.ToInt32(ddlCotizanteCotizacion.SelectedValue);
                int idTasa = Convert.ToInt32(ddlTasaCotizacion.SelectedValue);
                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                Inverix.Controllers.TasaController tasaController = new Inverix.Controllers.TasaController();
                var tasa = tasaController.ObtenerTodas().Find(t => t.IdTasa == idTasa);

                if (tasa == null)
                {
                    lblMensajeCotizacion.Text = "La tasa seleccionada no es válida.";
                    lblMensajeCotizacion.Style["background-color"] = "#f8d7da";
                    lblMensajeCotizacion.Style["color"] = "#721c24";
                    lblMensajeCotizacion.Visible = true;
                    return;
                }

                int idUsuarioLogueado = 1; 
                Inverix.Controllers.UsuarioController uCtrl = new Inverix.Controllers.UsuarioController();

                Inverix.Controllers.CotizacionController cotController = new Inverix.Controllers.CotizacionController();
                bool exito = cotController.GenerarCotizacion(idUsuarioLogueado, idCotizante, tasa.IdProducto, tasa.IdPlazo, monto, usuarioLogueado);

                if (exito)
                {
                    lblMensajeCotizacion.Text = "Cotización creada exitosamente. Puede ver el detalle en 'Mis Cotizaciones'.";
                    lblMensajeCotizacion.Style["background-color"] = "#d4edda";
                    lblMensajeCotizacion.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeCotizacion.Style["color"] = "#155724";
                    lblMensajeCotizacion.Visible = true;
                    txtMontoCotizacion.Text = string.Empty;
                }
                else
                {
                    lblMensajeCotizacion.Text = "La base de datos no pudo procesar la cotización. Verifique los datos.";
                    lblMensajeCotizacion.Style["background-color"] = "#f8d7da";
                    lblMensajeCotizacion.Style["color"] = "#721c24";
                    lblMensajeCotizacion.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeCotizacion.Text = "Error al generar cotización: " + ex.Message;
                lblMensajeCotizacion.Style["background-color"] = "#f8d7da";
                lblMensajeCotizacion.Style["color"] = "#721c24";
                lblMensajeCotizacion.Visible = true;
            }
        }

        private void CargarFiltroCotizantes()
        {
            // Reemplazado por filtro de texto - ya no se usa dropdown
        }


        private void CargarCotizaciones(string cotizanteFiltro = null, string numeroCotizacionFiltro = null)
        {
            try
            {
                Inverix.Controllers.CotizacionController cotController = new Inverix.Controllers.CotizacionController();
                gvCotizaciones.DataSource = cotController.ObtenerTodas(cotizanteFiltro, numeroCotizacionFiltro);
                gvCotizaciones.DataBind();
            }
            catch (Exception ex)
            {
                lblMensajeListaCotizaciones.Text = "Error al cargar cotizaciones: " + ex.Message;
                lblMensajeListaCotizaciones.Style["background-color"] = "#f8d7da";
                lblMensajeListaCotizaciones.Style["color"] = "#721c24";
                lblMensajeListaCotizaciones.Visible = true;
            }
        }

        protected void btnFiltrarCotizaciones_Click(object sender, EventArgs e)
        {
            string cotizante = txtFiltroCotizante.Text.Trim();
            string numeroCot = txtFiltroNumeroCotizacion.Text.Trim();

            Session["FiltroCotizante"] = string.IsNullOrEmpty(cotizante) ? null : (object)cotizante;
            Session["FiltroNumeroCotizacion"] = string.IsNullOrEmpty(numeroCot) ? null : (object)numeroCot;

            pnlDetallesView.Visible = false;
        }

        protected void btnVerTodos_Click(object sender, EventArgs e)
        {
            Session["FiltroCotizante"] = null;
            Session["FiltroNumeroCotizacion"] = null;
            txtFiltroCotizante.Text = string.Empty;
            txtFiltroNumeroCotizacion.Text = string.Empty;
            CargarCotizaciones();
            pnlDetallesView.Visible = false;
        }

        protected void gvCotizaciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalles")
            {
                int idCotizacion = Convert.ToInt32(e.CommandArgument);
                hfIdCotizacionActual.Value = idCotizacion.ToString();
                pnlDetallesView.Visible = true;
                CargarDetallesCotizacion(idCotizacion);
            }
            else if (e.CommandName == "EditarCotizacion")
            {
                try
                {
                    int idCotizacion = Convert.ToInt32(e.CommandArgument);
                    hfIdCotizacionEditar.Value = idCotizacion.ToString();

                    // Poblar dropdowns de edición
                    CargarDropdownsCotizanteEdicion();
                    CargarDropdownsTasaEdicion();

                    // Obtener datos actuales de la cotización para PreSeleccionar
                    Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                    var detalle = cotCtrl.ObtenerDetallePorId(idCotizacion);
                    if (detalle != null)
                    {
                        // Pre-seleccionar cotizante
                        if (ddlCotizanteEdicion.Items.FindByValue(detalle.IdCotizante.ToString()) != null)
                            ddlCotizanteEdicion.SelectedValue = detalle.IdCotizante.ToString();
                        // Pre-seleccionar tasa
                        if (ddlTasaEdicion.Items.FindByValue(detalle.IdTasa.ToString()) != null)
                            ddlTasaEdicion.SelectedValue = detalle.IdTasa.ToString();
                        txtMontoEdicion.Text = detalle.Monto.ToString("F2");
                    }

                    pnlDetallesView.Visible = false;
                    pnlEditarCotizacion.Visible = true;
                    lblMensajeEdicion.Visible = false;
                }
                catch (Exception ex)
                {
                    lblMensajeListaCotizaciones.Text = "Error al cargar datos para edición: " + ex.Message;
                    lblMensajeListaCotizaciones.Style["background-color"] = "#f8d7da";
                    lblMensajeListaCotizaciones.Style["color"] = "#721c24";
                    lblMensajeListaCotizaciones.Visible = true;
                }
            }
            else if (e.CommandName == "InactivarCotizacion")
            {
                try
                {
                    int idCotizacion = Convert.ToInt32(e.CommandArgument);
                    string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";

                    Inverix.Controllers.CotizacionController cotController = new Inverix.Controllers.CotizacionController();
                    bool exito = cotController.InactivarCotizacion(idCotizacion, usuarioLogueado);

                    if (exito)
                    {
                        lblMensajeListaCotizaciones.Text = "Cotización inactivada exitosamente.";
                        lblMensajeListaCotizaciones.Style["background-color"] = "#d4edda";
                        lblMensajeListaCotizaciones.Style["border"] = "1px solid #c3e6cb";
                        lblMensajeListaCotizaciones.Style["color"] = "#155724";
                        lblMensajeListaCotizaciones.Visible = true;
                        CargarCotizaciones(); // Recargar historial de cotizaciones
                        pnlDetallesView.Visible = false; // Ocultar detalles por si acaso
                    }
                    else
                    {
                        lblMensajeListaCotizaciones.Text = "No se pudo inactivar la cotización.";
                        lblMensajeListaCotizaciones.Style["background-color"] = "#f8d7da";
                        lblMensajeListaCotizaciones.Style["color"] = "#721c24";
                        lblMensajeListaCotizaciones.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    lblMensajeListaCotizaciones.Text = "Error al inactivar cotización: " + ex.Message;
                    lblMensajeListaCotizaciones.Style["background-color"] = "#f8d7da";
                    lblMensajeListaCotizaciones.Style["color"] = "#721c24";
                    lblMensajeListaCotizaciones.Visible = true;
                }
            }
        }

        private void CargarDetallesCotizacion(int idCotizacion)
        {
            try
            {
                Inverix.Controllers.CotizacionController cotController = new Inverix.Controllers.CotizacionController();
                gvDetallesCotizacion.DataSource = cotController.ObtenerDetallesActivos(idCotizacion);
                gvDetallesCotizacion.DataBind();
            }
            catch (Exception ex)
            {
                lblMensajeListaCotizaciones.Text = "Error al cargar detalles de la cotización: " + ex.Message;
                lblMensajeListaCotizaciones.Style["background-color"] = "#f8d7da";
                lblMensajeListaCotizaciones.Style["color"] = "#721c24";
                lblMensajeListaCotizaciones.Visible = true;
            }
        }
        // Acumuladores para el pie de totales del GridView de detalles
        private decimal _totalInteresBruto = 0;
        private decimal _totalImpuesto = 0;
        private decimal _totalInteresNeto = 0;

        protected void gvDetallesCotizacion_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                // DataSource es List<DetalleCotizacionClass>, leer directo del objeto
                Inverix.Classes.DetalleCotizacionClass item = (Inverix.Classes.DetalleCotizacionClass)e.Row.DataItem;
                _totalInteresBruto += item.InteresBruto;
                _totalImpuesto     += item.Impuesto;
                _totalInteresNeto  += item.InteresNeto;
            }
        }

        protected void gvDetallesCotizacion_DataBound(object sender, EventArgs e)
        {
            // Solo agregar fila de totales si hay datos
            if (gvDetallesCotizacion.Rows.Count == 0) return;

            System.Web.UI.WebControls.GridViewRow footerRow = gvDetallesCotizacion.FooterRow;
            if (footerRow == null) return;

            // Estilo general de la fila totales
            footerRow.BackColor = System.Drawing.ColorTranslator.FromHtml("#1693A5");
            footerRow.ForeColor = System.Drawing.Color.White;
            footerRow.Font.Bold = true;

            // Celda 0: etiqueta
            footerRow.Cells[0].Text = "TOTALES";
            footerRow.Cells[0].ForeColor = System.Drawing.Color.White;
            footerRow.Cells[0].Font.Bold = true;

            // Celdas 1, 2 y 3: valores acumulados formateados
            footerRow.Cells[1].Text = _totalInteresBruto.ToString("N2");
            footerRow.Cells[2].Text = _totalImpuesto.ToString("N2");
            footerRow.Cells[3].Text = _totalInteresNeto.ToString("N2");

            // Alinear a la derecha los números
            footerRow.Cells[1].HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
            footerRow.Cells[2].HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
            footerRow.Cells[3].HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;

            // Resetear acumuladores para llamadas posteriores
            _totalInteresBruto = 0;
            _totalImpuesto = 0;
            _totalInteresNeto = 0;
        }

        protected void btnCerrarDetalles_Click(object sender, EventArgs e)
        {
            pnlDetallesView.Visible = false;
        }

        private void CargarDropdownsCotizanteEdicion()
        {
            try
            {
                Inverix.Controllers.CotizanteController cCtrl = new Inverix.Controllers.CotizanteController();
                var lista = cCtrl.ObtenerTodos().FindAll(c => c.Estado);
                ddlCotizanteEdicion.Items.Clear();
                ddlCotizanteEdicion.Items.Add(new System.Web.UI.WebControls.ListItem("-- Seleccione Cotizante --", "0"));
                foreach (var c in lista)
                    ddlCotizanteEdicion.Items.Add(new System.Web.UI.WebControls.ListItem(
                        c.Nombre + " " + c.PrimerApellido, c.IdCotizante.ToString()));
            }
            catch { }
        }

        private void CargarDropdownsTasaEdicion()
        {
            try
            {
                Inverix.Controllers.TasaController tCtrl = new Inverix.Controllers.TasaController();
                var lista = tCtrl.ObtenerTodas().FindAll(t => t.Estado);
                ddlTasaEdicion.Items.Clear();
                ddlTasaEdicion.Items.Add(new System.Web.UI.WebControls.ListItem("-- Seleccione Tasa --", "0"));
                foreach (var t in lista)
                    ddlTasaEdicion.Items.Add(new System.Web.UI.WebControls.ListItem(
                        t.NombreProducto + " | " + t.PlazoDias + " días | " + t.Porcentaje.ToString("P2"),
                        t.IdTasa.ToString()));
            }
            catch { }
        }

        protected void btnActualizarCotizacion_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensajeEdicion.Visible = false;

                int idCotizacion = Convert.ToInt32(hfIdCotizacionEditar.Value);
                int idCotizante  = Convert.ToInt32(ddlCotizanteEdicion.SelectedValue);
                int idTasa       = Convert.ToInt32(ddlTasaEdicion.SelectedValue);
                decimal monto;

                if (idCotizante == 0 || idTasa == 0)
                {
                    lblMensajeEdicion.Text = "Debe seleccionar un Cotizante y una Tasa.";
                    lblMensajeEdicion.Style["background-color"] = "#f8d7da";
                    lblMensajeEdicion.Style["color"] = "#721c24";
                    lblMensajeEdicion.Visible = true;
                    return;
                }

                if (!decimal.TryParse(txtMontoEdicion.Text.Trim(), out monto) || monto <= 0)
                {
                    lblMensajeEdicion.Text = "Ingrese un monto válido mayor a cero.";
                    lblMensajeEdicion.Style["background-color"] = "#f8d7da";
                    lblMensajeEdicion.Style["color"] = "#721c24";
                    lblMensajeEdicion.Visible = true;
                    return;
                }

                string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                bool exito = cotCtrl.ActualizarCotizacion(idCotizacion, idCotizante, idTasa, monto, usuarioLogueado);

                if (exito)
                {
                    pnlEditarCotizacion.Visible = false;
                    string filtroCot = Session["FiltroCotizante"] != null ? Session["FiltroCotizante"].ToString() : null;
                    string filtroNum = Session["FiltroNumeroCotizacion"] != null ? Session["FiltroNumeroCotizacion"].ToString() : null;
                    CargarCotizaciones(filtroCot, filtroNum);

                    // Mensaje verde arriba del historial (JS lo oculta en 3 s)
                    lblMensajeListaCotizaciones.Text = "Cotización actualizada exitosamente.";
                    lblMensajeListaCotizaciones.Style["background-color"] = "#d4edda";
                    lblMensajeListaCotizaciones.Style["border"] = "1px solid #c3e6cb";
                    lblMensajeListaCotizaciones.Style["color"] = "#155724";
                    lblMensajeListaCotizaciones.Visible = true;
                }
                else
                {
                    lblMensajeEdicion.Text = "No se pudo actualizar la cotización.";
                    lblMensajeEdicion.Style["background-color"] = "#f8d7da";
                    lblMensajeEdicion.Style["color"] = "#721c24";
                    lblMensajeEdicion.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensajeEdicion.Text = "Error al actualizar: " + ex.Message;
                lblMensajeEdicion.Style["background-color"] = "#f8d7da";
                lblMensajeEdicion.Style["color"] = "#721c24";
                lblMensajeEdicion.Visible = true;
            }
        }

        protected void btnCancelarEdicion_Click(object sender, EventArgs e)
        {
            pnlEditarCotizacion.Visible = false;
            lblMensajeEdicion.Visible = false;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Se requiere para exportar GridView a Excel sin errores.
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=MisCotizaciones.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        // Ocultar la columna de acciones (Ver Detalles) para la exportación
                        gvCotizaciones.Columns[5].Visible = false;

                        gvCotizaciones.RenderControl(hw);

                        // Restaurar visibilidad
                        gvCotizaciones.Columns[5].Visible = true;

                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensajeListaCotizaciones.Text = "Error al exportar a Excel: " + ex.Message;
                lblMensajeListaCotizaciones.Style["background-color"] = "#f8d7da";
                lblMensajeListaCotizaciones.Style["color"] = "#721c24";
                lblMensajeListaCotizaciones.Visible = true;
            }
        }






        // ==========================================
        //  EVENTOS DE NAVEGACIÓN Y REGISTRO
        // ==========================================
        protected void lnkMostrarRegistro_Click(object sender, EventArgs e)
        {
            // Ocultar Login y mostrar Registro
            pnlLogin.Visible = false;
            pnlRegistro.Visible = true;
            lblMensajeRegistro.Text = "";

            // Cargar los tipos de identificacion en el DropDown
            CargarTiposIdentificacionRegistro();
        }
        protected void btnVolverLogin_Click(object sender, EventArgs e)
        {
            // Ocultar Registro y volver a Login
            pnlRegistro.Visible = false;
            pnlLogin.Visible = true;
            lblMensaje.Text = "";
        }
        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtRegUsuario.Text.Trim();
                string clave = txtRegClave.Text.Trim();
                string identificacion = txtRegIdentificacion.Text.Trim();
                string nombre = txtRegNombre.Text.Trim();
                string apellido1 = txtRegPrimerApellido.Text.Trim();
                string apellido2 = txtRegSegundoApellido.Text.Trim();
                string telefono = txtRegTelefono.Text.Trim();

                // Validación básica de campos vacíos
                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave) || string.IsNullOrWhiteSpace(identificacion) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido1))
                {
                    lblMensajeRegistro.Text = "Por favor, complete todos los campos obligatorios (Usuario, Clave, Identificación, Nombre y Primer Apellido).";
                    return;
                }

                // Validación de Seguridad: Longitud Mínima
                if (usuario.Length < 4)
                {
                    lblMensajeRegistro.Text = "El usuario debe tener al menos 4 caracteres.";
                    return;
                }
                
                if (clave.Length < 6)
                {
                    lblMensajeRegistro.Text = "La contraseña ingresada no cumple con la longitud mínima de seguridad de 6 caracteres .";
                    return;
                }

                // Validación de Seguridad: Inyección / Caracteres no permitidos
                if (usuario.Contains(" ") || usuario.Contains("'") || usuario.Contains("\"") || usuario.Contains(";"))
                {
                    lblMensajeRegistro.Text = "El nombre de usuario contiene caracteres de sistema no permitidos.";
                    return;
                }

                // Validar contraseñas
                if (clave != txtRegConfirmarClave.Text.Trim())
                {
                    lblMensajeRegistro.Text = "Las contraseñas no coinciden.";
                    return;
                }

                // Validación de Nombre (Solo letras y espacios)
                if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    lblMensajeRegistro.Text = "El nombre no puede contener números ni caracteres especiales.";
                    return;
                }
                if (!Regex.IsMatch(apellido1, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    lblMensajeRegistro.Text = "El primer apellido no puede contener números ni caracteres especiales.";
                    return;
                }
                if (!string.IsNullOrEmpty(apellido2) && !Regex.IsMatch(apellido2, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    lblMensajeRegistro.Text = "El segundo apellido no puede contener números ni caracteres especiales.";
                    return;
                }

                // Validación de Teléfono (Solo números, espacios, guiones o signo +)
                if (!string.IsNullOrEmpty(telefono) && !Regex.IsMatch(telefono, @"^[0-9+\s-]+$"))
                {
                    lblMensajeRegistro.Text = "El teléfono solo puede contener números, espacios, guiones o el signo +.";
                    return;
                }

                int idTipoId = Convert.ToInt32(ddlRegTipoIdentificacion.SelectedValue);

                // Validación de Formato de Identificación (según tipo)
                if (idTipoId == 1) // Cédula Física Costa Rica: 9 dígitos
                {
                    if (!Regex.IsMatch(identificacion, @"^[0-9]{9}$"))
                    {
                        lblMensajeRegistro.Text = "La cédula física debe tener exactamente 9 dígitos, sin letras ni guiones.";
                        return;
                    }
                }
                else if (idTipoId == 2) // Cédula Jurídica: 10 dígitos
                {
                    if (!Regex.IsMatch(identificacion, @"^[0-9]{10}$"))
                    {
                        lblMensajeRegistro.Text = "La cédula jurídica debe tener exactamente 10 dígitos.";
                        return;
                    }
                }
                else if (idTipoId == 3) // DIMEX: 11 o 12 dígitos
                {
                    if (!Regex.IsMatch(identificacion, @"^[0-9]{11,12}$"))
                    {
                        lblMensajeRegistro.Text = "El DIMEX debe tener entre 11 y 12 dígitos.";
                        return;
                    }
                }

                // Llamar al Controlador
                Inverix.Controllers.UsuarioController uController = new Inverix.Controllers.UsuarioController();

                bool exito = uController.RegistrarCotizanteWeb(
                    usuario,
                    clave,
                    idTipoId,
                    identificacion,
                    nombre,
                    apellido1,
                    apellido2,
                    txtRegCorreo.Text.Trim(),
                    telefono
                );
                if (exito)
                {
                    // Si todo salió bien, limpiar formulario, volver al login y mostrar mensaje de éxito
                    pnlRegistro.Visible = false;
                    pnlLogin.Visible = true;
                    lblMensaje.Text = "¡Cuenta creada con éxito! Ahora puede iniciar sesión.";
                    lblMensaje.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblMensajeRegistro.Text = "Error al intentar registrar la cuenta: " + ex.Message;
                lblMensajeRegistro.ForeColor = System.Drawing.Color.Red;
            }
        }
        // ==========================================
        //  MÉTODO DE APOYO (Cargar DropDown)
        // ==========================================
        private void CargarTiposIdentificacionRegistro()
        {
            try
            {
                Inverix.Controllers.CotizanteController cotController = new Inverix.Controllers.CotizanteController();
                System.Data.DataTable dtTipos = cotController.ListarTiposIdentificacion();

                ddlRegTipoIdentificacion.DataSource = dtTipos;
                ddlRegTipoIdentificacion.DataTextField = "nombre";
                ddlRegTipoIdentificacion.DataValueField = "id_tipo_identificacion";
                ddlRegTipoIdentificacion.DataBind();
            }
            catch (Exception)
            {
                lblMensajeRegistro.Text = "Ocurrió un error al cargar los tipos de identificación.";
            }
        }

        // ==========================================
        //  CRUD USUARIOS
        // ==========================================
        private void CargarUsuarios()
        {
            try
            {
                Inverix.Controllers.UsuarioController uCtrl = new Inverix.Controllers.UsuarioController();
                gvUsuarios.DataSource = uCtrl.ObtenerTodos();
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                MostrarErrorUsuario("Error cargando usuarios: " + ex.Message);
            }
        }

        private void CargarRolesDropdown()
        {
            try
            {
                Inverix.Controllers.RoleController rCtrl = new Inverix.Controllers.RoleController();
                ddlRolUsuario.DataSource = rCtrl.ObtenerTodos();
                ddlRolUsuario.DataTextField = "Nombre";
                ddlRolUsuario.DataValueField = "IdRole";
                ddlRolUsuario.DataBind();
            }
            catch { }
        }

        private void MostrarErrorUsuario(string msg)
        {
            lblMensajeUsuario.Text = msg;
            lblMensajeUsuario.Style["background-color"] = "#f8d7da";
            lblMensajeUsuario.Style["color"] = "#721c24";
            lblMensajeUsuario.Visible = true;
        }

        private void MostrarExitoUsuario(string msg)
        {
            lblMensajeUsuario.Text = msg;
            lblMensajeUsuario.Style["background-color"] = "#d4edda";
            lblMensajeUsuario.Style["color"] = "#155724";
            lblMensajeUsuario.Visible = true;
        }

        protected void btnNuevoUsuario_Click(object sender, EventArgs e)
        {
            LimpiarFormularioUsuario();
            CargarRolesDropdown();
            lblTituloFormularioUsuario.InnerText = "Nuevo Usuario";
            divClaveUsuario.Visible = true;
            pnlFormUsuario.Visible = true;
            lblMensajeUsuario.Visible = false;
        }

        private void LimpiarFormularioUsuario()
        {
            hfIdUsuario.Value = string.Empty;
            txtNombreUsuario.Text = string.Empty;
            txtClaveUsuario.Text = string.Empty;
            if (ddlRolUsuario.Items.Count > 0) ddlRolUsuario.SelectedIndex = 0;
        }

        protected void gvUsuarios_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            e.Cancel = true;
            LimpiarFormularioUsuario();
            CargarRolesDropdown();
            
            lblMensajeUsuario.Visible = false;
            int idUsuario = Convert.ToInt32(gvUsuarios.DataKeys[e.NewEditIndex].Values["IdUsuario"]);
            string rolCodigoStr = gvUsuarios.DataKeys[e.NewEditIndex].Values["RolCodigo"] != null ? gvUsuarios.DataKeys[e.NewEditIndex].Values["RolCodigo"].ToString() : "";
            
            System.Web.UI.WebControls.GridViewRow row = gvUsuarios.Rows[e.NewEditIndex];
            string nombreUsuario = row.Cells[1].Text;

            hfIdUsuario.Value = idUsuario.ToString();
            txtNombreUsuario.Text = nombreUsuario;
            
            if (!string.IsNullOrEmpty(rolCodigoStr))
            {
                try {
                    ddlRolUsuario.SelectedValue = rolCodigoStr;
                } catch { }
            }

            lblTituloFormularioUsuario.InnerText = "Editar Usuario (La clave no se puede ver ni editar aquí)";
            divClaveUsuario.Visible = false;
            pnlFormUsuario.Visible = true;
        }

        protected void gvUsuarios_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            try
            {
                int idUsuario = Convert.ToInt32(gvUsuarios.DataKeys[e.RowIndex].Values["IdUsuario"]);
                Inverix.Controllers.UsuarioController uCtrl = new Inverix.Controllers.UsuarioController();
                string admin = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                
                if (uCtrl.Inactivar(idUsuario, admin))
                {
                    CargarUsuarios();
                    MostrarExitoUsuario("Usuario inactivado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MostrarErrorUsuario("Error al inactivar usuario: " + ex.Message);
            }
        }

        protected void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                string admin = Session["Usuario"] != null ? Session["Usuario"].ToString() : "admin";
                Inverix.Controllers.UsuarioController uCtrl = new Inverix.Controllers.UsuarioController();
                
                string usuario = txtNombreUsuario.Text.Trim();
                int idRol = Convert.ToInt32(ddlRolUsuario.SelectedValue);

                if (usuario.Length < 4)
                {
                    MostrarErrorUsuario("El nombre de usuario debe tener mínimo 4 caracteres.");
                    return;
                }
                if (usuario.Contains(" ") || usuario.Contains("'"))
                {
                    MostrarErrorUsuario("Usuario contiene caracteres inválidos.");
                    return;
                }

                if (string.IsNullOrEmpty(hfIdUsuario.Value))
                {
                    // Crear
                    string clave = txtClaveUsuario.Text.Trim();
                    if (clave.Length < 6)
                    {
                        MostrarErrorUsuario("La clave debe tener mínimo 6 caracteres.");
                        return;
                    }
                    if (uCtrl.Crear(usuario, clave, idRol, admin))
                    {
                        MostrarExitoUsuario("Usuario creado.");
                        pnlFormUsuario.Visible = false;
                        CargarUsuarios();
                    }
                    else
                    {
                        MostrarErrorUsuario("El nombre de usuario ya existe o hubo un error.");
                    }
                }
                else
                {
                    // Actualizar
                    int idEdit = Convert.ToInt32(hfIdUsuario.Value);
                    if (uCtrl.Actualizar(idEdit, usuario, idRol, admin))
                    {
                        MostrarExitoUsuario("Usuario actualizado.");
                        pnlFormUsuario.Visible = false;
                        CargarUsuarios();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorUsuario("Error: " + ex.Message);
            }
        }

        protected void btnCancelarUsuario_Click(object sender, EventArgs e)
        {
            pnlFormUsuario.Visible = false;
            lblMensajeUsuario.Visible = false;
        }

        // ==========================================
        //  ROLES (READ)
        // ==========================================
        private void CargarRoles()
        {
            try
            {
                Inverix.Controllers.RoleController rCtrl = new Inverix.Controllers.RoleController();
                gvRoles.DataSource = rCtrl.ObtenerTodos();
                gvRoles.DataBind();
            }
            catch { }
        }

        // Convierte excepciones SQL en mensajes comprensibles para el usuario
        private string ObtenerMensajeAmigable(Exception ex, string entidad)
        {
            string msg = ex.Message;
            // Error de clave duplicada (UNIQUE constraint / PRIMARY KEY)
            if (msg.Contains("UNIQUE") || msg.Contains("duplicate") || msg.Contains("Violation") || msg.Contains("IX_") || msg.Contains("PRIMARY KEY"))
                return $"Ya existe un {entidad} con ese código o nombre. Por favor verifique los datos ingresados.";
            // Referencia de clave foránea
            if (msg.Contains("FOREIGN KEY") || msg.Contains("REFERENCE"))
                return $"No se puede eliminar este {entidad} porque está siendo utilizado en otro registro.";
            // Error de valor nulo
            if (msg.Contains("NULL") || msg.Contains("null"))
                return $"Hay campos obligatorios vacíos. Por favor complete todos los datos requeridos.";
            // Genérico amigable
            return $"Ocurrió un error al guardar el {entidad}. Por favor intente de nuevo.";
        }

        // ==========================================
        //  REPORTES ADMINISTRATIVOS
        // ==========================================
        private void CargarCotizantesReporte()
        {
            try
            {
                CotizanteController cCtrl = new CotizanteController();
                var lista = cCtrl.ObtenerTodos();
                
                ddlReporteCotizante.Items.Clear();
                ddlReporteCotizante.DataSource = lista;
                ddlReporteCotizante.DataTextField = "Nombre";
                ddlReporteCotizante.DataValueField = "IdCotizante";
                ddlReporteCotizante.DataBind();
                
                ddlReporteCotizante.Items.Insert(0, new ListItem("-- Todos los Cotizantes --", "0"));
            }
            catch { }
        }

        private void CargarReporteAdmin(bool force)
        {
            try
            {
                if (!force && gvReporteAdmin.Rows.Count > 0) return;

                int idCotizante = Convert.ToInt32(ddlReporteCotizante.SelectedValue);
                DateTime? desde = null;
                DateTime? hasta = null;

                if (!string.IsNullOrEmpty(txtFechaDesdeAdmin.Text)) desde = Convert.ToDateTime(txtFechaDesdeAdmin.Text);
                if (!string.IsNullOrEmpty(txtFechaHastaAdmin.Text)) hasta = Convert.ToDateTime(txtFechaHastaAdmin.Text);

                CotizacionController cCtrl = new CotizacionController();
                DataTable dt = cCtrl.ObtenerParaReporte(idCotizante, desde, hasta);
                
                gvReporteAdmin.DataSource = dt;
                gvReporteAdmin.DataBind();

                if (dt.Rows.Count == 0)
                    lblMensajeReporteAdmin.Text = "No hay datos para los filtros seleccionados.";
                else
                    lblMensajeReporteAdmin.Text = "";
            }
            catch (Exception ex)
            {
                lblMensajeReporteAdmin.Text = "Error: " + ex.Message;
            }
        }

        protected void btnFiltrarReporteAdmin_Click(object sender, EventArgs e)
        {
            CargarReporteAdmin(true);
        }

        protected void btnReportePDFAdmin_Click(object sender, EventArgs e)
        {
            ExportarReporteAdmin("PDF");
        }

        protected void btnReporteExcelAdmin_Click(object sender, EventArgs e)
        {
            ExportarReporteAdmin("Excel");
        }

        private void ExportarReporteAdmin(string formato)
        {
            try
            {
                int idCotizante = Convert.ToInt32(ddlReporteCotizante.SelectedValue);
                DateTime? desde = null;
                DateTime? hasta = null;
                if (!string.IsNullOrEmpty(txtFechaDesdeAdmin.Text)) desde = Convert.ToDateTime(txtFechaDesdeAdmin.Text);
                if (!string.IsNullOrEmpty(txtFechaHastaAdmin.Text)) hasta = Convert.ToDateTime(txtFechaHastaAdmin.Text);

                CotizacionController cCtrl = new CotizacionController();
                DataTable dt = cCtrl.ObtenerParaReporte(idCotizante, desde, hasta);

                if (dt.Rows.Count == 0)
                {
                    lblMensajeReporteAdmin.Text = "No hay datos para exportar.";
                    return;
                }

                string nombreReporte = (idCotizante == 0) ? "Reporte Global Financiero" : "Reporte Financiero por Cliente";
                string nombreFirma = (idCotizante == 0) ? "Corte Administrativo General" : ddlReporteCotizante.SelectedItem.Text;

                if (formato == "PDF")
                    ReporteClass.GenerarPDF(dt, nombreReporte, nombreFirma, Response);
                else
                    ReporteClass.GenerarExcel(dt, nombreReporte, Response);
            }
            catch (Exception ex)
            {
                lblMensajeReporteAdmin.Text = "Error al exportar: " + ex.Message;
            }
        }
    }
}

