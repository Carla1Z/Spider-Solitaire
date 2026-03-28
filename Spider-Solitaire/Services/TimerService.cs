// Implementación del temporizador usando IDispatcher de MAUI.
// Dispara el evento Tick cada segundo en el hilo principal.
using System;
using Microsoft.Maui.Dispatching;
using SpiderSolitaire.Interfaces;

namespace SpiderSolitaire.Services
{
    /// <summary>
    /// Temporizador de la partida basado en IDispatcherTimer de MAUI.
    /// Se ejecuta en el UI thread para evitar problemas de threading con bindings.
    /// </summary>
    public class TimerService : ITimerService, IDisposable
    {
        // ── Dependencias ───────────────────────────────────────────
        private readonly IDispatcher _dispatcher;
        private IDispatcherTimer? _timer;

        // ── Estado ─────────────────────────────────────────────────
        private DateTime _startTime;
        private TimeSpan _accumulatedTime = TimeSpan.Zero;

        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        public bool IsRunning { get; private set; }

        public event EventHandler<TimeSpan>? Tick;

        // ── Constructor ────────────────────────────────────────────
        public TimerService(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InitializeTimer();
        }

        // ── Operaciones ────────────────────────────────────────────
        public void Start()
        {
            if (IsRunning) return;
            _startTime = DateTime.UtcNow;
            IsRunning = true;
            _timer?.Start();
        }

        public void Pause()
        {
            if (!IsRunning) return;
            _accumulatedTime += DateTime.UtcNow - _startTime;
            IsRunning = false;
            _timer?.Stop();
        }

        public void Resume()
        {
            if (IsRunning) return;
            Start();
        }

        public void Reset()
        {
            _timer?.Stop();
            _accumulatedTime = TimeSpan.Zero;
            Elapsed = TimeSpan.Zero;
            IsRunning = false;
        }

        // ── Privados ───────────────────────────────────────────────
        private void InitializeTimer()
        {
            // IDispatcherTimer garantiza ejecución en el UI thread
            _timer = _dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            Elapsed = _accumulatedTime + (DateTime.UtcNow - _startTime);
            Tick?.Invoke(this, Elapsed);
        }

        public void Dispose()
        {
            _timer?.Stop();
            if (_timer != null)
                _timer.Tick -= OnTimerTick;
        }
    }
}