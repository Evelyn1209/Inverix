/*============================================================
  INVERIX - SCRIPT CONSOLIDADO Y LIMPIO (SIN ALTERS)
  Versión Final: Incluye Auditoría y todas las correcciones
============================================================*/

CREATE DATABASE SistemInverixDB
GO
USE SistemInverixDB
GO

/*========================================
  1. TABLAS BASE
========================================*/

-- Roles
CREATE TABLE Roles (
    id_role               INT IDENTITY(1,1) PRIMARY KEY,
    codigo                VARCHAR(20)  NOT NULL,
    nombre                VARCHAR(100) NOT NULL,
    estado                BIT          NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL,
    CONSTRAINT UQ_Roles_Codigo UNIQUE (codigo)
)
GO

-- Usuarios
CREATE TABLE Usuarios (
    id_usuario            INT IDENTITY(1,1) PRIMARY KEY,
    usuario               VARCHAR(50)  NOT NULL,
    clave                 VARCHAR(255) NOT NULL,
    estado                BIT          NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL,
    CONSTRAINT UQ_Usuarios_Usuario UNIQUE (usuario)
)
GO

-- Usuarios_Roles
CREATE TABLE Usuarios_Roles (
    id_usuario_role       INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario            INT          NOT NULL,
    id_role               INT          NOT NULL,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL,
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario),
    FOREIGN KEY (id_role)    REFERENCES Roles(id_role),
    CONSTRAINT UQ_Usuario_Rol UNIQUE (id_usuario, id_role)
)
GO

-- Tipo de Identificación
CREATE TABLE Tipo_Identificacion (
    id_tipo_identificacion INT IDENTITY(1,1) PRIMARY KEY,
    nombre                VARCHAR(100) NOT NULL,
    mascara               VARCHAR(50)  NULL,
    estado                BIT          NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL
)
GO

-- Monedas
CREATE TABLE Monedas (
    id_moneda             INT IDENTITY(1,1) PRIMARY KEY,
    codigo                VARCHAR(10)  NOT NULL,
    nombre                VARCHAR(50)  NOT NULL,
    simbolo               VARCHAR(5)   NOT NULL,
    estado                BIT          NOT NULL DEFAULT 1,
    eliminado             BIT          NOT NULL DEFAULT 0,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL,
    CONSTRAINT UQ_Monedas_Codigo UNIQUE (codigo)
)
GO

-- Productos
CREATE TABLE Productos (
    id_producto           INT IDENTITY(1,1) PRIMARY KEY,
    codigo                VARCHAR(20)  NOT NULL,
    nombre                VARCHAR(100) NOT NULL,
    id_moneda             INT          NOT NULL,
    estado                BIT          NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL,
    FOREIGN KEY (id_moneda) REFERENCES Monedas(id_moneda),
    CONSTRAINT UQ_Productos_Codigo UNIQUE (codigo)
)
GO

-- Plazos
CREATE TABLE Plazos (
    id_plazo              INT IDENTITY(1,1) PRIMARY KEY,
    dias                  INT          NOT NULL,
    descripcion           VARCHAR(50)  NULL,
    estado                BIT          NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)  NOT NULL,
    fecha_creacion        DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)  NULL,
    fecha_modificacion    DATETIME     NULL,
    CONSTRAINT UQ_Plazos_Dias UNIQUE (dias)
)
GO

-- Tasas
CREATE TABLE Tasas (
    id_tasa               INT IDENTITY(1,1) PRIMARY KEY,
    id_producto           INT            NOT NULL,
    id_plazo              INT            NOT NULL,
    porcentaje            DECIMAL(6,4)   NOT NULL,
    estado                BIT            NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)    NOT NULL,
    fecha_creacion        DATETIME       NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)    NULL,
    fecha_modificacion    DATETIME       NULL,
    FOREIGN KEY (id_producto) REFERENCES Productos(id_producto),
    FOREIGN KEY (id_plazo)    REFERENCES Plazos(id_plazo),
    CONSTRAINT UQ_Tasa_Producto_Plazo UNIQUE (id_producto, id_plazo),
    CONSTRAINT CK_Tasa_Porcentaje CHECK (porcentaje >= 0)
)
GO

-- Cotizantes (INTEGRADO CON id_usuario DESDE EL INICIO)
CREATE TABLE Cotizantes (
    id_cotizante           INT IDENTITY(1,1) PRIMARY KEY,
    id_tipo_identificacion INT          NOT NULL,
    identificacion         VARCHAR(50)  NOT NULL,
    nombre                 VARCHAR(100) NOT NULL,
    primer_apellido        VARCHAR(100) NOT NULL,
    segundo_apellido       VARCHAR(100) NOT NULL,
    correo                 VARCHAR(100) NULL,
    telefono               VARCHAR(20)  NULL,
    estado                 BIT          NOT NULL DEFAULT 1,
    usuario_creacion       VARCHAR(50)  NOT NULL,
    fecha_creacion         DATETIME     NOT NULL DEFAULT GETDATE(),
    usuario_modificacion   VARCHAR(50)  NULL,
    fecha_modificacion     DATETIME     NULL,
    id_usuario             INT          NULL, -- Columna integrada
    FOREIGN KEY (id_tipo_identificacion) REFERENCES Tipo_Identificacion(id_tipo_identificacion),
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario),
    CONSTRAINT UQ_Cotizante_Identificacion UNIQUE (identificacion)
)
GO

-- Cotizaciones
CREATE TABLE Cotizaciones (
    id_cotizacion          INT IDENTITY(1,1) PRIMARY KEY,
    numero_cotizacion      VARCHAR(50)   NOT NULL,
    idUsuario              INT           NOT NULL,
    idTasa                 INT           NOT NULL,
    id_producto            INT           NOT NULL,
    id_plazo               INT           NOT NULL,
    id_cotizante           INT           NOT NULL,
    fechaCotizacion        DATETIME      NOT NULL DEFAULT GETDATE(),
    fechaInicio            DATETIME      NOT NULL,
    fechaFinal             DATETIME      NOT NULL,
    monto                  DECIMAL(18,2) NOT NULL,
    tasaAplicada           DECIMAL(6,4)  NOT NULL,
    porcentajeImpuesto     DECIMAL(5,2)  NOT NULL,
    montoTotalInteresBruto DECIMAL(18,2) NOT NULL,
    montoTotalImpuesto     DECIMAL(18,2) NOT NULL,
    montoTotalNeto         DECIMAL(18,2) NOT NULL,
    estado                 BIT           NOT NULL DEFAULT 1,
    usuario_creacion       VARCHAR(50)   NOT NULL,
    fecha_creacion         DATETIME      NOT NULL DEFAULT GETDATE(),
    usuario_modificacion   VARCHAR(50)   NULL,
    fecha_modificacion     DATETIME      NULL,
    FOREIGN KEY (idUsuario)    REFERENCES Usuarios(id_usuario),
    FOREIGN KEY (idTasa)       REFERENCES Tasas(id_tasa),
    FOREIGN KEY (id_producto)  REFERENCES Productos(id_producto),
    FOREIGN KEY (id_plazo)     REFERENCES Plazos(id_plazo),
    FOREIGN KEY (id_cotizante) REFERENCES Cotizantes(id_cotizante),
    CONSTRAINT UQ_NumeroCotizacion UNIQUE (numero_cotizacion),
    CONSTRAINT CK_Cotizacion_Monto CHECK (monto > 0)
)
GO

