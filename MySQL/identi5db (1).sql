-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主機： 127.0.0.1
-- 產生時間： 2024-09-20 14:15:11
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

--
-- 傾印資料表的資料 `outfits`
--

INSERT INTO `outfits` (`Player_id`, `Player_colors`, `Player_outfits`) VALUES
(102, '[{\"r\":0.911041796207428,\"g\":0.9717227816581726,\"b\":1,\"a\":1},{\"r\":0.4225468039512634,\"g\":0.33120405673980713,\"b\":0.6105301380157471,\"a\":1}]', '[\"Hair2\",\"Eyes5\",\"Mouth4\",\"Sleeve(R)1\",\"Clothes1\",\"Shoe(R)3\",\"Leg(R)3\",\"Shoe(L)3\",\"Leg(L)3\",\"Sleeve(L)1\"]'),
(103, '[{\"r\":1,\"g\":0.8887485861778259,\"b\":0.8887485861778259,\"a\":1},{\"r\":1,\"g\":0.4566934108734131,\"b\":0.4566934108734131,\"a\":1}]', '[\"Hair1\",\"Eyes4\",\"Mouth2\",\"Sleeve(R)1\",\"Clothes1\",\"Shoe(R)1\",\"Leg(R)1\",\"Shoe(L)1\",\"Leg(L)1\",\"Sleeve(L)1\"]'),
(104, '[{\"r\":1,\"g\":1,\"b\":1,\"a\":1},{\"r\":1,\"g\":1,\"b\":1,\"a\":1}]', '[\"Hair2\",\"Eyes4\",\"Mouth2\",\"Sleeve(R)1\",\"Clothes1\",\"Shoe(R)1\",\"Leg(R)1\",\"Shoe(L)1\",\"Leg(L)1\",\"Sleeve(L)1\"]'),
(105, '[{\"r\":1,\"g\":0.9459248185157776,\"b\":0.8545889854431152,\"a\":1},{\"r\":0,\"g\":0,\"b\":0,\"a\":1}]', '[\"Hair1\",\"Eyes4\",\"Mouth1\",\"Sleeve(R)4\",\"Clothes4\",\"Shoe(R)4\",\"Leg(R)3\",\"Shoe(L)4\",\"Leg(L)3\",\"Sleeve(L)4\"]'),
(106, '[{\"r\":1,\"g\":1,\"b\":1,\"a\":1},{\"r\":1,\"g\":1,\"b\":1,\"a\":1}]', '[\"Hair1\",\"Eyes4\",\"Mouth1\",\"Sleeve(R)4\",\"Clothes4\",\"Shoe(R)4\",\"Leg(R)3\",\"Shoe(L)4\",\"Leg(L)3\",\"Sleeve(L)4\"]'),
(107, '[{\"r\":1,\"g\":1,\"b\":1,\"a\":1},{\"r\":1,\"g\":1,\"b\":1,\"a\":1}]', '[\"Hair1\",\"Eyes4\",\"Mouth1\",\"Sleeve(R)4\",\"Clothes4\",\"Shoe(R)4\",\"Leg(R)3\",\"Shoe(L)4\",\"Leg(L)3\",\"Sleeve(L)4\"]'),
(108, '[{\"r\":1,\"g\":0.9321023225784302,\"b\":0.9321023225784302,\"a\":1},{\"r\":1,\"g\":0.560036301612854,\"b\":0.560036301612854,\"a\":1}]', '[\"Hair5\",\"Eyes2\",\"Mouth4\",\"Sleeve(R)2\",\"Clothes2\",\"Shoe(R)3\",\"Leg(R)5\",\"Shoe(L)3\",\"Leg(L)5\",\"Sleeve(L)2\"]'),
(109, '[{\"r\":1,\"g\":1,\"b\":1,\"a\":1},{\"r\":0.2876049876213074,\"g\":0.09343215823173523,\"b\":0.09343215823173523,\"a\":1}]', '[\"Hair5\",\"Eyes4\",\"Mouth1\",\"Sleeve(R)1\",\"Clothes1\",\"Shoe(R)3\",\"Leg(R)3\",\"Shoe(L)3\",\"Leg(L)3\",\"Sleeve(L)1\"]'),
(110, '[{\"r\":1,\"g\":0.911041796207428,\"b\":0.911041796207428,\"a\":1},{\"r\":0.36833637952804565,\"g\":0.20498892664909363,\"b\":0.20498892664909363,\"a\":1}]', '[\"Hair4\",\"Eyes3\",\"Mouth2\",\"Sleeve(R)2\",\"Clothes2\",\"Shoe(R)2\",\"Leg(R)4\",\"Shoe(L)2\",\"Leg(L)4\",\"Sleeve(L)2\"]'),
(111, '[{\"r\":1,\"g\":1,\"b\":1,\"a\":1},{\"r\":1,\"g\":1,\"b\":1,\"a\":1}]', '[\"Hair1\",\"Eyes4\",\"Mouth1\",\"Sleeve(R)4\",\"Clothes4\",\"Shoe(R)4\",\"Leg(R)3\",\"Shoe(L)4\",\"Leg(L)3\",\"Sleeve(L)4\"]'),
(112, '[{\"r\":1,\"g\":0.9346308708190918,\"b\":0.9346308708190918,\"a\":1},{\"r\":0.3379865288734436,\"g\":0.08361194282770157,\"b\":0.08361194282770157,\"a\":1}]', '[\"Hair1\",\"Eyes4\",\"Mouth1\",\"Sleeve(R)4\",\"Clothes4\",\"Shoe(R)4\",\"Leg(R)3\",\"Shoe(L)4\",\"Leg(L)3\",\"Sleeve(L)4\"]');

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
  `collisionMapNo` int(11) NOT NULL,
  `bulletOnCollisions` int(11) NOT NULL,
  `remainHP` varchar(45) NOT NULL,
  `remainBullet` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- 傾印資料表的資料 `output_data`
