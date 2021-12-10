using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Excel = Microsoft.Office.Interop.Excel;

namespace BinPP //bin packing problem
{
    class BinPP_program
    {
        public string path = @"C:\git\bin_packing_problem\tests_{0}_items";
        class ExcelTable
        { 
            Excel.Application app;
            Excel.Workbook wb;
            Excel.Worksheet sheet;
            string path;
            public ExcelTable(string path)
            {
                app = new Excel.Application();
                app.DisplayAlerts = true;
                wb = app.Workbooks.Add(Type.Missing);
                app.SheetsInNewWorkbook = 1;
                sheet = (Excel.Worksheet)app.Worksheets[1];
                this.path = path;

                sheet.Cells[1, 1].Value = "Кол-во предметов";
                sheet.Cells[1, 2].Value = "Номер теста";
                sheet.Cells[1, 3].Value = "Решение BF";
                sheet.Cells[1, 4].Value = "Время BF";
                sheet.Cells[1, 5].Value = "Решение FF";
                sheet.Cells[1, 6].Value = "Время FF";
                sheet.Cells[1, 7].Value = "Решение FFS";
                sheet.Cells[1, 8].Value = "Время FFS";

            }
            void AddTest(int items_amnt, int test_i, int bf_amnt, double bf_time, int ff_amnt, int ff_time, int ffs_amnt, double ffs_time)
            {
                //test_i - индекс строки, куда добавлять
                //test_i > 0, а первая строка - шапка, поэтому + 2
                sheet.Cells[test_i + 2, 1].Value = items_amnt;
                sheet.Cells[test_i + 2, 2].Value = test_i;
                sheet.Cells[test_i + 2, 3].Value = bf_amnt;
                sheet.Cells[test_i + 2, 4].Value = bf_time;
                sheet.Cells[test_i + 2, 5].Value = ff_amnt;
                sheet.Cells[test_i + 2, 6].Value = ff_time;
                sheet.Cells[test_i + 2, 7].Value = ffs_amnt;
                sheet.Cells[test_i + 2, 8].Value = ffs_time;
            }
            public void SaveTable()
            {
                wb.SaveAs(path);
            }
        }
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
        public class Item
        {
            int index;
            int weight;
            public Item(int index, int weight)
            {
                this.index = index;
                this.weight = weight;
            }
            public int Index
            {
                get { return index; }
            }
            public int Weight
            {
                get { return weight; }
            }
        }
        public class Bin
        {
            int total_capacity = 0;
            double current_capacity = 0;
            
            List<Item> items = new List<Item>();
            public Bin(int total_capacity)
            {
                this.total_capacity = total_capacity;
            }
            public List<Item> Items
            {
                get { return items; }
            }
            public bool AddItem(Item item)
            {
                if(FreeCapacity < item.Weight)
                    return false;
                items.Add(item);
                current_capacity += item.Weight;
                return true;
            }

            private double FreeCapacity 
            {
                get { return total_capacity - current_capacity; }
            }
        }
        public class Packing
        {
            List<Bin> bins = new List<Bin>();       int bin_capacity = 0;
            List<Item> items = new List<Item>();
            List<List<Item>> items_combs = new List<List<Item>>();
            List<PackResult> pack_results = new List<PackResult>();
            Stopwatch sw = Stopwatch.StartNew();
            public Packing(Test test)
            {
                this.items = new List<Item>(test.Items);
                this.bin_capacity = test.BinCapacity;
                for(int i=0; i<3; ++i)
                    pack_results.Add(new PackResult(0, 0));
            }

            //перебор всех возможных n! комбинаций предметов с целью поиска лучшей для умещения в меньшее кол-во контейнеров
            //буду использовать алгоритм FF
            public List<Bin> BruteForce()
            {
                //сгенерировать i-ую комбинацию
                //применить FF
                //сохранить информацию о заполненных контейнерах и саму i-ую комбинацию
                //найти лучшую комбинацию;
                sw.Restart();
                int best_bin = 0; //индекс комбинации с минимальным кол-вом использованных контейнеров
                List<List<Bin>> bins_combs = new List<List<Bin>>(); //все возможные комбиеации заполнения контейнеров
                PermuteItems(items, 0);
                for(int i=0;i<items_combs.Count ;++i)
                {
                    items = items_combs[i];
                    bins_combs.Add(FF());
                }
                for (int i = 1; i < bins_combs.Count; ++i)
                    if (bins_combs[i].Count < bins_combs[best_bin].Count)
                        best_bin = i;
                sw.Stop();
                pack_results[0].TimeAmnt = sw.ElapsedTicks;
                pack_results[0].BinAmnt = bins_combs[best_bin].Count;
                return bins_combs[best_bin];
            }
            private void PermuteItems(List<Item> perm, int start)
            {
                if (start == items.Count)
                {
                    items_combs.Add(new List<Item>(perm));
                }
                else
                {
                    for (int j = start; j < items.Count; ++j)
                    {
                        SwapItems(ref perm, start, j);
                        PermuteItems(perm, start + 1);
                        SwapItems(ref perm, start, j);
                    }
                }
            }
            private void SwapItems(ref List<Item> items_list, int item1_i, int item2_i)
            {
                Item buff = items_list[item1_i];
                items_list[item1_i] = items_list[item2_i];
                items_list[item2_i] = buff;
            }
            
