using System;
using System.Collections.Generic;
using System.Text;

// DTO de una columna del tablero.
// Agrupa los CardDto y expone metadatos que la View necesita.
using System.Collections.Generic;

namespace SpiderSolitaire.DTOs
{
    /// <summary>
    /// Representa el estado visual de una columna del tablero.
    /// </summary>
    public class ColumnDto
    {
        public int Index { get; set; }
        public List<CardDto> Cards { get; set; } = new();
        public bool IsEmpty { get; set; }
        public bool IsDropTarget { get; set; } // Resaltar durante drag

        /// <summary>
        /// Cantidad de cartas boca abajo (apiladas al inicio de la columna).
        /// La UI puede renderizarlas como un stack compacto.
        /// </summary>
        public int FaceDownCount { get; set; }

        /// <summary>
        /// Carta superior de la columna (para validación de drop rápido en UI).
        /// </summary>
        public CardDto? TopCard => Cards.Count > 0 ? Cards[^1] : null;
    }
}
