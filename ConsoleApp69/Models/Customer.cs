
using System;

namespace RestaurantProgram
{
    public class Customer
    {
        // Закриті поля (інкапсуляція)
        private string name = "";
        private int age;
        private string phone = "";

        public int Id { get; set; }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ім'я не може бути порожнім.");

                name = value.Trim();
            }
        }

        public int Age
        {
            get => age;
            set
            {
                if (value < 0 || value > 120)
                    throw new ArgumentException("Некоректний вік клієнта.");

                age = value;
            }
        }

        public string Phone
        {
            get => phone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Телефон не може бути порожнім.");

                phone = value.Trim();
            }
        }

        // Конструктор без параметрів
        public Customer()
        {
        }

        // Конструктор з параметрами
        public Customer(int id, string name, int age, string phone)
        {
            Id = id;
            Name = name;
            Age = age;
            Phone = phone;
        }

        // Віртуальний метод для майбутнього поліморфізму
        public virtual void ShowInformation()
        {
            Console.WriteLine(
                $"{Id}. {Name} | {Age} років | {Phone}");
        }

        // Перевантаження методу
        public void ShowInformation(bool showPhone)
        {
            if (showPhone)
                ShowInformation();
            else
                Console.WriteLine($"{Id}. {Name} | {Age} років");
        }

        public bool IsAdult()
        {
            return Age >= 18;
        }

        public string GetContact()
        {
            return Name + " - " + Phone;
        }
    }
}
