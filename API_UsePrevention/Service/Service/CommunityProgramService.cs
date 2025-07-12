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
    public class CommunityProgramService : ICommunityProgramService
    {
        private readonly IUnitOfWork _unitOfWork;


        public CommunityProgramService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CommunityProgram>> GetAllAsync()
        {
            return await _unitOfWork.CommunityProgram.GetAll();
        }

        public async Task<CommunityProgram> GetByIdAsync(int id)
        {
            return await _unitOfWork.CommunityProgram.GetByIdAsync(id);
        }

        public async Task<CommunityProgram> CreateAsync(CommunityProgramDto dto)
        {
            var program = new CommunityProgram
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ImgUrl = dto.ImgUrl
            };
            await _unitOfWork.CommunityProgram.AddAsync(program);
            await _unitOfWork.CommitAsync();
            return program;
        }

        public async Task<bool> UpdateAsync(int id, CommunityProgramDto dto)
        {
            var program = await _unitOfWork.CommunityProgram.GetByIdAsync(id);
            if (program == null) return false;

            program.Name = dto.Name;
            program.Description = dto.Description;
            program.StartDate = dto.StartDate;
            program.EndDate = dto.EndDate;
            program.ImgUrl = dto.ImgUrl;

            await _unitOfWork.CommunityProgram.UpdateAsync(program);
             await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.CommunityProgram.GetByIdAsync(id);
            if (existing == null) return false;

            await _unitOfWork.CommunityProgram.DeleteAsync(existing);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
