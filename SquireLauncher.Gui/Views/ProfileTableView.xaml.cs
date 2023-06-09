using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Views
{
    /// <summary>
    /// Interaction logic for ProfileTableView.xaml
    /// </summary>
    public partial class ProfileTableView : UserControl
    {
        private readonly BotDbContext context;


        public ProfileTableView(BotDbContext context)
        {
            InitializeComponent();
            this.context = context;
        }

        public ObservableCollection<Profile> Profiles { get; } = new ObservableCollection<Profile>();


        private async void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await LoadProfilesFromDisk();
            await LoadProfilesFromDb();
        }

        private async Task LoadProfilesFromDb()
        {
            Profiles.Clear();
            foreach (var profile in await context.Profiles.ToListAsync())
            {
                Profiles.Add(profile);
            }
        }

        private async Task LoadProfilesFromDisk()
        {
            var noneFarm = context.Farms.Where(x => x.Name == "NONE").First();

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var profiles2Path = $".openosrs/profiles2";

            var filesInProfilesDirectory = Directory.GetFiles(Path.Combine(userProfile, profiles2Path));
            for (int i = 0; i < filesInProfilesDirectory.Length; i++)
            {
                await ProcessDirectory(filesInProfilesDirectory[i]);
            }

            if (context.ChangeTracker.HasChanges())
            {
                context.SaveChanges();
            }
        }

        private async Task ProcessDirectory(string profilePath)
        {
            var profileFileName = Path.GetFileName(profilePath);

            if (profileFileName.StartsWith("$") || !profileFileName.EndsWith("properties"))
            {
                return;
            }

            if (profilePath.Contains("choev"))
            {
                profilePath = profilePath.Replace("choev", "squire");
            }

            if (!context.Profiles.Any(p => p.Path == profilePath))
            {
                context.Profiles.Add(new Profile()
                {
                    Name = profileFileName.Split("-")[0],
                    Path = profilePath,
                });
            }
        }
    }
}
