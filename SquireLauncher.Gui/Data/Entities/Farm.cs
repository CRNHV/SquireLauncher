using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquireLauncher.Gui.Data.Entities;

public class Farm
{
    public int Id { get; set; }
    public string Name { get; set; }
    [ForeignKey("FarmId")]
    public List<Bot> Bots { get; set; }
}
