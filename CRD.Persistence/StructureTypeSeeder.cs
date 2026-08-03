using CRD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CRD.Persistence
{
    public class StructureTypeSeeder
    {
        private readonly ApplicationDbContext _context;

        public StructureTypeSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDefaultStructureTypesAsync()
        {
            // Check if structure types already exist
            if (await _context.StructureTypes.AnyAsync())
                return;

            var structureTypes = new[]
            {
                new StructureType { Id = 1, Name = "Permanent" },
                new StructureType { Id = 2, Name = "Semi-Permanent" }
            };

            // Add structure types
            _context.StructureTypes.AddRange(structureTypes);
            await _context.SaveChangesAsync();
        }
    }
}
