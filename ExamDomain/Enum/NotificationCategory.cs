using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum NotificationCategory
    {
        // Exam
        ExamCreated = 1,
        ExamPublished = 2,
        ExamActivated = 3,
        ExamCompleted = 4,
        ExamCancelled = 5,
        ExamRescheduled = 6,
        ExamResultPublished = 7,
        ExamAccessCodeReady = 8,
        OpenQuestionReviewPending = 9,

        // Task
        TaskAssigned = 20,
        TaskDeadlineSoon = 21,
        TaskOverdue = 22,
        TaskSubmitted = 23,
        TaskReviewed = 24,
        TaskGradePublished = 25,

        // Attendance
        AttendanceMarked = 40,
        AttendanceAbsent = 41,
        AttendanceLate = 42,
        AttendanceChangeRequestCreated = 43,
        AttendanceChangeRequestApproved = 44,
        AttendanceChangeRequestRejected = 45,

        // User / Security
        PasswordChanged = 60,
        ProfileUpdated = 61,
        UserStatusChanged = 62,
        RoleChanged = 63,

        // Admin/System
        AnnouncementPublished = 80,
        SystemWarning = 81,
        SystemError = 82
    }
}
