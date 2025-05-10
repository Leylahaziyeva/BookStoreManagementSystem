using BookStore.UI.Menus;
using System.Text;
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        RunMainMenu();
    }
    static void RunMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("*** BOOKSTORE SYSTEM ***\n");
            Console.WriteLine("1. Müellifler");
            Console.WriteLine("2. Janrlar");
            Console.WriteLine("3. Kitablar");
            Console.WriteLine("4. Müşteriler ve Sifarişler");
            Console.WriteLine("0. Çıxış");
            Console.Write("Seçiminizi daxil edin: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AuthorsMenu.DisplayMenu();
                    AuthorsMenu.HandleMenuChoice(Console.ReadLine());
                    break;
                case "2":
                    GenresMenu.DisplayMenu();
                    GenresMenu.HandleMenuChoice(Console.ReadLine());
                    break;
                case "3":
                    BooksMenu.DisplayMenu();
                    BooksMenu.HandleMenuChoice(Console.ReadLine());
                    break;
                case "4":
                    CustomersAndOrdersMenu.DisplayMenu();
                    CustomersAndOrdersMenu.HandleMenuChoice(Console.ReadLine());
                    break;
                case "0":
                    Console.WriteLine("Çıxılır...");
                    return;
                default:
                    Console.WriteLine("Yanlış seçim. Davam etmek üçün Enter basın.");
                    Console.ReadLine();
                    break;
            }
        }
    }
}