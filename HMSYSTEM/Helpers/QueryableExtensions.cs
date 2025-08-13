using HMSYSTEM.ViewModels;

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
    }
}
