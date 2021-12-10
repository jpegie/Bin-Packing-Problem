namespace BinPP
{
    internal class PackResult
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
}
