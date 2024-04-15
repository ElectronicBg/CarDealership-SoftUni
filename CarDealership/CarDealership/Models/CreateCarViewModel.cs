using CarDealership.Data;
using System.ComponentModel.DataAnnotations;

namespace CarDealership.Models
{
    public class CreateCarViewModel
    {
        public Car Car { get; set; }

        public List<string> Photos { get; set; }
    }
}
