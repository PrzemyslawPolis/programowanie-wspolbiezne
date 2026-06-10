
namespace Data
{
    internal class Ball : IBall
    {
        
        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }

        
        internal int IsUpdating = 0;
        internal DateTime LastUpdateTime { get; set; }
        internal double Accumulator { get; set; } = 0.0;

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }


        internal Vector Position { get; set; }

        public void SetPosition(double newX, double newY)
        {
            Position = new Vector(newX, newY);
        }

        public void SetVelocity(double newX, double newY)
        {
            Velocity = new Vector(newX, newY);
        }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta, bool silent)
        {
            Position = new Vector(Position.x + delta.x, Position.y + delta.y);
            if (!silent) RaiseNewPositionChangeNotification();
        }

    }
}