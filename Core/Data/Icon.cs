namespace Core.Data
{
    public class Icon
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - ({1},{2})", Name, X, Y);
        }
    }
}
