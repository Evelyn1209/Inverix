using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class MonedaController
    {
        private Conexion conexionDB;

        public MonedaController()
        {
            conexionDB = new Conexion();
        }

        public List<MonedaClass> ObtenerTodas()
        {
            List<MonedaClass> lista = new List<MonedaClass>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarMonedas"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        lista.Add(new MonedaClass
                        {
                            IdMoneda = Convert.ToInt32(row["id_moneda"]),
                            Codigo = row["codigo"].ToString(),
                            Nombre = row["nombre"].ToString(),
                            Simbolo = row["simbolo"].ToString(),
                            Estado = Convert.ToBoolean(row["estado"]),
                            UsuarioCreacion = row["usuario_creacion"] != DBNull.Value ? row["usuario_creacion"].ToString() : string.Empty,
                            FechaCreacion = row["fecha_creacion"] != DBNull.Value ? Convert.ToDateTime(row["fecha_creacion"]) : (DateTime?)null,
                            UsuarioModificacion = row["usuario_modificacion"] != DBNull.Value ? row["usuario_modificacion"].ToString() : string.Empty,
                            FechaModificacion = row["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(row["fecha_modificacion"]) : (DateTime?)null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar monedas: " + ex.Message);
            }
            return lista;
        }

        // 🔥 CAMBIO AQUÍ: Moneda → MonedaClass
        public bool Crear(MonedaClass moneda, string usuarioCreacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearMoneda"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codigo", moneda.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", moneda.Nombre);
                    cmd.Parameters.AddWithValue("@simbolo", moneda.Simbolo);
                    cmd.Parameters.AddWithValue("@usuario_creacion", usuarioCreacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la moneda: " + ex.Message);
            }
        }

        // 🔥 CAMBIO AQUÍ: Moneda → MonedaClass
        public bool Actualizar(MonedaClass moneda, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarMoneda"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_moneda", moneda.IdMoneda);
                    cmd.Parameters.AddWithValue("@codigo", moneda.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", moneda.Nombre);
                    cmd.Parameters.AddWithValue("@simbolo", moneda.Simbolo);
                    cmd.Parameters.AddWithValue("@estado", moneda.Estado);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la moneda: " + ex.Message);
            }
        }

        public bool Inactivar(int idMoneda, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarMoneda"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_moneda", idMoneda);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    return conexionDB.EjecutarInstruccion(cmd) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar la moneda (eliminación lógica): " + ex.Message);
            }
        }
    }
}
