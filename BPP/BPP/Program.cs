global using System.Collections.Generic;
global using System.Diagnostics;
global using Microsoft.Data.Sqlite;

namespace BinPP //bin packing problem
{
    class BinPP_program
    {
        //[0] - BF, [1] - FF, [2] - FFS
        public string path = @"C:\git\bin_packing_problem\tests_{0}_items";
        class DataBase
        {
            static string connection_string = @"Data Source=C:\git\bin_packing_problem\db_input.db";
            string table_name = "", add_query = "", find_max_query = "", get_query = "";
            SqliteConnection connection = new SqliteConnection(connection_string);
            SqliteCommand command;
            SqliteDataReader reader;
            public DataBase(string table_name)
            {
                this.table_name = table_name;
                add_query = "INSERT INTO "  + table_name + "(ID,AMOUNT,CAPACITY,ITEMS) Values ({0},{1},{2},{3})";
                find_max_query = "SELECT MAX({0}) FROM " + table_name;
                get_query = "SELECT * from " + table_name;
                command = connection.CreateCommand();

            }
            
            public int GetSize { get { return GetMaxID(); } }

            private int GetMaxID()
            {
                connection.Open();
                command.CommandText = String.Format(find_max_query, "ID");
                int max = 0;
                try
                {
                    max = int.Parse(command.ExecuteScalar().ToString());
                }
                catch (Exception) {}
                connection.Close();
                return max;
            }
            public void AddCase(int AMOUNT, int CAPACITY, List<Item>ITEMS)
            {
                int ID = GetMaxID() + 1;
                connection.Open();
                Console.WriteLine($"ID = {ID}");
                SqliteCommand prepared_items = new SqliteCommand();
                command.CommandText = String.Format(add_query, ID, AMOUNT, CAPACITY, ItemsConverter(ITEMS));
                command.Prepare();
                Console.WriteLine(String.Format(add_query, ID, AMOUNT, CAPACITY, ItemsConverter(ITEMS)));
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Строка в БД добавлена!");
                }
                catch (Exception) {}
                connection.Close();
                
                using (var connection = new SqliteConnection())
                {
                    connection.Open();
                    SqliteCommand command = connection.CreateCommand();
                    int max_id; string query;

                    query = "SELECT MAX(ID) from Cases";
                    command.CommandText = query;

                    try
                    {
                        max_id = int.Parse(command.ExecuteScalar().ToString());
                        Console.WriteLine($"Max ID = {max_id}");
                    }
                    catch (Exception) { }

                    query = "INSERT INTO Cases (ID,AMOUNT,CAPACITY,ITEMS) Values(1,15,15,15)";
                    command.CommandText = query;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception) { }
                    connection.Close();
                }
            }
            private string ItemsConverter(List<Item> ITEMS)
            {
                string result="'";
                foreach(Item itm in ITEMS)
                    result += $"{itm.Index}/{itm.Weight} ";
                result += "'";
                return result;
            }
            public (int, int, List<Item>) GetRow(int ID)
            {
                connection.Open();
                int AMOUNT = 0, CAPACITY = 0;
                string ITEMS_IN = "";
                List<Item> ITEMS_OUT = new List<Item>();
                command.CommandText = get_query;
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (int.Parse(reader.GetValue(0).ToString()) == ID)
                        {
                            AMOUNT = int.Parse(reader.GetValue(1).ToString());
                            CAPACITY = int.Parse(reader.GetValue(2).ToString());
                            ITEMS_IN = reader.GetValue(3).ToString();
                            break;
                        }
                    }
                }
                string[] only_splitted_items = ITEMS_IN.Split(' ');
                for(int i=0;i<AMOUNT ; ++i)
                {
                    string[] ITEM = only_splitted_items[i].Split('/');
                    ITEMS_OUT.Add(new Item(int.Parse(ITEM[0]), int.Parse(ITEM[1])));
                }
                reader.Close();
                connection.Close();
                return (AMOUNT, CAPACITY, ITEMS_OUT);
            }
            public void PrintRow((int, int, List<Item>) row)
            {
                Console.WriteLine($"Кол-во предметов: {row.Item1}\nРазмерность контейнера: {row.Item2}");
                for (int i = 0; i < row.Item3.Count; ++i)
                {
                    Console.Write($"{row.Item3[i].Index} / {row.Item3[i].Weight}\t");
                }
                Console.WriteLine();
            }
        }
        static void Main()
        {
            //DataBase db = new DataBase("Cases");
            List<Bin> bins = new List<Bin>(); int bin_capacity = 0;
            List<Item> items = new List<Item>(); int items_amnt = 0;
            Packing packing;
            
            Test test = null;

            /*for(int i=0;i<db.GetSize ; ++i)
            {
                rows.Add(db.GetRow(i));
                db.PrintRow(rows[i]);
            }*/

            bool gen_next = true;
            bool continue_testing = true;
            Console.Write("Запустить автоматическое тестирование (0/1) : "); 
            if (Console.ReadLine() == "0") continue_testing = false;
            while(continue_testing)
            {
                int amnt_testing_items;
                int tests_amnt;
                string path = "";
                Console.Write("[AutoTest] Кол-во предметов для теста : "); amnt_testing_items = int.Parse(Console.ReadLine());
                Console.Write("[AutoTest] Кол-во тестов : "); tests_amnt = int.Parse(Console.ReadLine());
                AutoTesting testing = new AutoTesting(amnt_testing_items, tests_amnt, path);
                for(int i=0;i<tests_amnt ; ++i)
                    testing.Start(i);
                for (int i = 0; i < tests_amnt; ++i)
                    testing.PrintResult(i);
                Console.Write("Запустить еще тестирование (0/1) : ");
                if (Console.ReadLine() == "0") continue_testing = false;
            }

            bool use_same = false;
            while (gen_next)
            {
                if (test != null)
                {
                    Console.Write("Использовать тот же тест (1/0)?");
                    if (Console.ReadLine() == "1") use_same = true;
                    else use_same = false;
                }
                if(!use_same) Console.Write("Генерация ручная - 0, автоматическая - 1 : ");
                
                if(!use_same && Console.ReadLine() == "0")
                {
                    test = new Test();
                    test.AddManualy();
                    test.Print();
                }
                else if(!use_same)
                {
                    Console.Write("[Auto] Кол-во предметов : "); items_amnt = int.Parse(Console.ReadLine());
                    test = new Test();
                    test.Generate(items_amnt);
                    test.Print();
                }
                Console.Write("Упаковка в контейнеры (0 - BF, 1 - FF, 2 - FFS) : "); int pack_id = int.Parse(Console.ReadLine());
                packing = new Packing(test);
 
                switch (pack_id)
                {
                    case 0:
                        {
                            bins = packing.BruteForce();
                            packing.PrintBins(bins);
                            break;
                        }
                    case 1:
                        {
                            bins = packing.FF();
                            packing.PrintBins(bins);
                            break;
                        }
                    case 2:
                        {
                            bins = packing.FFS();
                            packing.PrintBins(bins);
                            break;
                        }
                }
                use_same = false;
                Console.Write("Обработать еще один тест (1/0)? : ");
                if (Console.ReadLine() == "0") gen_next = false;
            }
        }
    }
}