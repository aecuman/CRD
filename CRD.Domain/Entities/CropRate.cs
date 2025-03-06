using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class CropRate:Rate
    {
        public string GrowthStageId { get; set; }
        public string Quality { get; set; }
    }
}
