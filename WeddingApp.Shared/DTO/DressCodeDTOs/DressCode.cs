
namespace WeddingApp.Shared.DTO.DressCodeDTOs
{
    public class DressCode
    {
        public Roles Role { get; set; }
        public List<string>? ColorPalette { get; set; }
        public List<string>? Attires { get; set; }
        public string? Description { get; set; }
    }
}
