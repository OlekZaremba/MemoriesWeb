-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Maj 26, 2025 at 02:21 PM
-- Wersja serwera: 10.4.32-MariaDB
-- Wersja PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `memories_net`
--

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `class`
--

CREATE TABLE `class` (
  `idclass` int(11) NOT NULL,
  `class_name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `class`
--

INSERT INTO `class` (`idclass`, `class_name`) VALUES
(1, 'Geografia 1'),
(2, 'Historia 1');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `grades`
--

CREATE TABLE `grades` (
  `idgrades` int(11) NOT NULL,
  `grade` double NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `users_idstudent` int(11) NOT NULL,
  `users_idteacher` int(11) NOT NULL,
  `class_idclass` int(11) NOT NULL,
  `type` varchar(50) DEFAULT 'Sprawdzian',
  `issue_date` date NOT NULL DEFAULT curdate(),
  `notified` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `grades`
--

INSERT INTO `grades` (`idgrades`, `grade`, `description`, `users_idstudent`, `users_idteacher`, `class_idclass`, `type`, `issue_date`, `notified`) VALUES
(1, 5, 'Sprawdzian z mapy', 1, 2, 1, 'Sprawdzian', '2025-05-26', 0);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `group_members`
--

CREATE TABLE `group_members` (
  `idgroup_members` int(11) NOT NULL,
  `user_group_id` int(11) NOT NULL,
  `users_idusers` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `group_members`
--

INSERT INTO `group_members` (`idgroup_members`, `user_group_id`, `users_idusers`) VALUES
(1, 1, 1),
(5, 1, 2),
(7, 1, 3),
(8, 1, 4);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `group_members_has_class`
--

CREATE TABLE `group_members_has_class` (
  `id` int(11) NOT NULL,
  `group_members_idgroup_members` int(11) NOT NULL,
  `class_idclass` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `group_members_has_class`
--

INSERT INTO `group_members_has_class` (`id`, `group_members_idgroup_members`, `class_idclass`) VALUES
(1, 1, 1),
(7, 1, 2),
(5, 5, 1),
(6, 7, 1);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `schedule`
--

CREATE TABLE `schedule` (
  `idschedule` int(11) NOT NULL,
  `lesson_date` date NOT NULL,
  `start_time` time NOT NULL,
  `end_time` time NOT NULL,
  `group_members_has_class_id` int(11) NOT NULL,
  `generated` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `schedule`
--

INSERT INTO `schedule` (`idschedule`, `lesson_date`, `start_time`, `end_time`, `group_members_has_class_id`, `generated`) VALUES
(1, '2025-05-27', '08:00:00', '08:45:00', 1, 0);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `sensitive_data`
--

CREATE TABLE `sensitive_data` (
  `idsensitive_data` int(11) NOT NULL,
  `login` varchar(255) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `password` varchar(256) NOT NULL,
  `users_idusers` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sensitive_data`
--

INSERT INTO `sensitive_data` (`idsensitive_data`, `login`, `email`, `password`, `users_idusers`) VALUES
(1, 'student', 'anna@student.com', 'AQAAAAIAAYagAAAAEOJMmQpDHjjOZlJM5Ei/cTREyajn48tY/ZeEmnBTDeFp3bS7BgLYgr91voYW2hyIaQ==', 1),
(2, 'teacher', 'tomasz@teacher.com', 'AQAAAAIAAYagAAAAEEvodAO1C+u6od4BVI4EzJ1K/ei4E5Qlq+kVHHw96Vu/13mGlLDlBv4NQiZaGcgihA==', 2),
(3, 'admin', 'barbara@admin.com', 'AQAAAAIAAYagAAAAENbPCiM7ungdNXQ8HFGjFPf9NPbQHM/Jrru5J4FxytNYF4gN4z4Zq4wtDdidnaj7Fg==', 3),
(4, 'ruanrashmir', 'ruanrashmir@gmail.com', 'AQAAAAIAAYagAAAAEEzSHM9EirrY0b2wAB0vQJxjVDdeeD/iCl9u0S4/eG6U/Sc2sdPJCBcNJlFYok23Ew==', 4);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `users`
--

CREATE TABLE `users` (
  `idusers` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `surname` varchar(255) NOT NULL,
  `role` varchar(1) DEFAULT NULL,
  `image` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`idusers`, `name`, `surname`, `role`, `image`) VALUES
(1, 'Anna', 'Kowalska', 'S', NULL),
(2, 'Tomasz', 'Nowak', 'T', NULL),
(3, 'Barbara', 'Wiśniewska', 'A', NULL),
(4, 'ruan', 'rashmir', 'S', NULL);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `user_group`
--

CREATE TABLE `user_group` (
  `id` int(11) NOT NULL,
  `group_name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user_group`
--

INSERT INTO `user_group` (`id`, `group_name`) VALUES
(1, 'Klasa 1');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indeksy dla zrzutów tabel
--

--
-- Indeksy dla tabeli `class`
--
ALTER TABLE `class`
  ADD PRIMARY KEY (`idclass`);

--
-- Indeksy dla tabeli `grades`
--
ALTER TABLE `grades`
  ADD PRIMARY KEY (`idgrades`),
  ADD KEY `fk_grades_student_idx` (`users_idstudent`),
  ADD KEY `fk_grades_teacher_idx` (`users_idteacher`),
  ADD KEY `fk_grades_class_idx` (`class_idclass`);

--
-- Indeksy dla tabeli `group_members`
--
ALTER TABLE `group_members`
  ADD PRIMARY KEY (`idgroup_members`),
  ADD KEY `fk_group_members_user_group_idx` (`user_group_id`),
  ADD KEY `fk_group_members_users_idx` (`users_idusers`);

--
-- Indeksy dla tabeli `group_members_has_class`
--
ALTER TABLE `group_members_has_class`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ux_gmc_group_member_class` (`group_members_idgroup_members`,`class_idclass`),
  ADD KEY `fk_gmc_group_member_idx` (`group_members_idgroup_members`),
  ADD KEY `fk_gmc_class_idx` (`class_idclass`);

--
-- Indeksy dla tabeli `schedule`
--
ALTER TABLE `schedule`
  ADD PRIMARY KEY (`idschedule`),
  ADD KEY `idx_schedule_gmhc` (`group_members_has_class_id`);

--
-- Indeksy dla tabeli `sensitive_data`
--
ALTER TABLE `sensitive_data`
  ADD PRIMARY KEY (`idsensitive_data`),
  ADD KEY `fk_sensitive_data_users_idx` (`users_idusers`);

--
-- Indeksy dla tabeli `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`idusers`);

--
-- Indeksy dla tabeli `user_group`
--
ALTER TABLE `user_group`
  ADD PRIMARY KEY (`id`);

--
-- Indeksy dla tabeli `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `class`
--
ALTER TABLE `class`
  MODIFY `idclass` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `grades`
--
ALTER TABLE `grades`
  MODIFY `idgrades` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `group_members`
--
ALTER TABLE `group_members`
  MODIFY `idgroup_members` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `group_members_has_class`
--
ALTER TABLE `group_members_has_class`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `schedule`
--
ALTER TABLE `schedule`
  MODIFY `idschedule` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `sensitive_data`
--
ALTER TABLE `sensitive_data`
  MODIFY `idsensitive_data` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `idusers` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `user_group`
--
ALTER TABLE `user_group`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `grades`
--
ALTER TABLE `grades`
  ADD CONSTRAINT `fk_grades_class` FOREIGN KEY (`class_idclass`) REFERENCES `class` (`idclass`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_grades_student` FOREIGN KEY (`users_idstudent`) REFERENCES `users` (`idusers`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_grades_teacher` FOREIGN KEY (`users_idteacher`) REFERENCES `users` (`idusers`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `group_members`
--
ALTER TABLE `group_members`
  ADD CONSTRAINT `fk_group_members_user_group` FOREIGN KEY (`user_group_id`) REFERENCES `user_group` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_group_members_users` FOREIGN KEY (`users_idusers`) REFERENCES `users` (`idusers`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `group_members_has_class`
--
ALTER TABLE `group_members_has_class`
  ADD CONSTRAINT `fk_gmc_class` FOREIGN KEY (`class_idclass`) REFERENCES `class` (`idclass`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_gmc_group_member` FOREIGN KEY (`group_members_idgroup_members`) REFERENCES `group_members` (`idgroup_members`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `schedule`
--
ALTER TABLE `schedule`
  ADD CONSTRAINT `fk_schedule_gmhc` FOREIGN KEY (`group_members_has_class_id`) REFERENCES `group_members_has_class` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `sensitive_data`
--
ALTER TABLE `sensitive_data`
  ADD CONSTRAINT `fk_sensitive_data_users` FOREIGN KEY (`users_idusers`) REFERENCES `users` (`idusers`) ON DELETE NO ACTION ON UPDATE NO ACTION;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
