using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDivision
{
    public long DivisionId { get; set; }

    public long ClassId { get; set; }

    public string Division { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public Guid DivisionGuid { get; set; }

    public bool IsActive { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual ICollection<TbArtCraftSkill> TbArtCraftSkills { get; set; } = new List<TbArtCraftSkill>();

    public virtual ICollection<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; } = new List<TbAspectsMathsSkill>();

    public virtual ICollection<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; } = new List<TbAspectsNaturalSciencesSkill>();

    public virtual ICollection<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; } = new List<TbAspectsRelativeAreaSkill>();

    public virtual ICollection<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; } = new List<TbAspectsSocialStudiesSkill>();

    public virtual ICollection<TbAttendancePeriod> TbAttendancePeriods { get; set; } = new List<TbAttendancePeriod>();

    public virtual ICollection<TbAttendance> TbAttendances { get; set; } = new List<TbAttendance>();

    public virtual ICollection<TbBiometricDivision> TbBiometricDivisions { get; set; } = new List<TbBiometricDivision>();

    public virtual ICollection<TbHealth> TbHealths { get; set; } = new List<TbHealth>();

    public virtual ICollection<TbListenningSkill> TbListenningSkills { get; set; } = new List<TbListenningSkill>();

    public virtual ICollection<TbMusicDanceSkill> TbMusicDanceSkills { get; set; } = new List<TbMusicDanceSkill>();

    public virtual ICollection<TbReadingSkill> TbReadingSkills { get; set; } = new List<TbReadingSkill>();

    public virtual ICollection<TbSpeakingSkill> TbSpeakingSkills { get; set; } = new List<TbSpeakingSkill>();

    public virtual ICollection<TbStudentPremotion> TbStudentPremotionFromDivisionNavigations { get; set; } = new List<TbStudentPremotion>();

    public virtual ICollection<TbStudentPremotion> TbStudentPremotionToDivisionNavigations { get; set; } = new List<TbStudentPremotion>();

    public virtual ICollection<TbStudentTotalScore> TbStudentTotalScores { get; set; } = new List<TbStudentTotalScore>();

    public virtual ICollection<TbStudent> TbStudents { get; set; } = new List<TbStudent>();

    public virtual ICollection<TbTeacherClassSubject> TbTeacherClassSubjects { get; set; } = new List<TbTeacherClassSubject>();

    public virtual ICollection<TbTeacherClass> TbTeacherClasses { get; set; } = new List<TbTeacherClass>();

    public virtual ICollection<TbTimeTable> TbTimeTables { get; set; } = new List<TbTimeTable>();

    public virtual ICollection<TbVGame> TbVGames { get; set; } = new List<TbVGame>();

    public virtual ICollection<TbVHealth> TbVHealths { get; set; } = new List<TbVHealth>();

    public virtual ICollection<TbVPersonalityDevelopment> TbVPersonalityDevelopments { get; set; } = new List<TbVPersonalityDevelopment>();

    public virtual ICollection<TbVRemark> TbVRemarks { get; set; } = new List<TbVRemark>();

    public virtual ICollection<TbWritingSkill> TbWritingSkills { get; set; } = new List<TbWritingSkill>();
}
