using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Satluj_Latest.Models;
[Table("tb_Student")]
public partial class TbStudent
{
    public TbStudent()
    {

    }
    public TbStudent(TbStudent q)
    {
        Q = q;
    }

    public long StudentId { get; set; }

    public long SchoolId { get; set; }

    public string StudentSpecialId { get; set; } = null!;

    public string StundentName { get; set; } = null!;

    public string ParentName { get; set; } = null!;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? ContactNumber { get; set; }

    public string? ClasssNumber { get; set; }

    public long ClassId { get; set; }

    public long DivisionId { get; set; }

    public long BusId { get; set; }

    public string? TripNo { get; set; }

    public string? FilePath { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid? StudentGuid { get; set; }

    public bool IsActive { get; set; }

    public long? ParentId { get; set; }

    public string? State { get; set; }

    public string? Gender { get; set; }

    public string? BloodGroup { get; set; }

    public string? ParentEmail { get; set; }

    public string? PostalCode { get; set; }

    public DateTime? Dob { get; set; }

    public string? Aadhaar { get; set; }

    public string? BioNumber { get; set; }

    public string MotherName { get; set; } = null!;

    public string? MobileNo { get; set; }

    public string? PlaceOfBirth { get; set; }

    public string? MotherTongue { get; set; }

    public DateTime? DateOfJoining { get; set; }

    public int? NationalityId { get; set; }

    public int? CountryId { get; set; }

    public int? CategoryId { get; set; }

    public virtual TbBu Bus { get; set; } = null!;

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbParent? Parent { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbArtCraftSkill> TbArtCraftSkills { get; set; } = new List<TbArtCraftSkill>();

    public virtual ICollection<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; } = new List<TbAspectsMathsSkill>();

    public virtual ICollection<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; } = new List<TbAspectsNaturalSciencesSkill>();

    public virtual ICollection<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; } = new List<TbAspectsRelativeAreaSkill>();

    public virtual ICollection<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; } = new List<TbAspectsSocialStudiesSkill>();

    public virtual ICollection<TbAttendancePeriod> TbAttendancePeriods { get; set; } = new List<TbAttendancePeriod>();

    public virtual ICollection<TbAttendance> TbAttendances { get; set; } = new List<TbAttendance>();

    public virtual ICollection<TbCoScholasticResultmain> TbCoScholasticResultmains { get; set; } = new List<TbCoScholasticResultmain>();

    public virtual ICollection<TbDeletedFeeStudent> TbDeletedFeeStudents { get; set; } = new List<TbDeletedFeeStudent>();

    public virtual ICollection<TbExamResult> TbExamResults { get; set; } = new List<TbExamResult>();

    public virtual ICollection<TbFeeDiscount> TbFeeDiscounts { get; set; } = new List<TbFeeDiscount>();

    public virtual ICollection<TbFeeDue> TbFeeDues { get; set; } = new List<TbFeeDue>();

    public virtual ICollection<TbFeeStudent> TbFeeStudents { get; set; } = new List<TbFeeStudent>();

    public virtual ICollection<TbHealth> TbHealths { get; set; } = new List<TbHealth>();

    public virtual ICollection<TbLibraryBookStudent> TbLibraryBookStudents { get; set; } = new List<TbLibraryBookStudent>();

    public virtual ICollection<TbListenningSkill> TbListenningSkills { get; set; } = new List<TbListenningSkill>();

    public virtual ICollection<TbMessage> TbMessages { get; set; } = new List<TbMessage>();

    public virtual ICollection<TbMusicDanceSkill> TbMusicDanceSkills { get; set; } = new List<TbMusicDanceSkill>();

    public virtual ICollection<TbOptionalSubjectStudent> TbOptionalSubjectStudents { get; set; } = new List<TbOptionalSubjectStudent>();

    public virtual ICollection<TbOtpmessage> TbOtpmessages { get; set; } = new List<TbOtpmessage>();

    public virtual ICollection<TbParentMessage> TbParentMessages { get; set; } = new List<TbParentMessage>();

    public virtual ICollection<TbPayment> TbPayments { get; set; } = new List<TbPayment>();

    public virtual ICollection<TbReadingSkill> TbReadingSkills { get; set; } = new List<TbReadingSkill>();

    public virtual ICollection<TbScholasticResultMain> TbScholasticResultMains { get; set; } = new List<TbScholasticResultMain>();

    public virtual ICollection<TbSmsHistory> TbSmsHistories { get; set; } = new List<TbSmsHistory>();

    public virtual ICollection<TbSpeakingSkill> TbSpeakingSkills { get; set; } = new List<TbSpeakingSkill>();

    public virtual ICollection<TbStudentBalance> TbStudentBalances { get; set; } = new List<TbStudentBalance>();

    public virtual ICollection<TbStudentMark> TbStudentMarks { get; set; } = new List<TbStudentMark>();

    public virtual ICollection<TbStudentPaidAmount> TbStudentPaidAmounts { get; set; } = new List<TbStudentPaidAmount>();

    public virtual ICollection<TbStudentPremotion> TbStudentPremotions { get; set; } = new List<TbStudentPremotion>();

    public virtual ICollection<TbStudentRemark> TbStudentRemarks { get; set; } = new List<TbStudentRemark>();

    public virtual ICollection<TbStudentTotalScore> TbStudentTotalScores { get; set; } = new List<TbStudentTotalScore>();

    public virtual ICollection<TbVGame> TbVGames { get; set; } = new List<TbVGame>();

    public virtual ICollection<TbVHealth> TbVHealths { get; set; } = new List<TbVHealth>();

    public virtual ICollection<TbVPersonalityDevelopment> TbVPersonalityDevelopments { get; set; } = new List<TbVPersonalityDevelopment>();

    public virtual ICollection<TbVRemark> TbVRemarks { get; set; } = new List<TbVRemark>();

    public virtual ICollection<TbWritingSkill> TbWritingSkills { get; set; } = new List<TbWritingSkill>();
    public TbStudent Q { get; }
}
