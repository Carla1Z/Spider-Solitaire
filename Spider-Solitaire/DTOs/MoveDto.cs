using System;
using System.Collections.Generic;
using System.Text;

// DTO para representar un movimiento en logs o historial visible al usuario.
using System;

namespace SpiderSolitaire.DTOs
{
    /// <summary>
    /// Representa un movimiento para mostrarlo en el historial de la UI.
    /// </summary>
    public class MoveDto
    {
        public string Description { get; set; } = string.Empty;
        public int ScoreDelta { get; set; }
        public DateTime ExecutedAt { get; set; }
        public string TimeLabel => ExecutedAt.ToString("HH:mm:ss");
    }
}
