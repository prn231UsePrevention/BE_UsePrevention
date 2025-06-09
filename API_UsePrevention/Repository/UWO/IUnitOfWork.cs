using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models;
using Repository.Repositories;

namespace Repository.UWO
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> User { get; }
        IGenericRepository<Role> Role { get; }
        IGenericRepository<Participation> Participation { get; }
        IGenericRepository<Enrollment> Enrollment { get; }
        IGenericRepository<Course> Course { get; }
        IGenericRepository<Consultant> Consultant { get; }
        IGenericRepository<CommunityProgram> CommunityProgram { get; }
        IGenericRepository<BlogPost> BlogPost { get; }
        IGenericRepository<AssessmentResult> AssessmentResult { get; }
        IGenericRepository<Assessment> Assessment { get; }
        IGenericRepository<Appointment> Appointment { get; }
        IGenericRepository<AssessmentQuestion> AssessmentQuestion { get; }
        IGenericRepository<AssessmentAnswer> AssessmentAnswer { get; }
        Task<int> CommitAsync();
    }
}
