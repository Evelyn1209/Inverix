using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class CotizanteController
    {
        private Conexion conexionDB;

        public CotizanteController()
        {
            conexionDB = new Conexion();
        }

        public List<CotizanteClass> ObtenerTodos()
        {
            List<CotizanteClass> lista = new List<CotizanteClass>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarCotizantes"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        lista.Add(new CotizanteClass
                        {
                            IdCotizante = Convert.ToInt32(row["IdCotizante"]),
                            IdTipoIdentificacion = Convert.ToInt32(row["IdTipoIdentificacion"]),
                            TipoIdentificacionNombre = row["TipoIdentificacionNombre"].ToString(),
                            Identificacion = row["Identificacion"].ToString(),
                            Nombre = row["Nombre"].ToString(),
                            PrimerApellido = row["PrimerApellido"].ToString(),
                            SegundoApellido = row["SegundoApellido"].ToString(),
                            Correo = row["Correo"].ToString(),
                            Telefono = row["Telefono"].ToString(),
                            Estado = Convert.ToBoolean(row["Estado"]),
                            UsuarioCreacion = row["UsuarioCreacion"].ToString(),
                            FechaCreacion = row["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(row["FechaCreacion"]) : (DateTime?)null,
                            UsuarioModificacion = row["UsuarioModificacion"] != DBNull.Value ? row["UsuarioModificacion"].ToString() : string.Empty,
                            FechaModificacion = row["FechaModificacion"] != DBNull.Value ? Convert.ToDateTime(row["FechaModificacion"]) : (DateTime?)null, IdUsuario = row["IdUsuario"] != DBNull.Value ? Convert.ToInt32(row["IdUsuario"]) : (int?)null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener cotizantes: " + ex.Message);
            }
            return lista;
        }

        // ?? CAMBIO AQUÍ: Cotizante ? CotizanteClass
        public bool Crear(CotizanteClass cotizante, string usuarioCreacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearCotizante"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_tipo_identificacion", cotizante.IdTipoIdentificacion);
                    cmd.Parameters.AddWithValue("@identificacion", cotizante.Identificacion);
                    cmd.Parameters.AddWithValue("@nombre", cotizante.Nombre);
                    cmd.Parameters.AddWithValue("@primer_apellido", cotizante.PrimerApellido);
                    cmd.Parameters.AddWithValue("@segundo_apellido", cotizante.SegundoApellido);
                    cmd.Parameters.AddWithValue("@correo", cotizante.Correo);
                    cmd.Parameters.AddWithValue("@telefono", cotizante.Telefono);
                    cmd.Parameters.AddWithValue("@usuario_creacion", usuarioCreacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ?? CAMBIO AQUÍ: Cotizante ? CotizanteClass
        public bool Actualizar(CotizanteClass cotizante, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarCotizante"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cotizante", cotizante.IdCotizante);
                    cmd.Parameters.AddWithValue("@id_tipo_identificacion", cotizante.IdTipoIdentificacion);
                    cmd.Parameters.AddWithValue("@identificacion", cotizante.Identificacion);
                    cmd.Parameters.AddWithValue("@nombre", cotizante.Nombre);
                    cmd.Parameters.AddWithValue("@primer_apellido", cotizante.PrimerApellido);
                    cmd.Parameters.AddWithValue("@segundo_apellido", cotizante.SegundoApellido);
                    cmd.Parameters.AddWithValue("@correo", cotizante.Correo);
                    cmd.Parameters.AddWithValue("@telefono", cotizante.Telefono);
                    cmd.Parameters.AddWithValue("@estado", cotizante.Estado);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar cotizante: " + ex.Message);
            }
        }

        public bool Inactivar(int idCotizante, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarCotizante"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cotizante", idCotizante);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar cotizante: " + ex.Message);
            }
        }

        public DataTable ListarTiposIdentificacion()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarTiposIdentificacion"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    return conexionDB.EjecutarConsulta(cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar tipos de identificación: " + ex.Message);
            }
        }

        public bool GenerarAccesoCotizante(int idCotizante, string usuario, string clave, string adminLogueado)
        {
            try
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("sp_GenerarAccesoCotizante"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cotizante", idCotizante);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@usuario_creador", adminLogueado);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar acceso: " + ex.Message);
            }
        }
    }
}
