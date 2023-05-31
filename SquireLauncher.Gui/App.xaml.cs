using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;
using SquireLauncher.Gui.Views;

namespace SquireLauncher.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<BotDbContext>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ProfileTableView>();
            services.AddSingleton<BotTableView>();
            services.AddSingleton<LauncherView>();
            services.AddSingleton<FarmTableView>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var context = serviceProvider.GetService<BotDbContext>();
            context.Database.EnsureCreated();

            if (!context.Farms.Any())
            {
                context.Farms.Add(new Farm()
                {
                    Id = 1,
                    Name = "NONE"
                });

                context.SaveChanges();
            }

            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
