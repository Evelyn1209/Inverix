using Inverix.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inverix.Controllers
{
    public class AuditoriaController
    {
        public List<AuditoriaClass> ObtenerHistorialCompleto()
        {
            List<AuditoriaClass> lista = new List<AuditoriaClass>();
            try
            {
                Conexion conexionDB = new Conexion();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Auditoria_Completa ORDER BY FechaHoraDeActualizacion DESC"))
                {
                    cmd.CommandType = CommandType.Text;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        lista.Add(new AuditoriaClass
                        {
                            IdAuditoria = Convert.ToInt32(row["IdAuditoria"]),
                            SeccionAfectada = row["SeccionAfectada"].ToString(),
                            CuentaAfectada = row["CuentaAfectada"].ToString(),
                            AccionRealizada = row["AccionRealizada"].ToString(),
                            DetalleDelCambio = row["DetalleDelCambio"] != DBNull.Value ? row["DetalleDelCambio"].ToString() : "",
                            CreadoOriginalmentePor = row["CreadoOriginalmentePor"].ToString(),
                            FechaHoraDeCreacion = Convert.ToDateTime(row["FechaHoraDeCreacion"]),
                            ActualizadoPor = row["ActualizadoPor"].ToString(),
                            FechaHoraDeActualizacion = Convert.ToDateTime(row["FechaHoraDeActualizacion"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener auditoría: " + ex.Message);
            }
            return lista;
        }
    }
}
