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
        private Farm selectedFarm;

        public ProfileTableView(BotDbContext context)
        {
            InitializeComponent();
            this.context = context;
        }

        public ObservableCollection<Farm> AvaillableFarms { get; } = new ObservableCollection<Farm>();
        public ObservableCollection<Profile> Profiles { get; } = new ObservableCollection<Profile>();

        private async void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var noneFarm = context.Farms.Where(x => x.Name == "NONE").First();

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var profiles2Path = $".openosrs/profiles2";

            var filesInProfilesDirectory = Directory.GetFiles(Path.Combine(userProfile, profiles2Path));
            for (int i = 0; i < filesInProfilesDirectory.Length; i++)
            {
                var profilePath = filesInProfilesDirectory[i];
                var profileFileName = Path.GetFileName(profilePath);

                if (profileFileName.StartsWith("$") || !profileFileName.EndsWith("properties"))
                {
                    continue;
                }

                if (!context.Profiles.Any(p => p.Path == profilePath))
                {
                    context.Profiles.Add(new Profile()
                    {
                        Name = profileFileName.Split("-")[0],
                        Path = profilePath,
                        Farm = noneFarm
                    });
                }
            }

            if (context.ChangeTracker.HasChanges())
            {
                context.SaveChanges();
            }

            await LoadProfilesFromDb();
            await LoadFarmsFromDb();
        }

        private async Task LoadProfilesFromDb()
        {
            Profiles.Clear();
            foreach (var profile in await context.Profiles.ToListAsync())
            {
                Profiles.Add(profile);
            }
        }

        private async Task LoadFarmsFromDb()
        {
            AvaillableFarms.Clear();
            foreach (var farm in context.Farms)
            {
                AvaillableFarms.Add(farm);
            }
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProfile = (Profile)ProfileGrid.SelectedItem;
            if (selectedProfile == null)
            {
                return;
            }

            Farm? selectedFarm = (Farm)e.AddedItems[0];
            var dbProfile = context.Profiles.FirstOrDefault(x => x.Id == selectedProfile.Id);
            if (dbProfile == null || selectedFarm == null)
            {
                return;
            }

            if (context.Profiles.Any(x => x.Farm == selectedFarm && selectedFarm.Name != "NONE"))
            {
                MessageBox.Show("There already is a Farm associated with that profile");
                LoadProfilesFromDb(); // Dirty
                return;
            }

            dbProfile.Farm = selectedFarm;
            context.Update(dbProfile);
            await context.SaveChangesAsync();

        }
    }
}
