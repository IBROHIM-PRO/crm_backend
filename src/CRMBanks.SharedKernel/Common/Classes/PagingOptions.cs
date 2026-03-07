namespace CRMBanks.SharedKernel.Common.Classes;

public class PagingOptions
{
    public const int MaxPageSize = 100; // Maximum allowed page size to prevent memory issues
    
    public PagingOptions()
    {
        Normalize();
    }

    private int _page;

    public int Page
    {
        get => _page;
        set => _page = value <= 0 ? 1 : value;
    }

    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value == 0 ? 10 : Math.Min(value, MaxPageSize); // Enforce maximum page size
    }
    
    public const int PageSizeNoPagingFlag = -1;

    private string _orderBy = OrderByTypes.Fallback;

    public string OrderBy { get; set; } = OrderByTypes.DateCreated;
    public string? ThenOrderBy { get; set; }

        
    private string _order = OrderTypes.Fallback;
    public string Order { get; set; } = OrderTypes.Desc;
    public string ThenOrder { get; set; } = OrderTypes.Desc;

    public void Normalize()
    {
        if (Page == 0) Page = 0;
        if (PageSize == 0) PageSize = 0;
        if (PageSize > MaxPageSize) PageSize = MaxPageSize; // Additional safety check
    }

    public static class OrderTypes
    {
        public const string Asc = "asc";
        public const string Desc = "desc";

        public static string Fallback => Desc;

        public static readonly IReadOnlyList<string> All = new[]
        {
            Asc,
            Desc
        };
    }

    public static class OrderByTypes
    {
        public const string DateCreated = "date_created";
        public const string Name = "name";

        public static string Fallback => DateCreated;

        public static readonly IReadOnlyList<string> All = new[]
        {
            DateCreated,
            Name
        };
    }
}