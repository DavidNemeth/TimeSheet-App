using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Api.DTOs;

public class TimesheetEntryDto
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string Username { get; set; }
    public string EmployeeId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Range(0.1, double.MaxValue)]
    public double? Overtime { get; set; }

    [MinLength(5)]
    [MaxLength(500)]
    public string OvertimeDescription { get; set; }

    [Range(1, double.MaxValue)]
    public double? Dirtbonus { get; set; }

    [MinLength(5)]
    [StringLength(500)]
    public string DirtbonusDescription { get; set; }

    public string PayoutOption { get; set; }

    [Required]
    public string Machine { get; set; }

    public int Status { get; set; }

    [MinLength(5)]
    [StringLength(500)]
    public string RejectionReason { get; set; }

    public TimeOnly? OvertimeFrom { get; set; }
    public string ApprovedBy { get; set; }
    public string RejectedBy { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
