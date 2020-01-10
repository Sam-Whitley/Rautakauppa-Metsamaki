using System;

namespace Tilausjärjestelmä
{
    public class StaffMenu
    {
        public void Menu()
        {
            bool loopMenu = true;
            SqlClient s = new SqlClient();
            MainMenu m = new MainMenu();
            Items.Staff i = new Items.Staff();
            Items.Customer c = new Items.Customer();

            while (loopMenu)
            {
                i.MenuText();

                switch (Console.ReadLine())
                {
                    case "1":
                        c.NewStaffProductInsert(); //Tuotteiden lisääminen 
                        break;
                    case "2":
                        i.ChangeOrderStatus(); //Muokkaa tilauksen tilaa
                        break;
                    case "3":
                       i.ShowOrders(); //Tilaukset
                        break;
                    case "4":
                        s.DboReadProducts(); //Tuotteet
                        break;
                    case "5":
                        m.Menu(); //Takaisin (Päävalikko)
                        break;
                    case "6":
                        System.Environment.Exit(1); //Sulje ohjelma
                        break;
                    default:
                        Items.TextException(100); //Invalid input
                        break;
                }
            }
        }
    }
}