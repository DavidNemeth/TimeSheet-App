using SharedLib.Enums;
using SharedLib.Models;

namespace TimeSheet.Web.Helpers;

public static class TimesheetNotificationHelper
{
    public static NotificationDetails GenerateNotificationForRequest(string recordId)
    {
        return new NotificationDetails
        {
            Application = ApplicationEnum.TimeSheet,
            NotificationType = NotificationType.TimeSheetRequest,
            RecordId = recordId,
            CreatedAt = DateTime.UtcNow,
            Title = "Overtime/dirtbonus waiting for approval.",
            Description = "There is overtime and/or dirtbonus time sheet entry request waiting for approval.",
        };
    }

    public static NotificationDetails GenerateNotificationForApproval(string recordId)
    {
        return new NotificationDetails
        {
            Application = ApplicationEnum.TimeSheet,
            NotificationType = NotificationType.TimeSheetApproved,
            RecordId = recordId,
            CreatedAt = DateTime.UtcNow,
            Title = "Overtime/dirtbonus approved.",
            Description = "Your overtime and/or dirtbonus request has been approved.",
        };
    }

    public static NotificationDetails GenerateNotificationForRejection(string recordId)
    {
        return new NotificationDetails
        {
            Application = ApplicationEnum.TimeSheet,
            NotificationType = NotificationType.TimeSheetRejected,
            RecordId = recordId,
            CreatedAt = DateTime.UtcNow,
            Title = "Overtime/dirtbonus rejected.",
            Description = "Your overtime and/or dirtbonus request has been rejected.",
        };
    }
}