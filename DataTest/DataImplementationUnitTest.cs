namespace Data.Test
{
  [TestClass]
  public class DataImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        IEnumerable<IBall>? ballsList = null;
        newInstance.CheckBallsList(x => ballsList = x);
        Assert.IsNotNull(ballsList);
        int numberOfBalls = 0;
        newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
        Assert.AreEqual<int>(0, numberOfBalls);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataImplementation newInstance = new DataImplementation();
      bool newInstanceDisposed = false;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      IEnumerable<IBall>? ballsList = null;
      newInstance.CheckBallsList(x => ballsList = x);
      Assert.IsNotNull(ballsList);
      newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(0, x));
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
    }

    [TestMethod]
    public void StartTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        int numberOfCallbackInvoked = 0;
        int numberOfBallsToCreate = 10;
        newInstance.Start(numberOfBallsToCreate, (startingPosition, ball) =>
          {
            numberOfCallbackInvoked++;
            Assert.IsTrue(startingPosition.x >= 0);
            Assert.IsTrue(startingPosition.y >= 0);
            Assert.IsNotNull(ball);
          });
        Assert.AreEqual<int>(numberOfBallsToCreate, numberOfCallbackInvoked);
        newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(10, x));
      }
    }

    [TestMethod]
    public async Task StartsAsyncBallMovementTestMethod()
    {
        using (DataImplementation newInstance = new DataImplementation())
        {
            IVector? changedPosition = null;

            newInstance.Start(1, (startingPos, ball) =>
            {
                ball.NewPositionNotification += (sender, newPos) =>
                {
                    changedPosition = newPos;
                };
            });

            await Task.Delay(100);

            Assert.IsNotNull(changedPosition);
        }
       }
   }
}