-- Detalle Cotizacion
CREATE TABLE Detalle_Cotizacion (
    id_detalle_cotizacion INT IDENTITY(1,1) PRIMARY KEY,
    id_cotizacion         INT           NOT NULL,
    numero_mes            INT           NOT NULL,
    monto                 DECIMAL(18,2) NOT NULL,
    interesBruto          DECIMAL(18,2) NOT NULL,
    impuesto              DECIMAL(18,2) NOT NULL,
    interesNeto           DECIMAL(18,2) NOT NULL,
    estado                BIT           NOT NULL DEFAULT 1,
    usuario_creacion      VARCHAR(50)   NOT NULL,
    fecha_creacion        DATETIME      NOT NULL DEFAULT GETDATE(),
    usuario_modificacion  VARCHAR(50)   NULL,
    fecha_modificacion    DATETIME      NULL,
    FOREIGN KEY (id_cotizacion) REFERENCES Cotizaciones(id_cotizacion) ON DELETE CASCADE
)
GO

/*========================================
  2. ÍNDICES
========================================*/
CREATE INDEX IX_UsuariosRoles_Usuario      ON Usuarios_Roles(id_usuario)
CREATE INDEX IX_UsuariosRoles_Rol          ON Usuarios_Roles(id_role)
CREATE INDEX IX_Productos_Moneda           ON Productos(id_moneda)
CREATE INDEX IX_Tasas_Producto             ON Tasas(id_producto)
CREATE INDEX IX_Tasas_Plazo               ON Tasas(id_plazo)
CREATE INDEX IX_Cotizante_TipoIdentificacion ON Cotizantes(id_tipo_identificacion)
CREATE INDEX IX_Cotizacion_Usuario         ON Cotizaciones(idUsuario)
CREATE INDEX IX_Cotizacion_Tasa           ON Cotizaciones(idTasa)
CREATE INDEX IX_DetalleCotizacion_Cotizacion ON Detalle_Cotizacion(id_cotizacion)
GO

/*========================================
  3. FUNCIONES
========================================*/

-- Calcula el interés bruto mensual: (monto * tasa / 100) / 360 * 30
CREATE FUNCTION fn_CalcularInteresBruto (@monto DECIMAL(18,2), @tasa DECIMAL(6,4))
RETURNS DECIMAL(18,2)
AS
BEGIN
    RETURN (@monto * @tasa / 100) / 360 * 30
END
GO

-- Calcula el impuesto (13%) sobre el interés bruto
CREATE FUNCTION fn_CalcularImpuesto (@interes DECIMAL(18,2))
RETURNS DECIMAL(18,2)
AS
BEGIN
    RETURN @interes * 0.13
END
GO

/*========================================
  4. VISTAS
========================================*/

-- Vista historial completo de cotizaciones (CON id_cotizante)
CREATE VIEW vw_HistorialCotizaciones AS
SELECT
    c.id_cotizacion,
    c.id_cotizante,
    c.numero_cotizacion,
    c.fechaCotizacion,
    u.usuario,
    p.nombre AS producto,
    m.codigo AS moneda,
    pl.descripcion AS plazo,
    co.nombre + ' ' + co.primer_apellido + ' ' + co.segundo_apellido AS cotizante,
    co.identificacion,
    c.monto,
    c.tasaAplicada AS tasa,
    c.montoTotalInteresBruto AS montoTotalInteres,
    c.montoTotalImpuesto,
    c.montoTotalNeto
FROM Cotizaciones c
INNER JOIN Usuarios   u  ON c.idUsuario    = u.id_usuario
INNER JOIN Productos  p  ON c.id_producto  = p.id_producto
INNER JOIN Monedas    m  ON p.id_moneda    = m.id_moneda
INNER JOIN Plazos     pl ON c.id_plazo     = pl.id_plazo
INNER JOIN Cotizantes co ON c.id_cotizante = co.id_cotizante
GO

-- Vista de tasas con info de producto, plazo y moneda
CREATE VIEW vw_TasasProductos AS
SELECT
    t.id_tasa,
    t.id_producto,
    p.nombre   AS nombre_producto,
    t.id_plazo,
    pl.dias,
    t.porcentaje,
    t.estado,
    m.nombre AS nombre_moneda,
    t.usuario_creacion,
    t.fecha_creacion,
    t.usuario_modificacion,
    t.fecha_modificacion
FROM Tasas t
INNER JOIN Productos p  ON t.id_producto = p.id_producto
INNER JOIN Plazos    pl ON t.id_plazo    = pl.id_plazo
INNER JOIN Monedas   m  ON p.id_moneda    = m.id_moneda
GO

-- Vista cotizaciones por usuario
CREATE VIEW vw_CotizacionesUsuario AS
SELECT
    u.usuario,
    c.numero_cotizacion,
    p.nombre       AS producto,
    pl.descripcion AS plazo,
    c.monto,
    c.montoTotalNeto,
    c.fechaCotizacion
FROM Cotizaciones c
INNER JOIN Usuarios  u  ON c.idUsuario   = u.id_usuario
INNER JOIN Productos p  ON c.id_producto = p.id_producto
INNER JOIN Plazos    pl ON c.id_plazo    = pl.id_plazo
GO

/*========================================
  5. STORED PROCEDURES
========================================*/

-- ---- USUARIOS ----

