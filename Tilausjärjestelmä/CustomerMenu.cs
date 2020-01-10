using System;

namespace Tilausjärjestelmä
{
    public class CustomerMenu
    {
        public void Menu()
        {
            SqlClient s = new SqlClient();
            Items.Customer i = new Items.Customer();
            MainMenu m = new MainMenu();

            bool loopMenu = true;
            while (loopMenu)
            {
                i.MenuText();

                switch (Console.ReadLine())
                {
                    case "1":
                        i.NewCustomerOrder(); //Luo uusi tilaus
                        break;
                    case "2":
                        s.DboReadProducts(); //Luettele kaikki tuotteet
                        break;
                    case "3":
                        i.EditCustomerOrder(); //Tilauksen muokkaaminen (KESKEENERÄINEN)
                        break;
                    case "4":
                        Items.Customer.Help(); //Apua 
                        break;
                    case "5":
                        m.Menu(); //Takaisin (Päävalikko)
                        break;
                    case "6":
                        System.Environment.Exit(1); //Sulje ohjelma
                        break;
                    default:
                        Items.TextException(100); //Expection 100 (Invalid input)
                        break;
                }
            }
        }      
    }
}