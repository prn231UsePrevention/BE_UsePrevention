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

namespace Service.Service
{
    public class ParticipationService : IParticipationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ParticipationDto> CreateOrUpdateParticipationAsync(int userId, ParticipationCreateUpdateDto dto)
        {
            // Lấy chương trình từ repository
            var program = await _unitOfWork.CommunityProgram.GetByIdAsync(dto.ProgramId);

            if (program == null)
                return null;

            var now = DateTime.UtcNow;
            if (now < program.StartDate || now > program.EndDate)
                return null;
            
            // Kiểm tra nếu người dùng đã tham gia
            var existing = (await _unitOfWork.Participation.FindAsync(p =>
                p.UserId == userId && p.ProgramId == dto.ProgramId)).FirstOrDefault();

            if (existing != null)
            {
                existing.PreSurvey = dto.PreSurvey;
                existing.PostSurvey = dto.PostSurvey;
                existing.JoinedAt = dto.JoinedAt?.ToUniversalTime() ?? now;

                await _unitOfWork.Participation.UpdateAsync(existing);
                await _unitOfWork.CommitAsync();

                return MapToDto(existing);
            }

            var newParticipation = new Participation
            {
                UserId = userId,
                ProgramId = dto.ProgramId,
                PreSurvey = dto.PreSurvey,
                PostSurvey = dto.PostSurvey,
                JoinedAt = dto.JoinedAt?.ToUniversalTime() ?? now
            };

            await _unitOfWork.Participation.AddAsync(newParticipation);
            await _unitOfWork.CommitAsync();

            return MapToDto(newParticipation);
        }

        public async Task<IEnumerable<ParticipationDto>> GetUserParticipationsAsync(int userId)
        {
            var participations = await _unitOfWork.Participation.FindAsync(p => p.UserId == userId);
            return participations.Select(MapToDto);
        }

        private ParticipationDto MapToDto(Participation p) => new ParticipationDto
        {
            Id = p.Id,
            UserId = p.UserId,
            ProgramId = p.ProgramId,
            PreSurvey = p.PreSurvey,
            PostSurvey = p.PostSurvey,
            JoinedAt = p.JoinedAt
        };

        public async Task<IEnumerable<ParticipationDto>> GetAllParticipationsAsync()
        {
            var participations = await _unitOfWork.Participation.GetAll();
            return participations.Select(p => new ParticipationDto
            {
                Id = p.Id,
                UserId = p.UserId,
                ProgramId = p.ProgramId,
                PreSurvey = p.PreSurvey,
                PostSurvey = p.PostSurvey,
                JoinedAt = p.JoinedAt
            });
        }
    }
}
