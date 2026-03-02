using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Satluj_Latest.Models;

public partial class TbLogin
{
    public TbLogin()
    {
    }

    public TbLogin(TbLogin q)
    {
        Q = q;
    }

    public long UserId { get; set; }

    public long SchoolId { get; set; }

    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool DisableStatus { get; set; }

    public Guid LoginGuid { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbArtCraftSkill> TbArtCraftSkills { get; set; } = new List<TbArtCraftSkill>();

    public virtual ICollection<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; } = new List<TbAspectsMathsSkill>();

    public virtual ICollection<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; } = new List<TbAspectsNaturalSciencesSkill>();

    public virtual ICollection<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; } = new List<TbAspectsRelativeAreaSkill>();

    public virtual ICollection<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; } = new List<TbAspectsSocialStudiesSkill>();

    public virtual ICollection<TbAssetsLiabilityDatum> TbAssetsLiabilityData { get; set; } = new List<TbAssetsLiabilityDatum>();

    public virtual ICollection<TbAttendancePeriod> TbAttendancePeriods { get; set; } = new List<TbAttendancePeriod>();

    public virtual ICollection<TbAttendance> TbAttendances { get; set; } = new List<TbAttendance>();

    public virtual ICollection<TbBankBookDatum> TbBankBookData { get; set; } = new List<TbBankBookDatum>();

    public virtual ICollection<TbBankEntry> TbBankEntries { get; set; } = new List<TbBankEntry>();

    public virtual ICollection<TbCashEntry> TbCashEntries { get; set; } = new List<TbCashEntry>();

    public virtual ICollection<TbCircular> TbCirculars { get; set; } = new List<TbCircular>();

    public virtual ICollection<TbDayBookDatum> TbDayBookData { get; set; } = new List<TbDayBookDatum>();

    public virtual ICollection<TbExam> TbExams { get; set; } = new List<TbExam>();

    public virtual ICollection<TbListenningSkill> TbListenningSkills { get; set; } = new List<TbListenningSkill>();

    public virtual ICollection<TbMaster> TbMasters { get; set; } = new List<TbMaster>();

    public virtual ICollection<TbMusicDanceSkill> TbMusicDanceSkills { get; set; } = new List<TbMusicDanceSkill>();

    public virtual ICollection<TbReadingSkill> TbReadingSkills { get; set; } = new List<TbReadingSkill>();

    public virtual ICollection<TbSpeakingSkill> TbSpeakingSkills { get; set; } = new List<TbSpeakingSkill>();

    public virtual ICollection<TbStaffSmshistory> TbStaffSmshistories { get; set; } = new List<TbStaffSmshistory>();

    public virtual ICollection<TbStaff> TbStaffs { get; set; } = new List<TbStaff>();

    public virtual ICollection<TbStockUpdate> TbStockUpdates { get; set; } = new List<TbStockUpdate>();

    public virtual ICollection<TbTeacher> TbTeachers { get; set; } = new List<TbTeacher>();

    public virtual ICollection<TbUserAllotedMenu> TbUserAllotedMenus { get; set; } = new List<TbUserAllotedMenu>();

    public virtual ICollection<TbUserRole> TbUserRoles { get; set; } = new List<TbUserRole>();

    public virtual ICollection<TbVGame> TbVGames { get; set; } = new List<TbVGame>();

    public virtual ICollection<TbVHealth> TbVHealths { get; set; } = new List<TbVHealth>();

    public virtual ICollection<TbVPersonalityDevelopment> TbVPersonalityDevelopments { get; set; } = new List<TbVPersonalityDevelopment>();

    public virtual ICollection<TbVRemark> TbVRemarks { get; set; } = new List<TbVRemark>();

    public virtual ICollection<TbWritingSkill> TbWritingSkills { get; set; } = new List<TbWritingSkill>();
    public TbLogin Q { get; }
   
}
