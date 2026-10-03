using System;

namespace RestaurantProgram
{
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }

        public void ShowInformation() =>
            Console.WriteLine($"{Id}. {Name} | {Age} років | {Phone}");

        public bool IsAdult() => Age >= 18;

        public string GetContact() => Name + " - " + Phone;
    }
}
