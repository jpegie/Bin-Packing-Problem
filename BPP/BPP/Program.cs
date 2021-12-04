using System.Collections.Generic;
using System.IO;
using System;

namespace BPP
{
    class BPP_program
    {
        class Item
        {
            int index;
            double weight;
            public Item(int index, double weight)
            {
                this.index = index;
                this.weight = weight;
            }
            public int Index
            {
                get { return index; }
            }
            public double Weight
            {
                get { return weight; }
            }
        }
        class Bin
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
                if(FreeCapacity() < item.Weight)
                    return false;
                items.Add(item);
                current_capacity += item.Weight;
                return true;
            }
            /*public int TotalCapacity
            {
                get { return total_capacity; }
            }*/

            /*public double CurrentCapacity()
            {
                return current_capacity;
            }*/
            private double FreeCapacity()
            {
                return total_capacity - current_capacity;
            }
        }
        class Packing
        {
            List<List<Item>> items_combs = new List<List<Item>>();
            List<Item> items = new List<Item>();
            List<Bin> bins = new List<Bin>();
            int bin_capacity = 0;
            public Packing(List<Item> items, int bin_capacity)
            {
                this.items = items;
                this.bin_capacity = bin_capacity;
            }

            //перебор всех возможных n! комбинаций предметов с целью поиска лучшей для умещения в меньшее кол-во контейнеров
            //буду использовать алгоритм FF
            public List<Bin> BruteForce()
            {
                //сгенерировать i-ую комбинацию
                //применить FF
                //сохранить информацию о заполненных контейнерах и саму i-ую комбинацию
                //найти лучшую комбинацию

                int best_bin = 0;

                List<Bin> best_bin_pack = new List<Bin>();
                List<List<Bin>> bins_combs = new List<List<Bin>>();

                PermuteItems(items, 0);

                for(int i=0;i<items_combs.Count ;++i)
                {
                    items = items_combs[i];
                    bins.Clear();
                    FF();
                    bins_combs.Add(bins);
                }
                for (int i = 1; i < bins_combs.Count; ++i)
                    if (bins_combs[i].Count < bins_combs[best_bin].Count)
                        best_bin = i;
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
                return bins;
            }

            //"первый подходящий" только в отсортированном списке предметов
            public List<Bin> FFS()
            {
                items.Sort((x, y) => x.Weight.CompareTo(y.Weight));
                return FF();
            }
        }

        void TestGenerator()
        {

        }

        static void Main()
        {
            List<Bin> bins = new List<Bin>();       int bin_capacity = 0;
            List<Item> items = new List<Item>();    int items_amnt = 0;
            Packing packing; 

            Console.Write("Кол-во предметов: "); int.TryParse(Console.ReadLine(), out items_amnt);
            
            for (int i = 0; i < items_amnt; ++i)
            {
                double weight = 0;
                Console.Write($"Вес {i + 1} предмета: "); double.TryParse(Console.ReadLine(), out weight);
                items.Add(new Item(i, weight));
            }

            Console.Write("Размерность контейнера: "); int.TryParse(Console.ReadLine(), out bin_capacity);
            Console.Write(
                        "Выберите тип упаковки:\n" + 
                        "1. Перебор всех возможных комбинаций предметов для нахождеия лучшего случая\n" + 
                        "2. FF\n" + 
                        "3. FFS\n"
                         );
            packing = new Packing(items, bin_capacity);
            int pack_id; int.TryParse(Console.ReadLine(), out pack_id);
            switch (pack_id)
            {
                case (1):
                {
                        bins = packing.BruteForce();
                        break;
                }
                case (2):
                {
                        bins = packing.FF();
                        break;
                }
                case (3):
                {
                        bins = packing.FFS();
                        break;
                }
            }

            Console.WriteLine($"Кол-во использованных контейнеров: {bins.Count}");
            for (int i = 0; i < bins.Count; ++i)
            {
                Console.WriteLine($"Предметы контейнера {i + 1}: ");
                foreach (Item item in bins[i].Items)
                    Console.WriteLine($"[{item.Index}] : {item.Weight}");
                Console.WriteLine(new String('-', 25));
            }
        }
    }
}