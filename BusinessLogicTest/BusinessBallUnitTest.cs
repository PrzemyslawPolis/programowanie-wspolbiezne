using Data;

namespace BusinessLogic.Test
{
    [TestClass]
    public sealed class BusinessBallUnitTest
    {
        private class FakeDataBall : Data.IBall
        {
            public IVector Velocity { get; set; } = null!; 
            public event EventHandler<IVector>? NewPositionNotification;
            public void SetPosition(double x, double y) { }
            public void SetVelocity(double x, double y) { }
        }

        [TestMethod]
        public void CallbackTestMethod()
        {
            IPosition initialPosition = new Position(0.0, 0.0);
            Data.IBall fakeDataBall = new FakeDataBall();

            Ball newInstance = new Ball(initialPosition, fakeDataBall);

            IPosition currentPosition = new Position(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += ((sender, position) => {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            });

            newInstance.UpdatePosition(10.0, 5.0);

            Assert.AreEqual(1, numberOfCallBackCalled);
            Assert.AreEqual(10.0, currentPosition.x);
            Assert.AreEqual(5.0, currentPosition.y);
        }
    }
}