
CREATE DATABASE SECURITY_DB
GO

USE [SECURITY_DB];
go
CREATE TABLE UserMaster
(
  UserID INT PRIMARY KEY identity(1,1),
  UserName VARCHAR(50) unique,
  UserPassword VARCHAR(50),
  UserRoles VARCHAR(500),
  UserEmailID VARCHAR(100),
)
alter table UserMaster add  city nvarchar(20) ;
alter table UserMaster add  SSN nvarchar(20) ;
alter table UserMaster add  DriverLicense nvarchar(20) ;
alter table UserMaster add  FullName nvarchar(50)  ;
alter table UserMaster add  CarLicense nvarchar(20)  ;
alter table UserMaster add imagePath nvarchar(300); 
alter table UserMaster add PhoneNumber nvarchar(20) ;


INSERT INTO UserMaster VALUES('ab', '123456', 'Admin', 'a.1@g.com','sohag','562561','123','56')
INSERT INTO UserMaster VALUES( 'ab1', 'abcdef', 'User', 'a.2@g.com','assiut','562562','12356','5625')
INSERT INTO UserMaster VALUES( 'ab2', 'asdasd', 'SuperAdmin', 'a.3@g.com','assiut','562563','12399','56651')
INSERT INTO UserMaster VALUES( 'ab3', 'azz562', 'Admin, User', 'a.4@g.com','assiut','562564','56744','789895421')
GO
drop table trip
create table trip(
ID int primary key identity (1,1),
FromCity nvarchar(100)not null,
ToCity nvarchar(100) not null ,
PlaceToMeet nvarchar(30),
DateOfTrip date not null,
TimeOfTrip time not null ,

DriverId int references UserMaster(UserID),

);
Go


create table Reservation(
ID int primary key identity (1,1),
TravellerId int references UserMaster(UserID),
TripId int 

);
Go
select * from trip
select *from UserMaster