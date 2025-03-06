using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class TreeRate:Rate
    {
        public string CategoryId { get; set; }
        public string CategoryValue { get; set; }
        public string GrowthStageId { get; set; }
        public string Quality { get; set; }
    }
}
