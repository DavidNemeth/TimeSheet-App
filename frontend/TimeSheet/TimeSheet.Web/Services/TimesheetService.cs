using Polly;
using Polly.CircuitBreaker;

using TimeSheet.Web.DTOs;
using TimeSheet.Web.Services.Interfaces;

namespace TimeSheet.Web.Services;

public class TimesheetService : ITimesheetService
{
    private readonly HttpClient _http;
    private readonly ILogger<TimesheetService> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public TimesheetService(HttpClient http, ILogger<TimesheetService> logger)
    {
        _http = http;
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(15));
    }

    public async Task<List<TimesheetEntryDto>> GetTimesheetEntriesAsync(DateTime startDate, DateTime endDate, string userId = null, bool forRole = false)
    {
        return await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var url = $"api/timesheetentries?fromDate={startDate:yyyy-MM-dd}&toDate={endDate:yyyy-MM-dd}&userId={userId}&forRole={forRole}";
                var entries = await _http.GetFromJsonAsync<List<TimesheetEntryDto>>(url);
                return entries;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve timesheet entries.");
                throw new ApplicationException("An error occurred while fetching timesheet entries.", ex);
            }
        });
    }

    public async Task<List<TimesheetEntryDto>> GetArchivedEntriesAsync()
    {
        return await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var url = $"api/timesheetentries?archived=true";
                var entries = await _http.GetFromJsonAsync<List<TimesheetEntryDto>>(url);
                return entries;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve timesheet entries.");
                throw new ApplicationException("An error occurred while fetching timesheet entries.", ex);
            }
        });
    }

    public async Task<List<TimesheetEntryDto>> GetAllTimesheetEntriesAsync(DateTime startDate, DateTime endDate)
    {
        return await GetTimesheetEntriesAsync(startDate, endDate);
    }

    public async Task<TimesheetEntryDto> GetTimesheetEntryAsync(int id)
    {
        try
        {
            var entry = await _http.GetFromJsonAsync<TimesheetEntryDto>($"api/timesheetentries/{id}");
            if (entry == null)
            {
                _logger.LogWarning("No timesheet entry found with ID {ID}.", id);
                return null;
            }
            return entry;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Timesheet entry not found with ID {ID}: {Message}", id, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve timesheet entry with ID {ID}.", id);
            throw new ApplicationException($"Error retrieving timesheet entry with ID {id}.", ex);
        }
    }

    public async Task<TimesheetEntryDto> CreateTimesheetEntryAsync(TimesheetEntryDto timesheetEntryDto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/timesheetentries", timesheetEntryDto);
            var newTimesheetDto = await ProcessHttpResponse<TimesheetEntryDto>(response);
            return newTimesheetDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create timesheet entry.");
            throw new ApplicationException("Error creating timesheet entry.", ex);
        }
    }

    public async Task<TimesheetEntryDto> UpdateTimesheetEntryAsync(TimesheetEntryDto timesheetEntryDto)
    {
        try
        {
            ValidateTimesheetEntryDto(timesheetEntryDto);
            var response = await _http.PutAsJsonAsync($"api/timesheetentries/{timesheetEntryDto.Id}", timesheetEntryDto);
            var updatedTimesheet = await ProcessHttpResponse<TimesheetEntryDto>(response);
            return updatedTimesheet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update timesheet entry with ID {ID}.", timesheetEntryDto?.Id);
            throw new ApplicationException($"Error updating timesheet entry with ID {timesheetEntryDto?.Id}.", ex);
        }
    }

    public async Task DeleteTimesheetEntryAsync(int id)
    {
        try
        {
            HttpResponseMessage response = await _http.DeleteAsync($"api/timesheetentries/{id}");
            await ProcessHttpResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete timesheet entry with ID {ID}.", id);
            throw new ApplicationException($"Error deleting timesheet entry with ID {id}.", ex);
        }
    }

    public async Task ArchiveEntryAsync(int id, string archivedBy)
    {
        try
        {
            HttpResponseMessage response = await _http.PostAsJsonAsync($"api/timesheetentries/{id}/archive", archivedBy);
            await ProcessHttpResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to archive timesheet entry with ID {ID}.", id);
            throw new ApplicationException($"Error archive timesheet entry with ID {id}.", ex);
        }
    }

    public async Task UnArchiveEntryAsync(int id, string unArchivedBy)
    {
        try
        {
            HttpResponseMessage response = await _http.PostAsJsonAsync($"api/timesheetentries/{id}/unarchive", unArchivedBy);
            await ProcessHttpResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unarchive timesheet entry with ID {ID}.", id);
            throw new ApplicationException($"Error unarchive timesheet entry with ID {id}.", ex);
        }
    }

    private async Task<T> ProcessHttpResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError("HTTP error {StatusCode}: {Content}", response.StatusCode, content);
            throw new HttpRequestException($"Server returned status code {response.StatusCode}, response was: {content}");
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    private async Task ProcessHttpResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError("HTTP error {StatusCode}: {Content}", response.StatusCode, content);
            throw new HttpRequestException($"Server returned status code {response.StatusCode}, response was: {content}");
        }
    }

    private void ValidateTimesheetEntryDto(TimesheetEntryDto dto)
    {
        if (dto == null)
            throw new ArgumentException("TimesheetEntryDto cannot be null.", nameof(dto));

        if (dto.Id <= 0)
            throw new ArgumentException("TimesheetEntryDto must have a valid ID for update operations.", nameof(dto));
    }

    private DateTime ConvertToUtc(DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            : dateTime.ToUniversalTime();
    }
}
