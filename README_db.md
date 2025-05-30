# 📘 README – Baza danych `memories_net`

## 📌 Opis
Baza danych systemu **Memories++** służy do zarządzania użytkownikami (uczniami, nauczycielami, administratorami), klasami, przedmiotami, ocenami oraz planem zajęć.

Zawiera dane testowe umożliwiające logowanie i pełną funkcjonalność aplikacji.

---

## 🧪 Domyślni użytkownicy

| Login    | Hasło    | Rola             | Imię     | Nazwisko     | Email                     |
|----------|----------|------------------|----------|--------------|---------------------------|
| student  | test123  | Uczeń (S)        | Anna     | Kowalska     | anna@student.com          |
| teacher  | test123  | Nauczyciel (T)   | Tomasz   | Nowak        | tomasz@teacher.com        |
| admin    | test123  | Administrator (A)| Barbara  | Wiśniewska   | barbara@admin.com         |

Hasła zostały zahashowane zgodnie z ASP.NET Identity (`PasswordHasher`).

---

## 🗃️ Struktura danych

### 👤 `users`
- `idusers`, `name`, `surname`, `role (S/T/A)`, `image`

### 🔐 `sensitive_data`
- `login`, `email`, `password` (hash), `users_idusers`

### 🧑‍🏫 `user_group`
- Klasy użytkowników (np. Klasa 1, Klasa 2 itd.)

### 👥 `group_members`
- Przypisanie użytkowników (uczniów i nauczycieli) do klas

### 📚 `class`
- Lista dostępnych przedmiotów

### 🧩 `group_members_has_class`
- Przypisanie nauczyciela do przedmiotu w konkretnej klasie

### 🗓️ `schedule`
- Harmonogram lekcji z datą, godziną i przypisaniem do klasy/przedmiotu

### 📝 `grades`
- Oceny: wystawione przez nauczyciela uczniowi za konkretny przedmiot z typem oceny i datą

---

## 🧪 Testowanie API

### ✅ Logowanie
**Endpoint:** `POST /api/auth/login`

**Przykład:**
```json
{
  "login": "student",
  "password": "test123"
}
```

---

## 🔧 Uwagi techniczne

- Baza danych: MariaDB 10.4.32 (kompatybilna z MySQL)
- Relacje i klucze obce są zdefiniowane
- Email użytkownika jest wymagany do resetowania hasła
- Jeden uczeń przypisany może być tylko do jednej klasy
- Nauczyciel może być przypisany do wielu klas i przedmiotów

---

## 📎 Dodatkowe informacje

- Zdjęcia profilowe są zapisane jako ciągi Base64 w kolumnie `users.image`
- Klasa i przedmiot są wiązane przez relację: nauczyciel → klasa → przedmiot
- Tabela `group_members_has_class` łączy nauczyciela z konkretnym przedmiotem w konkretnej klasie
