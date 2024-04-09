using System.ComponentModel.DataAnnotations;

namespace CarDealership.Data.Enums
{
    public enum Condition
    {
        [Display(Name = "Новa")]
        New,

        [Display(Name = "Използванa")]
        Used
    }
}
