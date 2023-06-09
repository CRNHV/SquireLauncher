using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Views;

public partial class BotTableView : UserControl
{
    private readonly BotDbContext _context;
    private readonly ILogger<BotTableView> _logger;

    public BotTableView(BotDbContext context, ILogger<BotTableView> logger)
    {
        _context = context;
        _logger = logger;

        InitializeComponent();
    }

    public ObservableCollection<Farm> LoadedFarms { get; } = new ObservableCollection<Farm>();
    public ObservableCollection<Bot> LoadedBots { get; } = new ObservableCollection<Bot>();

    public Farm SelectedFarm { get; set; }
    public Bot? SelectedBot { get; set; }

    private async void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await LoadBots();
        await LoadFarms();
    }

    private async Task LoadBots()
    {
        LoadedBots.Clear();
        var bots = await _context.Bots.ToListAsync();
        foreach (Bot bot in bots)
        {
            LoadedBots.Add(bot);
        }
    }

    private async Task LoadFarms()
    {
        LoadedFarms.Clear();
        var farms = await _context.Farms.ToListAsync();
        foreach (var farm in farms)
        {
            LoadedFarms.Add(farm);
        }
    }

    private async void Button_AddBot(object sender, System.Windows.RoutedEventArgs e)
    {
        if (FarmBox.SelectedIndex == -1)
        {
            return;
        }

        var email = this.Email.Text;
        var username = this.Username.Text;
        var password = this.Password.Text;
        var farm = (Farm)FarmBox.Items.GetItemAt(FarmBox.SelectedIndex);

        _logger.LogInformation("Add bot with email = {email} username = {username} password = {password}", email, username, password);

        _context.Bots.Add(new Bot
        {
            Email = email,
            Username = username,
            Password = password,
            Farm = farm
        });

        _context.SaveChanges();

        await LoadBots();
    }

    private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        SelectedBot = (Bot)this.BotGrid.SelectedItem;
    }

    private async void BotGrid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (SelectedBot == null)
        {
            return;
        }

        if (e.Key != System.Windows.Input.Key.Delete)
        {
            return;
        }

        _context.Bots.Remove(SelectedBot);
        _context.SaveChanges();
        await LoadBots();
    }
}
