using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class RoleController
    {
        private Conexion conexionDB;

        public RoleController()
        {
            conexionDB = new Conexion();
        }

        public List<RoleClass> ObtenerTodos()
        {
            List<RoleClass> lista = new List<RoleClass>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarRoles"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            lista.Add(new RoleClass
                            {
                                IdRole = Convert.ToInt32(row["id_role"]),
                                Codigo = row["codigo"].ToString(),
                                Nombre = row["nombre"].ToString(),
                                Estado = Convert.ToBoolean(row["estado"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener roles: " + ex.Message);
            }
            return lista;
        }
    }
}
