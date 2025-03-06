using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common
{
    public interface ICurrentUser
    {
        public string UserId { get; }
        public bool IsAuthenticated { get; }
    }
}
