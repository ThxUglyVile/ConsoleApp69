using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantProgram
{
    // 1. Restaurant
    class Restaurant
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Cuisine { get; set; }
        public int TableCount { get; set; }
        public int EmployeeCount { get; set; }

        public void ShowInformation()
        {
            Console.WriteLine("\n===== РЕСТОРАН =====");
            Console.WriteLine("Назва: " + Name);
            Console.WriteLine("Адреса: " + Address);
            Console.WriteLine("Кухня: " + Cuisine);
            Console.WriteLine("Столиків: " + TableCount);
            Console.WriteLine("Працівників: " + EmployeeCount);
        }

        // Method 1
        public void AddTable()
        {
            TableCount++;
            Console.WriteLine("Кількість столиків збільшено.");
        }

        // Method 2
        public void AddEmployee()
        {
            EmployeeCount++;
            Console.WriteLine("Кількість працівників збільшено.");
        }
    }

    // 2. Employee
    class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public double Salary { get; set; }

        public void ShowInformation()
        {
            Console.WriteLine(
                Id + ". " + Name +
                " | " + Position +
                " | " + Salary + " грн" +
                " | " + Age + " років");
        }

        // Method 1
        public void IncreaseSalary(double amount)
        {
            if (amount > 0)
                Salary += amount;
        }

        // Method 2
        public bool IsAdult()
        {
            return Age >= 18;
        }
    }

    // 3. Dish
    class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Weight { get; set; }

        public void ShowInformation()
        {
            Console.WriteLine(
                Id + ". " + Name +
                " | " + Category +
                " | " + Price + " грн" +
                " | " + Weight + " г");
        }

        // Method 1
        public double GetPriceWithDiscount(double discount)
        {
            if (discount < 0 || discount > 100)
                return Price;

            return Price - Price * discount / 100;
        }

        // Method 2
        public bool IsExpensive(double limit)
        {
            return Price > limit;
        }
    }

    // 4. Menu
    class Menu
    {
        public string Name { get; set; }
        public List<Dish> Dishes { get; set; } = new List<Dish>();

        public void Show()
        {
            Console.WriteLine("\n===== МЕНЮ =====");

            if (Dishes.Count == 0)
            {
                Console.WriteLine("Меню порожнє.");
                return;
            }

            foreach (Dish dish in Dishes)
                dish.ShowInformation();
        }

        public void AddDish(Dish dish)
        {
            Dishes.Add(dish);
            Console.WriteLine("Страву додано.");
        }

        public void RemoveDish(int id)
        {
            Dish dish = Dishes.FirstOrDefault(d => d.Id == id);

            if (dish != null)
            {
                Dishes.Remove(dish);
                Console.WriteLine("Страву видалено.");
            }
            else
            {
                Console.WriteLine("Страву не знайдено.");
            }
        }

        // Method 1
        public Dish FindDish(string name)
        {
            return Dishes.FirstOrDefault(
                d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // Method 2
        public double GetAveragePrice()
        {
            if (Dishes.Count == 0)
                return 0;

            return Dishes.Average(d => d.Price);
        }
    }

    // 5. Customer
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }

        public void ShowInformation()
        {
            Console.WriteLine(
                Id + ". " + Name +
                " | " + Age + " років" +
                " | " + Phone);
        }

        // Method 1
        public bool IsAdult()
        {
            return Age >= 18;
        }

        // Method 2
        public string GetContact()
        {
            return Name + " - " + Phone;
        }
    }

    // 6. Order
    class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }

        public void ShowInformation()
        {
            Console.WriteLine(
                "Замовлення №" + Id +
                " | Клієнт: " + CustomerName +
                " | " + DishName +
                " x" + Quantity +
                " | Знижка: " + Discount + "%" +
                " | До сплати: " + TotalPrice + " грн");
        }

        // Method 1
        public double CalculateDiscount(double discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                return TotalPrice;

            return TotalPrice - TotalPrice * discountPercent / 100;
        }

        // Method 2
        public void ApplyDiscount(double discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                return;

            Discount = discountPercent;
            TotalPrice = CalculateDiscount(discountPercent);
        }
    }

    // 7. Table
    class Table
    {
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsFree { get; set; }
        public string ReservedBy { get; set; }

        public void ShowInformation()
        {
            string status = IsFree ? "Вільний" : "Заброньований";

            Console.WriteLine(
                "Столик №" + Number +
                " | Місць: " + Seats +
                " | " + status +
                (IsFree ? "" : " | Забронював: " + ReservedBy));
        }

        // Method 1
        public bool Reserve(string customerName)
        {
            if (!IsFree)
                return false;

            IsFree = false;
            ReservedBy = customerName;
            return true;
        }

        // Method 2
        public void CancelReservation()
        {
            IsFree = true;
            ReservedBy = "";
        }
    }

    // Main program
    class Program
    {
        static Restaurant restaurant = new Restaurant
        {
            Name = "La Piazza",
            Address = "вул. Центральна, 15",
            Cuisine = "Італійська",
            TableCount = 5,
            EmployeeCount = 2
        };

        static Menu menu = new Menu
        {
            Name = "Основне меню"
        };

        static List<Employee> employees = new List<Employee>();
        static List<Customer> customers = new List<Customer>();
        static List<Order> orders = new List<Order>();
        static List<Table> tables = new List<Table>();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            InitializeData();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("==========================");
                Console.WriteLine("       LA PIAZZA");
                Console.WriteLine("==========================");
                Console.WriteLine("1. Інформація про ресторан");
                Console.WriteLine("2. Показати меню");
                Console.WriteLine("3. Додати страву");
                Console.WriteLine("4. Видалити страву");
                Console.WriteLine("5. Додати клієнта");
                Console.WriteLine("6. Показати клієнтів");
                Console.WriteLine("7. Створити замовлення");
                Console.WriteLine("8. Показати замовлення");
                Console.WriteLine("9. Скасувати замовлення");
                Console.WriteLine("10. Забронювати столик");
                Console.WriteLine("11. Показати столики");
                Console.WriteLine("12. Скасувати бронювання");
                Console.WriteLine("13. Застосувати знижку до замовлення");
                Console.WriteLine("0. Вихід");
                Console.WriteLine("==========================");

                Console.Write("Оберіть дію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        restaurant.ShowInformation();
                        break;

                    case "2":
                        menu.Show();
                        break;

                    case "3":
                        AddDish();
                        break;

                    case "4":
                        RemoveDish();
                        break;

                    case "5":
                        AddCustomer();
                        break;

                    case "6":
                        ShowCustomers();
                        break;

                    case "7":
                        CreateOrder();
                        break;

                    case "8":
                        ShowOrders();
                        break;

                    case "9":
                        CancelOrder();
                        break;

                    case "10":
                        ReserveTable();
                        break;

                    case "11":
                        ShowTables();
                        break;

                    case "12":
                        CancelTableReservation();
                        break;

                    case "13":
                        ApplyDiscountToOrder();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невірний вибір.");
                        break;
                }

                Console.WriteLine("\nНатисніть Enter...");
                Console.ReadLine();
            }
        }

        static void InitializeData()
        {
            menu.AddDish(new Dish
            {
                Id = 1,
                Name = "Маргарита",
                Category = "Піца",
                Price = 250,
                Weight = 500
            });

            menu.AddDish(new Dish
            {
                Id = 2,
                Name = "Пепероні",
                Category = "Піца",
                Price = 300,
                Weight = 550
            });

            menu.AddDish(new Dish
            {
                Id = 3,
                Name = "Карбонара",
                Category = "Паста",
                Price = 220,
                Weight = 350
            });

            customers.Add(new Customer
            {
                Id = 1,
                Name = "Михайло",
                Age = 21,
                Phone = "380502223344"
            });

            employees.Add(new Employee
            {
                Id = 1,
                Name = "Олексій",
                Age = 28,
                Position = "Кухар",
                Salary = 25000
            });

            employees.Add(new Employee
            {
                Id = 2,
                Name = "Анна",
                Age = 25,
                Position = "Офіціант",
                Salary = 18000
            });

            for (int i = 1; i <= 5; i++)
            {
                tables.Add(new Table
                {
                    Number = i,
                    Seats = i % 2 == 0 ? 4 : 2,
                    IsFree = true,
                    ReservedBy = ""
                });
            }
        }

        static void AddDish()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ СТРАВИ =====");

            string name = EnterNonEmptyText("Назва: ");
            string category = EnterNonEmptyText("Категорія: ");
            double price = EnterPositiveNumber("Ціна: ");
            int weight = EnterPositiveInteger("Вага (г): ");

            if (menu.Dishes.Any(d =>
                d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Страва з такою назвою вже існує.");
                return;
            }

            int id = menu.Dishes.Count == 0
                ? 1
                : menu.Dishes.Max(d => d.Id) + 1;

            menu.AddDish(new Dish
            {
                Id = id,
                Name = name,
                Category = category,
                Price = price,
                Weight = weight
            });
        }

        static void RemoveDish()
        {
            menu.Show();

            if (menu.Dishes.Count == 0)
                return;

            int id = EnterPositiveInteger("\nВведіть ID страви: ");
            menu.RemoveDish(id);
        }

        static void AddCustomer()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ КЛІЄНТА =====");

            string name = EnterName("Ім'я: ");
            int age = EnterAge("Вік: ");
            string phone = EnterPhone("Телефон (тільки цифри): ");

            if (customers.Any(c => c.Phone == phone))
            {
                Console.WriteLine("Клієнт з таким номером телефону вже існує.");
                return;
            }

            int id = customers.Count == 0
                ? 1
                : customers.Max(c => c.Id) + 1;

            customers.Add(new Customer
            {
                Id = id,
                Name = name,
                Age = age,
                Phone = phone
            });

            Console.WriteLine("Клієнта додано.");
        }

        static void ShowCustomers()
        {
            Console.WriteLine("\n===== КЛІЄНТИ =====");

            if (customers.Count == 0)
            {
                Console.WriteLine("Клієнтів немає.");
                return;
            }

            foreach (Customer customer in customers)
                customer.ShowInformation();
        }

        static void CreateOrder()
        {
            Console.WriteLine("\n===== СТВОРЕННЯ ЗАМОВЛЕННЯ =====");

            if (customers.Count == 0)
            {
                Console.WriteLine("Немає клієнтів. Спочатку додайте клієнта.");
                return;
            }

            if (menu.Dishes.Count == 0)
            {
                Console.WriteLine("Меню порожнє. Спочатку додайте страву.");
                return;
            }

            ShowCustomers();

            int customerId = EnterPositiveInteger("\nID клієнта: ");
            Customer customer =
                customers.FirstOrDefault(c => c.Id == customerId);

            if (customer == null)
            {
                Console.WriteLine("Клієнта не знайдено.");
                return;
            }

            menu.Show();

            int dishId = EnterPositiveInteger("\nID страви: ");
            Dish dish =
                menu.Dishes.FirstOrDefault(d => d.Id == dishId);

            if (dish == null)
            {
                Console.WriteLine("Страву не знайдено.");
                return;
            }

            int quantity = EnterPositiveInteger("Кількість: ");

            int orderId = orders.Count == 0
                ? 1
                : orders.Max(o => o.Id) + 1;

            orders.Add(new Order
            {
                Id = orderId,
                CustomerName = customer.Name,
                DishName = dish.Name,
                Quantity = quantity,
                TotalPrice = dish.Price * quantity,
                Discount = 0
            });

            Console.WriteLine("Замовлення створено.");
        }

        static void ShowOrders()
        {
            Console.WriteLine("\n===== ЗАМОВЛЕННЯ =====");

            if (orders.Count == 0)
            {
                Console.WriteLine("Замовлень немає.");
                return;
            }

            foreach (Order order in orders)
                order.ShowInformation();
        }

        static void CancelOrder()
        {
            ShowOrders();

            if (orders.Count == 0)
                return;

            int id = EnterPositiveInteger("\nВведіть ID замовлення: ");

            Order orderToRemove =
                orders.FirstOrDefault(o => o.Id == id);

            if (orderToRemove == null)
            {
                Console.WriteLine("Замовлення не знайдено.");
                return;
            }

            orders.Remove(orderToRemove);
            Console.WriteLine("Замовлення скасовано.");
        }

        static void ReserveTable()
        {
            ShowTables();

            int number = EnterPositiveInteger("\nНомер столика: ");

            Table table =
                tables.FirstOrDefault(t => t.Number == number);

            if (table == null)
            {
                Console.WriteLine("Столик не знайдено.");
                return;
            }

            if (!table.IsFree)
            {
                Console.WriteLine(
                    "Столик уже заброньований клієнтом: " +
                    table.ReservedBy);
                return;
            }

            if (customers.Count == 0)
            {
                Console.WriteLine("Спочатку додайте клієнта.");
                return;
            }

            ShowCustomers();

            int customerId = EnterPositiveInteger("\nID клієнта: ");

            Customer customer =
                customers.FirstOrDefault(c => c.Id == customerId);

            if (customer == null)
            {
                Console.WriteLine("Клієнта не знайдено.");
                return;
            }

            if (table.Reserve(customer.Name))
                Console.WriteLine(
                    "Столик №" + table.Number +
                    " заброньовано для " + customer.Name + ".");
        }

        static void ShowTables()
        {
            Console.WriteLine("\n===== СТОЛИКИ =====");

            foreach (Table table in tables)
                table.ShowInformation();
        }

        static void CancelTableReservation()
        {
            ShowTables();

            int number = EnterPositiveInteger(
                "\nНомер столика для скасування бронювання: ");

            Table table =
                tables.FirstOrDefault(t => t.Number == number);

            if (table == null)
            {
                Console.WriteLine("Столик не знайдено.");
                return;
            }

            if (table.IsFree)
            {
                Console.WriteLine("Столик вільний.");
                return;
            }

            Console.WriteLine(
                "Бронювання клієнта " + table.ReservedBy + " скасовано.");

            table.CancelReservation();
        }

        static void ApplyDiscountToOrder()
        {
            ShowOrders();

            if (orders.Count == 0)
                return;

            int id = EnterPositiveInteger("\nID замовлення: ");

            Order order = orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                Console.WriteLine("Замовлення не знайдено.");
                return;
            }

            double discount = EnterDiscount("Знижка (%): ");

            order.ApplyDiscount(discount);

            Console.WriteLine(
                "Знижку " + discount + "% застосовано. " +
                "Нова сума: " + order.TotalPrice + " грн");
        }

        // ===== INPUT METHODS =====

        static string EnterNonEmptyText(string message)
        {
            while (true)
            {
                Console.Write(message);
                string value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();

                Console.WriteLine("Поле не може бути порожнім.");
            }
        }

        static string EnterName(string message)
        {
            while (true)
            {
                Console.Write(message);
                string name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ім'я не може бути порожнім.");
                    continue;
                }

                name = name.Trim();

                bool correct = name.All(c =>
                    char.IsLetter(c) || c == ' ' || c == '-' || c == '\'');

                if (!correct)
                {
                    Console.WriteLine(
                        "Ім'я може містити тільки літери, пробіл, дефіс або апостроф.");
                    continue;
                }

                // First letter must be uppercase.
                if (!char.IsUpper(name[0]))
                {
                    Console.WriteLine(
                        "Перша буква імені повинна бути великою.");
                    continue;
                }

                return name;
            }
        }

        static string EnterPhone(string message)
        {
            while (true)
            {
                Console.Write(message);
                string phone = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(phone))
                {
                    Console.WriteLine("Номер телефону не може бути порожнім.");
                    continue;
                }

                bool onlyDigits = phone.All(char.IsDigit);

                if (!onlyDigits)
                {
                    Console.WriteLine(
                        "Телефон може містити тільки цифри без пробілів, +, дужок та дефісів.");
                    continue;
                }

                if (phone.Length < 10 || phone.Length > 15)
                {
                    Console.WriteLine(
                        "Номер телефону повинен містити від 10 до 15 цифр.");
                    continue;
                }

                return phone;
            }
        }

        static int EnterAge(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                int age;

                if (!int.TryParse(input, out age))
                {
                    Console.WriteLine("Вік повинен бути цілим числом.");
                    continue;
                }

                if (age < 1 || age > 120)
                {
                    Console.WriteLine("Вік повинен бути від 1 до 120 років.");
                    continue;
                }

                return age;
            }
        }

        static int EnterPositiveInteger(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                int number;

                if (!int.TryParse(input, out number) || number <= 0)
                {
                    Console.WriteLine("Введіть ціле число більше 0.");
                    continue;
                }

                return number;
            }
        }

        static double EnterPositiveNumber(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                double number;

                if (!double.TryParse(input, out number) || number <= 0)
                {
                    Console.WriteLine("Введіть число більше 0.");
                    continue;
                }

                return number;
            }
        }

        static double EnterDiscount(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                double discount;

                if (!double.TryParse(input, out discount) ||
                    discount < 0 || discount > 100)
                {
                    Console.WriteLine(
                        "Знижка повинна бути від 0 до 100%.");
                    continue;
                }

                return discount;
            }
        }
    }
}
