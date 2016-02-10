using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightFX;

namespace MineFX
{
    class Program
    {
        public static LightFXController lightFX = new LightFXController();
        public static int FadeTime = 3800; //3800

        static void Main(string[] args)
        {

            var result = lightFX.LFX_Initialize();
            if (result == LFX_Result.LFX_Success)
            {

                var server = new Server();
                
                lightFX.LFX_SetTiming(100);
                lightFX.LFX_Reset();

                var Green = new LFX_ColorStruct(255, 0, 255, 0);
                var Red = new LFX_ColorStruct(255, 255, 0, 0);

                System.Threading.Thread.Sleep(3000);

                

                while (!server.ClientConnected)
                {

                    lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, Green, Red);
                    //Console.WriteLine(string.Format("Morphing all lights from color {0} to {1}.", Green, Red));
                    lightFX.LFX_Update();

                    System.Threading.Thread.Sleep(FadeTime);

                    lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, Red, Green);
                    //Console.WriteLine(string.Format("Morphing all lights from color {0} to {1}.", Red, Green));
                    lightFX.LFX_Update();

                    lightFX.LFX_Light(LFX_Position.LFX_All, Green);
                    System.Threading.Thread.Sleep(FadeTime);
                    lightFX.LFX_Update();
                }




            }
            else
            {
                switch (result)
                {
                    case LFX_Result.LFX_Error_NoDevs:
                        Console.WriteLine("There is not AlienFX device available.");
                    default:
                        Console.WriteLine("There was an error initializing the AlienFX device.");
                        break;
                }
            }

        }
    }
}
