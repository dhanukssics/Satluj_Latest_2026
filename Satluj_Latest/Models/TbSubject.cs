using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSubject
{
    public long SubId { get; set; }

    public long SchoolI { get; set; }

    public string SubjectName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TmeStamp { get; set; }

    public bool IsOptonal { get; set; }

    public string? Abbreviation { get; set; }

    public string? Code { get; set; }

    public int? EnumTypeId { get; set; }

    public bool? HavePractical { get; set; }

    public virtual TbSchool SchoolINavigation { get; set; } = null!;

    public virtual ICollection<TbArtCraftSkill> TbArtCraftSkills { get; set; } = new List<TbArtCraftSkill>();

    public virtual ICollection<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; } = new List<TbAspectsMathsSkill>();

    public virtual ICollection<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; } = new List<TbAspectsNaturalSciencesSkill>();

    public virtual ICollection<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; } = new List<TbAspectsRelativeAreaSkill>();

    public virtual ICollection<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; } = new List<TbAspectsSocialStudiesSkill>();

    public virtual ICollection<TbDeclaredExamSubject> TbDeclaredExamSubjects { get; set; } = new List<TbDeclaredExamSubject>();

    public virtual ICollection<TbExamSubject> TbExamSubjects { get; set; } = new List<TbExamSubject>();

    public virtual ICollection<TbListenningSkill> TbListenningSkills { get; set; } = new List<TbListenningSkill>();

    public virtual ICollection<TbMusicDanceSkill> TbMusicDanceSkills { get; set; } = new List<TbMusicDanceSkill>();

    public virtual ICollection<TbOptionalSubjectStudent> TbOptionalSubjectStudents { get; set; } = new List<TbOptionalSubjectStudent>();

    public virtual ICollection<TbReadingSkill> TbReadingSkills { get; set; } = new List<TbReadingSkill>();

    public virtual ICollection<TbScolasticAreaResultDetail> TbScolasticAreaResultDetails { get; set; } = new List<TbScolasticAreaResultDetail>();

    public virtual ICollection<TbSpeakingSkill> TbSpeakingSkills { get; set; } = new List<TbSpeakingSkill>();

    public virtual ICollection<TbTeacherClassSubject> TbTeacherClassSubjects { get; set; } = new List<TbTeacherClassSubject>();

    public virtual ICollection<TbTimeTable> TbTimeTables { get; set; } = new List<TbTimeTable>();

    public virtual ICollection<TbWritingSkill> TbWritingSkills { get; set; } = new List<TbWritingSkill>();
}
