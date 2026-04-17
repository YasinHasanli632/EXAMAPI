using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;
using ExamApplication.DTO.Settings;

namespace ExamApplication.Services
{
    public class StudentExamService : IStudentExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        // YENI
        private readonly ISystemSettingService _systemSettingService;

        public StudentExamService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            // YENI
            ISystemSettingService systemSettingService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;

            // YENI
            _systemSettingService = systemSettingService;
        }

        public async Task<List<StudentExamSummaryDto>> GetMyExamsAsync(CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExams = await _unitOfWork.StudentExams
                .GetByStudentIdWithDetailsAsync(student.Id, cancellationToken);

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            var result = new List<StudentExamSummaryDto>();

            foreach (var studentExam in studentExams)
            {
                var exam = studentExam.Exam;

                var accessCode = await _unitOfWork.ExamAccessCodes
                    .GetByExamAndStudentAsync(exam.Id, student.Id, cancellationToken);

                // YENI
                var isAccessCodeReady = ShouldExposeAccessCode(exam, studentExam, accessCode, settings);
                var canEnter = CanEnterExam(exam, studentExam, settings);
                var canStart = CanStartExam(exam, studentExam, settings);

                var dto = new StudentExamSummaryDto
                {
                    StudentExamId = studentExam.Id,
                    ExamId = exam.Id,
                    ExamTitle = exam.Title,
                    SubjectName = exam.Subject?.Name ?? string.Empty,
                    TeacherName = exam.Teacher?.User?.FullName ?? exam.Teacher?.FullName ?? string.Empty,
                    Score = studentExam.Score,
                    MaxScore = exam.TotalScore ?? exam.Questions.Sum(q => q.Points),
                    IsCompleted = studentExam.IsCompleted,
                    StartTime = exam.StartTime,
                    EndTime = exam.EndTime,
                    ExamType = exam.Type.ToString(),
                    Note = exam.Description,
                    Status = studentExam.Status.ToString(),
                    ExamStartTime = exam.StartTime,
                    ExamEndTime = exam.EndTime,
                    DurationMinutes = exam.DurationMinutes,

                    // YENI
                    IsAccessCodeReady = isAccessCodeReady,
                    CanEnter = canEnter,
                    CanStart = canStart,
                    IsMissed = studentExam.Status == StudentExamStatus.Missed,
                    LateEntryToleranceMinutes = settings.LateEntryToleranceMinutes,
                    // YENI
                    AccessCode = isAccessCodeReady ? accessCode?.AccessCode : null,
                    AccessCodeActivationMinutes = settings.AccessCodeActivationMinutes

                };

                result.Add(dto);
            }

            return result
                .OrderByDescending(x => x.ExamStartTime)
                .ToList();
        }

        public async Task<StudentExamDetailDto> GetMyExamDetailAsync(int examId, CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByStudentAndExamWithFullDetailsAsync(student.Id, examId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Bu imtahan tələbə üçün tapılmadı.");

            var exam = studentExam.Exam;

            var accessCode = await _unitOfWork.ExamAccessCodes
                .GetByExamAndStudentAsync(exam.Id, student.Id, cancellationToken);

            var settings = await _systemSettingService.GetAsync(cancellationToken);

            var isAccessCodeReady = ShouldExposeAccessCode(exam, studentExam, accessCode, settings);
            var canVerifyCode = CanEnterExam(exam, studentExam, settings);
            var canStart = CanStartExam(exam, studentExam, settings);

            var hasOpenQuestions = HasOpenQuestions(exam);
            var isMissed = studentExam.Status == StudentExamStatus.Missed;
            var isCompleted = studentExam.IsCompleted;

            var requiresManualReview = isCompleted && hasOpenQuestions && !studentExam.IsReviewed;
            var canShowScoreImmediately = settings.ShowScoreImmediately && !hasOpenQuestions;
            var isResultAutoPublished = settings.AutoPublishResults && !hasOpenQuestions;

            decimal? publishedScore = null;

            if (isMissed)
            {
                publishedScore = 0;
            }
            else if (isCompleted && canShowScoreImmediately)
            {
                publishedScore = studentExam.Score;
            }
            else if (isCompleted && hasOpenQuestions && studentExam.IsReviewed)
            {
                publishedScore = studentExam.Score;
            }

            var resultMessage = string.Empty;

            if (isMissed)
            {
                resultMessage = "İmtahan buraxılıb. Bal avtomatik olaraq 0 hesablandı.";
            }
            else if (isCompleted && hasOpenQuestions && !studentExam.IsReviewed)
            {
                resultMessage = "Açıq suallar müəllim tərəfindən yoxlanılır.";
            }
            else if (isCompleted && hasOpenQuestions && studentExam.IsReviewed)
            {
                resultMessage = "Açıq suallar yoxlanıldı. Yekun nəticə hazırdır.";
            }
            else if (isCompleted && publishedScore.HasValue)
            {
                resultMessage = "İmtahan nəticəsi hazırdır.";
            }
            else if (isCompleted)
            {
                resultMessage = "İmtahan tamamlanıb.";
            }

            return new StudentExamDetailDto
            {
                ExamId = exam.Id,
                StudentExamId = studentExam.Id,
                ExamTitle = exam.Title,
                SubjectName = exam.Subject?.Name ?? string.Empty,
                TeacherName = exam.Teacher?.User?.FullName ?? exam.Teacher?.FullName ?? string.Empty,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                DurationMinutes = exam.DurationMinutes,
                Instructions = exam.Instructions ?? exam.Description ?? string.Empty,
                Status = studentExam.Status.ToString(),
                IsAccessCodeReady = isAccessCodeReady,
                CanVerifyCode = canVerifyCode,
                CanStart = canStart,
                IsCompleted = studentExam.IsCompleted,
                AccessCode = isAccessCodeReady ? accessCode?.AccessCode : null,

                // YENI
                IsMissed = isMissed,
                AccessCodeActivationMinutes = settings.AccessCodeActivationMinutes,
                LateEntryToleranceMinutes = settings.LateEntryToleranceMinutes,
                Score = studentExam.Score,
                PublishedScore = publishedScore,
                RequiresManualReview = requiresManualReview,
                CanShowScoreImmediately = canShowScoreImmediately,
                IsResultAutoPublished = isResultAutoPublished,
                HasOpenQuestions = hasOpenQuestions,
                ResultMessage = resultMessage
            };
        }
        // YENI
        // Layihədə exam vaxtları lokal vaxt kimi işlədiyi üçün local now istifadə edirik
        private static DateTime GetNow()
        {
            return DateTime.Now;
        }

        // KOHNE QALIR
        private static bool IsAccessWindowOpen(Exam exam)
        {
            var now = GetNow();
            var activationTime = exam.StartTime.AddMinutes(-5);

            return now >= activationTime && now <= exam.EndTime;
        }

        // YENI
        private static bool IsAccessWindowOpen(Exam exam, SystemSettingDto settings)
        {
            var now = GetNow();
            var activationMinutes = Math.Max(0, settings.AccessCodeActivationMinutes);
            var lateToleranceMinutes = Math.Max(0, settings.LateEntryToleranceMinutes);

            var activationTime = exam.StartTime.AddMinutes(-activationMinutes);
            var closeTime = exam.EndTime.AddMinutes(lateToleranceMinutes);

            return now >= activationTime && now <= closeTime;
        }

        // KOHNE QALIR
        private static bool CanEnterExam(Exam exam, StudentExam studentExam)
        {
            if (!exam.IsPublished)
                return false;

            if (exam.Status == ExamStatus.Cancelled)
                return false;

            if (studentExam.IsCompleted)
                return false;

            if (studentExam.Status == StudentExamStatus.Missed)
                return false;

            return IsAccessWindowOpen(exam);
        }

        // YENI
        private static bool CanEnterExam(Exam exam, StudentExam studentExam, SystemSettingDto settings)
        {
            if (!exam.IsPublished)
                return false;

            if (exam.Status == ExamStatus.Cancelled)
                return false;

            if (studentExam.IsCompleted)
                return false;

            if (studentExam.Status == StudentExamStatus.Missed)
                return false;

            return IsAccessWindowOpen(exam, settings);
        }

        // KOHNE QALIR
        private static bool CanStartExam(Exam exam, StudentExam studentExam)
        {
            if (!CanEnterExam(exam, studentExam))
                return false;

            return studentExam.Status == StudentExamStatus.ReadyToStart
                || studentExam.Status == StudentExamStatus.InProgress
                || studentExam.Status == StudentExamStatus.Pending
                || studentExam.Status == StudentExamStatus.CodeReady;
        }

        // YENI
        private static bool CanStartExam(Exam exam, StudentExam studentExam, SystemSettingDto settings)
        {
            if (!CanEnterExam(exam, studentExam, settings))
                return false;

            return studentExam.Status == StudentExamStatus.ReadyToStart
                || studentExam.Status == StudentExamStatus.InProgress
                || studentExam.Status == StudentExamStatus.Pending
                || studentExam.Status == StudentExamStatus.CodeReady;
        }

        // KOHNE QALIR
        private static string? GetVisibleAccessCode(
            Exam exam,
            StudentExam studentExam,
            ExamAccessCode? accessCode)
        {
            if (!ShouldExposeAccessCode(exam, studentExam, accessCode))
                return null;

            return accessCode?.AccessCode;
        }

        // KOHNE QALIR
        private static bool ShouldExposeAccessCode(
            Exam exam,
            StudentExam studentExam,
            ExamAccessCode? accessCode)
        {
            if (accessCode is null)
                return false;

            if (string.IsNullOrWhiteSpace(accessCode.AccessCode))
                return false;

            if (!exam.IsPublished)
                return false;

            if (exam.Status == ExamStatus.Cancelled)
                return false;

            if (studentExam.IsCompleted)
                return false;

            if (studentExam.Status == StudentExamStatus.Missed)
                return false;

            var now = GetNow();

            // Kod yalnız imtahana 5 dəqiqə qalmışdan görünür
            if (now < exam.StartTime.AddMinutes(-5))
                return false;

            // Exam bitibsə kod göstərilmir
            if (now > exam.EndTime)
                return false;

            // Kodun expire vaxtı keçibsə göstərilmir
            if (accessCode.ExpireAt <= now)
                return false;

            return true;
        }

        // YENI
        private static bool ShouldExposeAccessCode(
            Exam exam,
            StudentExam studentExam,
            ExamAccessCode? accessCode,
            SystemSettingDto settings)
        {
            if (accessCode is null)
                return false;

            if (string.IsNullOrWhiteSpace(accessCode.AccessCode))
                return false;

            if (!exam.IsPublished)
                return false;

            if (exam.Status == ExamStatus.Cancelled)
                return false;

            if (studentExam.IsCompleted)
                return false;

            if (studentExam.Status == StudentExamStatus.Missed)
                return false;

            var now = GetNow();
            var activationMinutes = Math.Max(0, settings.AccessCodeActivationMinutes);
            var lateToleranceMinutes = Math.Max(0, settings.LateEntryToleranceMinutes);

            if (now < exam.StartTime.AddMinutes(-activationMinutes))
                return false;

            if (now > exam.EndTime.AddMinutes(lateToleranceMinutes))
                return false;

            if (accessCode.ExpireAt <= now)
                return false;

            return true;
        }

        public async Task<bool> VerifyAccessCodeAsync(
            VerifyStudentExamAccessCodeRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByStudentAndExamAsync(student.Id, request.ExamId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam qeydi tapılmadı.");

            if (studentExam.IsCompleted)
                throw new Exception("Bu imtahan artıq tamamlanıb.");

            var exam = await _unitOfWork.Exams.GetByIdAsync(request.ExamId, cancellationToken);
            if (exam is null)
                throw new Exception("İmtahan tapılmadı.");

            if (!exam.IsPublished || exam.Status == ExamStatus.Cancelled)
                throw new Exception("İmtahana giriş mümkün deyil.");

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            var now = GetNow();

            // YENI
            if (now < exam.StartTime.AddMinutes(-settings.AccessCodeActivationMinutes))
                throw new Exception("Giriş kodu hələ aktiv deyil.");

            // YENI
            if (now > exam.EndTime.AddMinutes(settings.LateEntryToleranceMinutes))
                throw new Exception("İmtahana giriş vaxtı bitib.");

            var accessCode = await _unitOfWork.ExamAccessCodes
                .GetByExamAndStudentAsync(request.ExamId, student.Id, cancellationToken);

            if (accessCode is null)
                throw new Exception("Bu tələbə üçün giriş kodu tapılmadı.");

            if (!string.Equals(accessCode.AccessCode?.Trim(), request.AccessCode?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("Giriş kodu yanlışdır.");

            if (accessCode.IsUsed && studentExam.Status != StudentExamStatus.InProgress)
                throw new Exception("Bu giriş kodu artıq istifadə olunub.");

            if (accessCode.ExpireAt <= now)
                throw new Exception("Giriş kodunun vaxtı bitib.");

            studentExam.Status = StudentExamStatus.ReadyToStart;
            studentExam.LastActivityAt = now;

            _unitOfWork.StudentExams.Update(studentExam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<StudentExamSessionDto> StartExamAsync(
            StartStudentExamRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByStudentAndExamWithFullDetailsAsync(student.Id, request.ExamId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam qeydi tapılmadı.");

            var exam = studentExam.Exam;

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            var now = GetNow();

            if (studentExam.IsCompleted)
                throw new Exception("Bu imtahan artıq tamamlanıb.");

            if (!exam.IsPublished || exam.Status == ExamStatus.Cancelled)
                throw new Exception("İmtahana giriş mümkün deyil.");

            if (!request.AcceptRules)
                throw new Exception("İmtahan qaydaları təsdiqlənməlidir.");

            if (studentExam.Status != StudentExamStatus.ReadyToStart &&
                studentExam.Status != StudentExamStatus.InProgress)
                throw new Exception("İmtahanı başlatmaq üçün əvvəlcə kod təsdiqlənməlidir.");

            if (now < exam.StartTime)
                throw new Exception("İmtahan hələ başlamayıb.");

            // YENI
            if (now > exam.EndTime.AddMinutes(settings.LateEntryToleranceMinutes))
                throw new Exception("İmtahana giriş vaxtı bitib.");

            var accessCode = await _unitOfWork.ExamAccessCodes
                .GetByExamAndStudentAsync(request.ExamId, student.Id, cancellationToken);

            if (accessCode is null)
                throw new Exception("Access code tapılmadı.");

            if (accessCode.ExpireAt <= now && studentExam.Status != StudentExamStatus.InProgress)
                throw new Exception("Giriş kodunun vaxtı bitib.");

            if (!accessCode.IsUsed)
            {
                accessCode.IsUsed = true;
                accessCode.UsedAt = now;
                _unitOfWork.ExamAccessCodes.Update(accessCode);
            }

            if (studentExam.Status != StudentExamStatus.InProgress)
            {
                studentExam.StartTime = now;
                studentExam.Status = StudentExamStatus.InProgress;
            }

            var deadline = GetStudentDeadline(studentExam);

            if (now > deadline)
                throw new Exception("İmtahanın vaxtı bitib.");

            studentExam.LastActivityAt = now;

            _unitOfWork.StudentExams.Update(studentExam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StudentExamSessionDto
            {
                StudentExamId = studentExam.Id,
                ExamId = exam.Id,
                ExamTitle = exam.Title,
                SubjectName = exam.Subject?.Name ?? string.Empty,
                StartTime = studentExam.StartTime,
                EndTime = studentExam.EndTime,
                IsCompleted = studentExam.IsCompleted,
                Score = studentExam.Score,
                IsReviewed = studentExam.IsReviewed,
                SubmittedAt = studentExam.SubmittedAt,
                DurationMinutes = exam.DurationMinutes,
                Instructions = exam.Instructions ?? exam.Description ?? string.Empty,
                Status = studentExam.Status.ToString(),
                TotalScore = exam.TotalScore ?? exam.Questions.Sum(q => q.Points),
                WarningCount = studentExam.WarningCount,
                TabSwitchCount = studentExam.TabSwitchCount,
                FullScreenExitCount = studentExam.FullScreenExitCount,
                Questions = exam.Questions
                    .OrderBy(q => q.OrderNumber)
                    .Select(q => new StudentExamQuestionDto
                    {
                        Id = q.Id,
                        OrderNumber = q.OrderNumber,
                        QuestionText = q.QuestionText,
                        Type = q.QuestionType.ToString(),
                        Points = q.Points,
                        Description = q.Description,
                        Options = q.Options
                            .OrderBy(o => o.OrderNumber)
                            .Select(o => new StudentExamQuestionOptionDto
                            {
                                Id = o.Id,
                                OptionText = o.OptionText,
                                OptionKey = o.OptionKey,
                                OrderNumber = o.OrderNumber
                            })
                            .ToList(),
                        ExistingAnswer = MapExistingAnswer(studentExam, q.Id)
                    })
                    .ToList()
            };
        }

        public async Task<StudentAnswerDto> SaveAnswerAsync(
            SaveStudentAnswerRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByIdAndStudentWithFullDetailsAsync(request.StudentExamId, student.Id, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı.");

            if (studentExam.IsCompleted)
                throw new Exception("Tamamlanmış imtahana cavab əlavə etmək olmaz.");

            if (studentExam.Status != StudentExamStatus.InProgress)
                throw new Exception("İmtahan aktiv deyil.");

            var exam = studentExam.Exam;
            var now = GetNow();

            await EnsureExamIsStillAvailableOrAutoSubmitAsync(studentExam, cancellationToken);

            var question = exam.Questions.FirstOrDefault(x => x.Id == request.ExamQuestionId);
            if (question is null)
                throw new Exception("Sual tapılmadı.");

            var existingAnswer = await _unitOfWork.StudentAnswers
                .GetByStudentExamAndQuestionAsync(request.StudentExamId, request.ExamQuestionId, cancellationToken);

            if (existingAnswer is null)
            {
                existingAnswer = new StudentAnswer
                {
                    StudentExamId = request.StudentExamId,
                    ExamQuestionId = request.ExamQuestionId,
                    SelectedOptionId = null,
                    AnswerText = null,
                    PointsAwarded = 0,
                    IsReviewed = false,
                    IsCorrect = null,
                    TeacherFeedback = null,
                    LastSavedAt = now
                };

                await _unitOfWork.StudentAnswers.AddAsync(existingAnswer, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            existingAnswer.SelectedOptionId = null;
            existingAnswer.AnswerText = null;
            existingAnswer.IsCorrect = null;
            existingAnswer.LastSavedAt = now;

            if (question.QuestionType == QuestionType.OpenText)
            {
                existingAnswer.AnswerText = request.AnswerText?.Trim();
            }
            else if (question.QuestionType == QuestionType.SingleChoice)
            {
                if (!request.SelectedOptionId.HasValue)
                    throw new Exception("Single choice sual üçün seçim edilməlidir.");

                var isValidOption = question.Options.Any(o => o.Id == request.SelectedOptionId.Value);
                if (!isValidOption)
                    throw new Exception("Seçilən cavab variantı suala aid deyil.");

                existingAnswer.SelectedOptionId = request.SelectedOptionId.Value;
            }
            else if (question.QuestionType == QuestionType.MultipleChoice)
            {
                var selectedIds = request.SelectedOptionIds?.Distinct().ToList() ?? new List<int>();

                if (selectedIds.Any())
                {
                    var validOptionIds = question.Options.Select(o => o.Id).ToHashSet();
                    if (selectedIds.Any(id => !validOptionIds.Contains(id)))
                        throw new Exception("Seçilən cavab variantlarından bəziləri suala aid deyil.");
                }

                var oldSelectedOptions = await _unitOfWork.StudentAnswerOptions
                    .GetByStudentAnswerIdAsync(existingAnswer.Id, cancellationToken);

                if (oldSelectedOptions.Any())
                {
                    _unitOfWork.StudentAnswerOptions.RemoveRange(oldSelectedOptions);
                }

                if (selectedIds.Any())
                {
                    var newSelectedOptions = selectedIds.Select(optionId => new StudentAnswerOption
                    {
                        StudentAnswerId = existingAnswer.Id,
                        ExamOptionId = optionId
                    }).ToList();

                    await _unitOfWork.StudentAnswerOptions.AddRangeAsync(newSelectedOptions, cancellationToken);
                }
            }

            _unitOfWork.StudentAnswers.Update(existingAnswer);

            studentExam.LastActivityAt = now;
            _unitOfWork.StudentExams.Update(studentExam);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var selectedOptionIds = await _unitOfWork.StudentAnswerOptions
                .GetByStudentAnswerIdAsync(existingAnswer.Id, cancellationToken);

            return new StudentAnswerDto
            {
                StudentAnswerId = existingAnswer.Id,
                StudentExamId = existingAnswer.StudentExamId,
                ExamQuestionId = existingAnswer.ExamQuestionId,
                SelectedOptionId = existingAnswer.SelectedOptionId,
                AnswerText = existingAnswer.AnswerText,
                PointsAwarded = existingAnswer.PointsAwarded,
                IsReviewed = existingAnswer.IsReviewed,
                IsCorrect = existingAnswer.IsCorrect,
                TeacherFeedback = existingAnswer.TeacherFeedback,
                SelectedOptionIds = selectedOptionIds.Select(x => x.ExamOptionId).ToList()
            };
        }

        public async Task<StudentExamSubmitResultDto> SubmitExamAsync(
            SubmitStudentExamRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByIdAndStudentWithFullDetailsAsync(request.StudentExamId, student.Id, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı.");

            if (studentExam.IsCompleted)
                throw new Exception("Bu imtahan artıq submit olunub.");

            if (studentExam.Status != StudentExamStatus.InProgress)
                throw new Exception("Yalnız aktiv imtahan təhvil verilə bilər.");

            var exam = studentExam.Exam;
            var now = GetNow();

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            // YENI
            if (!request.ForceAutoSubmit && !settings.AllowEarlySubmit)
            {
                var deadline = GetStudentDeadline(studentExam);

                if (now < deadline)
                    throw new Exception("Erkən submit ayarlarda deaktiv edilib.");
            }

            var answers = await _unitOfWork.StudentAnswers
                .GetByStudentExamIdWithDetailsAsync(studentExam.Id, cancellationToken);

            decimal autoGradedScore = 0;
            decimal manualGradedScore = 0;
            bool requiresManualReview = false;

            foreach (var question in exam.Questions)
            {
                var answer = answers.FirstOrDefault(x => x.ExamQuestionId == question.Id);

                if (question.QuestionType == QuestionType.OpenText)
                {
                    requiresManualReview = true;

                    if (answer is not null)
                    {
                        manualGradedScore += answer.PointsAwarded;
                    }

                    continue;
                }

                if (answer is null)
                    continue;

                if (question.QuestionType == QuestionType.SingleChoice)
                {
                    var correctOption = question.Options.FirstOrDefault(x => x.IsCorrect);

                    if (correctOption is not null && answer.SelectedOptionId == correctOption.Id)
                    {
                        answer.PointsAwarded = question.Points;
                        answer.IsCorrect = true;
                    }
                    else
                    {
                        answer.PointsAwarded = 0;
                        answer.IsCorrect = false;
                    }
                }
                else if (question.QuestionType == QuestionType.MultipleChoice)
                {
                    var selectedIds = answer.SelectedOptions
                        .Select(x => x.ExamOptionId)
                        .OrderBy(x => x)
                        .ToList();

                    var correctIds = question.Options
                        .Where(x => x.IsCorrect)
                        .Select(x => x.Id)
                        .OrderBy(x => x)
                        .ToList();

                    var isCorrect = selectedIds.SequenceEqual(correctIds);

                    answer.PointsAwarded = isCorrect ? question.Points : 0;
                    answer.IsCorrect = isCorrect;
                }

                _unitOfWork.StudentAnswers.Update(answer);
                autoGradedScore += answer.PointsAwarded;
            }

            foreach (var openAnswer in answers.Where(x => x.ExamQuestion.QuestionType == QuestionType.OpenText))
            {
                manualGradedScore += openAnswer.PointsAwarded;
            }

            studentExam.AutoGradedScore = autoGradedScore;
            studentExam.ManualGradedScore = manualGradedScore;
            studentExam.Score = autoGradedScore + manualGradedScore;
            studentExam.IsCompleted = true;
            studentExam.SubmittedAt = now;
            studentExam.EndTime = now;
            studentExam.LastActivityAt = now;
            studentExam.IsAutoSubmitted = request.ForceAutoSubmit;
            studentExam.Status = request.ForceAutoSubmit
                ? StudentExamStatus.AutoSubmitted
                : StudentExamStatus.Submitted;

            _unitOfWork.StudentExams.Update(studentExam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // YENI
            var hasOpenQuestions = HasOpenQuestions(exam);

            // YENI
            var isResultAutoPublished =
                settings.AutoPublishResults && !hasOpenQuestions;

            // YENI
            var canShowScoreImmediately =
                settings.ShowScoreImmediately && !hasOpenQuestions;

            // YENI
            var canShowCorrectAnswers =
                settings.ShowCorrectAnswersAfterCompletion && !hasOpenQuestions;

            return new StudentExamSubmitResultDto
            {
                StudentExamId = studentExam.Id,
                ExamId = studentExam.ExamId,
                StartTime = studentExam.StartTime,
                EndTime = studentExam.EndTime ?? now,
                IsCompleted = studentExam.IsCompleted,

                // Sistem daxilində tam score saxlanır
                Score = studentExam.Score,

                Message = request.ForceAutoSubmit
                    ? "İmtahan sistem tərəfindən avtomatik təhvil verildi."
                    : "İmtahan uğurla təhvil verildi.",

                AutoGradedScore = autoGradedScore,
                ManualGradedScore = manualGradedScore,
                RequiresManualReview = requiresManualReview,

                // YENI
                HasOpenQuestions = hasOpenQuestions,
                IsResultAutoPublished = isResultAutoPublished,
                CanShowScoreImmediately = canShowScoreImmediately,
                CanShowCorrectAnswers = canShowCorrectAnswers,

                // Frontend score göstərmək üçün əsas buna baxsın
                PublishedScore = canShowScoreImmediately ? studentExam.Score : null
            };
        }

        public async Task<StudentExamReviewDto> GetMyExamReviewAsync(
            int studentExamId,
            CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams.GetByIdWithAnswersAsync(studentExamId, cancellationToken);

            if (studentExam == null || studentExam.StudentId != student.Id)
                throw new KeyNotFoundException("Student exam review tapılmadı.");

            if (!studentExam.IsCompleted)
                throw new InvalidOperationException("Tamamlanmamış imtahan üçün review göstərilə bilməz.");

            if (studentExam.Exam == null)
                throw new InvalidOperationException("İmtahan məlumatı tapılmadı.");

            // YENI
            var hasOpenQuestions = HasOpenQuestions(studentExam.Exam);

            // YENI
            if (!settings.ShowCorrectAnswersAfterCompletion || hasOpenQuestions)
                throw new InvalidOperationException("Bu imtahan üçün düzgün cavablar hazırda göstərilmir.");

            var answers = await _unitOfWork.StudentAnswers
                .GetByStudentExamIdWithDetailsAsync(studentExamId, cancellationToken);

            return new StudentExamReviewDto
            {
                StudentExamId = studentExam.Id,
                ExamId = studentExam.ExamId,
                ExamTitle = studentExam.Exam?.Title ?? string.Empty,
                SubjectName = studentExam.Exam?.Subject?.Name ?? string.Empty,
                TeacherName = studentExam.Exam?.Teacher?.User?.FullName
                    ?? studentExam.Exam?.Teacher?.FullName
                    ?? string.Empty,
                ExamDate = studentExam.StartTime,
                Score = studentExam.Score,
                Questions = answers
                    .OrderBy(x => x.ExamQuestion.OrderNumber)
                    .Select(x => new StudentExamReviewQuestionDto
                    {
                        Id = x.ExamQuestionId,
                        ExamId = studentExam.ExamId,
                        QuestionNo = x.ExamQuestion.OrderNumber,
                        Type = MapQuestionType(x.ExamQuestion.QuestionType),
                        QuestionText = x.ExamQuestion.QuestionText,
                        Options = x.ExamQuestion.Options
                            .OrderBy(o => o.OrderNumber)
                            .Select(o => new StudentExamReviewOptionDto
                            {
                                Id = o.Id,
                                Text = o.OptionText,
                                IsCorrect = o.IsCorrect
                            })
                            .ToList(),
                        CorrectAnswerText = BuildCorrectAnswerText(x),
                        StudentAnswerText = BuildStudentAnswerText(x),
                        AwardedScore = x.PointsAwarded,
                        MaxScore = x.ExamQuestion.Points,
                        TeacherFeedback = x.TeacherFeedback ?? string.Empty
                    })
                    .ToList()
            };
        }

        public async Task LogSecurityEventAsync(
            LogExamSecurityEventRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var studentExam = await _unitOfWork.StudentExams
                .GetByIdAndStudentAsync(request.StudentExamId, student.Id, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı.");

            if (studentExam.IsCompleted)
                return;

            if (!Enum.TryParse<ExamSecurityEventType>(request.EventType, true, out var eventType))
                throw new Exception("Təhlükəsizlik event tipi yanlışdır.");

            var now = GetNow();

            var log = new ExamSecurityLog
            {
                StudentExamId = studentExam.Id,
                EventType = eventType,
                Description = request.Description,
                OccurredAt = now
            };

            await _unitOfWork.ExamSecurityLogs.AddAsync(log, cancellationToken);

            switch (eventType)
            {
                case ExamSecurityEventType.TabSwitch:
                case ExamSecurityEventType.Blur:
                    studentExam.TabSwitchCount++;
                    studentExam.WarningCount++;
                    break;

                case ExamSecurityEventType.FullScreenExit:
                    studentExam.FullScreenExitCount++;
                    studentExam.WarningCount++;
                    break;

                case ExamSecurityEventType.CopyAttempt:
                case ExamSecurityEventType.PasteAttempt:
                case ExamSecurityEventType.RightClickAttempt:
                    studentExam.WarningCount++;
                    break;
            }

            studentExam.LastActivityAt = now;
            _unitOfWork.StudentExams.Update(studentExam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            const int maxWarnings = 3;

            if (studentExam.WarningCount >= maxWarnings && !studentExam.IsCompleted)
            {
                await ForceAutoSubmitInternalAsync(studentExam.Id, cancellationToken);
            }
        }

        private async Task<Student> GetCurrentStudentAsync(CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser is null)
                throw new Exception("Cari istifadəçi tapılmadı.");

            var student = await _unitOfWork.Students.GetByUserIdAsync(currentUser.UserId, cancellationToken);

            if (student is null)
                throw new Exception("Student tapılmadı.");

            return student;
        }

        private StudentAnswerDto? MapExistingAnswer(StudentExam studentExam, int questionId)
        {
            var answer = studentExam.Answers.FirstOrDefault(x => x.ExamQuestionId == questionId);
            if (answer is null)
                return null;

            return new StudentAnswerDto
            {
                StudentAnswerId = answer.Id,
                StudentExamId = answer.StudentExamId,
                ExamQuestionId = answer.ExamQuestionId,
                SelectedOptionId = answer.SelectedOptionId,
                AnswerText = answer.AnswerText,
                PointsAwarded = answer.PointsAwarded,
                IsReviewed = answer.IsReviewed,
                IsCorrect = answer.IsCorrect,
                TeacherFeedback = answer.TeacherFeedback,
                SelectedOptionIds = answer.SelectedOptions
                    .Select(x => x.ExamOptionId)
                    .ToList()
            };
        }

        private DateTime GetStudentDeadline(StudentExam studentExam)
        {
            var exam = studentExam.Exam;

            var durationDeadline = studentExam.StartTime.AddMinutes(exam.DurationMinutes);

            return durationDeadline <= exam.EndTime
                ? durationDeadline
                : exam.EndTime;
        }

        private async Task EnsureExamIsStillAvailableOrAutoSubmitAsync(
            StudentExam studentExam,
            CancellationToken cancellationToken)
        {
            var now = GetNow();
            var deadline = GetStudentDeadline(studentExam);

            if (now <= deadline)
                return;

            if (!studentExam.IsCompleted)
            {
                await ForceAutoSubmitInternalAsync(studentExam.Id, cancellationToken);
            }

            throw new Exception("İmtahanın vaxtı bitdiyi üçün avtomatik təhvil verildi.");
        }

        // YENI
        private static bool HasOpenQuestions(Exam exam)
        {
            if (exam.OpenQuestionCount > 0)
                return true;

            return exam.Questions.Any(q => q.QuestionType == QuestionType.OpenText);
        }

        // YENI
        private static string MapQuestionType(QuestionType type)
        {
            return type switch
            {
                QuestionType.SingleChoice => "MultipleChoice",
                QuestionType.MultipleChoice => "MultipleChoice",
                QuestionType.OpenText => "OpenEnded",
                _ => type.ToString()
            };
        }

        // YENI
        private static string BuildCorrectAnswerText(StudentAnswer answer)
        {
            if (answer.ExamQuestion.QuestionType == QuestionType.OpenText)
                return "Açıq cavab";

            var correctOptions = answer.ExamQuestion.Options
                .Where(x => x.IsCorrect)
                .OrderBy(x => x.OrderNumber)
                .Select(x => x.OptionText)
                .ToList();

            return string.Join(", ", correctOptions);
        }

        // YENI
        private static string BuildStudentAnswerText(StudentAnswer answer)
        {
            if (answer.ExamQuestion.QuestionType == QuestionType.OpenText)
                return answer.AnswerText ?? string.Empty;

            if (answer.SelectedOptions?.Any() == true)
            {
                return string.Join(", ",
                    answer.SelectedOptions
                        .OrderBy(x => x.ExamOption?.OrderNumber ?? 0)
                        .Select(x => x.ExamOption?.OptionText ?? string.Empty)
                        .Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            if (answer.SelectedOptionId.HasValue)
            {
                var selectedOption = answer.ExamQuestion.Options
                    .FirstOrDefault(x => x.Id == answer.SelectedOptionId.Value);

                return selectedOption?.OptionText ?? string.Empty;
            }

            return string.Empty;
        }

        private async Task ForceAutoSubmitInternalAsync(int studentExamId, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.StudentExams.GetByIdAsync(studentExamId, cancellationToken);

            if (existing is null || existing.IsCompleted)
                return;

            var request = new SubmitStudentExamRequestDto
            {
                StudentExamId = studentExamId,
                ForceAutoSubmit = true
            };

            await SubmitExamAsync(request, cancellationToken);

            var autoLog = new ExamSecurityLog
            {
                StudentExamId = studentExamId,
                EventType = ExamSecurityEventType.AutoSubmitTriggered,
                Description = "Warning limiti və ya vaxt limiti keçildiyi üçün imtahan avtomatik təhvil verildi.",
                OccurredAt = GetNow()
            };

            await _unitOfWork.ExamSecurityLogs.AddAsync(autoLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
