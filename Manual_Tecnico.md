# Manual Técnico del Sistema Inverix

## 1. Introducción
El manual técnico es un documento integral que proporciona una guía detallada sobre el desarrollo, implementación y mantenimiento del proyecto Inverix. Esta es una aplicación web utilizando Web Forms de ASP.NET y ADO.NET. El propósito es servir como referencia para desarrolladores, administradores y cualquier persona involucrada en el ciclo de vida del proyecto. La arquitectura se basa en una estructura N-Capas (Presentación con Web Forms, Lógica de Negocio y de Acceso a Base de Dato en Controladores), asegurando la escalabilidad.

## 2. Requisitos del Sistema
**Software:**
- **Sistema Operativo:** Windows Server 2016 o superior / Windows 10/11 para desarrollo.
- **Framework:** .NET Framework 4.7.2.
- **Servidor Web:** IIS (Internet Information Services) 10.0+ o IIS Express (desarrollo).
- **Base de Datos:** SQL Server 2017+ Management Studio.
- **Lenguaje Principal:** C# (C-Sharp).
- **Plataforma Web:** ASP.NET Web Forms.

**Hardware Mínimo Recomendado:**
- **Procesador:** Intel Core i5 / AMD Ryzen 5 o superior.
- **Memoria RAM:** Mínimo 8 GB.
- **Almacenamiento:** Mínimo 50 GB de espacio disponible.

## 3. Diseño de la Base de Datos
La base de datos `InverixDB` es de tipo relacional, completamente normalizada para asegurar la integridad de los datos financieros. 
**Tablas Principales y Relaciones:**
- `Roles` y `Usuarios`: Control de acceso (relación 1:N a través de `Usuarios_Roles`).
- `Tipo_Identificacion`: Clasificación de los formatos de identificación (NITE, Cédula física, DIMEX, etc.).
- `Cotizantes`: Información personal y de contacto de usuarios clientes (vinculado a Tipo de Identificación).
- `Monedas`: Catálogo de divisas manejadas (Colones (CRC), Dólares (USD)).
- `Productos`: Catálogo de productos de inversión (ej. Colón Crece, Dólar Visión). Tiene una dependencia foránea sobre `Monedas` (N:1).
- `Plazos`: Catálogo de la duración en días y su descripción semántica.
- `Tasas`: Matriz de cruce entre `Productos` y `Plazos`, clave foránea a ambas y define el porcentaje de rendimiento para la intersección de estos.
- `Cotizaciones`: Entidad transaccional central. Vincula a un `Cotizante`, `Usuario`, `Tasa`, `Producto` y `Plazo`, almacenando explícitamente el cálculo de la proyección de interés, rendimientos e impuestos.
- `Detalle_Cotizacion`: Tabla hija de `Cotizaciones` que especifica la proyección y el interés neto y bruto por mes.

## 4. Diccionario de Datos
- **Rol:** Nivel de acceso al sistema (ej. Administrador).
- **Cotizante:** Cliente o prospecto interesado en realizar la apertura o proyección de una inversión.
- **Tasa:** Porcentaje de interés aplicado a un monto de inversión para un plazo y producto específico.
- **Cotización:** Documento virtual que proyecta el comportamiento de una inversión a plazos y su rendimiento.
- **Interés Bruto:** Rendimiento generado sin deducir obligaciones tributarias. `(monto * tasa / 100) / 360 * 30`.
- **Impuesto:** Deducción establecida (ej. 13%) deducido sobre el interés bruto generado.
- **Interés Neto:** Producto de restar el impuesto al interés bruto.

## 5. Configuración de ADO.NET
El acceso a la información se gestiona enteramente de forma nativa mediante la interfaz de ADO.NET, haciendo uso de los proveedores `System.Data.SqlClient`.
- **Cadena de Conexión:** Definida en el `Web.config` bajo el elemento `<connectionStrings>`.
  ```xml
  <add name="Conexion" connectionString="Data Source=SERVIDOR;Initial Catalog=InverixDB;Integrated Security=True" providerName="System.Data.SqlClient" />
  ```
- **Gestión transaccional:** Se implementa un modelo de conexión (`SqlConnection`) dentro de bloques `using()`, garantizando la liberación de memoria (Dispose) e incorporación en el connection pool tras ejecución.
- Toda ejecución se delega a **Stored Procedures** mitigando vulnerabilidades como inyección SQL y potenciando desempeño.

## 6. Estructura del Proyecto
El proyecto sigue una separación en directorios basada en responsabilidades lógicas:
- **`Classes/`**: Entidades de negocio (ej. `Entidades.cs`) que definen cómo se transportan los datos de una capa a otra.
- **`Controllers/`**: Centralizan lógica de negocio transaccional y el acceso a base de datos. (`TasasController.cs`, `ProductoController.cs`, etc.).
- **`DataBase/`**: Contiene los scripts de base de datos (`InverixDB.sql`).
- **`Images/`**: Recursos y gráficos visuales para el diseño UI.
- **`Styles/`**: Código Vainilla CSS y estandarización UI / UX (`InverixStyle.css`, `CalculadoraStyle.css`).
- **`Views/`**: Interfaces en Web Forms (archivos `.aspx` y su respectiva lógica de Code-Behind `.cs`).
- **`Web.config`**: Configuración central del servidor IIS e hilos de ejecución .NET.

