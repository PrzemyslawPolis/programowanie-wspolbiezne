using Data;

namespace BusinessLogic
{
    public class BorderDetection
    {
        public BorderDetection() { }
        
        public void NewBall(IVector startingPos, IBall ball)
        {
            ball.NewPositionNotification += OnBallPositionNotification;
        }

        private void OnBallPositionNotification(object? sender, IVector newPos)
        {
            if (sender != null)
            {
                IBall ball = (IBall)sender;

                //przyjmujemy r=15
                if (newPos.x <= 15 || newPos.x >= 585)
                {
                    if (newPos.x < 15)
                    {
                        ball.SetPosition(15, newPos.y);
                    }
                    else if (newPos.x > 585)
                    {
                        ball.SetPosition(585, newPos.y);
                    }
                    ball.SetVelocity(-ball.Velocity.x, ball.Velocity.y);
                }
                if (newPos.y <= 15 || newPos.y >= 585)
                {
                    if (newPos.y < 15)
                    {
                        ball.SetPosition(newPos.x, 15);
                    }
                    else if (newPos.y > 585)
                    {
                        ball.SetPosition(newPos.x, 585);
                    }
                    ball.SetVelocity(ball.Velocity.x, -ball.Velocity.y);
                }
            }
        }

        internal void DrawBall(double posX, double posY)
        {

        }
    }
}