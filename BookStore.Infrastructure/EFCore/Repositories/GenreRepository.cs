using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Infrastructure.EFCore.Repositories
{
    public class GenreRepository : EfCoreRepository<Genre>, IGenreRepository
    {
        public GenreRepository() : base() 
        {
            
        }
    }
}