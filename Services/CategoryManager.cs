using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryManager : ICategoryService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;

        public CategoryManager(IRepositoryManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateOneCategoryAsync(CategoryDtoForInsertion categoryDto)
        {
            var entity = _mapper.Map<Category>(categoryDto);
            _manager.Category.Create(entity);
            await _manager.SaveAsync();
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task DeleteOneCategoryAsync(int id, bool trackChanges)
        {
            var categoryID = await GetOneCategoryByIdCheckExists(id, trackChanges);  // belirtilen id'ye sahip kayit var mi diye kontrol edecek olan method
            _manager.Category.Delete(categoryID);
            await _manager.SaveAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges)
        {
            return await _manager.Category.GetAllCategoriesAsync(trackChanges);
        }

        public async Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges)
        {
            var category = await _manager.Category.GetOneCategoryByIdAsync(id, trackChanges);

            if (category is null)  // eğer girilen id yoksa hata fırlat (global hata yönetimi ile)
                throw new CategoryNotFoundException(id);

            return category;
        }

        public async Task UpdateOneCategoryAsync(int id, CategoryDtoForUpdate categoryDto, bool trackChanges)
        {
            var categoryID = await GetOneCategoryByIdCheckExists(id, trackChanges);
            categoryID=_mapper.Map<Category>(categoryDto);
            _manager.Category.Update(categoryID);
            await _manager.SaveAsync();
        }
        private async Task<Category> GetOneCategoryByIdCheckExists(int id, bool trackChanges)
        {
            var categoryID = await _manager.Category.GetOneCategoryByIdAsync(id, trackChanges);
            if(categoryID is null)
                throw new CategoryNotFoundException(id);

            return categoryID;

        }
    }
}
