using BookStore.Application.DTOs.AuthorDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using FluentValidation;

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

            var dto = new AuthorCreateDto
            {
                Name = name,
                Surname = surname
            };

            try
            {
                var result = _authorService.Add(dto);
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
            foreach (var author in authors)
            {
                Console.WriteLine($"ID: {author.Id}, Ad: {author.FullName}");
            }
        }

        static void ListAuthorBooks()
        {
            try
            {
                Console.Write("Kitablarını görmek istediyiniz müellifin ID-sini daxil edin: ");
                int authorId = Convert.ToInt32(Console.ReadLine());

                var books = _bookService.GetBooksByAuthorId(authorId);

                if (books.Any())
                {
                    Console.WriteLine("Müellifin kitabları:");
                    foreach (var book in books)
                    {
                        Console.WriteLine($"ID: {book.Id}, Ad: {book.Title}, Qiymet: {book.Price} AZN, Stok: {book.Stock}");
                    }
                }
                else
                {
                    Console.WriteLine("Bu müellifin kitabları tapılmadı.");
                }
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
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }
    }
}