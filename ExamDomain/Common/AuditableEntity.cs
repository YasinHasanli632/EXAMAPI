using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; } // Obyektin yaradıldığı tarix və vaxt

        public int? CreatedByUserId { get; set; } // Obyekti yaradan istifadəçinin Id-si

        public DateTime? UpdatedAt { get; set; } // Obyektin son dəyişdirildiyi tarix və vaxt

        public int? UpdatedByUserId { get; set; } // Obyekti son dəyişdirən istifadəçinin Id-si
    }
}
