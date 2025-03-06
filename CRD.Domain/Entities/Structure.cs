using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class Structure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StructureTypeId { get; set; }
        public int CategoryId { get; set; }

        public virtual StructureType StructureType { get; set; }
        public virtual StructureCategory Category { get; set; }
        public virtual ICollection<StructureAttributeSelection> AttributeSelections { get; set; } = new List<StructureAttributeSelection>();
    }

    public class StructureAttributeSelection
    {
        public int Id { get; set; }
        public int StructureId { get; set; }
        public int AttributeId { get; set; }

        public virtual StructureAttribute Attribute { get; set; }
        public virtual Structure Structure { get; set; }
        public virtual ICollection<StructureOptionSelection> OptionSelections { get; set; } = new List<StructureOptionSelection>();
    }

    public class StructureOptionSelection
    {
        public int Id { get; set; }
        public int AttributeSelectionId { get; set; }
        public int OptionId { get; set; }

        public virtual StructureOption Option { get; set; }
        public virtual StructureAttributeSelection AttributeSelection { get; set; }
    }

    public class StructureCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StructureAttribute> Attributes { get; set; } = new List<StructureAttribute>();
    }

    public class StructureType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StructureAttribute
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual StructureCategory Category { get; set; }
        public virtual ICollection<StructureOption> Options { get; set; } = new List<StructureOption>();
    }

    public class StructureOption
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Name { get; set; }

        public virtual StructureAttribute Attribute { get; set; }
    }
}

