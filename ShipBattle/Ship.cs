using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipBattle
{
    class Ship
    {
        private List<MyPoint> ship;
        private bool isAlive;

        public Ship()
        {
            ship = new List<MyPoint>();
            isAlive = true;
        }

        public void addPoint(MyPoint p)
        {
            ship.Add(p);
        }

        public void checkLife()
        {
            int k = 0;
            foreach (MyPoint point in ship)
            {
                if (point.getIsAlive()) {
                    k++;
                }
            }

            isAlive = k != 0;
        }

        public bool getIsAlive()
        {
            return isAlive;
        }
        public void getHit(int y, int x) 
        {
            foreach(MyPoint p in ship)
            {
                if (p.point.Y == y && p.point.X == x)
                {
                    p.setIsAlive(false);
                }
            }
        }

        public List<MyPoint> getPoints()
        {
            return ship;
        }


    }
}
