using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCaddy.Objects;
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
            return _database.Table<Course>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public Task SaveCourseAsync(Course course)
        {
            if (course.Id != 0)
                return _database.UpdateAsync(course);
            return _database.InsertAsync(course);
        }
    }
}
