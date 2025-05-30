-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Maj 30, 2025 at 05:59 PM
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
(5, 'Chemia 2'),
(7, 'Fizyka 1'),
(1, 'Geografia 1'),
(2, 'Historia 1'),
(8, 'Informatyka 2'),
(6, 'Matematyka 1');

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
(1, 5, 'Sprawdzian z mapy', 1, 2, 1, 'Sprawdzian', '2025-05-26', 0),
(3, 3.5, 'jest srednio', 4, 2, 1, 'Sprawdzian', '2025-05-30', 0),
(4, 4.5, 'Brawo', 4, 2, 5, 'Kartkowka', '2025-05-30', 0),
(5, 5.5, 'Bardzo dobrze', 5, 10, 6, 'Odpowiedz', '2025-05-30', 0),
(6, 5.5, 'brawo', 9, 11, 5, 'Kartkowka', '2025-05-30', 0);

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
(8, 1, 4),
(9, 3, 5),
(10, 3, 6),
(11, 4, 7),
(14, 3, 10),
(15, 4, 11),
(16, 3, 8),
(17, 6, 11),
(18, 6, 9),
(19, 1, 12),
(20, 1, 13);

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
(8, 5, 2),
(6, 7, 1),
(18, 8, 1),
(24, 9, 6),
(21, 14, 6),
(23, 15, 6),
(22, 15, 7),
(25, 17, 5);

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
(1, '2025-05-27', '08:00:00', '08:45:00', 1, 0),
(2, '2025-05-27', '06:30:00', '07:30:00', 5, 0),
(3, '2025-05-27', '06:30:00', '07:30:00', 5, 0),
(5, '2025-05-29', '08:00:00', '09:30:00', 5, 0);

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
(4, 'ruanrashmir', 'ruanrashmir@gmail.com', 'AQAAAAIAAYagAAAAEEzSHM9EirrY0b2wAB0vQJxjVDdeeD/iCl9u0S4/eG6U/Sc2sdPJCBcNJlFYok23Ew==', 4),
(5, 'jnowicki', 'jan.nowicki@student.com', 'AQAAAAIAAYagAAAAEEr23Ycqxtb69LePnol/Jdtu33Mk2t6kyLii3YU8asd9i23e8gp5xPYhDCIEZO/dZA==', 5),
(6, 'kmazur', 'katarzyna.mazur@student.com', 'AQAAAAIAAYagAAAAEMqvCdPWGsczz1fmmO9uoWb/ZwQzeYuLpZ0C+br945HB9/8m32foSTVmnLzroQi7Kg==', 6),
(7, 'mlewandowski', 'marek.lewandowski@student.com', 'AQAAAAIAAYagAAAAECC6YnxIOZUE1SeQZvf1kLLiV3adbVZtu2Cxay3EhDIJFFDADbCyb+ozT5wH8ScV3A==', 7),
(8, 'zkaczmarek', 'zofia.kaczmarek@student.com', 'AQAAAAIAAYagAAAAEOmLySiHVXP1gLUrsGhTRbGdDNJxCJKQWz9bTsKW9USzF6hJjx62sDUmYjcGbwEOng==', 8),
(9, 'awojcik', 'aleksander.wojcik@student.com', 'AQAAAAIAAYagAAAAEJ02QTFBaIvncK3Nl5aFFJshGp5ZZ9jIk8NhwWwRS/Ld58RAgv8UGrzlfvN6EG3l2g==', 9),
(10, 'ekowal', 'elzbieta.kowal@teacher.com', 'AQAAAAIAAYagAAAAEHrdwzF2lXGCr9Dhn75gkOnmrLNbBLCkj6CzdPTexwKGwyWZBAKxNHu38UoKcAa2yg==', 10),
(11, 'pjablonski', 'pawel.jablonski@teacher.com', 'AQAAAAIAAYagAAAAEMBh4ViwWuTJygHLKRiP+pnwso8nIkoVGQnWK3bpjfn2nXykEVJ8ojV2WrBdy8T6Ng==', 11),
(13, 'mpazdzioch@student.com', 'mpazdzioch@student.com', 'AQAAAAIAAYagAAAAEFuyyiuw/Fk72ovj92W59sEqezuMYvlmUaumz+GTZaKGF5fZ/hVjvLyF51jQWE0lfA==', 13);

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
(1, 'Anna', 'Kowalska', 'S', 'data:image/jpeg;base64,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'),
(2, 'Tomasz', 'Nowak', 'T', NULL),
(3, 'Barbara', 'Wiśniewska', 'A', NULL),
(4, 'ruan', 'rashmir', 'S', NULL),
(5, 'Jan', 'Nowicki', 'S', 'data:image/jpeg;base64,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'),
(6, 'Katarzyna', 'Mazur', 'S', NULL),
(7, 'Marek', 'Lewandowski', 'S', NULL),
(8, 'Zofia', 'Kaczmarek', 'S', NULL),
(9, 'Aleksander', 'Wojcik', 'S', NULL),
(10, 'Elzbieta', 'Kowal', 'T', NULL),
(11, 'Pawel', 'Jablonski', 'T', NULL),
(12, 'Marian', 'Pazdzioch', 'S', NULL),
(13, 'Marian ', 'Pazdzioch', 'S', NULL);

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
(1, 'Klasa 1'),
(2, 'Klasa 2'),
(3, 'Klasa 3'),
(4, 'Klasa 4'),
(5, 'Klasa 5'),
(6, 'Klasa 6'),
(7, 'Klasa 7'),
(8, 'Klasa 8');

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
  ADD PRIMARY KEY (`idclass`),
  ADD UNIQUE KEY `unique_class_name` (`class_name`);

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
  MODIFY `idclass` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `grades`
--
ALTER TABLE `grades`
  MODIFY `idgrades` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `group_members`
--
ALTER TABLE `group_members`
  MODIFY `idgroup_members` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `group_members_has_class`
--
ALTER TABLE `group_members_has_class`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT for table `schedule`
--
ALTER TABLE `schedule`
  MODIFY `idschedule` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `sensitive_data`
--
ALTER TABLE `sensitive_data`
  MODIFY `idsensitive_data` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `idusers` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `user_group`
--
ALTER TABLE `user_group`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

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
