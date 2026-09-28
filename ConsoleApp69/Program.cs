using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantProgram
{
    // 1. Ресторан
    class Restaurant
    {
        public string Назва { get; set; }
        public string Адреса { get; set; }
        public string Кухня { get; set; }
        public int КількістьСтоликів { get; set; }
        public int КількістьПрацівників { get; set; }

        public void ПоказатиІнформацію()
        {
            Console.WriteLine("\n===== РЕСТОРАН =====");
            Console.WriteLine("Назва: " + Назва);
            Console.WriteLine("Адреса: " + Адреса);
            Console.WriteLine("Кухня: " + Кухня);
            Console.WriteLine("Столиків: " + КількістьСтоликів);
            Console.WriteLine("Працівників: " + КількістьПрацівників);
        }
    }

    // 2. Працівник
    class Employee
    {
        public int Id { get; set; }
        public string Імя { get; set; }
        public int Вік { get; set; }
        public string Посада { get; set; }
        public double Зарплата { get; set; }

        public void ПоказатиІнформацію()
        {
            Console.WriteLine(
                Id + ". " + Імя +
                " | " + Посада +
                " | " + Зарплата + " грн" +
                " | " + Вік + " років");
        }
    }

    // 3. Страва
    class Dish
    {
        public int Id { get; set; }
        public string Назва { get; set; }
        public string Категорія { get; set; }
        public double Ціна { get; set; }
        public int Вага { get; set; }

        public void ПоказатиІнформацію()
        {
            Console.WriteLine(
                Id + ". " + Назва +
                " | " + Категорія +
                " | " + Ціна + " грн" +
                " | " + Вага + " г");
        }
    }

    // 4. Меню
    class Menu
    {
        public string Назва { get; set; }
        public List<Dish> Страви { get; set; } = new List<Dish>();

        public void Показати()
        {
            Console.WriteLine("\n===== МЕНЮ =====");

            if (Страви.Count == 0)
            {
                Console.WriteLine("Меню порожнє.");
                return;
            }

            foreach (Dish страва in Страви)
                страва.ПоказатиІнформацію();
        }

        public void ДодатиСтраву(Dish страва)
        {
            Страви.Add(страва);
            Console.WriteLine("Страву додано.");
        }

        public void ВидалитиСтраву(int id)
        {
            Dish страва = Страви.FirstOrDefault(d => d.Id == id);

            if (страва != null)
            {
                Страви.Remove(страва);
                Console.WriteLine("Страву видалено.");
            }
            else
            {
                Console.WriteLine("Страву не знайдено.");
            }
        }
    }

    // 5. Клієнт
    class Customer
    {
        public int Id { get; set; }
        public string Імя { get; set; }
        public int Вік { get; set; }
        public string Телефон { get; set; }

        public void ПоказатиІнформацію()
        {
            Console.WriteLine(
                Id + ". " + Імя +
                " | " + Вік + " років" +
                " | " + Телефон);
        }
    }

    // 6. Замовлення
    class Order
    {
        public int Id { get; set; }
        public string ІмяКлієнта { get; set; }
        public string НазваСтрави { get; set; }
        public int Кількість { get; set; }
        public double ЗагальнаЦіна { get; set; }

        public void ПоказатиІнформацію()
        {
            Console.WriteLine(
                "Замовлення №" + Id +
                " | Клієнт: " + ІмяКлієнта +
                " | " + НазваСтрави +
                " x" + Кількість +
                " | " + ЗагальнаЦіна + " грн");
        }
    }

    // 7. Столик
    class Table
    {
        public int Номер { get; set; }
        public int Місця { get; set; }
        public bool Вільний { get; set; }

        public void ПоказатиІнформацію()
        {
            string статус = Вільний ? "Вільний" : "Зайнятий";

            Console.WriteLine(
                "Столик №" + Номер +
                " | Місць: " + Місця +
                " | " + статус);
        }

        public void Забронювати()
        {
            if (Вільний)
            {
                Вільний = false;
                Console.WriteLine("Столик заброньовано.");
            }
            else
            {
                Console.WriteLine("Столик уже зайнятий.");
            }
        }
    }

    // Основна програма
    class Program
    {
        static Restaurant ресторан = new Restaurant
        {
            Назва = "La Piazza",
            Адреса = "вул. Центральна, 15",
            Кухня = "Італійська",
            КількістьСтоликів = 5,
            КількістьПрацівників = 2
        };

        static Menu меню = new Menu
        {
            Назва = "Основне меню"
        };

        static List<Employee> працівники = new List<Employee>();
        static List<Customer> клієнти = new List<Customer>();
        static List<Order> замовлення = new List<Order>();
        static List<Table> столики = new List<Table>();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ІніціалізуватиДані();

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
                Console.WriteLine("0. Вихід");
                Console.WriteLine("==========================");

                Console.Write("Оберіть дію: ");
                string вибір = Console.ReadLine();

                switch (вибір)
                {
                    case "1":
                        ресторан.ПоказатиІнформацію();
                        break;

                    case "2":
                        меню.Показати();
                        break;

                    case "3":
                        ДодатиСтраву();
                        break;

                    case "4":
                        ВидалитиСтраву();
                        break;

                    case "5":
                        ДодатиКлієнта();
                        break;

                    case "6":
                        ПоказатиКлієнтів();
                        break;

                    case "7":
                        СтворитиЗамовлення();
                        break;

                    case "8":
                        ПоказатиЗамовлення();
                        break;

                    case "9":
                        СкасуватиЗамовлення();
                        break;

                    case "10":
                        ЗабронюватиСтолик();
                        break;

                    case "11":
                        ПоказатиСтолики();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невірний вибір. Введіть номер пункту меню від 0 до 11.");
                        break;
                }

                Console.WriteLine("\nНатисніть Enter...");
                Console.ReadLine();
            }
        }

        static void ІніціалізуватиДані()
        {
            меню.ДодатиСтраву(new Dish
            {
                Id = 1,
                Назва = "Маргарита",
                Категорія = "Піца",
                Ціна = 250,
                Вага = 500
            });

            меню.ДодатиСтраву(new Dish
            {
                Id = 2,
                Назва = "Пепероні",
                Категорія = "Піца",
                Ціна = 300,
                Вага = 550
            });

            меню.ДодатиСтраву(new Dish
            {
                Id = 3,
                Назва = "Карбонара",
                Категорія = "Паста",
                Ціна = 220,
                Вага = 350
            });

            клієнти.Add(new Customer
            {
                Id = 1,
                Імя = "Михайло",
                Вік = 21,
                Телефон = "380502223344"
            });

            працівники.Add(new Employee
            {
                Id = 1,
                Імя = "Олексій",
                Вік = 28,
                Посада = "Кухар",
                Зарплата = 25000
            });

            працівники.Add(new Employee
            {
                Id = 2,
                Імя = "Анна",
                Вік = 25,
                Посада = "Офіціант",
                Зарплата = 18000
            });

            for (int i = 1; i <= 5; i++)
            {
                столики.Add(new Table
                {
                    Номер = i,
                    Місця = i % 2 == 0 ? 4 : 2,
                    Вільний = true
                });
            }
        }

        static void ДодатиСтраву()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ СТРАВИ =====");

            string назва = ВвестиНепорожнійТекст("Назва: ");
            string категорія = ВвестиНепорожнійТекст("Категорія: ");
            double ціна = ВвестиДодатнеЧисло("Ціна: ");
            int вага = ВвестиДодатнеЦіле("Вага (г): ");

            if (меню.Страви.Any(d =>
                d.Назва.Equals(назва, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Страва з такою назвою вже існує.");
                return;
            }

            int id = меню.Страви.Count == 0
                ? 1
                : меню.Страви.Max(d => d.Id) + 1;

            меню.ДодатиСтраву(new Dish
            {
                Id = id,
                Назва = назва,
                Категорія = категорія,
                Ціна = ціна,
                Вага = вага
            });
        }

        static void ВидалитиСтраву()
        {
            меню.Показати();

            if (меню.Страви.Count == 0)
                return;

            int id = ВвестиДодатнеЦіле("\nВведіть ID страви: ");
            меню.ВидалитиСтраву(id);
        }

        static void ДодатиКлієнта()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ КЛІЄНТА =====");

            string імʼя = ВвестиІмя("Ім'я: ");
            int вік = ВвестиВік("Вік: ");
            string телефон = ВвестиТелефон("Телефон (тільки цифри): ");

            if (клієнти.Any(c => c.Телефон == телефон))
            {
                Console.WriteLine("Клієнт з таким номером телефону вже існує.");
                return;
            }

            int id = клієнти.Count == 0
                ? 1
                : клієнти.Max(c => c.Id) + 1;

            клієнти.Add(new Customer
            {
                Id = id,
                Імя = імʼя,
                Вік = вік,
                Телефон = телефон
            });

            Console.WriteLine("Клієнта додано.");
        }

        static void ПоказатиКлієнтів()
        {
            Console.WriteLine("\n===== КЛІЄНТИ =====");

            if (клієнти.Count == 0)
            {
                Console.WriteLine("Клієнтів немає.");
                return;
            }

            foreach (Customer клієнт in клієнти)
                клієнт.ПоказатиІнформацію();
        }

        static void СтворитиЗамовлення()
        {
            Console.WriteLine("\n===== СТВОРЕННЯ ЗАМОВЛЕННЯ =====");

            if (клієнти.Count == 0)
            {
                Console.WriteLine("Немає клієнтів. Спочатку додайте клієнта.");
                return;
            }

            if (меню.Страви.Count == 0)
            {
                Console.WriteLine("Меню порожнє. Спочатку додайте страву.");
                return;
            }

            ПоказатиКлієнтів();

            int idКлієнта = ВвестиДодатнеЦіле("\nID клієнта: ");
            Customer клієнт =
                клієнти.FirstOrDefault(c => c.Id == idКлієнта);

            if (клієнт == null)
            {
                Console.WriteLine("Клієнта не знайдено.");
                return;
            }

            меню.Показати();

            int idСтрави = ВвестиДодатнеЦіле("\nID страви: ");
            Dish страва =
                меню.Страви.FirstOrDefault(d => d.Id == idСтрави);

            if (страва == null)
            {
                Console.WriteLine("Страву не знайдено.");
                return;
            }

            int кількість = ВвестиДодатнеЦіле("Кількість: ");

            int idЗамовлення = замовлення.Count == 0
                ? 1
                : замовлення.Max(o => o.Id) + 1;

            замовлення.Add(new Order
            {
                Id = idЗамовлення,
                ІмяКлієнта = клієнт.Імя,
                НазваСтрави = страва.Назва,
                Кількість = кількість,
                ЗагальнаЦіна = страва.Ціна * кількість
            });

            Console.WriteLine("Замовлення створено.");
        }

        static void ПоказатиЗамовлення()
        {
            Console.WriteLine("\n===== ЗАМОВЛЕННЯ =====");

            if (замовлення.Count == 0)
            {
                Console.WriteLine("Замовлень немає.");
                return;
            }

            foreach (Order замовленняItem in замовлення)
                замовленняItem.ПоказатиІнформацію();
        }

        static void СкасуватиЗамовлення()
        {
            ПоказатиЗамовлення();

            if (замовлення.Count == 0)
                return;

            int id = ВвестиДодатнеЦіле("\nВведіть ID замовлення: ");

            Order замовленняДляВидалення =
                замовлення.FirstOrDefault(o => o.Id == id);

            if (замовленняДляВидалення == null)
            {
                Console.WriteLine("Замовлення не знайдено.");
                return;
            }

            замовлення.Remove(замовленняДляВидалення);
            Console.WriteLine("Замовлення скасовано.");
        }

        static void ЗабронюватиСтолик()
        {
            ПоказатиСтолики();

            int номер = ВвестиДодатнеЦіле("\nНомер столика: ");
            Table столик =
                столики.FirstOrDefault(t => t.Номер == номер);

            if (столик == null)
            {
                Console.WriteLine("Столик не знайдено.");
                return;
            }

            столик.Забронювати();
        }

        static void ПоказатиСтолики()
        {
            Console.WriteLine("\n===== СТОЛИКИ =====");

            foreach (Table столик in столики)
                столик.ПоказатиІнформацію();
        }

        // ===== МЕТОДИ ПЕРЕВІРКИ ВВЕДЕННЯ =====

        static string ВвестиНепорожнійТекст(string повідомлення)
        {
            while (true)
            {
                Console.Write(повідомлення);
                string значення = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(значення))
                    return значення.Trim();

                Console.WriteLine("Поле не може бути порожнім.");
            }
        }

        static string ВвестиІмя(string повідомлення)
        {
            while (true)
            {
                Console.Write(повідомлення);
                string імя = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(імя))
                {
                    Console.WriteLine("Ім'я не може бути порожнім.");
                    continue;
                }

                bool правильне = імя.All(c =>
                    char.IsLetter(c) || c == ' ' || c == '-' || c == '\'');

                if (!правильне)
                {
                    Console.WriteLine("Ім'я може містити тільки літери, пробіл, дефіс або апостроф.");
                    continue;
                }

                return імя.Trim();
            }
        }

        static string ВвестиТелефон(string повідомлення)
        {
            while (true)
            {
                Console.Write(повідомлення);
                string телефон = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(телефон))
                {
                    Console.WriteLine("Номер телефону не може бути порожнім.");
                    continue;
                }

                bool тількиЦифри = телефон.All(char.IsDigit);

                if (!тількиЦифри)
                {
                    Console.WriteLine("Телефон може містити тільки цифри без пробілів, +, дужок та дефісів.");
                    continue;
                }

                if (телефон.Length < 10 || телефон.Length > 15)
                {
                    Console.WriteLine("Номер телефону повинен містити від 10 до 15 цифр.");
                    continue;
                }

                return телефон;
            }
        }

        static int ВвестиВік(string повідомлення)
        {
            while (true)
            {
                Console.Write(повідомлення);
                string введення = Console.ReadLine();
                int вік;

                if (!int.TryParse(введення, out вік))
                {
                    Console.WriteLine("Вік повинен бути цілим числом.");
                    continue;
                }

                if (вік < 1 || вік > 120)
                {
                    Console.WriteLine("Вік повинен бути від 1 до 120 років.");
                    continue;
                }

                return вік;
            }
        }

        static int ВвестиДодатнеЦіле(string повідомлення)
        {
            while (true)
            {
                Console.Write(повідомлення);
                string введення = Console.ReadLine();
                int число;

                if (!int.TryParse(введення, out число) || число <= 0)
                {
                    Console.WriteLine("Введіть ціле число більше 0.");
                    continue;
                }

                return число;
            }
        }

        static double ВвестиДодатнеЧисло(string повідомлення)
        {
            while (true)
            {
                Console.Write(повідомлення);
                string введення = Console.ReadLine();
                double число;

                if (!double.TryParse(введення, out число) || число <= 0)
                {
                    Console.WriteLine("Введіть число більше 0.");
                    continue;
                }

                return число;
            }
        }
    }
}