--

INSERT INTO `output_data` (`id`, `player_id`, `isFinished`, `signUptime`, `playTime`, `outfitTime`, `oufitChangedNo`, `placeholderNo`, `buildingVisit`, `manualTime`, `failGameNo`, `deathNo`, `killNo`, `organizeNo`, `fullNo`, `zombieInShelteNo`, `surviveTime`, `contribution`, `messageSent`, `teamCreated`, `quitTeamNo`, `totalVoiceDetectionDuration`, `joinTeamNo`, `giftNo`, `rankClikedNo`, `bulletOnLiving`, `bulletOnPlayer`, `interactNo`, `collisionMapNo`, `bulletOnCollisions`, `remainHP`, `remainBullet`) VALUES
(1, 102, 0, '2024-09-17 08:32:44', '2024-09-17 11:25:57', 27.0246, 0, 0, '[3]', 0, 0, 2, 2, 32, 0, 0, 202.156, 2, 0, 8, 7, 0, 0, 0, 26, 18, 0, 9, 32, 0, '[]', '[]'),
(2, 103, 0, '2024-09-17 09:47:41', '2024-09-17 09:47:50', 8.44687, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(3, 104, 0, '2024-09-17 09:58:10', '2024-09-17 09:58:11', 1.32008, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(4, 105, 0, '2024-09-17 10:53:56', '2024-09-17 11:20:41', 21.6981, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(5, 106, 1, '2024-09-17 11:31:55', '2024-09-18 15:21:34', 1.08844, 0, 0, '[4,2,3]', 0, 0, 1, 3, 3, 2, 0, 79.9688, 0, 1, 1, 0, 0, 0, 0, 2, 0, 0, 7, 16, 3, '[80]', '[]'),
(6, 107, 0, '2024-09-17 12:15:01', '2024-09-18 04:26:20', 1.14033, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(7, 108, 0, '2024-09-19 11:02:11', '2024-09-19 11:04:33', 23.9962, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(8, 109, 0, '2024-09-19 13:09:49', '2024-09-19 13:10:01', 12.1027, 0, 0, NULL, 2.91064, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(9, 110, 0, '2024-09-20 00:35:33', '2024-09-20 14:04:18', 12.1549, 0, 2, '[1]', 0, 2, 3, 2, 0, 0, 89, 199.969, 0, 0, 1, 1, 0, 0, 0, 0, 0, 39, 1, 0, 0, '[]', '[]'),
(10, 111, 0, '2024-09-20 04:40:44', '2024-09-20 11:51:39', 1.88066, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', ''),
(11, 112, 0, '2024-09-20 11:43:10', '2024-09-20 11:46:02', 12.5995, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', '');

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
-- 傾印資料表的資料 `player`
--

INSERT INTO `player` (`Player_id`, `Player_name`, `Player_password`) VALUES
(102, '測試帳號1', '12345678'),
(103, '4444', '12345678'),
(104, 'rrrr', 'rrrrrrrrr'),
(105, '測試帳號2', '12345678'),
(106, 'aaaa', 'aaaaaaaa'),
(107, 'qqqq', 'qqqqqqqq'),
(108, 'aaaaa', '**********'),
(109, 'aaaaaaaaaaaa', '********'),
(110, 'aaaaaaaa', '********'),
(111, 'qqqqqqqq', '********'),
(112, 'wwwwwwww', '********');

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
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=113;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `output_data`
--
ALTER TABLE `output_data`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `player`
--
ALTER TABLE `player`
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=113;

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
