using MyExcelApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using MyExcelApplication.Data;
using Newtonsoft.Json;

namespace MyExcelApplication.Controllers
{
    public class StudentListController : Controller
    {
        TreeHierrarchyEntities db = new TreeHierrarchyEntities();
        // GET: StudentList
        public ActionResult Index()
        {
            return View();
        }
       
        //insert MULTIPLE records
        public ActionResult InsertStudent(List<StudentList> Students)
        {
            //Check for NULL.
            if (Students == null)
            {
                Students = new List<StudentList>();
            }

            //Loop and insert records.
            //foreach (StudentList stud in Students)
            //{
            //    db.StudentLists.Add(stud);
            //}
            //int insertedRecords = db.SaveChanges();
            //return Json(insertedRecords);


            //new  method
            List<StudentList> studentList = new List<StudentList>();
            foreach (StudentList list in Students)
            {
                studentList.Add(list);
            }
            db.StudentLists.AddRange(studentList);
            int Count = db.SaveChanges();
            return Json(Count);
        }

        //search students from db
        //public ActionResult SearchStudents(string students)
        //{
        //    //var searchRecords = db.StudentLists.Where(s => s.Name == students).ToList();
        //    var searchRecords = db.StudentLists.Where(s => s.Name.Contains(students)|| 
        //    s.Name.StartsWith(students) || s.Name.EndsWith(students)).ToList();
        //    if (searchRecords.Count() == 0)
        //    {
        //        //var JsonResult = Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
        //        return Json(0);
        //    }

        //    return Json(searchRecords.ToList());
        //}

        //to render jquery template search page

        #region pagination
        public ActionResult paginationIndex()
        {
            return View();
        }

        [HttpPost]
        public string paginationIndex(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;
            var students = new Pagination().GetPaginated(search, start.Value, length ?? 10, out totalRecords, out recordsFiltered);
            return JsonConvert.SerializeObject(students);
        }
        #endregion


        public ActionResult TSeacrh()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertExcelData()
        {
            string filePath = string.Empty;
            string conString = string.Empty;
            DataTable dt = new DataTable();

            //Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    //Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file != null)
                        {
                            string path = Server.MapPath("~/Uploads/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            filePath = path + Path.GetFileName(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            file.SaveAs(filePath);
                            switch (extension)
                            {
                                case ".xls": //Excel 97-03.
                                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                    break;
                                case ".xlsx": //Excel 07 and above.
                                    conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                    break;
                            }

                            conString = string.Format(conString, filePath);

                            using (OleDbConnection connExcel = new OleDbConnection(conString))
                            {
                                using (OleDbCommand cmdExcel = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                    {
                                        cmdExcel.Connection = connExcel;

                                        //Get the name of First Sheet.
                                        connExcel.Open();
                                        DataTable dtExcelSchema;
                                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString().Trim();
                                        connExcel.Close();

                                        //Read Data from First Sheet.
                                        connExcel.Open();
                                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                        odaExcel.SelectCommand = cmdExcel;
                                        odaExcel.Fill(dt);
                                        connExcel.Close();
                                    }
                                }
                            }
                            //Insert records to database table.
                            TreeHierrarchyEntities entities = new TreeHierrarchyEntities();
                            foreach (DataRow row in dt.Rows)
                            {
                                entities.StudentLists.Add(new StudentList
                                {
                                    Name = row["Name"].ToString().Trim(),
                                    Marks = Convert.ToInt32(row["Marks"]),
                                    Result = row["Result"].ToString().Trim()
                                });
                            }
                            entities.SaveChanges();
                        }
                    }

                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            return Json("File uploaded successfully");
        }

        public ActionResult TStudentList()
        {
            List<StudentList> student = db.StudentLists.ToList();
            return View(student);
        }

