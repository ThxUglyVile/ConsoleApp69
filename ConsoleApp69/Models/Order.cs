
using System;

namespace RestaurantProgram
{
    public class Order
    {
        // Інкапсуляція
        private int quantity = 1;
        private double originalTotalPrice;
        private double totalPrice;
        private double discount;

        // Дані оплати
        private IPaymentMethod? paymentMethod;
        private bool isPaid = false;

        public int Id { get; set; }

        public string CustomerName { get; set; } = "";
        public string DishName { get; set; } = "";

        public int Quantity
        {
            get => quantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException(
                        "Кількість повинна бути більше нуля.");

                quantity = value;
            }
        }

        public double OriginalTotalPrice
        {
            get => originalTotalPrice;
            set
            {
                ValidatePrice(value);
                originalTotalPrice = value;
                TotalPrice = CalculateDiscount(Discount);
            }
        }

        public double TotalPrice
        {
            get => totalPrice;
            set
            {
                ValidatePrice(value);
                totalPrice = value;
            }
        }

        public double Discount
        {
            get => discount;
            set
            {
                if (double.IsNaN(value) ||
                    double.IsInfinity(value) ||
                    value < 0 || value > 100)
                {
                    throw new ArgumentException(
                        "Знижка повинна бути від 0 до 100.");
                }

                discount = value;
            }
        }

        public bool IsPaid => isPaid;

        public string PaymentType =>
            paymentMethod == null
                ? "Не вибрано"
                : paymentMethod.GetType().Name;

        // Перевірка ціни
        private void ValidatePrice(double price)
        {
            if (double.IsNaN(price) ||
                double.IsInfinity(price) ||
                price < 0)
            {
                throw new ArgumentException(
                    "Ціна не може бути від'ємною або некоректною.");
            }
        }

        // Розрахунок знижки
        public virtual double CalculateDiscount(double percent)
        {
            if (double.IsNaN(percent) ||
                double.IsInfinity(percent) ||
                percent < 0 || percent > 100)
            {
                throw new ArgumentException(
                    "Невірний відсоток знижки.");
            }

            return OriginalTotalPrice * (1 - percent / 100);
        }

        // Застосування знижки
        public virtual void ApplyDiscount(double percent)
        {
            if (IsPaid)
            {
                Console.WriteLine(
                    "Не можна змінити оплачене замовлення.");
                return;
            }

            if (double.IsNaN(percent) ||
                double.IsInfinity(percent) ||
                percent < 0 || percent > 100)
                return;

            Discount = percent;
            TotalPrice = CalculateDiscount(percent);
        }

        // Скасування знижки
        public void CancelDiscount()
        {
            ApplyDiscount(0);
        }

        // Перевірка великого замовлення
        public bool IsLargeOrder(double limit)
        {
            return TotalPrice >= limit;
        }

        // Інформація про економію
        public double GetSavedMoney()
        {
            return OriginalTotalPrice - TotalPrice;
        }

        // Вибір способу оплати
        public void SetPaymentMethod(IPaymentMethod method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (IsPaid)
            {
                Console.WriteLine(
                    "Замовлення вже оплачено.");
                return;
            }

            paymentMethod = method;

            Console.WriteLine(
                $"Спосіб оплати: {PaymentType}");
        }

        // Обробка оплати
        public bool ProcessPayment()
        {
            if (IsPaid)
            {
                Console.WriteLine(
                    "Замовлення вже оплачено.");
                return false;
            }

            if (paymentMethod == null)
            {
                Console.WriteLine(
                    "Спочатку виберіть спосіб оплати.");
                return false;
            }

            if (paymentMethod.Pay(TotalPrice))
            {
                isPaid = true;

                Console.WriteLine(
                    "Замовлення успішно оплачено!");

                return true;
            }

            Console.WriteLine("Помилка оплати.");
            return false;
        }

        // Виведення інформації про замовлення
        public void ShowInformation()
        {
            Console.WriteLine(
                $"ID: {Id} | " +
                $"Клієнт: {CustomerName} | " +
                $"Страва: {DishName} | " +
                $"Кількість: {Quantity} | " +
                $"Сума: {TotalPrice:F2} грн | " +
                $"Знижка: {Discount}% | " +
                $"Оплата: {(IsPaid ? "Оплачено" : "Не оплачено")}");
        }
    }
}
