using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class CRDFile
    {
        public int Id { get; set; }
        public string FileClass { get; set; }
        public int RefId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
