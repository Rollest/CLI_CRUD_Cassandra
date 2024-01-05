using Cassandra;
using CLI_CRUD_Cassandra.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI_CRUD_Cassandra.User
{
    public class UserRepository : IRepository<UserEntity>
    {
        private readonly ISession _session;

        public UserRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void Insert(UserEntity entity)
        {
            var insertUserStatement = _session.Prepare("INSERT INTO users (id, email, password, created_Date, updated_Date) VALUES (?, ?, ?, ?, ?)");

            var boundStatement = insertUserStatement.Bind(Guid.NewGuid(), entity.Email, entity.Password, entity.Created_Date, entity.Updated_Date);

            _session.Execute(boundStatement);
        }

        public UserEntity CreateEntity()
        {
            Console.WriteLine("Введите Email пользователя");
            Console.WriteLine("Введите пароль");
            string Email = Console.ReadLine();
            string Password = Console.ReadLine();
            UserEntity entity = new UserEntity()
            {
                Email = Email,
                Password = Password,
                Created_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };
            return entity;
        }

        public UserEntity GetById(string email)
        {
            var selectUserStatement = _session.Prepare("SELECT email, password, created_date, updated_date FROM users WHERE email = ?");

            var boundStatement = selectUserStatement.Bind(email);

            var row = _session.Execute(boundStatement).FirstOrDefault();

            if (row != null)
            {
                // Assuming your UserEntity has a constructor that takes id and name
                return new UserEntity {Email = row.GetValue<string>("email"), Password = row.GetValue<string>("password"), Created_Date = row.GetValue<DateTime>("created_date"), Updated_Date = row.GetValue<DateTime>("updated_date") };
            }

            return null;
        }

        public IEnumerable<UserEntity> GetAll()
        {
            var selectAllUsersStatement = _session.Prepare("SELECT email, password, created_date, updated_date FROM users").Bind();

            var rows = _session.Execute(selectAllUsersStatement);

            return rows.Select(row => new UserEntity { Email = row.GetValue<string>("email"), Password = row.GetValue<string>("password"), Created_Date = row.GetValue<DateTime>("created_date"), Updated_Date = row.GetValue<DateTime>("updated_date") });
        }

        public void Update(string email)
        {
            var entity = CreateEntity();
            var updateUserStatement = _session.Prepare("UPDATE users SET email = ?, password = ?, updated_date = ? WHERE email = ?");

            var boundStatement = updateUserStatement.Bind(entity.Email, entity.Password, DateTime.Now);

            _session.Execute(boundStatement);
        }

        public void Delete(string email)
        {
            var deleteUserStatement = _session.Prepare("DELETE FROM users WHERE email = ?");

            var boundStatement = deleteUserStatement.Bind(email);

            _session.Execute(boundStatement);
        }
    }
}
