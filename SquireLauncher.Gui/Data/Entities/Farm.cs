using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquireLauncher.Gui.Data.Entities;

public class Farm
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ProfileId { get; set; }

    public Profile? Profile { get; set; }

    [ForeignKey("FarmId")]
    public List<Bot> Bots { get; set; }

    [NotMapped]
    public string ProfileName => this.Profile?.Name ?? "None";
}
