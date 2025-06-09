using Dto.Request;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IParticipationService
    {
        Task<ParticipationDto> CreateOrUpdateParticipationAsync(int userId, ParticipationCreateUpdateDto dto);
        Task<IEnumerable<ParticipationDto>> GetUserParticipationsAsync(int userId);

        Task<IEnumerable<ParticipationDto>> GetAllParticipationsAsync();
    }
}
