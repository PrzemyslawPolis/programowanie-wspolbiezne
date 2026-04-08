
namespace Data
{
    internal class Ball : IBall
    {
        
        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }



        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }


        private Vector Position;

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