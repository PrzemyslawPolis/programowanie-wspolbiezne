using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{    internal record Boundary
    {
        public double x { get; }
        public double y { get;  }
        public double width { get; }
        public double height { get; }

        public Boundary(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Contains(IPosition pos)
        {
            return pos.x >= x && pos.y >= y && pos.x < x + width && pos.y < y + height; 
        }

        public bool Intersects(Boundary bound)
        {
            
        }

        public Boundary NewQuarter (int number)
        {
            if (number >= 0 && number <= 3)
            {
                double newX = x;
                double newY = y;

                switch (number)
                {
                    case 0:
                        break;
                    case 1:
                        newX = x + width / 2;
                        break;
                    case 2:
                        newY = y + height / 2;
                        break;
                    case 3:
                        newX = x + width / 2;
                        newY = y + height / 2;
                        break;
                }

                return new Boundary(newX, newY, width / 2, height / 2);
            }
            else return null;
        }
    }
}
