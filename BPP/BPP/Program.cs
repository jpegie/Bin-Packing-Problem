﻿using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;

namespace BinPP //bin packing problem
{
    class BinPP_program
    {
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
            List<List<Item>> items_combs = new List<List<Item>>();
            List<Item> items = new List<Item>();
            List<Bin> bins = new List<Bin>();
            Stopwatch taken_time = new Stopwatch();
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
                taken_time.Start();
                if (items.Count > 10) {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Метод перебора завершает свою работу за конечное время только с кол-вом предметов не более 10!");
                    Console.BackgroundColor = ConsoleColor.Black;
                    return bins;
                }
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
                taken_time.Stop();
                return bins_combs[best_bin];
            }
            public Stopwatch GetTakenTime()
            {
                Stopwatch s = taken_time;
                taken_time.Reset();
                return s;
                //Console.WriteLine($"Затраченное время (мс, кол-во тиков): {taken_time.ElapsedMilliseconds}, {taken_time.ElapsedTicks}");
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
                taken_time.Start();
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
                taken_time.Stop();
                return bins;
            }

            //"первый подходящий" только в отсортированном списке предметов

            public List<Bin> FFS()
            {
                taken_time.Start();
                items.Sort((x, y) => x.Weight.CompareTo(y.Weight));
                taken_time.Stop();
                return FF();
            }
        }

        static public void TestGenerator(ref List<Item> items, ref int bin_capacity, ref int items_amnt)
        {
            Console.Write("генерирую");
            items_amnt = new Random().Next(1,100);
            items = new List<Item>();
            for(int i=0;i<items_amnt ; ++i)
                items.Add(new Item(i, new Random().Next(1, 100)));
            bin_capacity = items.Max(x => x.Weight) + new Random().Next(1, 100); //вместивмость контейнера делаем больше, чем максимальный вес предмета, чтобы избежать невместимость
            Console.WriteLine("Сгенерированные входные данные:");
            Console.WriteLine($"Кол-во предметов: {items_amnt}");
            int cols_in_output = 5;
            for(int i=0;i<items_amnt ; ++i)
            {
                Console.Write($"[{items[i].Index}] {items[i].Weight}\t\t");
                if (i % cols_in_output == 0 && i !=0) Console.WriteLine();
            }
            Console.WriteLine($"\nВместимость контейнера: {bin_capacity}");
        }

        static void Main()
        {
            List<Bin> bins = new List<Bin>();       int bin_capacity = 0;
            List<Item> items = new List<Item>();    int items_amnt = 0;
            Packing packing_type;
            bool generate_again = true;
            while (generate_again) {
                Console.Write("Сгенерировать входные данные автоматически (y - да/n - нет, ввести вручную) : ");
                if (Console.ReadLine() == "y")
                {
                    TestGenerator(ref items, ref bin_capacity, ref items_amnt);
                }
                else
                {
                    items.Clear();
                    int weight = 0;
                    Console.Write("Кол-во предметов: "); int.TryParse(Console.ReadLine(), out items_amnt);
                    for (int i = 0; i < items_amnt; ++i)
                    {
                        Console.Write($"Вес {i + 1} предмета: "); int.TryParse(Console.ReadLine(), out weight);
                        items.Add(new Item(i, weight));
                    }

                    Console.Write("Размерность контейнера: "); int.TryParse(Console.ReadLine(), out bin_capacity);
                }
                Console.Write("Сгенерировать/ввести заново (y/n)? : ");
                if (Console.ReadLine() == "n") generate_again = false;
            }

            Console.Write(
                        "Выберите тип упаковки:\n" + 
                        "1. Перебор всех возможных комбинаций предметов для нахождеия лучшего случая (с использованием FF)\n" + 
                        "2. FF\n" + 
                        "3. FFS\n"
                         );
            packing_type = new Packing(items, bin_capacity);
            int pack_id; int.TryParse(Console.ReadLine(), out pack_id);
            switch (pack_id)
            {
                case (1):
                {
                        bins = packing_type.BruteForce();
                        break;
                }
                case (2):
                {
                        bins = packing_type.FF();
                        break;
                }
                case (3):
                {
                        bins = packing_type.FFS();
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
            Console.WriteLine($"Затраченное время (мс): {packing_type.GetTakenTime().ElapsedMilliseconds}");
        }
    }
}