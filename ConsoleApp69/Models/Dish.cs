
using System;

namespace RestaurantProgram
{
    public class Dish
    {
        private string name = "";
        private double price;
        private int weight;

        public int Id { get; set; }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва страви не може бути порожньою.");

                name = value.Trim();
            }
        }

        public string Category { get; set; } = "";

        public double Price
        {
            get => price;
            set
            {
                if (double.IsNaN(value) ||
                    double.IsInfinity(value) || value < 0)
                    throw new ArgumentException("Некоректна ціна.");

                price = value;
            }
        }

        public int Weight
        {
            get => weight;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Вага повинна бути більшою за 0.");

                weight = value;
            }
        }

        // Конструктор без параметрів
        public Dish()
        {
        }

        // Конструктор з параметрами
        public Dish(int id, string name, string category,
                    double price, int weight)
        {
            Id = id;
            Name = name;
            Category = category;
            Price = price;
            Weight = weight;
        }

        // Поліморфізм: метод можна перевизначити
        public virtual void ShowInformation()
        {
            Console.WriteLine(
                $"{Id}. {Name} | {Category} | {Price} грн | {Weight} г");
        }

        // Віртуальний метод розрахунку ціни
        public virtual double GetPriceWithDiscount(double discount)
        {
            if (discount < 0 || discount > 100)
                throw new ArgumentException("Знижка повинна бути від 0 до 100%.");

            return Price * (1 - discount / 100);
        }

        // Перевантаження методу
        public double GetPriceWithDiscount(double discount,
                                            double extraDiscount)
        {
            if (extraDiscount < 0 || extraDiscount > 100)
                throw new ArgumentException("Некоректна додаткова знижка.");

            double result = GetPriceWithDiscount(discount);
            return result * (1 - extraDiscount / 100);
        }

        public bool IsExpensive(double limit)
        {
            return Price > limit;
        }

        // Protected: доступний дочірнім класам
        protected virtual string GetDishType()
        {
            return "Звичайна страва";
        }

        public virtual string GetDescription()
        {
            return $"{Name} - {GetDishType()}";
        }
    }
}

