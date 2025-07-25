using Repository.Models;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ConsultantService : IConsultantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConsultantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Consultant>> GetAllAsync()
        {
            return await _unitOfWork.Consultant.GetAll();
        }

        public async Task<Consultant> GetByIdAsync(int id)
        {
            return await _unitOfWork.Consultant.GetByIdAsync(id);
        }

        public async Task AddAsync(Consultant consultant)
        {
            await _unitOfWork.Consultant.AddAsync(consultant);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Consultant consultant)
        {
            await _unitOfWork.Consultant.UpdateAsync(consultant);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Consultant.GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.Consultant.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
