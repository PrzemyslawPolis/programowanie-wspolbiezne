namespace Data.Test
{
    [TestClass]
    public class LoggerUnitTest
    {
        [TestMethod]
        public void LoggerSaveToFileWithDisposeTestMethod()
        {
            string expectedFilePath;

            using (Logger logger = new Logger())
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                expectedFilePath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\..\Data", "logs.json"));

                Ball testBall = new Ball(new Vector(10.0, 20.0), new Vector(1.0, -1.0));

                logger.LogBallState(testBall);

            } //wywołanie Dispose()

            Assert.IsTrue(File.Exists(expectedFilePath));

            string[] savedLines = File.ReadAllLines(expectedFilePath);
            Assert.IsTrue(savedLines.Length >= 1);

            string lastLog = savedLines.Last();
            Assert.IsTrue(lastLog.Contains("\"X\": 10.00") || lastLog.Contains("\"X\": 10,00"));
            Assert.IsTrue(lastLog.Contains("\"Y\": 20.00") || lastLog.Contains("\"Y\": 20,00"));
        }
    }
}