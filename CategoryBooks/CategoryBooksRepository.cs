using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLI_CRUD_Cassandra.CategoryBooks
{
    public class CategoryBooksRepository : IRepository<CategoryBooksEntity>
    {
        private readonly ISession _session;

        public CategoryBooksRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void Insert(CategoryBooksEntity entity)
        {
            var insertCategoryBooksStatement = _session.Prepare("INSERT INTO category_books (id, email, category_name, book_name, created_date, updated_date) VALUES (?, ?, ?, ?, ?, ?)");

            var boundStatement = insertCategoryBooksStatement.Bind(
                Guid.NewGuid(),
                entity.Email,
                entity.Category_Name,
                entity.Book_Name,
                entity.Created_Date,
                entity.Updated_Date
            );

            _session.Execute(boundStatement);
        }

        public CategoryBooksEntity CreateEntity()
        {
            Console.WriteLine("Введите email пользователя");
            Console.WriteLine("Введите название категории");
            Console.WriteLine("Введите название книги");
            string Email = Console.ReadLine();
            string Category_Name = Console.ReadLine();
            string Book_Name = Console.ReadLine();
            CategoryBooksEntity entity = new CategoryBooksEntity()
            {
                Id = Guid.NewGuid(),
                Email = Email,
                Category_Name = Category_Name,
                Book_Name = Book_Name,
                Created_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };
            return entity;
        }

        public CategoryBooksEntity GetById(string id)
        {
            var selectCategoryBooksStatement = _session.Prepare("SELECT id, email, category_name, book_name, created_date, updated_date FROM category_books WHERE id = ?");

            var boundStatement = selectCategoryBooksStatement.Bind(Guid.Parse(id));

            var row = _session.Execute(boundStatement).FirstOrDefault();

            if (row != null)
            {
                return new CategoryBooksEntity
                {
                    Id = row.GetValue<Guid>("id"),
                    Email = row.GetValue<string>("email"),
                    Category_Name = row.GetValue<string>("category_name"),
                    Book_Name = row.GetValue<string>("book_name"),
                    Created_Date = row.GetValue<DateTime>("created_date"),
                    Updated_Date = row.GetValue<DateTime>("updated_date")
                };
            }

            return null;
        }

        public IEnumerable<CategoryBooksEntity> GetAll()
        {
            var selectAllCategoryBooksStatement = _session.Prepare("SELECT id, email, category_name, book_name, created_date, updated_date FROM category_books").Bind();

            var rows = _session.Execute(selectAllCategoryBooksStatement);

            return rows.Select(row => new CategoryBooksEntity
            {
                Id = row.GetValue<Guid>("id"),
                Email = row.GetValue<string>("email"),
                Category_Name = row.GetValue<string>("category_name"),
                Book_Name = row.GetValue<string>("book_name"),
                Created_Date = row.GetValue<DateTime>("created_date"),
                Updated_Date = row.GetValue<DateTime>("updated_date")
            });
        }

        public void Update(string categoryBooks_id)
        {
            var entity = CreateEntity();
            var updateCategoryBooksStatement = _session.Prepare("UPDATE category_books SET email = ?, category_name = ?, book_name = ?, updated_date = ? WHERE id = ?");

            var boundStatement = updateCategoryBooksStatement.Bind(
                entity.Email,
                entity.Category_Name,
                entity.Book_Name,
                DateTime.Now,
                Guid.Parse(categoryBooks_id)
            );

            _session.Execute(boundStatement);
        }

        public void Delete(string id)
        {
            var deleteCategoryBooksStatement = _session.Prepare("DELETE FROM category_books WHERE id = ?");

            var boundStatement = deleteCategoryBooksStatement.Bind(Guid.Parse(id));

            _session.Execute(boundStatement);
        }
    }
}