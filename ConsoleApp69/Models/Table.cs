using System;

namespace RestaurantProgram
{
    class Table
    {
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsFree { get; set; }
        public string ReservedBy { get; set; }

        public void ShowInformation() =>
            Console.WriteLine(
                $"Столик №{Number} | Місць: {Seats} | " +
                (IsFree
                    ? "Вільний"
                    : "Заброньований | Забронював: " + ReservedBy));

        public bool Reserve(string customerName)
        {
            if (!IsFree)
                return false;

            IsFree = false;
            ReservedBy = customerName;
            return true;
        }

        public void CancelReservation()
        {
            IsFree = true;
            ReservedBy = "";
        }
    }
}