CREATE PROCEDURE sp_CrearUsuario @usuario VARCHAR(50), @clave VARCHAR(100), @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    IF EXISTS (SELECT 1 FROM Usuarios WHERE usuario = @usuario) BEGIN RAISERROR('El usuario ya existe',16,1); RETURN END
    INSERT INTO Usuarios (usuario, clave, estado, usuario_creacion, fecha_creacion)
    VALUES (@usuario, CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave), 2), 1, @usuario_creacion, GETDATE())
    SELECT * FROM Usuarios WHERE id_usuario = SCOPE_IDENTITY()
END
GO

-- Actualizar Usuario (Versión Final con Clave Nueva y HEX)
CREATE PROCEDURE sp_ActualizarUsuario 
    @id_usuario INT, 
    @usuario VARCHAR(50), 
    @clave_nueva VARCHAR(255) = NULL, 
    @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    IF (@clave_nueva IS NOT NULL AND RTRIM(LTRIM(@clave_nueva)) != '')
    BEGIN
        UPDATE Usuarios 
        SET usuario = @usuario, 
            clave = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave_nueva), 2), 
            usuario_modificacion = @usuario_modificacion, 
            fecha_modificacion = GETDATE() 
        WHERE id_usuario = @id_usuario
    END
    ELSE
    BEGIN
        UPDATE Usuarios 
        SET usuario = @usuario, 
            usuario_modificacion = @usuario_modificacion, 
            fecha_modificacion = GETDATE() 
        WHERE id_usuario = @id_usuario
    END
END
GO

CREATE PROCEDURE sp_InactivarUsuario @id_usuario INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Usuarios SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_usuario = @id_usuario
END
GO

CREATE PROCEDURE sp_ListarUsuarios
AS BEGIN
    SET NOCOUNT ON
    SELECT u.id_usuario, u.usuario, u.estado, r.nombre AS rol, ur.id_role
    FROM Usuarios u
    LEFT JOIN Usuarios_Roles ur ON u.id_usuario = ur.id_usuario
    LEFT JOIN Roles r ON ur.id_role = r.id_role
END
GO

-- Login (Versión Final con id_cotizante y rolCodigo)
CREATE PROCEDURE sp_LoginUsuario 
    @usuario VARCHAR(50), 
    @clave VARCHAR(100)
AS BEGIN
    SET NOCOUNT ON
    SELECT 
        u.id_usuario, 
        u.usuario, 
        r.nombre AS rol,
        r.codigo  AS rolCodigo,
        co.id_cotizante
    FROM Usuarios u
    LEFT JOIN Usuarios_Roles ur ON u.id_usuario = ur.id_usuario
    LEFT JOIN Roles r           ON ur.id_role   = r.id_role
    LEFT JOIN Cotizantes co     ON co.id_usuario = u.id_usuario
    WHERE u.usuario = @usuario 
      AND u.clave = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave), 2) 
      AND u.estado = 1
END
GO

CREATE PROCEDURE sp_ActualizarClave
    @id_usuario INT,
    @clave_actual NVARCHAR(255),
    @clave_nueva NVARCHAR(255)
AS BEGIN
    SET NOCOUNT ON
    IF EXISTS (
        SELECT 1 FROM Usuarios
        WHERE id_usuario = @id_usuario
          AND clave = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave_actual), 2)
    )
    BEGIN
        UPDATE Usuarios
        SET clave = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave_nueva), 2),
            fecha_modificacion = GETDATE()
        WHERE id_usuario = @id_usuario
        SELECT 1 AS resultado, 'Contraseña actualizada correctamente.' AS mensaje
    END
    ELSE
    BEGIN
        SELECT 0 AS resultado, 'La contraseña actual es incorrecta.' AS mensaje
    END
END
GO

-- ---- ROLES ----

CREATE PROCEDURE sp_CrearRol @codigo VARCHAR(20), @nombre VARCHAR(100), @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    INSERT INTO Roles (codigo, nombre, estado, usuario_creacion, fecha_creacion) VALUES (@codigo, @nombre, 1, @usuario_creacion, GETDATE())
END
GO

CREATE PROCEDURE sp_ActualizarRol @id_role INT, @codigo VARCHAR(20), @nombre VARCHAR(100), @usuario_modificacion VARCHAR(50)
AS BEGIN
    UPDATE Roles SET codigo = @codigo, nombre = @nombre, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_role = @id_role
END
GO

CREATE PROCEDURE sp_InactivarRol @id_role INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Roles SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_role = @id_role
END
GO

CREATE PROCEDURE sp_ListarRoles
AS BEGIN
    SET NOCOUNT ON
    SELECT id_role, codigo, nombre, estado FROM Roles
END
GO

CREATE PROCEDURE sp_AsignarRolUsuario @id_usuario INT, @id_role INT, @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion, fecha_creacion) VALUES (@id_usuario, @id_role, @usuario_creacion, GETDATE())
END
GO

CREATE PROCEDURE sp_ActualizarRolUsuario 
    @id_usuario INT, 
    @id_role INT, 
    @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    IF EXISTS (SELECT 1 FROM Usuarios_Roles WHERE id_usuario = @id_usuario)
    BEGIN
        UPDATE Usuarios_Roles SET id_role = @id_role, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_usuario = @id_usuario
    END
    ELSE
    BEGIN
        INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion) VALUES (@id_usuario, @id_role, @usuario_modificacion)
    END
END
GO

-- ---- MONEDAS ----

CREATE PROCEDURE sp_CrearMoneda @codigo VARCHAR(10), @nombre VARCHAR(50), @simbolo VARCHAR(5), @usuario_creacion VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Monedas (codigo, nombre, simbolo, estado, usuario_creacion, fecha_creacion)
    VALUES (@codigo, @nombre, @simbolo, 1, @usuario_creacion, GETDATE())
END
GO

CREATE PROCEDURE sp_ActualizarMoneda @id_moneda INT, @codigo VARCHAR(10), @nombre VARCHAR(50), @simbolo VARCHAR(5), @estado BIT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Monedas SET codigo = @codigo, nombre = @nombre, simbolo = @simbolo, estado = @estado, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_moneda = @id_moneda
END
GO

CREATE PROCEDURE sp_InactivarMoneda @id_moneda INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Monedas SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_moneda = @id_moneda
END
GO

CREATE PROCEDURE sp_EliminarMoneda @id_moneda INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Monedas SET eliminado = 1, estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_moneda = @id_moneda
END
GO

CREATE PROCEDURE sp_ListarMonedas
AS BEGIN
    SET NOCOUNT ON
    SELECT id_moneda, codigo, nombre, simbolo, estado, usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion FROM Monedas WHERE eliminado = 0
END
GO

-- ---- PRODUCTOS ----

