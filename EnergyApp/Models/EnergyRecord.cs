using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyApp.Models
{
    public class EnergyRecord
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public double KilowattHours { get; set; }
        public double CostPerKwh { get; set; }
        public double TotalCost => KilowattHours * CostPerKwh;
    }
}