using CRD.Domain.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Entities
{
    public class DistrictRate:AuditableEntity
    {
        //public int Id { get; set; }

        // The district this rate applies to
        public int DistrictId { get; set; }
        public virtual District District { get; set; }

        // The year this rate is valid for (applies for 2 years)
        public int Year { get; set; }
        


        // Approval status
        public DistrictRateStatus Status { get; set; }

        // Uploads (Supporting Documents)
        public List<int> UploadIds { get; set; } = new List<int>();
        public virtual ICollection<CRDFile> Uploads { get; } = new HashSet<CRDFile>();

        // All compensation rates linked to this district rate
        public virtual ICollection<PlantRate> PlantRates { get; set; } = new List<PlantRate>();
        public virtual ICollection<StructureRate> StructureRates { get; set; } = new List<StructureRate>();

        public List<int>?ComparableDistrictRatesIds { get; set; } = new List<int>();
        public virtual ICollection<DistrictRate> ComparableStrictRates {  get; set; } = new List<DistrictRate>();
    }

    // Enum for status tracking
    public enum DistrictRateStatus
    {
        Pending,
        Approved,
        Rejected,
        UnderReview,
        Published
    }



    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
         
    }
}
