using System;
using System.Data;
using System.Web.UI.WebControls;
using Inverix.Classes;

namespace Inverix.Views
{
    public partial class CotizanteForm : System.Web.UI.Page
    {
        // Acumuladores para totales del GridView de detalles (Mis Cotizaciones)
        private decimal _totalBruto = 0;
        private decimal _totalImpuesto = 0;
        private decimal _totalNeto = 0;

        // Acumuladores para totales del GridView de detalles (Mis Reportes)
        private decimal _totalBrutoReporte = 0;
        private decimal _totalImpuestoReporte = 0;
        private decimal _totalNetoReporte = 0;

        // ================================================
        //  PAGE LOAD
        // ================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad: solo pueden entrar usuarios COT autenticados
            if (Session["Usuario"] == null || Session["RolCodigo"] == null)
            {
                Response.Redirect("InverixForm.aspx");
                return;
            }

            string rolCodigo = Session["RolCodigo"].ToString().Trim().ToUpper();
            if (rolCodigo != "COT")
            {
                Response.Redirect("InverixForm.aspx");
                return;
            }

            // Mostrar nombre en header
            lblUsuarioLogueado.Text = "usuario: " + Session["Usuario"].ToString();

            if (!IsPostBack)
            {
                // Cargar nombre completo en Inicio y Perfil
                CargarDatosCotizante();
                OcultarTodosLosPaneles();
                pnlInicio.Visible = true;
            }
            else
            {
                // Restaurar panel activo en postback
                string panel = Session["PanelActivoCot"] != null ? Session["PanelActivoCot"].ToString() : "Inicio";
                OcultarTodosLosPaneles();
                switch (panel)
                {
                    case "Finanzas":    pnlFinanzas.Visible = true; break;
                    case "NuevaCot":    pnlNuevaCotizacion.Visible = true; break;
                    case "MisCot":      pnlMisCotizaciones.Visible = true; break;
                    case "Reportes":    pnlReportes.Visible = true; break;
                    case "MiPerfil":    pnlMiPerfil.Visible = true; break;
                    default:            pnlInicio.Visible = true; break;
                }
            }
        }

