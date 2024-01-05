using Cassandra;
using CLI_CRUD_Cassandra.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLI_CRUD_Cassandra.Types
{
    public class TypeRepository : IRepository<TypeEntity>
    {
        private readonly ISession _session;

        public TypeRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void Insert(TypeEntity entity)
        {
            var insertTypeStatement = _session.Prepare("INSERT INTO types (id, type, description, created_date, updated_date) VALUES (?, ?, ?, ?, ?)");

            var boundStatement = insertTypeStatement.Bind(entity.Id, entity.Type, entity.Description, entity.Created_Date, entity.Updated_Date);

            _session.Execute(boundStatement);
        }

        public TypeEntity CreateEntity()
        {
            Console.WriteLine("Введите тип");
            Console.WriteLine("Введите описание");
            string Type = Console.ReadLine();
            string Description = Console.ReadLine();
            TypeEntity entity = new TypeEntity()
            {
                Id = Guid.NewGuid(),
                Type = Type,
                Description = Description,
                Created_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };
            return entity;
        }

        public TypeEntity GetById(string id)
        {
            var selectTypeStatement = _session.Prepare("SELECT type, description, created_date, updated_date FROM types WHERE id = ?");

            var boundStatement = selectTypeStatement.Bind(Guid.Parse(id));

            var row = _session.Execute(boundStatement).FirstOrDefault();

            if (row != null)
            {
                return new TypeEntity
                {
                    Type = row.GetValue<string>("type"),
                    Description = row.GetValue<string>("description"),
                    Created_Date = row.GetValue<DateTime>("created_date"),
                    Updated_Date = row.GetValue<DateTime>("updated_date")
                };
            }

            return null;
        }

        public IEnumerable<TypeEntity> GetAll()
        {
            var selectAllTypesStatement = _session.Prepare("SELECT id, type, description, created_date, updated_date FROM types").Bind();

            var rows = _session.Execute(selectAllTypesStatement);

            return rows.Select(row => new TypeEntity
            {
                Id = row.GetValue<Guid>("id"),
                Type = row.GetValue<string>("type"),
                Description = row.GetValue<string>("description"),
                Created_Date = row.GetValue<DateTime>("created_date"),
                Updated_Date = row.GetValue<DateTime>("updated_date")
            });
        }

        public void Update(string id)
        {
            var entity = CreateEntity();
            var updateTypeStatement = _session.Prepare("UPDATE types SET description = ?, updated_date = ? WHERE id = ?");

            var boundStatement = updateTypeStatement.Bind(entity.Description, DateTime.Now, id);

            _session.Execute(boundStatement);
        }

        public void Delete(string type)
        {
            var deleteTypeStatement = _session.Prepare("DELETE FROM types WHERE type = ?");

            var boundStatement = deleteTypeStatement.Bind(type);

            _session.Execute(boundStatement);
        }
    }
}