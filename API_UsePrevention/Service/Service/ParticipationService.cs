using Dto.Request;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;
using Service.Interface;

using AutoMapper;


namespace Service.Service
{
    public class ParticipationService : IParticipationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ParticipationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ParticipationDto> CreateOrUpdateParticipationAsync(int userId, ParticipationCreateUpdateDto dto)
        {
            var program = await _unitOfWork.CommunityProgram.GetByIdAsync(dto.ProgramId);

            if (program == null)
                return null;

            var now = DateTime.UtcNow.AddHours(7);
            if (now < program.StartDate || now > program.EndDate)
                return null;

            var existing = (await _unitOfWork.Participation.FindAsync(p =>
                p.UserId == userId && p.ProgramId == dto.ProgramId)).FirstOrDefault();

            if (existing != null)
            {
                _mapper.Map(dto, existing);
                existing.JoinedAt = now;

                await _unitOfWork.Participation.UpdateAsync(existing);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<ParticipationDto>(existing);
            }

            var newParticipation = _mapper.Map<Participation>(dto);
            newParticipation.UserId = userId;
            newParticipation.JoinedAt = now;

            await _unitOfWork.Participation.AddAsync(newParticipation);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ParticipationDto>(newParticipation);
        }

        public async Task<IEnumerable<ParticipationDto>> GetUserParticipationsAsync(int userId)
        {
            var participations = await _unitOfWork.Participation.FindAsync(p => p.UserId == userId);
            return _mapper.Map<IEnumerable<ParticipationDto>>(participations);
        }

        public async Task<IEnumerable<ParticipationDto>> GetAllParticipationsAsync()
        {
            var participations = await _unitOfWork.Participation.GetAll();
            return _mapper.Map<IEnumerable<ParticipationDto>>(participations);
        }
    }
}
