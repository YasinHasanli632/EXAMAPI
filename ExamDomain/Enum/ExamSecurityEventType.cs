using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum ExamSecurityEventType
    {
        TabSwitch = 1,
        FullScreenExit = 2,
        CopyAttempt = 3,
        PasteAttempt = 4,
        RightClickAttempt = 5,
        Blur = 6,
        FocusReturn = 7,
        AutoSubmitTriggered = 8
    }
}
