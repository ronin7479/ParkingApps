using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Data;
using ParkingApp.Models;

namespace ParkingApp.Services
{
    public class DatabaseService
    {
        // ── db context factory ──────────────────────────────────────────────
        private AppDbContext CreateContext() => new AppDbContext();

        // ── constructor: migrate + seed ──────────────────────────────────────
        public DatabaseService()
        {
            using var db = CreateContext();

            // EnsureCreated створює БД і таблиці якщо їх немає
            db.Database.EnsureCreated();

            // Seed parking lots if table is empty
            if (!db.ParkingLots.Any())
            {
                SeedParkingLots(db);
            }
            else
            {
                // Оновлюємо ImagePath для існуючих паркінгів
                // щоб виправити повторювані зображення
                UpdateImagePaths(db);
            }
        }

        // Оновлюємо шляхи до зображень якщо вони повторюються
        private void UpdateImagePaths(AppDbContext db)
        {
            var imageMap = new Dictionary<int, string>
            {
                { 1,  "parking_lot_1.jpg"  },
                { 2,  "parking_lot_2.jpg"  },
                { 3,  "parking_lot_3.jpg"  },
                { 4,  "parking_lot_4.JPG"  },
                { 5,  "parking_lot_5.jpg"  },
                { 6,  "parking_lot_6.jpg"  },
                { 7,  "parking_lot_7.jpeg" },
                { 8,  "parking_lot_8.jpeg" },
                { 9,  "parking_lot_9.jpg"  },
                { 10, "parking_lot_10.jpg" },
            };

            var lots = db.ParkingLots.ToList();
            bool changed = false;

            foreach (var lot in lots)
            {
                if (imageMap.TryGetValue(lot.Id, out var correctPath))
                {
                    if (lot.ImagePath != correctPath)
                    {
                        lot.ImagePath = correctPath;
                        changed = true;
                    }
                }
            }

            if (changed)
                db.SaveChanges();
        }

        // ── SEED ─────────────────────────────────────────────────────────────
        private void SeedParkingLots(AppDbContext db)
        {
            var lots = new[]
            {
                new ParkingLot { Name = "ТРЦ \"Ocean Plaza\"",   Location = "вул. Антоновича, 176",       ImagePath = "parking_lot_1.jpg",  TotalSpaces = 800,  OccupiedSpaces = 320, PricePerHour = 30m,  Latitude = 50.4195, Longitude = 30.5222 },
                new ParkingLot { Name = "ТРЦ \"Gulliver\"",      Location = "вул. Спортивна пл., 1",      ImagePath = "parking_lot_2.jpg",  TotalSpaces = 600,  OccupiedSpaces = 450, PricePerHour = 40m,  Latitude = 50.4330, Longitude = 30.5208 },
                new ParkingLot { Name = "Паркінг \"Майдан\"",    Location = "пл. Незалежності, 1",        ImagePath = "parking_lot_3.jpg",  TotalSpaces = 200,  OccupiedSpaces = 185, PricePerHour = 50m,  Latitude = 50.4501, Longitude = 30.5234 },
                new ParkingLot { Name = "ТРЦ \"Respublika\"",    Location = "просп. Бажана, 8",           ImagePath = "parking_lot_4.JPG",  TotalSpaces = 1200, OccupiedSpaces = 80,  PricePerHour = 25m,  Latitude = 50.4064, Longitude = 30.6187 },
                new ParkingLot { Name = "Паркінг \"Хрещатик\"",  Location = "вул. Хрещатик, 22",          ImagePath = "parking_lot_5.jpg",  TotalSpaces = 150,  OccupiedSpaces = 100, PricePerHour = 60m,  Latitude = 50.4474, Longitude = 30.5228 },
                new ParkingLot { Name = "ТРЦ \"Dream Town\"",    Location = "просп. Оболонський, 1Б",     ImagePath = "parking_lot_6.jpg",  TotalSpaces = 500,  OccupiedSpaces = 120, PricePerHour = 20m,  Latitude = 50.5065, Longitude = 30.4981 },
                new ParkingLot { Name = "Паркінг \"Аеропорт\"",  Location = "просп. Перемоги, 1",         ImagePath = "parking_lot_7.jpeg", TotalSpaces = 400,  OccupiedSpaces = 300, PricePerHour = 35m,  Latitude = 50.4018, Longitude = 30.4493 },
                new ParkingLot { Name = "ТРЦ \"SkyMall\"",       Location = "вул. Бориспільська, 9",      ImagePath = "parking_lot_8.jpeg", TotalSpaces = 700,  OccupiedSpaces = 200, PricePerHour = 22m,  Latitude = 50.4200, Longitude = 30.6850 },
                new ParkingLot { Name = "Паркінг \"Поділ\"",     Location = "Контрактова пл., 4",         ImagePath = "parking_lot_9.jpg",  TotalSpaces = 120,  OccupiedSpaces = 90,  PricePerHour = 45m,  Latitude = 50.4633, Longitude = 30.5165 },
                new ParkingLot { Name = "ТРЦ \"Lavina Mall\"",   Location = "просп. Берестейський, 75Ж",  ImagePath = "parking_lot_10.jpg", TotalSpaces = 3000, OccupiedSpaces = 500, PricePerHour = 18m,  Latitude = 50.4698, Longitude = 30.3659 },
            };

            db.ParkingLots.AddRange(lots);
            db.SaveChanges();

            // Seed parking spaces (max 50 per lot for performance)
            foreach (var lot in db.ParkingLots.ToList())
            {
                int display = Math.Min(lot.TotalSpaces, 50);
                int occupied = (int)(lot.OccupiedSpaces * (double)display / lot.TotalSpaces);

                for (int i = 1; i <= display; i++)
                {
                    db.ParkingSpaces.Add(new ParkingSpace
                    {
                        ParkingLotId = lot.Id,
                        SpaceNumber = i,
                        IsOccupied = i <= occupied
                    });
                }
            }
            db.SaveChanges();
        }

