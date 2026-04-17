using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // YENI - SAGIRD UCUN
    public class SubmitStudentTaskDto
    {
        public string? SubmissionText { get; set; }

        public string? SubmissionLink { get; set; }

        public string? SubmissionFileUrl { get; set; }
        public bool ForceAutoSubmit { get; set; } // YENI
    }
}
