using BookStore.Application.DTOs.CustomerDtos;
using BookStore.Application.DTOs.OrderDetailDtos;
using BookStore.Application.DTOs.OrderDtos;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Application.Validators.OrderDetailValidator;
using BookStore.Application.Validators.OrderValidators;
using BookStore.Infrastructure.EFCore.DataContext;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TableTower.Core.Builder;
using TableTower.Core.Rendering;
using TableTower.Core.Themes;

namespace BookStore.UI.Menus
{
    public static class CustomersAndOrdersMenu
    {
        static IBookService _bookService = new BookManager();
        static ICustomerService _customerService = new CustomerManager();
        static IOrderService _orderService = new OrderManager();
        public static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Müşteriler ve Sifarişler Menyusu");
            Console.WriteLine("4.1 Yeni Müşteri Qeydiyyatı");
            Console.WriteLine("4.2 Yeni Sifariş Yarat");
            Console.WriteLine("4.3 Müşterinin sifarişlerini göster");
            Console.WriteLine("4.4 Sifarişlere Baxış");
            Console.WriteLine("0. Ana menyuya qayıt");
            Console.Write("Seçiminizi daxil edin: ");
        }

        public static void HandleMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "4.1":
                    AddCustomer();
                    break;
                case "4.2":
                    CreateOrder();
                    break;
                case "4.3":
                    ListCustomerOrders();
                    break;
                case "4.4":
                    OrderDetails();
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

