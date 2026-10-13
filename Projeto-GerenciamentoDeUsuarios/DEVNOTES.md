---

CREATE DATABASE GEARU;

USE GEARU;

SELECT * FROM log; #Mostrar  o banco de dados
SELECT * FROM users; #Mostrar  o banco de dados 

CREATE TABLE `GEARU`.`users` (`id` INT NOT NULL AUTO_INCREMENT ,
`email` VARCHAR(255) UNIQUE NOT NULL ,
`user` VARCHAR(255) UNIQUE NOT NULL , 
`name` VARCHAR(255) NOT NULL,
`password` VARCHAR(255) NOT NULL ,
`imageValue` INT NOT NULL DEFAULT 0,
`IsAdmin` TINYINT(1) NOT NULL DEFAULT 0,
`IsBanned` TINYINT(1) NOT NULL DEFAULT 0,
`PasswordTry` INT not null default 0,
`lastTryLogin` DATETIME null,
`lastLogin` DATETIME null, 
`LockedTime` DATETIME null,
`IsLocked` TINYINT(1) not null default 0,
`lastActive` DATETIME null,
`lastEdit` DATETIME null default current_timestamp,
`creationDate` DATETIME null default current_timestamp,
`systemConfig` VARCHAR (300) not null default "DarkMode:false SaveLogin:false",
`AdminLevel` int not null default 0,
PRIMARY KEY (`id`))
ENGINE = InnoDB;

CREATE TABLE `GEARU`.`log` (`id` INT NOT NULL AUTO_INCREMENT ,
`location` VARCHAR(255) NOT NULL ,
`adminUser` VARCHAR(255) NOT NULL , 
`adminID` int not null default 0,
`userChanged` varchar(255) not null,
`userChangedID` int not null default 0,
`time` DATETIME NOT null default current_timestamp,
`changes` VARCHAR(300) NOT NULL DEFAULT "Error: action not registered",
PRIMARY KEY (`id`));

CREATE TABLE `GEARU`.`eventlog` (`id` INT NOT NULL AUTO_INCREMENT ,
`location` VARCHAR(255) NOT NULL ,
`userChanged` varchar(255) not null,
`userChangedID` int not null default 0,
`time` DATETIME NOT null default current_timestamp,
`event` VARCHAR(300) NOT NULL DEFAULT "Error: action not registered",
`result` VARCHAR(300) NOT NULL DEFAULT "Error: action not registered",
PRIMARY KEY (`id`));


TRUNCATE TABLE users; #Limpa a tabela de usuários
TRUNCATE TABLE log;

drop table users;
drop table log;

INSERT INTO log (location, user, time, action) values ("SQL", "SYSTEM", now(), "Test");

set time_zone = "+00:00";

---