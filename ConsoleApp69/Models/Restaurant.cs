
using System;

namespace RestaurantProgram
{
    class Restaurant
    {
        // Інкапсуляція: закриті поля
        private string name = "";
        private string address = "";
        private string cuisine = "";
        private int tableCount;

        // Константа
        private const int MaxTables = 100;

        // Властивості з перевірками
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Назва ресторану не може бути порожньою.");

                name = value.Trim();
            }
        }

        public string Address
        {
            get => address;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Адреса не може бути порожньою.");

                address = value.Trim();
            }
        }

        public string Cuisine
        {
            get => cuisine;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Тип кухні не може бути порожнім.");

                cuisine = value.Trim();
            }
        }

        public int TableCount
        {
            get => tableCount;
            set
            {
                if (value < 0 || value > MaxTables)
                    throw new ArgumentException(
                        $"Кількість столиків повинна бути від 0 до {MaxTables}.");

                tableCount = value;
            }
        }

        // Конструктор без параметрів
        public Restaurant()
        {
        }

        // Конструктор з параметрами
        public Restaurant(string name, string address,
                          string cuisine, int tableCount)
        {
            Name = name;
            Address = address;
            Cuisine = cuisine;
            TableCount = tableCount;
        }

        // Віртуальний метод
        public virtual void ShowInformation()
        {
            Console.WriteLine("\n===== РЕСТОРАН =====");
            Console.WriteLine($"Назва: {Name}");
            Console.WriteLine($"Адреса: {Address}");
            Console.WriteLine($"Кухня: {Cuisine}");
            Console.WriteLine($"Столиків: {TableCount}");
            Console.WriteLine($"Максимум гостей: {GetMaxGuests()}");
        }

        // Перевантаження методу
        public void ShowInformation(bool detailed)
        {
            if (detailed)
            {
                ShowInformation();
                Console.WriteLine($"Вільних місць за максимальною місткістю: {GetMaxGuests()}");
            }
            else
            {
                Console.WriteLine($"{Name} | {Cuisine}");
            }
        }

        // Додавання одного столика
        public virtual void AddTable()
        {
            AddTable(1);
        }

        // Перевантаження: додавання декількох столиків
        public virtual void AddTable(int count)
        {
            if (count <= 0)
                throw new ArgumentException(
                    "Кількість повинна бути більшою за 0.");

            if (count > MaxTables - TableCount)
                throw new InvalidOperationException(
                    "Перевищено максимальну кількість столиків.");

            TableCount += count;
        }

        // Видалення столиків
        public virtual void RemoveTable(int count = 1)
        {
            if (count <= 0 || count > TableCount)
                throw new ArgumentException(
                    "Некоректна кількість столиків.");

            TableCount -= count;
        }

        // Розрахунок максимальної місткості
        public virtual int GetMaxGuests(int seatsPerTable = 4)
        {
            if (seatsPerTable <= 0)
                throw new ArgumentException(
                    "Кількість місць повинна бути більшою за 0.");

            return TableCount * seatsPerTable;
        }

        // Перевірка можливості додати столики
        public bool CanAddTables(int count)
        {
            return count > 0 && count <= MaxTables - TableCount;
        }

        // Перевірка наявності столиків
        public bool HasTables()
        {
            return TableCount > 0;
        }

        // Захищений метод для майбутніх нащадків
        protected virtual string GetRestaurantType()
        {
            return "Звичайний ресторан";
        }

        public virtual string GetDescription()
        {
            return $"{Name} - {GetRestaurantType()}, кухня: {Cuisine}";
        }
    }
}