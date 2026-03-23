namespace DataTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int[] tablica = new int[5] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(5, tablica.Length);
        }
    }
}
