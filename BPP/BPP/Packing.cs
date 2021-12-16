namespace BinPP
{
    internal class Packing
    {
        List<Bin> bins = new List<Bin>(); int bin_capacity = 0;
        List<Item> items = new List<Item>();
        List<List<Item>> items_combs = new List<List<Item>>();
        List<PackResult> pack_results = new List<PackResult>();
        Stopwatch sw = Stopwatch.StartNew();
        public Packing(Test test)
        {
            this.items = new List<Item>(test.Items);
            this.bin_capacity = test.BinCapacity;
            for (int i = 0; i < 3; ++i)
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
            for (int i = 0; i < items_combs.Count; ++i)
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
            for (int i = 0; i < items.Count; ++i)
            {
                found_fit_bin = false;
                //проход по всем контейнерам с целью поиска первого подходящего
                for (int j = 0; j < bins.Count && !found_fit_bin; ++j)
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
            items.Sort((x, y) => y.Weight.CompareTo(x.Weight));
            List<Bin> ff = FF();
            sw_1.Stop();
            pack_results[2].TimeAmnt = sw_1.ElapsedTicks;
            pack_results[2].BinAmnt = bins.Count;
            return ff;
        }
        public void PrintBins(List<Bin> bins)
        {
            Console.WriteLine($"Кол-во контейнеров : {bins.Count}");
            for (int i = 0; i < bins.Count; ++i)
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
}
