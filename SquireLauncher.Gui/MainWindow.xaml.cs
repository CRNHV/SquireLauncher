using System.Windows;
using Microsoft.Extensions.Logging;
using SquireLauncher.Gui.Views;

namespace SquireLauncher.Gui;

public partial class MainWindow : Window
{
    private readonly ProfileTableView _profileTableView;
    private readonly BotTableView _botTableView;
    private readonly LauncherView _launchView;
    private readonly FarmTableView _farmTableView;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ProfileTableView profileTableView,
                      BotTableView botTableView,
                      LauncherView launchView,
                      FarmTableView farmTableView,
                      ILogger<MainWindow> logger)
    {
        InitializeComponent();

        contentControl.Content = profileTableView;

        _profileTableView = profileTableView;
        _botTableView = botTableView;
        _launchView = launchView;
        _farmTableView = farmTableView;
        _logger = logger;
    }

    private void ProfilesButton_Click(object sender, RoutedEventArgs e)
    {
        contentControl.Content = _profileTableView;
    }

    private void BotsButton_Click(object sender, RoutedEventArgs e)
    {
        contentControl.Content = _botTableView;
    }

    private void LauncherButton_Click(object sender, RoutedEventArgs e)
    {
        contentControl.Content = _launchView;
    }

    private void FarmButton_Click(object sender, RoutedEventArgs e)
    {
        contentControl.Content = _farmTableView;
    }
}
