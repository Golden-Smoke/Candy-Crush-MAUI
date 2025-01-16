using System;

namespace Match3
{
	public partial class MainMenuPage : ContentPage
	{
		public MainMenuPage()
		{
			InitializeComponent();
		}
        private void OnStartButtonClicked(object sender, EventArgs e)
        {
           Navigation.PushAsync(new UsernameEntryPage());
        }
        private void OnLeaderboardsButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LeaderboardsPage());  
        }

        private void OnExitButtonClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}