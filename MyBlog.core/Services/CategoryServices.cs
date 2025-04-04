using MyBlog.domain.Models;
using MyBlog.infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyBlog.core.Services
{
    public class CategoryServices
    {
        private readonly IRepository<Category> _catRepo;
        private readonly Action<string> _logAction;

        public CategoryServices(IRepository<Category> catRepo)
        {
            _logAction = message => Console.WriteLine($"LOG: {message}");
            _catRepo = catRepo;
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesWithSelectListItem()
        {
            var categories = await _catRepo.GetAllAsync();
            return categories.Select(c => new SelectListItem {
                Text = c.Name, // label
                Value = c.Id.ToString()
            });
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {
            var categories = await _catRepo.GetAllAsync();
            return categories;
        }
    }
}
