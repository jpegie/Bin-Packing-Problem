namespace BinPP
{
    internal class AutoTesting
    {
        //[0] - BF, [1] - FF, [2] - FFS
        List<Test> tests;
        Packing pack_type;
        ExcelTable exl;
        public AutoTesting(int amnt_testing_items, int tests_amnt, string path_out)
        {
            exl = new ExcelTable(path_out);
            tests = new List<Test>();
            for (int i = 0; i < tests_amnt; ++i)
            {
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
            return new(bin_amnt, sw.ElapsedTicks);
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
            foreach (Test t in tests)
                t.PrintSlim();
        }
    }
}
