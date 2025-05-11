using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TableTower.Core.Builder;
using TableTower.Core.Rendering;
using TableTower.Core.Themes;

namespace BookStore.UI.Menus
{
    public static class BooksMenu
    {
        static IBookService _bookService = new BookManager();
        static IAuthorService _authorService = new AuthorManager();
        static IGenreService _genreService = new GenreManager();
        public static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Kitablar Menyusu");
            Console.WriteLine("3.1 Yeni Kitab Elave Et");
            Console.WriteLine("3.2 Kitab Siyahısı");
            Console.WriteLine("3.3 Kitab Axtar");
            Console.WriteLine("3.4 Kitabı Redakte Et");
            Console.WriteLine("3.5 Kitabı Sil");
            Console.WriteLine("0. Ana menyuya qayıt");
            Console.Write("Seçiminizi daxil edin: ");
        }

        public static void HandleMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "3.1":
                    AddBook();
                    break;
                case "3.2":
                    ListBooks();
                    break;
                case "3.3":
                    SearchBook();
                    break;
                case "3.4":
                    EditBook();
                    break;
                case "3.5":
                    DeleteBook();
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

        static void AddBook()
        {
            try
            {
                Console.Write("Kitabın adı: ");
                string? title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title))
                {
                    Console.WriteLine("Kitabın adı boş ola bilməz.");
                    return;
                }

                var existingBook = _bookService.GetAll()
                    .FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
                if (existingBook != null)
                {
                    Console.WriteLine("Bu adda kitab artıq mövcuddur.");
                    return;
                }

