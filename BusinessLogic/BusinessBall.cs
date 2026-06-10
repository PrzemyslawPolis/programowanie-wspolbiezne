namespace BusinessLogic
{
    internal class Ball : IBall
    {
        private static int idCounter = 0;
        public int Id { get; } = Interlocked.Increment(ref idCounter); // Unikalne ID bezpieczne dla wątków
        public object BallLock { get; } = new object();
        public IPosition position { get; set; }
        public Data.IBall underneathBall { get; init; }
        public Ball (IPosition position, Data.IBall underneathBall)
        {
            this.position = position;
            this.underneathBall = underneathBall;
        }

        public event EventHandler<IPosition>? NewPositionNotification;



        internal void UpdatePosition(double x, double y)
        {            
            this.position = new Position(x, y);
            NewPositionNotification?.Invoke(this, new Position(x, y));
        }

    }
}
