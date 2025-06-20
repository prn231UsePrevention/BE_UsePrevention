using AutoMapper;
using Dto.Request;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mappings
{
    public class ParticipationProfile : Profile
    {
        public ParticipationProfile()
        {
            // Mapping từ entity sang DTO
            CreateMap<Participation, ParticipationDto>();

            // Mapping từ DTO tạo/cập nhật sang entity
            CreateMap<ParticipationCreateUpdateDto, Participation>();
        }
    }
}
