namespace BusinessLogic.Test
{
    [TestClass]
    public class DataAbstractAPIUnitTest
    {
        [TestMethod]
        public void ConstructorTestTestMethod()
        {
            BusinessLogicAbstractAPI instance1 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            BusinessLogicAbstractAPI instance2 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            Assert.AreSame(instance1, instance2);
            instance1.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => instance2.Dispose());
        }

        [TestMethod]
        public void GetDimensionsTestMethod()
        {
            Assert.AreEqual<Dimensions>(new(20.0, 600.0, 600.0), BusinessLogicAbstractAPI.GetDimensions);
        }
    }
}