        //trimmed spaces
        public ActionResult SearchStudents(string students)
        {
            //var searchRecords = db.StudentLists.Where(s => s.Name == students).ToList();            
            var searchRecords = db.StudentLists.Where(s => s.Name.Contains(students) ||
            s.Name.StartsWith(students) || s.Name.EndsWith(students)).ToList();
            List<StudentList> student = new List<StudentList>();
            foreach (var x in searchRecords)
            {
                x.RollNo = x.RollNo;
                x.Name = x.Name.ToString().TrimEnd();
                x.Marks = x.Marks;
                x.Result = x.Result.ToString().TrimEnd();
                student.Add(x);
            }


            if (student.Count() == 0)
            {
                //var JsonResult = Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
                return Json(0);
            }

            return Json(student);
        }

        [HttpGet]
        public ActionResult DataList()
        {
            List<StudentList> studentLists = db.StudentLists.ToList();
            List<StudentList> student = new List<StudentList>();
            foreach (var x in studentLists)
            {
                x.RollNo = x.RollNo;
                x.Name = x.Name.ToString().TrimEnd();
                x.Marks = x.Marks;
                x.Result = x.Result.ToString().TrimEnd();
                student.Add(x);
            }
            return Json(student, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Pagination()
        {
            return View();
        }


        //using linq
        //public ActionResult SearchStudents(string students)
        //{
        //    (db.StudentLists.Where(s => s.Name.Contains(students) ||
        //    s.Name.StartsWith(students) || s.Name.EndsWith(students))).ToList().ForEach(row =>
        //    {
        //        row.Name = row.Name.ToString().TrimEnd();
        //        row.RollNo = row.RollNo;
        //        row.Marks = row.Marks;
        //        row.Result = row.Result.ToString().TrimEnd();

        //    });


        //    if (student.Count() == 0)
        //    {
        //        return Json(0);
        //    }

        //    return Json(student);
        //}

        

        #region datatable pagination,searching,sorting,filters
        public ActionResult paginationIndex1()
        {
            return View();
        }
        public ActionResult paginationIndexList()
        {
            var data = db.StudentLists.OrderBy(a => a.Name).ToList();
            return Json(new { data=data},JsonRequestBehavior.AllowGet);
        }
        #endregion
    }



}





#region 
//upload excel file into db
//[HttpPost]
//public ActionResult InsertExcelData()
//{
//    // Checking no of files injected in Request object  
//    if (Request.Files.Count > 0)
//    {
//        try
//        {
//            //  Get all files from Request object  
//            HttpFileCollectionBase files = Request.Files;
//            for (int i = 0; i < files.Count; i++)
//            {

//                HttpPostedFileBase file = files[i];
//                string fname;

//                Stream stream = file.InputStream;

//                if (file.FileName.EndsWith(".xls") || file.FileName.EndsWith(".xlsx"))
//                {
//                    IExcelDataReader excelReader = null;


//                    if (file.FileName.EndsWith(".xls"))
//                    {
//                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
//                    }
//                    else if (file.FileName.EndsWith(".xlsx"))
//                    {
//                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("File", "This file format is not supported");
//                        return View();
//                    }

//                    //excelReader.IsFirstRowAsColumnNames = true;

//                    var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
//                    {
//                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
//                        {
//                            UseHeaderRow = true
//                        }
//                    });
//                    //DataSet result = excelReader.AsDataSet();
//                    excelReader.Close();
//                    DataTable Dtable = result.Tables[0];

//                }


//            }
//            // Returns message that successfully uploaded  
//            return Json("File Uploaded Successfully!");
//        }
//        catch (Exception ex)
//        {
//            return Json("Error occurred. Error details: " + ex.Message);
//        }
//    }
//    else
//    {
//        return Json("No files selected.");
//    }
//}

////template example - static list
//public ActionResult Template()
//{
//    return View();
//}

////template - dynamic
//public ActionResult TStudentList()
//{
//    return View();
//}

//public ActionResult DataList()
//{
//    List<StudentList> students = db.StudentLists.ToList();
//    return Json(students,JsonRequestBehavior.AllowGet);
//}

////practise
//public ActionResult StudentMarkPage()
//{
//    return View();
//}
#endregion