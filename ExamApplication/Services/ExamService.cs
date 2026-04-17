using ExamApplication.DTO.Exam;
using ExamApplication.DTO.Notification;
using ExamApplication.DTO.Teacher;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamInfrastucture.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;


        private readonly ISystemSettingService _systemSettingService;

        public ExamService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            // YENI
            ISystemSettingService systemSettingService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;

            // YENI
            _systemSettingService = systemSettingService;
        }

        public async Task<List<ExamListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var exams = await _unitOfWork.Exams.GetAllWithDetailsAsync(cancellationToken);

            return exams
                .OrderByDescending(x => x.StartTime)
                .Select(MapToListItemDto)
                .ToList();
        }

        public async Task<ExamDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(id, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            return MapToDetailDto(exam);
        }

        // YENI
        public async Task EnsureStudentExamRecordsAsync(int examId, CancellationToken cancellationToken = default)
        {
            await EnsureStudentExamRecordsInternalAsync(examId, cancellationToken);
        }

        // YENI
        public async Task<TeacherMyExamCreateOptionsDto> GetTeacherCreateOptionsAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdWithDetailsAsync(currentUser.UserId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException("Müəllim tapılmadı.");

            var assignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(teacher.Id, cancellationToken);

            var classOptions = assignments
                .Where(x => x.ClassRoom != null && x.ClassRoom.IsActive)
                .GroupBy(x => x.ClassRoomId)
                .Select(g => g.First().ClassRoom!)
                .OrderBy(x => x.Name)
                .Select(x => new ExamClassOptionDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();

            var subjectOptions = assignments
                .Where(x => x.Subject != null && x.Subject.IsActive)
                .GroupBy(x => x.SubjectId)
                .Select(g => g.First().Subject!)
                .OrderBy(x => x.Name)
                .Select(x => new ExamSubjectOptionDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();

            return new TeacherMyExamCreateOptionsDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.FullName,
                ClassOptions = classOptions,
                SubjectOptions = subjectOptions
            };
        }

        // YENI
        public async Task<List<ExamListItemDto>> GetMyExamsAsync(
            TeacherExamListFilterDto? filter = null,
            CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdAsync(currentUser.UserId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException("Müəllim tapılmadı.");

            var exams = await _unitOfWork.Exams.GetByTeacherIdWithFullStatsAsync(teacher.Id, cancellationToken);

            if (filter?.ClassRoomId.HasValue == true)
                exams = exams.Where(x => x.ClassRoomId == filter.ClassRoomId.Value).ToList();

            if (filter?.SubjectId.HasValue == true)
                exams = exams.Where(x => x.SubjectId == filter.SubjectId.Value).ToList();

            if (filter?.IsPublished.HasValue == true)
                exams = exams.Where(x => x.IsPublished == filter.IsPublished.Value).ToList();

            if (!string.IsNullOrWhiteSpace(filter?.Status))
            {
                exams = exams.Where(x =>
                {
                    var resolvedStatus = ResolveExamStatus(x.IsPublished, x.StartTime, x.EndTime, x.Status);
                    return resolvedStatus.ToString().Equals(filter.Status, StringComparison.OrdinalIgnoreCase);
                }).ToList();
            }

            return exams
                .OrderByDescending(x => x.StartTime)
                .Select(MapToListItemDto)
                .ToList();
        }

        public async Task<ExamDetailDto> CreateAsync(CreateExamDto request, CancellationToken cancellationToken = default)
        {
            await ValidateCreateOrUpdateAsync(request, null, cancellationToken);

            var questionPayload = request.Questions ?? new List<ExamQuestionDto>();

            var entity = new Exam
            {
                Title = request.Title.Trim(),
                SubjectId = request.SubjectId,
                TeacherId = request.TeacherId,
                ClassRoomId = request.ClassRoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                DurationMinutes = request.DurationMinutes,
                Description = request.Description?.Trim(),
                Instructions = request.Instructions?.Trim(),
                IsPublished = request.IsPublished,

                // YENI
                // Publish və tarixə görə real status hesablanır
                Status = ResolveExamStatus(request.IsPublished, request.StartTime, request.EndTime),

                TotalQuestionCount = questionPayload.Count,
                OpenQuestionCount = questionPayload.Count(x => IsOpenQuestionType(x.Type)),
                ClosedQuestionCount = questionPayload.Count(x => !IsOpenQuestionType(x.Type))
            };

            entity.TotalScore = request.TotalScore.HasValue && request.TotalScore.Value > 0
                ? request.TotalScore.Value
                : questionPayload.Sum(x => x.Points);

            entity.ClosedQuestionScore = request.ClosedQuestionScore.HasValue && request.ClosedQuestionScore.Value > 0
                ? request.ClosedQuestionScore.Value
                : questionPayload
                    .Where(x => !IsOpenQuestionType(x.Type))
                    .Sum(x => x.Points);

            entity.Questions = BuildQuestionEntities(questionPayload);

            await _unitOfWork.Exams.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // YENI
            // Exam create zamanı publish açıqdırsa student record + access code + notification yaradırıq
            if (entity.IsPublished)
            {
                await EnsurePublishedExamArtifactsAsync(entity.Id, cancellationToken);
            }

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<ExamDetailDto> UpdateAsync(UpdateExamDto request, CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            // YENI
            // Mövcud vəziyyətə görə real statusu hesablayırıq ki Completed exam sonradan dəyişdirilə bilməsin
            var currentResolvedStatus = ResolveExamStatus(exam.IsPublished, exam.StartTime, exam.EndTime, exam.Status);

            // YENI
            // Əgər imtahan tamamlanıbsa, heç bir redaktəyə icazə vermirik
            if (currentResolvedStatus == ExamStatus.Completed)
                throw new InvalidOperationException("Tamamlanmış imtahan təxirə salına və redaktə edilə bilməz.");

            await ValidateCreateOrUpdateAsync(request, request.Id, cancellationToken);

            // YENI
            // Köhnə publish, class və vaxt vəziyyətini saxlayırıq ki sonradan düzgün müqayisə edə bilək
            var wasPublished = exam.IsPublished;
            var oldClassRoomId = exam.ClassRoomId;
            var oldStartTime = exam.StartTime;
            var oldEndTime = exam.EndTime;

            exam.Title = request.Title.Trim();
            exam.SubjectId = request.SubjectId;
            exam.TeacherId = request.TeacherId;
            exam.ClassRoomId = request.ClassRoomId;
            exam.StartTime = request.StartTime;
            exam.EndTime = request.EndTime;
            exam.DurationMinutes = request.DurationMinutes;
            exam.Description = request.Description?.Trim();
            exam.Instructions = request.Instructions?.Trim();
            exam.IsPublished = request.IsPublished;

            var questionPayload = request.Questions ?? new List<ExamQuestionDto>();

            exam.TotalQuestionCount = questionPayload.Count;
            exam.OpenQuestionCount = questionPayload.Count(x => IsOpenQuestionType(x.Type));
            exam.ClosedQuestionCount = questionPayload.Count(x => !IsOpenQuestionType(x.Type));

            exam.TotalScore = request.TotalScore > 0
                ? request.TotalScore
                : questionPayload.Sum(x => x.Points);

            exam.ClosedQuestionScore = request.ClosedQuestionScore > 0
                ? request.ClosedQuestionScore
                : questionPayload
                    .Where(x => !IsOpenQuestionType(x.Type))
                    .Sum(x => x.Points);

            RemoveExistingQuestionGraph(exam);
            exam.Questions = BuildQuestionEntities(questionPayload);

            // YENI
            exam.Status = ResolveExamStatus(exam.IsPublished, exam.StartTime, exam.EndTime, exam.Status);

            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // YENI
            // Əvvəl publish deyildisə, indi publish oldusa bütün publish addımlarını bir dəfə işlədirik
            if (!wasPublished && exam.IsPublished)
            {
                await EnsurePublishedExamArtifactsAsync(exam.Id, cancellationToken);
            }
            // YENI
            // Exam əvvəldən publish idisə və sinif dəyişibsə student exam-ləri və access code-ları tamamlayırıq
            else if (exam.IsPublished && oldClassRoomId != exam.ClassRoomId)
            {
                await EnsureStudentExamRecordsInternalAsync(exam.Id, cancellationToken);
                await GenerateExamAccessCodesByExamIdAsync(exam.Id, cancellationToken);
            }
            // YENI
            // Publish olunmuş exam-da vaxt dəyişibsə reschedule notification göndəririk
            else if (
                exam.IsPublished &&
                (
                    oldStartTime != exam.StartTime ||
                    oldEndTime != exam.EndTime
                )
            )
            {
                // YENI
                var settings = await _systemSettingService.GetAsync(cancellationToken);

                if (settings.ExamRescheduleNotificationEnabled)
                {
                    await CreateExamRescheduledNotificationsAsync(exam.Id, cancellationToken);
                }
            }

            return await GetByIdAsync(exam.Id, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(id, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            if (exam.StudentExams != null && exam.StudentExams.Any())
                throw new InvalidOperationException("Bu imtahana artıq student nəticələri bağlıdır. Silmək olmaz.");

            RemoveExistingQuestionGraph(exam);

            _unitOfWork.Exams.Remove(exam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task PublishAsync(int id, CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(id, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser != null && string.Equals(currentUser.Role, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var currentTeacher = await _unitOfWork.Teachers.GetByUserIdAsync(currentUser.UserId, cancellationToken);
                if (currentTeacher == null)
                    throw new UnauthorizedAccessException("Cari müəllim tapılmadı.");

                if (exam.TeacherId != currentTeacher.Id)
                    throw new UnauthorizedAccessException("Bu imtahanı yayımlamaq icazəniz yoxdur.");
            }

            // YENI
            // Publish artıq açıqdırsa ikinci dəfə notification və access code yaratmırıq
            if (exam.IsPublished)
                return;

            exam.IsPublished = true;
            exam.Status = ResolveExamStatus(true, exam.StartTime, exam.EndTime, exam.Status);

            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await EnsurePublishedExamArtifactsAsync(exam.Id, cancellationToken);
        }

        public async Task<List<ExamClassOptionDto>> GetClassOptionsAsync(CancellationToken cancellationToken = default)
        {
            var classes = await _unitOfWork.ClassRooms.GetAllAsync(cancellationToken);

            return classes
                .Where(x => x.IsActive)
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .Select(x => new ExamClassOptionDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        public async Task<List<ExamSubjectOptionDto>> GetSubjectOptionsAsync(CancellationToken cancellationToken = default)
        {
            var subjects = await _unitOfWork.Subjects.GetAllAsync(cancellationToken);

            return subjects
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new ExamSubjectOptionDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        public async Task<List<ExamTeacherOptionDto>> GetTeacherOptionsAsync(CancellationToken cancellationToken = default)
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(cancellationToken);

            return teachers
                .Where(x => IsTeacherAvailable(x.Status))
                .OrderBy(x => x.FullName)
                .Select(x => new ExamTeacherOptionDto
                {
                    Id = x.Id,
                    FullName = x.FullName
                })
                .ToList();
        }

        private async Task ValidateCreateOrUpdateAsync(
            CreateExamDto request,
            int? currentExamId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("İmtahan adı boş ola bilməz.");

            if (request.SubjectId <= 0)
                throw new ArgumentException("Subject seçilməlidir.");

            if (request.TeacherId <= 0)
                throw new ArgumentException("Teacher seçilməlidir.");

            if (request.StartTime >= request.EndTime)
                throw new ArgumentException("Başlama vaxtı bitmə vaxtından kiçik olmalıdır.");

            if (request.DurationMinutes <= 0)
                throw new ArgumentException("DurationMinutes 0-dan böyük olmalıdır.");

            if (request.Questions == null || request.Questions.Count == 0)
                throw new ArgumentException("Ən azı 1 sual olmalıdır.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new InvalidOperationException("Seçilmiş subject tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new InvalidOperationException("Seçilmiş teacher tapılmadı.");

            if (request.ClassRoomId.HasValue)
            {
                var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId.Value, cancellationToken);
                if (classRoom == null)
                    throw new InvalidOperationException("Seçilmiş class tapılmadı.");
            }

            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser != null && string.Equals(currentUser.Role, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var currentTeacher = await _unitOfWork.Teachers.GetByUserIdAsync(currentUser.UserId, cancellationToken);
                if (currentTeacher == null)
                    throw new UnauthorizedAccessException("Cari müəllim tapılmadı.");

                if (request.TeacherId != currentTeacher.Id)
                    throw new UnauthorizedAccessException("Müəllim yalnız öz adından imtahan yarada və ya yeniləyə bilər.");

                var assignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(currentTeacher.Id, cancellationToken);

                var hasSubjectAccess = assignments.Any(x => x.SubjectId == request.SubjectId);
                if (!hasSubjectAccess)
                    throw new UnauthorizedAccessException("Bu fənn üzrə imtahan yaratmaq icazəniz yoxdur.");

                if (request.ClassRoomId.HasValue)
                {
                    var hasClassSubjectAccess = assignments.Any(x =>
                        x.ClassRoomId == request.ClassRoomId.Value &&
                        x.SubjectId == request.SubjectId);

                    if (!hasClassSubjectAccess)
                        throw new UnauthorizedAccessException("Bu sinif və fənn üzrə imtahan yaratmaq icazəniz yoxdur.");
                }
            }

            ValidateQuestions(request.Questions);
        }

        private static void ValidateQuestions(List<ExamQuestionDto> questions)
        {
            if (questions.Count == 0)
                throw new ArgumentException("Questions boş ola bilməz.");

            var orderNumbers = questions.Select(q => q.OrderNumber).ToList();
            if (orderNumbers.Any(x => x <= 0))
                throw new ArgumentException("Bütün sualların OrderNumber dəyəri 0-dan böyük olmalıdır.");

            if (orderNumbers.Count != orderNumbers.Distinct().Count())
                throw new ArgumentException("Sual sıraları təkrarlana bilməz.");

            foreach (var question in questions)
            {
                if (string.IsNullOrWhiteSpace(question.QuestionText))
                    throw new ArgumentException("QuestionText boş ola bilməz.");

                if (question.Points <= 0)
                    throw new ArgumentException("Sual balı 0-dan böyük olmalıdır.");

                var isOpen = IsOpenQuestionType(question.Type);

                if (isOpen)
                    continue;

                if (question.Options == null || question.Options.Count < 2)
                    throw new ArgumentException("Qapalı sual üçün ən azı 2 variant olmalıdır.");

                var optionOrderNumbers = question.Options.Select(x => x.OrderNumber).ToList();
                if (optionOrderNumbers.Any(x => x <= 0))
                    throw new ArgumentException("Variant sıraları 0-dan böyük olmalıdır.");

                if (optionOrderNumbers.Count != optionOrderNumbers.Distinct().Count())
                    throw new ArgumentException("Variant sıraları təkrarlana bilməz.");

                if (question.Options.Any(x => string.IsNullOrWhiteSpace(x.OptionText)))
                    throw new ArgumentException("Variant mətni boş ola bilməz.");

                var correctCount = question.Options.Count(x => x.IsCorrect);

                if (IsMultipleSelection(question.SelectionMode))
                {
                    if (correctCount < 1)
                        throw new ArgumentException("Multiple seçimli sualda ən azı 1 doğru cavab olmalıdır.");
                }
                else
                {
                    if (correctCount != 1)
                        throw new ArgumentException("Single seçimli sualda yalnız 1 doğru cavab olmalıdır.");
                }
            }
        }

        private List<ExamQuestion> BuildQuestionEntities(List<ExamQuestionDto> questions)
        {
            return questions
                .OrderBy(x => x.OrderNumber)
                .Select(q => new ExamQuestion
                {
                    QuestionText = q.QuestionText.Trim(),
                    QuestionType = MapQuestionType(q.Type),
                    Points = q.Points,
                    OrderNumber = q.OrderNumber,
                    Description = q.Description?.Trim(),
                    SelectionMode = IsOpenQuestionType(q.Type)
                        ? null
                        : MapSelectionMode(q.SelectionMode),
                    Options = IsOpenQuestionType(q.Type)
                        ? new List<ExamOption>()
                        : (q.Options ?? new List<ExamOptionDto>())
                            .OrderBy(x => x.OrderNumber)
                            .Select(o => new ExamOption
                            {
                                OptionText = o.OptionText.Trim(),
                                IsCorrect = o.IsCorrect,
                                OptionKey = string.IsNullOrWhiteSpace(o.OptionKey) ? null : o.OptionKey.Trim(),
                                OrderNumber = o.OrderNumber
                            })
                            .ToList()
                })
                .ToList();
        }

        private void RemoveExistingQuestionGraph(Exam exam)
        {
            if (exam.Questions == null || exam.Questions.Count == 0)
                return;

            foreach (var question in exam.Questions.ToList())
            {
                if (question.Options != null)
                {
                    foreach (var option in question.Options.ToList())
                    {
                        _unitOfWork.ExamOptions.Remove(option);
                    }
                }

                _unitOfWork.ExamQuestions.Remove(question);
            }

            exam.Questions.Clear();
        }

        // YENI
        private async Task GenerateExamAccessCodesByExamIdAsync(int examId, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            await GenerateExamAccessCodesInternalAsync(exam, cancellationToken);
        }

        // YENI
        private async Task GenerateExamAccessCodesInternalAsync(Exam exam, CancellationToken cancellationToken)
        {
            if (!exam.ClassRoomId.HasValue)
                return;

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);
            var lateToleranceMinutes = settings.LateEntryToleranceMinutes >= 0
                ? settings.LateEntryToleranceMinutes
                : 10;

            var studentClasses = await _unitOfWork.StudentClasses.GetActiveByClassRoomIdAsync(
                exam.ClassRoomId.Value,
                cancellationToken);

            foreach (var studentClass in studentClasses)
            {
                var exists = await _unitOfWork.ExamAccessCodes.ExistsAsync(
                    exam.Id,
                    studentClass.StudentId,
                    cancellationToken);

                if (exists)
                    continue;

                var code = await GenerateUniqueAccessCodeAsync(cancellationToken);

                var accessCode = new ExamAccessCode
                {
                    ExamId = exam.Id,
                    StudentId = studentClass.StudentId,
                    AccessCode = code,
                    IsUsed = false,

                    // YENI
                    ExpireAt = exam.EndTime.AddMinutes(lateToleranceMinutes)
                };

                await _unitOfWork.ExamAccessCodes.AddAsync(accessCode, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        private async Task<string> GenerateUniqueAccessCodeAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var code = $"EXM-{Random.Shared.Next(100000, 999999)}";
                var existing = await _unitOfWork.ExamAccessCodes.GetByCodeAsync(code, cancellationToken);

                if (existing == null)
                    return code;
            }
        }

        private ExamListItemDto MapToListItemDto(Exam exam)
        {
            var resolvedStatus = ResolveExamStatus(exam.IsPublished, exam.StartTime, exam.EndTime, exam.Status);

            return new ExamListItemDto
            {
                Id = exam.Id,
                Title = exam.Title,
                ClassRoomId = exam.ClassRoomId,
                ClassName = exam.ClassRoom?.Name,
                SubjectId = exam.SubjectId,
                SubjectName = exam.Subject?.Name ?? string.Empty,
                TeacherId = exam.TeacherId,
                TeacherName = exam.Teacher?.FullName ?? string.Empty,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,

                // YENI
                // List cavabında həmişə real status qaytarırıq
                Status = resolvedStatus.ToString(),

                // YENI
                TotalQuestionCount = exam.TotalQuestionCount > 0
             ? exam.TotalQuestionCount
             : (exam.Questions?.Count ?? 0),

                // YENI
                // İmtahanın öz ümumi balı
                TotalScore = exam.TotalScore.HasValue && exam.TotalScore.Value > 0
             ? exam.TotalScore.Value
             : (exam.Questions != null && exam.Questions.Any()
                 ? exam.Questions.Sum(x => x.Points)
                 : 0),

                IsPublished = exam.IsPublished
            };
        }

        public async Task<GradeStudentExamResultDto> GradeStudentExamAsync(
            GradeStudentExamRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var studentExam = await _unitOfWork.StudentExams
                .GetByIdWithFullDetailsAsync(request.StudentExamId, cancellationToken);

            if (studentExam == null)
                throw new KeyNotFoundException("StudentExam tapılmadı.");

            if (!studentExam.IsCompleted)
                throw new InvalidOperationException("İmtahan tamamlanmayıb.");

            var answers = await _unitOfWork.StudentAnswers
                .GetByStudentExamIdForTeacherReviewAsync(studentExam.Id, cancellationToken);

            foreach (var item in request.Answers)
            {
                var answer = answers.FirstOrDefault(x => x.Id == item.StudentAnswerId);
                if (answer == null)
                    continue;

                if (!IsOpenQuestionType(answer.ExamQuestion?.QuestionType.ToString()))
                    continue;

                var max = answer.ExamQuestion?.Points ?? 0;

                if (item.Score > max)
                    throw new Exception("Bal maksimumu keçə bilməz.");

                answer.PointsAwarded = item.Score;
                answer.IsReviewed = true;
                answer.ReviewedAt = DateTime.UtcNow;
                answer.TeacherFeedback = item.TeacherFeedback;

                _unitOfWork.StudentAnswers.Update(answer);
            }

            var openAnswers = answers
                .Where(x => IsOpenQuestionType(x.ExamQuestion?.QuestionType.ToString()))
                .ToList();

            var manualScore = openAnswers.Sum(x => x.PointsAwarded);

            studentExam.ManualGradedScore = manualScore;
            studentExam.Score = studentExam.AutoGradedScore + manualScore;
            studentExam.IsReviewed = openAnswers.All(x => x.IsReviewed);
            studentExam.ReviewedAt = DateTime.UtcNow;

            if (studentExam.IsReviewed)
            {
                studentExam.Status = StudentExamStatus.Reviewed;
            }

            _unitOfWork.StudentExams.Update(studentExam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (studentExam.Student?.UserId > 0)
            {
                await _notificationService.CreateAsync(new ExamApplication.DTO.Notification.CreateNotificationDto
                {
                    UserId = studentExam.Student.UserId,
                    Title = "İmtahan nəticəniz yoxlanıldı",
                    Message = $"{studentExam.Exam?.Title ?? "İmtahan"} üzrə nəticəniz müəllim tərəfindən yoxlanıldı.",
                    Type = (int)NotificationType.Exam,
                    Category = (int)NotificationCategory.ExamResultPublished,
                    Priority = (int)NotificationPriority.High,
                    RelatedEntityType = "StudentExam",
                    RelatedEntityId = studentExam.Id,
                    ActionUrl = $"/student/exams/{studentExam.ExamId}",
                    Icon = "exam-result",
                    MetadataJson = $@"{{""studentExamId"":{studentExam.Id},""examId"":{studentExam.ExamId},""score"":{studentExam.Score}}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                }, cancellationToken);
            }

            return new GradeStudentExamResultDto
            {
                StudentExamId = studentExam.Id,
                IsReviewed = studentExam.IsReviewed,
                AutoGradedScore = studentExam.AutoGradedScore,
                ManualGradedScore = studentExam.ManualGradedScore,
                TotalScore = studentExam.Score,
                Message = "Qiymətləndirmə tamamlandı"
            };
        }

        public async Task<List<ExamSubmissionStudentDto>> GetExamSubmissionsAsync(
            int examId,
            CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            await EnsureTeacherCanAccessExamAsync(exam, cancellationToken);

            var studentExams = await _unitOfWork.StudentExams
                .GetCompletedByExamIdWithDetailsAsync(examId, cancellationToken);

            return studentExams.Select(studentExam =>
            {
                var answers = studentExam.Answers?.ToList() ?? new List<StudentAnswer>();

                var openAnswers = answers
                    .Where(x => IsOpenQuestionType(x.ExamQuestion?.QuestionType.ToString()))
                    .ToList();

                var answeredQuestionsCount = answers.Count(IsAnsweredQuestion);

                return new ExamSubmissionStudentDto
                {
                    StudentExamId = studentExam.Id,
                    StudentId = studentExam.StudentId,
                    StudentFullName = studentExam.Student?.FullName ?? "Naməlum",
                    StudentNumber = studentExam.Student?.StudentNumber ?? "",
                    StartTime = studentExam.StartTime,
                    SubmittedAt = studentExam.SubmittedAt,
                    IsCompleted = studentExam.IsCompleted,
                    IsReviewed = studentExam.IsReviewed,
                    Score = studentExam.Score,
                    AutoGradedScore = studentExam.AutoGradedScore,
                    ManualGradedScore = studentExam.ManualGradedScore,
                    TotalQuestions = exam.Questions?.Count ?? 0,
                    AnsweredQuestions = answeredQuestionsCount,
                    OpenQuestionsCount = openAnswers.Count,
                    ReviewedOpenQuestionsCount = openAnswers.Count(x => x.IsReviewed)
                };
            }).ToList();
        }

        public async Task<ExamSubmissionDetailDto> GetStudentExamSubmissionDetailAsync(
            int examId,
            int studentExamId,
            CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            await EnsureTeacherCanAccessExamAsync(exam, cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByIdWithFullDetailsAsync(studentExamId, cancellationToken);

            if (studentExam == null)
                throw new KeyNotFoundException("Student exam tapılmadı.");

            if (studentExam.ExamId != examId)
                throw new Exception("Student exam bu imtahana aid deyil.");

            var answers = studentExam.Answers ?? new List<StudentAnswer>();

            return new ExamSubmissionDetailDto
            {
                StudentExamId = studentExam.Id,
                ExamId = studentExam.ExamId,
                ExamTitle = studentExam.Exam?.Title ?? "",
                SubjectName = studentExam.Exam?.Subject?.Name ?? "",
                StudentId = studentExam.StudentId,
                StudentFullName = studentExam.Student?.FullName ?? "",
                StudentNumber = studentExam.Student?.StudentNumber ?? "",
                StartTime = studentExam.StartTime,
                SubmittedAt = studentExam.SubmittedAt,
                IsCompleted = studentExam.IsCompleted,
                IsReviewed = studentExam.IsReviewed,
                Score = studentExam.Score,
                AutoGradedScore = studentExam.AutoGradedScore,
                ManualGradedScore = studentExam.ManualGradedScore,
                Answers = answers.Select(a => new ExamSubmissionAnswerDto
                {
                    StudentAnswerId = a.Id,
                    QuestionId = a.ExamQuestionId,
                    QuestionText = a.ExamQuestion?.QuestionText ?? "",
                    QuestionType = a.ExamQuestion?.QuestionType.ToString() ?? "",
                    MaxScore = a.ExamQuestion?.Points ?? 0,
                    AwardedScore = a.PointsAwarded,
                    IsReviewed = a.IsReviewed,
                    IsCorrect = a.IsCorrect,
                    StudentAnswerText = a.AnswerText,
                    TeacherFeedback = a.TeacherFeedback,
                    Options = (a.ExamQuestion?.Options ?? new List<ExamOption>())
                        .Select(o => new ExamSubmissionAnswerOptionDto
                        {
                            Id = o.Id,
                            Text = o.OptionText,
                            IsCorrect = o.IsCorrect,
                            IsSelected =
                                a.SelectedOptionId == o.Id ||
                                (a.SelectedOptions != null && a.SelectedOptions.Any(x => x.ExamOptionId == o.Id))
                        }).ToList()
                }).ToList()
            };
        }

        private ExamDetailDto MapToDetailDto(Exam exam)
        {
            var questions = (exam.Questions ?? new List<ExamQuestion>())
                .OrderBy(x => x.OrderNumber)
                .Select(q => new ExamQuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Type = q.QuestionType.ToString(),
                    Points = q.Points,
                    OrderNumber = q.OrderNumber,
                    Description = q.Description,
                    SelectionMode = q.SelectionMode?.ToString(),
                    Options = (q.Options ?? new List<ExamOption>())
                        .OrderBy(x => x.OrderNumber)
                        .Select(o => new ExamOptionDto
                        {
                            Id = o.Id,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect,
                            OptionKey = o.OptionKey,
                            OrderNumber = o.OrderNumber
                        })
                        .ToList()
                })
                .ToList();

            return new ExamDetailDto
            {
                Id = exam.Id,
                Title = exam.Title,
                SubjectId = exam.SubjectId,
                TeacherId = exam.TeacherId,
                ClassRoomId = exam.ClassRoomId,
                ClassName = exam.ClassRoom?.Name,
                SubjectName = exam.Subject?.Name ?? string.Empty,
                TeacherName = exam.Teacher?.FullName ?? string.Empty,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                DurationMinutes = exam.DurationMinutes,
                Description = exam.Description,
                TotalScore = exam.TotalScore ?? questions.Sum(x => x.Points),
                ClosedQuestionScore = exam.ClosedQuestionScore ?? questions.Where(x => !IsOpenQuestionType(x.Type)).Sum(x => x.Points),
                TotalQuestionCount = exam.TotalQuestionCount > 0 ? exam.TotalQuestionCount : questions.Count,
                OpenQuestionCount = exam.OpenQuestionCount > 0 ? exam.OpenQuestionCount : questions.Count(x => IsOpenQuestionType(x.Type)),
                ClosedQuestionCount = exam.ClosedQuestionCount > 0 ? exam.ClosedQuestionCount : questions.Count(x => !IsOpenQuestionType(x.Type)),
                Instructions = exam.Instructions,

                // YENI
                Status = ResolveExamStatus(exam.IsPublished, exam.StartTime, exam.EndTime, exam.Status).ToString(),

                IsPublished = exam.IsPublished,
                Questions = questions
            };
        }

        private static QuestionType MapQuestionType(string? type)
        {
            var value = (type ?? string.Empty).Trim().ToLowerInvariant();

            if (value is "open" or "opentext" or "essay" or "aciq" or "açıq")
            {
                if (Enum.TryParse<QuestionType>("OpenText", true, out var openText))
                    return openText;

                if (Enum.TryParse<QuestionType>("Open", true, out var open))
                    return open;
            }

            if (value is "closed" or "multiplechoice" or "singlechoice" or "test" or "qapali" or "qapalı")
            {
                if (Enum.TryParse<QuestionType>("MultipleChoice", true, out var mc))
                    return mc;

                if (Enum.TryParse<QuestionType>("Closed", true, out var closed))
                    return closed;
            }

            if (Enum.TryParse<QuestionType>(type, true, out var parsed))
                return parsed;

            return Enum.GetValues<QuestionType>().First();
        }

        private static QuestionSelectionMode? MapSelectionMode(string? selectionMode)
        {
            if (string.IsNullOrWhiteSpace(selectionMode))
                return null;

            var value = selectionMode.Trim().ToLowerInvariant();

            if (value is "multiple" or "multi" or "checkbox")
            {
                if (Enum.TryParse<QuestionSelectionMode>("Multiple", true, out var multiple))
                    return multiple;
            }

            if (value is "single" or "radio")
            {
                if (Enum.TryParse<QuestionSelectionMode>("Single", true, out var single))
                    return single;
            }

            if (Enum.TryParse<QuestionSelectionMode>(selectionMode, true, out var parsed))
                return parsed;

            return null;
        }

        private static bool IsOpenQuestionType(string? type)
        {
            var value = (type ?? string.Empty).Trim().ToLowerInvariant();
            return value is "open" or "opentext" or "essay" or "aciq" or "açıq";
        }

        private static bool IsMultipleSelection(string? selectionMode)
        {
            var value = (selectionMode ?? string.Empty).Trim().ToLowerInvariant();
            return value is "multiple" or "multi" or "checkbox";
        }

        private static bool IsTeacherAvailable(TeacherStatus status)
        {
            var name = status.ToString().ToLowerInvariant();
            return name != "passive";
        }

        // YENI
        // Exam status-un yeganə mənbəyi bu helper-dir
        private static ExamStatus ResolveExamStatus(
     bool isPublished,
     DateTime startTime,
     DateTime endTime,
     ExamStatus? currentStatus = null)
        {
            // Cancelled status varsa onu saxlayırıq
            if (currentStatus == ExamStatus.Cancelled)
                return ExamStatus.Cancelled;

            // Publish olunmayıbsa draft qalır
            if (!isPublished)
                return ExamStatus.Draft;

            // VACIB DÜZƏLİŞ:
            // StudentExamService DateTime.Now ilə işləyir.
            // Burada da eyni məntiq saxlanmalıdır ki, status fərqi yaranmasın.
            var now = DateTime.Now;

            // Vaxt bitibsə completed
            if (endTime <= now)
                return ExamStatus.Completed;

            // Hal-hazırda davam edirsə active
            if (startTime <= now && endTime > now)
                return ExamStatus.Active;

            // Hələ başlamayıbsa planned
            return ExamStatus.Planned;
        }

        // YENI
        private async Task EnsurePublishedExamArtifactsAsync(int examId, CancellationToken cancellationToken)
        {
            // Publish zamanı student exam qeydləri yaranır
            await EnsureStudentExamRecordsInternalAsync(examId, cancellationToken);

            // Publish zamanı access code da yaradılır
            await GenerateExamAccessCodesByExamIdAsync(examId, cancellationToken);

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            // Publish notification yalnız ayarlarda aktivdirsə göndərilir
            if (settings.ExamPublishNotificationEnabled)
            {
                await _notificationService.CreateExamPublishNotificationsAsync(examId, cancellationToken);
            }
        }

        // YENI
        private async Task EnsureStudentExamRecordsInternalAsync(int examId, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);
            if (exam == null)
                throw new KeyNotFoundException("İmtahan tapılmadı.");

            if (!exam.ClassRoomId.HasValue)
                throw new InvalidOperationException("İmtahan üçün sinif seçilməyib.");

            var studentClasses = await _unitOfWork.StudentClasses.GetActiveByClassRoomIdAsync(exam.ClassRoomId.Value, cancellationToken);

            var studentIds = studentClasses
                .Where(x => x.IsActive)
                .Select(x => x.StudentId)
                .Distinct()
                .ToList();

            foreach (var studentId in studentIds)
            {
                var exists = await _unitOfWork.StudentExams.ExistsAsync(studentId, exam.Id, cancellationToken);
                if (exists)
                    continue;

                await _unitOfWork.StudentExams.AddAsync(new StudentExam
                {
                    StudentId = studentId,
                    ExamId = exam.Id,
                    StartTime = exam.StartTime,
                    EndTime = null,
                    Score = 0,
                    IsCompleted = false,

                    // YENI
                    Status = StudentExamStatus.Pending,
                    IsReviewed = false,
                    AutoGradedScore = 0,
                    ManualGradedScore = 0,
                    WarningCount = 0,
                    TabSwitchCount = 0,
                    FullScreenExitCount = 0,
                    IsLocked = false,
                    IsAutoSubmitted = false
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        private static bool IsAnsweredQuestion(StudentAnswer answer)
        {
            if (!string.IsNullOrWhiteSpace(answer.AnswerText))
                return true;

            if (answer.SelectedOptionId.HasValue)
                return true;

            if (answer.SelectedOptions != null && answer.SelectedOptions.Any())
                return true;

            return false;
        }

        // YENI
        private async Task CreateExamRescheduledNotificationsAsync(int examId, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);
            if (exam == null || !exam.ClassRoomId.HasValue)
                return;

            var studentClasses = await _unitOfWork.StudentClasses.GetActiveByClassRoomIdAsync(
                exam.ClassRoomId.Value,
                cancellationToken);

            var studentIds = studentClasses
                .Select(x => x.StudentId)
                .Distinct()
                .ToList();

            if (!studentIds.Any())
                return;

            var students = await _unitOfWork.Students.GetByIdsWithDetailsAsync(studentIds, cancellationToken);

            var notifications = students
                .Where(s => s.UserId > 0)
                .Select(s => new CreateNotificationDto
                {
                    UserId = s.UserId,
                    Title = "İmtahan vaxtı dəyişdirildi",
                    Message = $"{exam.Title} imtahanının vaxtı yeniləndi.",
                    Type = (int)NotificationType.Exam,
                    Category = (int)NotificationCategory.ExamRescheduled,
                    Priority = (int)NotificationPriority.High,
                    RelatedEntityType = "Exam",
                    RelatedEntityId = exam.Id,
                    ActionUrl = $"/student/exams/{exam.Id}",
                    Icon = "exam",
                    MetadataJson = $@"{{""examId"":{exam.Id},""classRoomId"":{exam.ClassRoomId},""startTime"":""{exam.StartTime:O}"",""endTime"":""{exam.EndTime:O}""}}",
                    ExpiresAt = exam.EndTime.AddDays(7)
                })
                .ToList();

            await _notificationService.CreateBulkAsync(notifications, cancellationToken);
        }

        private async Task EnsureTeacherCanAccessExamAsync(Exam exam, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException();

            if (currentUser.Role == "Teacher")
            {
                var teacher = await _unitOfWork.Teachers
                    .GetByUserIdAsync(currentUser.UserId, cancellationToken);

                if (teacher == null || teacher.Id != exam.TeacherId)
                    throw new UnauthorizedAccessException("İcazə yoxdur");
            }
        }
    }
}