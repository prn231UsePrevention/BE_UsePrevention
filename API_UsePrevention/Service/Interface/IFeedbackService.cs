﻿using Dto.Request;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDto>> GetAllAsync();
        Task<FeedbackDto?> GetByIdAsync(int id);
        Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto, int userId);
        Task<bool> DeleteAsync(int id);
        Task<FeedbackDto?> UpdateAsync(int id, UpdateFeedbackDto dto, int userId);

    }

}
