using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Inverix.Classes;

namespace Inverix.Controllers
{
    public class ProductoController
    {
        private Conexion conexionDB;

        public ProductoController()
        {
            conexionDB = new Conexion();
        }

        // CAMBIO: Producto → ProductoClass
        public List<ProductoClass> ObtenerTodos()
        {
            // CAMBIO: Producto → ProductoClass
            List<ProductoClass> lista = new List<ProductoClass>();

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarProductos"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = conexionDB.EjecutarConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {
                        // CAMBIO: Producto → ProductoClass
                        ProductoClass p = new ProductoClass
                        {
                            IdProducto = Convert.ToInt32(row["id_producto"]),
                            Codigo = row["codigo"].ToString(),
                            Nombre = row["nombre"].ToString(),
                            IdMoneda = Convert.ToInt32(row["id_moneda"]),
                            NombreMoneda = dt.Columns.Contains("nombre_moneda") ? row["nombre_moneda"].ToString() : string.Empty,
                            // CodigoMoneda = dt.Columns.Contains("codigo_moneda") ? row["codigo_moneda"].ToString() : string.Empty,
                            // SimboloMoneda = dt.Columns.Contains("simbolo_moneda") ? row["simbolo_moneda"].ToString() : string.Empty,
                            Estado = Convert.ToBoolean(row["estado"]),
                            UsuarioCreacion = row["usuario_creacion"] != DBNull.Value ? row["usuario_creacion"].ToString() : string.Empty,
                            FechaCreacion = row["fecha_creacion"] != DBNull.Value ? Convert.ToDateTime(row["fecha_creacion"]) : (DateTime?)null,
                            UsuarioModificacion = row["usuario_modificacion"] != DBNull.Value ? row["usuario_modificacion"].ToString() : string.Empty,
                            FechaModificacion = row["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(row["fecha_modificacion"]) : (DateTime?)null
                        };
                        lista.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de productos: " + ex.Message);
            }

            return lista;
        }

        // CAMBIO: Producto → ProductoClass
        public bool Crear(ProductoClass producto, string usuarioCreacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearProducto"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codigo", producto.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@id_moneda", producto.IdMoneda);
                    cmd.Parameters.AddWithValue("@usuario_creacion", usuarioCreacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el producto: " + ex.Message);
            }
        }

        // CAMBIO: Producto → ProductoClass
        public bool Actualizar(ProductoClass producto, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarProducto"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", producto.IdProducto);
                    cmd.Parameters.AddWithValue("@codigo", producto.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@id_moneda", producto.IdMoneda);
                    cmd.Parameters.AddWithValue("@estado", producto.Estado);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el producto: " + ex.Message);
            }
        }

        public bool Inactivar(int idProducto, string usuarioModificacion)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarProducto"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", idProducto);
                    cmd.Parameters.AddWithValue("@usuario_modificacion", usuarioModificacion);

                    conexionDB.EjecutarInstruccion(cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inactivar el producto (eliminación lógica): " + ex.Message);
            }
        }
    }
}