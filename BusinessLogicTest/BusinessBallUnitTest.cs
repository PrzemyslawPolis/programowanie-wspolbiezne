namespace BusinessLogic.Test
{
    [TestClass]
    public sealed class BusinessBallUnitTest
    {
        [TestMethod]
        public void CallbackTestMethod()
        {
            Ball newInstance = new Ball();
            IPosition currentPosition = new Position(0.0, 0.0);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += ((sender, position)=> {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            });
            newInstance.UpdatePosition(10.0, 5.0);
            Assert.AreEqual(1, numberOfCallBackCalled);
        }
    }
}
