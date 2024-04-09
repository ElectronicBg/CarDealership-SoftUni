using System.ComponentModel.DataAnnotations;

namespace CarDealership.Data.Enums
{
    public enum TransmissionType
    {
        [Display(Name = "Ръчна")]
        Manual,

        [Display(Name = "Полуавтоматична")]
        SemiAutomatic,

        [Display(Name = "Автоматична")]
        Automatic
    }
}
