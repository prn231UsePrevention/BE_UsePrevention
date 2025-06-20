using Microsoft.EntityFrameworkCore;
using Repository.Models;
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

        public UnitOfWork(DrugUsePreventionSupportSystemContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }


    }
}
