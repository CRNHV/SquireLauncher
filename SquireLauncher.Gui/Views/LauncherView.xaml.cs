using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Views;

public partial class LauncherView : UserControl
{
    private readonly BotDbContext context;

    private Farm selectedFarm;
    public Farm SelectedFarm { get; set; }

    public Bot SelectedBot { get; set; }

    public LauncherView(BotDbContext context)
    {
        InitializeComponent();
        this.context = context;
    }

    public ObservableCollection<Bot> LoadedBots { get; } = new ObservableCollection<Bot>();
    public ObservableCollection<Farm> LoadedFarms = new ObservableCollection<Farm>();

    private async void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await LoadBots();
        await LoadFarms();
    }

    public async Task LoadBots()
    {
        LoadedBots.Clear();
        var botsInDb = await context.Bots.ToListAsync();
        foreach (Bot bot in botsInDb)
        {
            LoadedBots.Add(bot);
        }
    }

    public async Task LoadFarms()
    {
        LoadedBots.Clear();
        var farmsInDb = await context.Farms.ToListAsync();
        foreach (Farm bot in farmsInDb)
        {
            LoadedFarms.Add(bot);
        }
    }
}
