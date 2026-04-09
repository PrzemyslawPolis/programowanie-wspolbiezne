namespace BusinessLogic
{
    internal class Ball : IBall
    {
             

        public event EventHandler<IPosition>? NewPositionNotification;



        internal void UpdatePosition(double x, double y)
        {
            NewPositionNotification?.Invoke(this, new Position(x, y));
        }

    }
}
