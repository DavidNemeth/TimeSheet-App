using TimeSheet.Web.DTOs;

namespace TimeSheet.Web.Services.Interfaces
{
    public interface ITimesheetService
    {
        Task<List<TimesheetEntryDto>> GetTimesheetEntriesAsync(DateTime startDate, DateTime endDate, string userId = null, bool forRole = false);
        Task<List<TimesheetEntryDto>> GetArchivedEntriesAsync();
        Task<List<TimesheetEntryDto>> GetAllTimesheetEntriesAsync(DateTime startDate, DateTime endDate);
        Task<TimesheetEntryDto> GetTimesheetEntryAsync(int id);
        Task<TimesheetEntryDto> CreateTimesheetEntryAsync(TimesheetEntryDto timesheetEntryDto);
        Task<TimesheetEntryDto> UpdateTimesheetEntryAsync(TimesheetEntryDto timesheetEntryDto);
        Task DeleteTimesheetEntryAsync(int id);
        Task ArchiveEntryAsync(int id, string archivedBy);
        Task UnArchiveEntryAsync(int id, string unArchivedBy);
    }
}
