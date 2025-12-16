using Core.Models;
using SQLite;

namespace Core.Services;

public class DatabaseService(string databasePath)
{
        public SQLiteAsyncConnection _database;

        /// <summary>
        /// Initializes the database connection and creates tables if they don't exist
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(databasePath);

            // Create tables
            await _database.CreateTableAsync<Church>();
            await _database.CreateTableAsync<Member>();

            // Create indices for performance
            await _database.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_member_church ON Member(ChurchId)");
            await _database.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_church_synchronized ON Church(IsSynchronized)");
            await _database.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_member_synchronized ON Member(IsSynchronized)");
        }

       

      
}