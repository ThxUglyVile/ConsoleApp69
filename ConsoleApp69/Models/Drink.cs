
using System;

namespace RestaurantProgram
{
    class Drink : Dish
    {
        private bool isCold;

        public bool IsCold
        {
            get => isCold;
            set => isCold = value;
        }

        public Drink(int id, string name, string category,
                     double price, int weight, bool isCold)
            : base(id, name, category, price, weight)
        {
            IsCold = isCold;
        }


        public override void ShowInformation()
        {
            string temperature = IsCold ? "Холодний" : "Гарячий";

            Console.WriteLine(
                $"{Id}. {Name} | {temperature} | {Price} грн | {Weight} мл");
        }



        public override double GetPriceWithDiscount(double discount)
        {
            double result = base.GetPriceWithDiscount(discount);

            if (IsCold)
                result *= 0.95;

            return result;
        }

        protected override string GetDishType()
        {
            return IsCold ? "Холодний напій" : "Гарячий напій";
        }

        public override string GetDescription()
        {
            return $"{Name} - {GetDishType()}, {Weight} мл";
        }
    }
}
