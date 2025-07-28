using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dto.Response;

namespace Service.Mappings
{
    internal class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseResponseDto>()
                .ForMember(dest => dest.TargetAudience,
                    opt => opt.MapFrom(src => ConvertTargetGroup(src.TargetGroup)))
                .ForMember(dest => dest.Ratings,
                    opt => opt.MapFrom(src => src.Ratings))
                .ForMember(dest => dest.Modules,
                    opt => opt.MapFrom(src => src.Modules));

            CreateMap<CourseRating, CourseRatingResponseDto>();

            CreateMap<CourseModule, CourseModuleResponseDto>()
                .ForMember(dest => dest.Lessons,
                    opt => opt.MapFrom(src => src.Lessons.OrderBy(l => l.Order)));

            CreateMap<CourseLesson, CourseLessonResponseDto>();
        }
        private static List<string> ConvertTargetGroup(string? group)
        {
            if (string.IsNullOrWhiteSpace(group))
                return new List<string>();
            return group.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
