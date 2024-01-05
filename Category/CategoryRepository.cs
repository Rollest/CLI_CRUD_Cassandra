using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI_CRUD_Cassandra.Category
{
    public class CategoryRepository : IRepository<CategoryEntity>
    {
        private readonly ISession _session;

        public CategoryRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void Insert(CategoryEntity entity)
        {
            var insertCategoryStatement = _session.Prepare("INSERT INTO categories (id, email, category_name, created_date, updated_date) VALUES (?, ?, ?, ?, ?)");

            var boundStatement = insertCategoryStatement.Bind(
                entity.Id,
                entity.Email,
                entity.Category_Name,
                entity.Created_Date,
                entity.Updated_Date
            );

            _session.Execute(boundStatement);
        }
        public CategoryEntity CreateEntity()
        {
            Console.WriteLine("Введите название категории");
            Console.WriteLine("Введите email пользователя");
            string Category_name = Console.ReadLine();
            string email = Console.ReadLine();
            CategoryEntity entity = new CategoryEntity()
            {
                Id = Guid.NewGuid(),
                Category_Name = Category_name,
                Email = email,
                Created_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };
            return entity;
        }

        public CategoryEntity GetById(string id)
        {
            var selectCategoryStatement = _session.Prepare("SELECT id, email, category_name, created_date, updated_date FROM categories WHERE id = ?");

            var boundStatement = selectCategoryStatement.Bind(Guid.Parse(id));

            var row = _session.Execute(boundStatement).FirstOrDefault();

            if (row != null)
            {
                return new CategoryEntity
                {
                    Id = row.GetValue<Guid>("id"),
                    Email = row.GetValue<string>("email"),
                    Category_Name = row.GetValue<string>("category_name"),
                    Created_Date = row.GetValue<DateTime>("created_date"),
                    Updated_Date = row.GetValue<DateTime>("updated_date")
                };
            }

            return null;
        }

        public IEnumerable<CategoryEntity> GetAll()
        {
            var selectAllCategoriesStatement = _session.Prepare("SELECT id, email, category_name, created_date, updated_date FROM categories").Bind();

            var rows = _session.Execute(selectAllCategoriesStatement);

            return rows.Select(row => new CategoryEntity
            {
                Id = row.GetValue<Guid>("id"),
                Email = row.GetValue<string>("email"),
                Category_Name = row.GetValue<string>("category_name"),
                Created_Date = row.GetValue<DateTime>("created_date"),
                Updated_Date = row.GetValue<DateTime>("updated_date")
            });
        }

        public void Update(string category_id)
        {
            var entity = CreateEntity();
            var updateCategoryStatement = _session.Prepare("UPDATE categories SET email = ?, category_name = ?, updated_date = ? WHERE id = ?");

            var boundStatement = updateCategoryStatement.Bind(
                entity.Email,
                entity.Category_Name,
                DateTime.Now,
                Guid.Parse(category_id)
            );

            _session.Execute(boundStatement);
        }

        public void Delete(string id)
        {
            var deleteCategoryStatement = _session.Prepare("DELETE FROM categories WHERE id = ?");

            var boundStatement = deleteCategoryStatement.Bind(Guid.Parse(id));

            _session.Execute(boundStatement);
        }
    }
}
