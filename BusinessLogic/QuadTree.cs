namespace BusinessLogic
{
    internal class QuadTree
    {
        private const int capacity = 4; // pojemność kwadratu
        private Boundary boundary;        // obszar
        private List<Ball> balls = new();
        private QuadTree[] children = new QuadTree[4];   //węzły podrzędne
        private bool isDivided = false;

        public QuadTree(Boundary boundary)
        {
            this.boundary = boundary;
        }
        public bool Insert(Ball ball)
        {
            // sprawdzenie, czy kulka znajduje się w kwadracie
            if (!boundary.Contains(ball.position)) return false;

            // jeśli pasuje, a w kwadracie jest miejsce i nie ma on dzieci dodajemy
            if (balls.Count < capacity && !isDivided)
            {
                balls.Add(ball);
            }
            else
            {
                // jeśli nie ma już miejsca, a kwadrat jest niepodzielony - dzielimy go i przenosimy niżej kulki z bieżącego
                if (!isDivided)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        children[i] = new QuadTree(boundary.NewQuarter(i));
                    }

                    foreach (Ball b in balls)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (children[i].Insert(b)) break;
                        }
                    }
                    balls.Clear();

                    isDivided = true;
                }

                // próbujemy dodać kulkę do węzłów potomnych
                for (int i = 0; i < 4; i++) 
                { 
                    if (children[i].Insert(ball)) return true;
                }
            }
            return false;
        }

        public void Query(Boundary range, List<Ball> found)
        {
            if (!boundary.Intersects(range)) return;

            foreach (Ball b in balls)
            {
                found.Add(b);
            }

            if (isDivided)
            {
                foreach (QuadTree child in children) child.Query(range, found);
            }
        }
    }
}
