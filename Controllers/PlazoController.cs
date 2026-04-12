using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class PlazoController
    {
        private Conexion conexionDB;

        public PlazoController()
        {
            conexionDB = new Conexion();
        }

        //  CAMBIO: Se usa PlazoClass en lugar de Plazo
        public List<PlazoClass> ObtenerTodas()
        {
            List<PlazoClass> lista = new List<PlazoClass>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarPlazos"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        //  CAMBIO: Instanciamos PlazoClass
                        lista.Add(new PlazoClass
                        {
                            IdPlazo = Convert.ToInt32(row["id_plazo"]),
                            Dias = Convert.ToInt32(row["dias"]),
                            Descripcion = row["descripcion"].ToString(),
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
                throw new Exception("Error al listar plazos: " + ex.Message);
            }
            return lista;
        }

        // CAMBIO: Parámetro ahora es PlazoClass
        public bool Crear(PlazoClass plazo, string usuarioCreacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearPlazo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dias", plazo.Dias);
                    cmd.Parameters.AddWithValue("@descripcion", plazo.Descripcion);
                    cmd.Parameters.AddWithValue("@usuario_creacion", usuarioCreacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el plazo: " + ex.Message);
            }
        }

        //  CAMBIO: Parámetro ahora es PlazoClass
        public bool Actualizar(PlazoClass plazo, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarPlazo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_plazo", plazo.IdPlazo);
                    cmd.Parameters.AddWithValue("@dias", plazo.Dias);
                    cmd.Parameters.AddWithValue("@descripcion", plazo.Descripcion);
                    cmd.Parameters.AddWithValue("@estado", plazo.Estado);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el plazo: " + ex.Message);
            }
        }

        public bool Inactivar(int idPlazo, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarPlazo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_plazo", idPlazo);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar el plazo: " + ex.Message);
            }
        }
    }
}