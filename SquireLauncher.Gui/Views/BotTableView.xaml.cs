using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Views;

public partial class BotTableView : UserControl
{
    private readonly BotDbContext context;
    private Farm selectedFarm;
    private Bot? selectedBot;

    public BotTableView(BotDbContext context)
    {
        this.context = context;
        InitializeComponent();
    }

    public ObservableCollection<Farm> LoadedFarms { get; } = new ObservableCollection<Farm>();
    public ObservableCollection<Bot> LoadedBots { get; } = new ObservableCollection<Bot>();

    private async void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await LoadBots();
        await LoadFarms();
    }

    private async Task LoadBots()
    {
        LoadedBots.Clear();
        var bots = await context.Bots.ToListAsync();
        foreach (Bot bot in bots)
        {
            LoadedBots.Add(bot);
        }
    }

    private async Task LoadFarms()
    {
        LoadedFarms.Clear();
        var farms = await context.Farms.ToListAsync();
        foreach (var farm in farms)
        {
            LoadedFarms.Add(farm);
        }
    }

    private async void Button_AddBot(object sender, System.Windows.RoutedEventArgs e)
    {
        var email = this.Email.Text;
        var username = this.Username.Text;
        var password = this.Password.Text;
        var farm = (Farm)FarmBox.Items.GetItemAt(FarmBox.SelectedIndex);

        context.Bots.Add(new Bot
        {
            Email = email,
            Username = username,
            Password = password,
            Farm = farm
        });

        context.SaveChanges();

        await LoadBots();
    }

    private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        selectedBot = (Bot)this.BotGrid.SelectedItem;
    }

    private async void BotGrid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (selectedBot == null)
        {
            return;
        }

        if (e.Key != System.Windows.Input.Key.Delete)
        {
            return;
        }

        context.Bots.Remove(selectedBot);
        context.SaveChanges();
        await LoadBots();
    }

}
