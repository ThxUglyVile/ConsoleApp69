using System;

namespace RestaurantProgram
{
    class Restaurant
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Cuisine { get; set; }
        public int TableCount { get; set; }

        public void ShowInformation()
        {
            Console.WriteLine("\n===== РЕСТОРАН =====");
            Console.WriteLine("Назва: " + Name);
            Console.WriteLine("Адреса: " + Address);
            Console.WriteLine("Кухня: " + Cuisine);
            Console.WriteLine("Столиків: " + TableCount);
        }

        public void AddTable() => TableCount++;
    }
} 