using System;

namespace RestaurantProgram
{
    class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Weight { get; set; }

        public void ShowInformation() =>
            Console.WriteLine($"{Id}. {Name} | {Category} | {Price} грн | {Weight} г");

        public double GetPriceWithDiscount(double discount) =>
            Price - Price * discount / 100;

        public bool IsExpensive(double limit) => Price > limit;
    }
}
