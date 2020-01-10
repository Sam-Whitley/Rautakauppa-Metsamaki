using System;
using System.Data.SqlClient;
using System.Drawing;
using Console = Colorful.Console;
using System.Linq;
using System.Text;
using static Tilausjärjestelmä.Items;

namespace Tilausjärjestelmä
{
    /// SQLClient | Handles SQL Database methods
    /// NOTE: If you are try to run this application in a lab environment (e.g HAMK), you might run in "some" connection issues.
    /// This might be caused by the networks firewall or proxy rule that only allows for TCP traffic over a certain port.
    /// I'll try to check if I can bypass this issue in someway...
    
    /// Methods: DboInsert (INSERT VALUES) | DboUpdate (UPDATE, SET) | DboRead (SELECT * FROM) | DboCheckID (SELECT ID, ProductName, ProductQty FROM *)
    /// Public variable: ProductID, ProductName, ProductQty, QtyResult
    /// Local/private variables: ProductPrice, ProductTotalQty, CustomerName, CustomerPhoneNumber
    public class SqlClient
    {
        public string ProductID { get => Customer.ProductID; set => Customer.ProductID = value; }
        public string ProductName { get => Customer.ProductName; set => Customer.ProductName = value; }
        public string ProductQty { get => Customer.ProductQty; set => Customer.ProductQty = value; }

