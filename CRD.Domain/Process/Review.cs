using CRD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Process
{
    public class Review
    {
        public Review() {
            Trees = new List<TreeRateStatus>();
            Crops = new List<CropRateStatus>();
            Structures = new List<StructureRateStatus>();
            Info = new List<StageInfo>();
            ReviewRoles = new List<ReviewRole>();
        }
        public int Id { get; set; }
        public string DistrictRateId { get; set; }
        public int CurrentStageId { get; set; }
        public DateTime? ModerationDate { get; set; }
        public List<TreeRateStatus> Trees { get; set; }
        public List<CropRateStatus> Crops { get; set; }
        public List<StructureRateStatus> Structures { get; set; }
        public List<StageInfo> Info { get; set; }
        public List<ReviewRole> ReviewRoles { get; set; }

    }

    public class ReviewRole
    {
        public ReviewRole()
        {
            Users = new List<string>();
        }
        public int Id { get; set; }
        public string RoleName { get;set; }
        public List<string> Users { get; set; }
    }

    public class TreeRateStatus:TreeRate
    {
        public string Status { get; set; }
        public bool? IsAccepted { get; set; }
    }
    public class CropRateStatus : CropRate
    {
        public string Status { get; set; }
        public bool? IsAccepted { get; set; }
    }
    public class StructureRateStatus:StructureRate
    {
        public string Status { get; set; }
        public bool? IsAccepted { get; set; }
    }
    public class StageInfo
    {
        public StageInfo() {

            Uploads = new List<CRDFile>();
          
                }
        public int Id { get; set; }
        public int StageId { get; set; }  
        public string Comment { get; set; }
        public List<CRDFile> Uploads { get; set; }
    }

}
