// Abstracción del temporizador del juego.
// Permite pausar/reanudar y testear sin tiempo real.
using System;

namespace SpiderSolitaire.Interfaces
{
    /// <summary>
    /// Controla el temporizador de la partida.
    /// </summary>
    public interface ITimerService
    {
        TimeSpan Elapsed { get; }
        bool IsRunning { get; }

        void Start();
        void Pause();
        void Resume();
        void Reset();

        /// <summary>
        /// Se dispara cada segundo con el tiempo transcurrido.
        /// </summary>
        event EventHandler<TimeSpan> Tick;
    }
}