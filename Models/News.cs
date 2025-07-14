namespace HubCinemaAdmin.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Subtitle { get; set; }
        public string? Slug { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public int Category { get; set; } = 1; // tạm set cứng nếu chưa có select
    }

}
