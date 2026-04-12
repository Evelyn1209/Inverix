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

        /// <summary>
        /// Valida las credenciales de un usuario contra la base de datos de forma segura.
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="clave">Contraseña en texto plano (se hashea en la BD)</param>
        /// <returns>Objeto UsuarioClass si el login es correcto, nulo si las credenciales son inválidas.</returns>

        //  UsuarioClass
        public UsuarioClass ValidarLogin(string usuario, string clave)
        {
            // UsuarioClass
            UsuarioClass usuarioLogueado = null;

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_LoginUsuario"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros seguros para evitar Inyección SQL
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);

                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // UsuarioClass
                        usuarioLogueado = new UsuarioClass
                        {
                            IdUsuario = Convert.ToInt32(row["id_usuario"]),
                            NombreUsuario = row["usuario"].ToString(),
                            Rol = row["rol"] != DBNull.Value ? row["rol"].ToString() : "Sin Rol Asignado"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar iniciar sesión: " + ex.Message);
            }

            return usuarioLogueado;
        }
    }
}