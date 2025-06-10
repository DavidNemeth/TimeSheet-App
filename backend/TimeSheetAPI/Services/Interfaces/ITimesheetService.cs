using TimeSheet.Api.DTOs;

namespace TimeSheet.Api.Services.Interfaces
{
    public interface ITimesheetService
    {
        Task<IEnumerable<TimesheetEntryDto>> GetList(DateTime fromDate, DateTime toDate, bool archived = false);
        Task<IEnumerable<TimesheetEntryDto>> GetTimesheetsForUserAsync(string userId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<TimesheetEntryDto>> GetTimesheetsForRoleAsync(DateTime fromDate, DateTime toDate, string userId);
        Task<TimesheetEntryDto> Get(int id);
        Task<TimesheetEntryDto> AddOrUpdate(TimesheetEntryDto timesheetEntryDto);
        Task Delete(int id);
        Task Archive(int id, string archivedBy);
        Task UnArchive(int id, string unArchivedBy);
        Task InitHistory();
    }
}
