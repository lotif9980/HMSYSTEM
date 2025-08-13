namespace HMSYSTEM.ViewModels
{
    public interface IPagedResult
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
