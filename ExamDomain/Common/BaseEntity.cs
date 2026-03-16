using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; } // Bütün entity-lər üçün əsas primary key (unikal identifikator)
    }
}
