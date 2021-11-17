using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipBattle
{
    internal class MyPoint
    {
        public Point point;
        private bool isAlive;
        public MyPoint()
        {
            isAlive = true;
        }

        public MyPoint(Point point)
        {
            this.point = point;
            isAlive = true;
        }

        public bool getIsAlive()
        {
            return isAlive;
        }

        public void setIsAlive(bool flag)
        {
            isAlive = flag;
        }


    }
}
