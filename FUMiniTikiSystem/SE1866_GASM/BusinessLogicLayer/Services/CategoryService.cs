using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Category> GetAll() => _repository.GetAll();

        public Category? GetById(int id) => _repository.GetById(id);

        public void Add(Category category) => _repository.Add(category);

        public void Update(Category category) => _repository.Update(category);

        public void Delete(int id) => _repository.Delete(id);
    }
}
