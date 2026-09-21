
namespace WeddingApp.Shared.DTO.GuestDTOs
{
    public class WeddingTableDTO
    {
        public string TableNumber { get; set; } = string.Empty;

        public double X { get; set; }

        public double Y { get; set; }

        public int SeatCount { get; set; } = 8;
    }
}
