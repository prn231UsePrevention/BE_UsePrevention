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
    internal class ConsultantProfile : Profile
    {
        public ConsultantProfile()
        {
            CreateMap<ConsultantCreateDto, Consultant>();
            CreateMap<ConsultantUpdateDto, Consultant>();
        }
    
    }
}
