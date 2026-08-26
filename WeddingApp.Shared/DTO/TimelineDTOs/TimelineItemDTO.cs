
namespace WeddingApp.Shared.DTO.TimelineDTOs
{
    public class TimelineItemDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string Icon { get; set; } = string.Empty;
    }
}
