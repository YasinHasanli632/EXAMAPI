using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum StudentExamStatus
    {
        Pending = 1,      // Hələ giriş etməyib
        CodeReady = 2,    // Access code yaradılıb
        ReadyToStart = 3, // Kod təsdiqlənib, start ekranındadır
        InProgress = 4,   // İmtahan başlayıb
        Submitted = 5,    // Student özü submit edib
        AutoSubmitted = 6,// Sistem submit edib
        Missed = 7,       // Vaxtı qaçırıb
        Reviewed = 8      // Müəllim yekun review edib
    }
}
