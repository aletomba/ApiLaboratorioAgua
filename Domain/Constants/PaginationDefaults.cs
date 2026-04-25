namespace Dominio.Constants;

public static class PaginationDefaults
{
    public const int DefaultPageSize = 50;
    public const int MaxPageSize = 500;

    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        return (page, pageSize);
    }
}