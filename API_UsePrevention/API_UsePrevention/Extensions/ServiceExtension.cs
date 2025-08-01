﻿using Microsoft.AspNetCore.Cors.Infrastructure;
using Repository.UWO;
using Service.Interface;
using Service.Interface.Service.Interface;
using Service.Service;

namespace API_UsePrevention.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICommunityProgramService, CommunityProgramService>();
            services.AddScoped<IParticipationService, ParticipationService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IConsultantService, ConsultantService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IAssessmentQuestionService, AssessmentQuestionService>();
            services.AddScoped<IAssessmentAnswerService, AssessmentAnswerService>();

            return services;
        }
    }
}
