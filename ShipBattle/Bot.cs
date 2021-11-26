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
        AnimationForHit anim = new AnimationForHit();

        private List<Ship> ships = new List<Ship>();

        private int countOfAlives;

        Player player;

        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];// карта бота 
        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];// карта игрока 

        private Random rdn = new Random();

        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

        Dictionary<int, int> countShips = new Dictionary<int, int>();

        public Bot(ref int[,] myMap,ref int[,] enemyMap,ref Button[,] myButtons,ref Button[,] enemyButtons, ref Player player)
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

            this.player = player; //new Player(ref enemyMap, ref myMap, ref enemyButtons, ref myButtons);
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

        public List<List<MyPoint>> GetDeads()
        {
            List<List<MyPoint>> list = new List<List<MyPoint>>();

            foreach (Ship s in ships)
            {
                if (!s.getIsAlive())
                {
                    list.Add(s.getPoints());
                }
            }

            return list;
        }

        /// <summary>
        /// Проверка на жизнь корабля
        /// </summary>
        /// <returns></returns>
        public bool checkShips()
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
            //MessageBox.Show(isAliveNow.ToString());// Выводит кол-во живых кораблей
            bool flag = isAliveNow == countOfAlives;
            countOfAlives = isAliveNow;

            return flag;
        }

        public void getHit(int y, int x)
        {
            foreach (Ship s in ships)
            {
                s.getHit(y, x);
            }
        }

        public bool Shoot(bool isSmart)
        {
            anim.GettingImages();

            List<Ship> list = player.getShips();

            List<Ship> listOfShips = new List<Ship>();

            foreach (Ship s in list)
            {
                if (!s.getIsAlive()) {
                    continue;
                }

                bool flag = true;

                foreach (MyPoint p in s.getPoints())
                {
                    flag = flag && p.getIsAlive();
                }

                if (!flag)
                {
                    listOfShips.Add(s);
                }
            }

            Random r = new Random();

            double chance = r.NextDouble();

            if (chance < 0.5 && listOfShips.Count >= 1)
            {
                Ship s = listOfShips[r.Next(0, listOfShips.Count - 1)];
                MyPoint point = null;

                foreach (MyPoint p in s.getPoints())
                {
                    if (p.getIsAlive())
                    {
                        point = p;
                    }
                }

                if (point == null) 
                {
                    return doShoot(r.Next(1, Form1.mapSize), r.Next(1, Form1.mapSize),false);
                }

                return doShoot(point.point.X, point.point.Y, true);
            }
            else
            {
                return doShoot(r.Next(1, Form1.mapSize), r.Next(1, Form1.mapSize),false);
            }
        }

        private async void Print(Button pressedButton)
        {
            foreach (Image img in anim.frame.ToList())
            {
                pressedButton.Image = img;

                await Task.Delay(100);
            }
        }

        private bool doShoot(int posX, int posY, bool isSmart)
        {

            Random r = new Random();
            bool hit = false;

            if (Form1.isPlaying)
            {
                while (enemyMap[posY, posX] == 2 || enemyMap[posY, posX] == -2 || enemyButtons[posY, posX].BackColor == Color.Aquamarine)
                {
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);
                }

                if (enemyMap[posY, posX] == 1)
                {
                    player.getHit(posY, posX);

                    Print(enemyButtons[posY, posX]);

                    //enemyButtons[posY, posX].BackColor = Color.Green; // попадение
                    //enemyButtons[posY, posX].Text = "X";
                    enemyMap[posY, posX] = 2;
                    hit = true;
                }
                else
                {
                    hit = false;
                    enemyButtons[posY, posX].BackColor = Color.Black;  // промах 
                }
                if (hit)
                {
                    if (isSmart)
                    {
                        Shoot(true);
                    }
                    else
                    {
                        Shoot(false);
                    }

                }
            }
            return hit;
        }

        //public bool SmartShoot(bool isFirst = false)
        //{
        //    List<List<MyPoint>> list = player.GetDeads();
            
        //    bool hit = Shoot();
        //    Random r = new Random();
        //    double chance = r.NextDouble();

        //    if (hit)
        //    {

        //    }
        //    else
        //        Shoot();

        //    return hit;
        //}


        /// <summary>
        /// Отрисовка окрестности убитых ботом кораблей
        /// </summary>
        public void Render()
        {
            List<List<MyPoint>> list = player.GetDeads();

            foreach (List<MyPoint> l in list)
            {
                MyPoint first = l[0];
                MyPoint last = l[l.Count - 1];

                first = new MyPoint(new Point(first.point.X - 1, first.point.Y - 1));
                last = new MyPoint(new Point(last.point.X + 1, last.point.Y + 1));

                for (int i = first.point.Y; i <= last.point.Y; i++)
                {
                    for (int j = first.point.X; j <= last.point.X; j++)
                    {

                        if (i > 0 && j > 0 && i < 11 && j < 11 && enemyMap[i, j] != 2)
                        {
                            enemyButtons[i, j].BackColor = Color.Aquamarine;
                            enemyButtons[i, j].Enabled = false;
                        }

                    }
                }

            }
        }

    }
}