CREATE PROCEDURE sp_CrearProducto @codigo VARCHAR(20), @nombre VARCHAR(100), @id_moneda INT, @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    INSERT INTO Productos (codigo, nombre, id_moneda, estado, usuario_creacion, fecha_creacion) VALUES (@codigo, @nombre, @id_moneda, 1, @usuario_creacion, GETDATE())
END
GO

CREATE PROCEDURE sp_ActualizarProducto @id_producto INT, @codigo VARCHAR(20), @nombre VARCHAR(100), @id_moneda INT, @estado BIT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Productos SET codigo = @codigo, nombre = @nombre, id_moneda = @id_moneda, estado = @estado, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_producto = @id_producto
END
GO

CREATE PROCEDURE sp_InactivarProducto @id_producto INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Productos SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_producto = @id_producto
END
GO

CREATE PROCEDURE sp_ListarProductos
AS BEGIN
    SET NOCOUNT ON
    SELECT p.id_producto, p.codigo, p.nombre, p.id_moneda,
           m.nombre AS nombre_moneda, m.codigo AS codigo_moneda, m.simbolo AS simbolo_moneda,
           p.estado, p.usuario_creacion, p.fecha_creacion, p.usuario_modificacion, p.fecha_modificacion
    FROM Productos p INNER JOIN Monedas m ON p.id_moneda = m.id_moneda ORDER BY p.nombre ASC
END
GO

-- ---- PLAZOS ----

CREATE PROCEDURE sp_CrearPlazo @dias INT, @descripcion VARCHAR(50), @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    INSERT INTO Plazos (dias, descripcion, estado, usuario_creacion, fecha_creacion) VALUES (@dias, @descripcion, 1, @usuario_creacion, GETDATE())
END
GO

CREATE PROCEDURE sp_ActualizarPlazo @id_plazo INT, @dias INT, @descripcion VARCHAR(50), @estado BIT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    IF NOT EXISTS (SELECT 1 FROM Plazos WHERE id_plazo = @id_plazo) BEGIN RAISERROR('El plazo no existe.',16,1); RETURN END
    UPDATE Plazos SET dias = @dias, descripcion = @descripcion, estado = @estado, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_plazo = @id_plazo
    SELECT * FROM Plazos WHERE id_plazo = @id_plazo
END
GO

CREATE PROCEDURE sp_InactivarPlazo @id_plazo INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Plazos SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_plazo = @id_plazo
END
GO

CREATE PROCEDURE sp_ListarPlazos
AS BEGIN
    SET NOCOUNT ON
    SELECT id_plazo, dias, descripcion, estado, usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion FROM Plazos
END
GO

-- ---- TASAS ----

CREATE PROCEDURE sp_CrearTasa @id_producto INT, @id_plazo INT, @porcentaje DECIMAL(6,4), @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    INSERT INTO Tasas (id_producto, id_plazo, porcentaje, estado, usuario_creacion, fecha_creacion) VALUES (@id_producto, @id_plazo, @porcentaje, 1, @usuario_creacion, GETDATE())
END
GO

CREATE PROCEDURE sp_ActualizarTasa
    @id_tasa              INT,
    @porcentaje           DECIMAL(6,4),
    @estado               BIT,
    @usuario_modificacion VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON
    IF NOT EXISTS (SELECT 1 FROM Tasas WHERE id_tasa = @id_tasa)
    BEGIN RAISERROR('La tasa no existe', 16, 1); RETURN END
    UPDATE Tasas SET
        porcentaje           = @porcentaje,
        estado               = @estado,
        usuario_modificacion = @usuario_modificacion,
        fecha_modificacion   = GETDATE()
    WHERE id_tasa = @id_tasa
    SELECT * FROM Tasas WHERE id_tasa = @id_tasa
END
GO

CREATE PROCEDURE sp_InactivarTasa @id_tasa INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Tasas SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_tasa = @id_tasa
END
GO

CREATE PROCEDURE sp_ListarTasas
AS BEGIN
    SET NOCOUNT ON
    SELECT * FROM vw_TasasProductos
END
GO

CREATE PROCEDURE sp_ObtenerTasa @id_producto INT, @id_plazo INT
AS BEGIN
    SET NOCOUNT ON
    SELECT t.id_tasa, p.nombre AS producto, pl.dias AS plazo_dias, t.porcentaje
    FROM Tasas t
    INNER JOIN Productos p  ON t.id_producto = p.id_producto
    INNER JOIN Plazos    pl ON t.id_plazo    = pl.id_plazo
    WHERE t.id_producto = @id_producto AND t.id_plazo = @id_plazo AND t.estado = 1
END
GO

-- ---- TIPOS DE IDENTIFICACIÓN ----

CREATE PROCEDURE sp_ListarTiposIdentificacion
AS BEGIN
    SET NOCOUNT ON
    SELECT id_tipo_identificacion, nombre, mascara FROM Tipo_Identificacion WHERE estado = 1 ORDER BY nombre ASC
END
GO

-- ---- COTIZANTES ----

CREATE PROCEDURE sp_CrearCotizante @id_tipo_identificacion INT, @identificacion VARCHAR(50), @nombre VARCHAR(100), @primer_apellido VARCHAR(100), @segundo_apellido VARCHAR(100), @correo VARCHAR(100), @telefono VARCHAR(20), @usuario_creacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    IF EXISTS (SELECT 1 FROM Cotizantes WHERE identificacion = @identificacion) BEGIN RAISERROR('La identificación ya existe en el sistema',16,1); RETURN END
    INSERT INTO Cotizantes (id_tipo_identificacion, identificacion, nombre, primer_apellido, segundo_apellido, correo, telefono, estado, usuario_creacion, fecha_creacion)
    VALUES (@id_tipo_identificacion, @identificacion, @nombre, @primer_apellido, @segundo_apellido, @correo, @telefono, 1, @usuario_creacion, GETDATE())
    SELECT * FROM Cotizantes WHERE id_cotizante = SCOPE_IDENTITY()
END
GO

CREATE PROCEDURE sp_ActualizarCotizante @id_cotizante INT, @id_tipo_identificacion INT, @identificacion VARCHAR(50), @nombre VARCHAR(100), @primer_apellido VARCHAR(100), @segundo_apellido VARCHAR(100), @correo VARCHAR(100), @telefono VARCHAR(20), @estado BIT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Cotizantes SET id_tipo_identificacion = @id_tipo_identificacion, identificacion = @identificacion, nombre = @nombre,
        primer_apellido = @primer_apellido, segundo_apellido = @segundo_apellido, correo = @correo, telefono = @telefono,
        estado = @estado, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE()
    WHERE id_cotizante = @id_cotizante
