using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AprilBookStore.Application;
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAdministrationService, AdministrationService>();

            return services;
        }
    }

