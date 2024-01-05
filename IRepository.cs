using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI_CRUD_Cassandra
{
    public interface IRepository<T> where T : class
    {
        void Insert(T entity);
        T GetById(string id);
        IEnumerable<T> GetAll();
        void Update(string id);
        void Delete(string id);
        T CreateEntity();
    }
}
