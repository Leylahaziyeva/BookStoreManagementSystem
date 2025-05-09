using BookStore.Application.DTOs.GenreDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Application.Validators.GenreValidators;
using FluentValidation;

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
            foreach (var genre in genres)
            {
                Console.WriteLine($"ID: {genre.Id}, Ad: {genre.Name}, Kitab sayı: {genre.BookCount}");
            }
        }

        static void ListGenreBooks()
        {
            Console.Write("Kitablarını görmek istediyiniz janrın ID-sini daxil edin: ");

            if (!int.TryParse(Console.ReadLine(), out int genreId))
            {
                Console.WriteLine("ID düzgün deyil. Zehmet olmasa düzgün bir ID daxil edin.");
                return;
            }

            try
            {
                var genre = _genreService.GetById(genreId);
                if (genre == null)
                {
                    Console.WriteLine("Bu janr tapılmadı.");
                    return;
                }

                var books = _bookService.GetBooksByGenreId(genreId);

                if (books.Any())
                {
                    Console.WriteLine($"{genre.Name} janrına aid kitablar:");
                    foreach (var book in books)
                    {
                        Console.WriteLine($"ID: {book.Id}, Ad: {book.Title}, Qiymet: {book.Price} AZN, Stok: {book.Stock}");
                    }
                }
                else
                {
                    Console.WriteLine("Bu janra aid heç bir kitab tapılmadı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }
    }
}