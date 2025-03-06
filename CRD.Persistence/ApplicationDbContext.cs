
using CRD.Domain;
using CRD.Domain.Entities;
using CRD.Domain.Identity;
using CRD.Domain.Process;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Persistence
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<District> Districts { get; set; }

        public DbSet<Crop> Crops { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<Structure> Structures { get; set; }
        public DbSet<StructureCategory> StructureCategories { get; set; }
        public DbSet<StructureAttribute> StructureAttributes { get; set; }
        public DbSet<StructureOption> StructureOptions { get; set; }
        public DbSet<StructureAttributeSelection> StructureAttributeSelections { get; set; }
        public DbSet<StructureOptionSelection> StructureOptionSelections { get; set; }
        //public DbSet<StructureDescriptionName> StructureDescriptionNames { get; set; }
        //public DbSet<StructureDescriptionOption> StructureDescriptionOptions { get; set; }
        public DbSet<StructureType> StructureTypes { get; set; }
        public DbSet<Languange> Languanges { get; set; }
        public DbSet<GrowthStage> GrowthStages { get; set; }
        public DbSet<Category> PlantCategories { get; set; }
        public DbSet<Category> PlantInfoCategories { get; set; }
        /// <summary>
        /// Processing Rates
        /// </summary>
        /// <param name="builder"></param>
        /// 
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowSubStep> WorkflowSubSteps { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DistrictRate> DistrictRates { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CRDFile> CRDFiles { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Crop>().HasMany(u=>u.Translations).WithOne().HasForeignKey(c=>c.CropId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Crop>().HasMany(u => u.CategoryInform).WithOne().HasForeignKey(c => c.CropId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Tree>().HasMany(u => u.Translations).WithOne().HasForeignKey(c => c.TreeId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Tree>().HasMany(u => u.CategoryInform).WithOne().HasForeignKey(c => c.TreeId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Tree>().HasMany(u => u.Images).WithOne().HasForeignKey(c => c.RefId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Structure>()
            .HasOne(s => s.StructureType)
            .WithMany()
            .HasForeignKey(s => s.StructureTypeId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Structure>()
                .HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StructureAttribute>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Attributes)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StructureOption>()
                .HasOne(o => o.Attribute)
                .WithMany(a => a.Options)
                .HasForeignKey(o => o.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Structure to Category Relationship
            builder.Entity<Structure>()
                .HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CategoryId);

            // Structure to StructureType Relationship
            builder.Entity<Structure>()
                .HasOne(s => s.StructureType)
                .WithMany()
                .HasForeignKey(s => s.StructureTypeId);

            // Attribute Selection Relationship
            builder.Entity<StructureAttributeSelection>()
                .HasOne(s => s.Attribute)
                .WithMany()
                .HasForeignKey(s => s.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Option Selection Relationship
            builder.Entity<StructureOptionSelection>()
                .HasOne(s => s.Option)
                .WithMany()
                .HasForeignKey(s => s.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StructureAttributeSelection>()
                .HasOne(s => s.Structure)
                .WithMany(s => s.AttributeSelections)
                .HasForeignKey(s => s.StructureId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StructureOptionSelection>()
                .HasOne(s => s.AttributeSelection)
                .WithMany(s => s.OptionSelections)
                .HasForeignKey(s => s.AttributeSelectionId)
                .OnDelete(DeleteBehavior.NoAction);

            // builder.Entity<Structure>().HasMany(u => u.Description).WithOne().HasForeignKey(c => c.).IsRequired().OnDelete(DeleteBehavior.Cascade);

            //Workflow
            builder.Entity<WorkflowStep>()
            .HasOne(ws => ws.Workflow)
            .WithMany(w => w.Steps)
            .HasForeignKey(ws => ws.WorkflowId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WorkflowStep>().Property(ws => ws.AssignedToRoles).IsRequired();

            builder.Entity<Comment>().HasOne(c => c.WorkflowStep)
            .WithMany(ws => ws.Comments)
            .HasForeignKey(c => c.WorkflowStepId)
            .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<Comment>().HasOne(c => c.WorkflowSubStep)
            .WithMany(ws => ws.Comments)
            .HasForeignKey(c => c.WorkflowSubStepId)
            .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<WorkflowSubStep>().HasOne(wss => wss.WorkflowStep)
            .WithMany(ws => ws.SubSteps)
            .HasForeignKey(wss => wss.WorkflowStepId)
            .OnDelete(DeleteBehavior.NoAction); 
            builder.Entity<WorkflowSubStep>().Property(wss => wss.AssignedToRoles).IsRequired();


            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted || e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is ISoftDeletable entity)
                {
                    if (entry.State == EntityState.Deleted)
                    {
                        entity.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                }

                AuditLogs.Add(new AuditLog
                {
                    Action = entry.State.ToString(),
                    EntityName = entry.Entity.GetType().Name,
                    CreatedAt = DateTime.UtcNow
                });
            }
            return base.SaveChanges();
        }

    }
    
}
