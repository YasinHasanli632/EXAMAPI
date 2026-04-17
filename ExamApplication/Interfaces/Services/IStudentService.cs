//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ExamApplication.Interfaces.Services
//{
//    // Student tərəfdə profil, sinif, imtahan və nəticə əməliyyatlarını idarə edir.
//    public interface IStudentService
//    {
//        // UserId-yə görə student profilini qaytarır.
//        Task<StudentProfileDto> GetStudentProfileAsync(int userId, CancellationToken cancellationToken = default);

//        // UserId-yə görə student-in qoşulduğu sinifləri qaytarır.
//        Task<List<StudentClassDto>> GetStudentClassesAsync(int userId, CancellationToken cancellationToken = default);

//        // Student üçün əlçatan imtahanları qaytarır.
//        Task<List<StudentAvailableExamDto>> GetAvailableExamsAsync(int userId, CancellationToken cancellationToken = default);

//        // Student üçün konkret exam detail məlumatını qaytarır.
//        Task<StudentExamDetailDto> GetExamDetailAsync(int userId, int examId, CancellationToken cancellationToken = default);

//        // Student üçün exam session başladır.
//        Task<StudentExamSessionDto> StartExamAsync(StartStudentExamRequestDto request, CancellationToken cancellationToken = default);

//        // Student-in cavabını save edir və ya əvvəlki cavabı update edir.
//        Task<StudentAnswerDto> SaveAnswerAsync(SaveStudentAnswerRequestDto request, CancellationToken cancellationToken = default);

//        // Verilən student exam sessiyasına aid bütün cavabları qaytarır.
//        Task<List<StudentAnswerDto>> GetExamAnswersAsync(int studentExamId, CancellationToken cancellationToken = default);

//        // Student exam-i submit edir.
//        Task<StudentExamSubmitResultDto> SubmitExamAsync(SubmitStudentExamRequestDto request, CancellationToken cancellationToken = default);

//        // Student-in exam history-sini qaytarır.
//        Task<List<StudentExamHistoryDto>> GetExamHistoryAsync(int userId, CancellationToken cancellationToken = default);

//        // Student-in konkret exam nəticəsini qaytarır.
//        Task<StudentExamResultDto> GetExamResultAsync(int userId, int studentExamId, CancellationToken cancellationToken = default);
//    }
//}
