using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UWO
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DrugUsePreventionSupportSystemContext _context;

        

        public IGenericRepository<User> User { get; }

        public IGenericRepository<Role> Role { get; }

        public IGenericRepository<Participation> Participation { get; }

        public IGenericRepository<Enrollment> Enrollment { get; }

        public IGenericRepository<Course> Course { get; }

        public IGenericRepository<Consultant> Consultant { get; }

        public IGenericRepository<CommunityProgram> CommunityProgram { get; }

        public IGenericRepository<BlogPost> BlogPost { get; }

        public IGenericRepository<AssessmentResult> AssessmentResult { get; }

        public IGenericRepository<Assessment> Assessment { get; }

        public IGenericRepository<Appointment> Appointment { get; }

        public IGenericRepository<AssessmentQuestion> AssessmentQuestion { get; }

        public IGenericRepository<AssessmentAnswer> AssessmentAnswer { get; }

        public IGenericRepository<CourseRating> CourseRating { get; }
        public IGenericRepository<Feedback> Feedback { get; }
        public IGenericRepository<CourseModule> CourseModule { get; }
        public IGenericRepository<CourseLesson> CourseLesson { get; }
        public IGenericRepository<Result> Result { get;  }
        public UnitOfWork(DrugUsePreventionSupportSystemContext context)
        {
            _context = context;
            User = new GenericRepository<User>(_context);
            Role = new GenericRepository<Role>(_context);
            Participation = new GenericRepository<Participation>(_context);
            Enrollment = new GenericRepository<Enrollment>(_context);
            Course = new GenericRepository<Course>(_context);
            Consultant = new GenericRepository<Consultant>(_context);
            CommunityProgram = new GenericRepository<CommunityProgram>(_context);
            BlogPost = new GenericRepository<BlogPost>(_context);
            AssessmentResult = new GenericRepository<AssessmentResult>(_context);
            Assessment = new GenericRepository<Assessment>(_context);
            Appointment = new GenericRepository<Appointment>(_context);
            AssessmentQuestion = new GenericRepository<AssessmentQuestion>(_context);
            AssessmentAnswer = new GenericRepository<AssessmentAnswer>(_context);
            Feedback = new GenericRepository<Feedback>(_context);
            Result = new GenericRepository<Result>(_context);
            CourseRating = new GenericRepository<CourseRating>(_context);
            CourseLesson = new GenericRepository<CourseLesson>(_context);
            CourseModule = new GenericRepository<CourseModule>(_context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
