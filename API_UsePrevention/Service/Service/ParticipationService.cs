using AutoMapper;
using Dto.Request;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Service.Service
{
    public class ParticipationService : IParticipationService
    {
        private readonly IGenericRepository<Participation> _participationRepository;
        private readonly IGenericRepository<CommunityProgram> _communityProgramRepository;
        private readonly IUnitOfWork _unitOfWork;
<<<<<<< Updated upstream

        public ParticipationService(IGenericRepository<Participation> participationRepository,
             IGenericRepository<CommunityProgram> communityProgramRepository, IUnitOfWork unitOfWork)
=======
        private readonly IMapper _mapper;
        public ParticipationService(IUnitOfWork unitOfWork, IMapper mapper)
>>>>>>> Stashed changes
        {
            _participationRepository = participationRepository;
            _communityProgramRepository = communityProgramRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ParticipationDto> CreateOrUpdateParticipationAsync(int userId, ParticipationCreateUpdateDto dto)
        {
<<<<<<< Updated upstream
            // Lấy chương trình từ repository
            var program = await _communityProgramRepository.GetByIdAsync(dto.ProgramId);

            if (program == null)
                return null;

            var now = DateTime.UtcNow;
            if (now < program.StartDate || now > program.EndDate)
                return null;

            // Kiểm tra nếu người dùng đã tham gia
            var existing = (await _participationRepository.FindAsync(p =>
=======
            var program = await _unitOfWork.CommunityProgram.GetByIdAsync(dto.ProgramId);
            if (program == null || DateTime.UtcNow < program.StartDate || DateTime.UtcNow > program.EndDate)
                return null;

            var now = DateTime.UtcNow.AddHours(7); 
            var existing = (await _unitOfWork.Participation.FindAsync(p =>
>>>>>>> Stashed changes
                p.UserId == userId && p.ProgramId == dto.ProgramId)).FirstOrDefault();

            if (existing != null)
            {
                _mapper.Map(dto, existing);
                existing.JoinedAt = now;

                await _participationRepository.UpdateAsync(existing);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<ParticipationDto>(existing);
            }

            var newParticipation = _mapper.Map<Participation>(dto);
            newParticipation.UserId = userId;
            newParticipation.JoinedAt = now;

            await _participationRepository.AddAsync(newParticipation);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ParticipationDto>(newParticipation);
        }

        public async Task<IEnumerable<ParticipationDto>> GetUserParticipationsAsync(int userId)
        {
<<<<<<< Updated upstream
            var participations = await _participationRepository.FindAsync(p => p.UserId == userId);
            return participations.Select(MapToDto);
=======
            var participations = await _unitOfWork.Participation.FindAsync(p => p.UserId == userId);
            return _mapper.Map<IEnumerable<ParticipationDto>>(participations);
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<ParticipationDto>> GetAllParticipationsAsync()
        {
<<<<<<< Updated upstream
            var participations = await _participationRepository.GetAll();
            return participations.Select(p => new ParticipationDto
            {
                Id = p.Id,
                UserId = p.UserId,
                ProgramId = p.ProgramId,
                PreSurvey = p.PreSurvey,
                PostSurvey = p.PostSurvey,
                JoinedAt = p.JoinedAt
            });
=======
            var participations = await _unitOfWork.Participation.GetAll();
            return _mapper.Map<IEnumerable<ParticipationDto>>(participations);
>>>>>>> Stashed changes
        }

    }
}
