using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // YENI - SAGIRD UCUN
    public class StudentTaskSummaryDto
    {
        public int TotalCount { get; set; }

        public int PendingCount { get; set; }

        public int SubmittedCount { get; set; }

        public int ReviewedCount { get; set; }

        public int LateCount { get; set; }

        public int MissingCount { get; set; }
    }
}
