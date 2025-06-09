using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using DisCaddy.Models;
namespace DisCaddy
{
    public class SQLiteDiscRepository : IDiscRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public SQLiteDiscRepository(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Disc>().Wait();
        }

        public Task<int> DeleteAsync(Disc disc)
            => _database.DeleteAsync(disc);

        public Task<List<Disc>> GetAllAsync()
            => _database.Table<Disc>().ToListAsync();

        public Task<Disc?> GetByIdAsync(int id)
            => _database.Table<Disc>().Where(d => d.Id == id).FirstOrDefaultAsync();

        public Task<int> SaveAsync(Disc disc)
            => disc.Id == 0 ? _database.InsertAsync(disc) : _database.UpdateAsync(disc);
    }
}