        static void AddCustomer()
        {
            try
            {
                Console.Write("Müşterinin adı: ");
                string? name = Console.ReadLine();
                Console.Write("Müşterinin soyadı: ");
                string? surname = Console.ReadLine();
                Console.Write("Müşterinin Email ünvanı: ");
                string? email = Console.ReadLine();

                var existingCustomers = _customerService.GetAll();
                bool customerExists = existingCustomers.Any(c =>
                    c.FullName.Equals($"{name} {surname}", StringComparison.OrdinalIgnoreCase) &&
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (customerExists)
                {
                    Console.WriteLine("Bu müştəri artıq mövcuddur.");
                    return;
                }

                var dto = new CustomerCreateDto
                {
                    Name = name,
                    Surname = surname,
                    Email = email
                };

                var result = _customerService.Add(dto);

                var builder = new TableBuilder()
                    .WithColumns("ID", "FullName", "Email")
                    .WithTheme(new AsciiTheme());
                var customers = _customerService.GetAll();
                foreach (CustomerDto customer in customers)
                {
                    builder.AddRow(customer.Id, customer.FullName, customer.Email);
                }

                var table = builder.Build();
                new ConsoleRenderer().Print(table);

                Console.WriteLine($"Yeni müşteri elave olundu: {result.FullName}");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Yoxlama xetası: {string.Join(", ", ex.Errors.Select(e => e.ErrorMessage))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }

        static void CreateOrder()
        {
            try
            {
                Console.WriteLine("Mövcud Müşteriler:");
                var customers = _customerService.GetAll();

                var customerTableBuilder = new TableBuilder()
                    .WithColumns("ID", "FullName", "Email")
                    .WithTheme(new AsciiTheme());

                foreach (var customer in customers)
                {
                    customerTableBuilder.AddRow(customer.Id, customer.FullName, customer.Email);
                }

                var customerTable = customerTableBuilder.Build();
                new ConsoleRenderer().Print(customerTable);

                Console.Write("Sifariş üçün Müşteri ID daxil edin: ");
                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    Console.WriteLine("Yanlış ID.");
                    return;
                }

                Console.WriteLine("Mövcud Kitablar:");

                var books = _bookService.GetAll();

                var bookTableBuilder = new TableBuilder()
                    .WithColumns("ID", "Title", "AuthorName", "Genre", "Price", "Stock")
                    .WithTheme(new AsciiTheme());

                foreach (var book in books)
                {
                    bookTableBuilder.AddRow(book.Id, book.Title, book.AuthorFullName, book.GenreName, book.Price, book.Stock);
                }

                var bookTable = bookTableBuilder.Build();
                new ConsoleRenderer().Print(bookTable);

                List<OrderDetailDto> orderDetails = new();
                decimal totalPrice = 0;
                string choice = "he";

                do
                {
                    Console.Write("Kitab ID daxil edin: ");
                    if (!int.TryParse(Console.ReadLine(), out int bookId))
                    {
                        Console.WriteLine("Yanlış kitab ID-si.");
                        continue;
                    }

                    var selectedBook = _bookService.GetById(bookId);
                    if (selectedBook == null)
                    {
                        Console.WriteLine("Kitab tapılmadı.");
                        continue;
                    }

                    Console.Write("Eded sayını daxil edin: ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Yanlış eded.");
                        continue;
                    }

                    int availableStock = selectedBook.Stock;
                    int totalQuantity = orderDetails.Where(d => d.BookId == bookId).Sum(d => d.Quantity) + quantity;

                    if (totalQuantity > availableStock)
                    {
                        Console.WriteLine($"Yalnız {availableStock} eded mövcuddur. Hal-hazırda {orderDetails.Where(d => d.BookId == bookId).Sum(d => d.Quantity)} eded almışsınız.");
                        continue;
                    }

                    var orderDetail = new OrderDetailDto
                    {
                        BookId = bookId,
                        Quantity = quantity,
                        UnitPrice = selectedBook.Price,
                        BookStock = selectedBook.Stock
                    };

                    var orderDetailValidator = new OrderDetailDtoValidator();
                    var orderDetailValidationResult = orderDetailValidator.Validate(orderDetail);

                    if (!orderDetailValidationResult.IsValid)
                    {
                        Console.WriteLine("\n Order Detail Validation Errors: ");
                        foreach (var failure in orderDetailValidationResult.Errors)
                        {
                            Console.WriteLine($"- {failure.ErrorMessage}");
                        }
                        continue;
                    }

                    orderDetails.Add(orderDetail);
                    totalPrice += selectedBook.Price * quantity;
                    selectedBook.Stock -= quantity;

                    Console.Write("Başqa kitab elave etmek isteyirsiniz? (he / yox): ");
                    choice = (Console.ReadLine() ?? "").Trim().ToLower();

                } while (choice == "he");

                Console.WriteLine("\n---------- Sifariş Melumatları -------");

                var orderTableBuilder = new TableBuilder()
                    .WithColumns("Book ID", "Quantity", "Unit Price", "Total Price")
                    .WithTheme(new AsciiTheme());

                foreach (var detail in orderDetails)
                {
                    decimal total = detail.Quantity * detail.UnitPrice;
                    orderTableBuilder.AddRow(detail.BookId, detail.Quantity, detail.UnitPrice, total);
                }

                var orderTable = orderTableBuilder.Build();
                new ConsoleRenderer().Print(orderTable);

                Console.WriteLine($"Umumi Mebleg: {totalPrice} AZN");

                var orderDto = new OrderCreateDto
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    OrderDetails = orderDetails
                };

                if (orderDto.OrderDate > DateTime.Now)
                {
                    Console.WriteLine("Sifariş tarixi gelecekde ola bilmez.");
                    return;
                }

                var orderValidator = new OrderCreateDtoValidator();
                var orderValidationResult = orderValidator.Validate(orderDto);

                if (!orderValidationResult.IsValid)
                {
                    Console.WriteLine("\n Order Validation Errors: ");
                    foreach (var failure in orderValidationResult.Errors)
                    {
                        Console.WriteLine($"- {failure.ErrorMessage}");
                    }
                    return;
                }

                var result = _orderService.Add(orderDto);

                Console.WriteLine($"\nSifariş uğurla yaradıldı! Sifariş No : {result.Id}, Mebleg: {result.TotalPrice} AZN");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }

        static void OrderDetails()
        {
            try
            {
                var orders = _orderService.GetAll(include: query => query.Include(o => o.Customer)
                                                 .Include(o => o.OrderDetails).ThenInclude(od => od.Book)
                );

                Console.WriteLine("\n *****Bütün Sifarişler:*****");

                var builder = new TableBuilder()
                    .WithColumns("ID", "CustomerName", "OrderDate", "Total")
                    .WithTheme(new AsciiTheme());

                foreach (OrderDto order in orders)
                {
                    builder.AddRow(order.Id, order.CustomerName, order.OrderDate, order.TotalPrice);
                }

                var table = builder.Build();
                new ConsoleRenderer().Print(table);

                if (!orders.Any())
                {
                    Console.WriteLine("Heç bir sifariş tapılmadı.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }

        public static void ListCustomerOrders()
        {
            try
            {
                Console.WriteLine("\n================= Mövcud Müşteriler =================");

                var customers = _customerService.GetAll();

                if (!customers.Any())
                {
                    Console.WriteLine("Heç bir müşteri tapılmadı.");
                    return;
                }

                var customerTableBuilder = new TableBuilder()
                    .WithColumns("ID", "FullName")
                    .WithTheme(new AsciiTheme());

                foreach (var cust in customers)
                {
                    customerTableBuilder.AddRow(cust.Id, cust.FullName);
                }

                var customerTable = customerTableBuilder.Build();
                new ConsoleRenderer().Print(customerTable);

                Console.WriteLine("\n=====================================================");
                Console.Write("Müşteri ID-sini daxil edin: ");

                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    Console.WriteLine("Yanlış ID formatı.");
                    return;
                }

                var customer = _customerService.GetById(customerId);

                if (customer == null)
                {
                    Console.WriteLine("Müşteri tapılmadı.");
                    return;
                }

                Console.WriteLine($"\n{customer.FullName} adlı müşterinin sifarişleri: ");

                using (var context = new BookDbContext())
                {
                    var orders = context.Orders
                        .Where(order => order.CustomerId == customerId)
                        .Include(order => order.OrderDetails)
                        .ThenInclude(detail => detail.Book)
                        .ToList();

                    if (!orders.Any())
                    {
                        Console.WriteLine("Bu müşterinin heç bir sifarişi yoxdur.");
                        return;
                    }

                    foreach (var order in orders)
                    {
                        var combinedTableBuilder = new TableBuilder()
                            .WithColumns("Order No", "OrderDate", "Book", "Stock", "UnitPrice (AZN)", "Total (AZN)")
                            .WithTheme(new RoundedTheme());

                        foreach (var detail in order.OrderDetails)
                        {
                            combinedTableBuilder.AddRow(
                                order.Id,
                                order.OrderDate.ToString("dd.MM.yyyy"),
                                //order.TotalPrice.ToString("F2"),
                                detail.Book.Title,
                                detail.Quantity,
                                detail.UnitPrice.ToString("F2"),
                                (detail.UnitPrice * detail.Quantity).ToString("F2")
                            );
                        }

                        var combinedTable = combinedTableBuilder.Build();
                        new ConsoleRenderer().Print(combinedTable);

                        Console.WriteLine($" Total: {order.OrderDetails.Count} row(s)");
                   
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta baş verdi: {ex.Message}");
            }
        }
    }
}

