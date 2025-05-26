# 📘 README – Baza danych `memories_net`

## 📌 Opis
Baza danych systemu **Memories++** służy do zarządzania użytkownikami (uczniowie, nauczyciele, administratorzy), klasami, ocenami oraz planem zajęć.

Zawiera już dane testowe umożliwiające szybkie logowanie i pracę z systemem.

---

## 🧪 Domyślni użytkownicy

| Login    | Hasło    | Rola             | Imię     | Nazwisko     | Email                |
|----------|----------|------------------|----------|--------------|----------------------|
| student  | test123  | Uczeń (S)        | Anna     | Kowalska     | anna@student.com     |
| teacher  | test123  | Nauczyciel (T)   | Tomasz   | Nowak        | tomasz@teacher.com   |
| admin    | test123  | Administrator (A)| Barbara  | Wiśniewska   | barbara@admin.com    |

Hasła zostały **poprawnie zahashowane** zgodnie z algorytmem .NET (`ASP.NET Identity PasswordHasher`).

---

## 🗃️ Struktura danych

### 🧑‍🎓 `users`
Dane osobowe użytkownika:
- `name`, `surname` – imię i nazwisko
- `role` – `S` (student), `T` (teacher), `A` (admin)
- `image` – zdjęcie profilowe (opcjonalne)

### 🔐 `sensitive_data`
Dane logowania i hasło użytkownika (hashowane)
- `email` – adres e-mail (wymagany do odzyskiwania hasła)

### 🧑‍🏫 `user_group`
Grupy użytkowników (np. klasy)

### 👥 `group_members`
Powiązanie użytkownika z grupą

### 📚 `class`
Lista przedmiotów (np. Geografia)

### 🧩 `group_members_has_class`
Powiązanie członka grupy z przedmiotem

### 📅 `schedule`
Plan zajęć – domyślnie jedna lekcja na 27 maja 2025

### 📝 `grades`
Oceny uczniów – domyślnie jedna ocena `5` za `Sprawdzian z mapy`

---

## 🧪 Testowanie

### ✅ Logowanie (POST `/api/auth/login`)
```json
{
  "login": "student",
  "password": "test123"
}

```

---

## 🔐 Uwagi
- Baza została wyeksportowana z MySQL/MariaDB 10.4.32.
- Wszystkie relacje i indeksy są poprawnie ustawione.
- Nie jest wymagane resetowanie haseł po imporcie.
- Kolumna email została dodana do tabeli users i jest wymagana przy rejestracji i resetowaniu hasła.
