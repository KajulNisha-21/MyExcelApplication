using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyExcelApplication.Models;

namespace MyExcelApplication.Data
{
    public class Pagination : IPagination<StudentList>
    {
        TreeHierrarchyEntities db = new TreeHierrarchyEntities();

        public IQueryable<StudentList> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords, out int recordsFiltered)
        {
            var data = db.StudentLists.AsQueryable();
            totalRecords = data.Count();
            if(!string.IsNullOrEmpty(filter))
            {
                data = data.Where(x=>x.Name.ToUpper().Contains(filter.ToUpper()));
            }

            recordsFiltered = data.Count();
            data = data.OrderBy(x => x.Name).Skip(initialPage * pageSize).Take(pageSize);
            return data;
        }
    }
}