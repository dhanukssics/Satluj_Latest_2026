
using System.ComponentModel.DataAnnotations;


namespace Satluj_Latest.Models
{
    public class StudentModel
    {
        public string BusId { get; set; }
        public long schoolId { get; set; }
        public string stringStudentId { get; set; }

        [Required(ErrorMessage = "Class Required")]
        public long classId { get; set; }
        public long studentId { get; set; }
        [Required(ErrorMessage = "Division Required")]
        public long divisionId { get; set; }
        public long toDivisionId { get; set; }

        [Required(ErrorMessage = "Admission No Required")]
        public string admissionNo { get; set; }
        [Required(ErrorMessage = "Name Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string studentName { get; set; }
        public string division { get; set; }
        public string rollNo { get; set; }
        public string className { get; set; }
        //[StringLength(10, ErrorMessage = "Number cannot be longer than 10 characters.")]
        //[MaxLength(10, ErrorMessage = "Number cannot be longer than 10 characters"), MinLength(10, ErrorMessage = "Number cannot be less than 10 characters")]
        //[StringLength(11, MinimumLength = 10, ErrorMessage = "Number must be 10 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Contact must be numeric")]
        //[Required(ErrorMessage = "ContactNo Required")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Number must be 10 or 11 digit")]
        [Required(ErrorMessage = "Contact Required")]
        public string contactNo { get; set; }
        [Required(ErrorMessage = "Father Name Required")]
        public string parentName { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string parentEmail { get; set; }

        public string tripNumber { get; set; }
        public string address { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string state { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string city { get; set; }
        public string classInCharge { get; set; }
        public string profilePic { get; set; }
        public string filePath { get; set; }
        [Required(ErrorMessage = "Gender Required")]
        public string gender { get; set; }
        public string bloodGroup { get; set; }
        //[Required(ErrorMessage = "DOB Required")]
        public DateTime DOB { get; set; }
        //[Required(ErrorMessage = "DOB Required")]
        public String DOBstring { get; set; }
        public string CurrentDate { get; set; }
        public string biometricId { get; set; }

        public string AcademinYear { get; set; }
        public string SchoolName { get; set; }
        public string Data { get; set; }
        //--------------------------------------------
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Number must be 10 or 11 digit")]
        public string MobileNo { get; set; }
        public string RollNo1 { get; set; }
        public DateTime DateOfJoining { get; set; }
        public String DateOfJoiningString { get; set; }

        public Nationality NationalityId { get; set; }
        public Country CountryId { get; set; }
        public StudentCategory CategoryId { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string PlaceOfBirth { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string MotherTongue { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Number must be 6 digit")]
        public string Pincode { get; set; }

        //--------------------------------------------
        //---------------------Parents Informations-----------------------
        [Required(ErrorMessage = "Father Name Required")]
        public string FatherName { get; set; }

        
        public string MotherName { get; set; }
        public string GuardianName { get; set; }
        public string FatherEducation { get; set; }
        public string MotherEducaton { get; set; }
        public string GuardianEduation { get; set; }
        public string FatherOccupation { get; set; }
        public string MotherOccupation { get; set; }
        public string GuardianOccupation { get; set; }
        [Required(ErrorMessage = "Contact  Required")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Number must be 10 or 11 digit")]
        public string FatherContact { get; set; }
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Number must be 10 or 11 digit")]
        public string MotherContact { get; set; }
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Number must be 10 or 11 digit")]
        public string GuardianContact { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(ErrorMessage = "Email Required")]
        public string FatherEmail { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string MotherEmail { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        //[Required(ErrorMessage = "Email Required")]
        public string GuardianEmail { get; set; }
        public string FatherAddress { get; set; }
        public string MotherAddress { get; set; }
        public string GuardianAddress { get; set; }
        public Country FatherCountryId { get; set; }
        public Country MotherCountryId { get; set; }
        public Country GuardianCountryId { get; set; }
        public string FatherState { get; set; }
        public string MotherState { get; set; }
        public string GuardianState { get; set; }
        public string FatherCity { get; set; }
        public string MotherCity { get; set; }
        public string GuardianCity { get; set; }
        public string FatherPincode { get; set; }
        public string MotherPincode { get; set; }
        public string GuardianPincode { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
        public long ParentId { get; set; }
        public string SchoolAddress { get; set; }
        public string Nationality { get; set; }
        public string Category { get; set; }
        public string Country { get; set; }
        public string SchoolLogo { get; set; }
        public long CurrentStudentId { get; set; }
        public long ClassIdForParent { get; set; }
    }

}