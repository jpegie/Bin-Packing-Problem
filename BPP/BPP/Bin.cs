namespace BinPP
{
    internal class Bin
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
            if (FreeCapacity < item.Weight)
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
}
