using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using Console = Colorful.Console;

namespace Tilausjärjestelmä
{
    public class Items
    {
        public string applicationName = "TILAUSJÄRJESTELMÄ",
            applicationVersion = "Versio 1.3 (v.25.10.2019)",
            applicationCopyright = "Copyright (©) 2019 HAMK Development Team 02";
        public static void TextException(int errorCode) 
        {
            Console.Clear();
            if (errorCode == 100)
            {
                Console.Write(String.Format("{0}: Syötteessä oli virhe. Paina mitä tahansa näppäintä jatkaaksesi . . . ", errorCode), Color.Yellow);
                Console.ReadKey();
            }
            Console.Clear();
        } 
        public class Main
        {
            Items i = new Items();
            public void MenuText()
            {              
                Console.Clear();
                Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Päävalikko]"
                    + "\n\t\t1. Henkilökunta"
                    + "\n\t\t2. Asiakas"
                    + "\n\t\t3. Sulje ohjelma"
                    + "\n\n"
                    + "\n\t\tValitse valinta [1-3]: ");
            }
        }
        public class Staff
        {
            Items i = new Items();
            internal static string ProductName { get; set; }      
            public void MenuText()
            {
                Console.Clear();
                Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Henkilökunta]"
                    + "\n\t\t1. Tuotteiden lisääminen"
                    + "\n\t\t2. Muokkaa tilauksen tilaa"
                    + "\n\t\t3. Tilaukset"
                    + "\n\t\t4. Tuotteet"
                    + "\n\t\t5. Takaisin (Päävalikko)"
                    + "\n\t\t6. Sulje ohjelma"
                    + "\n\n"
                    + "\n\t\tValitse valinta [1-6]: ");
            }
            public void ShowOrders() //Luettele tilauksia (Vastaanotetut, käsitelty, sekä lähetetyt tilauksia)
            {
                SqlClient s = new SqlClient();
                StaffMenu sm = new StaffMenu();
                bool loopMenu = true;
                while (loopMenu)
                {
                    Console.Clear();
                    Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Tilaukset]"
                    + "\n\t\t1. Luettele vastaanotetut tilaukset"
                    + "\n\t\t2. Luettele käsitellyt tilaukset"
                    + "\n\t\t3. Luettele lähetetyt tilaukset"
                    + "\n\t\t4. Takaisin (Henkilökunta)"
                    + "\n\n"
                    + "\n\t\tValitse valinta [1-4]: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            s.DboReadOrders("'Vastaanotettu'");
                            break;
                        case "2":
                            s.DboReadOrders("'Käsitelty'");
                            break;
                        case "3":
                            s.DboReadOrders("'Lähetetty'");
                            break;
                        case "4":
                            sm.Menu();
                            break;
                        default:
                            Items.TextException(100);
                            break;
                    }
                }
            }

            public void ChangeOrderStatus() //Vaihda tilauksen tilaa (Vastaanotetuksi, käsiteltyksi, sekä lähetetyksi.)
            {
                SqlClient s = new SqlClient();
                StaffMenu sm = new StaffMenu();
                bool loopMenu = true;
                while (loopMenu)
                {
                    Console.Clear();
                    Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Muokkaa tilauksen tilaa]"
                    + "\n\t\t1. Merkkaa tilaus vastaanotetuksi"
                    + "\n\t\t2. Merkkaa tilaus käsitelltyksi"
                    + "\n\t\t3. Merkkaa tilaus lähetetyksi"
                    + "\n\t\t4. Takaisin (Henkilökunta)"
                    + "\n\n"
                    + "\n\t\tValitse valinta [1-4]: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            s.DboChangeOrderStatus("'vastaanotetuksi'");
                            break;
                        case "2":
                            s.DboChangeOrderStatus("'käsiteltyksi'");
                            break;
                        case "3":
                            s.DboChangeOrderStatus("'lähetetyksi'");
                            break;
                        case "4":
                            sm.Menu();
                            break;
                        default:
                            Items.TextException(100);
                            break;
                    }
                }
            }
        }
        public class Customer
        {
            Items i = new Items();
            public static string ProductQty { get; set; } //Connected to sqlClient.cs
            public static string ProductID { get; set; } //Connected to sqlClient.cs
            public static string ProductName { get; set; } //Connected to sqlClient.cs

            public void MenuText()
            {
                Console.Clear();
                Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Asiakas]"
                    + "\n\t\t1. Uusi tilaus"
                    + "\n\t\t2. Tuotteet"
                    + "\n\t\t3. Tilauksen muokkaaminen"
                    + "\n\t\t4. Apua"
                    + "\n\t\t5. Takaisin (Päävalikko)"
                    + "\n\t\t6. Sulje ohjelma"
                    + "\n\n"
                    + "\n\t\tValitse valinta [1-6]: ");
            }
            public void NewCustomerOrder()
            {
                CustomerMenu c = new CustomerMenu();
                SqlClient s = new SqlClient();

                bool loopProduct = true;
                while (loopProduct)
                {
                    Console.Clear();
                    Console.Write("\n\t\t[Uusi tilaus]");
                    Console.Write("\n\n\t\tAnna tuotteen sarjanumero: ");
                    
                    string productIDResult = Console.ReadLine().ToString();

                    if (productIDResult.Any(char.IsDigit)) //Tarkistaa onko tuote annettu sarjanumerona
                    {
                        Console.WriteLine("\n\t\t[Tarkistetaan kyseistä tuotetta]", Color.Yellow);
                        ProductID = productIDResult;
                        s.DboCheckID();
                        loopProduct = false;
                    }
                    else
                    {
                        Items.TextException(100);
                        NewCustomerOrder();
                    }
                }
                NewCustomerOrderQuantity();
            }
            public void NewCustomerOrderQuantity()
            {
                CustomerMenu c = new CustomerMenu();
                SqlClient s = new SqlClient();

                bool loopQty = true;
                while (loopQty)
                {    
                    Console.Write("\n\t\tAnna tuotteen määrä: "); //Tarkistaa tuotteen määrää
                    ProductQty = Console.ReadLine();
                    bool success = int.TryParse(ProductQty, out int value);
                    if (success)
                    {
                        if (value < 0)
                        {
                            TextException(100);
                            NewCustomerOrderQuantity();
                        }
                        Console.Write("\n\t\t[Tarkistetaan tuotteen saatavuus]", Color.Yellow);
                        s.DboCheckQty();
                        loopQty = false;
                    }
                    else
                    {
                        TextException(100);
                    }
                }
                c.Menu();
            }
            public void NewStaffProductInsert()
            {
                bool loopProduct = true;
                while (loopProduct)
                {
                    Console.Clear();
                    Console.Write("\n\t\t[Tuotteiden lisääminen]");
                    Console.Write("\n\n\t\tAnna tuotteen nimi: ");

                    ProductName = Console.ReadLine().ToString();

                    if (!Regex.IsMatch(ProductName, @"^[a-zA-Z ]+$")) //Text and whitespace
                    {
                        Items.TextException(100);
                        NewStaffProductInsert();
                    }
                    Console.Write("\n\t\tAnna tuotteen määrä: "); //Tarkistaa tuotteen määrää
                    ProductQty = Console.ReadLine();
                    int.TryParse(ProductQty, out int value);
                    if (value == 0)
                    {
                        TextException(100);
                        NewCustomerOrderQuantity();
                    }

                    Console.WriteLine(value); //ProductQty
                    Console.Write("\n\n\t\tAnna tuotteen hinta (x.xx): ");
                    //ProductPrice = Console.ReadLine();
                    Console.ReadKey();
                }
            }

            public void EditCustomerOrder() //Tilauksen muokkaaminen
            {
                SqlClient s = new SqlClient();
                CustomerMenu c = new CustomerMenu();

                bool loopMenu = true;
                while (loopMenu)
                {
                    Console.Clear();
                    Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Tilauksen muokkaaminen]"
                    + "\n\t\t1. Muokkaa tuotetta"
                    + "\n\t\t2. Muokkaa tuotteen määrään"
                    + "\n\t\t3. Muokkaa nimeä"
                    + "\n\t\t4. Muokkaa puhelinnumeroa"
                    + "\n\t\t5. Takaisin (Asiakas)"
                    + "\n\n"
                    + "\n\t\tValitse valinta [1-5]: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            s.DboChangeProduct(); 
                            break;
                        case "2":
                            //s.DboChangeQty(); 
                            break;
                        case "3":
                            //s.DboChangeCustomerName(); 
                            break;
                        case "4":
                           //s.DboChangeCustomerPhoneNumber();
                            break;
                        case "5":
                            c.Menu();
                            break;
                        default:
                            Items.TextException(100);
                            break;
                    }
                }
            }

            public static void Help() //Apua (Ohjeistusta asiakkaalle mahdollisista ongelmista)
            {
                Items i = new Items();

                Console.Clear();
                Console.Write($"\n\t\t{i.applicationName} {i.applicationVersion}"
                    + $"\n\t\t{i.applicationCopyright}"
                    + "\n\n"
                    + "\n\t\t[Apua]"
                    + "\n"
                    + "\n\t\tVirhetilanteiden tapahtuessa, seuraa virheilmoitusten ohjeita"
                    + "\n\t\tota tarpeeksitullen yhteyttä asiakaspalveluun."
                    + "\n\n\t\tPuhelin: 0441234567"
                    + "\n\t\tSähköposti: helpdesk@metsamaki.fi"
                    + "\n\n\t\tPaina mitä tahansa näppäintä jatkaaksesi . . .");

                Console.ReadKey();
            }
        }
    }
}

