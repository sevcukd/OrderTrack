namespace OrderTrack.Models
{
    public class Order
    {
        public int OrderNumber { get => Id < 100 ? Id : Id % 100; }
        public eStatus Status { get; set; }
        public string StatusIcon { get => $"Images/{Status}.png"; }
        public Order()
        {
            DateCreate = DateTime.Now;
        }
        public int Id { get; set; }
        public int IdWorkplace { get; set; }
        public int CodePeriod { get; set; }
        public int CodeReceipt { get; set; }
        public int CodeWares { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Type { get; set; }
        public string JSON { get; set; }
    }
    public enum eStatus
    {
        Waiting,
        Preparing,
        Ready,
    }


}
