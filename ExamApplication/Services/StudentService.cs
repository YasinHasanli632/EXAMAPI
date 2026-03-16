using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamApplication.Services
{
    // Student tərəfdə profil, sinif, imtahan və nəticə əməliyyatlarını idarə edir.
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // UserId-yə görə student profilini qaytarır.
        public async Task<StudentProfileDto> GetStudentProfileAsync(int userId, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(userId, cancellationToken);

            if (student is null)
                throw new Exception("Student tapılmadı");

            var studentWithDetails = await _unitOfWork.Students.GetByIdWithDetailsAsync(student.Id, cancellationToken);

            if (studentWithDetails is null)
                throw new Exception("Student məlumatları tapılmadı");

            return new StudentProfileDto
            {
                StudentId = studentWithDetails.Id,
                UserId = studentWithDetails.UserId,
                FullName = studentWithDetails.FullName,
                DateOfBirth = studentWithDetails.DateOfBirth,
                StudentNumber = studentWithDetails.StudentNumber,
                Username = studentWithDetails.User.Username,
                Email = studentWithDetails.User.Email
            };
        }

        // UserId-yə görə student-in qoşulduğu sinifləri qaytarır.
        public async Task<List<StudentClassDto>> GetStudentClassesAsync(int userId, CancellationToken cancellationToken = default)
        {
            var student = await GetStudentByUserIdAsync(userId, cancellationToken);

            var studentClasses = await _unitOfWork.StudentClasses.GetByStudentIdAsync(student.Id, cancellationToken);

            return studentClasses.Select(x => new StudentClassDto
            {
                StudentClassId = x.Id,
                ClassRoomId = x.ClassRoomId,
                ClassName = x.ClassRoom.Name,
                Grade = x.ClassRoom.Grade
            }).ToList();
        }

        // Student üçün əlçatan imtahanları qaytarır.
        // Hazırkı struktura görə exam-class əlaqəsi görünmədiyi üçün burada aktiv exam-lər qaytarılır.
        public async Task<List<StudentAvailableExamDto>> GetAvailableExamsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var student = await GetStudentByUserIdAsync(userId, cancellationToken);

            var activeExams = await _unitOfWork.Exams.GetActiveExamsAsync(cancellationToken);
            var studentExams = await _unitOfWork.StudentExams.GetByStudentIdAsync(student.Id, cancellationToken);

            var result = activeExams
                .Select(exam =>
                {
                    var studentExam = studentExams.FirstOrDefault(x => x.ExamId == exam.Id);

                    return new StudentAvailableExamDto
                    {
                        ExamId = exam.Id,
                        Title = exam.Title,
                      
                        SubjectName = exam.Subject.Name,
                        StartDate = exam.StartTime,
                        EndDate = exam.EndTime,
                        DurationMinutes = exam.DurationMinutes,
                        HasStarted = studentExam is not null,
                        IsCompleted = studentExam?.IsCompleted ?? false,
                        StudentExamId = studentExam?.Id
                    };
                })
                .OrderBy(x => x.StartDate)
                .ToList();

            return result;
        }

        // Student üçün konkret exam detail məlumatını qaytarır.
        public async Task<StudentExamDetailDto> GetExamDetailAsync(int userId, int examId, CancellationToken cancellationToken = default)
        {
            var student = await GetStudentByUserIdAsync(userId, cancellationToken);

            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);

            if (exam is null)
                throw new Exception("İmtahan tapılmadı");

            var studentExam = await _unitOfWork.StudentExams.GetByStudentAndExamAsync(student.Id, examId, cancellationToken);

            var answers = new List<StudentAnswer>();

            if (studentExam is not null)
            {
                answers = await _unitOfWork.StudentAnswers.GetByStudentExamIdAsync(studentExam.Id, cancellationToken);
            }

            return new StudentExamDetailDto
            {
                ExamId = exam.Id,
                Title = exam.Title,
               
                SubjectName = exam.Subject.Name,
                StartDate = exam.StartTime,
                EndDate = exam.EndTime,
                DurationMinutes = exam.DurationMinutes,
                TotalQuestionCount = exam.Questions.Count,
                TotalPoints = exam.Questions.Sum(x => x.Points),
                HasStarted = studentExam is not null,
                IsCompleted = studentExam?.IsCompleted ?? false,
                StudentExamId = studentExam?.Id,
                Questions = exam.Questions
                    .OrderBy(x => x.Id)
                    .Select(question =>
                    {
                        var existingAnswer = answers.FirstOrDefault(a => a.ExamQuestionId == question.Id);

                        return new StudentExamQuestionDto
                        {
                            ExamQuestionId = question.Id,
                            QuestionText = question.QuestionText,
                            QuestionType = question.QuestionType,
                            Points = question.Points,
                            OrderNumber = question.Id,
                            SelectedOptionId = existingAnswer?.SelectedOptionId,
                            AnswerText = existingAnswer?.AnswerText,
                            Options = question.Options
                                .OrderBy(o => o.Id)
                                .Select(option => new StudentQuestionOptionDto
                                {
                                    OptionId = option.Id,
                                    OptionText = option.OptionText,
                                    OrderNumber = option.Id
                                })
                                .ToList()
                        };
                    })
                    .ToList()
            };
        }

        // Student üçün exam session başladır.
        public async Task<StudentExamSessionDto> StartExamAsync(StartStudentExamRequestDto request, CancellationToken cancellationToken = default)
        {
            var student = await GetStudentByUserIdAsync(request.UserId, cancellationToken);

            var exam = await _unitOfWork.Exams.GetByIdAsync(request.ExamId, cancellationToken);

            if (exam is null)
                throw new Exception("İmtahan tapılmadı");

            var existingStudentExam = await _unitOfWork.StudentExams.GetByStudentAndExamAsync(student.Id, request.ExamId, cancellationToken);

            if (existingStudentExam is not null)
            {
                return new StudentExamSessionDto
                {
                    StudentExamId = existingStudentExam.Id,
                    StudentId = existingStudentExam.StudentId,
                    ExamId = existingStudentExam.ExamId,
                    StartTime = existingStudentExam.StartTime,
                    EndTime = existingStudentExam.EndTime,
                    IsCompleted = existingStudentExam.IsCompleted,
                    Score = existingStudentExam.Score
                };
            }

            var studentExam = new StudentExam
            {
                StudentId = student.Id,
                ExamId = request.ExamId,
                StartTime = DateTime.UtcNow,
                EndTime = null,
                Score = 0,
                IsCompleted = false
            };

            await _unitOfWork.StudentExams.AddAsync(studentExam, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StudentExamSessionDto
            {
                StudentExamId = studentExam.Id,
                StudentId = studentExam.StudentId,
                ExamId = studentExam.ExamId,
                StartTime = studentExam.StartTime,
                EndTime = studentExam.EndTime,
                IsCompleted = studentExam.IsCompleted,
                Score = studentExam.Score
            };
        }

        // Student-in cavabını save edir və ya əvvəlki cavabı update edir.
        public async Task<StudentAnswerDto> SaveAnswerAsync(SaveStudentAnswerRequestDto request, CancellationToken cancellationToken = default)
        {
            var studentExam = await _unitOfWork.StudentExams.GetByIdAsync(request.StudentExamId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı");

            if (studentExam.IsCompleted)
                throw new Exception("Tamamlanmış imtahana cavab əlavə etmək olmaz");

            var question = await _unitOfWork.ExamQuestions.GetByIdWithOptionsAsync(request.ExamQuestionId, cancellationToken);

            if (question is null)
                throw new Exception("Sual tapılmadı");

            var existingAnswer = await _unitOfWork.StudentAnswers.GetByStudentExamAndQuestionAsync(
                request.StudentExamId,
                request.ExamQuestionId,
                cancellationToken);

            if (existingAnswer is null)
            {
                var newAnswer = new StudentAnswer
                {
                    StudentExamId = request.StudentExamId,
                    ExamQuestionId = request.ExamQuestionId,
                    SelectedOptionId = request.SelectedOptionId,
                    AnswerText = request.AnswerText,
                    PointsAwarded = 0
                };

                await _unitOfWork.StudentAnswers.AddAsync(newAnswer, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new StudentAnswerDto
                {
                    StudentAnswerId = newAnswer.Id,
                    StudentExamId = newAnswer.StudentExamId,
                    ExamQuestionId = newAnswer.ExamQuestionId,
                    SelectedOptionId = newAnswer.SelectedOptionId,
                    AnswerText = newAnswer.AnswerText,
                    PointsAwarded = newAnswer.PointsAwarded
                };
            }

            existingAnswer.SelectedOptionId = request.SelectedOptionId;
            existingAnswer.AnswerText = request.AnswerText;

            _unitOfWork.StudentAnswers.Update(existingAnswer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StudentAnswerDto
            {
                StudentAnswerId = existingAnswer.Id,
                StudentExamId = existingAnswer.StudentExamId,
                ExamQuestionId = existingAnswer.ExamQuestionId,
                SelectedOptionId = existingAnswer.SelectedOptionId,
                AnswerText = existingAnswer.AnswerText,
                PointsAwarded = existingAnswer.PointsAwarded
            };
        }

        // Verilən student exam sessiyasına aid bütün cavabları qaytarır.
        public async Task<List<StudentAnswerDto>> GetExamAnswersAsync(int studentExamId, CancellationToken cancellationToken = default)
        {
            var studentExam = await _unitOfWork.StudentExams.GetByIdAsync(studentExamId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı");

            var answers = await _unitOfWork.StudentAnswers.GetByStudentExamIdAsync(studentExamId, cancellationToken);

            return answers.Select(x => new StudentAnswerDto
            {
                StudentAnswerId = x.Id,
                StudentExamId = x.StudentExamId,
                ExamQuestionId = x.ExamQuestionId,
                SelectedOptionId = x.SelectedOptionId,
                AnswerText = x.AnswerText,
                PointsAwarded = x.PointsAwarded
            }).ToList();
        }

        // Student exam-i submit edir.
        public async Task<StudentExamSubmitResultDto> SubmitExamAsync(SubmitStudentExamRequestDto request, CancellationToken cancellationToken = default)
        {
            var studentExam = await _unitOfWork.StudentExams.GetByIdWithAnswersAsync(request.StudentExamId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı");

            if (studentExam.IsCompleted)
                throw new Exception("Bu imtahan artıq submit olunub");

            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(studentExam.ExamId, cancellationToken);

            if (exam is null)
                throw new Exception("İmtahan tapılmadı");

            var answers = await _unitOfWork.StudentAnswers.GetByStudentExamIdAsync(studentExam.Id, cancellationToken);

            decimal totalScore = 0;

            foreach (var answer in answers)
            {
                var question = exam.Questions.FirstOrDefault(x => x.Id == answer.ExamQuestionId);

                if (question is null)
                    continue;

                if (question.QuestionType == QuestionType.OpenText)
                {
                    totalScore += answer.PointsAwarded;
                    continue;
                }

                var correctOption = question.Options.FirstOrDefault(x => x.IsCorrect);

                if (correctOption is not null && answer.SelectedOptionId == correctOption.Id)
                {
                    answer.PointsAwarded = question.Points;
                }
                else
                {
                    answer.PointsAwarded = 0;
                }

                _unitOfWork.StudentAnswers.Update(answer);
                totalScore += answer.PointsAwarded;
            }

            studentExam.Score = totalScore;
            studentExam.IsCompleted = true;
            studentExam.EndTime = DateTime.UtcNow;

            _unitOfWork.StudentExams.Update(studentExam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StudentExamSubmitResultDto
            {
                StudentExamId = studentExam.Id,
                ExamId = studentExam.ExamId,
                StartTime = studentExam.StartTime,
                EndTime = studentExam.EndTime ?? DateTime.UtcNow,
                IsCompleted = studentExam.IsCompleted,
                Score = studentExam.Score,
                Message = "İmtahan uğurla tamamlandı"
            };
        }

        // Student-in exam history-sini qaytarır.
        public async Task<List<StudentExamHistoryDto>> GetExamHistoryAsync(int userId, CancellationToken cancellationToken = default)
        {
            var student = await GetStudentByUserIdAsync(userId, cancellationToken);

            var studentExams = await _unitOfWork.StudentExams.GetByStudentIdAsync(student.Id, cancellationToken);

            return studentExams.Select(x => new StudentExamHistoryDto
            {
                StudentExamId = x.Id,
                ExamId = x.ExamId,
                ExamTitle = x.Exam.Title,
                SubjectName = x.Exam.Subject.Name,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                IsCompleted = x.IsCompleted,
                Score = x.Score
            }).ToList();
        }

        // Student-in konkret exam nəticəsini qaytarır.
        public async Task<StudentExamResultDto> GetExamResultAsync(int userId, int studentExamId, CancellationToken cancellationToken = default)
        {
            var student = await GetStudentByUserIdAsync(userId, cancellationToken);

            var studentExam = await _unitOfWork.StudentExams.GetByIdWithAnswersAsync(studentExamId, cancellationToken);

            if (studentExam is null)
                throw new Exception("Student exam session tapılmadı");

            if (studentExam.StudentId != student.Id)
                throw new Exception("Bu nəticəyə baxmaq icazən yoxdur");

            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(studentExam.ExamId, cancellationToken);

            if (exam is null)
                throw new Exception("İmtahan tapılmadı");

            var answers = await _unitOfWork.StudentAnswers.GetByStudentExamIdAsync(studentExam.Id, cancellationToken);

            return new StudentExamResultDto
            {
                StudentExamId = studentExam.Id,
                ExamId = studentExam.ExamId,
                ExamTitle = exam.Title,
                SubjectName = exam.Subject.Name,
                StudentFullName = student.FullName,
                StudentNumber = student.StudentNumber,
                StartTime = studentExam.StartTime,
                EndTime = studentExam.EndTime,
                Score = studentExam.Score,
                IsCompleted = studentExam.IsCompleted,
                TotalQuestions = exam.Questions.Count,
                AnsweredQuestions = answers.Count,
                Answers = answers.Select(x => new StudentAnswerDto
                {
                    StudentAnswerId = x.Id,
                    StudentExamId = x.StudentExamId,
                    ExamQuestionId = x.ExamQuestionId,
                    SelectedOptionId = x.SelectedOptionId,
                    AnswerText = x.AnswerText,
                    PointsAwarded = x.PointsAwarded
                }).ToList()
            };
        }

        // UserId-yə görə student entity-sini qaytarır.
        private async Task<Student> GetStudentByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(userId, cancellationToken);

            if (student is null)
                throw new Exception("Student tapılmadı");

            return student;
        }
    }
}