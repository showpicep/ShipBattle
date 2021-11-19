using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ShipBattle
{
    static class SoundForGame
    {
        static public void LakadMatatag()
        {
            SoundPlayer sp = new SoundPlayer(@"C:\Users\Acer\Desktop\myproj2113\ShipBattle\Sounds\Lakad-Matatag.wav");
            sp.Play();
        }

        static public void FB()
        {
            SoundPlayer sp = new SoundPlayer(@"C:\Users\Acer\Desktop\myproj2113\ShipBattle\Sounds\firstblood.wav");
            sp.Play();
        }

        static public void PlayerWin()
        {
            SoundPlayer sp = new SoundPlayer(@"C:\Users\Acer\Desktop\myproj2113\ShipBattle\Sounds\Eta-GG.wav");
            sp.Play();
        }

        static public void BotWin()
        {
            SoundPlayer sp = new SoundPlayer(@"C:\Users\Acer\Desktop\myproj2113\ShipBattle\Sounds\Eta-Prosto-Nechta.wav");
            sp.Play();
        }

        static public void Vilat()
        {
            SoundPlayer sp = new SoundPlayer(@"C:\Users\Acer\Desktop\myproj2113\ShipBattle\Sounds\Chat-Wheel.wav");
            sp.Play();
        }
    }

}
