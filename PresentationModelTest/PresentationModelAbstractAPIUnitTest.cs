namespace PresentationModel.Test
{
    [TestClass]
    public class PresentationModelAbstractAPIUnitTest
    {
        [TestMethod]
        public void ConstructorTestTestMethod()
        {
            PresentationModelAbstractAPI instance1 = PresentationModelAbstractAPI.CreateModel();
            PresentationModelAbstractAPI instance2 = PresentationModelAbstractAPI.CreateModel();
            Assert.AreSame(instance1, instance2);
            instance1.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => instance2.Dispose());
        }
    }
}
