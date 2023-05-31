# Squire launcher

## Important files
- MainWindow.xaml: 
  - Contains the markup for the main window. It contains the top navigation bar and a `ContentControl` which contains the views which are displayed in the center
- MainWindow.xaml.cs:
  - Contains the code for the main window. Each view is a seperate class which is injected via the DI container.
- App.xaml.cs: Don't touch it, I barely touch it idk for sure what it does