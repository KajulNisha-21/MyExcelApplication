using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelApplication.Data
{
    public interface IPagination<T> where T: class
    {
        IQueryable<T> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords, out int recordsFiltered);
    }
}
