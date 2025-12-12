using System.ComponentModel.DataAnnotations;

namespace NexusProcure.Core.Enums
{
    public enum PaymentTerm
    {
        [Display(Name = "Immediate Payment")]
        Immediate = 0,

        [Display(Name = "Net 7 Days")]
        Net7 = 1,

        [Display(Name = "Net 15 Days")]
        Net15 = 2,

        [Display(Name = "Net 30 Days")]
        Net30 = 3,

        [Display(Name = "Net 45 Days")]
        Net45 = 4,

        [Display(Name = "Net 60 Days")]
        Net60 = 5,

        [Display(Name = "Advance Payment")]
        Advance = 6,

        [Display(Name = "Partial Payment")]
        Partial = 7
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var attr = enumValue.GetType()
                .GetField(enumValue.ToString())
                ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return attr?.Name ?? enumValue.ToString();
        }
    }
}