
using CRD.Domain;
using CRD.Domain.Entities;
using CRD.Domain.Identity;
using CRD.Domain.Process;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public DbSet<PlantRate> PlantRates { get; set; }
        public DbSet<PlantRateGroup> PlantRateGroups { get; set; }
        public DbSet<CompensationRateModeration> CompensationRateModerations { get; set; }
        public DbSet<GroupedPlants> GroupedPlants { get; set; }
        public DbSet<GroupedPlantItem> GroupedPlantItems { get; set; }

        public DbSet<StructureRate> StructureRates { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<DistrictWorkflow> DistrictWorkflows { get; set; }
        public DbSet<DistrictWorkflowStep> DistrictWorkflowSteps { get; set; }
        public DbSet<DistrictWorkflowSubStep> DistrictWorkflowSubSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // ✅ Map DbSet<Workflow> to table "Workflow" (singular, as created in migration)
            builder.Entity<Workflow>().ToTable("Workflow");

            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Crop>().HasMany(u=>u.Translations).WithOne().HasForeignKey(c=>c.CropId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Crop>().HasMany(u => u.CategoryInform).WithOne().HasForeignKey(c => c.CropId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Crop>().Property(p => p.RowVersion).IsRowVersion();
            builder.Entity<Tree>().HasMany(u => u.Translations).WithOne().HasForeignKey(c => c.TreeId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Tree>().HasMany(u => u.CategoryInform).WithOne().HasForeignKey(c => c.TreeId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Tree>().Property(p => p.RowVersion).IsRowVersion();

            builder.Entity<Crop>()
                .HasMany(p => p.GroupedPlantItems)
          .WithOne(gp => gp.Crop)
          .HasForeignKey(gp => gp.CropId)
          .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tree>()
                .HasMany(t => t.GroupedPlantItems)
          .WithOne(g => g.Tree)
          .HasForeignKey(g => g.TreeId)
          .OnDelete(DeleteBehavior.Restrict);
       

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
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StructureOptionSelection>()
                .HasOne(s => s.AttributeSelection)
                .WithMany(s => s.OptionSelections)
                .HasForeignKey(s => s.AttributeSelectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // builder.Entity<Structure>().HasMany(u => u.Description).WithOne().HasForeignKey(c => c.).IsRequired().OnDelete(DeleteBehavior.Cascade);

            //Workflow
            builder.Entity<Workflow>().HasMany(w => w.Steps).WithOne(ws => ws.Workflow).HasForeignKey(ws => ws.WorkflowId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WorkflowStep>()
            .HasOne(ws => ws.Workflow)
            .WithMany(w => w.Steps)
            .HasForeignKey(ws => ws.WorkflowId)
            .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<WorkflowStep>().Property(ws => ws.AssignedToRoles)
       .HasConversion(
           v => string.Join(",", v),
           v => v.Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList()
       );

            builder.Entity<WorkflowStep>().HasOne(ws => ws.Workflow)
                   .WithMany(w => w.Steps)
                   .HasForeignKey(ws => ws.WorkflowId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkflowStep>().HasMany(ws => ws.SubSteps)
                   .WithOne(ss => ss.WorkflowStep)
                   .HasForeignKey(ss => ss.WorkflowStepId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<WorkflowSubStep>().Property(wss => wss.AssignedToRoles)
       .HasConversion(
           v => string.Join(",", v),
           v => v.Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList()
       );

            builder.Entity<WorkflowSubStep>().HasOne(wss => wss.WorkflowStep)
            .WithMany(ws => ws.SubSteps)
            .HasForeignKey(wss => wss.WorkflowStepId)
            .OnDelete(DeleteBehavior.NoAction); 
            builder.Entity<WorkflowSubStep>().Property(wss => wss.AssignedToRoles).IsRequired();

            //district workflow
            builder.Entity<DistrictWorkflow>().HasOne(d => d.District)
           .WithMany()
           .HasForeignKey(d => d.DistrictId)
           .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DistrictWorkflow>().HasOne(d => d.DistrictRate)
          .WithMany()
          .HasForeignKey(d => d.DistrictRateId)
          .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DistrictWorkflow>().HasOne(d => d.Workflow)
                .WithMany(w => w.DistrictWorkflows)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DistrictWorkflowStep>().HasOne(dws => dws.WorkflowStep)
    .WithMany()
    .HasForeignKey(dws => dws.WorkflowStepId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DistrictWorkflowStep>().HasOne(dws => dws.DistrictWorkflow)
                .WithMany(dw => dw.DistrictSteps)
                .HasForeignKey(dws => dws.DistrictWorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DistrictWorkflowSubStep>().HasOne(dwss => dwss.WorkflowSubStep)
    .WithMany()
    .HasForeignKey(dwss => dwss.WorkflowSubStepId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DistrictWorkflowSubStep>().HasOne(dwss => dwss.DistrictWorkflowStep)
                .WithMany(dws => dws.DistrictSubSteps)
                .HasForeignKey(dwss => dwss.DistrictWorkflowStepId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Comment>().HasOne(c => c.DistrictWorkflowStep)
    .WithMany(dws => dws.Comments)
    .HasForeignKey(c => c.DistrictWorkflowStepId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>().HasOne(c => c.DistrictWorkflowSubStep)
                .WithMany(dwss => dwss.Comments)
                .HasForeignKey(c => c.DistrictWorkflowSubStepId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Document>().HasOne(d => d.DistrictWorkflowStep)
    .WithMany(dws => dws.Documents)
    .HasForeignKey(d => d.DistrictWorkflowStepId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Document>().HasOne(d => d.DistrictWorkflowSubStep)
                .WithMany(dwss => dwss.Documents)
                .HasForeignKey(d => d.DistrictWorkflowSubStepId)
                .OnDelete(DeleteBehavior.NoAction);
            /* builder.Entity<DistrictRate>(entity =>
             {


                 entity.HasOne(e => e.District).WithMany().HasForeignKey(f => f.DistrictId).OnDelete(DeleteBehavior.Restrict);


             });*/
            // CompensationRate → Plant(One - to - Many)
            /* builder.Entity<PlantRate>()
                 .HasOne(cr => cr.Plant)
                 .WithMany(p => p.PlantRates)
                 .HasForeignKey(cr => cr.PlantId)
                 .OnDelete(DeleteBehavior.Cascade);*/
            // ✅ Configure Many-to-Many Relationship for Grouped Plants using PlantRateGroup

            builder.Entity<PlantRateGroup>()
                .HasOne(pg => pg.PlantRate)
                .WithMany(pr => pr.PlantRateGroups) // Links to PlantRate
                .HasForeignKey(pg => pg.PlantRateId)
                .OnDelete(DeleteBehavior.NoAction); // ✅ Deleting a PlantRate removes junction table references, but not Plants

            builder.Entity<PlantRateGroup>()
                .HasOne(pg => pg.Plant)
                .WithMany() // Links to Plant
                .HasForeignKey(pg => pg.PlantId)
                .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents delete cycles and multiple cascade paths

            // ✅ Ensure PlantRate → Plant (Single FK) does not interfere with GroupedPlants relationship
            builder.Entity<PlantRate>()
                .HasOne(pr => pr.Plant)
                .WithMany(p => p.PlantRates)
                .HasForeignKey(pr => pr.PlantId)
                .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents multiple cascade paths
            builder.Entity<PlantRate>()
    .HasOne(p => p.GroupedPlants)
    .WithMany()
    .HasForeignKey(p => p.GroupedPlantId)
    .OnDelete(DeleteBehavior.Restrict);

       

            // 📌 CompensationRateModeration → PlantRate (One-to-Many, Optional)
            builder.Entity<CompensationRateModeration>()
                .HasOne(m => m.PlantRate)
                .WithMany(p => p.ModerationHistory)
                .HasForeignKey(m => m.PlantRateId)
                .OnDelete(DeleteBehavior.Cascade); // Or use Restrict depending on preference

            // 📌 CompensationRateModeration → StructureRate (One-to-Many, Optional)
            builder.Entity<CompensationRateModeration>()
                .HasOne(m => m.StructureRate)
                .WithMany(s => s.ModerationHistory)
                .HasForeignKey(m => m.StructureRateId)
                .OnDelete(DeleteBehavior.Restrict); // Same here – use Restrict if needed

            // 📌 DistrictRate → District (One-to-Many)
            builder.Entity<DistrictRate>()
                .HasOne(dr => dr.District)
                .WithMany()
                .HasForeignKey(dr => dr.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DistrictRate>()
    .HasMany(dr => dr.PlantRates)
    .WithOne(pr => pr.DistrictRate)
    .HasForeignKey(pr => pr.DistrictRateId);

            builder.Entity<DistrictRate>()
                .HasMany(dr => dr.StructureRates)
                .WithOne(sr => sr.DistrictRate)
                .HasForeignKey(sr => sr.DistrictRateId);

            // 📌 CategoryInfo → Category (One-to-Many)
            builder.Entity<CategoryInfo>()
                .HasOne(ci => ci.Category)
                .WithMany()
                .HasForeignKey(ci => ci.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📌 Configure Default Values
            builder.Entity<PlantRate>()
                .Property(cr => cr.Status)
                .HasDefaultValue(ModerationStatus.Pending);

            builder.Entity<GroupedPlantItem>()
            .HasKey(x => x.Id);

            builder.Entity<GroupedPlantItem>()
                .HasOne(x => x.GroupedPlant)
                .WithMany(g => g.GroupedPlantItems)
                .HasForeignKey(x => x.GroupedPlantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GroupedPlantItem>()
                .HasOne(x => x.Crop)
                .WithMany(p => p.GroupedPlantItems)
                .HasForeignKey(x => x.CropId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<GroupedPlantItem>()
                .HasOne(x => x.Tree)
                .WithMany(p => p.GroupedPlantItems)
                .HasForeignKey(x => x.TreeId)
                .OnDelete(DeleteBehavior.Restrict);

            // GroupedPlants config
            builder.Entity<GroupedPlants>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.Property(x => x.Description).HasMaxLength(500);
            });



            // Required Relationship to Structure
            builder.Entity<StructureRate>().HasOne(x => x.Structure)
                   .WithMany()
                   .HasForeignKey(x => x.StructureId)
                   .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
            // Apply a global query filter
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Check if the entity type implements ISoftDeletable
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    // Create a parameter expression for the entity
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // Build the condition: e => !e.IsDeleted
                    var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                    var condition = Expression.Lambda(
                        Expression.Not(property),
                        parameter
                    );

                    // Apply the filter to the model
                    builder.Entity(entityType.ClrType).HasQueryFilter(condition);
                }
            }
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted || e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
               
                    if (entry.Entity is AuditableEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedAt = DateTime.UtcNow; // Ensure CreatedAt is set
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.UpdatedAt = DateTime.UtcNow; // Update timestamp when modified
                        }

                        if (entry.State == EntityState.Deleted)
                        {
                            entity.IsDeleted = true;
                            entry.State = EntityState.Modified; // Avoid hard deletion
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
