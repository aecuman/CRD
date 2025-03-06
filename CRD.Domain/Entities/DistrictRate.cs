using CRD.Domain.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class DistrictRate
    {
        public DistrictRate() {

            Uploads = new List<CRDFile>(); 
                }
        public int Id { get; set; }
        public string DistrictId { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public List<CRDFile> Uploads { get; set; }
        public virtual District District { get; set; }
    }
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
         
    }
}
