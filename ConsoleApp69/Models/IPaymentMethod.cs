
using System;

namespace RestaurantProgram
{
    // Інтерфейс визначає спільний контракт оплати
    interface IPaymentMethod
    {
        bool Pay(double amount);
        string GetPaymentInfo();
    }

    // Оплата готівкою
    class CashPayment : IPaymentMethod
    {
        public bool Pay(double amount)
        {
            if (double.IsNaN(amount) ||
                double.IsInfinity(amount) || amount <= 0)
            {
                Console.WriteLine("Некоректна сума оплати.");
                return false;
            }

            Console.WriteLine(
                $"Оплата готівкою: {amount:F2} грн");

            return true;
        }

        public string GetPaymentInfo()
        {
            return "Готівка";
        }
    }

    // Оплата банківською карткою
    class CardPayment : IPaymentMethod
    {
        public bool Pay(double amount)
        {
            if (double.IsNaN(amount) ||
                double.IsInfinity(amount) || amount <= 0)
            {
                Console.WriteLine("Некоректна сума оплати.");
                return false;
            }

            Console.WriteLine(
                $"Оплата карткою: {amount:F2} грн");

            return true;
        }

        public string GetPaymentInfo()
        {
            return "Банківська картка";
        }
    }
}
