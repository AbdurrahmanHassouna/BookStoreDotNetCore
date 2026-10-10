namespace AprilBookStore.Application.Models.Book;

public class BookAdminFilterDto
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortOrder { get; set; }
    public string? PrevSort { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
