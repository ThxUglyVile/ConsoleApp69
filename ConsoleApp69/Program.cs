
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
                        Console.WriteLine("Невірний вибір.");
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
                Телефон = "+380 50 222 33 44"
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

            Console.Write("Назва: ");
            string назва = Console.ReadLine();

            Console.Write("Категорія: ");
            string категорія = Console.ReadLine();

            Console.Write("Ціна: ");
            double ціна;

            if (!double.TryParse(Console.ReadLine(), out ціна))
            {
                Console.WriteLine("Невірна ціна.");
                return;
            }

            Console.Write("Вага: ");
            int вага;

            if (!int.TryParse(Console.ReadLine(), out вага))
            {
                Console.WriteLine("Невірна вагааааа тест зміненно.");
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

            Console.Write("\nВведіть ID страви: ");
            int id;

            if (int.TryParse(Console.ReadLine(), out id))
                меню.ВидалитиСтраву(id);
            else
                Console.WriteLine("Невірний ID.");
        }

        static void ДодатиКлієнта()
        {
            Console.WriteLine("\n===== ДОДАВАННЯ КЛІЄНТА =====");

            Console.Write("Ім'я: ");
            string імʼя = Console.ReadLine();

            Console.Write("Вік: ");
            int вік;

            if (!int.TryParse(Console.ReadLine(), out вік))
            {
                Console.WriteLine("Невірний вік.");
                return;
            }

            Console.Write("Телефон: ");
            string телефон = Console.ReadLine();

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

            foreach (Customer клієнт in клієнти)
                клієнт.ПоказатиІнформацію();
        }

        static void СтворитиЗамовлення()
        {
            Console.WriteLine("\n===== СТВОРЕННЯ ЗАМОВЛЕННЯ =====");

            ПоказатиКлієнтів();

            Console.Write("\nID клієнта: ");
            int idКлієнта;

            if (!int.TryParse(Console.ReadLine(), out idКлієнта))
            {
                Console.WriteLine("Невірний ID.");
                return;
            }

            Customer клієнт =
                клієнти.FirstOrDefault(c => c.Id == idКлієнта);

            if (клієнт == null)
            {
                Console.WriteLine("Клієнта не знайдено.");
                return;
            }

            меню.Показати();

            Console.Write("\nID страви: ");
            int idСтрави;

            if (!int.TryParse(Console.ReadLine(), out idСтрави))
            {
                Console.WriteLine("Невірний ID.");
                return;
            }

            Dish страва =
                меню.Страви.FirstOrDefault(d => d.Id == idСтрави);

            if (страва == null)
            {
                Console.WriteLine("Страву не знайдено.");
                return;
            }

            Console.Write("Кількість: ");
            int кількість;

            if (!int.TryParse(Console.ReadLine(), out кількість) ||
                кількість <= 0)
            {
                Console.WriteLine("Невірна кількість.");
                return;
            }

            int idЗамовлення = замовлення.Count + 1;

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

            foreach (Order замовлення in Program.замовлення)
                замовлення.ПоказатиІнформацію();
        }

        static void СкасуватиЗамовлення()
        {
            ПоказатиЗамовлення();

            if (замовлення.Count == 0)
                return;

            Console.Write("\nВведіть ID замовлення: ");
            int id;

            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Невірний ID.");
                return;
            }

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

            Console.Write("\nНомер столика: ");
            int номер;

            if (!int.TryParse(Console.ReadLine(), out номер))
            {
                Console.WriteLine("Невірний номер.");
                return;
            }

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
    }
}
