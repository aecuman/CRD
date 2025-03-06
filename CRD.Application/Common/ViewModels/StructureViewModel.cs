using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common.ViewModels
{
    public class StructureCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StructureAttributeViewModel> Attributes { get; set; } = new List<StructureAttributeViewModel>();
    }
    public class StructureAttributeViewModel
    {
        public int CategoryId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StructureOptionViewModel> Options { get; set; } = new List<StructureOptionViewModel>();
    }
    public class StructureOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class StructureViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StructureCategoryViewModel Category { get; set; }
        public StructureTypeViewModel Type { get; set; }
        public List<StructureAttributeViewModel> Attributes { get; set; } = new List<StructureAttributeViewModel>();
    }

    public class StructureTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }



}
