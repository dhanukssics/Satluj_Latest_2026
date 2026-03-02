using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class Lkg_Ukg_Nu_ReportModel
    {
        public Title_Report Title_Report { get; set; }
        public English_Report English_Report { get; set; }
        public Hindi_Report Hindi_Report { get; set; }
        public Mathematics_Report Mathematics_Report { get; set; }
        public EnvironmentalStudies_Report EnvironmentalStudies_Report { get; set; }
        public SCHOLASTICAREAS_Report SCHOLASTICAREAS_Report { get; set; }
        public PERSONALASSESSMENTS_Report PERSONALASSESSMENTS_Report { get; set; }

        public Attendance_Report Attendance_Report { get; set; }
        public Term_Remarks_Report Term_Remarks_Report { get; set; }
       
        public HealthStatus_Report HealthStatus_Report { get; set; }
    }

    public class Title_Report
    {
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DateOfBirth { get; set; }
        public string Session { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }

        public string Division { get; set; }

        public string ReportName { get; set; }
        public string SchoolName { get; set; }
        public string SchoolLogo { get; set; }
        public string StudentProfile { get; set; }
        public string Static_Date { get; set; }



    }

    public class English_Report
    {
        public string WritingSkills_I { get; set; }
        public string ReadingSkills_I { get; set; }
        public string SpeakingSkills_I { get; set; }
        public string RecitingSkills_I { get; set; }
        public string ListeningSkills_I { get; set; }
        public string Oral_I { get; set; }

        public string WritingSkills_II { get; set; }
        public string ReadingSkills_II { get; set; }
        public string SpeakingSkills_II { get; set; }
        public string RecitingSkills_II { get; set; }
        public string ListeningSkills_II { get; set; }
        public string Oral_II { get; set; }

        public string WritingSkills_III { get; set; }
        public string ReadingSkills_III { get; set; }
        public string SpeakingSkills_III { get; set; }
        public string RecitingSkills_III { get; set; }
        public string ListeningSkills_III { get; set; }
        public string Oral_III { get; set; }

    }
    public class Hindi_Report
    {
        public string WritingSkills_I { get; set; }
        public string ReadingSkills_I { get; set; }
        public string SpeakingSkills_I { get; set; }
        public string RecitingSkills_I { get; set; }
        public string ListeningSkills_I { get; set; }
        public string Oral_I { get; set; }

        public string WritingSkills_II { get; set; }
        public string ReadingSkills_II { get; set; }
        public string SpeakingSkills_II { get; set; }
        public string RecitingSkills_II { get; set; }
        public string ListeningSkills_II { get; set; }
        public string Oral_II { get; set; }

        public string WritingSkills_III { get; set; }
        public string ReadingSkills_III { get; set; }
        public string SpeakingSkills_III { get; set; }
        public string RecitingSkills_III { get; set; }
        public string ListeningSkills_III { get; set; }
        public string Oral_III { get; set; }
    }
    public class Mathematics_Report
    {
        public string LogicalandReasoning_I { get; set; }
        public string Understandingofconcepts_I { get; set; }
        public string MentalAbility_I { get; set; }
        public string Oral_I { get; set; }

        public string LogicalandReasoning_II { get; set; }
        public string Understandingofconcepts_II { get; set; }
        public string MentalAbility_II { get; set; }
        public string Oral_II { get; set; }

        public string LogicalandReasoning_III { get; set; }
        public string Understandingofconcepts_III { get; set; }
        public string MentalAbility_III { get; set; }
        public string Oral_III { get; set; }
    }
    public class EnvironmentalStudies_Report
    {
        public string AdventureStudies_I { get; set; }
        public string Understandingofconcepts_I { get; set; }
        public string Activity_I { get; set; }
        public string Oral_I { get; set; }

        public string AdventureStudies_II { get; set; }
        public string Understandingofconcepts_II { get; set; }
        public string Activity_II { get; set; }
        public string Oral_II { get; set; }

        public string AdventureStudies_III { get; set; }
        public string Understandingofconcepts_III { get; set; }
        public string Activity_III { get; set; }
        public string Oral_III { get; set; }
    }

    public class SCHOLASTICAREAS_Report
    {
        public string Rhymes_I { get; set; }
        public string ComputerSkills_I { get; set; }
        public string GeneralStudies_I { get; set; }
        public string StoryTelling_I { get; set; }
        public string PatternWriting_I { get; set; }
        public string SmartClassPerformance_I { get; set; }
        public string ArtandCraft_I { get; set; }
        public string Handwriting_I { get; set; }
        public string PhysicalEducation_I { get; set; }
        public string MusicandDance_I { get; set; }
        public string CommunicationSkills_I { get; set; }
        public string Worksheets_I { get; set; }
        public string InformationSheets_I { get; set; }

        public string Rhymes_II { get; set; }
        public string ComputerSkills_II { get; set; }
        public string GeneralStudies_II { get; set; }
        public string StoryTelling_II { get; set; }
        public string PatternWriting_II { get; set; }
        public string SmartClassPerformance_II { get; set; }
        public string ArtandCraft_II { get; set; }
        public string Handwriting_II { get; set; }
        public string PhysicalEducation_II { get; set; }
        public string MusicandDance_II { get; set; }
        public string CommunicationSkills_II { get; set; }
        public string Worksheets_II { get; set; }
        public string InformationSheets_II { get; set; }

        public string Rhymes_III { get; set; }
        public string ComputerSkills_III { get; set; }
        public string GeneralStudies_III { get; set; }
        public string StoryTelling_III { get; set; }
        public string PatternWriting_III { get; set; }
        public string SmartClassPerformance_III { get; set; }
        public string ArtandCraft_III { get; set; }
        public string Handwriting_III { get; set; }
        public string PhysicalEducation_III { get; set; }
        public string MusicandDance_III { get; set; }
        public string CommunicationSkills_III { get; set; }
        public string Worksheets_III { get; set; }
        public string InformationSheets_III { get; set; }

    }

    public class PERSONALASSESSMENTS_Report
    {
        public string Courteousness_I { get; set; }
        public string Relationship_I { get; set; }
        public string Confidence_I { get; set; }
        public string CareOfBelongings_I { get; set; }
        public string Neatness_I { get; set; }
        public string Regularity_I { get; set; }
        public string Punctuality_I { get; set; }
        public string Initiative_I { get; set; }
        public string SharingandCaring_I { get; set; }
        public string RespectForOthersProperty_I { get; set; }
        public string Concentration_I { get; set; }
        public string Cleanliness_I { get; set; }
        public string GeneralConduct_I { get; set; }
        public string LeadershipSkills_I { get; set; }

        public string Courteousness_II { get; set; }
        public string Relationship_II { get; set; }
        public string Confidence_II { get; set; }
        public string CareOfBelongings_II { get; set; }
        public string Neatness_II { get; set; }
        public string Regularity_II { get; set; }
        public string Punctuality_II { get; set; }
        public string Initiative_II { get; set; }
        public string SharingandCaring_II { get; set; }
        public string RespectForOthersProperty_II { get; set; }
        public string Concentration_II { get; set; }
        public string Cleanliness_II { get; set; }
        public string GeneralConduct_II { get; set; }
        public string LeadershipSkills_II { get; set; }

        public string Courteousness_III { get; set; }
        public string Relationship_III { get; set; }
        public string Confidence_III { get; set; }
        public string CareOfBelongings_III { get; set; }
        public string Neatness_III { get; set; }
        public string Regularity_III { get; set; }
        public string Punctuality_III { get; set; }
        public string Initiative_III { get; set; }
        public string SharingandCaring_III { get; set; }
        public string RespectForOthersProperty_III { get; set; }
        public string Concentration_III { get; set; }
        public string Cleanliness_III { get; set; }
        public string GeneralConduct_III { get; set; }
        public string LeadershipSkills_III { get; set; }

    }

    public class Attendance_Report
    {
        public string No_of_Working_Days_I { get; set; }
        public string Days_Attended_I { get; set; }

        public string No_of_Working_Days_II { get; set; }
        public string Days_Attended_II { get; set; }

        public string No_of_Working_Days_III { get; set; }
        public string Days_Attended_III { get; set; }



    }

    public class Term_Remarks_Report
    {
        public string Term_Remarks_I { get; set; }

        public string Term_Remarks_II { get; set; }

        public string Term_Remarks_III { get; set; }
    }
    
    public class HealthStatus_Report
    {
        public string Height_Cms_I { get; set; }
        public string Weight_Kg_I { get; set; }

        public string Height_Cms_II { get; set; }
        public string Weight_Kg_II { get; set; }

        public string Height_Cms_III { get; set; }
        public string Weight_Kg_III { get; set; }
    }


}