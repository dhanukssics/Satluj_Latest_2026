using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ExamResultMainModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public long ScholasticAreaId { get; set; }
        public long CoScholasticAreaId { get; set; }
        public long UserId { get; set; }
        public List<SelectListItem> ClassList { get; set; }
        public List<SelectListItem> ClassList_wise { get; set; }




        //18-sep-2020 Jibin Start..........................................


        public string SubjectName { get; set; }
            public string TeacherName { get; set; }
            public long TeacherId { get; set; }
            public int FileId { get; set; }

            public string Description { get; set; }
            public long divisionId { get; set; }
            public int StudentId { get; set; }
            public List<FileListData> Data { get; set; }
        public List<SelectListItem> SubjectList { get; set; }
        public List<SelectListItem> OptionalSubjectList { get; set; }


    }

    public class FileListData
    {

        public int FileId { get; set; }
        public long SchoolId { get; set; }

        public string SubjectName { get; set; }
        public string TeacherName { get; set; }

        public long UserId { get; set; }
        public long ClassId { get; set; }
        public long ExamId { get; set; }
        public long divisionId { get; set; }

        public long TeacherId { get; set; }

    }

    //18-sep-2020 Jibin End..........................................


}