using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipBattle
{
    static class Errors
    {

        static public void showError_1()
        {
            MessageBox.Show("Вы ставите в окрестность другого корабля");
        }

        static public void showError_2()
        {
            MessageBox.Show("Корабль выходит за границы карты");
        }

        static public void showError_3()
        {
            MessageBox.Show("Выберите количество палуб у корабля");
        }

        static public void showError_4()
        {
            MessageBox.Show("Выберите положение коробля");
        }

        static public void showError_5()
        {
            MessageBox.Show("Вы не выбрали корабль");
        }

    }
}
