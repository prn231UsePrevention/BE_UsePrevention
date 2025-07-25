using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IConsultantService
    {
        Task<IEnumerable<Consultant>> GetAllAsync();
        Task<Consultant> GetByIdAsync(int id);
        Task AddAsync(Consultant consultant);
        Task UpdateAsync(Consultant consultant);
        Task DeleteAsync(int id);
    }
}
