namespace DocSpaceSemanticExample.Api.Models;

public class BaseResponse<T>
{
    public required T Response { get; set; }
    public int Count { get; set; }
    public required List<Link> Links { get; set; }
    public int Status { get; set; }
    public int StatusCode { get; set; }
}

public class Link
{
    public required string Href { get; set; }
    public required string Action { get; set; }
}