-- ============================================================================
-- SCRIPT COMPLETO DE BASE DE DATOS - SISTEMA DE GESTIÓN DE PROYECTOS
-- Incluye: Estructura, Triggers, Datos de Prueba, Contraseñas Hasheadas
--     y Procedimientos para Asignar Empleados a Proyectos
-- ============================================================================

DROP DATABASE IF EXISTS gestion_proyectos;
CREATE DATABASE gestion_proyectos;
USE gestion_proyectos;

-- ============================================================================
-- TABLAS
-- ============================================================================

CREATE TABLE Proyecto (
  id_proyecto INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(30) NOT NULL,
    descripcion VARCHAR(255),
    fecha_inicio DATETIME NULL,
    fecha_fin DATETIME NULL,
    estado TINYINT DEFAULT 1,
    fechaRegistro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimaModificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Usuario (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    primer_apellido VARCHAR(50) NOT NULL,
    segundo_apellido VARCHAR(50),
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
    contraseña VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
rol VARCHAR(50),
    estado TINYINT DEFAULT 1,
    
    INDEX idx_nombre_usuario (nombre_usuario),
    INDEX idx_email (email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Tareas (
    id_tarea INT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(100) NOT NULL,
    descripcion TEXT,
    prioridad VARCHAR(30),
    estado TINYINT DEFAULT 1,
    fechaRegistro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimaModificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    id_proyecto INT NULL,
    id_usuario_asignado INT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'SinIniciar',

    CONSTRAINT fk_tarea_proyecto FOREIGN KEY (id_proyecto) REFERENCES Proyecto(id_proyecto) ON DELETE SET NULL,
    CONSTRAINT fk_tarea_usuario_asignado FOREIGN KEY (id_usuario_asignado) REFERENCES Usuario(id_usuario) ON DELETE SET NULL,
    
  INDEX idx_tareas_proyecto (id_proyecto)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Tarea_Usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_tarea INT NOT NULL,
    id_usuario INT NOT NULL,
    fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  estado TINYINT DEFAULT 1,
    
    CONSTRAINT fk_tu_tarea FOREIGN KEY (id_tarea) REFERENCES Tareas(id_tarea) ON DELETE CASCADE,
  CONSTRAINT fk_tu_usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    UNIQUE KEY unique_assignment (id_tarea, id_usuario),
    INDEX idx_tarea (id_tarea),
    INDEX idx_usuario (id_usuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ? TABLA PARA ASIGNAR EMPLEADOS A PROYECTOS
CREATE TABLE Proyecto_Usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_proyecto INT NOT NULL,
    id_usuario INT NOT NULL,
    fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    estado TINYINT DEFAULT 1,

    CONSTRAINT fk_pu_proyecto FOREIGN KEY (id_proyecto) REFERENCES Proyecto(id_proyecto) ON DELETE CASCADE,
    CONSTRAINT fk_pu_usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    UNIQUE KEY unique_proyecto_usuario (id_proyecto, id_usuario),
  INDEX idx_proyecto (id_proyecto),
    INDEX idx_usuario (id_usuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Comentario (
    id_comentario INT AUTO_INCREMENT PRIMARY KEY,
    contenido VARCHAR(1000) NOT NULL,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    estado TINYINT DEFAULT 1,

    id_tarea INT NOT NULL,
    id_usuario INT NOT NULL,
    id_destinatario INT NULL,

    CONSTRAINT fk_comentario_tarea FOREIGN KEY (id_tarea) REFERENCES Tareas(id_tarea) ON DELETE CASCADE,
    CONSTRAINT fk_comentario_usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    CONSTRAINT fk_comentario_destinatario FOREIGN KEY (id_destinatario) REFERENCES Usuario(id_usuario) ON DELETE SET NULL,
    
    INDEX idx_comentario_tarea (id_tarea),
    INDEX idx_comentario_usuario (id_usuario),
    INDEX idx_comentario_destinatario (id_destinatario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TRIGGERS
-- ============================================================================

DELIMITER $$

-- Trigger: Mantener id_usuario_asignado sincronizado al INSERTAR
CREATE TRIGGER after_tarea_usuario_insert
AFTER INSERT ON Tarea_Usuario
FOR EACH ROW
BEGIN
    DECLARE primer_usuario INT;
    
    SELECT id_usuario INTO primer_usuario
    FROM Tarea_Usuario
    WHERE id_tarea = NEW.id_tarea AND estado = 1
    ORDER BY fecha_asignacion ASC
  LIMIT 1;
    
    UPDATE Tareas
    SET id_usuario_asignado = primer_usuario
    WHERE id_tarea = NEW.id_tarea;
END$$

-- Trigger: Mantener id_usuario_asignado sincronizado al ACTUALIZAR
CREATE TRIGGER after_tarea_usuario_update
AFTER UPDATE ON Tarea_Usuario
FOR EACH ROW
BEGIN
    DECLARE primer_usuario INT;
    
    IF OLD.estado != NEW.estado THEN
        SELECT id_usuario INTO primer_usuario
   FROM Tarea_Usuario
        WHERE id_tarea = NEW.id_tarea AND estado = 1
        ORDER BY fecha_asignacion ASC
        LIMIT 1;
        
UPDATE Tareas
  SET id_usuario_asignado = IFNULL(primer_usuario, NULL)
        WHERE id_tarea = NEW.id_tarea;
    END IF;
END$$

-- Trigger: Actualizar cuando se elimina una asignación
CREATE TRIGGER after_tarea_usuario_delete
AFTER DELETE ON Tarea_Usuario
FOR EACH ROW
BEGIN
    DECLARE primer_usuario INT;
    
    SELECT id_usuario INTO primer_usuario
    FROM Tarea_Usuario
    WHERE id_tarea = OLD.id_tarea AND estado = 1
    ORDER BY fecha_asignacion ASC
    LIMIT 1;
    
    UPDATE Tareas
    SET id_usuario_asignado = IFNULL(primer_usuario, NULL)
    WHERE id_tarea = OLD.id_tarea;
END$$

DELIMITER ;

-- ============================================================================
-- DATOS DE PRUEBA - USUARIOS (CON CONTRASEÑAS HASHEADAS PBKDF2)
-- ============================================================================

INSERT INTO Usuario (nombres, primer_apellido, segundo_apellido, nombre_usuario, email, contraseña, rol, estado)
VALUES 
('Admin', 'Sistema', NULL, 'admin', 'admin@sgpt.com', 'PBKDF2:100000:TOyLH4UjYqErl+jMwtoA6g==:ubvXDdiou2WvqLcGMYMEL5RI8IHJS+zcCkADhIuO7x4=', 'SuperAdmin', 1),
('Carlos', 'Ramirez', 'Torres', 'jefeproy', 'jefeProy@sgpt.com', 'PBKDF2:100000:uEDMK2mzBD5QfzMz1OBmWQ==:roZkO9R5SbpjNYzU6HDmdAucBQCU93c/KGY0wYQBCsk=', 'JefeDeProyecto', 1),
('Ana Maria', 'Gomez', 'Lopez', 'empleado1', 'empleado1@sgpt.com', 'PBKDF2:100000:UgvKZb0Fj2MnGmkkF3jsCA==:Ge6J4DdpXYuEAXEScXrUCfHGF2/VWGo+/wfmihU22yY=', 'Empleado', 1),
('Luis Fernando', 'Martinez', 'Diaz', 'empleado2', 'empleado2@sgpt.com', 'PBKDF2:100000:vxMWOif7sGobUYFcIImyyw==:Ci3y2g59hThTNfq553V+7R+oNNdu7ecwmJUlM8lba7Q=', 'Empleado', 1),
('Sofia', 'Rodriguez', 'Sanchez', 'empleado3', 'empleado3@sgpt.com', 'PBKDF2:100000:MPJvhGu34fYU3ZT3U1/xpg==:ReZknkpXjkrhVBKt58CshduyQvyp2z4PdFiHJ+/Ylys=', 'Empleado', 1);

-- ============================================================================
-- DATOS DE PRUEBA - PROYECTOS
-- ============================================================================

INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin, estado)
VALUES 
('Sistema de Gestion', 'Desarrollo del sistema de gestion de proyectos', NOW(), DATE_ADD(NOW(), INTERVAL 3 MONTH), 1),
('Portal Web Corporativo', 'Creacion del portal web para clientes', NOW(), DATE_ADD(NOW(), INTERVAL 2 MONTH), 1),
('App Movil', 'Desarrollo de aplicacion movil multiplataforma', NOW(), DATE_ADD(NOW(), INTERVAL 4 MONTH), 1);

-- ============================================================================
-- DATOS DE PRUEBA - ASIGNACIÓN PROYECTO-USUARIO
-- ============================================================================

-- Proyecto 1: Sistema de Gestion ? Ana, Luis, Sofia
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (1, 3, 1);
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (1, 4, 1);
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (1, 5, 1);

-- Proyecto 2: Portal Web ? Luis, Sofia
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (2, 4, 1);
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (2, 5, 1);

-- Proyecto 3: App Movil ? Sofia, Ana
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (3, 5, 1);
INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, estado) VALUES (3, 3, 1);

-- ============================================================================
-- DATOS DE PRUEBA - TAREAS
-- ============================================================================

INSERT INTO Tareas (titulo, descripcion, prioridad, id_proyecto, status, estado)
VALUES 
-- Proyecto 1: Sistema de Gestión
('Configurar Base de Datos', 'Configurar la base de datos MySQL con las nuevas estructuras', 'Alta', 1, 'EnProgreso', 1),
('Disenar Interfaz de Usuario', 'Crear mockups y prototipos de la interfaz', 'Media', 1, 'SinIniciar', 1),
('Implementar Dashboard', 'Crear componentes del dashboard principal - TAREA COMPARTIDA', 'Alta', 1, 'EnProgreso', 1),
('Documentacion Tecnica', 'Documentar arquitectura completa - TAREA COMPARTIDA', 'Media', 1, 'SinIniciar', 1),

-- Proyecto 2: Portal Web
('Implementar API REST', 'Desarrollar endpoints para el backend', 'Alta', 2, 'EnProgreso', 1),
('Configurar Servidor', 'Setup del servidor de produccion', 'Media', 2, 'Completada', 1),
('Sistema de Autenticacion', 'Implementar JWT y pagina de login - TAREA COMPARTIDA', 'Alta', 2, 'EnProgreso', 1),

-- Proyecto 3: App Móvil
('Desarrollar Modulo de Login', 'Implementar autenticacion en app movil', 'Alta', 3, 'EnProgreso', 1),
('Testing de Componentes', 'Realizar pruebas unitarias', 'Baja', 3, 'SinIniciar', 1),
('Disenar Flujo de Usuario', 'Crear wireframes y flujos - TAREA COMPARTIDA', 'Media', 3, 'SinIniciar', 1),

-- Tarea sin asignar
('Revision de Seguridad', 'Auditar codigo y configuraciones', 'Alta', 1, 'SinIniciar', 1);

-- ============================================================================
-- DATOS DE PRUEBA - ASIGNACIÓN TAREA-USUARIO
-- ============================================================================

-- Tarea 1: Solo Ana Maria
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (1, 3);

-- Tarea 2: Solo Ana Maria
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (2, 3);

-- Tarea 3: Ana Maria + Luis Fernando (COMPARTIDA)
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (3, 3);
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (3, 4);

-- Tarea 4: Ana Maria + Luis + Sofia (TRIPLE COLABORACIÓN)
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (4, 3);
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (4, 4);
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (4, 5);

-- Tarea 5: Solo Luis Fernando
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (5, 4);

-- Tarea 6: Solo Luis Fernando
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (6, 4);

-- Tarea 7: Luis Fernando + Sofia (COMPARTIDA)
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (7, 4);
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (7, 5);

-- Tarea 8: Solo Sofia
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (8, 5);

-- Tarea 9: Solo Sofia
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (9, 5);

-- Tarea 10: Sofia + Ana Maria (COMPARTIDA)
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (10, 5);
INSERT INTO Tarea_Usuario (id_tarea, id_usuario) VALUES (10, 3);

-- ============================================================================
-- DATOS DE PRUEBA - COMENTARIOS
-- ============================================================================

INSERT INTO Comentario (contenido, id_tarea, id_usuario, id_destinatario, estado)
VALUES 
-- Jefe comenta a empleados específicos
('Iniciando la configuracion de la base de datos con los nuevos campos.', 1, 2, 3, 1),
('Necesito referencias del diseño para empezar los mockups.', 2, 2, 3, 1),
('Revisar la documentacion de la API antes de implementar.', 5, 2, 4, 1),
('Excelente trabajo con el servidor.', 6, 2, 4, 1),
('Implementar JWT para la autenticacion.', 8, 2, 5, 1),

-- Comentarios de colaboración entre empleados EN LA MISMA TAREA
('Luis, ya tengo los componentes base listos para el dashboard.', 3, 3, 4, 1),
('Ana, perfecto, empiezo la integracion con el backend.', 3, 4, 3, 1),
('Equipo, dividamos la documentacion por modulos.', 4, 2, 3, 1),
('Sofia, el backend de auth esta listo, coordina tu parte.', 7, 4, 5, 1),
('Luis, ajustando el formulario de login.', 7, 5, 4, 1),
('Sofia, revisa mi avance en los wireframes.', 10, 3, 5, 1),
('Ana, excelentes wireframes, empiezo implementacion.', 10, 5, 3, 1);

-- ============================================================================
-- VISTAS
-- ============================================================================

-- Vista: Tareas con todos sus usuarios asignados
CREATE VIEW vista_tareas_usuarios AS
SELECT 
    t.id_tarea,
    t.titulo,
    p.nombre AS proyecto,
    GROUP_CONCAT(CONCAT(u.nombres, ' ', u.primer_apellido) SEPARATOR ', ') AS usuarios_asignados,
  COUNT(tu.id_usuario) AS cantidad_usuarios,
    t.status,
    t.prioridad
FROM Tareas t
LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
LEFT JOIN Tarea_Usuario tu ON t.id_tarea = tu.id_tarea AND tu.estado = 1
LEFT JOIN Usuario u ON tu.id_usuario = u.id_usuario
WHERE t.estado = 1
GROUP BY t.id_tarea, t.titulo, p.nombre, t.status, t.prioridad
ORDER BY t.id_tarea;

-- Vista: Asignaciones detalladas
CREATE VIEW vista_asignaciones_detalladas AS
SELECT 
    tu.id,
    t.titulo AS tarea,
    CONCAT(u.nombres, ' ', u.primer_apellido) AS usuario,
    u.rol,
    p.nombre AS proyecto,
    tu.fecha_asignacion,
    t.status
FROM Tarea_Usuario tu
INNER JOIN Tareas t ON tu.id_tarea = t.id_tarea
INNER JOIN Usuario u ON tu.id_usuario = u.id_usuario
LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
WHERE tu.estado = 1 AND t.estado = 1
ORDER BY tu.fecha_asignacion DESC;

-- Vista: Comentarios con información completa
CREATE VIEW vista_comentarios_completos AS
SELECT 
    c.id_comentario,
    c.contenido,
    c.fecha,
    CONCAT(u_autor.nombres, ' ', u_autor.primer_apellido) AS autor,
    u_autor.rol AS rol_autor,
    CONCAT(u_dest.nombres, ' ', u_dest.primer_apellido) AS destinatario,
    u_dest.rol AS rol_destinatario,
    t.titulo AS tarea,
    p.nombre AS proyecto,
    GROUP_CONCAT(DISTINCT CONCAT(u2.nombres, ' ', u2.primer_apellido) SEPARATOR ', ') AS usuarios_en_tarea
FROM Comentario c
INNER JOIN Usuario u_autor ON c.id_usuario = u_autor.id_usuario
LEFT JOIN Usuario u_dest ON c.id_destinatario = u_dest.id_usuario
INNER JOIN Tareas t ON c.id_tarea = t.id_tarea
LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
LEFT JOIN Tarea_Usuario tu ON t.id_tarea = tu.id_tarea AND tu.estado = 1
LEFT JOIN Usuario u2 ON tu.id_usuario = u2.id_usuario
WHERE c.estado = 1
GROUP BY c.id_comentario, c.contenido, c.fecha, u_autor.nombres, u_autor.primer_apellido, u_autor.rol, 
         u_dest.nombres, u_dest.primer_apellido, u_dest.rol, t.titulo, p.nombre
ORDER BY c.fecha DESC;

-- Vista: Proyectos con usuarios asignados
CREATE VIEW vista_proyectos_usuarios AS
SELECT 
    p.id_proyecto,
  p.nombre AS proyecto,
    p.descripcion,
    GROUP_CONCAT(CONCAT(u.nombres, ' ', u.primer_apellido) SEPARATOR ', ') AS usuarios_asignados,
    COUNT(pu.id_usuario) AS cantidad_usuarios
FROM Proyecto p
LEFT JOIN Proyecto_Usuario pu ON p.id_proyecto = pu.id_proyecto AND pu.estado = 1
LEFT JOIN Usuario u ON pu.id_usuario = u.id_usuario
WHERE p.estado = 1
GROUP BY p.id_proyecto, p.nombre, p.descripcion
ORDER BY p.nombre;

-- ============================================================================
-- PROCEDIMIENTOS ALMACENADOS - GESTIÓN DE TAREAS
-- ============================================================================

DELIMITER $$

-- Asignar múltiples usuarios a una tarea
CREATE PROCEDURE sp_asignar_usuarios_a_tarea(
    IN p_id_tarea INT,
    IN p_ids_usuarios VARCHAR(500)
)
BEGIN
  DECLARE v_usuario_id INT;
    DECLARE v_pos INT;
    DECLARE v_ids VARCHAR(500);
    
    UPDATE Tarea_Usuario 
    SET estado = 0 
WHERE id_tarea = p_id_tarea;
    
    SET v_ids = CONCAT(p_ids_usuarios, ',');
    
    WHILE LENGTH(v_ids) > 0 DO
        SET v_pos = LOCATE(',', v_ids);
    SET v_usuario_id = CAST(SUBSTRING(v_ids, 1, v_pos - 1) AS UNSIGNED);
    
        INSERT INTO Tarea_Usuario (id_tarea, id_usuario, estado)
        VALUES (p_id_tarea, v_usuario_id, 1)
        ON DUPLICATE KEY UPDATE estado = 1, fecha_asignacion = NOW();
    
        SET v_ids = SUBSTRING(v_ids, v_pos + 1);
    END WHILE;
END$$

-- Obtener usuarios asignados a una tarea
CREATE PROCEDURE sp_obtener_usuarios_tarea(
    IN p_id_tarea INT
)
BEGIN
    SELECT 
   u.id_usuario,
    CONCAT(u.nombres, ' ', u.primer_apellido, 
               IFNULL(CONCAT(' ', u.segundo_apellido), '')) AS nombre_completo,
        u.rol,
        tu.fecha_asignacion
    FROM Tarea_Usuario tu
    INNER JOIN Usuario u ON tu.id_usuario = u.id_usuario
    WHERE tu.id_tarea = p_id_tarea AND tu.estado = 1
    ORDER BY tu.fecha_asignacion;
END$$

-- Obtener tareas compartidas entre dos usuarios
CREATE PROCEDURE sp_tareas_compartidas(
    IN p_id_usuario1 INT,
    IN p_id_usuario2 INT
)
BEGIN
    SELECT DISTINCT
        t.id_tarea,
   t.titulo,
     t.descripcion,
        p.nombre AS proyecto
    FROM Tareas t
    INNER JOIN Tarea_Usuario tu1 ON t.id_tarea = tu1.id_tarea
    INNER JOIN Tarea_Usuario tu2 ON t.id_tarea = tu2.id_tarea
    LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
    WHERE tu1.id_usuario = p_id_usuario1 
      AND tu2.id_usuario = p_id_usuario2
 AND tu1.estado = 1 
      AND tu2.estado = 1
      AND t.estado = 1
    ORDER BY t.id_tarea;
END$$

-- Obtener comentarios dirigidos a un usuario
CREATE PROCEDURE sp_comentarios_para_usuario(
    IN p_id_usuario INT
)
BEGIN
    SELECT 
        c.id_comentario,
        c.contenido,
        c.fecha,
        CONCAT(u_autor.nombres, ' ', u_autor.primer_apellido) AS autor,
        t.titulo AS tarea,
 p.nombre AS proyecto
    FROM Comentario c
    INNER JOIN Usuario u_autor ON c.id_usuario = u_autor.id_usuario
    INNER JOIN Tareas t ON c.id_tarea = t.id_tarea
    LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
    WHERE c.id_destinatario = p_id_usuario 
      AND c.estado = 1
    ORDER BY c.fecha DESC;
END$$

DELIMITER ;

-- ============================================================================
-- ? PROCEDIMIENTOS ALMACENADOS - GESTIÓN DE EMPLEADOS EN PROYECTOS
-- ============================================================================

DELIMITER $$

-- ? 1. ASIGNAR USUARIO A PROYECTO
DROP PROCEDURE IF EXISTS sp_asignar_usuario_proyecto$$

CREATE PROCEDURE sp_asignar_usuario_proyecto(
    IN p_id_proyecto INT,
    IN p_id_usuario INT
)
BEGIN
    DECLARE v_count INT;
    
  -- Verificar si la asignación ya existe
    SELECT COUNT(*) INTO v_count
    FROM Proyecto_Usuario
    WHERE id_proyecto = p_id_proyecto 
      AND id_usuario = p_id_usuario;
    
    -- Si no existe, insertar
    IF v_count = 0 THEN
        INSERT INTO Proyecto_Usuario (id_proyecto, id_usuario, fecha_asignacion, estado)
        VALUES (p_id_proyecto, p_id_usuario, NOW(), 1);
     
        SELECT 'Usuario asignado exitosamente' AS Mensaje;
    ELSE
        -- Si existe pero estaba desactivado, reactivar
        UPDATE Proyecto_Usuario
        SET estado = 1, fecha_asignacion = NOW()
        WHERE id_proyecto = p_id_proyecto 
          AND id_usuario = p_id_usuario;
        
        SELECT 'El usuario ya esta asignado a este proyecto' AS Mensaje;
    END IF;
END$$

-- ? 2. DESASIGNAR USUARIO DE PROYECTO
DROP PROCEDURE IF EXISTS sp_desasignar_usuario_proyecto$$

CREATE PROCEDURE sp_desasignar_usuario_proyecto(
    IN p_id_proyecto INT,
    IN p_id_usuario INT
)
BEGIN
    -- Eliminar la asignación del proyecto
    DELETE FROM Proyecto_Usuario
    WHERE id_proyecto = p_id_proyecto 
      AND id_usuario = p_id_usuario;
    
    -- Desactivar tareas del usuario en ese proyecto (Tarea_Usuario)
    UPDATE Tarea_Usuario tu
    INNER JOIN Tareas t ON tu.id_tarea = t.id_tarea
    SET tu.estado = 0
    WHERE t.id_proyecto = p_id_proyecto 
      AND tu.id_usuario = p_id_usuario;
  
    -- Limpiar id_usuario_asignado si es el único asignado
UPDATE Tareas
    SET id_usuario_asignado = NULL
  WHERE id_proyecto = p_id_proyecto 
      AND id_usuario_asignado = p_id_usuario
    AND NOT EXISTS (
          SELECT 1 FROM Tarea_Usuario tu2 
          WHERE tu2.id_tarea = Tareas.id_tarea 
       AND tu2.estado = 1 
 AND tu2.id_usuario != p_id_usuario
      );
    
    SELECT 'Usuario desasignado y sus tareas removidas' AS Mensaje;
END$$

-- ? 3. OBTENER USUARIOS ASIGNADOS A UN PROYECTO
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
    INNER JOIN Proyecto_Usuario pu ON u.id_usuario = pu.id_usuario
    WHERE pu.id_proyecto = p_id_proyecto
      AND pu.estado = 1
      AND u.estado = 1
    ORDER BY u.primer_apellido, u.nombres;
END$$

DELIMITER ;

-- ============================================================================
-- VERIFICACIONES Y CONSULTAS ÚTILES
-- ============================================================================

-- Verificar usuarios creados con contraseñas hasheadas
SELECT 
    nombre_usuario AS Usuario,
    email AS Email,
    rol AS Rol,
  CASE WHEN contraseña LIKE 'PBKDF2:%' THEN 'Hasheado' ELSE 'Texto Plano' END AS Estado_Seguridad,
    LEFT(contraseña, 30) AS Hash_Preview
FROM Usuario
WHERE estado = 1
ORDER BY 
    CASE 
        WHEN rol = 'SuperAdmin' THEN 1
     WHEN rol = 'JefeDeProyecto' THEN 2
        ELSE 3
    END,
    nombre_usuario;

-- Verificar proyectos creados
SELECT id_proyecto, nombre, descripcion, estado FROM Proyecto;

-- Verificar tareas creadas
SELECT id_tarea, titulo, prioridad, status, id_proyecto FROM Tareas;

-- Verificar asignaciones proyecto-usuario
SELECT * FROM vista_proyectos_usuarios;

-- Verificar asignaciones tarea-usuario
SELECT * FROM vista_tareas_usuarios;

-- ============================================================================
-- MENSAJE FINAL
-- ============================================================================

SELECT CONCAT(
    'Base de datos creada exitosamente. ',
    'Tablas: ', (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'gestion_proyectos'), ', ',
    'Vistas: ', (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'gestion_proyectos'), ', ',
    'Procedimientos: ', (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'gestion_proyectos' AND routine_type = 'PROCEDURE'), ', ',
    'Usuarios: ', (SELECT COUNT(*) FROM Usuario WHERE estado = 1)
) AS Status;

/*
??????????????????????????????????????????????????????????????????
?          CREDENCIALES DEL SISTEMA      ?
??????????????????????????????????????????????????????????????????

1. SUPER ADMINISTRADOR
   ? Email:      admin@sgpt.com
   ? Usuario:    admin
   ? Contraseña: Admin123!

2. JEFE DE PROYECTO
   ? Email:    jefeProy@sgpt.com
 ? Usuario:    jefeproy
   ? Contraseña: Jefe123!

3. EMPLEADO 1 - Ana Maria Gomez
   ? Email:  empleado1@sgpt.com
   ? Usuario:    empleado1
? Contraseña: Empleado1!
   
   Proyectos asignados:
   - Sistema de Gestion (Proyecto 1)
   - App Movil (Proyecto 3)

4. EMPLEADO 2 - Luis Fernando Martinez
   ? Email:      empleado2@sgpt.com
   ? Usuario:    empleado2
   ? Contraseña: Empleado2!
   
   Proyectos asignados:
   - Sistema de Gestion (Proyecto 1)
   - Portal Web Corporativo (Proyecto 2)

5. EMPLEADO 3 - Sofia Rodriguez
   ? Email:      empleado3@sgpt.com
   ? Usuario:    empleado3
   ? Contraseña: Empleado3!
   
   Proyectos asignados:
   - Todos los proyectos

??????????????????????????????????????????????????????????????????
?   ? INCLUYE PROCEDIMIENTOS PARA ASIGNAR EMPLEADOS       ?
??????????????????????????????????????????????????????????????????

- sp_asignar_usuario_proyecto(id_proyecto, id_usuario)
- sp_desasignar_usuario_proyecto(id_proyecto, id_usuario)
- sp_obtener_usuarios_proyecto(id_proyecto)

*/
