

namespace EventModuleApi.Core.Helpers;

public class PagedApiResponse : ApiResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }

    public PagedApiResponse(int statusCode, string message, object? payload, int pageNumber, int pageSize, int totalPages, int totalRecords)
        : base(statusCode, message, payload)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;
        TotalRecords = totalRecords;
    }
}