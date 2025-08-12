using DisCaddy.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Repository.Interfaces
{
    public interface ICourseRepository
    {
        Task SaveCourseAsync(Course course);
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task DeleteCourseAsync(Course course);
    }
}
