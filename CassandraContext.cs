using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CLI_CRUD_Cassandra
{
    public class CassandraContext : IDisposable
    {
        private Cluster _cluster;
        private ISession _session;

        public CassandraContext(string contactPoint, string keyspace)
        {
            _cluster = Cluster.Builder().AddContactPoint(contactPoint).Build();
            _session = _cluster.Connect();
            _session.Execute("CREATE KEYSPACE IF NOT EXISTS User WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1}");
            _session = _cluster.Connect(keyspace);
            _session.Execute("CREATE TABLE IF NOT EXISTS users(id UUID, email TEXT, password TEXT, created_date TIMESTAMP, updated_date TIMESTAMP, " +
                "PRIMARY KEY(email, created_date));");
            //_session.Execute("CREATE INDEX IF NOT EXISTS ON users(email);");
            _session.Execute(@"
            CREATE TABLE IF NOT EXISTS categories (
                id UUID,
                email TEXT,
                category_name TEXT,
                created_date TIMESTAMP,
                updated_date TIMESTAMP,
                PRIMARY KEY(email, created_date) 
            )");
            //_session.Execute("CREATE INDEX IF NOT EXISTS ON categories(email);");
            _session.Execute(@"
            CREATE TABLE IF NOT EXISTS transactions (
                id UUID,
                email TEXT,
                category_name TEXT,
                title TEXT,
                type TEXT,
                amount FLOAT,
                created_date TIMESTAMP,
                updated_date TIMESTAMP,
                PRIMARY KEY((email, type), created_date)
            )");
            _session.Execute("CREATE INDEX IF NOT EXISTS ON transactions(email);");
            _session.Execute("CREATE INDEX IF NOT EXISTS ON transactions(type);");
            _session.Execute(@"
            CREATE TABLE IF NOT EXISTS category_books (
                id UUID,
                email TEXT,
                category_name TEXT,
                book_name TEXT,
                created_date TIMESTAMP,
                updated_date TIMESTAMP,
                PRIMARY KEY((email, category_name), created_date)
            )");
            _session.Execute("CREATE INDEX IF NOT EXISTS ON category_books(email);");
            _session.Execute("CREATE INDEX IF NOT EXISTS ON category_books(category_name);");
            _session.Execute(@"
            CREATE TABLE IF NOT EXISTS types (
                id UUID,
                type TEXT,
                description TEXT,
                created_date TIMESTAMP,
                updated_date TIMESTAMP,
                PRIMARY KEY(type, created_date)
            )");
            //_session.Execute("CREATE INDEX IF NOT EXISTS ON types(type);");

        }

        public ISession GetSession()
        {
            return _session;
        }

        public void Dispose()
        {
            _session.Dispose();
            _cluster.Dispose();
        }
    }
}
