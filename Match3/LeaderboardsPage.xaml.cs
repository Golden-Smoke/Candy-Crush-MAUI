using Match3.Data;
using Microsoft.EntityFrameworkCore;

namespace Match3;

public partial class LeaderboardsPage : ContentPage
{
	public LeaderboardsPage()
	{
		InitializeComponent();
        LoadLeaderboardData();
	}

    private async void LoadLeaderboardData()
    {
        using (var dbContext = new AppDbContext(App.ServiceProvider.GetService<DbContextOptions<AppDbContext>>()))
        {
            var leaderboard = await dbContext.HighScores
                .Include(h => h.Player)
                .OrderByDescending(h => h.Score)
                .Take(10)
                .ToListAsync();

            LeaderboardsCollection.ItemsSource = leaderboard;
        }
    }
}