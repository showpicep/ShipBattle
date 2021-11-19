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
        List<MyPoint> ship;
        private bool isAlive;

        public Ship()
        {
            ship = new List<MyPoint>();
            isAlive = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void addPoint(MyPoint p)
        {
            ship.Add(p);
        }
        /// <summary>
        /// Проверка на жизнь клетки
        /// </summary>
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
        /// <summary>
        /// Возвращаем значение жив ли корабль
        /// </summary>
        /// <returns></returns>
        public bool getIsAlive()
        {
            return isAlive;
        }


        /// <summary>
        /// Задаем значение жив ли корабль 
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MyPoint> getPoints()
        {
            return ship;
        }


    }
}
