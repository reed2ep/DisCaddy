using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCaddy.Objects;

namespace DisCaddy.Repository.Interfaces
{
    public interface IHoleRepository
    {
        Task SaveHoleAsync(Hole hole);
        Task<List<Hole>> GetAllHolesAsync();
        Task<List<Hole>> GetHolesByCourseIdAsync(int courseId);
        Task DeleteHoleAsync(Hole hole);
    }
}
