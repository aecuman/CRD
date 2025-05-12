using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class Crop : Plant
    {
       
        public string? CropType { get; set; }
        //AEZ Agroeconomic Zone
        public string? Aez { get; set; }
    }
    public enum CropType
    {
        PERENIAL,ANNUAL
    }
}
