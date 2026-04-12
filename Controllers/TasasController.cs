using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class TasaController
    {
        private Conexion conexionDB;

        public TasaController()
        {
            conexionDB = new Conexion();
        }

        // TasaClass
        public List<TasaClass> ObtenerTodas()
        {
            List<TasaClass> lista = new List<TasaClass>();

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarTasas"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        //  TasaClass
                        TasaClass t = new TasaClass
                        {
                            IdTasa = dt.Columns.Contains("id_tasa") ? Convert.ToInt32(row["id_tasa"]) : 0,
                            IdProducto = dt.Columns.Contains("id_producto") ? Convert.ToInt32(row["id_producto"]) : 0,
                            NombreProducto = dt.Columns.Contains("nombre_producto") ? row["nombre_producto"].ToString() : string.Empty,
                            IdPlazo = dt.Columns.Contains("id_plazo") ? Convert.ToInt32(row["id_plazo"]) : 0,
                            PlazoDias = dt.Columns.Contains("dias") ? Convert.ToInt32(row["dias"]) : 0,
                            Porcentaje = dt.Columns.Contains("porcentaje") ? Convert.ToDecimal(row["porcentaje"]) : 0,
                            Estado = dt.Columns.Contains("estado") ? Convert.ToBoolean(row["estado"]) : false,

                            // Auditoría
                            UsuarioCreacion = dt.Columns.Contains("usuario_creacion") && row["usuario_creacion"] != DBNull.Value ? row["usuario_creacion"].ToString() : string.Empty,
                            FechaCreacion = dt.Columns.Contains("fecha_creacion") && row["fecha_creacion"] != DBNull.Value ? Convert.ToDateTime(row["fecha_creacion"]) : (DateTime?)null
                        };
                        lista.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de tasas: " + ex.Message);
            }

            return lista;
        }

        //  CAMBIO: antes era (Tasa tasa)
        // AHORA: usamos TasaClass
        public bool Crear(TasaClass tasa, string usuarioCreacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearTasa"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", tasa.IdProducto);
                    cmd.Parameters.AddWithValue("@id_plazo", tasa.IdPlazo);
                    cmd.Parameters.AddWithValue("@porcentaje", tasa.Porcentaje);
                    cmd.Parameters.AddWithValue("@usuario_creacion", usuarioCreacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la tasa: " + ex.Message);
            }
        }

        
        //  usamos TasaClass
        public bool Actualizar(TasaClass tasa, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarTasa"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_tasa", tasa.IdTasa);
                    cmd.Parameters.AddWithValue("@porcentaje", tasa.Porcentaje);
                    cmd.Parameters.AddWithValue("@estado", tasa.Estado);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la tasa: " + ex.Message);
            }
        }

        public bool Inactivar(int idTasa, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarTasa"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_tasa", idTasa);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar la tasa: " + ex.Message);
            }
        }
    }
}