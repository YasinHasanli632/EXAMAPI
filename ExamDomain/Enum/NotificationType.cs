using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum NotificationType
    {
        NewExam = 1, // Yeni imtahan təyin edildiyi barədə bildiriş

        ExamStarted = 2, // İmtahan başladığı barədə bildiriş

        ExamFinished = 3, // İmtahanın bitdiyi barədə bildiriş

        ResultPublished = 4, // Nəticələrin elan edildiyi barədə bildiriş

        OpenQuestionReview = 5 // Müəllim üçün açıq sualların yoxlanmalı olduğu barədə bildiriş
    }
}
