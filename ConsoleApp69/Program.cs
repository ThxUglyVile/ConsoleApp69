using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantProgram
{

    public enum ConsoleOption
    {
        ShowMenu,
        GetOrder,

    }
    class Program
    {
        static Restaurant restaurant = new Restaurant
        {
            Name = "La Piazza",
            Address = "вул. Центральна, 15",
            Cuisine = "Італійська",
            TableCount = 5
        };

        static Menu menu = new Menu { Name = "Основне меню" };
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
                Console.WriteLine("13. Керування знижкою");
                Console.WriteLine("0. Вихід");
                Console.WriteLine("==========================");

                Console.Write("Оберіть дію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": restaurant.ShowInformation(); Pause(); break;
                    case "2": menu.Show(); Pause(); break;
                    case "3": AddDish(); break;
                    case "4": RemoveDish(); break;
                    case "5": AddCustomer(); break;
                    case "6": ShowCustomers(); Pause(); break;
                    case "7": CreateOrder(); break;
                    case "8": ShowOrders(); Pause(); break;
                    case "9": CancelOrder(); break;
                    case "10": ReserveTable(); break;
                    case "11": ShowTables(); Pause(); break;
                    case "12": CancelTableReservation(); break;
                    case "13": ManageDiscount(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        Pause();
                        break;
                }
            }
        }

        static void InitializeData()
        {
            menu.AddDish(new Dish { Id = 1, Name = "Маргарита", Category = "Піца", Price = 250, Weight = 500 });
            menu.AddDish(new Dish { Id = 2, Name = "Пепероні", Category = "Піца", Price = 300, Weight = 550 });
            menu.AddDish(new Dish { Id = 3, Name = "Карбонара", Category = "Паста", Price = 220, Weight = 350 });

            customers.Add(new Customer
            {
                Id = 1,
                Name = "Михайло",
                Age = 21,
                Phone = "380502223344"
            });

            for (int i = 1; i <= 5; i++)
                tables.Add(new Table
                {
                    Number = i,
                    Seats = i % 2 == 0 ? 4 : 2,
                    IsFree = true,
                    ReservedBy = ""
                });
        }

        static void AddDish()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ СТРАВИ =====");
            Console.WriteLine("0 — повернутися назад");

            string name = EnterText("Назва: ");
            if (name == null) return;

            string category = EnterText("Категорія: ");
            if (category == null) return;

            double price = EnterPositiveNumber("Ціна: ");
            if (price == -1) return;

            int weight = EnterPositiveInteger("Вага (г): ");
            if (weight == -1) return;

            if (menu.Dishes.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Страва з такою назвою вже існує.");
                Pause();
                return;
            }

            int id = menu.Dishes.Count == 0 ? 1 : menu.Dishes.Max(d => d.Id) + 1;
            menu.AddDish(new Dish
            {
                Id = id,
                Name = name,
                Category = category,
                Price = price,
                Weight = weight
            });
            Pause();
        }

        static void RemoveDish()
        {
            menu.Show();
            if (menu.Dishes.Count == 0) { Pause(); return; }

            int id = EnterPositiveInteger("\nID страви (0 — назад): ");
            if (id == -1) return;

            menu.RemoveDish(id);
            Pause();
        }

        static void AddCustomer()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ КЛІЄНТА =====");
            Console.WriteLine("0 — повернутися назад");

            string name = EnterName("Ім'я: ");
            if (name == null) return;

            int age = EnterAge("Вік: ");
            if (age == -1) return;

            string phone = EnterPhone("Телефон: ");
            if (phone == null) return;

            if (customers.Any(c => c.Phone == phone))
            {
                Console.WriteLine("Клієнт з таким номером телефону вже існує.");
                Pause();
                return;
            }

            int id = customers.Count == 0 ? 1 : customers.Max(c => c.Id) + 1;
            customers.Add(new Customer { Id = id, Name = name, Age = age, Phone = phone });
            Console.WriteLine("Клієнта додано.");
            Pause();
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
            if (customers.Count == 0) { Console.WriteLine("Спочатку додайте клієнта."); Pause(); return; }
            if (menu.Dishes.Count == 0) { Console.WriteLine("Меню порожнє."); Pause(); return; }

            ShowCustomers();
            int customerId = EnterPositiveInteger("\nID клієнта (0 — назад): ");
            if (customerId == -1) return;

            Customer customer = customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null) { Console.WriteLine("Клієнта не знайдено."); Pause(); return; }

            menu.Show();
            int dishId = EnterPositiveInteger("\nID страви (0 — назад): ");
            if (dishId == -1) return;

            Dish dish = menu.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish == null) { Console.WriteLine("Страву не знайдено."); Pause(); return; }

            int quantity = EnterPositiveInteger("Кількість (0 — назад): ");
            if (quantity == -1) return;

            double total = dish.Price * quantity;
            int id = orders.Count == 0 ? 1 : orders.Max(o => o.Id) + 1;

            orders.Add(new Order
            {
                Id = id,
                CustomerName = customer.Name,
                DishName = dish.Name,
                Quantity = quantity,
                OriginalTotalPrice = total,
                TotalPrice = total,
                Discount = 0
            });

            Console.WriteLine("Замовлення створено.");
            Pause();
        }

        static void ShowOrders()
        {
            Console.WriteLine("\n===== ЗАМОВЛЕННЯ =====");
            if (orders.Count == 0) { Console.WriteLine("Замовлень немає."); return; }

            foreach (Order order in orders)
                order.ShowInformation();
        }

        static void CancelOrder()
        {
            ShowOrders();
            if (orders.Count == 0) { Pause(); return; }

            int id = EnterPositiveInteger("\nID замовлення (0 — назад): ");
            if (id == -1) return;

            Order order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) Console.WriteLine("Замовлення не знайдено.");
            else { orders.Remove(order); Console.WriteLine("Замовлення скасовано."); }

            Pause();
        }

        static void ReserveTable()
        {
            ShowTables();
            int number = EnterPositiveInteger("\nНомер столика (0 — назад): ");
            if (number == -1) return;

            Table table = tables.FirstOrDefault(t => t.Number == number);
            if (table == null) { Console.WriteLine("Столик не знайдено."); Pause(); return; }

            if (!table.IsFree)
            {
                Console.WriteLine("Столик вже заброньований клієнтом: " + table.ReservedBy);
                Pause();
                return;
            }

            if (customers.Count == 0) { Console.WriteLine("Спочатку додайте клієнта."); Pause(); return; }

            ShowCustomers();
            int customerId = EnterPositiveInteger("\nID клієнта (0 — назад): ");
            if (customerId == -1) return;

            Customer customer = customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null) Console.WriteLine("Клієнта не знайдено.");
            else if (table.Reserve(customer.Name))
                Console.WriteLine($"Столик №{table.Number} заброньовано для {customer.Name}.");

            Pause();
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
            int number = EnterPositiveInteger("\nНомер столика (0 — назад): ");
            if (number == -1) return;

            Table table = tables.FirstOrDefault(t => t.Number == number);
            if (table == null) Console.WriteLine("Столик не знайдено.");
            else if (table.IsFree) Console.WriteLine("Столик вільний.");
            else
            {
                Console.WriteLine($"Бронювання клієнта {table.ReservedBy} скасовано.");
                table.CancelReservation();
            }

            Pause();
        }

        static void ManageDiscount()
        {
            ShowOrders();
            if (orders.Count == 0) { Pause(); return; }

            int id = EnterPositiveInteger("\nID замовлення (0 — назад): ");
            if (id == -1) return;

            Order order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) { Console.WriteLine("Замовлення не знайдено."); Pause(); return; }

            Console.WriteLine("\n1. Встановити знижку");
            Console.WriteLine("2. Збільшити знижку");
            Console.WriteLine("3. Зменшити знижку");
            Console.WriteLine("4. Повністю скасувати знижку");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть дію: ");

            string choice = Console.ReadLine();
            if (choice == "0") return;

            if (choice == "4")
            {
                order.ApplyDiscount(0);
                Console.WriteLine("Знижку повністю скасовано.");
            }
            else if (choice == "1" || choice == "2" || choice == "3")
            {
                double value = EnterDiscount("Відсоток (0 — назад): ");
                if (value == -1) return;

                double newDiscount = choice == "1" ? value :
                    choice == "2" ? order.Discount + value : order.Discount - value;

                if (newDiscount < 0 || newDiscount > 100)
                {
                    Console.WriteLine("Знижка повинна бути від 0 до 100%.");
                }
                else
                {
                    order.ApplyDiscount(newDiscount);
                    Console.WriteLine($"Знижка: {order.Discount}%. До сплати: {order.TotalPrice:F2} грн");
                }
            }
            else Console.WriteLine("Невірний вибір.");

            Pause();
        }

        // Текст: тільки літери та пробіли, перша буква велика.
        static string EnterText(string message)
        {
            while (true)
            {
                Console.Write(message);
                string value = Console.ReadLine()?.Trim();

                if (value == "0") return null;
                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("Поле не може бути порожнім.");
                    continue;
                }

                bool lettersOnly = value.All(c => char.IsLetter(c) || c == ' ');
                if (!lettersOnly)
                {
                    Console.WriteLine("Можна вводити тільки текст.");
                    continue;
                }

                if (!char.IsUpper(value[0]))
                {
                    Console.WriteLine("Перша буква повинна бути великою.");
                    continue;
                }

                return value;
            }
        }

        static string EnterName(string message) => EnterText(message);

        static string EnterPhone(string message)
        {
            while (true)
            {
                Console.Write(message);
                string phone = Console.ReadLine()?.Trim();

                if (phone == "0") return null;
                if (phone.All(char.IsDigit) && phone.Length >= 10 && phone.Length <= 15)
                    return phone;

                Console.WriteLine("Телефон: тільки 10–15 цифр.");
            }
        }

        static int EnterAge(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (input == "0") return -1;
                if (int.TryParse(input, out int age) && age >= 1 && age <= 120)
                    return age;

                Console.WriteLine("Введіть вік від 1 до 120.");
            }
        }

        static int EnterPositiveInteger(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (input == "0") return -1;
                if (int.TryParse(input, out int number) && number > 0)
                    return number;

                Console.WriteLine("Введіть ціле число більше 0.");
            }
        }

        static double EnterPositiveNumber(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (input == "0") return -1;
                if (double.TryParse(input, out double number) && number > 0)
                    return number;

                Console.WriteLine("Введіть число більше 0.");
            }
        }

        static double EnterDiscount(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (input == "0") return -1;
                if (double.TryParse(input, out double discount) &&
                    discount >= 0 && discount <= 100)
                    return discount;

                Console.WriteLine("Введіть відсоток від 0 до 100.");
            }
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть Enter...");
            Console.ReadLine();
        }
    }
}
