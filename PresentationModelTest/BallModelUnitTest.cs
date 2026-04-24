namespace PresentationModel.Test
{
    [TestClass]
    public class BallModelUnitTest
    {
        internal class FakePosition : BusinessLogic.IPosition
        {
            public double x { get; init; }
            public double y { get; init; }
        }

        internal class FakeLogicBall : BusinessLogic.IBall
        {
            public event EventHandler<BusinessLogic.IPosition>? NewPositionNotification;

            public void SimulateMove(double newX, double newY)
            {
                NewPositionNotification?.Invoke(this, new FakePosition { x = newX, y = newY });
            }
        }

        [TestMethod]
        public void BallModelCoordinateRecalculationAndNotificationTestMethod()
        {
            FakeLogicBall fakeLogicBall = new();
            BallModel ballModel = new BallModel(0, 0, 20, fakeLogicBall);

            var changedProperties = new List<string>();
            ballModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

            fakeLogicBall.SimulateMove(100, 100);

            Assert.AreEqual(90, ballModel.Left);
            Assert.AreEqual(90, ballModel.Top);

            Assert.IsTrue(changedProperties.Contains(nameof(BallModel.Left)));
            Assert.IsTrue(changedProperties.Contains(nameof(BallModel.Top)));
        }
    }
}
