using Dto.Request;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICommunityProgramService
    {
        Task<IEnumerable<CommunityProgram>> GetAllAsync();
        Task<CommunityProgram> GetByIdAsync(int id);
        Task<CommunityProgram> CreateAsync(CommunityProgramDto dto);
        Task<bool> UpdateAsync(int id, CommunityProgramDto dto);
        Task<bool> DeleteAsync(int id);
    }

}