## 7. Desarrollo de Controles
La interfaz se construyó priorizando el uso de *ASP.NET Server Controls* (`<asp:TextBox>`, `<asp:DropDownList>`, `<asp:GridView>`, `<asp:Button>`).
- Habilitación de la propiedad `AutoPostBack="True"` para elementos de lista donde se deban encadenar comportamientos lógicos en tiempo real.
- A nivel del Code-Behind se implementó la delegación enlazada de eventos (`OnClick`, `OnSelectedIndexChanged`).
- Manipulación de elementos de enlace (`ItemTemplate`) en listas usando comandos (`CommandName` y `CommandArgument`) de forma nativa para control CRUD.

## 8. Manejo de Datos
Operaciones de ciclo de vida (CRUD) bajo patrón centralizado:
- **Lectura:** Recuperación en memoria de estructuras tipo Listas Genéricas o `DataTable` utilizando iteradores veloces (`SqlDataReader`) para bindeo a GridViews (DataBind).
- **Creación / Modificación (CUD):** Petición inyectada con mapeo directo de validadores SqlDbType a los Parámetros del Comando enviando estado y variables a ejecutar.
- **Eliminación Lógica:** Restricción intencional de borrados físicos `DELETE`. El sistema manipula banderas booleanas (`estado` o `eliminado = 1`) que inhabilitan y excluyen los registros para velar por integridad relacional histórica.

## 9. Validación y Manejo de Errores
- **Validación Front-end:** Inyección nativa a controles validadores como `RequiredFieldValidator` a nivel UI forzando reglas del Input previas a ejecución de PostBacks.
- **Validación Transaccional en Base:** Bloques de encapsulado `BEGIN TRY... BEGIN TRANSACTION... COMMIT` para inserciones atómicas (ej. Cotizaciones o creación con detalles). Contingencias emiten un `ROLLBACK`.
- **Control Defensivo en C#:** Peticiones ADO.NET recubiertas bajo bloques `try...catch (SqlException ex)` con cierres efectivos (bloque `finally`) impidiendo fugas (Leaks).

## 10. Pruebas y Validación
Para la validación del sistema, el equipo procede bajo estos pilares:
- **Pruebas de Componente / Unitarias Manuales:** Se validan procedimientos críticos transaccionales por separado (Stored Procedures matemáticos y generación de tablas lógicas vs Excel auxiliar/calculadora).
- **Integración T2B (Top To Bottom):** Pruebas validando formularios y PostBacks, mitigando la pérdida de de llaves o `DataKeys` al recargar el Page Life-Cycle (ej: Uso defensivo de `!IsPostBack`).

## 11. Documentación y Comentarios
Se mantiene la exigencia de un código legible y ordenado:
- Los Stored Procedures están definidos bajo normas semánticas descriptivas (Prefijo `sp_` + Acción `Listar, Generar, etc.`). Funciones o fórmulas incluyen comentarios descriptivos paralelos (SQL `--`).
- En lógica C#, clases y métodos exponen explícitamente comentarios documentando sobre lógicas matemáticas (`//`).
- Convención *PascalCase* de nomenclatura general para Clases, SPs y Controladores, y *camelCase* para variables internas.

## 12. Implementación y Despliegue
Guía simplificada de paso a Producción:
1. **Ejecución DDL:** Ejecutar el maestro total `InverixDB.sql` sobre el nuevo target SQL Management para preparar el esquema predefinido completo.
2. **Reconfiguración Config:** Adaptar el `Web.config` `connectionString` apuntando las credenciales necesarias del servidor hospedador.
3. **Build:** Compilación .NET (Clean & Build) asegurando la optimización (Release).
4. **Deploy IIS:** Realizar `Publish Profile` y apuntar a la ruta en un Application Pool configurado a .NET CLR 4.0 bajo usuario autorizado con permisos sobre la carpeta local.

## 13. Mantenimiento y Soporte
- La base operativa requiere de resguardos y políticas de Backups SQL recurrentes (Full o Incremental) vía SQL Server Agent debido a que se prohíben borrados físicos, implicando un crecimiento incremental seguro.
- La trazabilidad centraliza problemas: todo registro es auditado bajo los campos `usuario_creacion`, `fecha_creacion`, `usuario_modificacion` y `fecha_modificacion`. Eventos erróneos se resguardan ahí.
- Toda corrección transaccional recae en modificar el estado por aplicativo y no por sentencias crudas a manubrio (`UPDATE` plano). Esto conserva el historial transaccional de cálculos inmutado.
