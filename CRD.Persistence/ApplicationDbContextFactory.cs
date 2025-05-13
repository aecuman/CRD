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
                UseSqlServer(@"Server=localhost,1433;Database=crd-mssql-db;User Id=sa;Password=Str0p@ssword;Encrypt=false");
               // UseSqlServer(@"Server=tcp:redah-db.database.windows.net,1433;Initial Catalog=crd;Persist Security Info=False;User ID=CloudSAb2bf6a20;Password=Str0p@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            /*.UseSeeding((context, _) => {

            });*/

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
