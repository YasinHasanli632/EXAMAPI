using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IExamAccessCodeService
    {
        Task GenerateCodesForUpcomingExamsAsync(CancellationToken cancellationToken = default);

        // YENI
        Task EnsureStudentExamRecordsForPublishedExamAsync(int examId, CancellationToken cancellationToken = default);

        // YENI
        Task AutoCloseExpiredStudentExamsAsync(CancellationToken cancellationToken = default);
    }
}
