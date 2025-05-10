using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.DTOs.GenreDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Application.Validators.GenreValidators;
using FluentValidation;
using TableTower.Core.Builder;
using TableTower.Core.Rendering;
using TableTower.Core.Themes;

namespace BookStore.UI.Menus
{
    public static class GenresMenu
    {
        static IBookService _bookService = new BookManager();
        static IGenreService _genreService = new GenreManager();
        public static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Janrlar Menyusu");
            Console.WriteLine("2.1 Yeni Janr Elave Et");
            Console.WriteLine("2.2 Janr Siyahısı");
            Console.WriteLine("2.3 Janra göre kitabları göster");
            Console.WriteLine("0. Ana menyuya qayıt");
            Console.Write("Seçiminizi daxil edin: ");
        }

        public static void HandleMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "2.1":
                    AddGenre();
                    break;
                case "2.2":
                    ListGenres();
                    break;
                case "2.3":
                    ListGenreBooks();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Yanlış seçim.");
                    break;
            }
            Console.WriteLine("Davam etmek üçün Enter basın.");
            Console.ReadLine();
        }

        static void AddGenre()
        {
            try
            {
                Console.Write("Janr adı: ");
                string? name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Janr adı boş ola bilmez.");
                    return; 
                }

                var existingGenre = _genreService.GetAll().FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (existingGenre != null)
                {
                    Console.WriteLine("Bu adda janr artıq mövcuddur.");
                    return;
                }

                var dto = new GenreCreateDto { Name = name };

                var validator = new GenreCreateDtoValidator();
                var validationResult = validator.Validate(dto);

                if (!validationResult.IsValid)
                {
                    Console.WriteLine("Yanlış melumatlar daxil edildi:");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                    return;
                }

                var result = _genreService.Add(dto);

                var builder = new TableBuilder()
                    .WithColumns("ID", "Name")
                    .WithTheme(new AsciiTheme());

                var genres = _genreService.GetAll();
                foreach (GenreDto user in genres)
                {
                    builder.AddRow(user.Id, user.Name);
                }

                var table = builder.Build();
                new ConsoleRenderer().Print(table);

                Console.WriteLine($"Yeni janr elave olundu: {result?.Name ?? name}");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }

        static void ListGenres()
        {
            var genres = _genreService.GetAll();
            Console.WriteLine("Janr Siyahısı:");

            var builder = new TableBuilder()
              .WithColumns("ID", "Name")
              .WithTheme(new AsciiTheme());

            foreach (GenreDto genre in genres)
            {
                builder.AddRow(genre.Id, genre.Name);
            }

            var table = builder.Build();
            new ConsoleRenderer().Print(table);
        }

        static void ListGenreBooks()
        {
            var genres = _genreService.GetAll();
            Console.WriteLine("Janr Siyahısı:");

            var tableBuilder = new TableBuilder()  
                .WithColumns("ID", "Name")
                .WithTheme(new AsciiTheme());

            foreach (GenreDto genre in genres)
            {
                tableBuilder.AddRow(genre.Id, genre.Name);
            }

            var table = tableBuilder.Build();
            new ConsoleRenderer().Print(table);
            Console.Write("Kitablarını görmek istediyiniz janrın ID-sini daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int genreId))
            {
                Console.WriteLine("ID düzgün deyil. Zehmet olmasa düzgün bir ID daxil edin.");
                return;
            }

            try
            {
                var genreDetails = _genreService.GetById(genreId);  
                if (genreDetails == null)
                {
                    Console.WriteLine("Bu janr tapılmadı.");
                    return;
                }

                var books = _bookService.GetBooksByGenreId(genreId);
                if (books == null || !books.Any())
                {
                    Console.WriteLine("Bu janra aid kitab tapılmadı.");
                    return;
                }


                var bookTableBuilder = new TableBuilder()  
                    .WithColumns("ID", "Name", "Title", "Price", "Stok")
                    .WithTheme(new AsciiTheme());

                foreach (BookDto book in books)  
                {
                    bookTableBuilder.AddRow(book.Id, book.GenreName, book.Title, book.Price, book.Stock);
                }

                var bookTable = bookTableBuilder.Build();
                new ConsoleRenderer().Print(bookTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }
    }
}