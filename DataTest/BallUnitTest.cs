namespace Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testinVector = new Vector(0.0, 0.0);
            Ball newInstance = new(testinVector, testinVector);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0));
            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => 
            {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++; 
            };
            newInstance.Move(new Vector(0.0, 0.0), false);
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
            Assert.AreEqual<IVector>(initialPosition, currentPosition);

            newInstance.Move(new Vector(0.0, 0.0), true);
            Assert.AreEqual<int>(1, numberOfCallBackCalled);

            IVector destinationPosition = new Vector(20.0, 5.0);
            newInstance.Move(new Vector(10.0, -5.0), false);
            Assert.AreEqual<int>(2, numberOfCallBackCalled);
            Assert.AreEqual<IVector>(destinationPosition, currentPosition);
        }

        [TestMethod]
        public void SetterTestMethod()
        {
            Ball newInstance = new(new Vector(0.0, 0.0), new Vector(0.0, 0.0));
            newInstance.SetVelocity(-5.0, 5.0);
            Assert.AreEqual(new Vector(-5.0, 5.0), newInstance.Velocity);
        }
    }
}