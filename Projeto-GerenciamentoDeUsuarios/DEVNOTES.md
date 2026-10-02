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
`IsAdmin` TINYINT(1) NOT NULL DEFAULT 0,
`imageValue` INT NOT NULL DEFAULT 0,
`lastLogin` DATETIME NOT null,
`lastEdit` DATETIME NOT null default current_timestamp,
`creationDate` DATETIME NOT null default current_timestamp,
`status` VARCHAR(10) not null default "Ativado",
`systemConfig` varchar (300) not null default "DarkMode:False",
PRIMARY KEY (`id`))
ENGINE = InnoDB;

CREATE TABLE `GEARU`.`log` (`id` INT NOT NULL AUTO_INCREMENT ,
`location` VARCHAR(255) NOT NULL ,
`user` VARCHAR(255) NOT NULL , 
`time` TIMESTAMP NOT NULL,
`action` VARCHAR(300) NOT NULL DEFAULT "Error: action not registered",
PRIMARY KEY (`id`));


TRUNCATE TABLE users; #Limpa a tabela de usuários
TRUNCATE TABLE log;

drop table users;

INSERT INTO log (location, user, time, action) values ("SQL", "SYSTEM", now(), "Test");

set time_zone = "+00:00";

---

Validar registro de usuários pela janela de registro