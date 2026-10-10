using AprilBookStore.Application.Models.Cart;
using AprilBookStore.Application.Models.Common;
using AprilBookStore.Application.Models.Orders;
using AprilBookStore.Web.ViewModels.Cart;
using AprilBookStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace AprilBookStore.Web.Extensions;

public static class MappingExtensions
{
    public static FileUploadDto? ToFileUploadDto(this IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;
        return new FileUploadDto(file.OpenReadStream(), file.FileName, file.Length);
    }

    public static IPagedList<TDest> ToPagedList<TSrc, TDest>(
        this PagedResult<TSrc> source,
        Func<TSrc, TDest> map)
    {
        var items = source.Items.Select(map).ToList();
        var metaData = new StaticPagedList<TDest>(items, source.PageNumber, source.PageSize, source.TotalItemCount).GetMetaData();
        return new StaticPagedList<TDest>(items, metaData);
    }

    public static CartViewModel ToViewModel(this CartDto dto)
    {
        return new CartViewModel
        {
            Items = dto.Items.Select(c => new CartItemViewModel
            {
                Id = c.Id,
                BookId = c.BookId,
                BookName = c.BookName,
                ImgPath = c.ImgPath,
                UnitPrice = c.UnitPrice,
                Quantity = c.Quantity,
                QuantityInStock = c.QuantityInStock
            }).ToList()
        };
    }

    public static OrderListItemViewModel ToViewModel(this OrderSummaryDto dto)
    {
        return new OrderListItemViewModel
        {
            Id = dto.Id,
            OrderDate = dto.OrderDate,
            TotalAmount = dto.TotalAmount,
            ItemCount = dto.ItemCount
        };
    }

    public static OrderDetailsViewModel ToViewModel(this OrderDetailsDto dto)
    {
        return new OrderDetailsViewModel
        {
            Id = dto.Id,
            OrderDate = dto.OrderDate,
            TotalAmount = dto.TotalAmount,
            Items = dto.Items.Select(item => new OrderItemViewModel
            {
                Id = item.Id,
                BookName = item.BookName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        };
    }
}
