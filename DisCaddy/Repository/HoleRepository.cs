using DisCaddy.Objects;
using DisCaddy.Repository.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Repository
{
    public class HoleRepository : IHoleRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public HoleRepository(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Hole>().Wait();
        }
        public Task DeleteHoleAsync(Hole hole)
        {
            return _database.DeleteAsync(hole);
        }

        public Task<List<Hole>> GetAllHolesAsync()
        {
            return _database.Table<Hole>().ToListAsync();
        }

        public Task<List<Hole>> GetHolesByCourseIdAsync(int courseId)
        {
            return _database.Table<Hole>()
                .Where(h => h.CourseId == courseId)
                .OrderBy(h => h.HoleNumber)
                .ToListAsync();
        }

        public Task SaveHoleAsync(Hole hole)
        {
            if (hole.Id != 0)
                return _database.UpdateAsync(hole);
            return _database.InsertAsync(hole);
        }
    }
}
