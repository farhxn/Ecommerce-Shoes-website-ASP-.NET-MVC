CREATE DATABASE shoes
USE shoes

CREATE TABLE Slider(
S_id int primary key identity (1,1),
S_img varchar(250) null,
S_name varchar(250) null,
S_desc varchar(500) null,
S_status varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null,
);

create table Category(
C_id int primary key identity(1,1),
C_name varchar(250) null,
); 


SELECT * FROM Slider