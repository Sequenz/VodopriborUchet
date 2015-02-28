



-- ---
-- Globals
-- ---

-- SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
-- SET FOREIGN_KEY_CHECKS=0;

-- ---
-- Table 'category_pay_type'
-- категории ставки оплаты
-- ---

DROP TABLE IF EXISTS `category_pay_type`;
		
CREATE TABLE `category_pay_type` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `pay_rate` DOUBLE NOT NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'категории ставки оплаты';

-- ---
-- Table 'counters_type'
-- типы счетчиков
-- ---

DROP TABLE IF EXISTS `counters_type`;
		
CREATE TABLE `counters_type` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NULL DEFAULT NULL,
  `resource_id` INTEGER NULL DEFAULT NULL,
  `units_per_impulse` INTEGER NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'типы счетчиков';

-- ---
-- Table 'objects_place'
-- Список обьектов
-- ---

DROP TABLE IF EXISTS `objects_place`;
		
CREATE TABLE `objects_place` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `object_type_id` INTEGER NULL DEFAULT NULL,
  `net_id` INTEGER NULL DEFAULT NULL,
  `object_place_id` INTEGER NULL DEFAULT NULL,
  `name` MEDIUMTEXT NULL DEFAULT NULL,
  `kod_BTI` MEDIUMTEXT NULL DEFAULT NULL,
  `flod_ID` MEDIUMTEXT NULL DEFAULT NULL,
  `comment` MEDIUMTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
KEY (`id`),
KEY ()
) COMMENT 'Список обьектов';

-- ---
-- Table 'object_type'
-- типы обьектов
-- ---

DROP TABLE IF EXISTS `object_type`;
		
CREATE TABLE `object_type` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `comment` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  PRIMARY KEY (`id`)
) COMMENT 'типы обьектов';

-- ---
-- Table 'resource_type'
-- типы энерго ресурсов
-- ---

DROP TABLE IF EXISTS `resource_type`;
		
CREATE TABLE `resource_type` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `consumption_rate` DOUBLE NOT NULL DEFAULT NULL,
  `units_id` INTEGER NOT NULL DEFAULT NULL,
  `units_cost` DECIMAL NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'типы энерго ресурсов';

-- ---
-- Table 'units'
-- единицы измерений
-- ---

DROP TABLE IF EXISTS `units`;
		
CREATE TABLE `units` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  PRIMARY KEY (`id`)
) COMMENT 'единицы измерений';

-- ---
-- Table 'users'
-- пользователи
-- ---

DROP TABLE IF EXISTS `users`;
		
CREATE TABLE `users` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `login` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `surname` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `isadmin` BINARY NULL DEFAULT NULL,
  `passwd` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  PRIMARY KEY (`id`),
KEY ()
) COMMENT 'пользователи';

-- ---
-- Table 'recorder'
-- Хрень которая через ЖПРС передает данные с счетчиков
-- ---

DROP TABLE IF EXISTS `recorder`;
		
CREATE TABLE `recorder` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `serial` INTEGER NOT NULL DEFAULT NULL,
  `objects_place_id` INTEGER NOT NULL DEFAULT NULL,
  `recorder_type` INTEGER NOT NULL DEFAULT NULL,
  `comment` MEDIUMTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'Хрень которая через ЖПРС передает данные с счетчиков';

-- ---
-- Table 'recorder_type'
-- типы регистраторов
-- ---

DROP TABLE IF EXISTS `recorder_type`;
		
CREATE TABLE `recorder_type` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `chanels_max` INTEGER NOT NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'типы регистраторов';

-- ---
-- Table 'counters'
-- счетчики
-- ---

DROP TABLE IF EXISTS `counters`;
		
CREATE TABLE `counters` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `date_start` DATE NOT NULL DEFAULT 'NULL',
  `date_end` DATE NULL DEFAULT NULL,
  `channel` INTEGER NULL DEFAULT NULL,
  `objects_place_id` INTEGER NOT NULL DEFAULT NULL,
  `recorder_id` INTEGER NOT NULL DEFAULT NULL,
  `serial` INTEGER NULL DEFAULT NULL,
  `owner_id` INTEGER NULL DEFAULT NULL,
  `date_act` DATE NULL DEFAULT NULL,
  `value_act` DOUBLE NOT NULL DEFAULT NULL,
  `poverka` BINARY NULL DEFAULT NULL,
  `comment` MEDIUMTEXT NULL DEFAULT NULL,
  `net_id` INTEGER NULL DEFAULT NULL,
  `counter_type_id` INTEGER NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'счетчики';

-- ---
-- Table 'owners'
-- владелец обьекта
-- ---

