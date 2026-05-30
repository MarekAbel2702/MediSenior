# MediSenior

**MediSenior** to aplikacja mobilna napisana w technologii **.NET MAUI**, wspierająca osoby starsze oraz ich opiekunów w regularnym przyjmowaniu leków. Aplikacja umożliwia tworzenie profili użytkowników, dodawanie leków, ustawianie dawek oraz godzin przypomnień. Dane użytkowników i aplikacji są obsługiwane z wykorzystaniem **Firebase**.

Projekt powstał z myślą o seniorach, którzy często muszą przyjmować kilka leków dziennie o różnych porach. MediSenior pomaga ograniczyć ryzyko pominięcia dawki, ułatwia organizację leczenia oraz pozwala opiekunowi wspierać pacjenta w codziennej terapii.

---

## Opis aplikacji

MediSenior to aplikacja służąca do przypominania o przyjmowaniu leków, szczególnie skierowana do osób starszych oraz ich opiekunów. Użytkownik może zalogować się do aplikacji, wybrać odpowiedni typ profilu, a następnie zarządzać listą leków i przypomnieniami.

Aplikacja pozwala między innymi na:

- dodawanie leków,
- określanie dawkowania,
- ustawianie godzin przyjmowania leków,
- dodawanie kilku dawek jednego leku w ciągu dnia,
- przeglądanie zaplanowanych przypomnień,
- rozróżnienie konta pacjenta i opiekuna,
- przechowywanie danych z użyciem Firebase.

MediSenior może być szczególnie przydatny dla seniorów, którzy przyjmują wiele leków o różnych porach dnia, a także dla opiekunów chcących kontrolować lub wspierać proces leczenia.

---

## Główne funkcje

### Logowanie i rejestracja

Aplikacja posiada system logowania użytkowników. Do obsługi uwierzytelniania wykorzystywany jest Firebase.

Możliwe funkcje logowania:

- rejestracja nowego użytkownika,
- logowanie istniejącego użytkownika,
- rozpoznawanie typu konta,
- bezpieczne przechowywanie danych użytkownika,
- wylogowanie z aplikacji.

### 1. Logowanie i rejestracja
<img width="396" height="850" alt="obraz" src="https://github.com/user-attachments/assets/2e3e4e3c-7752-468d-8c31-eb40be18d56f" />

### 2. Ekran pacjenta
<img width="403" height="853" alt="obraz" src="https://github.com/user-attachments/assets/6cf4e942-87bf-4bc7-a49c-d05e081e6c3a" />

### 3. Szczegóły leku
<img width="352" height="175" alt="obraz" src="https://github.com/user-attachments/assets/838da8dc-9ef6-44db-8e4d-3235b00dd257" />

### 4. Formularz dodawania leku
<img width="390" height="857" alt="obraz" src="https://github.com/user-attachments/assets/d3e6ea91-14f2-40b5-9e18-a395c529b709" />

### 5. Ekran opiekuna
<img width="393" height="869" alt="obraz" src="https://github.com/user-attachments/assets/a2426a70-7073-48c5-b218-e1e8fbd47cb8" />

### 6. Połączenie seniora z opiekunem
<img width="393" height="826" alt="obraz" src="https://github.com/user-attachments/assets/2d276789-e2c2-47cb-adb2-4aa37fbbc2f9" />

---

### Dwa typy profili

W aplikacji dostępne są dwa typy kont:

1. **Pacjent**
2. **Opiekun**

Dzięki temu aplikacja może być używana zarówno bezpośrednio przez osobę starszą, jak i przez osobę, która pomaga jej w codziennym przyjmowaniu leków.

---

### Dodawanie leków

Użytkownik może dodać lek do listy leków. Przy dodawaniu można określić podstawowe informacje, takie jak:

- nazwa leku,
- dawka,
- liczba dawek,
- godziny przyjmowania,
- dodatkowy opis lub uwagi,
- częstotliwość przyjmowania.

---

### Przypomnienia o lekach

Jedną z najważniejszych funkcji aplikacji jest przypominanie użytkownikowi o konieczności przyjęcia leku.

Przypomnienie może zawierać:

- nazwę leku,
- dawkę,
- godzinę przyjęcia,
- dodatkowe instrukcje,
- informację, czy lek został już przyjęty.

Aplikacja może wyświetlać powiadomienie lokalne na telefonie użytkownika, aby przypomnieć o zbliżającej się dawce.

---

### Obsługa wielu dawek

MediSenior umożliwia ustawienie kilku dawek jednego leku w ciągu dnia. Jest to istotne w przypadku leków, które należy przyjmować rano, w południe i wieczorem.

