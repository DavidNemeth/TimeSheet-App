using Microsoft.AspNetCore.Mvc;

using TimeSheet.Api.DTOs;
using TimeSheet.Api.Services.Interfaces;

namespace TimeSheet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimesheetEntriesController : ControllerBase
{
    private readonly ITimesheetService _timesheetService;

    public TimesheetEntriesController(ITimesheetService timesheetService)
    {
        _timesheetService = timesheetService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimesheetEntryDto>>> GetTimesheetEntries(DateTime fromDate, DateTime toDate, string userId = null, bool forRole = false, bool archived = false)
    {
        fromDate = fromDate.ToUniversalTime();
        toDate = toDate.Date.AddDays(1).AddTicks(-1).ToUniversalTime();

        IEnumerable<TimesheetEntryDto> timesheetList;

        if (forRole && !string.IsNullOrEmpty(userId))
        {
            timesheetList = await _timesheetService.GetTimesheetsForRoleAsync(fromDate, toDate, userId);
        }
        else if (!string.IsNullOrEmpty(userId))
        {
            timesheetList = await _timesheetService.GetTimesheetsForUserAsync(userId, fromDate, toDate);
        }
        else
        {
            timesheetList = await _timesheetService.GetList(fromDate, toDate, archived);
        }

        return Ok(timesheetList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TimesheetEntryDto>> GetTimesheetEntry(int id)
    {
        var timesheetEntryDto = await _timesheetService.Get(id);
        if (timesheetEntryDto == null)
        {
            return NotFound();
        }
        return Ok(timesheetEntryDto);
    }

    [HttpPost]
    public async Task<ActionResult<TimesheetEntryDto>> PostTimesheetEntry(TimesheetEntryDto timesheetEntryDto)
    {
        if (timesheetEntryDto.Date.Kind != DateTimeKind.Utc)
        {
            timesheetEntryDto.Date = timesheetEntryDto.Date.ToUniversalTime();
        }

        var result = await _timesheetService.AddOrUpdate(timesheetEntryDto);
        return CreatedAtAction(nameof(GetTimesheetEntry), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTimesheetEntry(int id, TimesheetEntryDto timesheetEntryDto)
    {
        if (id != timesheetEntryDto.Id)
        {
            return BadRequest();
        }

        if (timesheetEntryDto.Date.Kind != DateTimeKind.Utc)
        {
            timesheetEntryDto.Date = timesheetEntryDto.Date.ToUniversalTime();
        }

        var result = await _timesheetService.AddOrUpdate(timesheetEntryDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimesheetEntry(int id)
    {
        await _timesheetService.Delete(id);
        return NoContent();
    }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> ArchiveTimesheetEntry(int id, [FromBody] string archivedBy)
    {
        await _timesheetService.Archive(id, archivedBy);
        return NoContent();
    }

    [HttpPost("{id}/unarchive")]
    public async Task<IActionResult> UnArchiveTimesheetEntry(int id, [FromBody] string unArchivedBy)
    {
        await _timesheetService.UnArchive(id, unArchivedBy);
        return NoContent();
    }

    [HttpPost("inithistory")]
    public async Task<IActionResult> InitHistory()
    {
        await _timesheetService.InitHistory();
        return NoContent();
    }
}
