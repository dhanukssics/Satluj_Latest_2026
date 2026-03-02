using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbVAssesment
{
    public long Id { get; set; }

    public int AssesmentId { get; set; }

    public long PeriodId { get; set; }

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbAcademicPeriod Period { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbArtCraftSkill> TbArtCraftSkills { get; set; } = new List<TbArtCraftSkill>();

    public virtual ICollection<TbAspectsMathsSkill> TbAspectsMathsSkills { get; set; } = new List<TbAspectsMathsSkill>();

    public virtual ICollection<TbAspectsNaturalSciencesSkill> TbAspectsNaturalSciencesSkills { get; set; } = new List<TbAspectsNaturalSciencesSkill>();

    public virtual ICollection<TbAspectsRelativeAreaSkill> TbAspectsRelativeAreaSkills { get; set; } = new List<TbAspectsRelativeAreaSkill>();

    public virtual ICollection<TbAspectsSocialStudiesSkill> TbAspectsSocialStudiesSkills { get; set; } = new List<TbAspectsSocialStudiesSkill>();

    public virtual ICollection<TbListenningSkill> TbListenningSkills { get; set; } = new List<TbListenningSkill>();

    public virtual ICollection<TbMusicDanceSkill> TbMusicDanceSkills { get; set; } = new List<TbMusicDanceSkill>();

    public virtual ICollection<TbReadingSkill> TbReadingSkills { get; set; } = new List<TbReadingSkill>();

    public virtual ICollection<TbSpeakingSkill> TbSpeakingSkills { get; set; } = new List<TbSpeakingSkill>();

    public virtual ICollection<TbVGame> TbVGames { get; set; } = new List<TbVGame>();

    public virtual ICollection<TbVHealth> TbVHealths { get; set; } = new List<TbVHealth>();

    public virtual ICollection<TbVPersonalityDevelopment> TbVPersonalityDevelopments { get; set; } = new List<TbVPersonalityDevelopment>();

    public virtual ICollection<TbVRemark> TbVRemarks { get; set; } = new List<TbVRemark>();

    public virtual ICollection<TbWritingSkill> TbWritingSkills { get; set; } = new List<TbWritingSkill>();
}
