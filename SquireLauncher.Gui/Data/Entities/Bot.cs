namespace SquireLauncher.Gui.Data.Entities;

public sealed class Bot
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public Farm Farm { get; set; }
}
