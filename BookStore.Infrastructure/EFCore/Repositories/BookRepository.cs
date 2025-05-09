using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Infrastructure.EFCore.Repositories
{
    public class BookRepository : EfCoreRepository<Book>, IBookRepository
    {
        public BookRepository() : base()
        {
        }
    }
}
