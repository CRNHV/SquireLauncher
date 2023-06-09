using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquireLauncher.Gui.Launcher;

internal interface IClientLauncher
{
    Task LaunchClientAsync(LaunchSettings launchSettings);
}
