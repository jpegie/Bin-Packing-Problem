namespace BinPP
{
    internal class Test
    {
        List<Item> items; int items_amnt;
        List<Bin> bins; int bin_capacity;

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
            bin_capacity = max_item + new Random().Next(1, max_item / 2);
        }
        public void AddManualy()
        {
            items = new List<Item>();
            Console.Write("Введите кол-во предметов: "); items_amnt = int.Parse(Console.ReadLine());
            Console.Write("Введите вместимость контейнера: "); bin_capacity = int.Parse(Console.ReadLine());
            for (int i = 0; i < items_amnt; ++i)
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
}
