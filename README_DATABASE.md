# 🗄️ База даних — SQLite + Entity Framework Core

## Що додано

| Файл | Опис |
|------|------|
| `Data/AppDbContext.cs` | EF Core DbContext — описує всі таблиці |
| `Migrations/20250101000000_InitialCreate.cs` | Міграція — створює схему БД |
| `Migrations/AppDbContextModelSnapshot.cs` | Знімок моделі для EF Core |
| `Services/DatabaseService.cs` | Перероблений — тепер використовує SQLite |
| `Models/ParkingLot.cs` | Додано EF атрибути (`[Table]`, `[Key]`, `[NotMapped]`) |
| `ParkingApp.csproj` | Додано NuGet пакети EF Core + SQLite |

---

## 🚀 Як запустити (покрокова інструкція)

### Крок 1 — Встановити NuGet пакети

У Visual Studio відкрий **Package Manager Console** (`Tools → NuGet Package Manager → Package Manager Console`) і виконай:

```powershell
Install-Package Microsoft.EntityFrameworkCore -Version 9.0.0
Install-Package Microsoft.EntityFrameworkCore.Sqlite -Version 9.0.0
Install-Package Microsoft.EntityFrameworkCore.Design -Version 9.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.0
```

Або просто **Rebuild** — пакети підтягнуться автоматично з NuGet через `.csproj`.

---

### Крок 2 — Застосувати міграцію

В **Package Manager Console**:

```powershell
Update-Database
```

Це створить файл `parking.db` у папці з `.exe` (зазвичай `bin\Debug\net10.0-windows\`).

> ✅ Якщо міграція вже є у папці `Migrations\` — просто запусти `Update-Database`.
> Якщо хочеш перегенерувати з нуля:
> ```powershell
> Add-Migration InitialCreate
> Update-Database
> ```

---

### Крок 3 — Запустити проект

Просто натисни **F5**. При першому запуску:
1. `DatabaseService` викликає `db.Database.Migrate()` — БД автоматично створюється/оновлюється
2. Якщо таблиця `ParkingLots` порожня — автоматично заповнюється 10 київськими паркінгами
3. Генеруються паркомісця для кожного паркінгу

---

## 📁 Де зберігається БД

```
bin\Debug\net10.0-windows\parking.db
```

Це звичайний SQLite файл. Переглянути можна через:
- **DB Browser for SQLite** (безкоштовно): https://sqlitebrowser.org
- **JetBrains DataGrip**
- Плагін **SQLite Viewer** у VS Code

---

## 🗂️ Структура таблиць

```
Users
├── Id (PK, autoincrement)
├── Email (unique)
├── Login (unique)
├── PasswordHash
├── AvatarPath
└── SuccessfulBookings

ParkingLots
├── Id (PK)
├── Name, Location, ImagePath
├── TotalSpaces, OccupiedSpaces
├── PricePerHour
├── Latitude, Longitude
└── CreatedDate

ParkingSpaces
├── Id (PK)
├── ParkingLotId (FK → ParkingLots)
├── SpaceNumber
└── IsOccupied

Bookings
├── Id (PK)
├── UserId (FK → Users)
├── ParkingLotId (FK → ParkingLots)
├── SpaceNumber, CarNumber
├── StartTime, EndTime
├── TotalPrice, ReceiptNumber
└── CreatedDate

Receipts
├── Id (PK)
├── ReceiptNumber (unique)
├── ParkingLotName, SpaceNumber, CarNumber
├── Amount
└── PaymentDate
```

---

## ⚠️ Якщо виникають помилки

**`No such table`** — запусти `Update-Database` у Package Manager Console

**`Cannot open database`** — перевір що `parking.db` існує у `bin\Debug\...`

**`Duplicate entry`** — очисти БД: видали `parking.db` і перезапусти програму

**`Migration already applied`** — це нормально, EF Core відстежує застосовані міграції
