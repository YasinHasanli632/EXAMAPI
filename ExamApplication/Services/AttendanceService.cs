using ExamApplication.DTO.Attendance;
using ExamApplication.DTO.Notification;
using ExamApplication.Helper;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;
using ExamDomain.ValueObjects;

namespace ExamApplication.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService; // YENI
        public AttendanceService(
     IUnitOfWork unitOfWork,
     ICurrentUserService currentUserService,
     INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<AttendanceSessionDetailDto> CreateAsync(
            CreateAttendanceSessionDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateTodayOnly(request.SessionDate);

            if (request.Records is null || request.Records.Count == 0)
                throw new InvalidOperationException("Ən azı 1 davamiyyət qeydi daxil edilməlidir.");

            EnsureNoDuplicateStudents(request.Records.Select(x => x.StudentId).ToList());

            foreach (var record in request.Records)
            {
                ValidateAttendanceRecord(record);
            }

            var currentUser = GetRequiredCurrentUser();
            var effectiveTeacherId = await ResolveTeacherIdAsync(currentUser, request.TeacherId, cancellationToken);

            await EnsureTeacherClassSubjectRelationAsync(
                request.ClassRoomId,
                request.SubjectId,
                effectiveTeacherId,
                cancellationToken);

            await EnsureStudentsBelongToClassAsync(
                request.ClassRoomId,
                request.Records.Select(x => x.StudentId).ToList(),
                cancellationToken);

            var existingSession =
                await _unitOfWork.AttendanceSessions.GetByClassRoomSubjectTeacherAndDateAsync(
                    request.ClassRoomId,
                    request.SubjectId,
                    effectiveTeacherId,
                    request.SessionDate.Date,
                    cancellationToken);

            if (existingSession is not null)
                throw new InvalidOperationException("Bu sinif, fənn, müəllim və tarix üçün davamiyyət artıq mövcuddur.");

            var session = new AttendanceSession
            {
                ClassRoomId = request.ClassRoomId,
                SubjectId = request.SubjectId,
                TeacherId = effectiveTeacherId,
                SessionDate = request.SessionDate.Date,
                Notes = request.Notes?.Trim(),

                StartTime = request.StartTime,
                EndTime = request.EndTime,
                SessionType = request.SessionType,

                IsLocked = false,

                Records = request.Records
                    .Select(CreateAttendanceRecord)
                    .ToList()
            };

            await _unitOfWork.AttendanceSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var created =
                await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(session.Id, cancellationToken)
                ?? throw new InvalidOperationException("Yaradılmış attendance məlumatı tapılmadı.");

            return MapSessionDetailDto(created);
        }

        public async Task<AttendanceSessionDetailDto> UpdateAsync(
     UpdateAttendanceSessionDto request,
     CancellationToken cancellationToken = default)
        {
            var session =
                await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(request.Id, cancellationToken)
                ?? throw new KeyNotFoundException("Attendance session tapılmadı.");

            ValidateTodayOnly(session.SessionDate);

            if (request.Records is null || request.Records.Count == 0)
                throw new InvalidOperationException("Ən azı 1 davamiyyət qeydi daxil edilməlidir.");

            EnsureNoDuplicateStudents(request.Records.Select(x => x.StudentId).ToList());

            foreach (var record in request.Records)
            {
                ValidateAttendanceRecord(record);
            }

            var currentUser = GetRequiredCurrentUser();
            await EnsureCanManageTeacherAttendanceAsync(currentUser, session.TeacherId, cancellationToken);

            await EnsureTeacherClassSubjectRelationAsync(
                session.ClassRoomId,
                session.SubjectId,
                session.TeacherId,
                cancellationToken);

            await EnsureStudentsBelongToClassAsync(
                session.ClassRoomId,
                request.Records.Select(x => x.StudentId).ToList(),
                cancellationToken);

            session.Notes = request.Notes?.Trim();

            // YENI
            session.StartTime = request.StartTime;
            // YENI
            session.EndTime = request.EndTime;
            // YENI
            session.SessionType = request.SessionType;

            var existingRecords = session.Records.ToList();
            _unitOfWork.AttendanceRecords.RemoveRange(existingRecords);

            session.Records = request.Records
                .Select(CreateAttendanceRecord)
                .ToList();

            _unitOfWork.AttendanceSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated =
                await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(session.Id, cancellationToken)
                ?? throw new InvalidOperationException("Yenilənmiş attendance məlumatı tapılmadı.");

            // YENI - attendance notification dedup üçün student userId-lərini çıxaraq
            var targetStudentUserIds = updated.Records
                .Where(x => x.Student?.UserId > 0)
                .Select(x => x.Student!.UserId)
                .Distinct()
                .ToList();

            var existingKeys = new HashSet<string>();

            foreach (var studentUserId in targetStudentUserIds)
            {
                var studentNotifications = await _unitOfWork.Notifications.GetByUserIdAsync(
                    studentUserId,
                    cancellationToken);

                foreach (var notification in studentNotifications
                             .Where(x => x.RelatedEntityType == "AttendanceSession" && x.RelatedEntityId == updated.Id))
                {
                    existingKeys.Add($"{notification.UserId}-{notification.Category}");
                }
            }

            var attendanceNotifications = new List<CreateNotificationDto>();

            foreach (var record in updated.Records)
            {
                if (record.Student?.UserId <= 0)
                    continue;

                if (record.Status != AttendanceStatus.Absent && record.Status != AttendanceStatus.Late)
                    continue;

                var category = record.Status == AttendanceStatus.Absent
                    ? NotificationCategory.AttendanceAbsent
                    : NotificationCategory.AttendanceLate;

                var key = $"{record.Student.UserId}-{category}";

                if (existingKeys.Contains(key))
                    continue;

                attendanceNotifications.Add(new CreateNotificationDto
                {
                    UserId = record.Student.UserId,
                    Title = "Davamiyyət qeyd edildi",
                    Message = record.Status == AttendanceStatus.Absent
                        ? $"{updated.SessionDate:dd.MM.yyyy} tarixli {updated.Subject?.Name ?? "dərs"} üzrə siz absent qeyd olundunuz."
                        : $"{updated.SessionDate:dd.MM.yyyy} tarixli {updated.Subject?.Name ?? "dərs"} üzrə siz late qeyd olundunuz.",
                    Type = (int)NotificationType.Attendance,
                    Category = (int)category,
                    Priority = record.Status == AttendanceStatus.Absent
                        ? (int)NotificationPriority.High
                        : (int)NotificationPriority.Medium,
                    RelatedEntityType = "AttendanceSession",
                    RelatedEntityId = updated.Id,
                    ActionUrl = "/student/attendance",
                    Icon = record.Status == AttendanceStatus.Absent ? "attendance-absent" : "attendance-late",
                    MetadataJson = $@"{{""sessionId"":{updated.Id},""studentId"":{record.StudentId},""status"":""{record.Status}""}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                });
            }

            if (attendanceNotifications.Any())
            {
                await _notificationService.CreateBulkAsync(attendanceNotifications, cancellationToken);
            }

            return MapSessionDetailDto(updated);
        }

        public async Task<AttendanceSessionDetailDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var session =
                await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException("Attendance session tapılmadı.");

            var currentUser = GetRequiredCurrentUser();
            await EnsureCanManageTeacherAttendanceAsync(currentUser, session.TeacherId, cancellationToken);

            return MapSessionDetailDto(session);
        }

        public async Task<List<AttendanceSessionDto>> GetByClassRoomIdAsync(
            int classRoomId,
            CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.AttendanceSessions.GetByClassRoomIdAsync(classRoomId, cancellationToken);
            var currentUser = GetRequiredCurrentUser();

            if (IsTeacherRole(currentUser.Role))
            {
                var teacher = await GetCurrentTeacherAsync(cancellationToken);
                sessions = sessions
                    .Where(x => x.TeacherId == teacher.Id)
                    .ToList();
            }

            return sessions
                .OrderByDescending(x => x.SessionDate)
                .ThenByDescending(x => x.StartTime)
                .Select(MapSessionDto)
                .ToList();
        }

        public async Task<List<AttendanceStudentHistoryDto>> GetByStudentIdAsync(
            int studentId,
            CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.AttendanceSessions.GetByStudentIdAsync(studentId, cancellationToken);
            var currentUser = GetRequiredCurrentUser();

            if (IsTeacherRole(currentUser.Role))
            {
                var teacher = await GetCurrentTeacherAsync(cancellationToken);
                sessions = sessions
                    .Where(x => x.TeacherId == teacher.Id)
                    .ToList();
            }

            var result = new List<AttendanceStudentHistoryDto>();

            foreach (var session in sessions.OrderByDescending(x => x.SessionDate).ThenByDescending(x => x.StartTime))
            {
                var record = session.Records.FirstOrDefault(x => x.StudentId == studentId);
                if (record is null) continue;

                result.Add(new AttendanceStudentHistoryDto
                {
                    AttendanceSessionId = session.Id,
                    SessionDate = AzerbaijanTimeHelper.ToBakuTime(session.SessionDate),
                    ClassName = session.ClassRoom?.Name ?? string.Empty,
                    SubjectName = session.Subject?.Name ?? string.Empty,
                    TeacherName = session.Teacher?.FullName ?? string.Empty,
                    Status = MapAttendanceStatusText(record.Status),
                    Notes = record.Notes,
                    AbsenceReasonType = record.AbsenceReasonType,
                    AbsenceReasonNote = record.AbsenceReasonNote,
                    LateArrivalTime = FormatTime(record.LateArrivalTime),
                    LateNote = record.LateNote
                });
            }

            return result;
        }

        public async Task<List<StudentAttendanceSummaryDto>> GetStudentSummaryByClassRoomIdAsync(
            int classRoomId,
            CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.AttendanceSessions.GetByClassRoomIdAsync(classRoomId, cancellationToken);
            var currentUser = GetRequiredCurrentUser();

            if (IsTeacherRole(currentUser.Role))
            {
                var teacher = await GetCurrentTeacherAsync(cancellationToken);
                sessions = sessions
                    .Where(x => x.TeacherId == teacher.Id)
                    .ToList();
            }

            var grouped = sessions
                .SelectMany(x => x.Records)
                .GroupBy(x => x.StudentId)
                .ToList();

            var result = new List<StudentAttendanceSummaryDto>();

            foreach (var group in grouped)
            {
                var first = group.First();

                var total = group.Count();
                var present = group.Count(x => x.Status == AttendanceStatus.Present);
                var absent = group.Count(x => x.Status == AttendanceStatus.Absent);
                var late = group.Count(x => x.Status == AttendanceStatus.Late);

                result.Add(new StudentAttendanceSummaryDto
                {
                    StudentId = first.StudentId,
                    StudentFullName = first.Student?.FullName ?? string.Empty,
                    StudentEmail = first.Student?.User?.Email ?? string.Empty,
                    TotalSessions = total,
                    PresentCount = present,
                    AbsentCount = absent,
                    LateCount = late,
                    AttendanceRate = total == 0 ? 0 : Math.Round(((double)(present + late) / total) * 100, 2)
                });
            }

            return result.OrderBy(x => x.StudentFullName).ToList();
        }

        // =========================================================
        // YENI - CHANGE REQUEST
        // =========================================================

        public async Task<AttendanceChangeRequestDto> CreateChangeRequestAsync(
            CreateAttendanceChangeRequestDto request,
            CancellationToken cancellationToken = default)
        {
            ValidatePastDateOnly(request.AttendanceDate);

            if (request.StudentId <= 0)
                throw new InvalidOperationException("StudentId düzgün deyil.");

            if (string.IsNullOrWhiteSpace(request.RequestedChangeReason))
                throw new InvalidOperationException("Dəyişiklik səbəbi boş ola bilməz.");

            var currentUser = GetRequiredCurrentUser();
            var effectiveTeacherId = await ResolveTeacherIdAsync(currentUser, request.TeacherId, cancellationToken);

            await EnsureTeacherClassSubjectRelationAsync(
                request.ClassRoomId,
                request.SubjectId,
                effectiveTeacherId,
                cancellationToken);

            await EnsureStudentsBelongToClassAsync(
                request.ClassRoomId,
                new List<int> { request.StudentId },
                cancellationToken);

            var existingPending =
                await _unitOfWork.AttendanceChangeRequest.GetPendingByAttendanceInfoAsync(
                    request.ClassRoomId,
                    request.SubjectId,
                    effectiveTeacherId,
                    request.StudentId,
                    request.AttendanceDate,
                    cancellationToken);

            if (existingPending is not null)
                throw new InvalidOperationException("Bu tarix üçün artıq pending dəyişiklik sorğusu mövcuddur.");

            var session =
                await _unitOfWork.AttendanceSessions.GetByClassRoomSubjectTeacherAndDateAsync(
                    request.ClassRoomId,
                    request.SubjectId,
                    effectiveTeacherId,
                    request.AttendanceDate,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Dəyişiklik tələb edilən attendance tapılmadı.");

            var currentRecord = session.Records.FirstOrDefault(x => x.StudentId == request.StudentId)
                ?? throw new KeyNotFoundException("Tələbə üzrə attendance qeydi tapılmadı.");

            var requestedStatus = MapAttendanceStatus(request.RequestedStatus);

            if (requestedStatus == AttendanceStatus.Absent &&
                string.IsNullOrWhiteSpace(request.RequestedAbsenceReasonType))
            {
                throw new InvalidOperationException("Absent dəyişiklik sorğusu üçün səbəb seçilməlidir.");
            }

            if (requestedStatus == AttendanceStatus.Late &&
                string.IsNullOrWhiteSpace(request.RequestedLateArrivalTime))
            {
                throw new InvalidOperationException("Late dəyişiklik sorğusu üçün gəlmə vaxtı daxil edilməlidir.");
            }

            var entity = new AttendanceChangeRequest
            {
                ClassRoomId = request.ClassRoomId,
                SubjectId = request.SubjectId,
                TeacherId = effectiveTeacherId,
                StudentId = request.StudentId,
                AttendanceDate = request.AttendanceDate.Date,
                CurrentStatus = currentRecord.Status,
                RequestedStatus = requestedStatus,
                RequestedChangeReason = request.RequestedChangeReason.Trim(),
                RequestedAbsenceReasonType = requestedStatus == AttendanceStatus.Absent
                    ? request.RequestedAbsenceReasonType?.Trim()
                    : null,
                RequestedAbsenceReasonNote = requestedStatus == AttendanceStatus.Absent
                    ? request.RequestedAbsenceReasonNote?.Trim()
                    : null,
                RequestedLateArrivalTime = requestedStatus == AttendanceStatus.Late
                    ? ParseTimeSpanOrNull(request.RequestedLateArrivalTime)
                    : null,
                RequestedLateNote = requestedStatus == AttendanceStatus.Late
                    ? request.RequestedLateNote?.Trim()
                    : null,
                RequestedByTeacherId = effectiveTeacherId,
                RequestedAt = DateTime.UtcNow,
                RequestStatus = "Pending"
            };

            await _unitOfWork.AttendanceChangeRequest.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // YENI - adminlərə yeni attendance dəyişiklik sorğusu notification-u
            var adminUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            var adminNotifications = adminUsers
                .Where(x => x.IsActive && (x.Role == UserRole.Admin || x.Role == UserRole.IsSuperAdmin))
                .Select(admin => new CreateNotificationDto
                {
                    UserId = admin.Id,
                    Title = "Yeni davamiyyət dəyişiklik sorğusu",
                    Message = $"{request.AttendanceDate:dd.MM.yyyy} tarixi üzrə yeni attendance change request yaradıldı.",
                    Type = (int)NotificationType.Attendance,
                    Category = (int)NotificationCategory.AttendanceChangeRequestCreated,
                    Priority = (int)NotificationPriority.High,
                    RelatedEntityType = "AttendanceChangeRequest",
                    RelatedEntityId = entity.Id,
                    ActionUrl = "/admin/attendance/change-requests",
                    Icon = "attendance-request",
                    MetadataJson = $@"{{""changeRequestId"":{entity.Id},""studentId"":{request.StudentId},""teacherId"":{effectiveTeacherId}}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                })
                .ToList();

            if (adminNotifications.Any())
            {
                await _notificationService.CreateBulkAsync(adminNotifications, cancellationToken);
            }
            var created =
                await _unitOfWork.AttendanceChangeRequest.GetByIdAsync(entity.Id, cancellationToken)
                ?? throw new InvalidOperationException("Dəyişiklik sorğusu tapılmadı.");

            return MapChangeRequestDto(created);
        }

        public async Task<List<AttendanceChangeRequestDto>> GetChangeRequestsAsync(
            AttendanceChangeRequestFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            int? effectiveTeacherId = filter.TeacherId;

            if (IsTeacherRole(currentUser.Role))
            {
                var teacher = await GetCurrentTeacherAsync(cancellationToken);
                effectiveTeacherId = teacher.Id;
            }

            var items = await _unitOfWork.AttendanceChangeRequest.GetFilteredAsync(
                filter.RequestStatus,
                filter.ClassRoomId,
                filter.SubjectId,
                effectiveTeacherId,
                filter.AttendanceDateFrom,
                filter.AttendanceDateTo,
                cancellationToken);

            return items
                .Select(MapChangeRequestDto)
                .ToList();
        }

        public async Task<AttendanceChangeRequestDto> ApproveChangeRequestAsync(
            int changeRequestId,
            ReviewAttendanceChangeRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            if (!IsAdminRole(currentUser.Role))
                throw new UnauthorizedAccessException("Bu əməliyyat yalnız admin üçün icazəlidir.");

            var changeRequest =
                await _unitOfWork.AttendanceChangeRequest.GetByIdAsync(changeRequestId, cancellationToken)
                ?? throw new KeyNotFoundException("Attendance change request tapılmadı.");

            if (!string.Equals(changeRequest.RequestStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Yalnız Pending sorğu approve edilə bilər.");

            var session =
                await _unitOfWork.AttendanceSessions.GetByClassRoomSubjectTeacherAndDateAsync(
                    changeRequest.ClassRoomId,
                    changeRequest.SubjectId,
                    changeRequest.TeacherId,
                    changeRequest.AttendanceDate,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Attendance session tapılmadı.");

            var record = session.Records.FirstOrDefault(x => x.StudentId == changeRequest.StudentId)
                ?? throw new KeyNotFoundException("Attendance record tapılmadı.");

            record.Status = changeRequest.RequestedStatus;
            record.AbsenceReasonType = changeRequest.RequestedStatus == AttendanceStatus.Absent
                ? changeRequest.RequestedAbsenceReasonType?.Trim()
                : null;
            record.AbsenceReasonNote = changeRequest.RequestedStatus == AttendanceStatus.Absent
                ? changeRequest.RequestedAbsenceReasonNote?.Trim()
                : null;
            record.LateArrivalTime = changeRequest.RequestedStatus == AttendanceStatus.Late
                ? changeRequest.RequestedLateArrivalTime
                : null;
            record.LateNote = changeRequest.RequestedStatus == AttendanceStatus.Late
                ? changeRequest.RequestedLateNote?.Trim()
                : null;

            changeRequest.RequestStatus = "Approved";
            changeRequest.ReviewedByAdminId = currentUser.UserId;
            changeRequest.ReviewedAt = DateTime.UtcNow;
            changeRequest.ReviewNote = request.ReviewNote?.Trim();

            _unitOfWork.AttendanceRecords.Update(record);
            _unitOfWork.AttendanceChangeRequest.Update(changeRequest);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // YENI - notify student after approve
            // YENI - tələbəyə approve notification-u
            var approvedStudent = await _unitOfWork.Students.GetByIdWithDetailsAsync(changeRequest.StudentId, cancellationToken);

            if (approvedStudent?.UserId > 0)
            {
                await _notificationService.CreateAsync(new CreateNotificationDto
                {
                    UserId = approvedStudent.UserId,
                    Title = "Davamiyyət sorğunuz təsdiqləndi",
                    Message = $"{changeRequest.AttendanceDate:dd.MM.yyyy} tarixli attendance dəyişiklik sorğunuz təsdiqləndi.",
                    Type = (int)NotificationType.Attendance,
                    Category = (int)NotificationCategory.AttendanceChangeRequestApproved,
                    Priority = (int)NotificationPriority.High,
                    RelatedEntityType = "AttendanceChangeRequest",
                    RelatedEntityId = changeRequest.Id,
                    ActionUrl = "/student/attendance",
                    Icon = "attendance-approved",
                    MetadataJson = $@"{{""changeRequestId"":{changeRequest.Id},""status"":""Approved""}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                }, cancellationToken);
            }
            var updated =
                await _unitOfWork.AttendanceChangeRequest.GetByIdAsync(changeRequest.Id, cancellationToken)
                ?? throw new InvalidOperationException("Approve olunmuş sorğu tapılmadı.");

            return MapChangeRequestDto(updated);
        }

        public async Task<AttendanceChangeRequestDto> RejectChangeRequestAsync(
            int changeRequestId,
            ReviewAttendanceChangeRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            if (!IsAdminRole(currentUser.Role))
                throw new UnauthorizedAccessException("Bu əməliyyat yalnız admin üçün icazəlidir.");

            var changeRequest =
                await _unitOfWork.AttendanceChangeRequest.GetByIdAsync(changeRequestId, cancellationToken)
                ?? throw new KeyNotFoundException("Attendance change request tapılmadı.");

            if (!string.Equals(changeRequest.RequestStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Yalnız Pending sorğu reject edilə bilər.");

            changeRequest.RequestStatus = "Rejected";
            changeRequest.ReviewedByAdminId = currentUser.UserId;
            changeRequest.ReviewedAt = DateTime.UtcNow;
            changeRequest.ReviewNote = request.ReviewNote?.Trim();

            _unitOfWork.AttendanceChangeRequest.Update(changeRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // YENI - notify student after reject
            // YENI - tələbəyə reject notification-u
            var rejectedStudent = await _unitOfWork.Students.GetByIdWithDetailsAsync(changeRequest.StudentId, cancellationToken);

            if (rejectedStudent?.UserId > 0)
            {
                await _notificationService.CreateAsync(new CreateNotificationDto
                {
                    UserId = rejectedStudent.UserId,
                    Title = "Davamiyyət sorğunuz rədd edildi",
                    Message = $"{changeRequest.AttendanceDate:dd.MM.yyyy} tarixli attendance dəyişiklik sorğunuz rədd edildi.",
                    Type = (int)NotificationType.Attendance,
                    Category = (int)NotificationCategory.AttendanceChangeRequestRejected,
                    Priority = (int)NotificationPriority.High,
                    RelatedEntityType = "AttendanceChangeRequest",
                    RelatedEntityId = changeRequest.Id,
                    ActionUrl = "/student/attendance",
                    Icon = "attendance-rejected",
                    MetadataJson = $@"{{""changeRequestId"":{changeRequest.Id},""status"":""Rejected""}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                }, cancellationToken);
            }
            var updated =
                await _unitOfWork.AttendanceChangeRequest.GetByIdAsync(changeRequest.Id, cancellationToken)
                ?? throw new InvalidOperationException("Reject olunmuş sorğu tapılmadı.");

            return MapChangeRequestDto(updated);
        }

        // =========================================================
        // YENI - SESSION COLUMN VE BOARD
        // =========================================================

        public async Task<AttendanceSessionDetailDto> CreateSessionColumnAsync(
            CreateAttendanceSessionColumnDto request,
            CancellationToken cancellationToken = default)
        {
            if (request.ClassRoomId <= 0)
                throw new InvalidOperationException("ClassRoomId düzgün deyil.");

            if (request.SubjectId <= 0)
                throw new InvalidOperationException("SubjectId düzgün deyil.");

            if (request.TeacherId <= 0)
                throw new InvalidOperationException("TeacherId düzgün deyil.");

            if (request.SessionDate == default)
                throw new InvalidOperationException("SessionDate daxil edilməlidir.");

            if (request.StartTime.HasValue && request.EndTime.HasValue && request.EndTime <= request.StartTime)
                throw new InvalidOperationException("Bitiş saatı başlanğıc saatından böyük olmalıdır.");

            var currentUser = GetRequiredCurrentUser();
            var effectiveTeacherId = await ResolveTeacherIdAsync(currentUser, request.TeacherId, cancellationToken);

            await EnsureTeacherClassSubjectRelationAsync(
                request.ClassRoomId,
                request.SubjectId,
                effectiveTeacherId,
                cancellationToken);

            var existingSession =
                await _unitOfWork.AttendanceSessions.GetByClassRoomSubjectTeacherAndDateAsync(
                    request.ClassRoomId,
                    request.SubjectId,
                    effectiveTeacherId,
                    request.SessionDate.Date,
                    cancellationToken);

            if (existingSession is not null)
                throw new InvalidOperationException("Bu tarix üçün attendance session artıq mövcuddur.");

            var session = new AttendanceSession
            {
                ClassRoomId = request.ClassRoomId,
                SubjectId = request.SubjectId,
                TeacherId = effectiveTeacherId,
                SessionDate = request.SessionDate.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Notes = request.Notes?.Trim(),
                SessionType = request.SessionType,
                IsLocked = false
            };

            await _unitOfWork.AttendanceSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var created =
                await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(session.Id, cancellationToken)
                ?? throw new InvalidOperationException("Yaradılmış attendance session tapılmadı.");

            return MapSessionDetailDto(created);
        }

        public async Task<AttendanceSessionDetailDto> SaveSessionRecordsAsync(
            SaveAttendanceSessionRecordsDto request,
            CancellationToken cancellationToken = default)
        {
            if (request.SessionId <= 0)
                throw new InvalidOperationException("SessionId düzgün deyil.");

            if (request.Records is null || request.Records.Count == 0)
                throw new InvalidOperationException("Ən azı 1 attendance record göndərilməlidir.");

            EnsureNoDuplicateStudents(request.Records.Select(x => x.StudentId).ToList());

            foreach (var record in request.Records)
            {
                ValidateUpsertAttendanceRecord(record);
            }

            var session =
                await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(request.SessionId, cancellationToken)
                ?? throw new KeyNotFoundException("Attendance session tapılmadı.");

            if (session.IsLocked)
                throw new InvalidOperationException("Bu attendance session artıq kilidlənib və dəyişdirilə bilməz.");

            var currentUser = GetRequiredCurrentUser();
            await EnsureCanManageTeacherAttendanceAsync(currentUser, session.TeacherId, cancellationToken);

            await EnsureStudentsBelongToClassAsync(
                session.ClassRoomId,
                request.Records.Select(x => x.StudentId).ToList(),
                cancellationToken);

            var existingRecords = session.Records.ToList();
            _unitOfWork.AttendanceRecords.RemoveRange(existingRecords);

            session.Records = request.Records
                .Select(CreateAttendanceRecordFromUpsert)
                .ToList();

            _unitOfWork.AttendanceSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _unitOfWork.AttendanceSessions.GetByIdWithDetailsAsync(session.Id, cancellationToken);

            if (updated == null)
            {
                throw new KeyNotFoundException("Yenilənmiş attendance session tapılmadı.");
            }

            // YENI - attendance notification dedup üçün mövcud notification-ları yoxlayaq
            var existingNotifications = await _unitOfWork.Notifications.GetByUserIdAsync(
                currentUser.UserId,
                cancellationToken);

            var existingKeys = existingNotifications
                .Where(x => x.RelatedEntityType == "AttendanceSession" && x.RelatedEntityId == updated.Id)
                .Select(x => $"{x.UserId}-{x.Category}")
                .ToHashSet();

            // YENI - absent / late notification-ları yaradaq
            var attendanceNotifications = new List<CreateNotificationDto>();

            foreach (var record in updated.Records)
            {
                if (record.Student?.UserId <= 0)
                    continue;

                if (record.Status != AttendanceStatus.Absent && record.Status != AttendanceStatus.Late)
                    continue;

                var category = record.Status == AttendanceStatus.Absent
                    ? NotificationCategory.AttendanceAbsent
                    : NotificationCategory.AttendanceLate;

                var key = $"{record.Student.UserId}-{category}";

                // YENI - artıq varsa ikinci dəfə yaratma
                if (existingKeys.Contains(key))
                    continue;

                attendanceNotifications.Add(new CreateNotificationDto
                {
                    UserId = record.Student.UserId,
                    Title = "Davamiyyət qeyd edildi",
                    Message = record.Status == AttendanceStatus.Absent
                        ? $"{updated.SessionDate:dd.MM.yyyy} tarixli {updated.Subject?.Name ?? "dərs"} üzrə siz absent qeyd olundunuz."
                        : $"{updated.SessionDate:dd.MM.yyyy} tarixli {updated.Subject?.Name ?? "dərs"} üzrə siz late qeyd olundunuz.",
                    Type = (int)NotificationType.Attendance,
                    Category = (int)category,
                    Priority = record.Status == AttendanceStatus.Absent
                        ? (int)NotificationPriority.High
                        : (int)NotificationPriority.Medium,
                    RelatedEntityType = "AttendanceSession",
                    RelatedEntityId = updated.Id,
                    ActionUrl = "/student/attendance",
                    Icon = record.Status == AttendanceStatus.Absent ? "attendance-absent" : "attendance-late",
                    MetadataJson = $@"{{""sessionId"":{updated.Id},""studentId"":{record.StudentId},""status"":""{record.Status}""}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                });
            }

            if (attendanceNotifications.Any())
            {
                await _notificationService.CreateBulkAsync(attendanceNotifications, cancellationToken);
            }

            return MapSessionDetailDto(updated);
        }

        public async Task<AttendanceBoardDto> GetBoardAsync(
            AttendanceBoardFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            if (filter.ClassRoomId <= 0)
                throw new InvalidOperationException("ClassRoomId düzgün deyil.");

            if (filter.SubjectId <= 0)
                throw new InvalidOperationException("SubjectId düzgün deyil.");

            if (filter.TeacherId <= 0)
                throw new InvalidOperationException("TeacherId düzgün deyil.");

            if (filter.Year < 2000 || filter.Year > 2100)
                throw new InvalidOperationException("Year düzgün deyil.");

            if (filter.Month < 1 || filter.Month > 12)
                throw new InvalidOperationException("Month düzgün deyil.");

            var currentUser = GetRequiredCurrentUser();
            var effectiveTeacherId = await ResolveTeacherIdForReadAsync(currentUser, filter.TeacherId, cancellationToken);

            await EnsureTeacherClassSubjectRelationAsync(
                filter.ClassRoomId,
                filter.SubjectId,
                effectiveTeacherId,
                cancellationToken);

            var sessions = await _unitOfWork.AttendanceSessions.GetBoardSessionsAsync(
                filter.ClassRoomId,
                filter.SubjectId,
                effectiveTeacherId,
                filter.Year,
                filter.Month,
                cancellationToken);

            var activeStudentClasses = await _unitOfWork.StudentClasses.GetActiveByClassRoomIdAsync(
                filter.ClassRoomId,
                cancellationToken);

            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(filter.ClassRoomId, cancellationToken)
                ?? throw new KeyNotFoundException("Sinif tapılmadı.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(filter.SubjectId, cancellationToken)
                ?? throw new KeyNotFoundException("Fənn tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(effectiveTeacherId, cancellationToken)
                ?? throw new KeyNotFoundException("Müəllim tapılmadı.");

            var orderedSessions = sessions
                .OrderBy(x => x.SessionDate)
                .ThenBy(x => x.StartTime)
                .ToList();

            var sessionColumns = orderedSessions
                .Select(x =>
                {
                    var bakuSessionDate = AzerbaijanTimeHelper.ToBakuTime(x.SessionDate);

                    return new AttendanceSessionColumnDto
                    {
                        SessionId = x.Id,
                        SessionDate = bakuSessionDate,
                        SessionDateText = bakuSessionDate.ToString("dd.MM.yyyy"),
                        StartTimeText = FormatTime(x.StartTime),
                        EndTimeText = FormatTime(x.EndTime),
                        SessionType = MapAttendanceSessionTypeText(x.SessionType),
                        IsExtraLesson = x.SessionType == AttendanceSessionType.ExtraLesson,
                        IsLocked = x.IsLocked,
                        Notes = x.Notes
                    };
                })
                .ToList();

            var studentRows = new List<AttendanceBoardStudentRowDto>();

            foreach (var studentClass in activeStudentClasses.OrderBy(x => x.Student.FullName))
            {
                var student = studentClass.Student;
                if (student == null) continue;

                var studentCells = new List<AttendanceBoardCellDto>();

                foreach (var session in orderedSessions)
                {
                    var record = session.Records.FirstOrDefault(r => r.StudentId == student.Id);

                    studentCells.Add(new AttendanceBoardCellDto
                    {
                        SessionId = session.Id,
                        AttendanceRecordId = record?.Id,
                        Status = record == null ? null : MapAttendanceStatusText(record.Status),
                        Notes = record?.Notes,
                        AbsenceReasonType = record?.AbsenceReasonType,
                        AbsenceReasonNote = record?.AbsenceReasonNote,
                        LateArrivalTime = FormatTime(record?.LateArrivalTime),
                        LateNote = record?.LateNote,
                        HasRecord = record != null,
                        IsEditable = !session.IsLocked,
                        IsLocked = session.IsLocked
                    });
                }

                var allRecords = orderedSessions
                    .SelectMany(x => x.Records)
                    .Where(x => x.StudentId == student.Id)
                    .ToList();

                var total = allRecords.Count;
                var present = allRecords.Count(x => x.Status == AttendanceStatus.Present);
                var absent = allRecords.Count(x => x.Status == AttendanceStatus.Absent);
                var late = allRecords.Count(x => x.Status == AttendanceStatus.Late);

                studentRows.Add(new AttendanceBoardStudentRowDto
                {
                    StudentId = student.Id,
                    StudentFullName = student.FullName,
                    StudentEmail = student.User?.Email ?? string.Empty,
                    StudentPhotoUrl = student.User?.PhotoUrl ?? string.Empty,
                    StudentNumber = student.StudentNumber ?? string.Empty,
                    PresentCount = present,
                    AbsentCount = absent,
                    LateCount = late,
                    AttendanceRate = total == 0
                        ? 0
                        : Math.Round(((double)(present + late) / total) * 100, 2),
                    Cells = studentCells
                });
            }

            var boardAllRecords = orderedSessions.SelectMany(x => x.Records).ToList();
            var boardTotal = boardAllRecords.Count;
            var boardPresent = boardAllRecords.Count(x => x.Status == AttendanceStatus.Present);
            var boardLate = boardAllRecords.Count(x => x.Status == AttendanceStatus.Late);

            return new AttendanceBoardDto
            {
                ClassRoomId = classRoom.Id,
                ClassName = classRoom.Name,
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                TeacherId = teacher.Id,
                TeacherName = teacher.FullName,
                Year = filter.Year,
                Month = filter.Month,
                TotalStudents = studentRows.Count,
                TotalSessions = sessionColumns.Count,
                AttendanceRate = boardTotal == 0
                    ? 0
                    : Math.Round(((double)(boardPresent + boardLate) / boardTotal) * 100, 2),
                Sessions = sessionColumns,
                Students = studentRows
            };
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var session =
                await _unitOfWork.AttendanceSessions.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException("Attendance session tapılmadı.");

            ValidateTodayOnly(session.SessionDate);

            _unitOfWork.AttendanceSessions.Remove(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

        private async Task EnsureTeacherClassSubjectRelationAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            CancellationToken cancellationToken)
        {
            var relationExists = await _unitOfWork.ClassTeacherSubjects.ExistsAsync(
                classRoomId,
                teacherId,
                subjectId,
                cancellationToken);

            if (!relationExists)
                throw new InvalidOperationException("Bu sinif, fənn və müəllim əlaqəsi sistemdə tapılmadı.");
        }

        private async Task EnsureStudentsBelongToClassAsync(
            int classRoomId,
            List<int> studentIds,
            CancellationToken cancellationToken)
        {
            if (studentIds == null || studentIds.Count == 0)
                throw new InvalidOperationException("Ən azı 1 şagird göndərilməlidir.");

            var distinctStudentIds = studentIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            var students = await _unitOfWork.Students.GetByIdsWithDetailsAsync(distinctStudentIds, cancellationToken);

            if (students.Count != distinctStudentIds.Count)
                throw new InvalidOperationException("Göndərilən şagirdlərdən bəziləri sistemdə tapılmadı.");

            var allBelongToClass = await _unitOfWork.StudentClasses.AreAllStudentsActiveInClassAsync(
                classRoomId,
                distinctStudentIds,
                cancellationToken);

            if (!allBelongToClass)
                throw new InvalidOperationException("Göndərilən şagirdlərdən bəziləri bu sinfə aid deyil.");
        }

        private async Task<int> ResolveTeacherIdAsync(
            JwtUserInfo currentUser,
            int requestedTeacherId,
            CancellationToken cancellationToken)
        {
            if (IsAdminRole(currentUser.Role))
            {
                if (requestedTeacherId <= 0)
                    throw new InvalidOperationException("TeacherId düzgün göndərilməlidir.");

                var teacher = await _unitOfWork.Teachers.GetByIdAsync(requestedTeacherId, cancellationToken);
                if (teacher == null)
                    throw new KeyNotFoundException("Müəllim tapılmadı.");

                return teacher.Id;
            }

            if (IsTeacherRole(currentUser.Role))
            {
                var teacher = await GetCurrentTeacherAsync(cancellationToken);

                if (requestedTeacherId > 0 && requestedTeacherId != teacher.Id)
                    throw new UnauthorizedAccessException("Başqa müəllimin attendance məlumatına əməliyyat etmək olmaz.");

                return teacher.Id;
            }

            throw new UnauthorizedAccessException("Bu əməliyyat üçün icazə yoxdur.");
        }

        private async Task<int> ResolveTeacherIdForReadAsync(
            JwtUserInfo currentUser,
            int requestedTeacherId,
            CancellationToken cancellationToken)
        {
            if (IsAdminRole(currentUser.Role))
            {
                if (requestedTeacherId <= 0)
                    throw new InvalidOperationException("TeacherId düzgün göndərilməlidir.");

                return requestedTeacherId;
            }

            if (IsTeacherRole(currentUser.Role))
            {
                var teacher = await GetCurrentTeacherAsync(cancellationToken);
                return teacher.Id;
            }

            throw new UnauthorizedAccessException("Bu əməliyyat üçün icazə yoxdur.");
        }

        private async Task EnsureCanManageTeacherAttendanceAsync(
            JwtUserInfo currentUser,
            int teacherId,
            CancellationToken cancellationToken)
        {
            if (IsAdminRole(currentUser.Role))
                return;

            if (!IsTeacherRole(currentUser.Role))
                throw new UnauthorizedAccessException("Bu əməliyyat üçün icazə yoxdur.");

            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            if (teacher.Id != teacherId)
                throw new UnauthorizedAccessException("Bu attendance məlumatı sizə aid deyil.");
        }

        private async Task<Teacher> GetCurrentTeacherAsync(CancellationToken cancellationToken)
        {
            var currentUser = GetRequiredCurrentUser();

            var teacher = await _unitOfWork.Teachers.GetByUserIdWithDetailsAsync(currentUser.UserId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException("Cari istifadəçi üçün müəllim profili tapılmadı.");

            return teacher;
        }

        private JwtUserInfo GetRequiredCurrentUser()
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser == null || currentUser.UserId <= 0)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            return currentUser;
        }

        private static bool IsAdminRole(string? role)
        {
            var normalized = (role ?? string.Empty).Trim();
            return normalized == "Admin" || normalized == "IsSuperAdmin";
        }

        private static bool IsTeacherRole(string? role)
        {
            return string.Equals((role ?? string.Empty).Trim(), "Teacher", StringComparison.OrdinalIgnoreCase);
        }

        private static AttendanceRecord CreateAttendanceRecord(CreateAttendanceRecordDto x)
        {
            var status = MapAttendanceStatus(x.Status);

            return new AttendanceRecord
            {
                StudentId = x.StudentId,
                Status = status,
                Notes = x.Notes?.Trim(),
                AbsenceReasonType = status == AttendanceStatus.Absent ? x.AbsenceReasonType?.Trim() : null,
                AbsenceReasonNote = status == AttendanceStatus.Absent ? x.AbsenceReasonNote?.Trim() : null,
                LateArrivalTime = status == AttendanceStatus.Late ? x.LateArrivalTime : null,
                LateNote = status == AttendanceStatus.Late ? x.LateNote?.Trim() : null
            };
        }

        private static AttendanceRecord CreateAttendanceRecordFromUpsert(UpsertAttendanceRecordDto x)
        {
            var status = MapAttendanceStatus(x.Status);

            return new AttendanceRecord
            {
                StudentId = x.StudentId,
                Status = status,
                Notes = x.Notes?.Trim(),
                AbsenceReasonType = status == AttendanceStatus.Absent ? x.AbsenceReasonType?.Trim() : null,
                AbsenceReasonNote = status == AttendanceStatus.Absent ? x.AbsenceReasonNote?.Trim() : null,
                LateArrivalTime = status == AttendanceStatus.Late ? x.LateArrivalTime : null,
                LateNote = status == AttendanceStatus.Late ? x.LateNote?.Trim() : null
            };
        }

        private static void ValidateAttendanceRecord(CreateAttendanceRecordDto record)
        {
            if (record.StudentId <= 0)
                throw new InvalidOperationException("StudentId düzgün deyil.");

            var status = MapAttendanceStatus(record.Status);

            if (status == AttendanceStatus.Absent &&
                string.IsNullOrWhiteSpace(record.AbsenceReasonType))
            {
                throw new InvalidOperationException("İştirak etməyib statusu üçün səbəb seçilməlidir.");
            }

            if (status == AttendanceStatus.Late &&
                !record.LateArrivalTime.HasValue)
            {
                throw new InvalidOperationException("Gecikib statusu üçün gəlmə vaxtı daxil edilməlidir.");
            }
        }

        private static void ValidateUpsertAttendanceRecord(UpsertAttendanceRecordDto record)
        {
            if (record.StudentId <= 0)
                throw new InvalidOperationException("StudentId düzgün deyil.");

            var status = MapAttendanceStatus(record.Status);

            if (status == AttendanceStatus.Absent &&
                string.IsNullOrWhiteSpace(record.AbsenceReasonType))
            {
                throw new InvalidOperationException("İştirak etməyib statusu üçün səbəb seçilməlidir.");
            }

            if (status == AttendanceStatus.Late &&
                !record.LateArrivalTime.HasValue)
            {
                throw new InvalidOperationException("Gecikib statusu üçün gəlmə vaxtı daxil edilməlidir.");
            }
        }

        private static void EnsureNoDuplicateStudents(List<int> studentIds)
        {
            var distinctCount = studentIds
                .Where(x => x > 0)
                .Distinct()
                .Count();

            if (distinctCount != studentIds.Count)
                throw new InvalidOperationException("Eyni şagird üçün bir session daxilində təkrar attendance göndərilə bilməz.");
        }

        private static void ValidateTodayOnly(DateTime sessionDate)
        {
            if (sessionDate.Date != AzerbaijanTimeHelper.GetBakuToday())
                throw new InvalidOperationException("Yalnız bugünkü tarix üçün davamiyyət yazmaq və redaktə etmək olar.");
        }

        private static void ValidatePastDateOnly(DateTime attendanceDate)
        {
            if (attendanceDate.Date >= AzerbaijanTimeHelper.GetBakuToday())
                throw new InvalidOperationException("Dəyişiklik sorğusu yalnız keçmiş tarixlər üçün yaradıla bilər.");
        }

        private static AttendanceStatus MapAttendanceStatus(string? value)
        {
            var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

            return normalized switch
            {
                "present" or "iştirak edib" or "istirak edib" => AttendanceStatus.Present,
                "absent" or "iştirak etməyib" or "istirak etmeyib" => AttendanceStatus.Absent,
                "late" or "gecikib" => AttendanceStatus.Late,
                _ => throw new InvalidOperationException("Attendance status düzgün deyil.")
            };
        }

        private static string MapAttendanceStatusText(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "Present",
                AttendanceStatus.Absent => "Absent",
                AttendanceStatus.Late => "Late",
                _ => "Unknown"
            };
        }

        private static string MapAttendanceSessionTypeText(AttendanceSessionType type)
        {
            return type switch
            {
                AttendanceSessionType.RegularLesson => "Əsas dərs",
                AttendanceSessionType.ExtraLesson => "Əlavə dərs",
                _ => "Əsas dərs"
            };
        }

        private static string? FormatTime(TimeSpan? value)
        {
            if (!value.HasValue)
                return null;

            return value.Value.ToString(@"hh\:mm");
        }

        private static TimeSpan? ParseTimeSpanOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (TimeSpan.TryParse(value, out var parsed))
                return parsed;

            throw new InvalidOperationException("Saat formatı düzgün deyil. Məsələn: 08:15");
        }

        private static AttendanceSessionDto MapSessionDto(AttendanceSession session)
        {
            return new AttendanceSessionDto
            {
                Id = session.Id,
                ClassRoomId = session.ClassRoomId,
                ClassName = session.ClassRoom?.Name ?? string.Empty,
                SubjectId = session.SubjectId,
                SubjectName = session.Subject?.Name ?? string.Empty,
                TeacherId = session.TeacherId,
                TeacherName = session.Teacher?.FullName ?? string.Empty,
                SessionDate = AzerbaijanTimeHelper.ToBakuTime(session.SessionDate),
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Notes = session.Notes,
                SessionType = MapAttendanceSessionTypeText(session.SessionType),
                IsExtraLesson = session.SessionType == AttendanceSessionType.ExtraLesson,
                IsLocked = session.IsLocked,
                PresentCount = session.Records.Count(x => x.Status == AttendanceStatus.Present),
                AbsentCount = session.Records.Count(x => x.Status == AttendanceStatus.Absent),
                LateCount = session.Records.Count(x => x.Status == AttendanceStatus.Late)
            };
        }

        private static AttendanceSessionDetailDto MapSessionDetailDto(AttendanceSession session)
        {
            return new AttendanceSessionDetailDto
            {
                Id = session.Id,
                ClassRoomId = session.ClassRoomId,
                ClassName = session.ClassRoom?.Name ?? string.Empty,
                SubjectId = session.SubjectId,
                SubjectName = session.Subject?.Name ?? string.Empty,
                TeacherId = session.TeacherId,
                TeacherName = session.Teacher?.FullName ?? string.Empty,
                SessionDate = AzerbaijanTimeHelper.ToBakuTime(session.SessionDate),
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Notes = session.Notes,
                SessionType = MapAttendanceSessionTypeText(session.SessionType),
                IsExtraLesson = session.SessionType == AttendanceSessionType.ExtraLesson,
                IsLocked = session.IsLocked,
                Records = session.Records
                    .OrderBy(x => x.Student?.FullName)
                    .Select(MapRecordDto)
                    .ToList()
            };
        }

        private static AttendanceRecordDto MapRecordDto(AttendanceRecord record)
        {
            return new AttendanceRecordDto
            {
                Id = record.Id,
                StudentId = record.StudentId,
                StudentFullName = record.Student?.FullName ?? string.Empty,
                StudentEmail = record.Student?.User?.Email ?? string.Empty,
                StudentPhotoUrl = record.Student?.User?.PhotoUrl ?? string.Empty,
                Status = MapAttendanceStatusText(record.Status),
                Notes = record.Notes,
                AbsenceReasonType = record.AbsenceReasonType,
                AbsenceReasonNote = record.AbsenceReasonNote,
                LateArrivalTime = FormatTime(record.LateArrivalTime),
                LateNote = record.LateNote
            };
        }

        private static AttendanceChangeRequestDto MapChangeRequestDto(AttendanceChangeRequest entity)
        {
            return new AttendanceChangeRequestDto
            {
                Id = entity.Id,
                ClassRoomId = entity.ClassRoomId,
                ClassName = entity.ClassRoom?.Name ?? string.Empty,
                SubjectId = entity.SubjectId,
                SubjectName = entity.Subject?.Name ?? string.Empty,
                TeacherId = entity.TeacherId,
                TeacherName = entity.Teacher?.FullName ?? string.Empty,
                StudentId = entity.StudentId,
                StudentFullName = entity.Student?.FullName ?? string.Empty,
                AttendanceDate = AzerbaijanTimeHelper.ToBakuTime(entity.AttendanceDate),
                CurrentStatus = MapAttendanceStatusText(entity.CurrentStatus),
                RequestedStatus = MapAttendanceStatusText(entity.RequestedStatus),
                RequestedChangeReason = entity.RequestedChangeReason,
                RequestedAbsenceReasonType = entity.RequestedAbsenceReasonType,
                RequestedAbsenceReasonNote = entity.RequestedAbsenceReasonNote,
                RequestedLateArrivalTime = FormatTime(entity.RequestedLateArrivalTime),
                RequestedLateNote = entity.RequestedLateNote,
                RequestedByTeacherId = entity.RequestedByTeacherId,
                RequestedAt = AzerbaijanTimeHelper.ToBakuTime(entity.RequestedAt),
                RequestStatus = entity.RequestStatus,
                ReviewedByAdminId = entity.ReviewedByAdminId,
                ReviewedAt = AzerbaijanTimeHelper.ToBakuTime(entity.ReviewedAt),
                ReviewNote = entity.ReviewNote
            };
        }
    }
}