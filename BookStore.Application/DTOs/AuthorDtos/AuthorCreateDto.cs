namespace BookStore.Application.DTOs.AuthorDtos
{
    public class AuthorCreateDto
    {
        public required string Name { get; set; } 
        public required string Surname { get; set; } 
    }
}