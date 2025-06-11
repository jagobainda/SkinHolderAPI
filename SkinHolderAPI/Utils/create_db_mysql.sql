DROP DATABASE IF EXISTS SkinHolderLog;

CREATE DATABASE SkinHolderLog CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE SkinHolderLog;

-- Creación de la tabla Users
CREATE TABLE Users(
    UserID INTEGER PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(30) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla LogType
CREATE TABLE LogType(
    LogTypeID INTEGER PRIMARY KEY AUTO_INCREMENT,
    TypeName VARCHAR(7) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla LogPlace
CREATE TABLE LogPlace(
    LogPlaceID INTEGER PRIMARY KEY AUTO_INCREMENT,
    PlaceName VARCHAR(7) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;


-- Creación de la tabla Logger
CREATE TABLE Logger(
    LoggerId BIGINT PRIMARY KEY AUTO_INCREMENT,
    LogDescription VARCHAR(5000) NOT NULL,
    LogDateTime DATETIME NOT NULL,
    LogTypeID INTEGER NOT NULL,
    LogPlaceID INTEGER NOT NULL,
    UserID INTEGER NOT NULL,
    CONSTRAINT fk_LogTypeID_LogType FOREIGN KEY (LogTypeID) REFERENCES LogType(LogTypeID),
    CONSTRAINT fk_LogPlaceID_LogPlace FOREIGN KEY (LogPlaceID) REFERENCES LogPlace(LogPlaceID),
    CONSTRAINT fk_UserID_Logger FOREIGN KEY (UserID) REFERENCES Users(UserID)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO LogType (TypeName) VALUES
    ('INFO'),
    ('WARNING'),
    ('ERROR');

INSERT INTO LogPlace (PlaceName) VALUES
    ('WPF'),
    ('ANDROID'),
    ('API');

DROP DATABASE IF EXISTS SkinHolderDB;

CREATE DATABASE SkinHolderDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE SkinHolderDB;

-- Creación de la tabla Users
CREATE TABLE Users (
    UserID INTEGER PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(30) NOT NULL,
    PasswordHash VARCHAR(128) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE, 
    IsBanned BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla Items
CREATE TABLE Items(
    ItemID INTEGER PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    HashNameSteam VARCHAR(300) NOT NULL,
    GamerPayNombre VARCHAR(300) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla UserItems
CREATE TABLE UserItems(
    UserItemID BIGINT PRIMARY KEY AUTO_INCREMENT,
    Cantidad INTEGER NOT NULL,
    PrecioMedioCompra DECIMAL(10, 2) NOT NULL,
    ItemID INTEGER NOT NULL,
    UserID INTEGER NOT NULL,
    CONSTRAINT fk_ItemID_UserItems FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
    CONSTRAINT fk_UserID_UserItems FOREIGN KEY (UserID) REFERENCES Users(UserID)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla RegistroType
CREATE TABLE RegistroType(
    RegistroTypeID INTEGER PRIMARY KEY AUTO_INCREMENT,
    Type VARCHAR(8) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla Registros
CREATE TABLE Registros(
    RegistroID BIGINT PRIMARY KEY AUTO_INCREMENT,
    FechaHora DATETIME NOT NULL,
    TotalSteam DECIMAL(10, 2) NOT NULL,
    TotalGamerPay DECIMAL(10, 2) NOT NULL,
    UserID INTEGER NOT NULL,
    RegistroTypeID INTEGER NOT NULL,
    CONSTRAINT fk_UserID_Registros FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT fk_UserID_RegistroType FOREIGN KEY (RegistroTypeID) REFERENCES RegistroType(RegistroTypeID)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Creación de la tabla ItemPrecio
CREATE TABLE ItemPrecio(
    ItemPrecioID BIGINT PRIMARY KEY AUTO_INCREMENT,
    PrecioSteam DECIMAL(10, 2) NOT NULL,
    PrecioGamerPay DECIMAL(10, 2) NOT NULL,
    UserItemID BIGINT NOT NULL,
    RegistroID BIGINT NOT NULL,
    CONSTRAINT fk_UserItemID_ItemPrecio FOREIGN KEY (UserItemID) REFERENCES UserItems(UserItemID),
    CONSTRAINT fk_RegistroID_ItemPrecio FOREIGN KEY (RegistroID) REFERENCES Registros(RegistroID)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO RegistroType (Type) VALUES
    ('STEAM'),
    ('GAMERPAY'),
    ('ALL');

USE SkinHolderLog;

DELIMITER $$

CREATE PROCEDURE DeleteLogsAntiguos()
BEGIN
    DELETE FROM Logger WHERE LogDateTime < DATE_SUB(CURDATE(), INTERVAL 6 MONTH);
END $$

DELIMITER ;