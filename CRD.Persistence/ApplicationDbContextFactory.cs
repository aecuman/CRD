using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder= new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.
                UseSqlServer(@"Server=localhost,1433;Database=crd-test-db;;User Id=sa;Password=Str0p@ssword;Encrypt=False");
                /*.UseSeeding((context, _) => {
                
                });*/
            
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
