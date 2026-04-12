using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Inverix.Classes
{
    public class Conexion
    {
        private string CadenaConexion = ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString;

        /// <summary>
        /// Obtiene una nueva instancia de la conexión a la base de datos y la abre.
        /// </summary>
        /// <returns>SqlConnection abierta.</returns>
        public SqlConnection ObtenerConexion()
        {
            try
            {
                SqlConnection conexion = new SqlConnection(CadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (Exception ex)
            {
                // En un escenario real aquí iría un log de errores
                throw new Exception("Error al conectar con la base de datos: " + ex.Message);
            }
        }

        /// <summary>
        /// Ejecuta una consulta SELECT y devuelve un DataTable.
        /// </summary>
        /// <param name="comando">SqlCommand con la consulta (Stored Procedure o texto).</param>
        /// <returns>DataTable con los resultados.</returns>
        public DataTable EjecutarConsulta(SqlCommand comando)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = ObtenerConexion())
            {
                comando.Connection = conexion;
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    adaptador.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Ejecuta comandos INSERT, UPDATE, DELETE y devuelve el número de filas afectadas.
        /// </summary>
        /// <param name="comando">SqlCommand con la instrucción.</param>
        /// <returns>Número de filas afectadas.</returns>
        public int EjecutarInstruccion(SqlCommand comando)
        {
            int filasAfectadas = 0;
            using (SqlConnection conexion = ObtenerConexion())
            {
                comando.Connection = conexion;
                filasAfectadas = comando.ExecuteNonQuery();
            }
            return filasAfectadas;
        }

        /// <summary>
        /// Ejecuta comandos INSERT y retorna el ID insertado (usando SCOPE_IDENTITY, si aplica).
        /// </summary>
        /// <param name="comando">SqlCommand con la instrucción.</param>
        /// <returns>El resultado del ExecuteScalar casteado a int, o 0 si no aplica.</returns>
        public int EjecutarInstruccionEscalar(SqlCommand comando)
        {
            object resultado = null;
            using (SqlConnection conexion = ObtenerConexion())
            {
                comando.Connection = conexion;
                resultado = comando.ExecuteScalar();
            }

            if (resultado != null && int.TryParse(resultado.ToString(), out int id))
            {
                return id;
            }
            return 0;
        }
    }
}
