using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repository.Models
{
    public class DrugUsePreventionSupportSystemContextFactory : IDesignTimeDbContextFactory<DrugUsePreventionSupportSystemContext>
    {
       
       
        public DrugUsePreventionSupportSystemContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DrugUsePreventionSupportSystemContext>();
            optionsBuilder.UseSqlServer("Data Source=PRN232_Drug.mssql.somee.com;Initial Catalog=PRN232_Drug;Persist Security Info=True;User ID=trung123_SQLLogin_1;Password=123456789;Encrypt=False;Trust Server Certificate=True");

            return new DrugUsePreventionSupportSystemContext(optionsBuilder.Options);
        }
    }
}
