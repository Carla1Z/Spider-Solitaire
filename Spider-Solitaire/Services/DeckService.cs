// Implementación del servicio de creación de mazos.
// Delega en DeckFactory del dominio — capa de servicio actúa como wrapper.
using System.Collections.Generic;
using SpiderSolitaire.Interfaces;
using SpiderSolitaire.Models;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Implementación de IDeckService.
    /// Thin wrapper sobre DeckFactory para permitir inyección de dependencias.
    /// </summary>
    public class DeckService : IDeckService
    {
        public List<Card> CreateShuffledDeck(Difficulty difficulty, int? seed = null)
            => DeckFactory.CreateShuffledDeck(difficulty, seed);
    }
}