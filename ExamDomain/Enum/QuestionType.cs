using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum QuestionType
    {
        SingleChoice = 1, // Tək düzgün cavabı olan test sualı (A,B,C,D)

        MultipleChoice = 2, // Bir neçə düzgün cavabı olan test sualı

        OpenText = 3 // Açıq tip sual (tələbə mətn yazır)
    }
}
