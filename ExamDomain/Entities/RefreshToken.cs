using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    // YENI
    public class RefreshToken : AuditableEntity
    {
        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByToken { get; set; }

        public string? RevocationReason { get; set; }

        public User User { get; set; } = null!;

        // YENI
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        // YENI
        public bool IsRevoked => RevokedAt.HasValue;

        // YENI
        public bool IsActive => !IsExpired && !IsRevoked;
    }
}
