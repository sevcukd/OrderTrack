using ModelMID;

namespace OrderTrack.Models
{
    public class CommandAPI<t>
    {
        public eCommand Command { get; set; }
        public t Data { get; set; }
    }
}
