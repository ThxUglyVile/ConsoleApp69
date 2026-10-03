using System;

namespace RestaurantProgram
{
    class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public double OriginalTotalPrice { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }

        public void ShowInformation() =>
            Console.WriteLine(
                $"Замовлення №{Id} | Клієнт: {CustomerName} | {DishName} x{Quantity} | " +
                $"Знижка: {Discount}% | До сплати: {TotalPrice:F2} грн");

        public double CalculateDiscount(double discount) =>
            OriginalTotalPrice - OriginalTotalPrice * discount / 100;

        public void ApplyDiscount(double discount)
        {
            if (discount < 0 || discount > 100)
                return;

            Discount = discount;
            TotalPrice = CalculateDiscount(discount);
        }
    }
}
