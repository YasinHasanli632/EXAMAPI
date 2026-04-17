using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    // YENI
    // YENI
    public class AttendanceChangeRequest : AuditableEntity
    {
        public int ClassRoomId { get; set; }
        public ClassRoom ClassRoom { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        // YENI
        // Keçmiş attendance hansı tarix üçündür
        public DateTime AttendanceDate { get; set; }

        // YENI
        // Hazırkı saxlanılmış status
        public AttendanceStatus CurrentStatus { get; set; }

        // YENI
        // Müəllimin dəyişmək istədiyi status
        public AttendanceStatus RequestedStatus { get; set; }

        // YENI
        // Müəllimin sorğu səbəbi
        public string RequestedChangeReason { get; set; } = string.Empty;

        // YENI
        // Əgər yeni status Absent-dirsə
        public string? RequestedAbsenceReasonType { get; set; }

        // YENI
        public string? RequestedAbsenceReasonNote { get; set; }

        // YENI
        // Əgər yeni status Late-dirsə
        public TimeSpan? RequestedLateArrivalTime { get; set; }

        // YENI
        public string? RequestedLateNote { get; set; }

        public int RequestedByTeacherId { get; set; }
        public Teacher RequestedByTeacher { get; set; } = null!;

        public DateTime RequestedAt { get; set; }

        // YENI
        // Pending / Approved / Rejected
        public string RequestStatus { get; set; } = "Pending";

        public int? ReviewedByAdminId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
    }
}
