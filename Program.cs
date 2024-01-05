using Cassandra;
using Cassandra.Mapping;
using CLI_CRUD_Cassandra.Category;
using CLI_CRUD_Cassandra.User;
using CLI_CRUD_Cassandra.Transaction;
using CLI_CRUD_Cassandra.CategoryBooks;
using CLI_CRUD_Cassandra.Types;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace CLI_CRUD_Cassandra
{
    class Program
    {

        static void Main()
        {
            var contactPoint = "127.0.0.1";
            var keyspace = "user";


            using (var context = new CassandraContext(contactPoint, keyspace))
            {
                IRepository<UserEntity> userRepository = new UserRepository(context.GetSession());
                var categoryRepository = new CategoryRepository(context.GetSession());
                var transactionRepository = new TransactionRepository(context.GetSession());
                var categoryBooksRepository = new CategoryBooksRepository(context.GetSession());
                var typesRepository = new TypeRepository(context.GetSession());

                while (true)
                {
                    Console.WriteLine("1. Manage Users");
                    Console.WriteLine("2. Manage Categories");
                    Console.WriteLine("3. Manage Transactions");
                    Console.WriteLine("4. Manage CategoriesBooks");
                    Console.WriteLine("5. Manage Types");
                    Console.WriteLine("6. 5 queries");
                    Console.WriteLine("7. Exit");

                    Console.Write("Choose an option: ");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ManageEntity<UserEntity>(userRepository, "User");
                            break;

                        case "2":
                            ManageEntity<CategoryEntity>(categoryRepository, "Category");
                            break;

                        case "3":
                            ManageEntity<TransactionEntity>(transactionRepository, "Transaction");
                            break;
                        case "4":
                            ManageEntity<CategoryBooksEntity>(categoryBooksRepository, "CategoryBook");
                            break;
                        case "5":
                            ManageEntity<TypeEntity>(typesRepository, "Type");
                            break;
                        case "6":
                            QUERY();
                            break;
                        case "7":
                            return;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
        }

        static void QUERY()
        {
            var contactPoint = "127.0.0.1";
            var keyspace = "user";
            using(var context = new CassandraContext(contactPoint, keyspace))
            {
                UserEntity? user = null;
                var _cluster = Cluster.Builder().AddContactPoint(contactPoint).Build();
                var session = _cluster.Connect(keyspace);
                IMapper mapper = new Mapper(session);
                while (user == null)
                {
                    Console.WriteLine("Введите email пользователя");
                    string email = Console.ReadLine();
                    Console.WriteLine("Введите пароль пользователя");
                    string password = Console.ReadLine();
                    // Запрос 1: Получение пользователя с определенным email и password
                    user = mapper.FirstOrDefault<UserEntity>($"SELECT * FROM users WHERE email = '{email}' AND password = '{password}' ALLOW FILTERING");
                    if (user == null)
                    {
                        Console.WriteLine("Такого пользователя нет, проверьте правильность введенных данных и попробуйте войти снова.");
                    }
                }
                Console.WriteLine("Успешно :D");


                // Запрос 2: Получение списка категорий пользователя
                Console.WriteLine("Список ваших категорий:");
                IEnumerable<CategoryEntity> categories = mapper.Fetch<CategoryEntity>($"SELECT * FROM categories WHERE email = '{user.Email}' ALLOW FILTERING");
                List<CategoryEntity> cats_list = categories.ToList();
                foreach (var category in cats_list)
                {
                    Console.WriteLine("\t" + category.Category_Name);
                }

                // Запрос 3: Получение списка категорий пользователя с списком книг
                Console.WriteLine("Список книг в ваших категориях:");
                IEnumerable<IEnumerable<CategoryBooksEntity>> categoryBooks = new List<List<CategoryBooksEntity>>();
                foreach (var category in cats_list)
                {
                    var booksForCategory = mapper.Fetch<CategoryBooksEntity>($"SELECT * FROM category_books WHERE email = '{category.Email}' AND category_name = '{category.Category_Name}' ALLOW FILTERING");
                    List<CategoryBooksEntity> booksList = booksForCategory.ToList();
                    ((List<List<CategoryBooksEntity>>)categoryBooks).Add(booksList);
                    Console.WriteLine($"\tКатегория {category.Category_Name}:");
                    foreach(var bookInCategory in booksList)
                    {
                        Console.WriteLine("\t\t" + bookInCategory.Book_Name);
                    }
                }

                //Запрос 4: Посмотреть все названия типов транзакций
                IEnumerable<TypeEntity> types = mapper.Fetch<TypeEntity>($"SELECT * FROM types ALLOW FILTERING");
                Console.WriteLine("Список типов транзакций:");
                foreach (var type in types)
                {
                    Console.WriteLine($"\t{type.Type}");
                    Console.WriteLine($"\t{type.Description}");
                }


                // Запрос 5: Получение транзакций пользователя
                Console.WriteLine("Введите название интересующего вас типа транзакций, или нажмите ENTER, если хотите посмотреть все транзакции");
                string type_name = Console.ReadLine();
                if (type_name == "")
                {
                    IEnumerable<IEnumerable<TransactionEntity>> transactions = new List<List<TransactionEntity>>();
                    Console.WriteLine("Тип: ");
                    types = mapper.Fetch<TypeEntity>($"SELECT * FROM types ALLOW FILTERING");
                    foreach (var type in types)
                    {
                        var transactiosForType = mapper.Fetch<TransactionEntity>($"SELECT * FROM transactions WHERE email = '{user.Email}' AND type = '{type.Type}' ALLOW FILTERING");
                        ((List<List<TransactionEntity>>)transactions).Add(transactiosForType.ToList());
                        Console.WriteLine($"\t{type.Type}");
                        transactiosForType = mapper.Fetch<TransactionEntity>($"SELECT * FROM transactions WHERE email = '{user.Email}' AND type = '{type.Type}' ALLOW FILTERING");
                        foreach (var transaction in transactiosForType.ToList())
                        {
                            Console.WriteLine($"\t\t{transaction.Category_Name}");
                            Console.WriteLine($"\t\t{transaction.Title}");
                            Console.WriteLine($"\t\t{transaction.Type}");
                            Console.WriteLine($"\t\t{transaction.Amount}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Тип: {type_name}");
                    IEnumerable<TransactionEntity> transactions = mapper.Fetch<TransactionEntity>($"SELECT * FROM transactions WHERE email = '{user.Email}' AND type = '{type_name}' ALLOW FILTERING");
                    foreach( var transaction in transactions)
                    {
                        Console.WriteLine($"\t{transaction.Category_Name}");
                        Console.WriteLine($"\t{transaction.Title}");
                        Console.WriteLine($"\t{transaction.Type}");
                        Console.WriteLine($"\t{transaction.Amount}");
                    }
                }
            }
            
        }

        static void PrintResultSet(RowSet resultSet)
        {
            Console.WriteLine("Query Result:");

            // Assuming the result set is not empty
            var columnDefinitions = resultSet.Columns;

            foreach (var row in resultSet)
            {
                for (int i = 0; i < columnDefinitions.Count(); i++)
                {
                    var columnName = columnDefinitions[i].Name;
                    var columnValue = row[i];
                    Console.WriteLine($"{columnName}: {columnValue}");
                }
                Console.WriteLine("-------------");
            }
        }
        static void ManageEntity<T>(IRepository<T> repository, string entityName) where T : class
        {
            while (true)
            {
                Console.WriteLine($"1. Create {entityName}");
                Console.WriteLine($"2. Read {entityName} by ID");
                Console.WriteLine($"3. Read All {entityName}s");
                Console.WriteLine($"4. Update {entityName}");
                Console.WriteLine($"5. Delete {entityName}");
                Console.WriteLine("6. Back");

                Console.Write($"Choose an option for {entityName}: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateEntity(repository);
                        break;

                    case "2":
                        ReadById(repository);
                        break;

                    case "3":
                        ReadAll(repository);
                        break;

                    case "4":
                        UpdateEntity(repository);
                        break;

                    case "5":
                        DeleteEntity(repository);
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void CreateEntity<T>(IRepository<T> repository) where T : class
        {
            // Add your logic to read and create a new entity
            Console.WriteLine("Creating a new entity...");
            repository.Insert(repository.CreateEntity()); // Adjust this method based on your repository interface
            Console.WriteLine("Entity created.");
        }

        static void ReadById<T>(IRepository<T> repository) where T : class
        {
            // Add your logic to read an entity by ID
            Console.Write("Enter entity ID: ");
            var entityId = Console.ReadLine();
            var entity = repository.GetById(entityId);
            PrintObjectProperties<T>(entity);
            //Console.WriteLine(entity != null ? $"{entityId} - {entity}" : "Entity not found.");
        }

        public static void PrintObjectProperties<T>(T obj)
        {
            if (obj == null)
            {
                Console.WriteLine("Object is null.");
                return;
            }

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            Console.WriteLine($"Properties of {type.Name}:");

            foreach (var property in properties)
            {
                object value = property.GetValue(obj);
                Console.WriteLine($"{property.Name}: {value}");
            }
        }

        static void ReadAll<T>(IRepository<T> repository) where T : class
        {
            // Add your logic to read all entities
            var entities = repository.GetAll();
            foreach (var entity in entities)
            {
                PrintObjectProperties<T>(entity);
            }
        }

        static void UpdateEntity<T>(IRepository<T> repository) where T : class
        {
            // Add your logic to update an entity
            Console.Write("Enter entity ID to update: ");
            var updateId = Console.ReadLine();
            var updateEntity = repository.GetById(updateId);
            if (updateEntity != null)
            {
                Console.WriteLine($"Updating {updateEntity}...");
                repository.Update(updateId);
                Console.WriteLine("Entity updated.");
            }
            else
            {
                Console.WriteLine("Entity not found.");
            }
        }

        static void DeleteEntity<T>(IRepository<T> repository) where T : class
        {
            // Add your logic to delete an entity
            Console.Write("Enter entity ID to delete: ");
            var deleteId = Console.ReadLine();
            repository.Delete(deleteId);
            Console.WriteLine("Entity deleted.");
        }
    }

}