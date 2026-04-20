using ExamApplication.DTO.Student;
using ExamApplication.Helper;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public StudentProfileService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<StudentDetailDto> GetMyProfileAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser == null || currentUser.UserId <= 0)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var student = await _unitOfWork.Students.GetByUserIdAsync(currentUser.UserId, cancellationToken);

            if (student == null)
                throw new KeyNotFoundException("Tələbə profili tapılmadı.");

            var detailedStudent = await _unitOfWork.Students.GetByIdWithDetailsAsync(student.Id, cancellationToken);

            if (detailedStudent == null)
                throw new KeyNotFoundException("Tələbə detail məlumatı tapılmadı.");

            return MapToDetailDto(detailedStudent);
        }

        public async Task<StudentDetailDto> UpdateMyProfileAsync(
            UpdateMyStudentProfileDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser == null || currentUser.UserId <= 0)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var student = await _unitOfWork.Students.GetByUserIdAsync(currentUser.UserId, cancellationToken);

            if (student == null)
                throw new KeyNotFoundException("Tələbə profili tapılmadı.");

            var user = await _unitOfWork.Users.GetByIdAsync(student.UserId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("İstifadəçi hesabı tapılmadı.");

            await ValidateUpdateAsync(student, user, request, cancellationToken);

            // YENI
            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.FatherName = request.FatherName.Trim();
            user.Email = request.Email.Trim();
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
            user.ParentPhone = string.IsNullOrWhiteSpace(request.ParentPhone) ? null : request.ParentPhone.Trim();
            user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
            user.PhotoUrl = string.IsNullOrWhiteSpace(request.PhotoUrl) ? null : request.PhotoUrl.Trim();
            user.BirthDate = request.DateOfBirth;
            user.Gender = ParseGender(request.Gender);

            // YENI
            student.FullName = !string.IsNullOrWhiteSpace(request.FullName)
                ? request.FullName.Trim()
                : $"{request.FirstName} {request.LastName}".Trim();

            student.DateOfBirth = request.DateOfBirth;
            student.StudentNumber = request.StudentNumber.Trim();
            student.Status = ParseStudentStatus(request.Status, student.Status);
            student.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            _unitOfWork.Users.Update(user);
            _unitOfWork.Students.Update(student);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetMyProfileAsync(cancellationToken);
        }

        private async Task ValidateUpdateAsync(
            Student student,
            User user,
            UpdateMyStudentProfileDto request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("Ad boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("Soyad boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.FatherName))
                throw new ArgumentException("Ata adı boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.StudentNumber))
                throw new ArgumentException("Tələbə nömrəsi boş ola bilməz.");

            var existingByEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email.Trim(), cancellationToken);
            if (existingByEmail != null && existingByEmail.Id != user.Id)
                throw new InvalidOperationException("Bu email artıq başqa istifadəçi tərəfindən istifadə olunur.");

            var existsStudentNumber = await _unitOfWork.Students.ExistsByStudentNumberAsync(
                request.StudentNumber.Trim(),
                student.Id,
                cancellationToken);

            if (existsStudentNumber)
                throw new InvalidOperationException("Bu tələbə nömrəsi artıq mövcuddur.");
        }

        private StudentDetailDto MapToDetailDto(Student student)
        {
            var activeClass = student.StudentClasses?
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.JoinedAt)
                .FirstOrDefault();

            var completedExams = student.StudentExams?
                .Where(x => x.IsCompleted)
                .ToList() ?? new List<StudentExam>();

            var attendanceRecords = student.AttendanceRecords?.ToList() ?? new List<AttendanceRecord>();
            var tasks = student.Tasks?.ToList() ?? new List<ExamDomain.Entities.StudentTask>();

            var averageScore = completedExams.Count > 0
                ? Math.Round(completedExams.Average(x => x.Score), 2)
                : 0;

            var completedTasksCount = tasks.Count(x =>
                x.Status == ExamDomain.Enum.StudentTaskStatus.Submitted ||
                x.Status == ExamDomain.Enum.StudentTaskStatus.Reviewed);

            var absentCount = attendanceRecords.Count(x => x.Status == AttendanceStatus.Absent);
            var lateCount = attendanceRecords.Count(x => x.Status == AttendanceStatus.Late);

            return new StudentDetailDto
            {
                Id = student.Id,
                UserId = student.UserId,
                FullName = student.FullName,
                FirstName = student.User?.FirstName ?? string.Empty,
                LastName = student.User?.LastName ?? string.Empty,
                Email = student.User?.Email ?? string.Empty,
                PhoneNumber = student.User?.PhoneNumber,
                ParentName = student.User?.FatherName ?? string.Empty,
                ParentPhone = student.User?.ParentPhone,
                Address = student.User?.Address,
                Gender = MapGender(student.User?.Gender),
                StudentNumber = student.StudentNumber,
                DateOfBirth = student.DateOfBirth,
                ClassName = activeClass?.ClassRoom?.Name,
                Status = MapStudentStatus(student.Status),
                Notes = student.Notes,
                AverageScore = averageScore,
                ExamsCount = completedExams.Count,
                AttendanceRate = CalculateAttendanceRate(attendanceRecords),
                PhotoUrl = student.User?.PhotoUrl,
                TasksCount = tasks.Count,
                CompletedTasksCount = completedTasksCount,
                AbsentCount = absentCount,
                LateCount = lateCount,

                Exams = completedExams
                    .OrderByDescending(x => x.StartTime)
                    .Select(x => new StudentExamSummaryDto
                    {
                        StudentExamId = x.Id,
                        ExamId = x.ExamId,
                        ExamTitle = x.Exam?.Title ?? string.Empty,
                        SubjectName = x.Exam?.Subject?.Name ?? string.Empty,
                        TeacherName = x.Exam?.Teacher?.FullName ?? string.Empty,
                        Score = x.Score,
                        MaxScore = x.Exam?.TotalScore ?? 100,
                        IsCompleted = x.IsCompleted,
                        StartTime = AzerbaijanTimeHelper.ToBakuTime(x.StartTime),
                        EndTime = AzerbaijanTimeHelper.ToBakuTime(x.EndTime),
                        ExamStartTime = AzerbaijanTimeHelper.ToBakuTime(x.Exam?.StartTime ?? x.StartTime),
                        ExamEndTime = AzerbaijanTimeHelper.ToBakuTime(x.Exam?.EndTime ?? x.EndTime ?? x.StartTime),
                        ExamType = MapExamType(x.Exam?.Type),
                        Note = null,
                        Status = x.IsCompleted ? "Completed" : "Pending",
                       
                        DurationMinutes = x.Exam?.DurationMinutes ?? 0,
                        IsAccessCodeReady = false,
                        CanEnter = false,
                        CanStart = false,
                        IsMissed = false,
                        AccessCode = null
                    })
                    .ToList(),

                Attendance = attendanceRecords
                    .OrderByDescending(x => x.AttendanceSession?.SessionDate)
                    .Select(x => new StudentAttendanceSummaryDto
                    {
                        AttendanceSessionId = x.AttendanceSessionId,
                        SessionDate = x.AttendanceSession != null
    ? AzerbaijanTimeHelper.ToBakuTime(x.AttendanceSession.SessionDate)
    : DateTime.MinValue,
                        SubjectName = x.AttendanceSession?.Subject?.Name ?? string.Empty,
                        TeacherName = x.AttendanceSession?.Teacher?.FullName ?? string.Empty,
                        Status = MapAttendanceStatus(x.Status),
                        StartTime = x.AttendanceSession?.StartTime,
                        EndTime = x.AttendanceSession?.EndTime,
                        Note = x.Notes
                    })
                    .ToList(),

                Tasks = tasks
                    .OrderByDescending(x => x.AssignedDate)
                    .Select(MapToTaskDto)
                    .ToList()
            };
        }

        private static StudentTaskDto MapToTaskDto(ExamDomain.Entities.StudentTask task)
        {
            return new StudentTaskDto
            {
                Id = task.Id,
                Title = task.Title ?? string.Empty,
                SubjectName = task.Subject?.Name ?? string.Empty,
                TeacherName = task.Teacher?.FullName ?? string.Empty,
                AssignedDate = AzerbaijanTimeHelper.ToBakuTime(task.AssignedDate),
                DueDate = AzerbaijanTimeHelper.ToBakuTime(task.DueDate),
                Status = MapTaskStatus(task.Status),
                Score = task.Score,
                MaxScore = task.MaxScore,
                Link = task.Link,
                Note = task.Note,
                Description = task.Description
            };
        }

        private static string MapStudentStatus(StudentStatus status)
        {
            return status switch
            {
                StudentStatus.Active => "Aktiv",
                StudentStatus.Passive => "Passiv",
                StudentStatus.Graduated => "Məzun",
                _ => "Aktiv"
            };
        }

        private static StudentStatus ParseStudentStatus(string? status, StudentStatus fallback)
        {
            var normalized = (status ?? string.Empty).Trim().ToLowerInvariant();

            return normalized switch
            {
                "aktiv" => StudentStatus.Active,
                "passiv" => StudentStatus.Passive,
                "məzun" => StudentStatus.Graduated,
                "mezun" => StudentStatus.Graduated,
                _ => fallback
            };
        }

        private static string MapAttendanceStatus(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "Gəlib",
                AttendanceStatus.Absent => "Yoxdur",
                AttendanceStatus.Late => "Gecikib",
                _ => status.ToString()
            };
        }

        private static string MapTaskStatus(ExamDomain.Enum.StudentTaskStatus status)
        {
            return status switch
            {
                ExamDomain.Enum.StudentTaskStatus.Pending => "Pending",
                ExamDomain.Enum.StudentTaskStatus.Submitted => "Submitted",
                ExamDomain.Enum.StudentTaskStatus.Reviewed => "Reviewed",
                ExamDomain.Enum.StudentTaskStatus.Late => "Late",
                ExamDomain.Enum.StudentTaskStatus.Missing => "Missing",
                _ => "Pending"
            };
        }

        private static string MapGender(Gender? gender)
        {
            return gender switch
            {
                Gender.Male => "Kişi",
                Gender.Female => "Qadın",
                _ => "Bilinmir"
            };
        }

        private static Gender ParseGender(string? gender)
        {
            var normalized = (gender ?? string.Empty).Trim().ToLowerInvariant();

            return normalized switch
            {
                "kişi" => Gender.Male,
                "kisi" => Gender.Male,
                "male" => Gender.Male,
                "qadın" => Gender.Female,
                "qadin" => Gender.Female,
                "female" => Gender.Female,
                _ => Gender.Unknown
            };
        }

        private static string MapExamType(ExamType? type)
        {
            return type switch
            {
                ExamType.Quiz => "Quiz",
                ExamType.Midterm => "Midterm",
                ExamType.Final => "Final",
                ExamType.Practice => "Practice",
                _ => "Unknown"
            };
        }

        private static double CalculateAttendanceRate(List<AttendanceRecord> records)
        {
            if (records.Count == 0)
                return 0;

            var positiveCount = records.Count(x =>
                x.Status == AttendanceStatus.Present ||
                x.Status == AttendanceStatus.Late);

            return Math.Round((double)positiveCount / records.Count * 100, 2);
        }
    }
}
