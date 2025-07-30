using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCaddy.Models;
using DisCaddy.Repository.Interfaces;
using SQLite;
namespace DisCaddy.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public CourseRepository(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Course>().Wait();
        }
        public Task DeleteCourseAsync(Course course)
        {
            return _database.DeleteAsync(course);
        }

        public Task<List<Course>> GetAllCoursesAsync()
        {
            return _database.Table<Course>().ToListAsync();
        }

        public Task<Course?> GetCourseByIdAsync(int id)
        {
            return _database.GetAsync<Course>(id);
        }

        public Task SaveCourseAsync(Course course)
        {
            return _database.InsertAsync(course);
        }
    }
}
