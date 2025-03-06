using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common.Dtos
{
    public class StructureViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StructureCategoryDto Category { get; set; }
        public StructureTypeDto StructureType { get; set; }
        public List<StructureAttributeSelectionDto> AttributeSelections { get; set; } = new List<StructureAttributeSelectionDto>();
    }

    public class StructureTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StructureCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StructureAttributeSelectionDto
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public List<StructureOptionDto> SelectedOptions { get; set; } = new List<StructureOptionDto>();
    }

    public class StructureOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
