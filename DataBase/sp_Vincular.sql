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
        
        -- 1. Validar que el usuario exista y este activo
        SELECT @idUsuario = id_usuario FROM Usuarios WHERE usuario = @usuario AND estado = 1;
        IF @idUsuario IS NULL
            BEGIN RAISERROR('El usuario no existe o esta inactivo.', 16, 1); RETURN; END

        -- 2. Validar que el usuario no este ya asignado a otro cotizante
        IF EXISTS (SELECT 1 FROM Cotizantes WHERE id_usuario = @idUsuario AND id_cotizante != @id_cotizante)
            BEGIN RAISERROR('Este usuario ya esta asignado a otro cotizante.', 16, 1); RETURN; END

        -- 3. Actualizar el Cotizante con el Usuario
        UPDATE Cotizantes 
        SET id_usuario = @idUsuario, 
            usuario_modificacion = @usuario_creador, 
            fecha_modificacion = GETDATE()
        WHERE id_cotizante = @id_cotizante;

        -- 4. Asegurar que el usuario tenga el rol COT
        IF NOT EXISTS (SELECT 1 FROM Usuarios_Roles WHERE id_usuario = @idUsuario AND id_role = @idRole)
        BEGIN
            INSERT INTO Usuarios_Roles (id_usuario, id_role, usuario_creacion, fecha_creacion)
            VALUES (@idUsuario, @idRole, @usuario_creador, GETDATE());
        END
        
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
GO
