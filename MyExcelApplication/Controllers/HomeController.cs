using MyExcelApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyExcelApplication.Controllers
{
    public class HomeController : Controller
    {
        TreeHierrarchyEntities db = new TreeHierrarchyEntities();
        //Home page
        public ActionResult Index()
        {
            return View();
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

        //get all students list
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
            if (student.Count() == 0)
            {
                //var JsonResult = Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(student, JsonRequestBehavior.AllowGet);
        }

        
        //upload excel data
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

        //to get datatable
        public ActionResult paginationIndexList()
        {
            var data = db.StudentLists.OrderBy(a => a.Name).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        // StudentList marklist web app page
        public ActionResult StudentMarkListIndex()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}