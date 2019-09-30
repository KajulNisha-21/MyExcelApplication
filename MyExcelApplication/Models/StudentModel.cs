using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyExcelApplication.Models
{
    public class StudentModel
    {
        public List<StudentList> students { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
    }
}