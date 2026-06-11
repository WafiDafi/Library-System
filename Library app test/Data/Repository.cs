// first modified in 18/04/20
//
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Library_app_test.Data
{
    internal static class Repository
    {
        public static IEnumerable<Book> GetAllBooks()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Title, AuthorId, ISBN, PublishedYear, Copies FROM Books ORDER BY Title";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                yield return new Book(
                    rdr.GetInt32(0),
                    rdr.GetString(1),
                    rdr.IsDBNull(2) ? null : rdr.GetInt32(2),
                    rdr.IsDBNull(3) ? null : rdr.GetString(3),
                    rdr.IsDBNull(4) ? null : rdr.GetInt32(4),
                    rdr.GetInt32(5)
                );
            }

        }

        // Attempts to delete a book by id. Returns true if a row was deleted.
        public static bool DeleteBook(int id)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Books WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            var affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }

        // Generate a pseudo-ISBN string that does not currently exist in the Books table.
        // Format follows existing pattern: 978-1-<5digits>-<3digits>
        public static string GenerateUniqueIsbn()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            var rnd = new Random();
            while (true)
            {
                var isbn = $"978-1-{rnd.Next(10000, 99999)}-{rnd.Next(100, 999)}";
                cmd.CommandText = "SELECT COUNT(1) FROM Books WHERE ISBN = @isbn";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@isbn", isbn);
                var exists = Convert.ToInt32(cmd.ExecuteScalar());
                if (exists == 0) return isbn;
            }
        }

        // Password hashing using PBKDF2 (Rfc2898). Stored format: iterations.saltBase64.hashBase64
        private const int Pbkdf2Iterations = 100_000;
        private const int SaltSize = 16; // bytes
        private const int HashSize = 32; // bytes

        private static string HashPassword(string password)
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);
            using var pbk = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, Pbkdf2Iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
            var hash = pbk.GetBytes(HashSize);
            return $"{Pbkdf2Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string stored)
        {
            try
            {
                var parts = stored.Split('.');
                if (parts.Length != 3) return false;
                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);
                using var pbk = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
                var actual = pbk.GetBytes(expected.Length);
                return CryptographicEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }

        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            var diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        // Returns admin info including MustChange flag
        public static (int Id, string Username, bool IsBuiltin, bool MustChange)? GetAdminByCredentials(string username, string password)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, Password, IsBuiltin, MustChangePassword FROM Admins WHERE Username = @u LIMIT 1";
            cmd.Parameters.AddWithValue("@u", username);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                var id = rdr.GetInt32(0);
                var user = rdr.GetString(1);
                var stored = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2);
                var isBuiltin = rdr.GetInt32(3) != 0;
                var must = rdr.GetInt32(4) != 0;
                if (VerifyPassword(password, stored))
                {
                    return (id, user, isBuiltin, must);
                }
            }
            return null;
        }

        public static (int Id, string Username, bool IsBuiltin, bool MustChange)? GetAdminById(int id)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, IsBuiltin, MustChangePassword FROM Admins WHERE Id = @id LIMIT 1";
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return (rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2) != 0, rdr.GetInt32(3) != 0);
            }
            return null;
        }

        // Admin management
        public static IEnumerable<(int Id, string Username, bool IsBuiltin)> GetAdmins()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, IsBuiltin FROM Admins ORDER BY Id";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                yield return (rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2) != 0);
            }
        }

        public static int AddAdmin(string username, string password, bool mustChange = false)
        {
            var hashed = HashPassword(password);
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Admins (Username, Password, IsBuiltin, MustChangePassword) VALUES (@u,@p, 0, @m); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hashed);
            cmd.Parameters.AddWithValue("@m", mustChange ? 1 : 0);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void ChangePassword(int adminId, string newPassword)
        {
            var hashed = HashPassword(newPassword);
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Admins SET Password = @p, MustChangePassword = 0 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@p", hashed);
            cmd.Parameters.AddWithValue("@id", adminId);
            cmd.ExecuteNonQuery();
        }

        // Attempts to remove an admin by id. Returns true if a row was deleted.
        public static bool RemoveAdmin(int id)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            // prevent deletion of builtin admins via WHERE clause; if no row affected, it was built-in or missing
            cmd.CommandText = "DELETE FROM Admins WHERE Id = @id AND IsBuiltin = 0";
            cmd.Parameters.AddWithValue("@id", id);
            var affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }

        public static bool GetDarkMode()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Value FROM Settings WHERE Key = 'DarkMode'";
            var v = cmd.ExecuteScalar();
            if (v == null) return false;
            return v.ToString() == "1";
        }

        public static void SetDarkMode(bool enabled)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Settings (Key,Value) VALUES ('DarkMode', @v) ON CONFLICT(Key) DO UPDATE SET Value = @v";
            cmd.Parameters.AddWithValue("@v", enabled ? "1" : "0");
            cmd.ExecuteNonQuery();
        }

        public static IEnumerable<Book> GetAvailableBooks()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Title, AuthorId, ISBN, PublishedYear, Copies FROM Books WHERE Copies > 0 ORDER BY Title";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                yield return new Book(
                    rdr.GetInt32(0),
                    rdr.GetString(1),
                    rdr.IsDBNull(2) ? null : rdr.GetInt32(2),
                    rdr.IsDBNull(3) ? null : rdr.GetString(3),
                    rdr.IsDBNull(4) ? null : rdr.GetInt32(4),
                    rdr.GetInt32(5)
                );
            }
        }

        public static IEnumerable<IssueView> GetIssuedIssues()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT i.Id, b.Title, m.FullName, i.IssueDate, i.DueDate, i.ReturnDate
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
WHERE i.ReturnDate IS NULL
ORDER BY i.IssueDate DESC";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var due = DateTime.Parse(rdr.GetString(4));
                var status = DateTime.UtcNow > due ? "passed due" : "borrowed";
                yield return new IssueView(
                    rdr.GetInt32(0),
                    rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                    DateTime.Parse(rdr.GetString(3)),
                    due,
                    rdr.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(rdr.GetString(5)),
                    status
                );
            }
        }

        public static IEnumerable<IssueView> GetReturnedIssues()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT i.Id, b.Title, m.FullName, i.IssueDate, i.DueDate, i.ReturnDate
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
WHERE i.ReturnDate IS NOT NULL
ORDER BY i.ReturnDate DESC";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var due = DateTime.Parse(rdr.GetString(4));
                var status = DateTime.UtcNow > due ? "passed due" : "borrowed";
                yield return new IssueView(
                    rdr.GetInt32(0),
                    rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                    DateTime.Parse(rdr.GetString(3)),
                    due,
                    rdr.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(rdr.GetString(5)),
                    status
                );
            }

            }

        public static int AddBook(string title, int? authorId, string? isbn, int? year, int copies)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Books (Title, AuthorId, ISBN, PublishedYear, Copies) VALUES (@t,@a,@i,@y,@c); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@a", authorId.HasValue ? (object)authorId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@i", string.IsNullOrEmpty(isbn) ? DBNull.Value : isbn);
            cmd.Parameters.AddWithValue("@y", year.HasValue ? (object)year.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@c", copies);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        public static IEnumerable<Member> GetAllMembers()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, FullName, Email, Phone FROM Members ORDER BY FullName";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                yield return new Member(
                    rdr.GetInt32(0),
                    rdr.GetString(1),
                    rdr.IsDBNull(2) ? null : rdr.GetString(2),
                    rdr.IsDBNull(3) ? null : rdr.GetString(3)
                );
            }
        }

        public static int AddMember(string fullName, string? email, string? phone)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Members (FullName, Email, Phone) VALUES (@n,@e,@p); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@n", fullName);
            cmd.Parameters.AddWithValue("@e", string.IsNullOrEmpty(email) ? DBNull.Value : email);
            cmd.Parameters.AddWithValue("@p", string.IsNullOrEmpty(phone) ? DBNull.Value : phone);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void IssueBook(int bookId, int memberId, DateTime issueDate, DateTime dueDate)
        {
            using var conn = LibraryDb.GetConnection();
            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            // decrease copies
            cmd.CommandText = "UPDATE Books SET Copies = Copies - 1 WHERE Id = @id AND Copies > 0";
            cmd.Parameters.AddWithValue("@id", bookId);
            var affected = cmd.ExecuteNonQuery();
            if (affected == 0)
            {
                tran.Rollback();
                throw new InvalidOperationException("No copies available");
            }

            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT INTO Issues (BookId, MemberId, IssueDate, DueDate) VALUES (@b,@m,@i,@d)";
            cmd.Parameters.AddWithValue("@b", bookId);
            cmd.Parameters.AddWithValue("@m", memberId);
            cmd.Parameters.AddWithValue("@i", issueDate.ToString("o"));
            cmd.Parameters.AddWithValue("@d", dueDate.ToString("o"));
            cmd.ExecuteNonQuery();

            // increment PrevBorrowedCount for the member
            cmd.Parameters.Clear();
            cmd.CommandText = "UPDATE Members SET PrevBorrowedCount = COALESCE(PrevBorrowedCount,0) + 1 WHERE Id = @m";
            cmd.Parameters.AddWithValue("@m", memberId);
            cmd.ExecuteNonQuery();

            tran.Commit();
        }

        public static void ReturnBook(int issueId, DateTime returnDate)
        {
            using var conn = LibraryDb.GetConnection();
            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            // set return date
            cmd.CommandText = "UPDATE Issues SET ReturnDate = @r WHERE Id = @id";
            cmd.Parameters.AddWithValue("@r", returnDate.ToString("o"));
            cmd.Parameters.AddWithValue("@id", issueId);
            var affected = cmd.ExecuteNonQuery();
            if (affected == 0)
            {
                tran.Rollback();
                throw new InvalidOperationException("Issue record not found");
            }

            cmd.Parameters.Clear();
            // increment copies
            cmd.CommandText = "UPDATE Books SET Copies = Copies + 1 WHERE Id = (SELECT BookId FROM Issues WHERE Id = @id)";
            cmd.Parameters.AddWithValue("@id", issueId);
            cmd.ExecuteNonQuery();

            tran.Commit();
        }
    }
}
