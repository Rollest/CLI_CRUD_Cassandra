using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLI_CRUD_Cassandra.Transaction
{
    public class TransactionRepository : IRepository<TransactionEntity>
    {
        private readonly ISession _session;

        public TransactionRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void Insert(TransactionEntity entity)
        {
            var insertTransactionStatement = _session.Prepare("INSERT INTO transactions (id, email, category_name, title, type, amount, created_date, updated_date) VALUES (?, ?, ?, ?, ?, ?, ?, ?)");

            var boundStatement = insertTransactionStatement.Bind(
                Guid.NewGuid(),
                entity.Email,
                entity.Category_Name,
                entity.Title,
                entity.Type,
                entity.Amount,
                entity.Created_Date,
                entity.Updated_Date
            );

            _session.Execute(boundStatement);
        }

        public TransactionEntity CreateEntity()
        {
            Console.WriteLine("Введите email пользователя");
            Console.WriteLine("Введите название категории");
            Console.WriteLine("Введите название транзакции");
            Console.WriteLine("Введите тип транзакции");
            Console.WriteLine("Введите сумму транзакции");
            string Email = Console.ReadLine();
            string Category_Name = Console.ReadLine();
            string Title = Console.ReadLine();
            string Type = Console.ReadLine();
            string Amount = Console.ReadLine();
            TransactionEntity entity = new TransactionEntity()
            {
                Id = Guid.NewGuid(),
                Email = Email,
                Category_Name = Category_Name,
                Title = Title,
                Type = Type,
                Amount = float.Parse(Amount),
                Created_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };
            return entity;
        }

        public TransactionEntity GetById(string id)
        {
            var selectTransactionStatement = _session.Prepare("SELECT id, email, category_name, title, type, amount, created_date, updated_date FROM transactions WHERE id = ?");

            var boundStatement = selectTransactionStatement.Bind(Guid.Parse(id));

            var row = _session.Execute(boundStatement).FirstOrDefault();

            if (row != null)
            {
                return new TransactionEntity
                {
                    Id = row.GetValue<Guid>("id"),
                    Email = row.GetValue<string>("email"),
                    Category_Name = row.GetValue<string>("category_name"),
                    Title = row.GetValue<string>("title"),
                    Type = row.GetValue<string>("type"),
                    Amount = row.GetValue<float>("amount"),
                    Created_Date = row.GetValue<DateTime>("created_date"),
                    Updated_Date = row.GetValue<DateTime>("updated_date")
                };
            }

            return null;
        }

        public IEnumerable<TransactionEntity> GetAll()
        {
            var selectAllTransactionsStatement = _session.Prepare("SELECT id, email, category_name, title, type, amount, created_date, updated_date FROM transactions").Bind();

            var rows = _session.Execute(selectAllTransactionsStatement);

            return rows.Select(row => new TransactionEntity
            {
                Id = row.GetValue<Guid>("id"),
                Email = row.GetValue<string>("email"),
                Category_Name = row.GetValue<string>("category_name"),
                Title = row.GetValue<string>("title"),
                Type = row.GetValue<string>("type"),
                Amount = row.GetValue<float>("amount"),
                Created_Date = row.GetValue<DateTime>("created_date"),
                Updated_Date = row.GetValue<DateTime>("updated_date")
            });
        }

        public void Update(string id)
        {
            var entity = CreateEntity();
            var updateTransactionStatement = _session.Prepare("UPDATE transactions SET email = ?, category_name = ?, title = ?, type = ?, amount = ?, updated_date = ? WHERE id = ?");

            var boundStatement = updateTransactionStatement.Bind(
                entity.Email,
                entity.Category_Name,
                entity.Title,
                entity.Type,
                entity.Amount,
                Guid.Parse(id)
            );

            _session.Execute(boundStatement);
        }

        public void Delete(string id)
        {
            var deleteTransactionStatement = _session.Prepare("DELETE FROM transactions WHERE id = ?");

            var boundStatement = deleteTransactionStatement.Bind(Guid.Parse(id));

            _session.Execute(boundStatement);
        }
    }
}