using AutoMapper;
using Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models;
namespace Dto.Mappings
{
    public class ParticipationProfile : Profile
    {
        public ParticipationProfile()
        {
            CreateMap<Participation, ParticipationDto>();

            CreateMap<ParticipationCreateUpdateDto, Participation>();
        }
    }
}