END
GO

CREATE PROCEDURE sp_InactivarCotizante @id_cotizante INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Cotizantes SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_cotizante = @id_cotizante
END
GO

-- Listar Cotizantes (Versión Final: Incluye IdUsuario y Filtra Administradores)
CREATE PROCEDURE sp_ListarCotizantes
AS BEGIN
    SET NOCOUNT ON;
    SELECT c.id_cotizante AS IdCotizante, 
           c.id_tipo_identificacion AS IdTipoIdentificacion, 
           ti.nombre AS TipoIdentificacionNombre,
           c.identificacion AS Identificacion, 
           c.nombre AS Nombre, 
           c.primer_apellido AS PrimerApellido, 
           c.segundo_apellido AS SegundoApellido, 
           c.correo AS Correo, 
           c.telefono AS Telefono, 
           c.estado AS Estado,
           c.usuario_creacion AS UsuarioCreacion, 
           c.fecha_creacion AS FechaCreacion, 
           c.usuario_modificacion AS UsuarioModificacion, 
           c.fecha_modificacion AS FechaModificacion,
           c.id_usuario AS IdUsuario
    FROM Cotizantes c
    INNER JOIN Tipo_Identificacion ti ON c.id_tipo_identificacion = ti.id_tipo_identificacion
    LEFT JOIN Usuarios_Roles ur ON c.id_usuario = ur.id_usuario
    LEFT JOIN Roles r ON ur.id_role = r.id_role
    WHERE c.estado = 1 
      AND (c.id_usuario IS NULL OR r.codigo = 'COT')
    ORDER BY c.primer_apellido, c.nombre
END
GO

-- Procedimientos Especiales de Cotizantes Web y Accesos
CREATE PROCEDURE sp_RegistrarCotizanteWeb
    @usuario VARCHAR(50),
    @clave VARCHAR(100),
    @id_tipo_identificacion INT,
    @identificacion VARCHAR(50),
    @nombre VARCHAR(100),
    @primer_apellido VARCHAR(100),
    @segundo_apellido VARCHAR(100),
    @correo VARCHAR(100),
    @telefono VARCHAR(20)
AS BEGIN
    SET NOCOUNT ON;
    DECLARE @idUsuario INT;
    DECLARE @idRole INT = (SELECT id_role FROM Roles WHERE codigo = 'COT');
    BEGIN TRY
        BEGIN TRANSACTION
        IF EXISTS (SELECT 1 FROM Usuarios WHERE usuario = @usuario) BEGIN RAISERROR('El nombre de usuario ya está registrado.', 16, 1); RETURN; END
        IF EXISTS (SELECT 1 FROM Cotizantes WHERE identificacion = @identificacion) BEGIN RAISERROR('La identificación ya está registrada.', 16, 1); RETURN; END
        INSERT INTO Usuarios (usuario, clave, estado, usuario_creacion, fecha_creacion)
        VALUES (@usuario, CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave), 2), 1, @usuario, GETDATE());
        SET @idUsuario = SCOPE_IDENTITY();
        INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion, fecha_creacion) VALUES (@idUsuario, @idRole, @usuario, GETDATE());
        INSERT INTO Cotizantes (id_tipo_identificacion, identificacion, nombre, primer_apellido, segundo_apellido, correo, telefono, estado, usuario_creacion, fecha_creacion, id_usuario)
        VALUES (@id_tipo_identificacion, @identificacion, @nombre, @primer_apellido, @segundo_apellido, @correo, @telefono, 1, @usuario, GETDATE(), @idUsuario);
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_GenerarAccesoCotizante
    @id_cotizante INT,
    @usuario VARCHAR(50),
    @clave VARCHAR(100),
    @usuario_creador VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON;
    DECLARE @idUsuario INT;
    DECLARE @idRole INT = (SELECT id_role FROM Roles WHERE codigo = 'COT');
    BEGIN TRY
        BEGIN TRANSACTION
        IF EXISTS (SELECT 1 FROM Usuarios WHERE usuario = @usuario) BEGIN RAISERROR('El nombre de usuario ya está registrado.', 16, 1); RETURN; END
        INSERT INTO Usuarios (usuario, clave, estado, usuario_creacion, fecha_creacion)
        VALUES (@usuario, CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @clave), 2), 1, @usuario_creador, GETDATE());
        SET @idUsuario = SCOPE_IDENTITY();
        INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion, fecha_creacion) VALUES (@idUsuario, @idRole, @usuario_creador, GETDATE());
        UPDATE Cotizantes SET id_usuario = @idUsuario, usuario_modificacion = @usuario_creador, fecha_modificacion = GETDATE() WHERE id_cotizante = @id_cotizante;
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_VincularUsuarioCotizante
    @id_cotizante INT,
    @usuario VARCHAR(50),
    @usuario_creador VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON;
    DECLARE @idUsuario INT;
    DECLARE @idRole INT = (SELECT id_role FROM Roles WHERE codigo = 'COT');
    BEGIN TRY
        BEGIN TRANSACTION
        SELECT @idUsuario = id_usuario FROM Usuarios WHERE usuario = @usuario AND estado = 1;
        IF @idUsuario IS NULL BEGIN RAISERROR('El usuario no existe o esta inactivo.', 16, 1); RETURN; END
        IF EXISTS (SELECT 1 FROM Cotizantes WHERE id_usuario = @idUsuario AND id_cotizante != @id_cotizante) BEGIN RAISERROR('Este usuario ya esta asignado a otro cotizante.', 16, 1); RETURN; END
        UPDATE Cotizantes SET id_usuario = @idUsuario, usuario_modificacion = @usuario_creador, fecha_modificacion = GETDATE() WHERE id_cotizante = @id_cotizante;
        IF NOT EXISTS (SELECT 1 FROM Usuarios_Roles WHERE id_usuario = @idUsuario AND id_role = @idRole)
        BEGIN INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion, fecha_creacion) VALUES (@idUsuario, @idRole, @usuario_creador, GETDATE()); END
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO

-- ---- COTIZACIONES ----

