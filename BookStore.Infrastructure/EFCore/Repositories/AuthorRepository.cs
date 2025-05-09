using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Infrastructure.EFCore.Repositories
{
    public class AuthorRepository : EfCoreRepository<Author>, IAuthorRepository
    {
        public AuthorRepository() : base()
        {
            
        }
    }
}