        public string OrderQty { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerOrderID { get; set; }
        public string CustomerPassword { get; set; }
        public string NewCustomerPrice { get; set; }
        public int NewOrderQty { get; set; }
        public int QtyResult { get; set; }

        public SqlClient()
        {

        }   

        public void DboInsertProduct() ///Inserts new products into the dbo.Product table (Add products)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open(); ///Open SqlConnection
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Products VALUES(@Name, @Qty, @Price)", con))
                    {
                        cmd.Parameters.AddWithValue("@Name", ProductName);
                        cmd.Parameters.AddWithValue("@Qty", ProductQty);
                        //cmd.Parameters.AddWithValue("@Price", ProductPrice);

                        int rows = cmd.ExecuteNonQuery(); ///Executes SQL-statement, returns number of rows affected
                    }
                }
            }
            catch (Exception e) ///Display Exception
            {
                Console.WriteLine($"\n\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }
        public void DboChangeProduct()
        {
            SqlClient s = new SqlClient();

            bool loopProduct = true;
            while (loopProduct)
            {
                Console.Clear();
                Console.WriteLine("\n\t\t[Muokkaa tuotetta]");
                Console.Write("\n\t\tAnna puhelinnumeroasi: ");
                CustomerPhoneNumber = Console.ReadLine().ToString();
                if (CustomerPhoneNumber.Any(char.IsDigit))
                {
                    Console.Write("\n\t\tAnna salasanasi: ");
                    CustomerPassword = Console.ReadLine().ToString();
                    if (CustomerPassword.Any(char.IsLetterOrDigit)) 
                    {
                        DboCheckPhoneNumberAndPassword();  //Check if phone number and password exists
                        loopProduct = false;
                    }
                    else
                    {
                        Items.TextException(100);
                    }

                }
                else
                {
                    Items.TextException(100);
                }

                Console.Write("\n\t\tAnna tuotteen sarjanumeroa mihin haluat vaihtaa: ");
                ProductID = Console.ReadLine().ToString();
                if (ProductID.Any(char.IsDigit)) //Tarkistaa onko tuotenimi char
                {
                    Console.WriteLine("\n\t\t[Tarkistetaan kyseistä tuotetta]", Color.Yellow);
                    s.DboCheckIDChange();
                    loopProduct = false;
                }
                else
                {
                    Items.TextException(100);
                }
                Console.WriteLine($"New productID, Name and Price: {ProductID}, {ProductName} and {NewCustomerPrice} {NewOrderQty}");
                Console.ReadKey();

                //DboUpdateOrder();
            }
        }
        public void DboUpdateOrder() ///Update new products into the dbo.Order table | Work in progress
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Orders SET ProductQty = ProductQty - @Qty WHERE ProductName = @Name", con))
                    {
                        cmd.Parameters.AddWithValue("@Name", ProductName);
                        cmd.Parameters.AddWithValue("@Qty", ProductQty);
                        //cmd.Parameters.AddWithValue("@Price", ProductPrice); TODO: FIX THIS

                        int rows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e) ///Display Exception
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }

        public void DboCheckPhoneNumberAndPassword()
        {
            CustomerMenu cm = new CustomerMenu();

            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT ID, ProductName, OrderQty, OrderPrice, OrderStatus From dbo.Orders WHERE CustomerPhoneNumber = @CustomerPhoneNumber AND CustomerPassword = @CustomerPassword", con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerPhoneNumber", CustomerPhoneNumber.ToString());
                        cmd.Parameters.AddWithValue("@CustomerPassword", CustomerPassword.ToString());

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bool loopTrue = true;
                                while (loopTrue)
                                {
                                    if (reader.GetString(4) != "Vastaanotettu")
                                    {
                                        Console.Write("\n\t\tTilauksesi on jo käsiltettu ja sitä ei voida enään muokata! Paina mitä tahansa näppäintä palataakseen päävalikkoon . . .", Color.Red);
                                        Console.ReadKey();
                                        cm.Menu();
                                    }
                                    Console.OutputEncoding = Encoding.UTF8; ///This is used for euro sign (\u20AC)
                                    Console.WriteLine("\n\t\t[Tilaus löytyi] Tilaus ID: {0} | Tuotenimi: {1} | Määrä: {2} kpl | Hinta: {3}\u20AC | Tila: {4}",
                                        reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3), reader.GetString(4), Color.LimeGreen);
                                    NewOrderQty = reader.GetInt32(2);

                                    Console.Write("\t\tOnko tilaus oikea? (K/E): ", Color.Yellow);

                                    switch (Console.ReadLine())
                                    {
                                        case "k":
                                            loopTrue = false;
                                            break;
                                        case "e":
                                            loopTrue = false;
                                            DboCheckPhoneNumberAndPassword();
                                            break;
                                        default:
                                            Items.TextException(100);
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool loopTrue = true;
                            while (loopTrue)
                            {
                                Console.Write("\n\t\tTilausta ei löytynyt! Etsi uudestaan? (K/E): ", Color.Red);

                                SqlClient s = new SqlClient();
                                CustomerMenu c = new CustomerMenu();

                                switch (Console.ReadLine())
                                {
                                    case "k":
                                        loopTrue = false;
                                        DboChangeProduct();
                                        break;
                                    case "e":
                                        loopTrue = false;
                                        c.Menu();
                                        break;
                                    default:
                                        Items.TextException(100);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.ReadLine();
                return;
            }
        }

        public void DboUpdate(string arrange) ///Update order status in the dbo.Orders table
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Orders SET OrderStatus = @OrderStatus WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@OrderStatus", arrange);
                        cmd.Parameters.AddWithValue("@ID", CustomerOrderID);

                        int rows = cmd.ExecuteNonQuery();

                    }
                }
                Console.WriteLine("\t\t[Tilaus tila päivitetty!]", Color.LimeGreen);
                Console.Write("\n\t\tPaina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();

            }
            catch (Exception e) ///Display Exception
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }
        public void DboReadProducts() ///Read products in the dbo.Product table and list them out for the user
        {
            Console.Clear();
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {                  
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Products ORDER BY ProductName;", con))
                    {
                        
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows) ///Check if SqlDataReader has rows
                        {
                            Console.WriteLine("\n\t\t[Tuotteet]" +
                                "\n\n\t\tID:" +
                                "\tTuotteen nimi:\t" +
                                "\t\t\t\t\tMäärä:" +
                                "\t\tHinta:");

                            while (reader.Read())
                            {
                                Console.OutputEncoding = Encoding.UTF8; ///This is used for euro sign (\u20AC)
                                string tableProducts = String.Format("\t\t{0,0}\t{1,0}\t\t\t\t{2,0} kpl\t\t{3,0}\u20AC", 
                                    reader.GetInt32(0), reader.GetString(1).PadRight(25), reader.GetInt32(2), reader.GetDecimal(3));

                                Console.WriteLine(tableProducts);                                  
                            }
                        }
                        else
                        {
                            Console.WriteLine("Tuotteita ei löytynyt");
                        }
                        reader.Close(); ///Close SqlDataReader
                        
                    }
                    Console.Write("\n\t\tPaina mitä tahansa näppäintä jatkaaksesi . . .");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }
        public void DboChangeOrderStatus(string arrange)
        {
            SqlClient s = new SqlClient();

            bool loopProduct = true;
            while (loopProduct)
            {
                Console.Clear();
                Console.WriteLine($"\n\t\t[Merkkaa tilausta {arrange.Trim('\'')}]");
                Console.Write("\n\t\tAnna tilauksen ID: ");
                CustomerOrderID = Console.ReadLine().ToString();
                if (CustomerOrderID.Any(char.IsDigit))
                {
                    Console.WriteLine("\n\t\t[Tarkistetaan kyseistä tuotetta]", Color.Yellow);
                    DboCheckOrderID(arrange);
                    loopProduct = false;
                }
                else
                {
                    Items.TextException(100);
                }
            }
        }
        public void DboReadOrders(string arrange) ///Read orders in the dbo.Orders table and list them out for the user
        {
            Console.Clear();
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();
                    //SELECT* FROM dbo.Orders WHERE OrderStatus = 'Vastaanotettu' ORDER BY Id
                    using (SqlCommand cmd = new SqlCommand($"SELECT * FROM dbo.Orders WHERE OrderStatus = {arrange} ORDER BY Id;", con))
                    {

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            Console.WriteLine("\n\t\t[Tilaukset]"
                                + "\n\n\t\tTilaus ID:"
                                + "\tTuoteen ID:"
                                + "\tNimi:"
                                + "\t\t\t\t\tMäärä:"
                                + "\t    Yhteishinta:"
                                + "\tAsiakkaan nimi:"
                                + "\t\tPuhelinnumero:"
                                + "\t\tTila:"
                                + "\t\t\tTilauspäivä:");

                            while (reader.Read())
                            {
                                Console.OutputEncoding = Encoding.UTF8; ///For euro sign (\u20AC)

                                string tableOrders = String.Format("\t\t{0,0}\t\t{1,0}\t\t{2,0}\t\t\t{3,0} kpl\t    {4,0}\u20AC\t\t{5,0}{6,0}\t\t{7,0}\t\t{8,0}",
                                    reader.GetInt32(0), reader.GetInt32(1),
                                    reader.GetString(2).PadRight(20), reader.GetInt32(3),
                                    reader.GetDecimal(4), reader.GetString(5).PadRight(24),
                                    reader.GetString(6), reader.GetString(7), reader.GetString(8));

                                Console.WriteLine(tableOrders);
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n\t\t[Tilaukset]");
                            Console.WriteLine($"\n\t\t{arrange.Trim('\'')} tilauksia ei löytynyt!", Color.Yellow);
                        }
                        reader.Close();
                    }
                    Console.Write("\n\t\tPaina mitä tahansa näppäintä jatkaaksesi . . .");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }
        //Using parameter here because SqlCommand can't "access" them for some reason. Didn't get an error code. Fix this later. This updates both dbo.Product and dbo.Order databases 
        public void DboOrder(int productID, string productName, int productQty, decimal productPrice, 
            int productTotalQty, string customerName, string customerPhoneNumber, string customerPassword)
        {
            try //Try to update [dbo.Product] table
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Products SET ProductQty = ProductQty - @Qty WHERE ProductName = @Name", con))
                    {
                        cmd.Parameters.AddWithValue("@Name", productName);
                        cmd.Parameters.AddWithValue("@Qty", productQty);
                        int rows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }

            try //Insert dbo.Orders
            {
                int.TryParse(ProductQty, out int result);
                QtyResult = result;
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Orders (ProductID, ProductName, OrderQty, OrderPrice, CustomerName, CustomerPhoneNumber, OrderStatus, Timestamp, CustomerPassword) " +
                        "VALUES (@ID, @Name, @Qty, @Price, @CustomerName, @PhoneNumber, @OrderStatus, @Timestamp, @CustomerPassword);", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", productID);
                        cmd.Parameters.AddWithValue("@Name", productName);
                        cmd.Parameters.AddWithValue("@Qty", productQty);
                        cmd.Parameters.AddWithValue("@Price", productPrice);
                        cmd.Parameters.AddWithValue("@CustomerName", customerName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", customerPhoneNumber);
                        cmd.Parameters.AddWithValue("@OrderStatus", "Vastaanotettu");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("dd/MM/yyyy klo HH.mm"));
                        cmd.Parameters.AddWithValue("@CustomerPassword", customerPassword);

                        int rows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }
        public void DboCheckQty()
        {          
            try
            {             
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();

                    Customer c = new Customer();
                    using (SqlCommand cmd = new SqlCommand("SELECT ID, ProductName, ProductQty, ProductPrice FROM dbo.Products WHERE ID = @ID AND ProductQty >= @Qty;", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", ProductID);
                        cmd.Parameters.AddWithValue("@Qty", ProductQty);
                        int rows = cmd.ExecuteNonQuery();

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {                     
                            while (reader.Read())
                            {
                                bool loopTrue = true;
                                while (loopTrue)
                                {
                                    Console.OutputEncoding = Encoding.UTF8; ///For euro sign
                                    Console.WriteLine("\n\t\t[Varasto saatavuus] Varastossa: {0} kpl | Tilattu määrä {1} kpl",
                                        reader.GetInt32(2), ProductQty, Color.LimeGreen);

                                    Console.Write("\n\t\tOnko tuotemäärä oikein? (K/E): ", Color.Yellow);
                                    switch (Console.ReadLine())
                                    {
                                        case "k":
                                            int intProductQty = int.Parse(ProductQty);
                                            decimal finalPrice = reader.GetDecimal(3) * intProductQty;

                                            Console.WriteLine("\n\t\t[Tilauksen yhteenveto] ID: {0} | Tuotenimi: {1} | Määrä: {2} kpl | Hinta {3}\u20AC",
                                                reader.GetInt32(0), reader.GetString(1), ProductQty,
                                                reader.GetDecimal(3) * intProductQty, Color.LimeGreen);

                                            Console.Write("\n\t\tAnna nimesi: ");
                                            string customerName = Console.ReadLine();

                                            Console.Write("\n\t\tAnna puhelinnumerosi (+358): ");
                                            string customerPhoneNumber = Console.ReadLine();

                                            //Generate 12 char password
                                            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                                            var stringChars = new char[12];
                                            var random = new Random();

                                            for (int i = 0; i < stringChars.Length; i++)
                                            {
                                                stringChars[i] = chars[random.Next(chars.Length)];
                                            }

                                            string customerPassword = new String(stringChars);

                                            DboOrder(reader.GetInt32(0), reader.GetString(1), intProductQty, finalPrice,
                                                reader.GetInt32(2), customerName, customerPhoneNumber, customerPassword);

                                            Console.Write($"\n\t\t[Tilaus lähetetty] Jos haluat muokata tilauksesi tiedot, "
                                                + $"voit tehdä sen [Tilauksen muokkaaminen] valikosta. "
                                                + $"\n\t\tKäyttäjänimesi on {customerPhoneNumber} ja salasanasi on {customerPassword})", Color.LimeGreen);

                                            Console.WriteLine($"\n\t\t(Tilausta ei voida muokata enään kun tilauksesi on lähetetty.", Color.Yellow);
                                            Console.Write("\n\t\tPaina mitä tahansa näppäintä palataksesi valikkoon . . .");
                                            Console.ReadKey();

                                            loopTrue = false;
                                            break;
                                        case "e":
                                            loopTrue = false;
                                            Console.Clear();
                                            c.NewCustomerOrderQuantity();
                                            break;
                                        default:
                                            Items.TextException(100);
                                            break;
                                    }
                                }                                
                            }                          
                        }
                        else
                        {
                            bool loopTrue = true;
                            while (loopTrue)
                            {
                                SqlClient s = new SqlClient();
                                Console.Write("\n\t\t[Tilaamaasi määrä meni varastonsaldon yli!]", Color.Red);

                                Console.Write("\n\t\tVaihda määrä? (K/E): ", Color.Yellow);

                                switch (Console.ReadLine())
                                {
                                    case "k":
                                        loopTrue = false;
                                        Console.Clear();
                                        c.NewCustomerOrderQuantity();
                                        break;
                                    case "e":
                                        loopTrue = false;
                                        break;
                                    default:
                                        Items.TextException(100);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.WriteLine("Paina mitä tahansa näppäintä jatkaaksesi . . . ");
                Console.ReadKey();
            }
        }
        public void DboCheckID()
        {
            Items.Customer i = new Items.Customer();
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT ID, ProductName, ProductQty, ProductPrice From dbo.Products WHERE ID = @ID;", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", ProductID.ToString());

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bool loopTrue = true;
                                while (loopTrue)
                                {
                                    Console.WriteLine("\n\t\t[Tuote löytyi] ID: {0} | Tuotenimi: {1} | Määrä: {2} kpl", 
                                        reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), Color.LimeGreen);

                                    Console.Write("\t\tOnko tuote oikea? (K/E): ", Color.Yellow);

                                    switch (Console.ReadLine())
                                    {
                                        case "k":
                                            ProductID = reader.GetInt32(0).ToString();
                                            ProductName = reader.GetString(1);
                                            NewCustomerPrice = reader.GetDecimal(3).ToString();
                                            loopTrue = false;
                                            break;
                                        case "e":
                                            loopTrue = false;
                                            i.NewCustomerOrder();
                                            break;
                                        default:
                                            Items.TextException(100);
                                            break;
                                    }                                      
                                }
                            }
                        }
                        else
                        {
                            bool loopTrue = true;
                            while (loopTrue)
                            {
                                Console.SetCursorPosition(6, 5);
                                Console.Write("\t\tTuotetta ei löytynyt! Etsi uudestaan? (K/E): ", Color.Red);

                                SqlClient s = new SqlClient();
                                CustomerMenu c = new CustomerMenu();

                                switch (Console.ReadLine())
                                {
                                    case "k":
                                        loopTrue = false;
                                        i.NewCustomerOrder();
                                        break;
                                    case "e":
                                        loopTrue = false;
                                        c.Menu();
                                        break;
                                    default:
                                        Items.TextException(100);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.ReadLine();
                return;
            }
        }

        public void DboCheckOrderID(string arrange)
        {
            Items.Staff s = new Items.Staff();
            StaffMenu sm = new StaffMenu();
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT ID, ProductName, OrderQty, OrderPrice, OrderStatus From dbo.Orders WHERE ID = @ID;", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", CustomerOrderID.ToString());

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bool loopTrue = true;
                                while (loopTrue)
                                {
                                    Console.OutputEncoding = Encoding.UTF8; ///For euro sign (\u20AC)
                                    Console.WriteLine("\n\t\t[Tilaus löytyi] ID: {0} | Tuotenimi: {1} | Määrä: {2} kpl | Hinta: {3}\u20AC | Tila: {4}",
                                        reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3), reader.GetString(4), Color.LimeGreen);

                                    Console.Write("\t\tOnko tilaus oikea? (K/E): ", Color.Yellow);

                                    switch (Console.ReadLine())
                                    {
                                        case "k":
                                            
                                            if (arrange == "'vastaanotetuksi'")
                                            {
                                                Console.WriteLine($"\n\t\tMerkitään tilausta vastaanotetuksi . . .", Color.Yellow);
                                                DboUpdate("Vastaanotettu");
                                            }
                                            if (arrange == "'käsiteltyksi'")
                                            {
                                                Console.WriteLine($"\n\t\tMerkitään tilausta käsiteltyksi . . .", Color.Yellow);
                                                DboUpdate("Käsitelty");
                                            }
                                            else if (arrange == "'lähetetyksi'")
                                            {
                                                Console.WriteLine($"\n\t\tMerkitään tilausta lähetetyksi . . .", Color.Yellow);
                                                DboUpdate("Lähetetty");
                                            }
                                            loopTrue = false;
                                            break;
                                        case "e":
                                            loopTrue = false;
                                            s.ChangeOrderStatus();
                                            break;
                                        default:
                                            Items.TextException(100);
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool loopTrue = true;
                            while (loopTrue)
                            {
                                Console.SetCursorPosition(6, 5);
                                Console.Write("\t\tTilausta ei löytynyt! Etsi uudestaan? (K/E): ", Color.Red);

                                switch (Console.ReadLine())
                                {
                                    case "k":
                                        loopTrue = false;
                                        s.ChangeOrderStatus();
                                        break;
                                    case "e":
                                        loopTrue = false;
                                        sm.Menu();
                                        break;
                                    default:
                                        Items.TextException(100);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.ReadLine();
                return;
            }
        }

        public void DboCheckIDChange()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlClientConnectionString"].ConnectionString))
                {
                    con.Open();

                    Items.Customer i = new Items.Customer();
                    using (SqlCommand cmd = new SqlCommand("SELECT ID, ProductName, ProductQty, ProductPrice From dbo.Products WHERE ID = @ID;", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", ProductID.ToString());

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bool loopTrue = true;
                                while (loopTrue)
                                {
                                    Console.WriteLine("\n\t\t[Tuote löytyi] ID: {0} | Tuotenimi: {1} | Määrä: {2} kpl",
                                        reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), Color.LimeGreen);

                                    Console.Write("\t\tOnko tuote oikea? (K/E): ", Color.Yellow);

                                    switch (Console.ReadLine())
                                    {
                                        case "k":
                                            ProductID = reader.GetInt32(0).ToString();
                                            ProductName = reader.GetString(1);
                                            NewCustomerPrice = reader.GetDecimal(3).ToString();
                                            Console.WriteLine($"{NewOrderQty}");
                                            Console.WriteLine(reader.GetDecimal(3));
                                            decimal test = (reader.GetDecimal(3));
                                            Console.WriteLine($"test: {test.ToString()}");

                                            Console.WriteLine($"Working? {NewCustomerPrice}");
                                            loopTrue = false;
                                            break;
                                        case "e":
                                            loopTrue = false;
                                            i.NewCustomerOrder();
                                            break;
                                        default:
                                            Items.TextException(100);
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool loopTrue = true;
                            while (loopTrue)
                            {
                                Console.SetCursorPosition(6, 5);
                                Console.Write("\t\tTuotetta ei löytynyt! Etsi uudestaan? (K/E): ", Color.Red);

                                SqlClient s = new SqlClient();
                                CustomerMenu c = new CustomerMenu();

                                switch (Console.ReadLine())
                                {
                                    case "k":
                                        loopTrue = false;
                                        i.NewCustomerOrder();
                                        break;
                                    case "e":
                                        loopTrue = false;
                                        c.Menu();
                                        break;
                                    default:
                                        Items.TextException(100);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t\t{GetType().Name}() | {e.Message}", Color.Yellow);
                Console.ReadLine();
                return;
            }
        }
    }
}