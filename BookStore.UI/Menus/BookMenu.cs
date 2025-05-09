using BookStore.Application.DTOs.BookDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using FluentValidation;

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
            Console.Write("Kitabın adı: ");
            string? title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Kitabın adı boş ola bilmez.");
                return;
            }

            Console.Write("Kitabın qiymeti (AZN): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("Qiymet düzgün deyil. Zehmet olmasa müsbet qiymet daxil edin.");
                return;
            }

            Console.Write("Kitabın stok miqdarı: ");
            if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
            {
                Console.WriteLine("Stok miqdarı düzgün deyil. Zehmet olmasa müsbet ve ya sıfırdan böyük say daxil edin.");
                return;
            }

            Console.Write("Kitabın janrını daxil edin (ID): ");
            if (!int.TryParse(Console.ReadLine(), out int genreId) || genreId <= 0)
            {
                Console.WriteLine("Yanlış janr ID-si.");
                return;
            }

            Console.Write("Kitabin muellifinin ID-sini daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int authorId) || authorId <= 0)
            {
                Console.WriteLine("Yanlış muellif ID-si.");
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

            try
            {
                var result = _bookService.Add(dto);
                Console.WriteLine($"Yeni kitab elave olundu: {result?.Title ?? title}");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Xeta: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir xeta baş verdi: {ex.Message}");
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
                    var bookByTitle = _bookService.Get(b => b.Title != null && b.Title.Contains(title ?? ""));
                    if (bookByTitle != null)
                        Console.WriteLine($"Tapıldı: {bookByTitle.Title}, Qiymet: {bookByTitle.Price}");
                    else
                        Console.WriteLine("Kitab tapılmadı.");
                    break;

                case "2":
                    Console.Write("Müellif adı ve ya soyadı: ");
                    string? authorSearch = Console.ReadLine();
                    var authors = _authorService.GetAll()
                        .Where(a => (a.Name + " " + a.Surname).Contains(authorSearch ?? "", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (authors.Any())
                    {
                        foreach (var author in authors)
                        {
                            Console.WriteLine($"Müellif: {author.Name} {author.Surname}");

                            if (author.Books.Any())
                            {
                                foreach (var book in author.Books)
                                {
                                    Console.WriteLine($" Kitab: {book.Title}, Qiymet: {book.Price}, Stok: {book.Stock}");
                                }
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
                        Console.WriteLine("Janr ve kitab tapılmadı.");
                    else
                    {
                        foreach (var genre in genres)
                        {
                            foreach (var book in genre.Books)
                            {
                                Console.WriteLine($"Kitab: {book.Title}, Janr: {genre.Name}, Qiymet: {book.Price}");
                            }
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
            var books = _bookService.GetAll();
            Console.WriteLine("Kitab Siyahısı:");
            foreach (var book in books)
            {
                Console.WriteLine($"ID: {book.Id}, Ad: {book.Title}, Qiymet: {book.Price}, Stok: {book.Stock}");
            }
        }

        static void EditBook()
        {
            Console.Write("Redakte etmek istediyiniz kitabin ID-sini daxil edin: ");
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

            Console.WriteLine($"Mövcud kitab: {book.Title}, Qiymet: {book.Price} AZN, Stok: {book.Stock}");

            Console.Write("Yeni kitabın adını daxil edin: ");
            string? title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Kitab adı boş ola bilmez.");
                return;  
            }

            Console.Write("Yeni qiymeti daxil edin (AZN): ");
            decimal price;
            if (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Qiymet düzgün deyil.");
                return;
            }

            Console.Write("Yeni stok miqdarını daxil edin: ");
            int stock;
            if (!int.TryParse(Console.ReadLine(), out stock))
            {
                Console.WriteLine("Stok miqdarı düzgün deyil.");
                return;
            }
            Console.Write("Yeni yazarın ID-sini daxil edin: ");
            string? authorInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(authorInput) || !int.TryParse(authorInput, out int authorId) || authorId <= 0)
            {
                Console.WriteLine("Yanlış yazar ID-si. Zehmet olmasa düzgün bir ID daxil edin.");
                return;
            }

            Console.Write("Yeni janrın ID-sini daxil edin: ");
            string? genreInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(genreInput) || !int.TryParse(genreInput, out int genreId) || genreId <= 0)
            {
                Console.WriteLine("Yanlış janr ID-si. Zehmet olmasa düzgün bir ID daxil edin.");
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
                Console.WriteLine($"Kitab uğurla yenilendi: {updatedBook.Title}, Qiymet: {updatedBook.Price} AZN, Stok: {updatedBook.Stock}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }

        static void DeleteBook()
        {
            Console.Write("Silmek istediyiniz kitabın ID-sini daxil edin: ");
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

                Console.WriteLine($"Silmek istediyiniz kitab: {book.Title}, Qiymet: {book.Price} AZN, Stok: {book.Stock}");

                Console.Write("Bu kitabı silmek istediyinizden eminsiniz? (beli/xeyr): ");
                string? confirmation = Console.ReadLine()?.ToLower();

                if (confirmation != "beli")
                {
                    Console.WriteLine("Kitab silinmedi.");
                    return;
                }

                var result = _bookService.Delete(bookId);
                if (result != null)
                {
                    Console.WriteLine($"Kitab uğurla silindi: {book.Title}");
                }
                else
                {
                    Console.WriteLine("Kitab tapılmadı ve silinmedi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }
    }
}