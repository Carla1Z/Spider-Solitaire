using System;
using System.Collections.Generic;
using System.Text;

// DTO para iniciar una nueva partida con la configuración elegida.
using SpiderSolitaire.Models;

namespace SpiderSolitaire.DTOs
{
    /// <summary>
    /// Parámetros que el usuario elige al iniciar una partida nueva.
    /// </summary>
    public class NewGameRequestDto
    {
        public Difficulty Difficulty { get; set; } = Difficulty.OneSuit;

        /// <summary>
        /// Seed opcional para reproducir partidas específicas (modo debug).
        /// Null = completamente aleatorio.
        /// </summary>
        public int? Seed { get; set; } = null;
    }
}
