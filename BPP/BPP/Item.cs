namespace BinPP
{
    internal class Item
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
}
