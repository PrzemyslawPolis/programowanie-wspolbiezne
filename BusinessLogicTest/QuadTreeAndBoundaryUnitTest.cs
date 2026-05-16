using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic;
using Data;

namespace BusinessLogic.Test
{
    [TestClass]
    public class QuadTreeAndBoundaryUnitTest
    {
        private class FakeDataBall : Data.IBall
        {
            public IVector Velocity { get; set; } = null!;
            public event EventHandler<IVector>? NewPositionNotification;
            public void SetPosition(double x, double y) { }
            public void SetVelocity(double x, double y) { }
        }

        private Ball CreateTestBall(double x, double y)
        {
            return new Ball(new Position(x, y), new FakeDataBall());
        }

        [TestMethod]
        public void BoundaryContainsShouldIdentifyPointsCorrectly()
        {
            Boundary boundary = new Boundary(0, 0, 100, 100);

            // punkt na środku
            Assert.IsTrue(boundary.Contains(new Position(50, 50)));

            // Punkt na lewej górnej krawędzi
            Assert.IsTrue(boundary.Contains(new Position(0, 0)));

            // Punkt całkowicie poza prostokątem
            Assert.IsFalse(boundary.Contains(new Position(150, 150)));
            Assert.IsFalse(boundary.Contains(new Position(-10, 50)));

            // Punkt na prawej i dolnej krawędzi
            Assert.IsFalse(boundary.Contains(new Position(100, 100)));
        }

        [TestMethod]
        public void BoundaryIntersectsShouldDetectOverlaps()
        {
            Boundary mainBoundary = new Boundary(0, 0, 100, 100);

            // Prostokąt wewnątrz
            Boundary inside = new Boundary(25, 25, 50, 50);
            Assert.IsTrue(mainBoundary.Intersects(inside));

            // Prostokąt częściowo nachodzący
            Boundary overlapping = new Boundary(80, 80, 50, 50);
            Assert.IsTrue(mainBoundary.Intersects(overlapping));

            // Prostokąt rozłączny
            Boundary outside = new Boundary(150, 150, 50, 50);
            Assert.IsFalse(mainBoundary.Intersects(outside));
        }

        [TestMethod]
        public void BoundaryNewQuarterShouldCreateCorrectSubBoundaries()
        {
            Boundary parent = new Boundary(0, 0, 100, 100);

            Boundary q0 = parent.NewQuarter(0); // Lewy górny
            Boundary q1 = parent.NewQuarter(1); // Prawy górny
            Boundary q2 = parent.NewQuarter(2); // Lewy dolny
            Boundary q3 = parent.NewQuarter(3); // Prawy dolny

            // czy wymiary zmniejszyły się o połowę
            Assert.AreEqual(50, q0.width);
            Assert.AreEqual(50, q0.height);

            // początki współrzędnych dla prostokątów
            Assert.AreEqual(0, q0.x); Assert.AreEqual(0, q0.y);
            Assert.AreEqual(50, q1.x); Assert.AreEqual(0, q1.y);
            Assert.AreEqual(0, q2.x); Assert.AreEqual(50, q2.y);
            Assert.AreEqual(50, q3.x); Assert.AreEqual(50, q3.y);
        }


        [TestMethod]
        public void QuadTreeInsertAndQueryShouldFilterBallsBySubdivision()
        {
            Boundary table = new Boundary(0, 0, 100, 100);
            QuadTree tree = new QuadTree(table);

            // 3 kule w lewym górnym rogu
            tree.Insert(CreateTestBall(10, 10));
            tree.Insert(CreateTestBall(20, 20));
            tree.Insert(CreateTestBall(30, 30));

            // 2 kule w prawym dolnym rogu, przekroczenie capacity
            tree.Insert(CreateTestBall(80, 80));
            tree.Insert(CreateTestBall(90, 90));

            List<Ball> foundTopLeft = new();
            tree.Query(new Boundary(0, 0, 50, 50), foundTopLeft);
            Assert.AreEqual(3, foundTopLeft.Count);

            List<Ball> foundBottomRight = new();
            tree.Query(new Boundary(50, 50, 50, 50), foundBottomRight);
            Assert.AreEqual(2, foundBottomRight.Count);

            List<Ball> foundTopRight = new();
            tree.Query(new Boundary(50, 0, 50, 50), foundTopRight);
            Assert.AreEqual(0, foundTopRight.Count);
        }
    }
}