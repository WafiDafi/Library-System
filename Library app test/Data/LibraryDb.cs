// first modified in 18/04/20
//
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Library_app_test.Data
{
    internal static class LibraryDb
    {
        private static string DbFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "library.db");
        private static string ConnectionString => $"Data Source={DbFile}";

        public static void EnsureDatabase()
        {
            // Always open the database and ensure required tables and seed data exist.
            // This will create missing tables if the DB file exists but is missing schema.
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Authors (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Books (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    AuthorId INTEGER,
    ISBN TEXT,
    PublishedYear INTEGER,
    Copies INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY(AuthorId) REFERENCES Authors(Id)
);

CREATE TABLE IF NOT EXISTS Members (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Email TEXT,
    Phone TEXT
);

CREATE TABLE IF NOT EXISTS Issues (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    BookId INTEGER NOT NULL,
    MemberId INTEGER NOT NULL,
    IssueDate TEXT NOT NULL,
    DueDate TEXT NOT NULL,
    ReturnDate TEXT,
    FOREIGN KEY(BookId) REFERENCES Books(Id),
    FOREIGN KEY(MemberId) REFERENCES Members(Id)
);

CREATE TABLE IF NOT EXISTS Admins (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    IsBuiltin INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Settings (
    Key TEXT PRIMARY KEY,
    Value TEXT
);

INSERT OR IGNORE INTO Admins (Username, Password, IsBuiltin) VALUES ('admin', '123', 1);
INSERT OR IGNORE INTO Settings (Key, Value) VALUES ('DarkMode', '0');
";
            cmd.ExecuteNonQuery();

            // If Books table is empty insert 20 sample/random books so the catalog has data
            using (var countCmd = conn.CreateCommand())
            {
                countCmd.CommandText = "SELECT COUNT(*) FROM Books";
                var cnt = Convert.ToInt32(countCmd.ExecuteScalar());
                if (cnt == 0)
                {
                    var rnd = new Random();
                    var sampleTitles = new[]
                    {
                        "The Last Sunrise",
                        "Whispers in the Wind",
                        "Echoes of Tomorrow",
                        "The Paper Garden",
                        "Midnight at the Library",
                        "Fragments of Light",
                        "The Clockmaker's Secret",
                        "Beneath the Willow",
                        "A Dance of Shadows",
                        "Northern Stars",
                        "The Painted Door",
                        "River of Stories",
                        "Orchid House",
                        "A Room of Maps",
                        "The Forgotten Key",
                        "Letters to June",
                        "The Quiet Architect",
                        "Glass and Ash",
                        "The Boy with Red Shoes",
                        "Paper Lanterns"
                    };

                    for (int i = 0; i < sampleTitles.Length; i++)
                    {
                        using var ins = conn.CreateCommand();
                        ins.CommandText = "INSERT INTO Books (Title, AuthorId, ISBN, PublishedYear, Copies) VALUES (@t, NULL, @isbn, @y, @c)";
                        ins.Parameters.AddWithValue("@t", sampleTitles[i]);
                        // generate a simple pseudo-ISBN
                        var isbn = $"978-1-{rnd.Next(10000, 99999)}-{rnd.Next(100, 999)}";
                        ins.Parameters.AddWithValue("@isbn", isbn);
                        ins.Parameters.AddWithValue("@y", rnd.Next(1990, DateTime.Now.Year + 1));
                        ins.Parameters.AddWithValue("@c", rnd.Next(1, 6));
                        ins.ExecuteNonQuery();
                    }
                }
            }

            // Ensure Members has extra student related columns (safe to run multiple times)
            try
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Members ADD COLUMN Course TEXT";
                alter.ExecuteNonQuery();
            }
            catch { /* ignore if column exists */ }
            try
            {
                using var alter2 = conn.CreateCommand();
                alter2.CommandText = "ALTER TABLE Members ADD COLUMN IdNumber TEXT";
                alter2.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var alter3 = conn.CreateCommand();
                alter3.CommandText = "ALTER TABLE Members ADD COLUMN OverduesCount INTEGER DEFAULT 0";
                alter3.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var alter4 = conn.CreateCommand();
                alter4.CommandText = "ALTER TABLE Members ADD COLUMN PrevBorrowedCount INTEGER DEFAULT 0";
                alter4.ExecuteNonQuery();
            }
            catch { }

            // Create Payments table to record fee payments by member
            using (var pay = conn.CreateCommand())
            {
                pay.CommandText = @"
CREATE TABLE IF NOT EXISTS Payments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MemberId INTEGER NOT NULL,
    Amount REAL NOT NULL,
    PaidAt TEXT NOT NULL,
    Note TEXT,
    IssueId INTEGER,
    FOREIGN KEY(MemberId) REFERENCES Members(Id)
);
";
                pay.ExecuteNonQuery();
            }

            // Ensure Payments has IssueId column for linking payments to specific issues (silent if already present)
            try
            {
                using var addCol = conn.CreateCommand();
                addCol.CommandText = "ALTER TABLE Payments ADD COLUMN IssueId INTEGER";
                addCol.ExecuteNonQuery();
            }
            catch { }

            // Seed some sample members (students) if none exist
            using (var mcmd = conn.CreateCommand())
            {
                mcmd.CommandText = "SELECT COUNT(*) FROM Members";
                var mcnt = Convert.ToInt32(mcmd.ExecuteScalar());
                if (mcnt == 0)
                {
                    var samples = new[]
                    {
                        ("Juan Dela Cruz","BS Computer Science","2021001","juan@example.com","09171234567"),
                        ("Maria Clara","BS Information Technology","2021002","maria@example.com","09171234568"),
                        ("Pedro Santos","BS Education","2021003","pedro@example.com","09171234569"),
                        ("Ana Reyes","BS Business Administration","2021004","ana@example.com","09171234570"),
                        ("Luis Gonzales","BS Accountancy","2021005","luis@example.com","09171234571"),
                        ("Rosa Lim","BS Psychology","2021006","rosa@example.com","09171234572"),
                        ("Mark Tan","BS Biology","2021007","mark@example.com","09171234573"),
                        ("Grace Lee","BS Hospitality","2021008","grace@example.com","09171234574"),
                        ("Tony Stark","BS Engineering","2021009","tony@example.com","09171234575"),
                        ("Natasha Romanoff","BS Criminology","2021010","natasha@example.com","09171234576")
                    };

                    foreach (var s in samples)
                    {
                        using var insm = conn.CreateCommand();
                        insm.CommandText = "INSERT INTO Members (FullName, Email, Phone, Course, IdNumber, OverduesCount, PrevBorrowedCount) VALUES (@n,@e,@p,@c,@idn,0,0)";
                        insm.Parameters.AddWithValue("@n", s.Item1);
                        insm.Parameters.AddWithValue("@e", s.Item4);
                        insm.Parameters.AddWithValue("@p", s.Item5);
                        insm.Parameters.AddWithValue("@c", s.Item2);
                        insm.Parameters.AddWithValue("@idn", s.Item3);
                        insm.ExecuteNonQuery();
                    }
                }
                // if there are less than 20 members, seed 10 more members with past returned issues and payments (no ongoing borrowed books)
                using (var count2 = conn.CreateCommand())
                {
                    count2.CommandText = "SELECT COUNT(*) FROM Members";
                    var total = Convert.ToInt32(count2.ExecuteScalar());
                    if (total < 20)
                    {
                        var rnd = new Random();
                        var more = new[]
                        {
                            ("Elena Cruz","BS Nursing","2021011","elena@example.com","09171234580"),
                            ("Carlos Rivera","BS Math","2021012","carlos@example.com","09171234581"),
                            ("Isabel Santos","BS Arts","2021013","isabel@example.com","09171234582"),
                            ("Miguel Ramos","BS Chemistry","2021014","miguel@example.com","09171234583"),
                            ("Sofia Diaz","BS Music","2021015","sofia@example.com","09171234584"),
                            ("Diego Lopez","BS Physics","2021016","diego@example.com","09171234585"),
                            ("Camila Ortega","BS Literature","2021017","camila@example.com","09171234586"),
                            ("Javier Morales","BS IT","2021018","javier@example.com","09171234587"),
                            ("Laura Gomez","BS Education","2021019","laura@example.com","09171234588"),
                            ("Andres Perez","BS Management","2021020","andres@example.com","09171234589")
                        };

                        // get available book ids to reference
                        var bookIds = new System.Collections.Generic.List<int>();
                        using (var bcmd = conn.CreateCommand())
                        {
                            bcmd.CommandText = "SELECT Id FROM Books";
                            using var br = bcmd.ExecuteReader();
                            while (br.Read()) bookIds.Add(br.GetInt32(0));
                        }

                        foreach (var s in more)
                        {
                            using var insm = conn.CreateCommand();
                            insm.CommandText = "INSERT INTO Members (FullName, Email, Phone, Course, IdNumber, OverduesCount, PrevBorrowedCount) VALUES (@n,@e,@p,@c,@idn,@od,@pb); SELECT last_insert_rowid();";
                            insm.Parameters.AddWithValue("@n", s.Item1);
                            insm.Parameters.AddWithValue("@e", s.Item4);
                            insm.Parameters.AddWithValue("@p", s.Item5);
                            insm.Parameters.AddWithValue("@c", s.Item2);
                            insm.Parameters.AddWithValue("@idn", s.Item3);
                            var prev = rnd.Next(1, 6);
                            var over = rnd.Next(0, 3);
                            insm.Parameters.AddWithValue("@od", over);
                            insm.Parameters.AddWithValue("@pb", prev);
                            var newId = Convert.ToInt32(insm.ExecuteScalar());

                            // create 1 returned issue (past) optionally overdue, then record a payment that covers the overdue fee so no ongoing fee remains
                            if (bookIds.Count > 0)
                            {
                                var bookId = bookIds[rnd.Next(bookIds.Count)];
                                var issueDate = DateTime.UtcNow.AddDays(-30 - rnd.Next(0, 60));
                                var dueDate = issueDate.AddDays(7);
                                var returnedLateDays = rnd.Next(0, 5);
                                var returnDate = dueDate.AddDays(returnedLateDays);
                                using var icmd = conn.CreateCommand();
                                icmd.CommandText = "INSERT INTO Issues (BookId, MemberId, IssueDate, DueDate, ReturnDate) VALUES (@b,@m,@i,@d,@r); SELECT last_insert_rowid();";
                                icmd.Parameters.AddWithValue("@b", bookId);
                                icmd.Parameters.AddWithValue("@m", newId);
                                icmd.Parameters.AddWithValue("@i", issueDate.ToString("o"));
                                icmd.Parameters.AddWithValue("@d", dueDate.ToString("o"));
                                icmd.Parameters.AddWithValue("@r", returnDate.ToString("o"));
                                var issueId = Convert.ToInt32(icmd.ExecuteScalar());

                                // compute fee and insert a payment that covers it
                                var fee = returnedLateDays * 1.0m;
                                if (fee > 0)
                                {
                                    using var pcmd = conn.CreateCommand();
                                    pcmd.CommandText = "INSERT INTO Payments (MemberId, Amount, PaidAt, Note, IssueId) VALUES (@m,@a,@t,@note,@iss)";
                                    pcmd.Parameters.AddWithValue("@m", newId);
                                    pcmd.Parameters.AddWithValue("@a", fee);
                                    pcmd.Parameters.AddWithValue("@t", DateTime.UtcNow.ToString("o"));
                                    pcmd.Parameters.AddWithValue("@note", "Past fee paid during seeding");
                                    pcmd.Parameters.AddWithValue("@iss", issueId);
                                    pcmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
