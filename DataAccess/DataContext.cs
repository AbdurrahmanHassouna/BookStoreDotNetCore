using AprilBookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.InteropServices;
namespace AprilBookStore.DataAccess
{
    public class DataContext : IData
    {
        private readonly BookStoreContext bookStoreContext;

        public DataContext(BookStoreContext bookStoreContext)
        {
            this.bookStoreContext = bookStoreContext;
        }
        public ICollection<Book> GetBooks(Author author)
        {
            return bookStoreContext.Books
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Where(b => b.AuthorId == author.Id)
                .ToList();
        }
        public ICollection<Book> GetBooks()
        {
            var books = bookStoreContext.Books.Include(a => a.Author).Include(a => a.Category).ToList();
            return books;
        }
        public IQueryable<Book> GetBooksQuery()
        {
            return bookStoreContext.Books.AsNoTracking()
                .Include(a => a.Author)
                .Include(a => a.Category)
                .AsQueryable();
        }
        public ICollection<Book> GetBooks(Category category)
        {
            return bookStoreContext.Books.AsNoTracking()
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Where(b => b.CategoryId == category.Id)
                .ToList();
        }
        public Book? GetBook(int id)
        {
            return bookStoreContext.Books
                .Include(a => a.Author)
                .Include(a => a.Category)
                .FirstOrDefault(b => b.Id == id);
        }
        public async Task<ICollection<Book>> SearchBook(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return new List<Book>();
            }

            var searchTerm = search.Trim();

            var result = await bookStoreContext.Books.AsNoTracking()
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%"))
                .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
                .ToListAsync();

            return result;
        }

        public async Task<int> GetCartItemsCountAsync(ClaimsPrincipal claims)
        {
            var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return 0;
            return await GetCartItemsCountAsync(userId);
        }

        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return 0;
            return await bookStoreContext.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => (int?)c.Quantity) ?? 0;
        }

        public async Task<ICollection<CartItem>> GetCartItemsAsync()
        {
            var cartItems = await bookStoreContext.CartItems
                .Include(c => c.User).Include(c => c.Book).ToListAsync();

            return cartItems;
        }

        public async Task<ICollection<CartItem>> GetCartItemsAsync(string userId)
        {
            return await bookStoreContext.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();
        }

        public async Task<int> AddToCart(string userId, Book book)
        {
            var cartItems = bookStoreContext.CartItems.Where(c => c.BookId == book.Id && c.UserId == userId);

            if (cartItems.Count() != 0)
            {
                var cartItem = cartItems.First(b => b.BookId == book.Id);
                if (cartItem.Quantity < book.QuantityInStock)
                {
                    cartItem.Quantity = cartItem.Quantity + 1;
                    cartItem.Price += book.Price;
                    await UpdateCartItem(cartItem);
                }
                return await GetCartItemsCountAsync(userId);
            }

            var newCartItem = new CartItem
            {
                BookId = book.Id,
                Quantity = 1,
                UserId = userId,
                Price = book.Price
            };

            await bookStoreContext.AddAsync(newCartItem);
            await bookStoreContext.SaveChangesAsync();
            return await GetCartItemsCountAsync(userId);
        }
        public async Task<int> DeleteCartItem(CartItem cartItem)
        {
            bookStoreContext.Remove(cartItem);

            return await bookStoreContext.SaveChangesAsync();
        }
        public async Task<int> UpdateCartItem(CartItem cartItem)
        {
           
            bookStoreContext.Update(cartItem);
            return await bookStoreContext.SaveChangesAsync();
        }
        public async Task<ICollection<Order>> GetOrders(ClaimsPrincipal claims)
        {
            var Orders = bookStoreContext.Orders.Where(o => o.UserId==claims.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
            return Orders;
        }
        public async Task<Order?> GetOrderDetails(int id)
        {
            Order? order = await bookStoreContext.Orders.Where(o=>o.Id==id).Include(o=>o.OrderItems).FirstOrDefaultAsync();

            return order;
        }
        public async Task<Order>? SubmitOrder(string UserId)
        {
            ICollection<CartItem> cartItems= bookStoreContext.CartItems.Where(c=>c.UserId==UserId).Include(c=>c.Book).ToList();
            if (cartItems.Count == 0)
            {
                return null;
            }
            Order order=new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c=>c.Book.Price*c.Quantity),
                UserId = UserId,
                OrderItems= new List<OrderItem>()
            };
            foreach(var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    BookName = cartItem.Book.Name,
                    Price = cartItem.Book.Price,
                    Quantity = cartItem.Quantity
                };
                cartItem.Book.QuantityInStock-=cartItem.Quantity;
                if (cartItem.Book.QuantityInStock <0) throw new Exception("Error Quantity");
                bookStoreContext.Update(cartItem.Book);
                order.OrderItems.Add(orderItem);
            }
            bookStoreContext.RemoveRange(cartItems);
            await bookStoreContext.AddAsync(order);
            await bookStoreContext.SaveChangesAsync();
            return order;
        }
        public ICollection<Author> GetAuthors()
        {
            return bookStoreContext.Authors.Include(a => a.Books).ToList();
        }
        public Author? GetAuthor(int id)
        {
            return bookStoreContext.Authors.Include(a => a.Books).FirstOrDefault(a => a.Id == id);
        }
        public void AddAuthor(Author author)
        {
            bookStoreContext.Authors.Add(author);
            bookStoreContext.SaveChanges();
        }
        public void UpdateAuthor(Author author)
        {
            bookStoreContext.Authors.Update(author);
            bookStoreContext.SaveChanges();
        }
        public void DeleteAuthor(int id)
        {
            var author = bookStoreContext.Authors.Find(id);
            if (author != null)
            {
                bookStoreContext.Authors.Remove(author);
                bookStoreContext.SaveChanges();
            }
        }
        public ICollection<Category> GetCategories()
        {
            return bookStoreContext.Categories.ToList();
        }
        public Category GetCategory(int id)
        {
            return bookStoreContext.Categories.Where(a => a.Id==id).FirstOrDefault();
        }
        public void AddCategory(Category category)
        {
            bookStoreContext.Categories.Add(category);
            bookStoreContext.SaveChanges();
        }
        public void UpdateCategory(Category category)
        {
            bookStoreContext.Categories.Update(category);
            bookStoreContext.SaveChanges();
        }
        public void DeleteCategory(int id)
        {
            var category = bookStoreContext.Categories.Find(id);
            if (category != null)
            {
                bookStoreContext.Categories.Remove(category);
                bookStoreContext.SaveChanges();
            }
        }
        public void AddBook(Book book) {
            bookStoreContext.Books.Add(book);
            bookStoreContext.SaveChanges();
        }
        public void UpdateBook(Book book)
        {
            bookStoreContext.Books.Update(book);
            bookStoreContext.SaveChanges();
        }
        public void DeleteBook(int id)
        {
            var book = bookStoreContext.Books.Find(id);
            if (book != null)
            {
                bookStoreContext.Books.Remove(book);
                bookStoreContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            bookStoreContext.Dispose();
        }
    }
}
