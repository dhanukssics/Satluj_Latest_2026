using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AssementUpload
    {

        public int ClassId { get; set; }
        public string Filename { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> File_Uploaded_Date { get; set; }

        public int FileId { get; set; }

        public string DivisionName { get; set; }
        public Nullable<int> Division { get; set; }

        public List<FileListDatanew> Data { get; set; }

        public Nullable<int> SubjectId { get; set; }

        public string TeacherName { get; set; }

        public string StudentName { get; set; }

        public int SchoolId { get; set; }

        public int StudentId { get; set; }//newly added on 9/20/2020

    }
    public class FileListDatanew
    {

        public int FileId { get; set; }
        public string FileName { get; set; }

        public Nullable<System.DateTime> File_Uploaded_Date { get; set; }

        public Nullable<int> SubjectId { get; set; }

        public string TeacherName { get; set; }

        public int ClassId { get; set; }

        public int SchoolId { get; set; }

        public string DivisionName { get; set; }


        public string StudentName { get; set; }

        public string Description { get; set; }

    }
}