// Helper para reproducir sonidos del juego (preparado para implementación futura).
using System.Threading.Tasks;

namespace SpiderSolitaire.Helpers
{
    /// <summary>
    /// Gestiona los efectos de sonido del juego.
    /// Actualmente usa stubs — se implementa con Plugin.Maui.Audio
    /// cuando se agregue la dependencia.
    /// Separado en helper para no contaminar ViewModels con audio.
    /// </summary>
    public static class SoundHelper
    {
        private static bool _soundEnabled = true;

        public static bool SoundEnabled
        {
            get => _soundEnabled;
            set => _soundEnabled = value;
        }

        /// <summary>
        /// Sonido al mover una carta.
        /// </summary>
        public static Task PlayCardMoveAsync()
            => PlayAsync("card_move.mp3");

        /// <summary>
        /// Sonido al completar una secuencia.
        /// </summary>
        public static Task PlaySequenceCompleteAsync()
            => PlayAsync("sequence_complete.mp3");

        /// <summary>
        /// Sonido al repartir cartas del stock.
        /// </summary>
        public static Task PlayDealAsync()
            => PlayAsync("card_deal.mp3");

        /// <summary>
        /// Sonido de victoria.
        /// </summary>
        public static Task PlayWinAsync()
            => PlayAsync("win.mp3");

        private static Task PlayAsync(string filename)
        {
            if (!_soundEnabled) return Task.CompletedTask;
            // TODO: implementar con Plugin.Maui.Audio
            // var player = AudioManager.Current.CreatePlayer(filename);
            // await player.PlayAsync();
            System.Diagnostics.Debug.WriteLine($"[Sound] Playing: {filename}");
            return Task.CompletedTask;
        }
    }
}