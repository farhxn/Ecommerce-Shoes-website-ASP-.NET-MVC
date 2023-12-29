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
createdAt varchar(250) null,
updatedAt varchar(250) null,
); 

create table Products(
P_id int primary key identity(1,1),
P_name varchar(250) null,
P_price varchar(250) null,
P_dprice varchar(250) null,
P_cat varchar(250) null,
P_img1 varchar(250) null,
P_img2 varchar(250) null,
P_img3 varchar(250) null,
P_stock varchar(250) null,
P_desc varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null,
);

create table Review(
R_id int primary key identity(1,1),
R_name varchar(250) null,
R_mail varchar(250) null,
R_review varchar(550) null,
R_userId varchar(250) null,
R_PID varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null
);


create table  Orders(
O_id int primary key identity(1,1),
U_id varchar(250) null,
P_id varchar(250) null,
P_qty varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null
);

create table  Billing(
B_id int primary key identity(1,1),
U_id varchar(250) null,
P_id varchar(250) null,
P_qty varchar(250) null,
O_status varchar(250) null,
O_id varchar(250) null,
B_first varchar(250) null,
B_last varchar(250) null,
B_number varchar(250) null,
B_city varchar(250) null,
B_address varchar(500) null,
B_zip varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null
);

create table  Contact(
C_id int primary key identity(1,1),
C_name varchar(250) null,
C_mail varchar(250) null,
C_subject varchar(250) null,
C_status varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null
)


create table  Users(
U_id int primary key identity(1,1),
U_name varchar(250) null,
U_mail varchar(250) null,
U_password varchar(250) null,
U_role varchar(250) null,
createdAt varchar(250) null,
updatedAt varchar(250) null
)

drop table Category
drop table Slider
drop table Orders



truncate table Orders
truncate table Billing
truncate table Users



SELECT * FROM Slider
SELECT * FROM Category
SELECT * FROM Products
SELECT * FROM Review
SELECT * FROM Orders
SELECT * FROM Contact

SELECT * FROM Billing
SELECT * FROM Users


select Review.*,Products.* from Review  join Products on  Review.R_PID = Products.P_id
delete from Users where U_id = 2

update Orders set P_qty  = '3' where O_id = 1
update Billing set isReview  = null where B_id = 2
update Users set U_role  = '1' where U_id = 1

EXEC sp_RENAME 'Orders.P_name', 'U_id', 'COLUMN';

ALTER TABLE Contact
ADD C_message varchar(2500) null;


ALTER TABLE billing
DROP COLUMN isReview;