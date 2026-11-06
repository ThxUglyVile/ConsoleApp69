
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantProgram
{
    public class Menu
    {
        // Інкапсуляція
        private string name = "";
        private readonly List<Dish> dishes = new List<Dish>();

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Назва меню не може бути порожньою.");

                name = value.Trim();
            }
        }

        // Список можна змінювати через методи,
        // але не можна замінити іншим списком
        public List<Dish> Dishes
        {
            get => dishes;
        }

        // Конструктори
        public Menu()
        {
        }

        public Menu(string name)
        {
            Name = name;
        }

        // Поліморфізм
        public virtual void Show()
        {
            Console.WriteLine("\n===== МЕНЮ =====");

            if (Dishes.Count == 0)
            {
                Console.WriteLine("Меню порожнє.");
                return;
            }

            foreach (Dish dish in Dishes)
            {
                // Викликається метод конкретного типу
                dish.ShowInformation();
            }
        }

        // Перевантаження методу Show
        public void Show(string category)
        {
            Console.WriteLine(
                $"\n===== КАТЕГОРІЯ: {category} =====");

            var filtered = Dishes.Where(d =>
                d.Category.Equals(
                    category,
                    StringComparison.OrdinalIgnoreCase)).ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine("Страв не знайдено.");
                return;
            }

            foreach (Dish dish in filtered)
                dish.ShowInformation();
        }

        // Додавання страви
        public virtual void AddDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentNullException(nameof(dish));

            if (Dishes.Any(d => d.Id == dish.Id))
            {
                Console.WriteLine(
                    "Страва з таким ID вже існує.");
                return;
            }

            Dishes.Add(dish);
            Console.WriteLine("Страву додано.");
        }

        // Видалення страви
        public virtual void RemoveDish(int id)
        {
            Dish? dish = Dishes.FirstOrDefault(
                d => d.Id == id);

            if (dish == null)
            {
                Console.WriteLine("Страву не знайдено.");
                return;
            }

            Dishes.Remove(dish);
            Console.WriteLine("Страву видалено.");
        }

        // Пошук за назвою
        public Dish? FindDish(string name)
        {
            return Dishes.FirstOrDefault(d =>
                d.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Перевантаження: пошук за ID
        public Dish? FindDish(int id)
        {
            return Dishes.FirstOrDefault(d => d.Id == id);
        }

        // Середня ціна
        public double GetAveragePrice()
        {
            return Dishes.Count == 0
                ? 0
                : Dishes.Average(d => d.Price);
        }

        // Сортування за ціною
        public List<Dish> SortByPrice(bool ascending = true)
        {
            return ascending
                ? Dishes.OrderBy(d => d.Price).ToList()
                : Dishes.OrderByDescending(d => d.Price).ToList();
        }

        // Фільтрація за категорією
        public List<Dish> GetByCategory(string category)
        {
            return Dishes.Where(d =>
                d.Category.Equals(
                    category,
                    StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Пошук найдорожчої страви
        public Dish? GetMostExpensiveDish()
        {
            return Dishes.OrderByDescending(
                d => d.Price).FirstOrDefault();
        }

        // Загальна кількість страв
        public int GetDishCount()
        {
            return Dishes.Count;
        }

        // Перевірка наявності страви
        public bool ContainsDish(int id)
        {
            return Dishes.Any(d => d.Id == id);
        }
    }
}
