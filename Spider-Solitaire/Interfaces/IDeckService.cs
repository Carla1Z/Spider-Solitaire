// Contrato para la creación y gestión de mazos.
using System.Collections.Generic;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Abstracción para crear mazos de cartas.
    /// Permite inyectar mazos deterministas en tests.
    /// </summary>
    public interface IDeckService
    {
        /// <summary>
        /// Crea y retorna un mazo mezclado de 104 cartas
        /// configurado según la dificultad.
        /// </summary>
        List<Card> CreateShuffledDeck(Difficulty difficulty, int? seed = null);
    }
}