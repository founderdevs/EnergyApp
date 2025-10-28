using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyApp.Models
{
    public class EnergyRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Consumption { get; set; }
        public double Cost { get; set; }
        public string Comment { get; set; }
    }
}
