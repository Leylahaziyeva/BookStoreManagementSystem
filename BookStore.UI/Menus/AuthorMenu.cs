using BookStore.Application.DTOs.AuthorDtos;
using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using FluentValidation;
using TableTower.Core.Builder;
using TableTower.Core.Rendering;
using TableTower.Core.Themes;

namespace BookStore.UI.Menus
{
    public static class AuthorsMenu
    {
        static IBookService _bookService = new BookManager();
        static IAuthorService _authorService = new AuthorManager();
        public static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Müellifler Menyusu");
            Console.WriteLine("1.1 Yeni Müellif Elave Et");
            Console.WriteLine("1.2 Müellif Siyahısı");
            Console.WriteLine("1.3 Müellifin Kitabları");
            Console.WriteLine("0. Ana menyuya qayıt");
            Console.Write("Seçiminizi daxil edin: ");
        }
        public static void HandleMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "1.1":
                    AddAuthor();
                    break;
                case "1.2":
                    ListAuthors();
                    break;
                case "1.3":
                    ListAuthorBooks();
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

        static void AddAuthor()
        {
            Console.Write("Müellifin adı: ");
            string? name = Console.ReadLine();
            Console.Write("Müellifin soyadı: ");
            string? surname = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
            {
                Console.WriteLine("Ad ve soyad boş ola bilmez.");
                return;
            }

            if (!name.All(char.IsLetter) || !surname.All(char.IsLetter))
            {
                Console.WriteLine("Ad ve soyad yalnız herflerden ibaret olmalıdır.");
                return;
            }

            var existingAuthor = _authorService.GetAll()
                .FirstOrDefault(a => a.FullName.Equals($"{name} {surname}", StringComparison.OrdinalIgnoreCase));
            if (existingAuthor != null)
            {
                Console.WriteLine("Bu adda və soyadla müellif artıq mövcuddur.");
                return;
            }

            var dto = new AuthorCreateDto
            {
                Name = name,
                Surname = surname
            };

            try
            {
                var result = _authorService.Add(dto);

                var builder = new TableBuilder()
                    .WithColumns("ID", "FullName")
                    .WithTheme(new AsciiTheme());
                var authors = _authorService.GetAll();
                foreach (AuthorDto user in authors)
                {
                    builder.AddRow(user.Id, user.FullName);
                }

                var table = builder.Build();
                new ConsoleRenderer().Print(table);

                Console.WriteLine($"Yeni müellif elave olundu: {result?.FullName ?? $"{name} {surname}"}");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Xeta: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir sehv baş verdi: {ex.Message}");
            }
        }

        static void ListAuthors()
        {
            var authors = _authorService.GetAll();
            Console.WriteLine("Müellif Siyahısı:");

            var builder = new TableBuilder()
                .WithColumns("ID", "FullName")
                .WithTheme(new AsciiTheme());

            foreach (AuthorDto user in authors)
            {
                builder.AddRow(user.Id, user.FullName);
            }

            var table = builder.Build();
            new ConsoleRenderer().Print(table);
        }

        static void ListAuthorBooks()
        {
            try
            {
                Console.Write("Kitablarını görmek istediyiniz müellifin ID-sini daxil edin: ");
                int authorId = Convert.ToInt32(Console.ReadLine());

                var books = _bookService.GetBooksByAuthorId(authorId);

                var builder = new TableBuilder()
                    .WithColumns("ID", "Name", "Title", "Price", "Stok")
                    .WithTheme(new AsciiTheme());

                foreach (BookDto user in books)
                {
                    builder.AddRow(user.Id, user.AuthorFullName, user.Title, user.Price, user.Stock);
                }

                var table = builder.Build();
                new ConsoleRenderer().Print(table);
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Yoxlama xetası: {string.Join(", ", ex.Errors.Select(e => e.ErrorMessage))}");
            }
            catch (FormatException)
            {
                Console.WriteLine("ID düzgün formatda deyil.");
            }
            catch (Exception ex)
            {
              Console.WriteLine("Bu ID-li müellifin kitabi yoxdur.");
            }
        }
    }
}