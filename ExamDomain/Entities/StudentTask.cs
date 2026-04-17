using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class StudentTask : AuditableEntity
    {
        public int StudentId { get; set; }

        public int? SubjectId { get; set; }

        public int? TeacherId { get; set; }

        // YENI
        // Eyni sinif taskının bütün şagirdlərdə ortaq açarı
        public string TaskGroupKey { get; set; } = null!;

        // YENI
        // Task hansı sinif üçün yaradılıb
        public int? ClassRoomId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime DueDate { get; set; }

        public StudentTaskStatus Status { get; set; } = StudentTaskStatus.Pending;

        public decimal Score { get; set; } = 0;

        public decimal MaxScore { get; set; } = 100;

        // Müəllimin task üçün verdiyi link
        public string? Link { get; set; }

        // Müəllimin task üçün qeyd/açıqlama hissəsi
        public string? Note { get; set; }

        // YENI
        // Şagirdin submit etdiyi mətn cavabı
        public string? SubmissionText { get; set; }

        // YENI
        // Şagirdin submit etdiyi link
        public string? SubmissionLink { get; set; }

        // YENI
        // Şagirdin submit etdiyi fayl linki/path-i
        public string? SubmissionFileUrl { get; set; }

        // YENI
        // Şagirdin təhvil vermə vaxtı
        public DateTime? SubmittedAt { get; set; }

        // YENI
        // Müəllimin yazdığı feedback / bal izahı
        public string? Feedback { get; set; }

        // YENI
        // Müəllimin yoxlama vaxtı
        public DateTime? CheckedAt { get; set; }

        // YENI
        // Həmin task sinifdə yaradılarkən sıra/yenilik üçün istifadə oluna bilər
        public bool IsActive { get; set; } = true;

        public Student Student { get; set; } = null!;

        public Subject? Subject { get; set; }

        public Teacher? Teacher { get; set; }

        // YENI
        public ClassRoom? ClassRoom { get; set; }
    }
}

