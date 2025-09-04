using HMSYSTEM.ViewModels;
using System.Security.Claims;

namespace HMSYSTEM.Helpers
{
    public static class QueryableExtensions
    {
        public static PagedResult<T> ToPagedList<T>(this IQueryable<T> source, int page, int pageSize)
        {
            var totalItems = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<T>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = page
            };
        }


        public static string TakaInWords(decimal amount)
        {
            // শুধু integer part consider করছি, ছোট amount-এর জন্য ঠিক থাকে
            string[] units = { "Zero","One","Two","Three","Four","Five","Six","Seven","Eight","Nine","Ten",
                       "Eleven","Twelve","Thirteen","Fourteen","Fifteen","Sixteen","Seventeen","Eighteen","Nineteen"};
            string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (amount == 0) return "Zero Taka";

            long number = (long)Math.Floor(amount);
            string words = "";

            if (number >= 1000)
            {
                words += units[number / 1000] + " Thousand ";
                number %= 1000;
            }

            if (number >= 100)
            {
                words += units[number / 100] + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (number < 20)
                    words += units[number];
                else
                    words += tens[number / 10] + (number % 10 > 0 ? "-" + units[number % 10] : "");
            }

            return words + " Taka";
        }

        public static class Helper
        {
            public static int GetRoleId(ClaimsPrincipal user)
            {
                var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "RoleId");
                return roleClaim != null ? int.Parse(roleClaim.Value) : 0;
            }

            public static int GetDoctorId(ClaimsPrincipal user)
            {
                var doctorClaim = user.Claims.FirstOrDefault(c => c.Type == "DoctorId");
                return doctorClaim != null ? int.Parse(doctorClaim.Value) : 0;
            }
        }
    }
}
