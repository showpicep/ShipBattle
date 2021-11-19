using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ShipBattle
{
    class Bot
    {
        private List<Ship> ships = new List<Ship>();

        private int countOfAlives;

        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];// карта бота 
        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];// карта игрока 
        private Random rdn = new Random();
        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

        Dictionary<int, int> countShips = new Dictionary<int, int>();

        public Bot(ref int[,] myMap,ref int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
        {
            countOfAlives = 10;
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.enemyButtons = enemyButtons;
            this.myButtons = myButtons;

            countShips.Add(4, 1);
            countShips.Add(3, 2);
            countShips.Add(2, 3);
            countShips.Add(1, 4);
        }

        public List<List<MyPoint>> GetDeads()
        {
            List<List<MyPoint>> list = new List<List<MyPoint>>();

            foreach(Ship s in ships)
            {
                if (!s.getIsAlive())
                {
                    list.Add(s.getPoints());
                }
            }

            return list;
        }
      

        /// <summary>
        /// Случайно выдается булевское значение, которое потом будем вставлять в функцию для определение положения корабля
        /// </summary>
        /// todo: какое распределение в рандоме в C#
        /// <returns></returns>
        public bool IsHorizontal()
        {
            return rdn.Next(0, 2) == 0;
        }

        /// <summary>
        /// Возвращает точку, от которой можно достроить вертикальный или горизонтальный корабль
        /// </summary>
        /// <param name="isHorizontal"></param>
        /// <param name="size">размер корабля</param>
        /// <returns></returns>
        public Point randomPoint(bool isHorizontal,int size)
        {
            Random rdn = new Random();
            Point point = new Point();

            point.X = rdn.Next(1, 11);
            point.Y = rdn.Next(1, 11);

            bool fit = false;

            while (!fit)
            {
                point.X = rdn.Next(1, 11);
                point.Y = rdn.Next(1, 11);

                if (isHorizontal)
                {
                    if (myMap[point.Y, point.X] != 1 && myMap[point.Y, point.X] != -1)
                        fit = point.X + size < 11;
                }
                else 
                {
                    if (myMap[point.Y, point.X] != 1 && myMap[point.Y, point.X] != -1)
                        fit = point.Y + size < 11;
                }
            }

            return point;
        }

        /// <summary>
        /// Обозначаем корабль числом 1 в матрице
        /// </summary>
        /// <param name="isHorizontal"></param>
        /// <param name="size"></param>
        /// <param name="point"></param>
        public void addShip(List<Point> l)
        {
            Ship ship = new Ship();

            foreach (Point p in l)
            {
                MyPoint newPoint = new MyPoint(p);
                ship.addPoint(newPoint);
                myMap[p.Y, p.X] = 1;
                //myButtons[p.Y, p.X].BackColor = Color.Red;//
            }

            ships.Add(ship);

            for (int i = 0; i < Form1.mapSize; i++)
            {
                for (int j = 0; j < Form1.mapSize; j++)
                {
                    if (myMap[i, j] == 1)
                    {
                        fillNeighborhoodShip(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// Полуучаем кол-во живых кораблей 
        /// </summary>
        /// <returns></returns>
        public int GetIsAlive()
        {
            int isAliveNow = 0;

            foreach (Ship s in ships)
            {
                s.checkLife();
                if (s.getIsAlive())
                {
                    isAliveNow++;
                }
            }

            return isAliveNow;
        }

        /// <summary>
        /// Проверка на жизнь корабля
        /// </summary>
        /// <returns></returns>
        public bool checkShips()
        {
            int isAliveNow = 0;

            foreach (Ship s in ships){
                s.checkLife();
                if (s.getIsAlive())
                {
                    isAliveNow++;
                }
            }
            //MessageBox.Show(isAliveNow.ToString());// Выводит кол-во живых кораблей
            bool flag = isAliveNow == countOfAlives;
            countOfAlives = isAliveNow;

            return flag;
        }

        public void getHit(int y, int x)
        {
            foreach(Ship s in ships)
            {
                s.getHit(y, x);
            }
        }

        /// <summary>
        /// Заполнение числом -1 окрестность корабля
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        public void fillNeighborhoodShip(int y,int x)
        {
            for (int k = y - 1; k <= y + 1; k++)
            {
                for (int c = x - 1; c <= x + 1; c++)
                    if (!cantFill(k, c))
                    {
                        myMap[k, c] = -1;
                        //if (k != 0 && c != 0 && k != 12 && c != 12)//
                        //{//
                        //    myButtons[k, c].BackColor = Color.Aquamarine;//
                        //}//
                    }
            }
        }

        /// <summary>
        /// Проверка на возможность задать значение в матрице
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool cantFill(int y, int x)
        {
            return !(y < Form1.mapSize && x < Form1.mapSize && myMap[y, x] != 1);
        }

        /// <summary>
        /// Проверка на возможность поставить корабль в окресности корабля или на сам корабль
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns> 
        public bool check(List<Point> l)
        {
            bool isChecked = true;

            foreach (Point p in l)
            {
                isChecked = isChecked && !(myMap[p.Y, p.X] == 1 || myMap[p.Y, p.X] == -1);
            }
            return isChecked;
        }

        public void placeShips()
        {
            int start = 4;

            while (start > 0)
            {
                int k = 0;
                int kx = 0;
                int ky = 0;

                if (countShips[start] > 0)
                {
                    List<Point> l = new List<Point>();
                    bool flag = IsHorizontal();
                    Point point = randomPoint(flag, start);

                    while (k < start)
                    {
                        int x = point.X + kx; // Горизонталь
                        int y = point.Y + ky; // Вертикаль
                        l.Add(new Point(x, y));

                        if (flag)
                        {
                            kx++;
                        }
                        else
                        {
                            ky++;
                        }
                        k++;
                    }

                    if (check(l))
                    {
                        addShip(l);
                        countShips[start]--;
                    }
                }
                else
                    start--;
            }
        }

        public void ConfigureShips()
        {
            placeShips();
            //for (int i = 0; i < Form1.mapSize; i++)
            //{
            //    for (int j = 0; j < Form1.mapSize; j++)
            //    {
            //        if (myMap[i,j] == 1)
            //        {
            //            myButtons[i, j].BackColor = Color.Red;
            //        }
            //    }
            //}
        }

        public bool Shoot()
        {
            bool hit =false;

            Random r = new Random();

            int posX = r.Next(1, Form1.mapSize);
            int posY = r.Next(1, Form1.mapSize);
            if (Form1.isPlaying)
            {
                while (enemyButtons[posY, posX].BackColor == Color.Blue || enemyButtons[posY, posX].BackColor == Color.Black)
                {
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);
                }

                if (enemyMap[posY, posX] == 1)
                {
                    hit = true;
                    enemyMap[posY, posX] = 0;
                    enemyButtons[posY, posX].BackColor = Color.Blue;
                    enemyButtons[posY, posX].Text = "X";
                }
                else
                {
                    hit = false;
                    enemyButtons[posY, posX].BackColor = Color.Black;
                }
                if (hit)
                    Shoot();
            }
            return hit;
        }

        public bool SmartShoot()
        {
            bool hit = false;

            Random r = new Random();

            int posX = r.Next(1, Form1.mapSize);
            int posY = r.Next(1, Form1.mapSize);
            if (Form1.isPlaying)
            {
                while (enemyButtons[posY, posX].BackColor == Color.Blue || enemyButtons[posY, posX].BackColor == Color.Black)
                {
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);
                }

                if (enemyMap[posY, posX] == 1)
                {
                    hit = true;
                    enemyMap[posY, posX] = 0;
                    enemyButtons[posY, posX].BackColor = Color.Blue;
                    enemyButtons[posY, posX].Text = "X";
                }
            }

                return hit;
        }
    }
}
