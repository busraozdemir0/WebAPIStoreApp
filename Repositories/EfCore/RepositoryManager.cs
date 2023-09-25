using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly Lazy<IBookRepository> _bookRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            _bookRepository = new Lazy<IBookRepository>(() => new BookRepository(_context)); // LazyLoading => nesne ancak ve ancak kullanıldığı anda new'lenecek. Aksi takdirde new'leme işlemi yapılmayacak.
            _categoryRepository=new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
        }

        public IBookRepository Book => _bookRepository.Value;

        public ICategoryRepository Category => _categoryRepository.Value;

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