DROP TABLE IF EXISTS `owners`;
		
CREATE TABLE `owners` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `surname` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `tel` MEDIUMTEXT NULL DEFAULT NULL,
  `category_pay_id` INTEGER NULL DEFAULT NULL,
  `objects_place_id` INTEGER NOT NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) COMMENT 'владелец обьекта';

-- ---
-- Table 'net'
-- 
-- ---

DROP TABLE IF EXISTS `net`;
		
CREATE TABLE `net` (
  `id` INTEGER NULL AUTO_INCREMENT DEFAULT NULL,
  `name` MEDIUMTEXT NOT NULL DEFAULT 'NULL',
  `comment` MEDIUMTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
);

-- ---
-- Foreign Keys 
-- ---

ALTER TABLE `counters_type` ADD FOREIGN KEY (resource_id) REFERENCES `resource_type` (`id`);
ALTER TABLE `objects_place` ADD FOREIGN KEY (object_type_id) REFERENCES `object_type` (`id`);
ALTER TABLE `objects_place` ADD FOREIGN KEY (net_id) REFERENCES `net` (`id`);
ALTER TABLE `objects_place` ADD FOREIGN KEY (object_place_id) REFERENCES `objects_place` (`id`);
ALTER TABLE `resource_type` ADD FOREIGN KEY (units_id) REFERENCES `units` (`id`);
ALTER TABLE `recorder` ADD FOREIGN KEY (objects_place_id) REFERENCES `objects_place` (`id`);
ALTER TABLE `recorder` ADD FOREIGN KEY (recorder_type) REFERENCES `recorder_type` (`id`);
ALTER TABLE `counters` ADD FOREIGN KEY (objects_place_id) REFERENCES `objects_place` (`id`);
ALTER TABLE `counters` ADD FOREIGN KEY (recorder_id) REFERENCES `recorder` (`id`);
ALTER TABLE `counters` ADD FOREIGN KEY (owner_id) REFERENCES `owners` (`id`);
ALTER TABLE `counters` ADD FOREIGN KEY (net_id) REFERENCES `net` (`id`);
ALTER TABLE `counters` ADD FOREIGN KEY (counter_type_id) REFERENCES `counters_type` (`id`);
ALTER TABLE `owners` ADD FOREIGN KEY (category_pay_id) REFERENCES `category_pay_type` (`id`);
ALTER TABLE `owners` ADD FOREIGN KEY (objects_place_id) REFERENCES `objects_place` (`id`);

-- ---
-- Table Properties
-- ---

-- ALTER TABLE `category_pay_type` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `counters_type` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `objects_place` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `object_type` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `resource_type` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `units` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `users` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `recorder` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `recorder_type` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `counters` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `owners` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
-- ALTER TABLE `net` ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- ---
-- Test Data
-- ---

-- INSERT INTO `category_pay_type` (`id`,`name`,`pay_rate`) VALUES
-- ('','','');
-- INSERT INTO `counters_type` (`id`,`name`,`resource_id`,`units_per_impulse`) VALUES
-- ('','','','');
-- INSERT INTO `objects_place` (`id`,`object_type_id`,`net_id`,`object_place_id`,`name`,`kod_BTI`,`flod_ID`,`comment`) VALUES
-- ('','','','','','','','');
-- INSERT INTO `object_type` (`id`,`name`,`comment`) VALUES
-- ('','','');
-- INSERT INTO `resource_type` (`id`,`name`,`consumption_rate`,`units_id`,`units_cost`) VALUES
-- ('','','','','');
-- INSERT INTO `units` (`id`,`name`) VALUES
-- ('','');
-- INSERT INTO `users` (`id`,`login`,`name`,`surname`,`isadmin`,`passwd`) VALUES
-- ('','','','','','');
-- INSERT INTO `recorder` (`id`,`serial`,`objects_place_id`,`recorder_type`,`comment`) VALUES
-- ('','','','','');
-- INSERT INTO `recorder_type` (`id`,`name`,`chanels_max`) VALUES
-- ('','','');
-- INSERT INTO `counters` (`id`,`date_start`,`date_end`,`channel`,`objects_place_id`,`recorder_id`,`serial`,`owner_id`,`date_act`,`value_act`,`poverka`,`comment`,`net_id`,`counter_type_id`) VALUES
-- ('','','','','','','','','','','','','','');
-- INSERT INTO `owners` (`id`,`name`,`surname`,`tel`,`category_pay_id`,`objects_place_id`) VALUES
-- ('','','','','','');
-- INSERT INTO `net` (`id`,`name`,`comment`) VALUES
-- ('','','');

