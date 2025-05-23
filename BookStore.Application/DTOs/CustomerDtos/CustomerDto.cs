﻿namespace BookStore.Application.DTOs.CustomerDtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; } 
        public string? Email { get; set; }
        public int OrderCount { get; set; }
    }
}