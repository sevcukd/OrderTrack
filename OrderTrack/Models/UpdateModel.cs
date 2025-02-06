using ModelMID;

namespace OrderTrack.Models
{
    public class UpdateModel
    {
        public eStatus Status { get; set; }
        public int Id { get; set; }

        public UpdateModel(eStatus status, int id)
        {
            Status = status;
            Id = id;
        }
    }
}
