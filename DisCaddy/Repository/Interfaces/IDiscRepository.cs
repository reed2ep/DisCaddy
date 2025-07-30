using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCaddy.Models;

namespace DisCaddy.Repository.Interfaces
{
    public interface IDiscRepository
    {
        Task<List<Disc>> GetAllAsync();
        Task<Disc?> GetByIdAsync(int id);
        Task<int> SaveAsync(Disc disc);
        Task<int> DeleteAsync(Disc disc);
    }
}
