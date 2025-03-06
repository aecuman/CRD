using CRD.Domain.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class Plant
    {
        public int Id { get; set; }
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public virtual List<Translation> Translations { get; set; }=new List<Translation>();
        public virtual List<int> GrowthStages { get; set; }=new List<int>();
        public virtual List<int> Categories { get; set; }=new List<int>();
        public virtual List<CategoryInfo> CategoryInform {  get; set; } =new List<CategoryInfo>();
        public string Info { get; set; }
        public virtual List<CRDFile> Images { get; set; } = new List<CRDFile>();
    }
}
