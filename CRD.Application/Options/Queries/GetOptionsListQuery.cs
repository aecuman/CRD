using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Options.Queries
{
    public class GetOptionsListQuery:IRequest<OptionsListViewModel>
    {
        public string Option { get; set; }
    }

    public class GetOptionsListQueryHandler : IRequestHandler<GetOptionsListQuery, OptionsListViewModel>
    {
        private readonly IRepository<StructureCategory> _structureCategoryRepo;
        private readonly IRepository<StructureType> _structureTypeRepo;
       // private readonly IRepository<StructureDescriptionOption> _structureDescriptionOptionRepo;
       // private readonly IRepository<StructureDescriptionName> _structureDescriptionNameRepo;
        private readonly IRepository<Category> _categoryCropRepo;
        private readonly IRepository<Languange> _languangeRepo;
        private readonly IRepository<GrowthStage> _growthStageRepo;

        public GetOptionsListQueryHandler(IRepository<StructureCategory> structureCategoryRepo, IRepository<StructureType> structureTypeRepo,/* IRepository<StructureDescriptionOption> structureDescriptionOptionRepo, IRepository<StructureDescriptionName> structureDescriptionNameRepo,*/ IRepository<Category> categoryCropRepo, IRepository<Languange> languangeRepo, IRepository<GrowthStage> growthStageRepo)
        {
            _structureCategoryRepo = structureCategoryRepo;
            _structureTypeRepo = structureTypeRepo;
           // _structureDescriptionOptionRepo = structureDescriptionOptionRepo;
           // _structureDescriptionNameRepo = structureDescriptionNameRepo;
            _categoryCropRepo = categoryCropRepo;
            _languangeRepo = languangeRepo;
            _growthStageRepo = growthStageRepo;
        }

        public Task<OptionsListViewModel> Handle(GetOptionsListQuery request, CancellationToken cancellationToken)
        {
            /*var categories = _structureCategoryRepo.IncludeMultiple(_structureCategoryRepo.GetAll().AsQueryable(), x => x.StructureCategoryOptions);
            var categoryOptions=categories.Select(c => new StructureCategoryListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                StructureCategories = c.StructureCategoryOptions.Select(cs => new StructureDescriptionNameListViewModel()
                {
                    Name = cs.Name,
                    Id = cs.Id,
                    StructureCategoryId=c.Id,
                    Options= _structureDescriptionOptionRepo.GetAll().Where(x=>x.StructureDescriptionNameId==cs.Id).Select(cso => new StructureDescriptionOptionListViewModel()
                    {
                        Id=cso.Id,
                        Name=cso.Name,
                        StructureDescriptionNameId=cs.Id,
                    }).ToList()
                }).ToList()
            }).ToList();*/

            var structureCategories = _structureCategoryRepo.GetAll().AsQueryable()
            .Include(sc => sc.Attributes)
                .ThenInclude(dn => dn.Options)
            .ToList();

            var structureTypes = _structureTypeRepo.GetAll();// await _context.StructureTypes.ToListAsync(cancellationToken);

/*
            var structure_categories = structureCategories.Select(sc => new StructureCategoryViewModel
            {
                Id = sc.Id,
                Name = sc.Name,
                Attributes = sc.Attributes.Select(dn => new StructureAttributeViewModel
                {
                    Id = dn.Id,
                    Name = dn.Name,
                    Options = dn.Options.Select(opt => new StructureOptionViewModel
                    {
                        Id = opt.Id,
                        Name = opt.Name
                    }).ToList()
                }).ToList()
            }).ToList();
            var structure_types = structureTypes.Select(st => new StructureTypeViewModel
            {
                Id = st.Id,
                Name = st.Name
            }).ToList();
           
            */

            var entity = new OptionsListViewModel()
            {
                GrowthStages = _growthStageRepo.GetAll().Select(x=> new OptionViewModel() { Id=x.Id,Name=x.Name}).ToList(),
               // StructureCategories = structure_categories,
               // StructureTypes = structure_types,
               // StructureCategoryOptions = categoryOptions,
                Languanges = _languangeRepo.GetAll().Select(x => new OptionViewModel() { Id = x.Id, Name = x.Name }).ToList(),
                PlantCategories = _categoryCropRepo.GetAll().Select(x => new OptionViewModel() { Id = x.Id, Name = x.Name }).ToList(),
                //StructureCategories = _structureCategoryRepo.GetAll().Select(x => new OptionViewModel() { Id = x.Id, Name = x.Name }).ToList(),
                //StructureDescriptionNames = _structureDescriptionNameRepo.GetAll().Select(x => new OptionViewModel() { Id = x.Id, Name = x.Name }).ToList(),
                //StructureDescriptionOptions = _structureDescriptionOptionRepo.GetAll().Select(x => new OptionViewModel() { Id = x.Id, Name = x.Name }).ToList(),
               // StructureTypes = _structureTypeRepo.GetAll().Select(x => new OptionViewModel() { Id = x.Id, Name = x.Name }).ToList()
            };
           return Task.FromResult(entity);
        }
    }

    public class OptionsListViewModel
    {
       // public List<OptionViewModel> StructureCategories { get; set; }=new List<OptionViewModel>();
        //public List<StructureCategoryListViewModel> StructureCategoryOptions { get; set; } = new List<StructureCategoryListViewModel>();
        //public List<OptionViewModel> StructureTypes { get; set; } = new List<OptionViewModel>();
       // public List<OptionViewModel> StructureDescriptionOptions { get; set; } = new List<OptionViewModel>();
       // public List<OptionViewModel> StructureDescriptionNames { get; set; } = new List<OptionViewModel>();
        public List<OptionViewModel> PlantCategories { get; set; }= new List<OptionViewModel>();
        public List<OptionViewModel> Languanges { get; set; } = new List<OptionViewModel>();
        public List<OptionViewModel> GrowthStages { get; set; }=new List<OptionViewModel>();
       // public List<StructureCategoryViewModel> StructureCategories { get; set; } = new List<StructureCategoryViewModel>();
       // public List<StructureTypeViewModel> StructureTypes { get; set; } = new List<StructureTypeViewModel>();
    }

    
    public class OptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    /* public class StructureCategoryListViewModel
     {
         public int Id { get; set; }
         public string Name { get; set; }
         public List<StructureDescriptionNameListViewModel> StructureCategories { get; set; } = new List<StructureDescriptionNameListViewModel>();
     }
     public class StructureDescriptionNameListViewModel
     {
         public int Id { get; set; }
         public int StructureCategoryId { get; set; }
         public string Name { get; set; }
         public List<StructureDescriptionOptionListViewModel> Options { get; set; } = new List<StructureDescriptionOptionListViewModel>();
     }
     public class StructureDescriptionOptionListViewModel
     {
         public int Id { get; set; }
         public int StructureDescriptionNameId {  get; set; }
         public string Name { get; set; }

     }
    */
  /*  public class StructureCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StructureAttributeViewModel> Attributes { get; set; } = new List<StructureAttributeViewModel>();
    }

    public class StructureTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StructureAttributeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StructureOptionViewModel> Options { get; set; } = new List<StructureOptionViewModel>();
    }

    public class StructureOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }*/
}
