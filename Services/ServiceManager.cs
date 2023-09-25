using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    // Bütün servisleri burada toplayıp lazy loading yöntemiyle kullanabiliyoruz.
    public class ServiceManager : IServiceManager
    {
        // Lazy loading
        private readonly Lazy<IBookService> _bookService;
        private readonly Lazy<ICategoryService> _categoryService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        public ServiceManager(IRepositoryManager repositoryManager, ILoggerService logger, IMapper mapper, IConfiguration configuration, UserManager<User> userManager, IBookLinks bookLinks)
        {
            _bookService = new Lazy<IBookService>(() => 
            new BookManager(repositoryManager, logger, mapper, bookLinks));

            _categoryService = new Lazy<ICategoryService>(() =>
             new CategoryManager(repositoryManager));

            _authenticationService = new Lazy<IAuthenticationService>(() =>
             new AuthenticationManager(logger, mapper, userManager, configuration));
        }
        public IBookService BookService => _bookService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        public ICategoryService CategoryService => _categoryService.Value;
    }
}
