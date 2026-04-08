namespace Data
{
    internal record Vector : IVector
    {
        
        public double x { get; init; }
        public double y { get; init; }


        public Vector(double XComponent, double YComponent)
        {
            x = XComponent;
            y = YComponent;
        }
    }
}