using AprilBookStore.Domain.Entities;

namespace AprilBookStore.Application.Interfaces;
    public interface ICategoryService
    {
        Task<ICollection<Category>> GetCategoriesAsync();
        ICollection<Category> GetCategories();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Category? GetCategoryById(Guid id);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid id);
    }

