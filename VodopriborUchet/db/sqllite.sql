-- Script Date: 17.10.2014 12:26  - ErikEJ.SqlCeScripting version 3.5.2.40
-- Database information:
-- Locale Identifier: 1049
-- Encryption Mode: 
-- Case Sensitive: False
-- Database: C:\Users\User\Documents\Visual Studio 2013\Projects\VodopriborUchet\VodopriborUchet\db\db_sqlce.sdf
-- ServerVersion: 4.0.8876.1
-- DatabaseSize: 192 KB
-- Created: 15.10.2014 11:32

-- User Table information:
-- Number of tables: 12
-- category_pay_type: 0 row(s)
-- counters: 0 row(s)
-- counters_type: 0 row(s)
-- net: 5 row(s)
-- object_type: 0 row(s)
-- objects_place: 0 row(s)
-- owners: 0 row(s)
-- recorder_type: 0 row(s)
-- resource_type: 5 row(s)
-- units: 0 row(s)
-- users: 0 row(s)
-- users_type: 0 row(s)

SELECT 1;
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [users_type] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NOT NULL
, CONSTRAINT [PK__users_type__00000000000000CA] PRIMARY KEY ([id])
);
CREATE TABLE [users] (
  [id] INTEGER NOT NULL
, [login] nvarchar(1) NOT NULL
, [name] nvarchar(1) NOT NULL
, [surname] nvarchar(1) NOT NULL
, [user_type_id] int NOT NULL
, [passwd] nvarchar(1) NOT NULL
, CONSTRAINT [PK__users__00000000000000C0] PRIMARY KEY ([id])
);
CREATE TABLE [units] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NOT NULL
, CONSTRAINT [PK__units__0000000000000046] PRIMARY KEY ([id])
);
CREATE TABLE [resource_type] (
  [id] INTEGER NOT NULL
, [name] nvarchar(100) NOT NULL
, [consumptoin_rate] float NOT NULL
, [units_id] int NOT NULL
, [units_cost] money NULL
, CONSTRAINT [PK__resource_type__000000000000003C] PRIMARY KEY ([id])
);
CREATE TABLE [recorder_type] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NOT NULL
, [comment] nvarchar(1) NULL
, CONSTRAINT [PK__recorder_type__0000000000000090] PRIMARY KEY ([id])
);
CREATE TABLE [owners] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NOT NULL
, [surname] nvarchar(1) NOT NULL
, [tel] nvarchar(1) NULL
, [category_pay_id] int NOT NULL
, [objects_place_id] int NULL
, CONSTRAINT [PK__owners__00000000000000A2] PRIMARY KEY ([id])
);
CREATE TABLE [object_type] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NOT NULL
, [comment] nvarchar(1) NULL
, CONSTRAINT [PK__object_type__000000000000001E] PRIMARY KEY ([id])
);
CREATE TABLE [net] (
  [id] INTEGER NOT NULL
, [name] nvarchar(100) NOT NULL
, [commnet] nvarchar(100) NULL
, CONSTRAINT [PK__net__0000000000000084] PRIMARY KEY ([id])
);
CREATE TABLE [objects_place] (
  [id] INTEGER NOT NULL
, [object_type_id] int NULL
, [objects_place_id] int NULL
, [net_id] int NULL
, [name] nvarchar(1) NOT NULL
, [kod_BTI] nvarchar(1) NULL
, [flat_id] nvarchar(1) NULL
, [comment] ntext NULL
, CONSTRAINT [PK__objects_place__0000000000000012] PRIMARY KEY ([id])
, FOREIGN KEY ([net_id]) REFERENCES [net] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE TABLE [counters_type] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NOT NULL
, [resource_type_id] int NOT NULL
, [units_per_impulse] float NULL
, CONSTRAINT [PK__counters_type__000000000000002C] PRIMARY KEY ([id])
);
CREATE TABLE [counters] (
  [id] INTEGER NOT NULL
, [date_start] datetime NULL
, [date_end] datetime NULL
, [chnannel] int NULL
, [objects_place_id] int NULL
, [owner_id] int NULL
, [date_act] datetime NULL
, [value_act] float NULL
, [net_id] int NULL
, [serial_amspi] nvarchar(25) NULL
, [serial_r1] nvarchar(25) NULL
, [serial_r2] nvarchar(25) NULL
, [date_calibration_r1] datetime NULL
, [date_calibration_r2] datetime NULL
, [date_init_calibration_r2] datetime NULL
, [date_init_calibration_r1] datetime NULL
, [date_calibration_amspi] datetime NULL
, [diameter_r1] int NULL
, [diameter_r2] int NULL
, CONSTRAINT [PK__counters__0000000000000078] PRIMARY KEY ([id])
);
CREATE TABLE [category_pay_type] (
  [id] INTEGER NOT NULL
, [name] nvarchar(1) NULL
, [pay_rate] float NOT NULL
, CONSTRAINT [PK__category_pay_type__00000000000000AE] PRIMARY KEY ([id])
);
INSERT INTO [resource_type] ([id],[name],[consumptoin_rate],[units_id],[units_cost]) VALUES (1,'q',10,1,25);
INSERT INTO [resource_type] ([id],[name],[consumptoin_rate],[units_id],[units_cost]) VALUES (2,'test_rs',11,3,25);
INSERT INTO [resource_type] ([id],[name],[consumptoin_rate],[units_id],[units_cost]) VALUES (3,'test_rs1',111,31,215);
INSERT INTO [resource_type] ([id],[name],[consumptoin_rate],[units_id],[units_cost]) VALUES (4,'test_rs1',111,31,215);
INSERT INTO [resource_type] ([id],[name],[consumptoin_rate],[units_id],[units_cost]) VALUES (5,'test_rs1',111,31,215);
INSERT INTO [net] ([id],[name],[commnet]) VALUES (8,'test1','test2');
INSERT INTO [net] ([id],[name],[commnet]) VALUES (9,'test1','test2');
INSERT INTO [net] ([id],[name],[commnet]) VALUES (10,'test1','test2');
INSERT INTO [net] ([id],[name],[commnet]) VALUES (11,'test1','test2');
INSERT INTO [net] ([id],[name],[commnet]) VALUES (12,'test1','test2');
COMMIT;

