﻿namespace BookStore.Application.DTOs.CustomerDtos
{
    public class CustomerCreateDto
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
    }
}
