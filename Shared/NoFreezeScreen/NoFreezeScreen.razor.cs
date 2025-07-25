namespace ZetaDashboard.Shared.NoFreezeScreen
{
    public partial class NoFreezeScreen
    {
        #region Vars
        private string _nofreezetime { get; set; }
        
        private System.Timers.Timer? _timer;

        #endregion

        #region LifeCycles
        protected override void OnInitialized()
        {
            _timer = new System.Timers.Timer(1000); // 1000 ms = 1 segundo
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }
        #endregion

        #region Timer
        private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _nofreezetime = DateTime.Now.ToString("HH:mm:ss");
            InvokeAsync(StateHasChanged); // Refresca el componente de forma segura
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
        #endregion
    }
}
