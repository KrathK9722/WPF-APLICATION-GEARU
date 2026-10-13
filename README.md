# GEARU APPLICATION — SENAI PROJECT

This application was developed while I was studying **C# and WPF** at SENAI. The project had a strict deadline of almost two weeks and a half, but I continued improving it after the teacher's evaluation to develop my programming skills, explore new features, and improve my implementation logic.

## 🛠️ TECHNOLOGIES USED IN THIS PROJECT

### 💻 Development & UI

| Logo | Technology | Description |
|:---:|---| --- |
| <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/csharp/csharp-original.svg" width="48" height="48" alt="C#"/> | **C#** | Main programming language used to develop the application's logic and functionality. |
| <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/dot-net/dot-net-original.svg" width="48" height="48" alt=".NET"/> | **WPF / XAML** | Framework and markup language used to build the Windows desktop user interface. |

---

### 🗄️ Database & Connectivity

| Logo | Technology | Description |
|:---:|---| --- |
| <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/mariadb/mariadb-original.svg" width="48" height="48" alt="MariaDB"/> | **MariaDB** | Relational database engine used to store and manage application data. |

---

### 📦 NuGet Packages

External NuGet packages used to support the application's functionality, database connectivity, and interface components.

<img src="https://github.com/user-attachments/assets/676a17e4-3584-44b8-8711-7dc4e9b5de39" alt="NuGet Packages" width="700"/>

---

### 📚 Project Information

- **Institution:** SENAI
- **Project Type:** Academic / Personal Learning
- **Language:** C#
- **UI Framework:** WPF / XAML
- **Database:** MariaDB
- **Platform:** Windows

---
### 📜 SQL SCRIPT

```sql
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


TRUNCATE TABLE users; #Limpa a tabela de usuários
TRUNCATE TABLE log;

drop table users;
drop table log;

INSERT INTO log (location, user, time, action) values ("SQL", "SYSTEM", now(), "Test");

set time_zone = "+00:00";

```
---

*This project continues to evolve as I learn new concepts and improve my software development skills.*
