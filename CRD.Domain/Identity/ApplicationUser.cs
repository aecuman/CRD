using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Identity
{
    public class ApplicationUser:IdentityUser<int>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<int>> Roles { get; } 
    }
}
