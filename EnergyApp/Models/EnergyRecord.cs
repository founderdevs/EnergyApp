using System;

namespace EnergyApp.Models
{
    public class EnergyRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Consumption { get; set; }
        public double PricePerKwh { get; set; }
        public double Cost { get; set; }
        public string Comment { get; set; }
    }
}