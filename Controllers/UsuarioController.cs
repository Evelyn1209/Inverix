using System;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;
namespace Inverix.Controllers
{
    public class UsuarioController
    {
        private Conexion conexionDB;
        public UsuarioController()
        {
            conexionDB = new Conexion();
        }
        // ==========================================
        //  Validar Login
        // ==========================================
        public UsuarioClass ValidarLogin(string usuario, string clave)
        {
            UsuarioClass usuarioLogueado = null;
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_LoginUsuario"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        usuarioLogueado = new UsuarioClass
                        {
                            IdUsuario = Convert.ToInt32(row["id_usuario"]),
                            NombreUsuario = row["usuario"].ToString(),
                            Rol = row["rol"] != DBNull.Value ? row["rol"].ToString() : "Sin Rol Asignado",
                            RolCodigo = row["rolCodigo"] != DBNull.Value ? row["rolCodigo"].ToString() : "",
                            IdCotizante = row["id_cotizante"] != DBNull.Value ? (int?)Convert.ToInt32(row["id_cotizante"]) : null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar iniciar sesion: " + ex.Message);
            }
            return usuarioLogueado;
        }
        // ==========================================
        //  Registro Cliente / Cotizante
        // ==========================================
        public bool RegistrarCotizanteWeb(string usuario, string clave, int idTipoId, string identificacion, string nombre, string apellido1, string apellido2, string correo, string telefono)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_RegistrarCotizanteWeb"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@id_tipo_identificacion", idTipoId);
                    cmd.Parameters.AddWithValue("@identificacion", identificacion);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@primer_apellido", apellido1);
                    cmd.Parameters.AddWithValue("@segundo_apellido", apellido2);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en registro: " + ex.Message);
            }
        }

        // ==========================================
        //  CRUD de Usuarios
        // ==========================================
        public System.Collections.Generic.List<UsuarioClass> ObtenerTodos()
        {
            System.Collections.Generic.List<UsuarioClass> lista = new System.Collections.Generic.List<UsuarioClass>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarUsuarios"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            UsuarioClass user = new UsuarioClass
                            {
                                IdUsuario = Convert.ToInt32(row["id_usuario"]),
                                NombreUsuario = row["usuario"].ToString(),
                                Estado = Convert.ToBoolean(row["estado"]),
                                Rol = row["rol"] != DBNull.Value ? row["rol"].ToString() : "Sin Rol Asignado"
                            };

                            // Check if id_role exists (it should via the modified sp_ListarUsuarios)
                            if (dt.Columns.Contains("id_role") && row["id_role"] != DBNull.Value)
                            {
                                // We map id_role into RolCodigo temporarily if needed, or better, we can read it.
                                // RolCodigo is string, id_role is int. We can stringify it to store id temporarily.
                                user.RolCodigo = row["id_role"].ToString();
                            }

                            lista.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuarios: " + ex.Message);
            }
            return lista;
        }

        public bool Crear(string usuario, string clave, int idRole, string adminLogueado)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearUsuario"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@usuario_creacion", adminLogueado);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int idUsuarioNuevo = Convert.ToInt32(dt.Rows[0]["id_usuario"]);
                        ActualizarRol(idUsuarioNuevo, idRole, adminLogueado);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear usuario: " + ex.Message);
            }
        }

        public bool Actualizar(int idUsuario, string usuario, int idRole, string claveNueva, string adminLogueado)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    
                    if (string.IsNullOrEmpty(claveNueva))
                        cmd.Parameters.AddWithValue("@clave_nueva", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@clave_nueva", claveNueva);
                        
                    cmd.Parameters.AddWithValue("@usuario_modificacion", adminLogueado);

                    conexionDB.EjecutarInstruccion(cmd);
                }

                // Update Role
                ActualizarRol(idUsuario, idRole, adminLogueado);
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar usuario: " + ex.Message);
            }
        }

        public bool Inactivar(int idUsuario, string adminLogueado)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarUsuario"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", adminLogueado);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar usuario: " + ex.Message);
            }
        }

        private void ActualizarRol(int idUsuario, int idRole, string adminLogueado)
        {
            using (SqlCommand cmd = new SqlCommand("sp_ActualizarRolUsuario"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@id_role", idRole);
                cmd.Parameters.AddWithValue("@usuario_modificacion", adminLogueado);
                
                conexionDB.EjecutarInstruccion(cmd);
            }
        }

        // ==========================================
        //  Actualizar Contraseña (Cotizante)
        // ==========================================
        public string ActualizarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarClave"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@clave_actual", claveActual);
                    cmd.Parameters.AddWithValue("@clave_nueva", claveNueva);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int resultado = Convert.ToInt32(dt.Rows[0]["resultado"]);
                        string mensaje = dt.Rows[0]["mensaje"].ToString();
                        return resultado == 1 ? "OK" : mensaje;
                    }
                    return "No se pudo procesar la solicitud.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar contraseña: " + ex.Message);
            }
        }
    }
}