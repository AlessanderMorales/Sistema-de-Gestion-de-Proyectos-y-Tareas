-- ============================================
-- PROCEDIMIENTOS ALMACENADOS PARA GESTIONAR ASIGNACIÓN DE EMPLEADOS A PROYECTOS
-- ============================================

USE gestion_proyectos;

-- ============================================
-- 1. PROCEDIMIENTO: Asignar Usuario a Proyecto
-- ============================================
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_asignar_usuario_proyecto$$

CREATE PROCEDURE sp_asignar_usuario_proyecto(
    IN p_id_proyecto INT,
    IN p_id_usuario INT
)
BEGIN
    DECLARE v_count INT;
    
    -- Verificar si la asignación ya existe
  SELECT COUNT(*) INTO v_count
    FROM proyecto_usuario
    WHERE id_proyecto = p_id_proyecto 
  AND id_usuario = p_id_usuario;
    
    -- Si no existe, insertar
  IF v_count = 0 THEN
        INSERT INTO proyecto_usuario (id_proyecto, id_usuario, fecha_asignacion)
        VALUES (p_id_proyecto, p_id_usuario, NOW());
    
        SELECT 'Usuario asignado exitosamente' AS Mensaje;
    ELSE
        SELECT 'El usuario ya esta asignado a este proyecto' AS Mensaje;
    END IF;
END$$

DELIMITER ;

-- ============================================
-- 2. PROCEDIMIENTO: Desasignar Usuario de Proyecto
-- ============================================
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_desasignar_usuario_proyecto$$

CREATE PROCEDURE sp_desasignar_usuario_proyecto(
    IN p_id_proyecto INT,
    IN p_id_usuario INT
)
BEGIN
    -- Eliminar la asignación
    DELETE FROM proyecto_usuario
    WHERE id_proyecto = p_id_proyecto 
      AND id_usuario = p_id_usuario;
  
    -- Desasignar tareas del usuario en ese proyecto
    UPDATE Tareas
    SET id_usuario_asignado = NULL
    WHERE id_proyecto = p_id_proyecto 
 AND id_usuario_asignado = p_id_usuario;
  
    -- Eliminar de la tabla intermedia tarea_usuario_asignado
    DELETE tua FROM tarea_usuario_asignado tua
  INNER JOIN Tareas t ON tua.id_tarea = t.id_tarea
    WHERE t.id_proyecto = p_id_proyecto
      AND tua.id_usuario = p_id_usuario;
    
    SELECT 'Usuario desasignado y sus tareas removidas' AS Mensaje;
END$$

DELIMITER ;

-- ============================================
-- 3. PROCEDIMIENTO: Obtener Usuarios Asignados a un Proyecto
-- ============================================
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_obtener_usuarios_proyecto$$

CREATE PROCEDURE sp_obtener_usuarios_proyecto(
    IN p_id_proyecto INT
)
BEGIN
    SELECT 
     u.id_usuario AS Id,
        u.nombres AS Nombres,
        u.primer_apellido AS PrimerApellido,
   u.segundo_apellido AS SegundoApellido,
      u.nombre_usuario AS NombreUsuario,
        u.email AS Email,
        u.rol AS Rol,
    u.estado AS Estado,
 pu.fecha_asignacion AS FechaAsignacion
    FROM Usuario u
    INNER JOIN proyecto_usuario pu ON u.id_usuario = pu.id_usuario
    WHERE pu.id_proyecto = p_id_proyecto
      AND u.estado = 1
    ORDER BY u.primer_apellido, u.nombres;
END$$

DELIMITER ;

-- ============================================
-- 4. VERIFICAR QUE LA TABLA proyecto_usuario EXISTE
-- ============================================
-- Si no existe, crearla:

CREATE TABLE IF NOT EXISTS proyecto_usuario (
    id_proyecto INT NOT NULL,
    id_usuario INT NOT NULL,
    fecha_asignacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_proyecto, id_usuario),
    FOREIGN KEY (id_proyecto) REFERENCES Proyecto(id_proyecto) ON DELETE CASCADE,
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    INDEX idx_proyecto (id_proyecto),
    INDEX idx_usuario (id_usuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- 5. PRUEBAS (Opcional - Comentar antes de ejecutar en producción)
-- ============================================
/*
-- Asignar usuario 2 al proyecto 1
CALL sp_asignar_usuario_proyecto(1, 2);

-- Ver usuarios asignados al proyecto 1
CALL sp_obtener_usuarios_proyecto(1);

-- Desasignar usuario 2 del proyecto 1
CALL sp_desasignar_usuario_proyecto(1, 2);

-- Verificar que fue desasignado
CALL sp_obtener_usuarios_proyecto(1);
*/

SELECT 'Procedimientos almacenados creados exitosamente' AS Status;
