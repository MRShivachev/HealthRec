namespace HealthRec.Services.Common.Models;

public class EmailModel
{
    public required string Email { get; set; }

    public required string Subject { get; set; }
    
    public required string To { get; set; }
    public required string Message { get; set; }
    
    public required string Body { get; set; }
}

public class PaginatedList<T>
{
    public List<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}

public class SearchParameters
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; }
    public bool SortAscending { get; set; } = true;
}