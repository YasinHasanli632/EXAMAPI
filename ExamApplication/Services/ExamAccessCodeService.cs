using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamApplication.Services
{
    // YENI
    public class ExamAccessCodeService : IExamAccessCodeService
    {
        private readonly IUnitOfWork _unitOfWork;

        // YENI
        private readonly ISystemSettingService _systemSettingService;

        public ExamAccessCodeService(
            IUnitOfWork unitOfWork,
            // YENI
            ISystemSettingService systemSettingService)
        {
            _unitOfWork = unitOfWork;

            // YENI
            _systemSettingService = systemSettingService;
        }

        // YENI
        public async Task EnsureStudentExamRecordsForPublishedExamAsync(int examId, CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);

            if (exam is null)
                throw new Exception("İmtahan tapılmadı.");

            if (!exam.IsPublished)
                return;

            if (!exam.ClassRoomId.HasValue)
                throw new Exception("İmtahan üçün sinif seçilməyib.");

            var students = await _unitOfWork.Students
                .GetStudentsByClassRoomIdAsync(exam.ClassRoomId.Value, cancellationToken);

            foreach (var student in students)
            {
                var exists = await _unitOfWork.StudentExams
                    .ExistsAsync(student.Id, exam.Id, cancellationToken);

                if (exists)
                    continue;

                await _unitOfWork.StudentExams.AddAsync(new StudentExam
                {
                    StudentId = student.Id,
                    ExamId = exam.Id,

                    // YENI
                    StartTime = exam.StartTime,
                    EndTime = null,
                    Score = 0,
                    IsCompleted = false,
                    SubmittedAt = null,
                    IsReviewed = false,
                    ReviewedAt = null,
                    ReviewedByTeacherId = null,
                    AutoGradedScore = 0,
                    ManualGradedScore = 0,
                    Status = StudentExamStatus.Pending,
                    IsLocked = false,
                    WarningCount = 0,
                    TabSwitchCount = 0,
                    FullScreenExitCount = 0,
                    LastActivityAt = null,
                    IsAutoSubmitted = false
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        public async Task GenerateCodesForUpcomingExamsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            // YENI
            var settings = await _systemSettingService.GetAsync(cancellationToken);

            // YENI
            var activationMinutes = settings.AccessCodeActivationMinutes >= 0
                ? settings.AccessCodeActivationMinutes
                : 5;

            // YENI
            var lateToleranceMinutes = settings.LateEntryToleranceMinutes >= 0
                ? settings.LateEntryToleranceMinutes
                : 10;

            // YENI
            var until = now.AddMinutes(Math.Max(1, activationMinutes));

            var upcomingExams = await _unitOfWork.Exams
                .GetPublishedUpcomingExamsAsync(now, until, cancellationToken);

            foreach (var exam in upcomingExams)
            {
                await EnsureStudentExamRecordsForPublishedExamAsync(exam.Id, cancellationToken);

                var studentExams = await _unitOfWork.StudentExams
                    .GetPendingByExamIdAsync(exam.Id, cancellationToken);

                foreach (var studentExam in studentExams)
                {
                    if (studentExam.IsCompleted)
                        continue;

                    var existingCode = await _unitOfWork.ExamAccessCodes
                        .GetByExamAndStudentAsync(exam.Id, studentExam.StudentId, cancellationToken);

                    if (existingCode is null)
                    {
                        var accessCode = new ExamAccessCode
                        {
                            ExamId = exam.Id,
                            StudentId = studentExam.StudentId,
                            AccessCode = await GenerateUniqueCodeAsync(cancellationToken),
                            IsUsed = false,

                            // YENI
                            ExpireAt = exam.StartTime.AddMinutes(lateToleranceMinutes),

                            // YENI
                            GeneratedAt = now,
                            UsedAt = null
                        };

                        await _unitOfWork.ExamAccessCodes.AddAsync(accessCode, cancellationToken);
                    }
                    else if (existingCode.ExpireAt <= now && !existingCode.IsUsed)
                    {
                        // YENI
                        existingCode.AccessCode = await GenerateUniqueCodeAsync(cancellationToken);
                        existingCode.GeneratedAt = now;

                        // YENI
                        existingCode.ExpireAt = exam.StartTime.AddMinutes(lateToleranceMinutes);

                        existingCode.UsedAt = null;

                        _unitOfWork.ExamAccessCodes.Update(existingCode);
                    }

                    // YENI
                    if (studentExam.Status == StudentExamStatus.Pending)
                    {
                        studentExam.Status = StudentExamStatus.CodeReady;
                        _unitOfWork.StudentExams.Update(studentExam);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        public async Task AutoCloseExpiredStudentExamsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            // YENI
            var activeSessions = await _unitOfWork.StudentExams
                .GetNotCompletedActiveSessionsAsync(cancellationToken);

            foreach (var studentExam in activeSessions)
            {
                var exam = studentExam.Exam;
                var studentEndTime = studentExam.StartTime.AddMinutes(exam.DurationMinutes);

                if (now >= studentEndTime || now >= exam.EndTime)
                {
                    await AutoSubmitStudentExamInternalAsync(studentExam.Id, cancellationToken);
                }
            }

            // YENI
            var endedExams = await _unitOfWork.Exams
                .GetPublishedEndedExamsAsync(now, cancellationToken);

            foreach (var exam in endedExams)
            {
                var studentExams = await _unitOfWork.StudentExams
                    .GetByExamIdAsync(exam.Id, cancellationToken);

                foreach (var studentExam in studentExams)
                {
                    if (studentExam.IsCompleted)
                        continue;

                    // YENI
                    if (studentExam.Status == StudentExamStatus.Pending ||
                        studentExam.Status == StudentExamStatus.CodeReady ||
                        studentExam.Status == StudentExamStatus.ReadyToStart)
                    {
                        studentExam.Status = StudentExamStatus.Missed;
                        studentExam.IsCompleted = true;
                        studentExam.EndTime = exam.EndTime;
                        studentExam.SubmittedAt = null;
                        studentExam.Score = 0;
                        studentExam.IsAutoSubmitted = false;

                        _unitOfWork.StudentExams.Update(studentExam);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        private async Task AutoSubmitStudentExamInternalAsync(int studentExamId, CancellationToken cancellationToken)
        {
            var studentExam = await _unitOfWork.StudentExams
                .GetByIdWithFullDetailsAsync(studentExamId, cancellationToken);

            if (studentExam is null || studentExam.IsCompleted)
                return;

            var exam = studentExam.Exam;
            var answers = await _unitOfWork.StudentAnswers
                .GetByStudentExamIdWithDetailsAsync(studentExam.Id, cancellationToken);

            decimal autoScore = 0;
            decimal manualScore = 0;

            foreach (var question in exam.Questions)
            {
                var answer = answers.FirstOrDefault(x => x.ExamQuestionId == question.Id);

                if (question.QuestionType == QuestionType.OpenText)
                {
                    if (answer is not null)
                    {
                        manualScore += answer.PointsAwarded;
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
                autoScore += answer.PointsAwarded;
            }

            studentExam.AutoGradedScore = autoScore;
            studentExam.ManualGradedScore = manualScore;
            studentExam.Score = autoScore + manualScore;
            studentExam.IsCompleted = true;
            studentExam.IsAutoSubmitted = true;
            studentExam.SubmittedAt = DateTime.UtcNow;
            studentExam.EndTime = DateTime.UtcNow;
            studentExam.Status = StudentExamStatus.AutoSubmitted;
            studentExam.LastActivityAt = DateTime.UtcNow;

            _unitOfWork.StudentExams.Update(studentExam);

            // YENI
            await _unitOfWork.ExamSecurityLogs.AddAsync(new ExamSecurityLog
            {
                StudentExamId = studentExam.Id,
                EventType = ExamSecurityEventType.AutoSubmitTriggered,
                Description = "İmtahan vaxtı bitdiyi üçün avtomatik təhvil verildi.",
                OccurredAt = DateTime.UtcNow
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
        {
            string code;
            do
            {
                code = GenerateCode();
            }
            while (await _unitOfWork.ExamAccessCodes.GetByCodeAsync(code, cancellationToken) is not null);

            return code;
        }

        // YENI
        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}