using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Api.Models;

public class TimesheetEntry : BaseModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Username { get; set; }
    public string EmployeeId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Range(0.1, double.MaxValue)]
    public double? Overtime { get; set; }

    [MinLength(5)]
    [StringLength(500)]
    public string OvertimeDescription { get; set; }

    [Range(1, double.MaxValue)]
    public double? Dirtbonus { get; set; }

    [MinLength(5)]
    [StringLength(500)]
    public string DirtbonusDescription { get; set; }

    [StringLength(100)]
    public string PayoutOption { get; set; }

    [Required]
    [StringLength(255)]
    public string Machine { get; set; }

    public int Status { get; set; }

    [MinLength(5)]
    [StringLength(500)]
    public string RejectionReason { get; set; }

    public TimeOnly? OvertimeFrom { get; set; }

    public string ApprovedBy { get; set; }
    public string RejectedBy { get; set; }
}
