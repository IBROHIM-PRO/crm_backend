namespace CRMBanks.Core.Dtos;

public class SelectItemDto
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class PagedRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

public class PagedResponseDto<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IEnumerable<T> Data { get; set; } = new List<T>();
}
