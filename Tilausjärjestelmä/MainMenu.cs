using System;

namespace Tilausjärjestelmä
{
    public class MainMenu
    {
        public void Menu()
        {
            CustomerMenu c = new CustomerMenu();
            StaffMenu s = new StaffMenu();
            Items.Main m = new Items.Main();

            bool loopMenu = true;
            while (loopMenu)
            {
                m.MenuText();

                switch (Console.ReadLine())
                {
                    case "1":
                        s.Menu(); //Henkilökunta (Staff)
                        break;
                    case "2":
                        c.Menu(); //Asiakas (Customer)
                        break;
                    case "3":
                        Environment.Exit(1); //Exit
                        break;
                    default:
                        Items.TextException(100); //Expection 100 (Invalid input)
                        break;
                }
            }
        }
    }
}