---

### Panel pacjenta

Profil pacjenta jest przeznaczony głównie dla osoby przyjmującej leki.

Możliwe funkcje panelu pacjenta:

- podgląd dzisiejszych leków,
- lista wszystkich leków,
- informacja o najbliższym przypomnieniu,
- oznaczenie dawki jako przyjętej,
- prosty i czytelny interfejs dostosowany do osób starszych.

---

### Panel opiekuna

Profil opiekuna może służyć do zarządzania lekami pacjenta lub kontrolowania harmonogramu leczenia.

Możliwe funkcje panelu opiekuna:

- dodawanie leków dla pacjenta,
- edycja harmonogramu przyjmowania leków,
- podgląd zaplanowanych dawek,
- kontrola regularności przyjmowania leków,
- pomoc pacjentowi w organizacji leczenia.

---

## Technologie

Projekt został wykonany z użyciem następujących technologii:

- **.NET MAUI** – framework do tworzenia aplikacji mobilnych na Androida, iOS, Windows oraz macOS,
- **C#** – główny język programowania aplikacji,
- **XAML** – tworzenie interfejsu użytkownika,
- **Firebase Authentication** – obsługa logowania i rejestracji,
- **Firebase Realtime Database** – przechowywanie danych użytkowników, leków i przypomnień,
- **Local Notifications** – lokalne przypomnienia na urządzeniu,
- **MVVM** – wzorzec architektoniczny ułatwiający oddzielenie logiki od interfejsu.

---

## Wymagania

Do uruchomienia projektu potrzebne są:

- Visual Studio 2022 lub nowszy,
- zainstalowany workload **.NET MAUI**,
- .NET SDK,
- konto Firebase,
- skonfigurowany projekt Firebase,
- emulator Androida lub fizyczne urządzenie z Androidem,
- opcjonalnie Xcode, jeśli aplikacja ma być uruchamiana na iOS.

---

## Konfiguracja Firebase

Aby aplikacja mogła korzystać z Firebase, należy utworzyć projekt w konsoli Firebase i skonfigurować odpowiednie usługi.

### 1. Utworzenie projektu Firebase

1. Przejdź do Firebase Console.
2. Utwórz nowy projekt.
3. Dodaj aplikację Android lub iOS.
4. Pobierz plik konfiguracyjny Firebase.

Dla Androida będzie to najczęściej:

```text
google-services.json
```

Dla iOS:

```text
GoogleService-Info.plist
```

---

### 2. Włączenie logowania

W Firebase należy włączyć metodę logowania, na przykład:

- Email/Password,
- Google Sign-In,
- inne metody dostępne w Firebase Authentication.

Najprostsza konfiguracja dla projektu edukacyjnego to logowanie przez adres e-mail i hasło.

---

### 3. Baza danych

Aplikacja może korzystać z jednej z baz Firebase:

- **Cloud Firestore**
- **Realtime Database**

Przykładowa struktura danych może wyglądać tak:

```json
{
  "users": {
    "userId123": {
      "email": "pacjent@example.com",
      "role": "pacjent",
      "name": "Jan Kowalski"
    }
  },
  "medicines": {
    "medicineId456": {
      "userId": "userId123",
      "name": "Apap",
      "dose": "1 tabletka",
      "times": ["08:00", "20:00"],
      "notes": "Przyjmować po posiłku"
    }
  },
  "reminders": {
    "reminderId789": {
      "medicineId": "medicineId456",
      "time": "08:00",
      "isTaken": false
    }
  }
}
```

---

## Instalacja i uruchomienie

### 1. Sklonowanie repozytorium

```bash
git clone https://github.com/twoj-login/MediSenior.git
cd MediSenior
```

---

### 2. Otwarcie projektu

Otwórz projekt w Visual Studio:

```text
MediSenior.sln
```

---

### 3. Instalacja zależności

Po otwarciu projektu Visual Studio powinno automatycznie przywrócić pakiety NuGet. W razie potrzeby można użyć polecenia:

```bash
dotnet restore
```

---

### 4. Dodanie konfiguracji Firebase

Dodaj plik Firebase do odpowiedniego miejsca w projekcie.


Należy upewnić się, że pliki konfiguracyjne nie są przypadkowo udostępniane publicznie, jeśli zawierają wrażliwe dane projektu.

---

### 5. Uruchomienie aplikacji

Aplikację można uruchomić z poziomu Visual Studio, wybierając emulator lub podłączone urządzenie.

Można również użyć polecenia:

```bash
dotnet build
dotnet run
```

---
