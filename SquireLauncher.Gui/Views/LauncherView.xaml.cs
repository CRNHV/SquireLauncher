using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;
using SquireLauncher.Gui.Launcher;
using WinForms = System.Windows.Forms;

namespace SquireLauncher.Gui.Views;

public partial class LauncherView : UserControl
{
    private readonly BotDbContext _context;
    private readonly IClientLauncher _clientLauncher;
    private readonly ILogger<LauncherView> _logger;

    public LauncherView(BotDbContext context, ILogger<LauncherView> logger)
    {
        InitializeComponent();
        _context = context;
        _clientLauncher = new ClientLauncher();
        _logger = logger;
    }

    public ObservableCollection<Bot> LoadedBots { get; } = new ObservableCollection<Bot>();
    public ObservableCollection<Farm> LoadedFarms { get; } = new ObservableCollection<Farm>();
    public ObservableCollection<Profile> LoadedProfiles { get; } = new ObservableCollection<Profile>();
    public Farm SelectedFarm { get; set; }
    public Bot SelectedBot { get; set; }
    public Profile SelectedProfile { get; set; }
    public string SelectedBranch { get; set; }

    private async void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        this.FolderText.Text = "No folder selected";

        await LoadBots();
        await LoadFarms();
        await LoadSettings();
        await LoadProfiles();
    }

    public async Task LoadBots()
    {
        LoadedBots.Clear();
        var botsInDb = await _context.Bots.ToListAsync();
        foreach (Bot bot in botsInDb)
        {
            LoadedBots.Add(bot);
        }
    }

    public async Task LoadFarms()
    {
        LoadedFarms.Clear();
        var farmsInDb = await _context.Farms.ToListAsync();
        foreach (Farm bot in farmsInDb)
        {
            LoadedFarms.Add(bot);
        }
    }

    public async Task LoadSettings()
    {
        var settings = await _context.Settings.FirstOrDefaultAsync();

        if (settings == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(settings.Branch))
        {
            branchBox.Text = settings.Branch;
        }

        if (settings.SquirePath != null)
        {
            FolderText.Text = settings.SquirePath;
        }
    }

    public async Task LoadProfiles()
    {
        this.LoadedProfiles.Clear();
        var profiles = await _context.Profiles.ToListAsync();

        if (profiles == null || profiles.Count == 0)
        {
            _logger.LogInformation("No profiles to load");
            return;
        }

        foreach (var profile in profiles)
        {
            this.LoadedProfiles.Add(profile);
        }
    }

    private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        using (WinForms.FolderBrowserDialog folderDialog = new())
        {
            WinForms.DialogResult result = folderDialog.ShowDialog();

            if (result != WinForms.DialogResult.OK || string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
            {
                return;
            }

            var selectedFolderPath = folderDialog.SelectedPath;

            var foldersInPath = Directory.GetDirectories(selectedFolderPath);

            if (foldersInPath.Length == 0)
            {
                _logger.LogWarning("There are no folders found in the selected path {path}", selectedFolderPath);
                return;
            }

            var jarFolder = foldersInPath.FirstOrDefault(x => x.EndsWith("jar"));
            var jreFolder = foldersInPath.FirstOrDefault(x => x.EndsWith("jre"));

            if (string.IsNullOrEmpty(jarFolder) || string.IsNullOrEmpty(jreFolder))
            {
                _logger.LogWarning("Jar folder or runtime folder is empty \r\n jar: {jar} jre: {jre}", jarFolder, jreFolder);
                return;
            }

            var dbSettings = await _context.Settings.FirstOrDefaultAsync();
            if (dbSettings == null)
            {
                _logger.LogWarning("Unable to load settings because there are no settings in the database.");
                return;
            }

            dbSettings.SquirePath = selectedFolderPath;
            dbSettings.SquireJarPath = jarFolder + "\\squire-launcher.jar";
            dbSettings.JavaExePath = jreFolder + "\\bin\\java.exe";

            _context.Update(dbSettings);
            await _context.SaveChangesAsync();

            this.FolderText.Text = selectedFolderPath;
        }
    }

    private async void LaunchFarm_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var bots = _context.Bots.Where(x => x.Farm.Id == SelectedFarm.Id);
        var botProfile = SelectedFarm.Profile;

        if (botProfile == null)
        {
            _logger.LogWarning("No profile selected for farm: {farm}", SelectedFarm.Name);
            return;
        }

        if (!bots.Any())
        {
            _logger.LogWarning("No bots found for farm: {farm}", SelectedFarm.Name);
            return;
        }


        var settings = _context.Settings.FirstOrDefault();
        if (settings == null || string.IsNullOrEmpty(settings.SquirePath))
        {
            _logger.LogWarning("Unable to launch farm, either settings or squirepath is null or empty " +
                "\r\n settings: {@settings} squirepath: {squirePath}", settings, settings.SquirePath);
            return;
        }

        if (string.IsNullOrEmpty(settings.Branch))
        {
            _logger.LogWarning("Branch has not been set yet");
        }

        foreach (var bot in bots)
        {
            var username = bot.Username;
            var password = bot.Password;

            await LaunchClient(settings, username, password, botProfile.Name);
            await Task.Delay(1000);
        }
    }

    private async void LaunchAccount_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var settings = _context.Settings.FirstOrDefault();
        if (settings == null || string.IsNullOrEmpty(settings.SquirePath))
        {
            _logger.LogWarning("Unable to launch account, either settings or squirepath is null or empty " +
                    "\r\n settings: {@settings} squirepath: {squirePath}", settings, settings.SquirePath);
            return;
        }

        var username = SelectedBot.Username;
        var password = SelectedBot.Password;
        var profileName = SelectedProfile.Name;

        await LaunchClient(settings, username, password, profileName);
    }

    private async Task LaunchClient(Settings? settings, string username, string password, string profileName)
    {
        var launchSettings = new LaunchSettings()
        {
            JavaExeLocation = settings.JavaExePath,
            SquireJarLocation = settings.SquireJarPath,
            SquireArguments = $"--{settings.Branch} --skip-auth --profile {profileName} --account {username}:{password}",
            Branch = settings.Branch,
        };

        _logger.LogInformation("Launching client with squire arguments: {args}", launchSettings.SquireArguments);

        await _clientLauncher.LaunchClientAsync(launchSettings);
    }

    private async void branchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = (string)((ComboBoxItem)e.AddedItems[0]).Content;
        if (selection == null)
        {
            return;
        }

        var dbSettings = await _context.Settings.FirstOrDefaultAsync();
        if (dbSettings == null)
        {
            return;
        }

        dbSettings.Branch = selection;
        _context.Update(dbSettings);
        await _context.SaveChangesAsync();
    }
}

