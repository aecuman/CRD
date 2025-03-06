using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Identity
{
    public class ApplicationRole:IdentityRole<int>
    {
        /// <summary>
        /// Navigation property for the users in this role.
        /// </summary>
        public virtual ICollection<IdentityUserRole<int>> Users { get; set; } =  new HashSet<IdentityUserRole<int>>();
    }
}
