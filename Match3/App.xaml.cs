using Match3.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Match3
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;

            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            MainPage = new NavigationPage(new MainMenuPage());
        }
    }
}