        // ── USERS ─────────────────────────────────────────────────────────────
        public bool AddUser(User user)
        {
            if (user == null) return false;
            using var db = CreateContext();
            db.Users.Add(user);
            db.SaveChanges();
            return true;
        }

        public User? GetUserByLogin(string login)
        {
            using var db = CreateContext();
            return db.Users.AsNoTracking()
                           .FirstOrDefault(u => u.Login == login);
        }

        public User? GetUserById(int id)
        {
            using var db = CreateContext();
            return db.Users.AsNoTracking()
                           .FirstOrDefault(u => u.Id == id);
        }

        public bool UserExists(string login)
        {
            using var db = CreateContext();
            return db.Users.Any(u => u.Login == login);
        }

        public void UpdateUser(User user)
        {
            using var db = CreateContext();
            db.Users.Update(user);
            db.SaveChanges();
        }

        // ── PARKING LOTS ──────────────────────────────────────────────────────
        public List<ParkingLot> GetAllParkingLots()
        {
            using var db = CreateContext();
            return db.ParkingLots.AsNoTracking().ToList();
        }

        public ParkingLot? GetParkingLotById(int id)
        {
            using var db = CreateContext();
            return db.ParkingLots.AsNoTracking()
                                 .FirstOrDefault(p => p.Id == id);
        }

        public void UpdateParkingLot(ParkingLot lot)
        {
            using var db = CreateContext();
            var existing = db.ParkingLots.Find(lot.Id);
            if (existing != null)
            {
                existing.OccupiedSpaces = lot.OccupiedSpaces;
                db.SaveChanges();
            }
        }

        // ── PARKING SPACES ────────────────────────────────────────────────────
        public List<ParkingSpace> GetParkingSpacesByLotId(int lotId)
        {
            using var db = CreateContext();
            return db.ParkingSpaces.AsNoTracking()
                                   .Where(s => s.ParkingLotId == lotId)
                                   .ToList();
        }

        public ParkingSpace? GetParkingSpace(int spaceId)
        {
            using var db = CreateContext();
            return db.ParkingSpaces.AsNoTracking()
                                   .FirstOrDefault(s => s.Id == spaceId);
        }

        public void UpdateParkingSpace(ParkingSpace space)
        {
            using var db = CreateContext();
            var existing = db.ParkingSpaces.Find(space.Id);
            if (existing != null)
            {
                existing.IsOccupied = space.IsOccupied;
                db.SaveChanges();
            }
        }

        // ── BOOKINGS ──────────────────────────────────────────────────────────
        public bool AddBooking(Booking booking)
        {
            if (booking == null) return false;
            using var db = CreateContext();
            db.Bookings.Add(booking);
            db.SaveChanges();
            return true;
        }

        public List<Booking> GetUserBookings(int userId)
        {
            using var db = CreateContext();
            return db.Bookings.AsNoTracking()
                              .Where(b => b.UserId == userId)
                              .OrderByDescending(b => b.CreatedDate)
                              .ToList();
        }

        public List<Booking> GetAllBookings()
        {
            using var db = CreateContext();
            return db.Bookings.AsNoTracking()
                              .OrderByDescending(b => b.CreatedDate)
                              .ToList();
        }

        public void UpdateBooking(Booking booking)
        {
            using var db = CreateContext();
            var existing = db.Bookings.Find(booking.Id);
            if (existing != null)
            {
                existing.IsActive = booking.IsActive;
                db.SaveChanges();
            }
        }

        // ── RECEIPTS ──────────────────────────────────────────────────────────
        public bool AddReceipt(Receipt receipt)
        {
            if (receipt == null) return false;
            using var db = CreateContext();
            db.Receipts.Add(receipt);
            db.SaveChanges();
            return true;
        }

        public Receipt? GetReceiptByNumber(string receiptNumber)
        {
            using var db = CreateContext();
            return db.Receipts.AsNoTracking()
                              .FirstOrDefault(r => r.ReceiptNumber == receiptNumber);
        }

        // ── STATISTICS ────────────────────────────────────────────────────────
        public int GetTodayBookingsCount()
        {
            using var db = CreateContext();
            var today = DateTime.Today;
            return db.Bookings.Count(b => b.CreatedDate.Date == today);
        }

        public decimal GetTodayRevenue()
        {
            using var db = CreateContext();
            var today = DateTime.Today;
            return db.Bookings
                     .Where(b => b.CreatedDate.Date == today)
                     .Sum(b => (decimal?)b.TotalPrice) ?? 0m;
        }

        public decimal GetWeekRevenue()
        {
            using var db = CreateContext();
            var weekAgo = DateTime.Now.AddDays(-7);
            return db.Bookings
                     .Where(b => b.CreatedDate >= weekAgo)
                     .Sum(b => (decimal?)b.TotalPrice) ?? 0m;
        }

        public int GetTotalUsers()
        {
            using var db = CreateContext();
            return db.Users.Count();
        }

        public int GetTotalBookings()
        {
            using var db = CreateContext();
            return db.Bookings.Count();
        }
    }
}
