namespace OrderTrack.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public string Status { get; set; }
        public string StatusIcon { get; set; }
        public DateTime TimeAdded { get; set; }

        public Order()
        {
            TimeAdded = DateTime.Now;
        }
    }

    public class OrderEntity
    {
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
}
