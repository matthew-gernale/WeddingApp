
namespace WeddingApp.Shared.DTO.GuestDTOs
{
    public class GuestDTO
    {
        public string Name { get; set; } = string.Empty;

        public int TableNumber { get; set; }

        public Roles Role { get; set; }
    }
}