        // ================================================
        //  PAGE PRE-RENDER (DataBind después de eventos)
        // ================================================
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack || Session["Usuario"] == null) return;

            string panel = Session["PanelActivoCot"] != null ? Session["PanelActivoCot"].ToString() : "Inicio";
            if (panel == "Finanzas")    CargarTasasInfo();
            if (panel == "NuevaCot")    CargarDropdownTasa();
            if (panel == "MisCot")      CargarMisCotizaciones();
            if (panel == "Reportes")    CargarReporte();
            if (panel == "MiPerfil")    CargarDatosPerfil();
        }

        // ================================================
        //  HELPERS
        // ================================================
        private void OcultarTodosLosPaneles()
        {
            pnlInicio.Visible = false;
            pnlFinanzas.Visible = false;
            pnlNuevaCotizacion.Visible = false;
            pnlMisCotizaciones.Visible = false;
            pnlReportes.Visible = false;
            pnlMiPerfil.Visible = false;
        }

        private int GetIdCotizante()
        {
            if (Session["IdCotizante"] != null && Session["IdCotizante"] is int)
                return (int)Session["IdCotizante"];
            if (Session["IdCotizante"] != null)
                return Convert.ToInt32(Session["IdCotizante"]);
            return 0;
        }

        private int GetIdUsuario()
        {
            if (Session["IdUsuario"] != null)
                return Convert.ToInt32(Session["IdUsuario"]);
            return 0;
        }

        private void MostrarMensaje(System.Web.UI.WebControls.Label lbl, string msg, bool esError)
        {
            lbl.Text = msg;
            lbl.Style["background-color"] = esError ? "#f8d7da" : "#d4edda";
            lbl.Style["color"] = esError ? "#721c24" : "#155724";
            lbl.Style["border"] = esError ? "1px solid #f5c6cb" : "1px solid #c3e6cb";
            lbl.Visible = true;
        }

        // ================================================
        //  NAVEGACIÓN
        // ================================================
        protected void lnkInicio_Click(object sender, EventArgs e)
        {
            Session["PanelActivoCot"] = "Inicio";
            OcultarTodosLosPaneles();
            pnlInicio.Visible = true;
        }

        protected void lnkFinanzas_Click(object sender, EventArgs e)
        {
            Session["PanelActivoCot"] = "Finanzas";
            OcultarTodosLosPaneles();
            CargarTasasInfo();
            pnlFinanzas.Visible = true;
        }

        protected void lnkNuevaCotizacion_Click(object sender, EventArgs e)
        {
            Session["PanelActivoCot"] = "NuevaCot";
            OcultarTodosLosPaneles();
            CargarDropdownTasa();
            pnlNuevaCotizacion.Visible = true;
        }

        protected void lnkMisCotizaciones_Click(object sender, EventArgs e)
        {
            Session["PanelActivoCot"] = "MisCot";
            OcultarTodosLosPaneles();
            CargarMisCotizaciones();
            gvMisCotizaciones.SelectedIndex = -1;
            foreach (GridViewRow row in gvMisCotizaciones.Rows) row.Visible = true;
            pnlMisCotizaciones.Visible = true;
        }

        protected void lnkReportes_Click(object sender, EventArgs e)
        {
            Session["PanelActivoCot"] = "Reportes";
            OcultarTodosLosPaneles();
            CargarReporte();
            pnlReportes.Visible = true;
        }

        protected void lnkMiPerfil_Click(object sender, EventArgs e)
        {
            Session["PanelActivoCot"] = "MiPerfil";
            OcultarTodosLosPaneles();
            CargarDatosPerfil();
            pnlMiPerfil.Visible = true;
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("InverixForm.aspx");
        }

        // ================================================
        //  CARGAR DATOS DE COTIZANTE
        // ================================================
        private void CargarDatosCotizante()
        {
            try
            {
                int idCotizante = GetIdCotizante();
                if (idCotizante == 0) return;

                Inverix.Controllers.CotizanteController cCtrl = new Inverix.Controllers.CotizanteController();
                var cotizante = cCtrl.ObtenerTodos().Find(c => c.IdCotizante == idCotizante);

                if (cotizante != null)
                {
                    string nombreCompleto = cotizante.Nombre + " " + cotizante.PrimerApellido + " " + cotizante.SegundoApellido;
                    lblNombreCompleto.Text = nombreCompleto;
                    Session["NombreCompletoCot"] = nombreCompleto;
                    Session["CorreoCot"] = cotizante.Correo;
                    Session["TelefonoCot"] = cotizante.Telefono;
                    Session["IdentificacionCot"] = cotizante.Identificacion;
                }
            }
            catch { }
        }

        private void CargarDatosPerfil()
        {
            lblPerfilNombre.Text = Session["NombreCompletoCot"] != null ? Session["NombreCompletoCot"].ToString() : "";
            lblPerfilUsuario.Text = Session["Usuario"] != null ? Session["Usuario"].ToString() : "";
            lblPerfilCorreo.Text = Session["CorreoCot"] != null ? Session["CorreoCot"].ToString() : "";
            lblPerfilTelefono.Text = Session["TelefonoCot"] != null ? Session["TelefonoCot"].ToString() : "";
            lblPerfilIdentificacion.Text = Session["IdentificacionCot"] != null ? Session["IdentificacionCot"].ToString() : "";
        }

        // ================================================
        //  FINANZAS — TASAS INFO
        // ================================================
        private void CargarTasasInfo()
        {
            try
            {
                Inverix.Controllers.TasaController tCtrl = new Inverix.Controllers.TasaController();
                var tasas = tCtrl.ObtenerTodas();

                var tabla = new System.Data.DataTable();
                tabla.Columns.Add("Producto");
                tabla.Columns.Add("Moneda");
                tabla.Columns.Add("Plazo");
                tabla.Columns.Add("Tasa", typeof(decimal));

                foreach (var t in tasas)
                {
                    if (t.Estado)
                        tabla.Rows.Add(t.NombreProducto, t.NombreMoneda, t.PlazoDias + " días", t.Porcentaje);
                }

                gvTasasInfo.DataSource = tabla;
                gvTasasInfo.DataBind();
            }
            catch { }
        }

        // ================================================
        //  NUEVA COTIZACIÓN
        // ================================================
        private void CargarDropdownTasa()
        {
            CargarListaTasas(ddlTasaCot);
        }

        private void CargarListaTasas(DropDownList ddl)
        {
            try
            {
                if (ddl.Items.Count > 1) return;
                Inverix.Controllers.TasaController tCtrl = new Inverix.Controllers.TasaController();
                var tasas = tCtrl.ObtenerTodas().FindAll(t => t.Estado);

                ddl.Items.Clear();
                ddl.Items.Add(new System.Web.UI.WebControls.ListItem("-- Seleccione una tasa --", "0"));
                foreach (var t in tasas)
                    ddl.Items.Add(new System.Web.UI.WebControls.ListItem(
                        t.NombreProducto + " | " + t.NombreMoneda + " | " + t.PlazoDias + " días" + " | " + t.Porcentaje + "%",
                        t.IdTasa.ToString()));
            }
            catch { }
        }

        protected void btnGenerarCotizacion_Click(object sender, EventArgs e)
        {
            lblMensajeNuevaCot.Visible = false;
            try
            {
                int idTasa = 0;
                int.TryParse(ddlTasaCot.SelectedValue, out idTasa);
                if (idTasa == 0)
                {
                    MostrarMensaje(lblMensajeNuevaCot, "Debe seleccionar una tasa.", true);
                    return;
                }

                decimal monto = 0;
                if (!decimal.TryParse(txtMontoCot.Text.Trim(), out monto) || monto <= 0)
                {
                    MostrarMensaje(lblMensajeNuevaCot, "El monto debe ser un número mayor a cero.", true);
                    return;
                }

                int idCotizante = GetIdCotizante();
                int idUsuario = GetIdUsuario();
                string usuario = Session["Usuario"].ToString();

                // Obtener id_producto e id_plazo desde la tasa
                Inverix.Controllers.TasaController tCtrl = new Inverix.Controllers.TasaController();
                var tasa = tCtrl.ObtenerTodas().Find(t => t.IdTasa == idTasa);
                if (tasa == null)
                {
                    MostrarMensaje(lblMensajeNuevaCot, "No se encontró la tasa seleccionada.", true);
                    return;
                }

                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                bool exito = cotCtrl.GenerarCotizacion(idUsuario, idCotizante, tasa.IdProducto, tasa.IdPlazo, monto, usuario);

                if (exito)
                {
                    MostrarMensaje(lblMensajeNuevaCot, "¡Cotización generada exitosamente! Puedes verla en 'Mis Cotizaciones'.", false);
                    txtMontoCot.Text = string.Empty;
                    ddlTasaCot.SelectedIndex = 0;
                }
                else
                    MostrarMensaje(lblMensajeNuevaCot, "No se pudo generar la cotización. Intente de nuevo.", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeNuevaCot, "Error: " + ex.Message, true);
            }
        }

        protected void btnLimpiarCot_Click(object sender, EventArgs e)
        {
            txtMontoCot.Text = string.Empty;
            ddlTasaCot.SelectedIndex = 0;
            lblMensajeNuevaCot.Visible = false;
        }

        // ================================================
        //  MIS COTIZACIONES
        // ================================================
        private void CargarMisCotizaciones()
        {
            try
            {
                int idCotizante = GetIdCotizante();
                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                gvMisCotizaciones.DataSource = cotCtrl.ObtenerTodas(null, null, idCotizante);
                gvMisCotizaciones.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeMisCot, "Error al cargar cotizaciones: " + ex.Message, true);
            }
        }

        protected void gvMisCotizaciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalles" || e.CommandName == "EditarCotizacion")
            {
                // Resaltar la fila seleccionada
                GridViewRow row = (GridViewRow)((System.Web.UI.Control)e.CommandSource).NamingContainer;
                gvMisCotizaciones.SelectedIndex = row.RowIndex;
            }

            int idCotizacion = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "VerDetalles")
            {
                // Ocultar las demás filas para una vista limpia (como en el Admin)
                GridViewRow selectedRow = (GridViewRow)((System.Web.UI.Control)e.CommandSource).NamingContainer;
                foreach (GridViewRow row in gvMisCotizaciones.Rows)
                {
                    if (row != selectedRow)
                        row.Visible = false;
                }

                hfIdCotizacionCot.Value = idCotizacion.ToString();
                CargarDetallesMensuales(idCotizacion);
                pnlDetallesCot.Visible = true;
                pnlEditarCotizacion.Visible = false;
            }
            else if (e.CommandName == "EditarCotizacion")
            {
                PrepararEdicion(idCotizacion);
            }
            else if (e.CommandName == "InactivarCotizacion")
            {
                EjecutarInactivacion(idCotizacion);
            }
        }

        private void PrepararEdicion(int idCotizacion)
        {
            try
            {
                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                var cot = cotCtrl.ObtenerCotizacionCompleta(idCotizacion);
                
                // Seguridad: solo puede editar las suyas
                if (cot == null || cot.IdCotizante != GetIdCotizante()) return;

                hfIdCotizacionEditar.Value = idCotizacion.ToString();
                txtMontoEdicion.Text = cot.Monto.ToString("F2");
                CargarListaTasas(ddlTasaEdicion);
                
                // Seleccionar la tasa actual si es posible (basado en producto/plazo)
                // Nota: sp_ObtenerCotizacion no devuelve id_tasa directamente, pero usamos el controlador de tasas para buscar
                Inverix.Controllers.TasaController tCtrl = new Inverix.Controllers.TasaController();
                var tasa = tCtrl.ObtenerTodas().Find(t => t.NombreProducto == cot.NombreProducto && t.PlazoDias.ToString() == cot.DescripcionPlazo);
                if (tasa != null) ddlTasaEdicion.SelectedValue = tasa.IdTasa.ToString();

                pnlEditarCotizacion.Visible = true;
                pnlDetallesCot.Visible = false;
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeMisCot, "Error al preparar edición: " + ex.Message, true);
            }
        }

        private void EjecutarInactivacion(int idCotizacion)
        {
            try
            {
                int idCotizanteLogueado = GetIdCotizante();
                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                
                // Validación de seguridad previa
                var cot = cotCtrl.ObtenerCotizacionCompleta(idCotizacion);
                if (cot == null || cot.IdCotizante != idCotizanteLogueado)
                {
                    MostrarMensaje(lblMensajeMisCot, "No tiene permisos para anular esta cotización.", true);
                    return;
                }

                bool exito = cotCtrl.InactivarCotizacion(idCotizacion, Session["Usuario"].ToString());
                if (exito)
                {
                    MostrarMensaje(lblMensajeMisCot, "Cotización anulada correctamente.", false);
                    CargarMisCotizaciones();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeMisCot, "Error: " + ex.Message, true);
            }
        }

        protected void btnActualizarCotizacion_Click(object sender, EventArgs e)
        {
            try
            {
                int idCotizacion = Convert.ToInt32(hfIdCotizacionEditar.Value);
                int idTasa = int.Parse(ddlTasaEdicion.SelectedValue);
                decimal monto = decimal.Parse(txtMontoEdicion.Text);
                int idCotizante = GetIdCotizante();

                if (idTasa == 0 || monto <= 0)
                {
                    MostrarMensaje(lblMensajeMisCot, "Debe seleccionar una tasa y un monto válido.", true);
                    return;
                }

                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                bool exito = cotCtrl.ActualizarCotizacion(idCotizacion, idCotizante, idTasa, monto, Session["Usuario"].ToString());

                if (exito)
                {
                    MostrarMensaje(lblMensajeMisCot, "¡Cotización actualizada exitosamente!", false);
                    pnlEditarCotizacion.Visible = false;
                    gvMisCotizaciones.SelectedIndex = -1;
                    CargarMisCotizaciones();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeMisCot, "Error al actualizar: " + ex.Message, true);
            }
        }

        protected void btnCancelarEdicion_Click(object sender, EventArgs e)
        {
            pnlEditarCotizacion.Visible = false;
            gvMisCotizaciones.SelectedIndex = -1;
        }

        private void CargarDetallesMensuales(int idCotizacion)
        {
            try
            {
                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                gvDetallesMensualesCot.DataSource = cotCtrl.ObtenerDetallesActivos(idCotizacion);
                gvDetallesMensualesCot.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeMisCot, "Error al cargar detalles: " + ex.Message, true);
            }
        }

        protected void gvDetallesMensualesCot_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Inverix.Classes.DetalleCotizacionClass item = (Inverix.Classes.DetalleCotizacionClass)e.Row.DataItem;
                _totalBruto     += item.InteresBruto;
                _totalImpuesto  += item.Impuesto;
                _totalNeto      += item.InteresNeto;
            }
        }

        protected void gvDetallesMensualesCot_DataBound(object sender, EventArgs e)
        {
            if (gvDetallesMensualesCot.Rows.Count == 0) return;
            GridViewRow footer = gvDetallesMensualesCot.FooterRow;
            if (footer == null) return;

            footer.BackColor = System.Drawing.ColorTranslator.FromHtml("#1693A5");
            footer.ForeColor = System.Drawing.Color.White;
            footer.Font.Bold = true;
            footer.Cells[0].Text = "TOTALES";
            footer.Cells[1].Text = _totalBruto.ToString("N2");
            footer.Cells[2].Text = _totalImpuesto.ToString("N2");
            footer.Cells[3].Text = _totalNeto.ToString("N2");
            footer.Cells[1].HorizontalAlign = HorizontalAlign.Right;
            footer.Cells[2].HorizontalAlign = HorizontalAlign.Right;
            footer.Cells[3].HorizontalAlign = HorizontalAlign.Right;

            _totalBruto = 0; _totalImpuesto = 0; _totalNeto = 0;
        }

        protected void btnCerrarDetalleCot_Click(object sender, EventArgs e)
        {
            pnlDetallesCot.Visible = false;
            gvMisCotizaciones.SelectedIndex = -1;
            
            // Volver a mostrar todas las filas
            foreach (GridViewRow row in gvMisCotizaciones.Rows)
            {
                row.Visible = true;
            }
        }

        // ================================================
        //  REPORTES
        // ================================================
        private void CargarReporte(DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                int idCotizante = GetIdCotizante();
                Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
                gvReporte.DataSource = cotCtrl.ObtenerParaReporte(idCotizante, desde, hasta);
                gvReporte.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeReporte, "Error al cargar reporte: " + ex.Message, true);
            }
        }

        protected void btnFiltrarReporte_Click(object sender, EventArgs e)
        {
            DateTime? desde = null, hasta = null;
            DateTime tmp;
            if (DateTime.TryParse(txtFechaDesde.Text, out tmp)) desde = tmp;
            if (DateTime.TryParse(txtFechaHasta.Text, out tmp)) hasta = tmp;
            CargarReporte(desde, hasta);
        }

        protected void gvReporte_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalles")
            {
                int idCotizacion = Convert.ToInt32(e.CommandArgument);
                hfIdCotizacionReporteCot.Value = idCotizacion.ToString();
                pnlDetallesReporteCot.Visible = true;

                // Cargar detalles mensuales
                Inverix.Controllers.CotizacionController cCtrl = new Inverix.Controllers.CotizacionController();
                var detalles = cCtrl.ObtenerDetallesActivos(idCotizacion);

                _totalBrutoReporte = 0;
                _totalImpuestoReporte = 0;
                _totalNetoReporte = 0;

                gvDetallesReporteCot.DataSource = detalles;
                gvDetallesReporteCot.DataBind();

                // Resaltar fila seleccionada y ocultar las demás
                GridViewRow row = (GridViewRow)((System.Web.UI.Control)e.CommandSource).NamingContainer;
                gvReporte.SelectedIndex = row.RowIndex;

                foreach (GridViewRow r in gvReporte.Rows)
                {
                    r.Visible = (r.RowIndex == row.RowIndex);
                }
            }
        }

        protected void btnCerrarDetallesReporteCot_Click(object sender, EventArgs e)
        {
            pnlDetallesReporteCot.Visible = false;
            gvReporte.SelectedIndex = -1;

            // Volver a mostrar todas las filas
            foreach (GridViewRow r in gvReporte.Rows)
            {
                r.Visible = true;
            }
        }

        protected void gvDetallesReporteCot_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DetalleCotizacionClass item = (DetalleCotizacionClass)e.Row.DataItem;
                _totalBrutoReporte += item.InteresBruto;
                _totalImpuestoReporte += item.Impuesto;
                _totalNetoReporte += item.InteresNeto;
            }
        }

        protected void gvDetallesReporteCot_DataBound(object sender, EventArgs e)
        {
            if (gvDetallesReporteCot.Rows.Count == 0) return;
            GridViewRow footer = gvDetallesReporteCot.FooterRow;
            footer.Cells[0].Text = "TOTALES";
            footer.Cells[0].Font.Bold = true;
            footer.Cells[1].Text = _totalBrutoReporte.ToString("N2");
            footer.Cells[1].Font.Bold = true;
            footer.Cells[2].Text = _totalImpuestoReporte.ToString("N2");
            footer.Cells[2].Font.Bold = true;
            footer.Cells[3].Text = _totalNetoReporte.ToString("N2");
            footer.Cells[3].Font.Bold = true;
        }

        protected void btnVerTodoReporte_Click(object sender, EventArgs e)
        {
            txtFechaDesde.Text = string.Empty;
            txtFechaHasta.Text = string.Empty;
            CargarReporte();
        }

        private DataTable ObtenerDatosReporte()
        {
            DateTime? desde = null, hasta = null;
            DateTime tmp;
            if (DateTime.TryParse(txtFechaDesde.Text, out tmp)) desde = tmp;
            if (DateTime.TryParse(txtFechaHasta.Text, out tmp)) hasta = tmp;

            int idCotizante = GetIdCotizante();
            Inverix.Controllers.CotizacionController cotCtrl = new Inverix.Controllers.CotizacionController();
            return cotCtrl.ObtenerParaReporte(idCotizante, desde, hasta);
        }

        protected void btnReporteExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ObtenerDatosReporte();
                if (dt.Rows.Count == 0)
                {
                    MostrarMensaje(lblMensajeReporte, "No hay datos para exportar.", true);
                    return;
                }

                // Verificar si hay una cotización con detalle abierto
                if (gvReporte.SelectedIndex != -1)
                {
                    int rowIndex = gvReporte.SelectedIndex;
                    int idCotizacionSeleccionada = Convert.ToInt32(gvReporte.DataKeys[rowIndex].Value);

                    DataTable dtFiltrado = dt.Clone();
                    DataRow[] filas = dt.Select("IdCotizacion = " + idCotizacionSeleccionada);
                    if (filas.Length > 0) dtFiltrado.ImportRow(filas[0]);

                    Inverix.Controllers.CotizacionController cCtrl = new Inverix.Controllers.CotizacionController();
                    var detalles = cCtrl.ObtenerDetallesActivos(idCotizacionSeleccionada);

                    ReporteClass.GenerarExcelConDetalles(dtFiltrado, detalles, "Reporte de Cotizaciones", Response);
                }
                else
                {
                    ReporteClass.GenerarExcel(dt, "Reporte de Cotizaciones", Response);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeReporte, "Error al generar Excel: " + ex.Message, true);
            }
        }

        protected void btnReportePDF_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ObtenerDatosReporte();
                if (dt.Rows.Count == 0)
                {
                    MostrarMensaje(lblMensajeReporte, "No hay datos para exportar.", true);
                    return;
                }
                string nombre = Session["NombreCompletoCot"] != null ? Session["NombreCompletoCot"].ToString() : "Cotizante";

                // Verificar si hay una cotización con detalle abierto
                if (gvReporte.SelectedIndex != -1)
                {
                    int rowIndex = gvReporte.SelectedIndex;
                    int idCotizacionSeleccionada = Convert.ToInt32(gvReporte.DataKeys[rowIndex].Value);

                    DataTable dtFiltrado = dt.Clone();
                    DataRow[] filas = dt.Select("IdCotizacion = " + idCotizacionSeleccionada);
                    if (filas.Length > 0) dtFiltrado.ImportRow(filas[0]);

                    Inverix.Controllers.CotizacionController cCtrl = new Inverix.Controllers.CotizacionController();
                    var detalles = cCtrl.ObtenerDetallesActivos(idCotizacionSeleccionada);

                    ReporteClass.GenerarPDFConDetalles(dtFiltrado, detalles, "Reporte Historial de Cotizaciones", nombre, Response);
                }
                else
                {
                    ReporteClass.GenerarPDF(dt, "Reporte Historial de Cotizaciones", nombre, Response);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajeReporte, "Error al generar PDF: " + ex.Message, true);
            }
        }

        public override void VerifyRenderingInServerForm(System.Web.UI.Control control) { }

        // ================================================
        //  MI PERFIL — CAMBIO DE CONTRASEÑA
        // ================================================
        protected void btnCambiarClave_Click(object sender, EventArgs e)
        {
            lblMensajePerfil.Visible = false;
            try
            {
                string claveActual = txtClaveActual.Text.Trim();
                string claveNueva = txtClaveNueva.Text.Trim();
                string claveConfirmar = txtClaveConfirmar.Text.Trim();

                if (string.IsNullOrEmpty(claveActual) || string.IsNullOrEmpty(claveNueva) || string.IsNullOrEmpty(claveConfirmar))
                {
                    MostrarMensaje(lblMensajePerfil, "Todos los campos de contraseña son obligatorios.", true);
                    return;
                }

                if (claveNueva.Length < 8)
                {
                    MostrarMensaje(lblMensajePerfil, "La contraseña debe tener al menos 8 caracteres.", true);
                    return;
                }

                int letrasPerfil = 0;
                foreach (char c in claveNueva) if (char.IsLetter(c)) letrasPerfil++;
                if (letrasPerfil < 4)
                {
                    MostrarMensaje(lblMensajePerfil, "La contraseña debe contener al menos 4 letras.", true);
                    return;
                }

                bool mayusculaPerfil = false;
                foreach (char c in claveNueva) if (char.IsUpper(c)) { mayusculaPerfil = true; break; }
                if (!mayusculaPerfil)
                {
                    MostrarMensaje(lblMensajePerfil, "La contraseña debe contener al menos una letra mayúscula.", true);
                    return;
                }

                if (claveNueva != claveConfirmar)
                {
                    MostrarMensaje(lblMensajePerfil, "La nueva contraseña y su confirmación no coinciden.", true);
                    return;
                }

                int idUsuario = GetIdUsuario();
                Inverix.Controllers.UsuarioController uCtrl = new Inverix.Controllers.UsuarioController();
                string resultado = uCtrl.ActualizarClave(idUsuario, claveActual, claveNueva);

                if (resultado == "OK")
                {
                    MostrarMensaje(lblMensajePerfil, "¡Contraseña actualizada correctamente!", false);
                    txtClaveActual.Text = string.Empty;
                    txtClaveNueva.Text = string.Empty;
                    txtClaveConfirmar.Text = string.Empty;
                }
                else
                    MostrarMensaje(lblMensajePerfil, resultado, true);
            }
            catch (Exception ex)
            {
                MostrarMensaje(lblMensajePerfil, "Error: " + ex.Message, true);
            }
        }
    }
}