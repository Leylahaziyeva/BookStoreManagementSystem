namespace BookStore.Application.DTOs.AuthorDtos
{
    public class AuthorUpdateDto
    {
        public int Id { get; set; }
        public required string Name { get; set; } 
        public required string Surname { get; set; } 
    }
}
