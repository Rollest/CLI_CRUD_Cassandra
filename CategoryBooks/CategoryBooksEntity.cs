using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI_CRUD_Cassandra.CategoryBooks
{
    public class CategoryBooksEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Category_Name { get; set; }
        public string Book_Name { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime Updated_Date { get; set; }
    }
}