CREATE PROCEDURE sp_GenerarCotizacion @idUsuario INT, @id_cotizante INT, @id_producto INT, @id_plazo INT, @monto DECIMAL(18,2), @usuario_ejecuta VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    DECLARE @tasa DECIMAL(6,4), @idTasa INT, @dias INT, @meses INT
    DECLARE @intBruto DECIMAL(18,2), @impuesto DECIMAL(18,2), @interesNeto DECIMAL(18,2)
    DECLARE @idCotizacion INT, @mes INT = 1, @ErrMsg NVARCHAR(4000)
    BEGIN TRY
        BEGIN TRANSACTION
        SELECT @dias = dias FROM Plazos WHERE id_plazo = @id_plazo
        SET @meses = @dias / 30
        SELECT @tasa = porcentaje, @idTasa = id_tasa FROM Tasas WHERE id_producto = @id_producto AND id_plazo = @id_plazo AND estado = 1
        IF @idTasa IS NULL BEGIN SET @ErrMsg = 'No existe una tasa activa para este producto y plazo.'; RAISERROR(@ErrMsg,16,1) END
        SET @intBruto    = dbo.fn_CalcularInteresBruto(@monto, @tasa)
        SET @impuesto    = dbo.fn_CalcularImpuesto(@intBruto)
        SET @interesNeto = @intBruto - @impuesto
        INSERT INTO Cotizaciones (numero_cotizacion, idUsuario, idTasa, id_producto, id_plazo, id_cotizante, fechaCotizacion, fechaInicio, fechaFinal, monto, tasaAplicada, porcentajeImpuesto, montoTotalInteresBruto, montoTotalImpuesto, montoTotalNeto, estado, usuario_creacion, fecha_creacion)
        VALUES (CONCAT('COT-', FORMAT(GETDATE(),'yyyyMMddHHmmss')), @idUsuario, @idTasa, @id_producto, @id_plazo, @id_cotizante, GETDATE(), GETDATE(), DATEADD(DAY,@dias,GETDATE()), @monto, @tasa, 13.00, (@intBruto*@meses), (@impuesto*@meses), ((@interesNeto*@meses)+@monto), 1, @usuario_ejecuta, GETDATE())
        SET @idCotizacion = SCOPE_IDENTITY()
        WHILE @mes <= @meses BEGIN
            INSERT INTO Detalle_Cotizacion (id_cotizacion, numero_mes, monto, interesBruto, impuesto, interesNeto, estado, usuario_creacion, fecha_creacion)
            VALUES (@idCotizacion, @mes, @monto, @intBruto, @impuesto, @interesNeto, 1, @usuario_ejecuta, GETDATE())
            SET @mes = @mes + 1
        END
        COMMIT TRANSACTION
        SELECT * FROM Cotizaciones WHERE id_cotizacion = @idCotizacion
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SET @ErrMsg = ERROR_MESSAGE(); RAISERROR(@ErrMsg,16,1)
    END CATCH
END
GO

CREATE PROCEDURE sp_ActualizarCotizacion @id_cotizacion INT, @id_cotizante INT, @id_tasa INT, @monto DECIMAL(18,2), @usuario_modifica VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    DECLARE @tasa DECIMAL(6,4), @id_producto INT, @id_plazo INT, @dias INT, @meses INT
    DECLARE @intBruto DECIMAL(18,2), @impuesto DECIMAL(18,2), @interesNeto DECIMAL(18,2)
    DECLARE @mes INT, @ErrorMsg NVARCHAR(4000)
    BEGIN TRY
        BEGIN TRANSACTION
        SELECT @tasa = t.porcentaje, @id_producto = t.id_producto, @id_plazo = t.id_plazo, @dias = p.dias
        FROM Tasas t INNER JOIN Plazos p ON t.id_plazo = p.id_plazo WHERE t.id_tasa = @id_tasa AND t.estado = 1
        IF @dias IS NULL BEGIN SET @ErrorMsg = 'La tasa no está activa o no existe.'; RAISERROR(@ErrorMsg,16,1); RETURN END
        SET @meses = @dias / 30
        SET @intBruto    = dbo.fn_CalcularInteresBruto(@monto, @tasa)
        SET @impuesto    = dbo.fn_CalcularImpuesto(@intBruto)
        SET @interesNeto = @intBruto - @impuesto
        UPDATE Cotizaciones SET id_cotizante = @id_cotizante, idTasa = @id_tasa, id_producto = @id_producto, id_plazo = @id_plazo,
            monto = @monto, tasaAplicada = @tasa, porcentajeImpuesto = 13.00,
            montoTotalInteresBruto = (@intBruto*@meses), montoTotalImpuesto = (@impuesto*@meses),
            montoTotalNeto = ((@interesNeto*@meses)+@monto), fechaFinal = DATEADD(DAY,@dias,fechaInicio),
            usuario_modificacion = @usuario_modifica, fecha_modificacion = GETDATE()
        WHERE id_cotizacion = @id_cotizacion
        DELETE FROM Detalle_Cotizacion WHERE id_cotizacion = @id_cotizacion
        SET @mes = 1
        WHILE @mes <= @meses BEGIN
            INSERT INTO Detalle_Cotizacion (id_cotizacion, numero_mes, monto, interesBruto, impuesto, interesNeto, estado, usuario_creacion, fecha_creacion)
            VALUES (@id_cotizacion, @mes, @monto, @intBruto, @impuesto, @interesNeto, 1, @usuario_modifica, GETDATE())
            SET @mes = @mes + 1
        END
        SELECT id_cotizacion, numero_cotizacion, monto, montoTotalNeto FROM Cotizaciones WHERE id_cotizacion = @id_cotizacion
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SET @ErrorMsg = ERROR_MESSAGE(); RAISERROR(@ErrorMsg,16,1)
    END CATCH
END
GO

CREATE PROCEDURE sp_InactivarCotizacion @id_cotizacion INT, @usuario_modificacion VARCHAR(50)
AS BEGIN
    SET NOCOUNT ON
    UPDATE Cotizaciones SET estado = 0, usuario_modificacion = @usuario_modificacion, fecha_modificacion = GETDATE() WHERE id_cotizacion = @id_cotizacion
END
GO

CREATE PROCEDURE sp_ListarCotizaciones
AS BEGIN
    SET NOCOUNT ON
    SELECT id_cotizacion, numero_cotizacion, producto, monto, montoTotalNeto, fechaCotizacion FROM vw_HistorialCotizaciones ORDER BY fechaCotizacion DESC
END
GO

