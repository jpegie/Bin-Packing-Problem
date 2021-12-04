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
            int m;
            List<Item> items = new List<Item>();
            public Bin(int m)
            {
                this.m = m;
            }
            public List<Item> Items
            {
                get { return items; }
            }
        }
        class Packing
        {
            List<Item> items = new List<Item>();
            List<Bin> bins = new List<Bin>();
            int bin_size = 0;
            public Packing(List<Item>items, int bin_size)
            {
                this.items = items;
                this.bin_size = bin_size;
            }

            //перебор всех возможных n! комбинаций предметов с целью поиска лучшей для умещения в меньшее кол-во контейнеров
            //буду использовать алгоритм FF
            public List<Bin> BruteForce()
            {
                //сгенерировать i-ую комбинацию
                //применить FF
                //сохранить информацию о заполненных контейнерах и саму i-ую комбинацию
                //найти лучшую комбинацию
                return bins;
            }
            //"первый подходящий"
            public List<Bin> FF()
            {
                return bins;
            }

            //"первый подходящий" только в отсортированном списке предметов
            public List<Bin> FFS()
            {
                //отсортировать
                //FF()
                return bins;
            }
        }

        void TestGenerator()
        {
            
        }

        static void Main()
        {
            List<Bin> bins = new List<Bin>();       int bin_size = 0;
            List<Item> items = new List<Item>();    int items_amnt = 0;
            Packing packing; 

            Console.WriteLine("Кол-во предметов: "); int.TryParse(Console.ReadLine(), out items_amnt);
            Console.WriteLine("Размерность контейнера: "); int.TryParse(Console.ReadLine(), out bin_size);

            for(int i=0;i<items_amnt ; ++i)
            {
                double weight = 0;
                Console.Write($"Вес {i + 1} предмета: "); double.TryParse(Console.ReadLine(), out weight);
                items.Add(new Item(i,weight));
            }

            Console.Write(
                        "Выберите тип упаковки: " + 
                        "1. Перебор всех возможных комбинаций предметов для нахождеия лучшего случая" + 
                        "2. FF" + 
                        "3. BF"
                         );
            packing = new Packing(items, bin_size);
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

            /*Console.WriteLine($"Кол-во использованных контейнеров: {bins.Count}");
            for(int i=0;i<bins.Count; ++i)
            {
                Console.WriteLine($"Предметы контейнера {i + 1}: ");
                foreach(Item item in bins[i].Items)
                    Console.WriteLine($"[{item.Index}] : {item.Weight}");
                Console.WriteLine(new String('-', 25));
            }*/
        }
    }
}