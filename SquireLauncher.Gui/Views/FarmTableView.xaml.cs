using System.Collections.ObjectModel;
using System.Windows.Controls;
using SquireLauncher.Gui.Data.Context;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Views;

public partial class FarmTableView : UserControl
{
    private readonly BotDbContext context;

    public FarmTableView(BotDbContext context)
    {
        this.context = context;
        InitializeComponent();
    }

    public ObservableCollection<Farm> AvaillableFarms { get; } = new ObservableCollection<Farm>();

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Farm newFarm = new Farm()
        {
            Name = FarmName.Text
        };

        context.Farms.Add(newFarm);
        context.SaveChanges();
        AvaillableFarms.Add(newFarm);
    }

    private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        AvaillableFarms.Clear();
        foreach (var farm in context.Farms)
        {
            AvaillableFarms.Add(farm);
        }
    }
}
