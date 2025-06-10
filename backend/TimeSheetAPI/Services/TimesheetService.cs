using AutoMapper;

using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Text.Json;

using TimeSheet.Api.Data;
using TimeSheet.Api.DTOs;
using TimeSheet.Api.Models;
using TimeSheet.Api.Services.Interfaces;

namespace TimeSheet.Api.Services;

public class TimesheetService : ITimesheetService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TimesheetService(AppDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient("AuthenticatedClient");
        _configuration = configuration;
    }

    public async Task<TimesheetEntryDto> Get(int id)
    {
        var query = _context.TimesheetEntries.Where(te => te.Id == id);
        return await _mapper.ProjectTo<TimesheetEntryDto>(query).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TimesheetEntryDto>> GetList(DateTime fromDate, DateTime toDate, bool archived = false)
    {
        var query = _context.TimesheetEntries
            .Where(x => x.Date >= fromDate && x.Date <= toDate && x.Archived == false);

        if (archived)
        {
            // Only return data archived in the last 365 days
            // If needed we can later add functionality to delete the entries older than 365 days.
            query = _context.TimesheetEntries.Where(x => x.Archived && x.ModifiedDate > DateTime.UtcNow.AddDays(-365));
        }

        return await _mapper.ProjectTo<TimesheetEntryDto>(query).ToListAsync();
    }

    public async Task<TimesheetEntryDto> AddOrUpdate(TimesheetEntryDto timesheetEntryDto)
    {
        TimesheetEntry timesheetEntry = _mapper.Map<TimesheetEntry>(timesheetEntryDto);
        if (timesheetEntryDto.Id == 0)
        {
            _context.TimesheetEntries.Add(timesheetEntry);
        }
        else
        {
            _context.Entry(timesheetEntry).State = EntityState.Modified;
            // Set EmployeeId to be ignored during update
            _context.Entry(timesheetEntry).Property(e => e.EmployeeId).IsModified = false;
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<TimesheetEntryDto>(timesheetEntry);
    }

    public async Task Delete(int id)
    {
        var timesheetEntry = await _context.TimesheetEntries.FindAsync(id);
        if (timesheetEntry != null)
        {
            _context.TimesheetEntries.Remove(timesheetEntry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Archive(int id, string archivedBy)
    {
        await _context.TimesheetEntries.Where(x => x.Id == id)
            .ExecuteUpdateAsync(p =>
                p.SetProperty(c => c.Archived, true)
                 .SetProperty(c => c.ModifiedDate, DateTime.UtcNow)
                 .SetProperty(c => c.ModifiedBy, archivedBy));
    }

    public async Task UnArchive(int id, string unArchivedBy)
    {
        await _context.TimesheetEntries.Where(x => x.Id == id)
            .ExecuteUpdateAsync(p =>
                p.SetProperty(c => c.Archived, false)
                 .SetProperty(c => c.ModifiedDate, DateTime.UtcNow)
                 .SetProperty(c => c.ModifiedBy, unArchivedBy));
    }

    public async Task<IEnumerable<TimesheetEntryDto>> GetTimesheetsForUserAsync(string userId, DateTime fromDate, DateTime toDate)
    {
        var query = _context.TimesheetEntries
            .Where(x => !x.Archived && x.Date >= fromDate && x.Date <= toDate && x.UserId == userId);

        return await _mapper.ProjectTo<TimesheetEntryDto>(query).ToListAsync();
    }

    public async Task<IEnumerable<TimesheetEntryDto>> GetTimesheetsForRoleAsync(DateTime fromDate, DateTime toDate, string userId)
    {
        var userRole = await GetUserRoleAsync(userId);
        var query = _context.TimesheetEntries
            .Where(x => !x.Archived && x.Date >= fromDate && x.Date <= toDate);

        var timesheets = await _mapper.ProjectTo<TimesheetEntryDto>(query).ToListAsync();
        var filteredTimesheets = new List<TimesheetEntryDto>();

        foreach (var timesheet in timesheets)
        {
            var timesheetUserRole = await GetUserRoleAsync(timesheet.UserId);
            if (timesheetUserRole == userRole)
            {
                filteredTimesheets.Add(timesheet);
            }
        }

        return filteredTimesheets;
    }

    public async Task<string> GetUserRoleAsync(string userId)
    {
        var baseUrl = _configuration["BaseApiUrl"];
        var response = await _httpClient.GetAsync($"{baseUrl}/api/user/GetUserRole?userId={userId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool> IsTeamHeadAsync(string userId)
    {
        var baseUrl = _configuration["BaseApiUrl"];
        var response = await _httpClient.GetAsync($"{baseUrl}/api/user/IsTeamHead?userId={userId}");
        response.EnsureSuccessStatusCode();
        return bool.Parse(await response.Content.ReadAsStringAsync());
    }

    public async Task InitHistory()
    {
        var timesheets = await _context.TimesheetEntries.ToListAsync();
        var histories = timesheets.Select(timesheet =>
            new
            {
                Id = Guid.NewGuid(),
                Action = 0,
                Type = 0,
                RecordId = timesheet.Id.ToString(),
                Date = timesheet.CreatedDate,
                State = JsonSerializer.Serialize(timesheet),
                UserId = timesheet.UserId
            }
        );

        var baseUrl = _configuration["BaseApiUrl"];

        foreach (var history in histories)
        {
            var existingHistories = await _httpClient.GetFromJsonAsync<List<object>>($"{baseUrl}/api/EntityHistory/0/{history.RecordId}");
            var historyExists = existingHistories.Count > 0;

            if (!historyExists)
            {
                await _httpClient.PostAsJsonAsync($"{baseUrl}/api/EntityHistory", history);
            }
        }
    }
}