                Console.Write("Kitabın qiyməti (AZN): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
                {
                    Console.WriteLine("Qiymət düzgün deyil. Zəhmət olmasa müsbət bir qiymət daxil edin.");
                    return;
                }

                Console.Write("Kitabın stok miqdarı: ");
                if (!int.TryParse(Console.ReadLine(), out int stock) || stock <= 0)
                {
                    Console.WriteLine("Stok miqdarı düzgün deyil. Zəhmət olmasa müsbət bir ədəd daxil edin.");
                    return;
                }

                Console.WriteLine("\n================= Mövcud Janrlar =================");
                var genres = _genreService.GetAll();

                if (!genres.Any())
                {
                    Console.WriteLine("Heç bir janr tapılmadı.");
                    return;
                }

                var genreTableBuilder = new TableBuilder()
                    .WithColumns("ID", "Name")
                    .WithTheme(new AsciiTheme());

                foreach (var genre in genres)
                {
                    genreTableBuilder.AddRow(genre.Id, genre.Name);
                }

                var genreTable = genreTableBuilder.Build();
                new ConsoleRenderer().Print(genreTable);

                Console.Write("\nKitabın janrını daxil edin (ID): ");
                if (!int.TryParse(Console.ReadLine(), out int genreId) || genreId <= 0 || !genres.Any(g => g.Id == genreId))
                {
                    Console.WriteLine("Yanlış janr ID-si.");
                    return;
                }

                Console.WriteLine("\n================= Mövcud Müəlliflər =================");
                var authors = _authorService.GetAll();

                if (!authors.Any())
                {
                    Console.WriteLine("Heç bir müəllif tapılmadı.");
                    return;
                }

                var authorTableBuilder = new TableBuilder()
                    .WithColumns("ID", "Full Name")
                    .WithTheme(new AsciiTheme());

                foreach (var author in authors)
                {
                    authorTableBuilder.AddRow(author.Id, author.FullName);
                }

                var authorTable = authorTableBuilder.Build();
                new ConsoleRenderer().Print(authorTable);

                Console.Write("\nKitabın müəllifinin ID-sini daxil edin: ");
                if (!int.TryParse(Console.ReadLine(), out int authorId) || authorId <= 0 || !authors.Any(a => a.Id == authorId))
                {
                    Console.WriteLine("Yanlış müəllif ID-si.");
                    return;
                }

                var dto = new BookCreateDto
                {
                    Title = title,
                    Price = price,
                    Stock = stock,
                    GenreId = genreId,
                    AuthorId = authorId
                };

                var result = _bookService.Add(dto);

                Console.WriteLine("\n================= Yeni Kitab Əlavə Olundu =================");

                var builder = new TableBuilder()
                    .WithColumns("ID", "Title", "Author", "Genre", "Price (AZN)", "Stock")
                    .WithTheme(new AsciiTheme());

                builder.AddRow(result.Id, result.Title, result.AuthorFullName, result.GenreName, result.Price.ToString("F2"), result.Stock);

                var table = builder.Build();
                new ConsoleRenderer().Print(table);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Yeni kitab əlavə olundu: {result?.Title ?? title}");
                Console.ResetColor();
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Xəta: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir xəta baş verdi: {ex.Message}");
            }
        }

        static void SearchBook()
        {
            Console.WriteLine("Axtarış növünü seçin:");
            Console.WriteLine("1. Kitab adına göre");
            Console.WriteLine("2. Müellif adına göre");
            Console.WriteLine("3. Janra göre");
            Console.Write("Seçiminiz: ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Write("Kitab adı: ");
                    string? title = Console.ReadLine();

                    var booksByTitle = _bookService.GetAll(include:x=>x.Include(x=>x.Author).Include(x=>x.Genre))
                                                   .Where(b => b.Title != null && b.Title.Contains(title ?? "", StringComparison.OrdinalIgnoreCase))
                                                   .ToList();

                    if (booksByTitle.Any())
                    {
                        var builder = new TableBuilder(opt =>
                        {
                            opt.WrapData = false;
                        })
                        .WithColumns("ID", "Title", "AuthorName", "Genre", "Price", "Stock")
                        .WithTheme(new AsciiTheme());

                        foreach (var book in booksByTitle)
                        {
                            builder.AddRow(book.Id, book.Title, book.AuthorFullName, book.GenreName, book.Price, book.Stock);
                        }

                        var table = builder.Build();
                        new ConsoleRenderer().Print(table);
                    }
                    else
                    {
                        Console.WriteLine("Kitab tapılmadı.");
                    }
                    break;

                case "2":          
                        Console.Write("Müellif adı ve ya soyadı: ");
                        string? authorSearch = Console.ReadLine();

                        var authors = _authorService.GetAll()
                            .Where(a => (a.FullName).Contains(authorSearch ?? "", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (authors.Any())
                        {
                            foreach (var author in authors)
                            {
                                if (author.Books.Any())
                                {
                                    var builder = new TableBuilder()
                                        .WithColumns("ID", "AuthorName", "Title", "Genre", "Price", "Stock")
                                        .WithTheme(new AsciiTheme());

                                    foreach (BookDto book in author.Books)
                                    {
                                        builder.AddRow(book.Id, book.AuthorFullName, book.Title, book.GenreName, book.Price, book.Stock);
                                    }

                                    var table = builder.Build();
                                    new ConsoleRenderer().Print(table);
                                }
                                else
                                {
                                    Console.WriteLine("Kitablar tapılmadı.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Müellif tapılmadı.");
                        }
                        break;

                case "3":
                    Console.Write("Janr adı: ");
                    string? genreName = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(genreName))
                    {
                        Console.WriteLine("Boş janr adı daxil edilemez.");
                        break;
                    }

                    var genres = _genreService.GetAllWithBooks()
                        .Where(g => g.Name.Contains(genreName ?? "", StringComparison.OrdinalIgnoreCase));

                    if (!genres.Any())
                    {
                        Console.WriteLine("Janr tapılmadı.");
                        break;
                    }

                    foreach (var genre in genres)
                    {
                        if (genre.Books.Any())
                        {
                            var builder = new TableBuilder()
                                .WithColumns("ID", "Genre", "Title", "AuthorName", "Price")
                                .WithTheme(new AsciiTheme());

                            foreach (BookDto book in genre.Books)
                            {
                                builder.AddRow(book.Id, book.GenreName, book.Title, book.AuthorFullName, book.Price);
                            }

                            var table = builder.Build();
                            new ConsoleRenderer().Print(table);
                        }
                        else
                        {
                            Console.WriteLine("Janr tapılmadı.");
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Yanlış seçim.");
                    break;
            }
        }

        static void ListBooks()
        {
            var books = _bookService.GetAll(include: x => x.Include(x => x.Author).Include(x => x.Genre));
            Console.WriteLine("Kitab Siyahısı:");

            var builder = new TableBuilder()
            .WithColumns("ID", "Title", "AuthorName", "Genre", "Price", "Stock")
               .WithTheme(new AsciiTheme());

            foreach (BookDto book in books)
            {
                builder.AddRow(book.Id, book.Title, book.AuthorFullName, book.GenreName, book.Price, book.Stock);
            }

            var table = builder.Build();
            new ConsoleRenderer().Print(table);
        }

        static void EditBook()
        {
            var allBooks = _bookService.GetAll();
            if (!allBooks.Any())
            {
                Console.WriteLine("Kitab siyahısı boşdur.");
                return;
            }

            Console.WriteLine("\n================= Mövcud Kitablar =================");

            var bookTableBuilder = new TableBuilder()
                .WithColumns("ID", "Title", "AuthorName", "Genre", "Price", "Stock")
                .WithTheme(new AsciiTheme());

            foreach (var b in allBooks)
            {
                bookTableBuilder.AddRow(b.Id, b.Title, b.AuthorFullName, b.GenreName, b.Price, b.Stock);
            }

            var bookTable = bookTableBuilder.Build();
            new ConsoleRenderer().Print(bookTable);

            Console.WriteLine("\n=====================================================");
            Console.Write("Redaktə etmək istədiyiniz kitabın ID-sini daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.WriteLine("Yanlış kitab ID-si.");
                return;
            }

            var book = _bookService.GetById(bookId);
            if (book == null)
            {
                Console.WriteLine("Bu kitab tapılmadı.");
                return;
            }

            Console.Write("Yeni kitabın adını daxil edin: ");
            string? title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Kitab adı boş ola bilməz.");
                return;
            }

            Console.Write("Yeni qiyməti daxil edin (AZN): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Qiymət düzgün deyil.");
                return;
            }

            Console.Write("Yeni stok miqdarını daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Stok miqdarı düzgün deyil.");
                return;
            }

            var allAuthors = _authorService.GetAll();
            if (!allAuthors.Any())
            {
                Console.WriteLine("Yazar siyahısı boşdur.");
                return;
            }

            var authorTableBuilder = new TableBuilder()
                .WithColumns("ID", "Ad Soyad")
                .WithTheme(new AsciiTheme());

            foreach (var author in allAuthors)
            {
                authorTableBuilder.AddRow(author.Id, author.FullName);
            }

            var authorTable = authorTableBuilder.Build();
            new ConsoleRenderer().Print(authorTable);

            Console.Write("Yeni yazarın ID-sini daxil edin: ");
            string? authorInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(authorInput) || !int.TryParse(authorInput, out int authorId) || authorId <= 0)
            {
                Console.WriteLine("Yanlış yazar ID-si. Zəhmət olmasa düzgün bir ID daxil edin.");
                return;
            }

            var allGenres = _genreService.GetAll();
            if (!allGenres.Any())
            {
                Console.WriteLine("Janr siyahısı boşdur.");
                return;
            }

            var genreTableBuilder = new TableBuilder()
                .WithColumns("ID", "Janr Adı")
                .WithTheme(new AsciiTheme());

            foreach (var genre in allGenres)
            {
                genreTableBuilder.AddRow(genre.Id, genre.Name);
            }

            var genreTable = genreTableBuilder.Build();
            new ConsoleRenderer().Print(genreTable);

            Console.Write("Yeni janrın ID-sini daxil edin: ");
            string? genreInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(genreInput) || !int.TryParse(genreInput, out int genreId) || genreId <= 0)
            {
                Console.WriteLine("Yanlış janr ID-si. Zəhmət olmasa düzgün bir ID daxil edin.");
                return;
            }

            var dto = new BookUpdateDto
            {
                Id = bookId,
                Title = title,
                Price = price,
                Stock = stock,
                AuthorId = authorId,
                GenreId = genreId
            };

            try
            {
                var updatedBook = _bookService.Update(dto);

                var updatedBookTableBuilder = new TableBuilder()
                    .WithColumns("ID", "Title", "AuthorName", "Genre", "Price", "Stock")
                    .WithTheme(new AsciiTheme());

                updatedBookTableBuilder.AddRow(updatedBook.Id, updatedBook.Title, updatedBook.AuthorFullName, updatedBook.GenreName, updatedBook.Price, updatedBook.Stock);

                var updatedTable = updatedBookTableBuilder.Build();
                new ConsoleRenderer().Print(updatedTable);

                Console.WriteLine($"Kitab uğurla yeniləndi: {updatedBook.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xəta baş verdi: {ex.Message}");
            }
        }

        static void DeleteBook()
        {
            Console.WriteLine("\n================= Mövcud Kitablar =================");

            var allBooks = _bookService.GetAll(include: x => x.Include(x => x.Author).Include(x => x.Genre));
            if (!allBooks.Any())
            {
                Console.WriteLine("Kitab siyahısı boşdur.");
                return;
            }

            var builder = new TableBuilder()
                .WithColumns("ID", "Title", "AuthorName", "Genre", "Price", "Stock")
                .WithTheme(new AsciiTheme());

            foreach (BookDto book in allBooks)
            {
                builder.AddRow(book.Id, book.Title, book.AuthorFullName, book.GenreName, book.Price, book.Stock);
            }

            var table = builder.Build();
            new ConsoleRenderer().Print(table);

            Console.Write("\nSilmek istədiyiniz kitabın ID-sini daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.WriteLine("Yanlış kitab ID-si.");
                return;
            }

            try
            {
                var book = _bookService.GetById(bookId);
                if (book == null)
                {
                    Console.WriteLine("Bu kitab tapılmadı.");
                    return;
                }

                Console.WriteLine("\n================= Seçilmiş Kitab =================");

                var selectedBookBuilder = new TableBuilder()
                    .WithColumns("ID", "Title", "Price (AZN)")
                    .WithTheme(new AsciiTheme());

                selectedBookBuilder.AddRow(book.Id, book.Title, book.Price.ToString("F2"));

                var selectedBookTable = selectedBookBuilder.Build();
                new ConsoleRenderer().Print(selectedBookTable);

                Console.Write("\nBu kitabı silmək istədiyinizdən əminsiniz? (beli/xeyr): ");
                string? confirmation = Console.ReadLine()?.ToLower();

                if (confirmation != "beli")
                {
                    Console.WriteLine("Kitab silinmədi.");
                    return;
                }

                var result = _bookService.Delete(bookId);
                if (result != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"'{book.Title}' - adlı kitab uğurla silindi.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Kitab tapılmadı və silinmədi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xəta baş verdi: {ex.Message}");
            }
        }
    }
}
