using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Author> Authors { get; }
    DbSet<Book> Books { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Category> Categories { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();

    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default);
}
