using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain
{
    // Entities
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }

    public abstract class AuditableEntity : ISoftDeletable
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class AuditLog : AuditableEntity
    {
        public string Action { get; set; }
        public string EntityName { get; set; }
    }
}
