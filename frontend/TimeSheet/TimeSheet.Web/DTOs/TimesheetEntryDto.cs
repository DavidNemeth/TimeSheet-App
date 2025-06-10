using SharedLib.Attributes;
using SharedLib.Enums;
using SharedLib.Localization;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

using TimeSheet.Web.Enums;

namespace TimeSheet.Web.DTOs;

public class TimesheetEntryDto
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string Username { get; set; }
    public string EmployeeId { get; set; }

    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    [RequiredIf(nameof(Dirtbonus), ComparisonType.IsNullOrDefault, ErrorMessageResourceName = "Either_Overtime_or_Dirtbonus_field_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    [Range(0.1, double.MaxValue, ErrorMessageResourceName = "OverTime_field_can_not_be_0_", ErrorMessageResourceType = typeof(SharedResources))]
    public double? Overtime { get; set; }
    public string OvertimeString => Overtime is null ? "" : Overtime.Value.ToString(new CultureInfo("de-DE"));

    [MinLength(5, ErrorMessageResourceName = "The_minimum_length_is_5_character_", ErrorMessageResourceType = typeof(SharedResources))]
    [MaxLength(500)]
    [RequiredIf(nameof(Overtime), ComparisonType.IsNotNullOrDefault, ErrorMessageResourceName = "The_Description_field_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    public string OvertimeDescription { get; set; }

    [RequiredIf(nameof(Overtime), ComparisonType.IsNullOrDefault, ErrorMessageResourceName = "Either_Overtime_or_Dirtbonus_field_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    [Range(1, double.MaxValue, ErrorMessageResourceName = "Dirtbonus_field_can_not_be_0_", ErrorMessageResourceType = typeof(SharedResources))]
    public double? Dirtbonus { get; set; }
    public string DirtbonusString => Dirtbonus is null ? "" : Dirtbonus.Value.ToString(new CultureInfo("de-DE"));

    [MinLength(5, ErrorMessageResourceName = "The_minimum_length_is_5_character_", ErrorMessageResourceType = typeof(SharedResources))]
    [StringLength(500)]
    [RequiredIf(nameof(Dirtbonus), ComparisonType.IsNotNullOrDefault, ErrorMessageResourceName = "The_Description_field_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    public string DirtbonusDescription { get; set; }

    [RequiredIf(nameof(Overtime), ComparisonType.IsNotNullOrDefault), Display(Name = "Overtime Payout")]
    public string PayoutOption { get; set; }
    [Required]
    public string Machine { get; set; } = "PM3";

    public Status Status { get; set; } = Status.New;

    private Status? _oldStatus;
    public Status OldStatus
    {
        get
        {
            _oldStatus ??= Status;
            return _oldStatus.Value;
        }
        set => _oldStatus = value;
    }

    public string StatusString => Status.ToString();

    [MinLength(5, ErrorMessageResourceName = "The_minimum_length_is_5_character_", ErrorMessageResourceType = typeof(SharedResources))]
    [StringLength(500)]
    [RequiredIf(nameof(Status), ComparisonType.Is, Status.Rejected, ErrorMessageResourceName = "Rejection_reason_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    public string RejectionReason { get; set; }

    private TimeOnly? _overtimeFrom;
    [RequiredIf(nameof(Overtime), ComparisonType.IsNotNullOrDefault, ErrorMessageResourceName = "The_From_field_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    public TimeOnly? OvertimeFrom
    {
        get
        {
            if ((_overtimeFrom, Overtime) is (null, not null))
            {
                _overtimeFrom = new TimeOnly(0, 0);
            }

            return _overtimeFrom;
        }
        set => _overtimeFrom = value;
    }

    private TimeOnly? _overtimeTo;
    [RequiredIf(nameof(Overtime), ComparisonType.IsNotNullOrDefault, ErrorMessageResourceName = "The_To_field_is_required_", ErrorMessageResourceType = typeof(SharedResources))]
    public TimeOnly? OvertimeTo
    {
        get
        {
            if ((_overtimeTo, Overtime) is (null, not null))
            {
                _overtimeTo = OvertimeFrom.Value.AddHours(Overtime.Value);
            }

            return _overtimeTo;
        }
        set => _overtimeTo = value;
    }
    public string ApprovedBy { get; set; }
    public string RejectedBy { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool Archived { get; set; }

    [JsonIgnore]
    public string ApprovedRejectedBy => ApprovedBy ?? RejectedBy;
}
