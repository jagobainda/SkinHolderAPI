DROP DATABASE IF EXISTS skinholderlog;

CREATE DATABASE skinholderlog CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE skinholderlog;

-- Creación de la tabla users
CREATE TABLE users (
    userid INTEGER PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(30) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla logtype
CREATE TABLE logtype (
    logtypeid INTEGER PRIMARY KEY AUTO_INCREMENT,
    typename VARCHAR(7) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla logplace
CREATE TABLE logplace (
    logplaceid INTEGER PRIMARY KEY AUTO_INCREMENT,
    placename VARCHAR(7) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla logger
CREATE TABLE logger (
    loggerid BIGINT PRIMARY KEY AUTO_INCREMENT,
    logdescription VARCHAR(5000) NOT NULL,
    logdatetime DATETIME NOT NULL,
    logtypeid INTEGER NOT NULL,
    logplaceid INTEGER NOT NULL,
    userid INTEGER NOT NULL,
    CONSTRAINT fk_logtypeid_logtype FOREIGN KEY (logtypeid) REFERENCES logtype(logtypeid),
    CONSTRAINT fk_logplaceid_logplace FOREIGN KEY (logplaceid) REFERENCES logplace(logplaceid),
    CONSTRAINT fk_userid_logger FOREIGN KEY (userid) REFERENCES users(userid)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO logtype (typename) VALUES
    ('INFO'),
    ('WARNING'),
    ('ERROR');

INSERT INTO logplace (placename) VALUES
    ('WPF'),
    ('ANDROID'),
    ('API');

DROP DATABASE IF EXISTS skinholderdb;

CREATE DATABASE skinholderdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE skinholderdb;

-- Creación de la tabla users
CREATE TABLE users (
    userid INTEGER PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(30) NOT NULL,
    passwordhash VARCHAR(128) NOT NULL,
    isactive BOOLEAN NOT NULL DEFAULT TRUE,
    isbanned BOOLEAN NOT NULL DEFAULT FALSE,
    createdat DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla items
CREATE TABLE items (
    itemid INTEGER PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    hashnamesteam VARCHAR(300) NOT NULL,
    gamerpaynombre VARCHAR(300) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla useritems
CREATE TABLE useritems (
    useritemid BIGINT PRIMARY KEY AUTO_INCREMENT,
    cantidad INTEGER NOT NULL,
    preciomediocompra DECIMAL(10, 2) NOT NULL,
    itemid INTEGER NOT NULL,
    userid INTEGER NOT NULL,
    CONSTRAINT fk_itemid_useritems FOREIGN KEY (itemid) REFERENCES items(itemid),
    CONSTRAINT fk_userid_useritems FOREIGN KEY (userid) REFERENCES users(userid)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla registrotType
CREATE TABLE registrotype (
    registrotypeid INTEGER PRIMARY KEY AUTO_INCREMENT,
    type VARCHAR(8) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla registros
CREATE TABLE registros (
    registroid BIGINT PRIMARY KEY AUTO_INCREMENT,
    fechahora DATETIME NOT NULL,
    totalsteam DECIMAL(10, 2) NOT NULL,
    totalgamerpay DECIMAL(10, 2) NOT NULL,
    userid INTEGER NOT NULL,
    registrotypeid INTEGER NOT NULL,
    CONSTRAINT fk_userid_registros FOREIGN KEY (userid) REFERENCES users(userid),
    CONSTRAINT fk_userid_registrotype FOREIGN KEY (registrotypeid) REFERENCES registrotType(registrotTypeid)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla itemprecio
CREATE TABLE itemprecio (
    itemprecioid BIGINT PRIMARY KEY AUTO_INCREMENT,
    preciosteam DECIMAL(10, 2) NOT NULL,
    preciogamerpay DECIMAL(10, 2) NOT NULL,
    useritemid BIGINT NOT NULL,
    registroid BIGINT NOT NULL,
    CONSTRAINT fk_useritemid_itemprecio FOREIGN KEY (useritemid) REFERENCES useritems(useritemid),
    CONSTRAINT fk_registroid_itemprecio FOREIGN KEY (registroid) REFERENCES registros(registroid)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO registrotType (type) VALUES
    ('STEAM'),
    ('GAMERPAY'),
    ('ALL');

USE skinholderlog;

DELIMITER $$

CREATE PROCEDURE deletelogsantiguos()
BEGIN
    DELETE FROM logger WHERE logdatetime < DATE_SUB(CURDATE(), INTERVAL 6 MONTH);
END $$

DELIMITER ;
