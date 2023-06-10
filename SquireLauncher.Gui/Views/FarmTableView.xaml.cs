using System.Collections.ObjectModel;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Views;

public partial class FarmTableView : UserControl
{
    private readonly BotDbContext _context;

    public FarmTableView(BotDbContext context)
    {
        _context = context;
        InitializeComponent();
    }

    public ObservableCollection<Farm> AvaillableFarms { get; } = new ObservableCollection<Farm>();
    public ObservableCollection<Profile> AvaillableProfiles { get; } = new ObservableCollection<Profile>();
    public Profile SelectedProfile { get; set; }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Farm newFarm = new()
        {
            Name = FarmName.Text
        };

        _context.Farms.Add(newFarm);
        _context.SaveChanges();
        AvaillableFarms.Add(newFarm);
    }

    private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        LoadFarms();
        LoadProfiles();
    }

    private void LoadProfiles()
    {
        AvaillableProfiles.Clear();
        foreach (var profile in _context.Profiles)
        {
            AvaillableProfiles.Add(profile);
        }
    }

    private void LoadFarms()
    {
        AvaillableFarms.Clear();
        foreach (var farm in _context.Farms.Include(x => x.Profile))
        {
            AvaillableFarms.Add(farm);
        }
    }

    private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedProfile = (Profile)e.AddedItems[0];
        var selectedFarm = (Farm)this.FarmGrid.SelectedCells[0].Item;

        if (selectedProfile == null || selectedFarm == null)
        {
            return;
        }

        var dbFarm = await _context.Farms.FirstOrDefaultAsync(x => x.Id == selectedFarm.Id);
        if (dbFarm == null)
        {
            return;
        }

        dbFarm.Profile = selectedProfile;
        _context.Update(dbFarm);
        await _context.SaveChangesAsync();
        SelectedProfile = null;
        LoadFarms();
    }

    private async void FarmGrid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        var selectedFarm = (Farm)this.FarmGrid.SelectedItem;

        if (selectedFarm == null)
        {
            return;
        }

        if (e.Key != System.Windows.Input.Key.Delete)
        {
            return;
        }

        _context.Farms.Remove(selectedFarm);
        _context.SaveChanges();
        LoadFarms();
    }
}