CREATE PROCEDURE sp_ObtenerCotizacion @id_cotizacion INT
AS BEGIN
    SET NOCOUNT ON
    SELECT c.id_cotizacion, c.id_cotizante, c.numero_cotizacion, u.usuario, p.nombre AS producto, pl.dias AS plazo_dias,
        c.fechaCotizacion, c.fechaInicio, c.fechaFinal, c.monto, c.tasaAplicada, c.porcentajeImpuesto,
        c.montoTotalInteresBruto, c.montoTotalImpuesto, c.montoTotalNeto
    FROM Cotizaciones c
    INNER JOIN Usuarios  u  ON c.idUsuario   = u.id_usuario
    INNER JOIN Productos p  ON c.id_producto = p.id_producto
    INNER JOIN Plazos    pl ON c.id_plazo    = pl.id_plazo
    WHERE c.id_cotizacion = @id_cotizacion
END
GO

CREATE PROCEDURE sp_ObtenerDetalleCotizacion @id_cotizacion INT
AS BEGIN
    SET NOCOUNT ON
    SELECT numero_mes AS Mes, monto AS Monto, interesBruto AS InteresBruto, impuesto AS Impuesto, interesNeto AS InteresNeto
    FROM Detalle_Cotizacion WHERE id_cotizacion = @id_cotizacion AND estado = 1 ORDER BY numero_mes
END
GO

CREATE PROCEDURE sp_CotizacionesPorUsuario @id_usuario INT
AS BEGIN
    SET NOCOUNT ON
    SELECT numero_cotizacion, producto, plazo, monto, montoTotalNeto, fechaCotizacion
    FROM vw_CotizacionesUsuario WHERE usuario = (SELECT usuario FROM Usuarios WHERE id_usuario = @id_usuario) ORDER BY fechaCotizacion DESC
END
GO

CREATE PROCEDURE sp_ReporteCotizacion @id_cotizacion INT
AS BEGIN
    SET NOCOUNT ON
    SELECT numero_cotizacion, fechaCotizacion, usuario AS usuario_creador, producto, moneda, plazo,
        cotizante, identificacion AS numeroIdentificacion, monto, tasa, montoTotalInteres, montoTotalImpuesto, montoTotalNeto
    FROM vw_HistorialCotizaciones WHERE id_cotizacion = @id_cotizacion
END
GO

CREATE PROCEDURE sp_ReporteDetalleCotizacion @id_cotizacion INT
AS BEGIN
    SET NOCOUNT ON
    SELECT numero_mes, interesBruto, impuesto, interesNeto
    FROM Detalle_Cotizacion WHERE id_cotizacion = @id_cotizacion ORDER BY numero_mes
END
GO

/*========================================
  6. DATOS INICIALES
========================================*/

-- Tipos de identificación
INSERT INTO Tipo_Identificacion (nombre, mascara, estado, usuario_creacion, fecha_creacion) VALUES
('Cédula Física',   '0-0000-0000',  1, 'admin', GETDATE()),
('Cédula Jurídica', '3-000-000000', 1, 'admin', GETDATE()),
('DIMEX',           '000000000000', 1, 'admin', GETDATE()),
('NITE',            '0000000000',   1, 'admin', GETDATE()),
('Pasaporte',       'XXXXXXXXX',    1, 'admin', GETDATE())
GO

-- Monedas
INSERT INTO Monedas (codigo, nombre, simbolo, usuario_creacion) VALUES
('CRC', 'Colones', '₡', 'admin'),
('USD', 'Dolares', '$', 'admin')
GO

-- Productos
INSERT INTO Productos (codigo, nombre, id_moneda, usuario_creacion) VALUES
('CC', 'Colón Crece',       1, 'admin'),
('CF', 'Colón Futuro Plus', 1, 'admin'),
('DS', 'Dólar Seguro',      2, 'admin'),
('DV', 'Dólar Visión',      2, 'admin')
GO

-- Plazos
INSERT INTO Plazos (dias, descripcion, usuario_creacion) VALUES
( 15, '15 días',   'admin'), ( 30, '1 mes',    'admin'), ( 60, '2 meses',  'admin'),
( 90, '3 meses',   'admin'), (120, '4 meses',  'admin'), (150, '5 meses',  'admin'),
(180, '6 meses',   'admin'), (210, '7 meses',  'admin'), (240, '8 meses',  'admin'),
(270, '9 meses',   'admin'), (300, '10 meses', 'admin'), (330, '11 meses', 'admin'),
(360, '12 meses',  'admin'), (390, '13 meses', 'admin'), (420, '14 meses', 'admin'),
(450, '15 meses',  'admin'), (480, '16 meses', 'admin'), (510, '17 meses', 'admin'),
(540, '18 meses',  'admin'), (570, '19 meses', 'admin'), (600, '20 meses', 'admin'),
(630, '21 meses',  'admin'), (660, '22 meses', 'admin'), (690, '23 meses', 'admin'),
(720, '24 meses',  'admin')
GO

-- Tasas Completas según tabla oficial
INSERT INTO Tasas (id_producto, id_plazo, porcentaje, usuario_creacion) VALUES
-- Colón Crece (CC) - ID 1
(1,1,3.1500,'admin'),(1,2,3.2500,'admin'),(1,3,3.4000,'admin'),(1,4,3.6000,'admin'),(1,5,3.7500,'admin'),
(1,6,3.9000,'admin'),(1,7,4.1000,'admin'),(1,8,4.2500,'admin'),(1,9,4.4000,'admin'),(1,10,4.5500,'admin'),
(1,11,4.7000,'admin'),(1,12,4.8500,'admin'),(1,13,5.0000,'admin'),(1,14,5.1000,'admin'),(1,15,5.2000,'admin'),
(1,16,5.3000,'admin'),(1,17,5.4000,'admin'),(1,18,5.5000,'admin'),(1,19,5.6500,'admin'),(1,20,5.8000,'admin'),
(1,21,5.9500,'admin'),(1,22,6.1000,'admin'),(1,23,6.2500,'admin'),(1,24,6.4000,'admin'),(1,25,6.5500,'admin'),

-- Colón Futuro Plus (CF) - ID 2
(2,4,3.8500,'admin'),(2,5,4.0000,'admin'),(2,6,4.1500,'admin'),(2,7,4.3500,'admin'),(2,8,4.5000,'admin'),
(2,9,4.6500,'admin'),(2,10,4.8000,'admin'),(2,11,4.9500,'admin'),(2,12,5.1000,'admin'),(2,13,5.3000,'admin'),
(2,14,5.4000,'admin'),(2,15,5.5000,'admin'),(2,16,5.6000,'admin'),(2,17,5.7000,'admin'),(2,18,5.8000,'admin'),
(2,19,5.9500,'admin'),(2,20,6.1000,'admin'),(2,21,6.2500,'admin'),(2,22,6.4000,'admin'),(2,23,6.5500,'admin'),
(2,24,6.7000,'admin'),(2,25,6.9000,'admin'),

