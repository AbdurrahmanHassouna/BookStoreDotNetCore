using AprilBookStore.Application.Models.Category;
using AprilBookStore.Application.Models.Common;

namespace AprilBookStore.Application.Interfaces;

public interface ICategoryService
{
    Task<ICollection<CategoryDto>> GetCategoriesAsync(bool forAdmin = false);
    Task<CategoryDetailsDto?> GetCategoryDetailsAsync(Guid id, bool forAdmin = false);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<ServiceResult> CreateCategoryAsync(CreateCategoryDto dto);
    Task<ServiceResult> UpdateCategoryAsync(UpdateCategoryDto dto);
    Task<ServiceResult> DeleteCategoryAsync(Guid id);
    Task<ServiceResult> RestoreCategoryAsync(Guid id);
    Task<bool> IsCategoryNameUniqueAsync(string name, Guid? currentCategoryId = null);
    Task<bool> HasAssociatedBooksAsync(Guid id);
}
