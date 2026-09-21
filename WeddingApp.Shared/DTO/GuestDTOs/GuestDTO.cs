
namespace WeddingApp.Shared.DTO.GuestDTOs
{
    public class GuestDTO
    {
        public string Name { get; set; } = string.Empty;

        public string TableNumber { get; set; } = string.Empty;

        public Roles Role { get; set; }
    }
}
