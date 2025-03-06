using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class Rate
    {
        public int Id { get; set; }
        public int DistrictRateId { get; set; }
        public string ItemId { get; set; }
        public string UnitId { get; set; }
        public string Assumption { get; set; }
        public int? Amount { get; set; }
    }
}
