using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class CotizacionController
    {
        private Conexion conexionDB;

        public CotizacionController()
        {
            conexionDB = new Conexion();
        }

        public List<CotizacionClass> ObtenerTodas(int? idCotizanteFiltro = null)
        {
            List<CotizacionClass> lista = new List<CotizacionClass>();

            try
            {
                string sql = @"
                    SELECT v.id_cotizacion, v.numero_cotizacion, v.producto, v.cotizante, v.monto, v.montoTotalNeto, v.fechaCotizacion 
                    FROM vw_HistorialCotizaciones v
                    INNER JOIN Cotizaciones c ON v.id_cotizacion = c.id_cotizacion
                    WHERE c.estado = 1
                ";

                if (idCotizanteFiltro.HasValue && idCotizanteFiltro.Value > 0)
                    sql += " AND c.id_cotizante = @id_cotizante";

                sql += " ORDER BY v.fechaCotizacion DESC";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;

                    if (idCotizanteFiltro.HasValue && idCotizanteFiltro.Value > 0)
                        cmd.Parameters.AddWithValue("@id_cotizante", idCotizanteFiltro.Value);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        lista.Add(new CotizacionClass
                        {
                            IdCotizacion = Convert.ToInt32(row["id_cotizacion"]),
                            NumeroCotizacion = row["numero_cotizacion"].ToString(),
                            NombreProducto = row["producto"].ToString(),
                            NombreCotizante = row["cotizante"].ToString(),
                            Monto = Convert.ToDecimal(row["monto"]),
                            MontoTotalNeto = Convert.ToDecimal(row["montoTotalNeto"]),
                            FechaCotizacion = Convert.ToDateTime(row["fechaCotizacion"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar cotizaciones: " + ex.Message);
            }

            return lista;
        }

        //  CAMBIO: Cotizacion → CotizacionClass
        public CotizacionClass ObtenerCotizacionCompleta(int idCotizacion)
        {
            //  CAMBIO: Cotizacion → CotizacionClass
            CotizacionClass cot = null;

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerCotizacion"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cotizacion", idCotizacion);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        //  CAMBIO: Cotizacion → CotizacionClass
                        cot = new CotizacionClass
                        {
                            IdCotizacion = Convert.ToInt32(row["id_cotizacion"]),
                            NumeroCotizacion = row["numero_cotizacion"].ToString(),
                            NombreProducto = row["producto"].ToString(),
                            DescripcionPlazo = row["plazo_dias"].ToString(),
                            FechaCotizacion = Convert.ToDateTime(row["fechaCotizacion"]),
                            FechaInicio = Convert.ToDateTime(row["fechaInicio"]),
                            FechaFinal = Convert.ToDateTime(row["fechaFinal"]),
                            Monto = Convert.ToDecimal(row["monto"]),
                            TasaAplicada = Convert.ToDecimal(row["tasaAplicada"]),
                            PorcentajeImpuesto = Convert.ToDecimal(row["porcentajeImpuesto"]),
                            MontoTotalInteresBruto = Convert.ToDecimal(row["montoTotalInteresBruto"]),
                            MontoTotalImpuesto = Convert.ToDecimal(row["montoTotalImpuesto"]),
                            MontoTotalNeto = Convert.ToDecimal(row["montoTotalNeto"])
                        };

                        cot.Detalles = ObtenerDetallesActivos(idCotizacion);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la cotización: " + ex.Message);
            }

            return cot;
        }

        // CAMBIO: Cotizacion → CotizacionClass
        public CotizacionClass ObtenerDetallePorId(int idCotizacion)
        {
            try
            {
                string sql = @"
                    SELECT id_cotizacion, id_cotizante, idTasa, monto
                    FROM Cotizaciones
                    WHERE id_cotizacion = @id AND estado = 1";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", idCotizacion);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // CAMBIO: Cotizacion → CotizacionClass
                        return new CotizacionClass
                        {
                            IdCotizacion = Convert.ToInt32(row["id_cotizacion"]),
                            IdCotizante = Convert.ToInt32(row["id_cotizante"]),
                            IdTasa = Convert.ToInt32(row["idTasa"]),
                            Monto = Convert.ToDecimal(row["monto"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener datos de cotización: " + ex.Message);
            }

            return null;
        }

        public List<DetalleCotizacionClass> ObtenerDetallesActivos(int idCotizacion)
        {
            List<DetalleCotizacionClass> detalles = new List<DetalleCotizacionClass>();

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetalleCotizacion"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cotizacion", idCotizacion);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        detalles.Add(new DetalleCotizacionClass
                        {
                            IdCotizacion = idCotizacion,
                            NumeroMes = Convert.ToInt32(row["Mes"]),
                            Monto = Convert.ToDecimal(row["Monto"]),
                            InteresBruto = Convert.ToDecimal(row["InteresBruto"]),
                            Impuesto = Convert.ToDecimal(row["Impuesto"]),
                            InteresNeto = Convert.ToDecimal(row["InteresNeto"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la cotización: " + ex.Message);
            }

            return detalles;
        }

        public bool GenerarCotizacion(int idUsuario, int idCotizante, int idProducto, int idPlazo, decimal monto, string usuarioEjecuta)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_GenerarCotizacion"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@id_cotizante", idCotizante);
                    cmd.Parameters.AddWithValue("@id_producto", idProducto);
                    cmd.Parameters.AddWithValue("@id_plazo", idPlazo);
                    cmd.Parameters.AddWithValue("@monto", monto);
                    cmd.Parameters.AddWithValue("@usuario_ejecuta", usuarioEjecuta);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);
                    return dt != null && dt.Rows.Count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar la cotización: " + ex.Message);
            }
        }

        public bool ActualizarCotizacion(int idCotizacion, int idCotizante, int idTasa, decimal monto, string usuarioModifica)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarCotizacion"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cotizacion", idCotizacion);
                    cmd.Parameters.AddWithValue("@id_cotizante", idCotizante);
                    cmd.Parameters.AddWithValue("@id_tasa", idTasa);
                    cmd.Parameters.AddWithValue("@monto", monto);
                    cmd.Parameters.AddWithValue("@usuario_modifica", usuarioModifica);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);
                    return dt != null && dt.Rows.Count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la cotización: " + ex.Message);
            }
        }

        public bool InactivarCotizacion(int idCotizacion, string usuarioModificacion)
        {
            try
            {
                string sql = @"
                    UPDATE Cotizaciones 
                    SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() 
                    WHERE id_cotizacion = @id_cotizacion;

                    UPDATE Detalle_Cotizacion 
                    SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() 
                    WHERE id_cotizacion = @id_cotizacion;
                ";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_cotizacion", idCotizacion);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    return conexionDB.EjecutarInstruccion(cmd) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar la cotización: " + ex.Message);
            }
        }

        public bool InactivarDetalleCotizacion(int idCotizacion, int numeroMes, string usuarioModificacion)
        {
            try
            {
                string query = @"
                    UPDATE Detalle_Cotizacion 
                    SET estado = 0, 
                        usuario_modificacion = @usuario, 
                        fecha_modificacion = GETDATE() 
                    WHERE id_cotizacion = @idCotizacion 
                      AND numero_mes = @numeroMes";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@idCotizacion", idCotizacion);
                    cmd.Parameters.AddWithValue("@numeroMes", numeroMes);
                    cmd.Parameters.AddWithValue("@usuario", usuarioModificacion);

                    return conexionDB.EjecutarInstruccion(cmd) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar el detalle de la cotización: " + ex.Message);
            }
        }
    }
}