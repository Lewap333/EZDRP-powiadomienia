# EZDRP-powiadomienia
Powiadomienia o zmianach na stronie https://podrecznik.ezdrp.gov.pl/zmiany-w-wersjach/

## Wykorzystane
- **.NET 8.0**
- **CsvHelper 33.1.0** – paczka z NuGet do obsługi plików CSV  

## Konfiguracja SMTP

Aplikacja do wysyłki e-maili wymaga ustawienia dwóch zmiennych środowiskowych:

- **`EZDRP_SMTP_USER`** – adres e-mail nadawcy  
- **`EZDRP_SMTP_PASS`** – hasło aplikacji 

Hasło aplikacji: z panelu Microsoft:
[https://mysignins.microsoft.com/security-info](https://mysignins.microsoft.com/security-info)

### Zmienne środowiskowe w Windows

Dla **aktualnego użytkownika** (Powershell): 

```powershell
setx EZDRP_SMTP_USER "me@domain.com"
setx EZDRP_SMTP_PASS "haslo_aplikacji"
```

### Plik `mailsettings.txt`

Do folderu, w którym znajduje się plik wykonywalny (`.exe`), trzeba wrzucić plik z ustawieniami do kogo będą wysyłane maile:

**mailsettings.txt**
