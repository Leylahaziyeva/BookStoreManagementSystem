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

                var dto = new CustomerCreateDto
                {
                    Name = name,
                    Surname = surname,
                    Email = email
                };

                var result = _customerService.Add(dto);

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
                foreach (var customer in customers)
                {
                    Console.WriteLine($"ID: {customer.Id}, Ad: {customer.FullName}");
                }

                Console.Write("Sifariş üçün Müşteri ID daxil edin: ");
                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    Console.WriteLine("Yanlış ID.");
                    return;
                }

                Console.WriteLine("Mövcud Kitablar: ");
                var books = _bookService.GetAll();
                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.Id}, Ad: {book.Title}, Qiymet: {book.Price} AZN, Stok: {book.Stock}");
                }

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

                    if (quantity > selectedBook.Stock)
                    {
                        Console.WriteLine($"Yalnız {selectedBook.Stock} eded mövcuddur.");
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

                Console.WriteLine("\n--- Sifariş Melumatları ---");
                Console.WriteLine($"Order Date (Local): {DateTime.Now}");
                Console.WriteLine($"Order Details Count: {orderDetails.Count}");
                foreach (var detail in orderDetails)
                {
                    Console.WriteLine($"Book ID: {detail.BookId}, Quantity: {detail.Quantity}, Unit Price: {detail.UnitPrice}");
                }
                Console.WriteLine("---------------------------\n");

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
                var orders = _orderService.GetAll();

                if (!orders.Any())
                {
                    Console.WriteLine("Heç bir sifariş tapılmadı.");
                    return;
                }

                Console.WriteLine("\n **Bütün Sifarişler: **");

                foreach (var order in orders)
                {
                    Console.WriteLine($"\n Sifariş No : {order.Id}");
                    Console.WriteLine($" Müşteri: {order.CustomerName}");
                    Console.WriteLine($" Tarix: {order.OrderDate:dd/MM/yyyy}");
                    Console.WriteLine($" Mebleğ: {order.TotalPrice} AZN");
                    Console.WriteLine(" **Kitablar:**");

                    if (order.OrderDetails.Any())
                    {
                        foreach (var detail in order.OrderDetails)
                        {
                            Console.WriteLine($"Kitab: {detail.BookTitle} | Miqdar: {detail.Quantity} eded | Qiymet: {detail.UnitPrice} AZN | Cemi: {detail.UnitPrice * detail.Quantity} AZN");
                        }
                    }
                    else
                    {
                        Console.WriteLine(" Kitab melumatı tapılmadı.");
                    }

                    Console.WriteLine(new string('-', 50)); 
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
                        Console.WriteLine($"\nSifariş No : {order.Id}");
                        Console.WriteLine($"Tarix: {order.OrderDate:dd/MM/yyyy}");
                        Console.WriteLine($"Cem Mebleg: {order.TotalPrice} AZN");
                        Console.WriteLine(" **Kitablar:**");

                        foreach (var detail in order.OrderDetails)
                        {
                            Console.WriteLine($"Kitab: {detail.Book.Title} | Miqdar: {detail.Quantity} | Qiymet: {detail.UnitPrice} AZN | Cemi: {detail.UnitPrice * detail.Quantity} AZN");
                        }

                        Console.WriteLine(new string('-', 50)); 
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

