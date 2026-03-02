using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbParent
{
    public long ParentId { get; set; }

    public string ParentName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public string Password { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public Guid ParentGuid { get; set; }

    public bool IsActive { get; set; }

    public string? State { get; set; }

    public string? FilePath { get; set; }

    public string? PostalCode { get; set; }

    public string? FatherEducation { get; set; }

    public string? FatherOccupation { get; set; }

    public int? FatherCountryId { get; set; }

    public string? MotherName { get; set; }

    public string? MotherEducation { get; set; }

    public string? MotherOccupation { get; set; }

    public string? MotherContactNo { get; set; }

    public string? MotherEmail { get; set; }

    public string? MotherAddress { get; set; }

    public int? MotherCountryId { get; set; }

    public string? MotherState { get; set; }

    public string? MotherCity { get; set; }

    public string? MotherPincode { get; set; }

    public string? GuardianName { get; set; }

    public string? GuardianEducation { get; set; }

    public string? GuardianOccupation { get; set; }

    public string? GuardianContactNo { get; set; }

    public string? GuardianEmail { get; set; }

    public string? GuardianAddress { get; set; }

    public int? GuardianCountryId { get; set; }

    public string? GuardianState { get; set; }

    public string? GuardianCity { get; set; }

    public string? GuardianPincode { get; set; }

    public long? StudentId { get; set; }

    public bool IsLoggedIn { get; set; }

    public virtual ICollection<TbCcavenueCourseResponse> TbCcavenueCourseResponses { get; set; } = new List<TbCcavenueCourseResponse>();

    public virtual ICollection<TbParentMessage> TbParentMessages { get; set; } = new List<TbParentMessage>();

    public virtual ICollection<TbStudent> TbStudents { get; set; } = new List<TbStudent>();
}
