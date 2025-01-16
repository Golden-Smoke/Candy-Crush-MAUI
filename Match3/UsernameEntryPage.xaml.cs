using Match3.Data;
using Match3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Linq;

namespace Match3
{
    public partial class UsernameEntryPage : ContentPage
    {
        private readonly AppDbContext _dbContext;

        public UsernameEntryPage()
        {
            InitializeComponent();
            _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
        }

        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();

            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "Username cannot be empty!", "OK");
                return;
            }

            // Check if the username already exists in the database
            var existingPlayer = _dbContext.Players.FirstOrDefault(p => p.Username == username);

            if(existingPlayer == null)
            {
                ErrorMessageLabel.IsVisible = false;

                var newPlayer = new Player { Username = username };
                _dbContext.Players.Add(newPlayer);
                await _dbContext.SaveChangesAsync();
            }
            await Navigation.PushAsync(new MainPage(username));
        }
    }
}