            //"первый подходящий"
            public List<Bin> FF()
            {
                sw.Restart();
                bins.Clear();
                bins.Add(new Bin(bin_capacity)); //добавляем первый контейнер
                bool found_fit_bin = false;
                for(int i=0;i<items.Count ; ++i)
                {
                    found_fit_bin = false;
                    //проход по всем контейнерам с целью поиска первого подходящего
                    for (int j=0; j<bins.Count && !found_fit_bin; ++j) 
                    {
                        found_fit_bin = bins[j].AddItem(items[i]);
                    }
                    //если подхожящего контейнера не найдено, то добавим новый контейнер
                    if (!found_fit_bin)
                    {
                        bins.Add(new Bin(bin_capacity));
                        bins[bins.Count - 1].AddItem(items[i]);
                    }
                }
                sw.Stop();
                pack_results[1].TimeAmnt = sw.ElapsedTicks;
                pack_results[1].BinAmnt = bins.Count;
                return bins;
            }

            //"первый подходящий" только в отсортированном списке предметов

            public List<Bin> FFS()
            {
                Stopwatch sw_1 = Stopwatch.StartNew(); 
                items.Sort((x, y) => x.Weight.CompareTo(y.Weight));
                List<Bin> ff = FF();
                sw_1.Stop();
                pack_results[2].TimeAmnt = sw_1.ElapsedTicks;
                pack_results[2].BinAmnt = bins.Count;
                return ff;
            }
            public void PrintBins(List<Bin>bins)
            {
                Console.WriteLine($"Кол-во контейнеров : {bins.Count}");
                for(int i=0;i<bins.Count ; ++i)
                {
                    Console.Write($"{i + 1} : ");
                    foreach (Item item in bins[i].Items)
                        Console.Write($"[{item.Index}] {item.Weight} ");
                    Console.WriteLine();
                }
                Console.Write(" - BF : ");
                if (pack_results[0].TimeAmnt == 0) Console.Write("-");
                else Console.Write($"{pack_results[0].BinAmnt} | {pack_results[0].TimeAmnt} тиков");

                Console.Write("\n - FF: ");
                if (pack_results[1].TimeAmnt == 0) Console.Write("-");
                else Console.Write($"{pack_results[1].BinAmnt} | {pack_results[1].TimeAmnt} тиков");

                Console.Write("\n - FFS: ");
                if (pack_results[2].TimeAmnt == 0) Console.Write("-");
                else Console.Write($"{pack_results[2].BinAmnt} | {pack_results[2].TimeAmnt} тиков");
                Console.WriteLine();
            }
        }
        public class Test
        {
            List<Item> items;   int items_amnt;
            List<Bin> bins;     int bin_capacity;

