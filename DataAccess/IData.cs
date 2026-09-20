using AprilBookStore.Controllers;
using AprilBookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace AprilBookStore.DataAccess
{
    public interface IData
    {
        ICollection<Book> GetBooks(Author author);
        ICollection<Book> GetBooks();
        IQueryable<Book> GetBooksQuery();
        Book? GetBook(int  id);
        ICollection<Book> GetBooks(Category category);
        ICollection<Author> GetAuthors();
        Author? GetAuthor(int id);
        void AddAuthor(Author author);
        void UpdateAuthor(Author author);
        void DeleteAuthor(int id);
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
        Task<ICollection<Book>> SearchBook(string search);
        Task<int> GetCartItemsCountAsync(ClaimsPrincipal claims);
        Task<int> GetCartItemsCountAsync(string userId);
        Task<ICollection<CartItem>> GetCartItemsAsync();
        Task<ICollection<CartItem>> GetCartItemsAsync(string userId);
        Task<int> AddToCart(string userId, Book book);
        Task<int> DeleteCartItem(CartItem cartItem);
        Task<int> UpdateCartItem(CartItem cartItem);
        Task<ICollection<Order>> GetOrders(ClaimsPrincipal claims);
        Task<Order?> GetOrderDetails(int orderId);
        Task<Order> SubmitOrder(string userId);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(int id);
        void Dispose();

    }
}
