namespace HubCinemaAdmin.Models
{
    public class AdminDashboardViewModel
    {
        public int TicketsToday { get; set; }
        public int TicketsThisMonth { get; set; }
        public int TicketsThisYear { get; set; }
        public int CurrentMovies { get; set; }
        public int UpcomingMovies { get; set; }
        public int ActiveCinemas { get; set; }
        public int TotalUsers { get; set; }
    }

}
