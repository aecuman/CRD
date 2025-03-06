using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class Tree : Plant
    {
        public List<string> Othernames { get; set; } = new List<string>();
    }
}
