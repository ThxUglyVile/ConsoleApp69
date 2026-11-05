
using System;

namespace RestaurantProgram
{
    class Table
    {
        // Закриті поля
        private int number;
        private int seats = 4;
        private bool isFree = true;
        private string reservedBy = "";

        // Інкапсуляція
        public int Number
        {
            get => number;
            set
            {
                if (value <= 0)
                    throw new ArgumentException(
                        "Номер столика повинен бути більшим за 0.");

                number = value;
            }
        }

        public int Seats
        {
            get => seats;
            set
            {
                if (value <= 0 || value > 20)
                    throw new ArgumentException(
                        "Кількість місць повинна бути від 1 до 20.");

                seats = value;
            }
        }

        public bool IsFree
        {
            get => isFree;
            set => isFree = value;
        }

        public string ReservedBy
        {
            get => reservedBy;
            set => reservedBy = value ?? "";
        }

        // Конструктор без параметрів
        public Table()
        {
        }

        // Конструктор з параметрами
        public Table(int number, int seats)
        {
            Number = number;
            Seats = seats;
        }

        // Віртуальний метод
        public virtual void ShowInformation()
        {
            Console.WriteLine(
                $"Столик №{Number} | Місць: {Seats} | " +
                (IsFree
                    ? "Вільний"
                    : "Заброньований | Забронював: " + ReservedBy));
        }

        // Бронювання столика
        public virtual bool Reserve(string customerName)
        {
            if (!IsFree ||
                string.IsNullOrWhiteSpace(customerName))
                return false;

            IsFree = false;
            ReservedBy = customerName.Trim();

            return true;
        }

        // Перевантаження методу
        public bool Reserve(string customerName, int guests)
        {
            if (guests <= 0 || guests > Seats)
                return false;

            return Reserve(customerName);
        }

        // Скасування бронювання
        public virtual void CancelReservation()
        {
            IsFree = true;
            ReservedBy = "";
        }

        // Перевірка доступності
        public bool CanAccommodate(int guests)
        {
            return IsFree && guests > 0 && guests <= Seats;
        }

        // Кількість доступних місць
        public int GetAvailableSeats()
        {
            return IsFree ? Seats : 0;
        }

        // Інформація про бронювання
        public string GetReservationInfo()
        {
            if (IsFree)
                return "Столик вільний";

            return $"Столик №{Number} заброньовано: {ReservedBy}";
        }

        // Захищений метод для нащадків
        protected virtual string GetTableType()
        {
            return "Стандартний столик";
        }

        public virtual string GetDescription()
        {
            return $"{GetTableType()} №{Number}, місць: {Seats}";
        }
    }
}
