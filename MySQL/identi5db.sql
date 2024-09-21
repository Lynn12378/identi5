-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主機： 127.0.0.1
-- 產生時間： 2024-09-21 14:16:52
-- 伺服器版本： 10.4.32-MariaDB
-- PHP 版本： 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 資料庫： `identi5db`
--

-- --------------------------------------------------------

--
-- 資料表結構 `bfi_responses`
--

CREATE TABLE `bfi_responses` (
  `id` int(11) NOT NULL,
  `player_id` int(11) NOT NULL,
  `responses` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `feedback` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `outfits`
--

CREATE TABLE `outfits` (
  `Player_id` int(11) NOT NULL,
  `Player_colors` varchar(500) DEFAULT NULL,
  `Player_outfits` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `output_data`
--

CREATE TABLE `output_data` (
  `id` int(11) NOT NULL,
  `player_id` int(11) NOT NULL,
  `isFinished` tinyint(1) NOT NULL DEFAULT 0,
  `signUptime` datetime NOT NULL,
  `playTime` datetime NOT NULL,
  `outfitTime` float NOT NULL,
  `oufitChangedNo` int(11) NOT NULL,
  `placeholderNo` int(11) NOT NULL,
  `buildingVisit` varchar(45) DEFAULT NULL,
  `manualTime` float NOT NULL,
  `failGameNo` int(11) NOT NULL,
  `deathNo` int(11) NOT NULL,
  `killNo` int(11) NOT NULL,
  `organizeNo` int(11) NOT NULL,
  `fullNo` int(11) NOT NULL,
  `zombieInShelteNo` int(11) NOT NULL,
  `surviveTime` float NOT NULL,
  `contribution` int(11) NOT NULL,
  `messageSent` int(11) NOT NULL,
  `teamCreated` int(11) NOT NULL,
  `quitTeamNo` int(11) NOT NULL,
  `totalVoiceDetectionDuration` double NOT NULL,
  `joinTeamNo` int(11) NOT NULL,
  `giftNo` int(11) NOT NULL,
  `rankClikedNo` int(11) NOT NULL,
  `bulletOnLiving` int(11) NOT NULL,
  `bulletOnPlayer` int(11) NOT NULL,
  `interactNo` int(11) NOT NULL,
  `timeInShelter` float NOT NULL,
  `collisionMapNo` int(11) NOT NULL,
  `bulletOnCollisions` int(11) NOT NULL,
  `remainHP` varchar(45) NOT NULL,
  `remainBullet` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `player`
--

CREATE TABLE `player` (
  `Player_id` int(11) NOT NULL,
  `Player_name` varchar(45) NOT NULL,
  `Player_password` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- 已傾印資料表的索引
--

--
-- 資料表索引 `bfi_responses`
--
ALTER TABLE `bfi_responses`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_player_id` (`player_id`);

--
-- 資料表索引 `outfits`
--
ALTER TABLE `outfits`
  ADD PRIMARY KEY (`Player_id`);

--
-- 資料表索引 `output_data`
--
ALTER TABLE `output_data`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `player_id` (`player_id`);

--
-- 資料表索引 `player`
--
ALTER TABLE `player`
  ADD PRIMARY KEY (`Player_id`);

--
-- 在傾印的資料表使用自動遞增(AUTO_INCREMENT)
--

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `bfi_responses`
--
ALTER TABLE `bfi_responses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `outfits`
--
ALTER TABLE `outfits`
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=120;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `output_data`
--
ALTER TABLE `output_data`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `player`
--
ALTER TABLE `player`
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=120;

--
-- 已傾印資料表的限制式
--

--
-- 資料表的限制式 `bfi_responses`
--
ALTER TABLE `bfi_responses`
  ADD CONSTRAINT `fk_player_id` FOREIGN KEY (`player_id`) REFERENCES `player` (`Player_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