            //long[] time_pack = { 0, 0, 0 };
            //int[] bin_packs = { 0, 0, 0 };
            List<PackResult> pack_results = new List<PackResult>();
            public Test()
            {
                for (int i = 0; i < 3; ++i)
                    pack_results.Add(new PackResult(0, 0));
            }
            public void Generate(int items_amnt)
            {
                if (items_amnt == 0) this.items_amnt = new Random().Next(1, 100);
                else this.items_amnt = items_amnt;
                items = new List<Item>();
                for (int i = 0; i < items_amnt; ++i)
                    items.Add(new Item(i, new Random().Next(1, 100)));
                bins = new List<Bin>();
                int max_item = items.Max(x => x.Weight);
                bin_capacity = max_item + new Random().Next(1,max_item/2);
            }
            public void AddManualy()
            {
                items = new List<Item>();
                Console.Write("Введите кол-во предметов: "); items_amnt = int.Parse(Console.ReadLine());
                Console.Write("Введите вместимость контейнера: "); bin_capacity = int.Parse(Console.ReadLine());
                for(int i=0;i<items_amnt ; ++i)
                {
                    int weight;
                    Console.Write($"- Вес {i + 1} предмета: "); weight = int.Parse(Console.ReadLine());
                    items.Add(new Item(i, weight));
                }
            }
            public void Print()
            {
                Console.WriteLine("-- Входные данные --");
                Console.WriteLine($"Вместимость контейнера: {bin_capacity}");
                Console.WriteLine($"Кол-во предметов: {items_amnt}\nПредметы [индекс, вес]:");
                int cols_in_output = 5;
                for (int i = 0; i < items_amnt; ++i)
                {
                    Console.Write($"[{items[i].Index}] {items[i].Weight}\t");
                    if (i % cols_in_output == 0 && i != 0) Console.WriteLine();
                }
                Console.WriteLine();
            }
            public void PrintSlim()
            {
                Console.Write($"* {bin_capacity}\t| {items_amnt} | : ");
                foreach (Item item in items)
                    Console.Write($"[{item.Index}] {item.Weight}\t");
                Console.WriteLine();
            }
            public List<Item> Items
            {
                get { return items; }
            }
            public int BinCapacity
            {
                get { return bin_capacity; }
            }
            public List<Bin> Bins
            {
                get { return bins; }    
            }
            public void SetResult(int pack_id, PackResult res)
            {
                pack_results[pack_id] = res;
            }
            public PackResult GetResult(int pack_id)
            {
                return pack_results[pack_id];
            }
        }
        public class PackResult
        {
            private int bin_amnt;
            private long time;
            public PackResult(int bins, long time)
            {
                this.time = time;
                this.bin_amnt = bins;
            }
            public int BinAmnt
            {
                get { return bin_amnt; }
                set { bin_amnt = value; }
            }
            public long TimeAmnt
            {
                get { return time; }
                set { time = value; }
            }
        }
        public class AutoTesting
        {
            List<Test> tests;
            //List<ResultTest> results; 
            Packing pack_type;
            ExcelTable exl;
            public AutoTesting(int amnt_testing_items, int tests_amnt, string path_out)
            {
                exl = new ExcelTable(path_out);
                tests = new List<Test>();
                //results = new List<ResultTest>(); //[0] - BF, [1] - FF, [2] - FFS
                for (int i = 0; i < tests_amnt; ++i)
                {
                    //results.Add(new ResultTest());
                    tests.Add(new Test());
                    tests[i].Generate(amnt_testing_items);
                } 
            }
            public void Start(int test_i)
            {
                // 0 - BF, 1 - FF, 2 - FFS
                PackResult result;

                result = BF_test(test_i);
                tests[test_i].SetResult(0, result);

                result = FF_test(test_i);
                tests[test_i].SetResult(1, result);
                
                result = FFS_test(test_i);
                tests[test_i].SetResult(2, result);
            }
            public void WriteToFile()
            {
                
            }

            private PackResult BF_test(int test_i)
            {
                Stopwatch sw = Stopwatch.StartNew();
                int bin_amnt = 0;
                pack_type = new Packing(tests[test_i]);
                bin_amnt = pack_type.BruteForce().Count;
                sw.Stop();
                return new (bin_amnt, sw.ElapsedTicks);
            }
            private PackResult FF_test(int test_i)
            {
                Stopwatch sw = Stopwatch.StartNew();
                int bin_amnt = 0;
                pack_type = new Packing(tests[test_i]);
                bin_amnt = pack_type.FF().Count;
                sw.Stop();
                return new PackResult(bin_amnt, sw.ElapsedTicks);
            }
            private PackResult FFS_test(int test_i)
            {
                Stopwatch sw = Stopwatch.StartNew();
                int bin_amnt = 0;
                pack_type = new Packing(tests[test_i]);
                bin_amnt = pack_type.FFS().Count;
                sw.Stop();
                return new PackResult(bin_amnt, sw.ElapsedTicks);
            }
            public void PrintResult(int test_i)
            {
                PackResult res;
                res = tests[test_i].GetResult(0);
                Console.WriteLine($"- BF  : {res.BinAmnt} | {res.TimeAmnt} ms");
                res = tests[test_i].GetResult(1);
                Console.WriteLine($"- FF  : {res.BinAmnt} | {res.TimeAmnt} ms");
                res = tests[test_i].GetResult(2);
                Console.WriteLine($"- FFS : {res.BinAmnt} | {res.TimeAmnt} ms");
                tests[test_i].PrintSlim();
            }
            public void PrintTests()
            {
                foreach(Test t in tests)
                    t.PrintSlim();
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