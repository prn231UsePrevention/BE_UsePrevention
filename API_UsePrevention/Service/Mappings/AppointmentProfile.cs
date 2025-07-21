using AutoMapper;
using Dto.Request;
using Dto.Response;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Consultant, ConsultantResponseDto>()
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"))
                    .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree ?? "N/A"))
                    .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialty ?? "N/A"));

            CreateMap<CreateSlotRequestDto, Appointment>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.StartTime));

            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.Ignore())
                .ForMember(dest => dest.ConsultantFullName, opt => opt.Ignore())
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.DateTime.AddHours(1)));

            CreateMap<Appointment, RevisitAppointmentResponseDto>();
        }
    }
    }