-- Dólar Seguro (DS) - ID 3
(3,2,1.0000,'admin'),(3,3,1.2500,'admin'),(3,4,1.5000,'admin'),(3,5,1.6500,'admin'),(3,6,1.8000,'admin'),
(3,7,2.0000,'admin'),(3,8,2.1500,'admin'),(3,9,2.3000,'admin'),(3,10,2.4500,'admin'),(3,11,2.6000,'admin'),
(3,12,2.7500,'admin'),(3,13,2.9000,'admin'),(3,14,3.0000,'admin'),(3,15,3.1000,'admin'),(3,16,3.2000,'admin'),
(3,17,3.3000,'admin'),(3,18,3.4000,'admin'),(3,19,3.5500,'admin'),(3,20,3.7000,'admin'),(3,21,3.8500,'admin'),
(3,22,4.0000,'admin'),(3,23,4.1500,'admin'),(3,24,4.3000,'admin'),(3,25,4.4500,'admin'),

-- Dólar Visión (DV) - ID 4
(4,4,2.7500,'admin'),(4,5,2.9000,'admin'),(4,6,3.0500,'admin'),(4,7,3.2500,'admin'),(4,8,3.4000,'admin'),
(4,9,3.5500,'admin'),(4,10,3.7000,'admin'),(4,11,3.8500,'admin'),(4,12,4.0000,'admin'),(4,13,4.2000,'admin'),
(4,14,4.3000,'admin'),(4,15,4.4000,'admin'),(4,16,4.5000,'admin'),(4,17,4.6000,'admin'),(4,18,4.7000,'admin'),
(4,19,4.8500,'admin'),(4,20,5.0000,'admin'),(4,21,5.1500,'admin'),(4,22,5.3000,'admin'),(4,23,5.4500,'admin'),
(4,24,5.6000,'admin'),(4,25,6.8500,'admin')
GO


-- Roles Iniciales
INSERT INTO Roles (codigo, nombre, estado, usuario_creacion, fecha_creacion) VALUES 
('ADM', 'Administrador del Sistema', 1, 'admin', GETDATE()),
('COT', 'Cotizante', 1, 'admin', GETDATE())
GO

-- Usuario admin
INSERT INTO Usuarios (usuario, clave, estado, usuario_creacion, fecha_creacion)
VALUES ('admin', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', 'Admin123'), 2), 1, 'admin', GETDATE())
GO

INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion, fecha_creacion)
VALUES ((SELECT id_usuario FROM Usuarios WHERE usuario = 'admin'), (SELECT id_role FROM Roles WHERE codigo = 'ADM'), 'admin', GETDATE())
GO

/*============================================================
  7. AUDITORIA DEL SISTEMA
============================================================*/

CREATE TABLE Auditoria_Completa (
    IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
    SeccionAfectada VARCHAR(50) NOT NULL,      
    CuentaAfectada VARCHAR(100) NOT NULL,      
    AccionRealizada VARCHAR(100) NOT NULL,     
    DetalleDelCambio VARCHAR(255) NULL,        
    CreadoOriginalmentePor VARCHAR(100) NOT NULL,
    FechaHoraDeCreacion DATETIME NOT NULL,
    ActualizadoPor VARCHAR(100) NOT NULL,
    FechaHoraDeActualizacion DATETIME DEFAULT GETDATE()
);
GO

CREATE TRIGGER trg_Auditoria_Usuarios ON Usuarios AFTER INSERT, UPDATE AS BEGIN
    IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO Auditoria_Completa (SeccionAfectada, CuentaAfectada, AccionRealizada, CreadoOriginalmentePor, FechaHoraDeCreacion, ActualizadoPor, DetalleDelCambio)
        SELECT CASE WHEN EXISTS(SELECT 1 FROM Usuarios_Roles ur WHERE ur.id_usuario = i.id_usuario AND ur.id_role = (SELECT id_role FROM Roles WHERE codigo='COT')) THEN 'Acceso de Cotizante' ELSE 'Acceso de Administrador' END,
               i.usuario, 'Creación Original de Acceso', i.usuario_creacion, i.fecha_creacion, i.usuario_creacion, 'Se registró este acceso por primera vez.'
        FROM inserted i;
    END
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        IF UPDATE(clave) BEGIN
            INSERT INTO Auditoria_Completa (SeccionAfectada, CuentaAfectada, AccionRealizada, CreadoOriginalmentePor, FechaHoraDeCreacion, ActualizadoPor, DetalleDelCambio)
            SELECT CASE WHEN EXISTS(SELECT 1 FROM Usuarios_Roles ur WHERE ur.id_usuario = i.id_usuario AND ur.id_role = (SELECT id_role FROM Roles WHERE codigo='COT')) THEN 'Acceso de Cotizante' ELSE 'Acceso de Administrador' END,
                   i.usuario, 'Cambio de Contraseña', i.usuario_creacion, i.fecha_creacion, ISNULL(i.usuario_modificacion, 'Sistema'), 'Se modificó la contraseña.'
            FROM inserted i JOIN deleted d ON i.id_usuario = d.id_usuario WHERE i.clave <> d.clave;
        END
    END
END;
GO

CREATE TRIGGER trg_Auditoria_Cotizantes ON Cotizantes AFTER INSERT, UPDATE AS BEGIN
    IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO Auditoria_Completa (SeccionAfectada, CuentaAfectada, AccionRealizada, CreadoOriginalmentePor, FechaHoraDeCreacion, ActualizadoPor, DetalleDelCambio)
        SELECT 'Ficha de Cotizante', i.identificacion, 'Creación Original del Perfil', i.usuario_creacion, i.fecha_creacion, i.usuario_creacion, 'Se registró el cotizante por primera vez.'
        FROM inserted i;
    END
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO Auditoria_Completa (SeccionAfectada, CuentaAfectada, AccionRealizada, CreadoOriginalmentePor, FechaHoraDeCreacion, ActualizadoPor, DetalleDelCambio)
        SELECT 'Ficha de Cotizante', i.identificacion, 'Actualización de Perfil', i.usuario_creacion, i.fecha_creacion, ISNULL(i.usuario_modificacion, 'Sistema'), 'Se actualizaron los datos personales.'
        FROM inserted i JOIN deleted d ON i.id_cotizante = d.id_cotizante;
    END
END;
GO
