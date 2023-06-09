namespace SquireLauncher.Gui.Launcher;

internal class LaunchSettings
{
    public string JavaExeLocation { get; set; }
    public string JavaArguments { get; set; } = "-XX:+DisableAttachMechanism -Xmx4G -Xss2m -XX:CompileThreshold=1500 -jar -Drunelite.launcher.nojvm=true";
    public string SquireJarLocation { get; set; }
    public string SquireArguments { get; set; }
    public string Branch { get; internal set; }
}
