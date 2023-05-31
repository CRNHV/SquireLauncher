# Squire launcher

## Folders 
 `/Data/Context` contains the DbContext

 `/Data/Entities` contains the Database entities

 `/Views/` contains the XAML views + codebehind 

## Important files
- MainWindow.xaml: 
  - Contains the markup for the main window. It contains the top navigation bar and a `ContentControl` which contains the views which are displayed in the center
- MainWindow.xaml.cs:
  - Contains the code for the main window. Each view is a seperate class which is injected via the DI container.
- App.xaml.cs: Don't touch it, I barely touch it idk for sure what it does
  - App.xaml.cs: In the function `ConfigureServices` you can add new objects to the DI container

## Using the dbcontext

the DbContext is injected via the DI container and you can request it like so:

LauncherView.xaml.cs:
```
private readonly BotDbContext context;
public LauncherView(BotDbContext context)
{
    InitializeComponent();
    this.context = context;
}
```

## Adding new views
  1. Kindly ask ChatGPT to make you a view
  2. Create a new XAML file with visual studio and copy the contents GPT spits out
  3. Make sure to set the `UserControl x:class` to `SquireLauncher.Gui.Views.ViewName`
  4. The other views probably have examples for how certain components are setup
  5. Adding events is done by typing the eventname in the markup and having the IDE create the function
     1. https://streamable.com/05sr8u

## Database
The database is a SQLITE file. It is used via the DbContext located at `/Data/Context/BotDbContext`
The location of the SQLITE file is decided by this function:

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlite("Data Source = bots.db");
    base.OnConfiguring(optionsBuilder);
}
```
And as such is probably located at `bin/Debug/net7.0-windows/bots.db` if you are debugging, you probably need to delete this file when you get errors regarding certain tables not existing

## Visual studio shortcuts I like:

`ctrl+space` to bring up all IDE hints about what to type

`ctrl+shift+space` to bring up a small hint window on function signatures

`ctrl+r r` quick rename