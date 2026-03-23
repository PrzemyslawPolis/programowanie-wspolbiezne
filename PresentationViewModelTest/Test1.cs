using PresentationModel;
using PresentationViewModel;

namespace PresentationViewModelTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void HelloDataTest()
        {
            ViewModel model = new ViewModel();
            Assert.AreEqual("Hello Data!", model.GetString());
        }
    }
}
