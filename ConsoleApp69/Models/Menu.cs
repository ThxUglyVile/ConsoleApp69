using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantProgram
{
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

            if (dish == null)
                Console.WriteLine("Страву не знайдено.");
            else
            {
                Dishes.Remove(dish);
                Console.WriteLine("Страву видалено.");
            }
        }

        public Dish FindDish(string name) =>
            Dishes.FirstOrDefault(d =>
                d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public double GetAveragePrice() =>
            Dishes.Count == 0 ? 0 : Dishes.Average(d => d.Price);
    }
}
