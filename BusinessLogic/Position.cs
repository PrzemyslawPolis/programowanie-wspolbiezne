namespace BusinessLogic
{
    internal record Position : IPosition
    {
       
        public double x { get; init; }
        public double y { get; init; }


        public Position(double posX, double posY)
        {
            x = posX;
            y = posY;
        }
    }
}
