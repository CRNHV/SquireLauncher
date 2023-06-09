using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquireLauncher.Gui.Launcher;

internal class ClientLauncher : IClientLauncher
{
    public ClientLauncher()
    {

    }

    public async Task LaunchClientAsync(LaunchSettings launchSettings)
    {
        Process process = new Process();
        process.StartInfo.FileName = launchSettings.JavaExeLocation;
        process.StartInfo.Arguments = launchSettings.JavaArguments + " " + launchSettings.SquireJarLocation + " " + launchSettings.SquireArguments;
        process.StartInfo.UseShellExecute = true;
        process.Start();
    }
}
