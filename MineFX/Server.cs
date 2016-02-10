using LightFX;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MineFX
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        public Boolean ClientConnected = false;
        protected Player player;
        private LightState prevState = LightState.Green;
        private double prevHealt;

        public Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();

        }

        private void ListenForClients()
        {
            this.tcpListener.Start();

            Console.WriteLine("------------Server started!------------");

                    Console.WriteLine("Waiting for connection...");
                    //blocks until a client has connected to the server
                    TcpClient client = this.tcpListener.AcceptTcpClient();

                    //create a thread to handle communication 
                    //with connected client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                    ClientConnected = true;
                    Console.WriteLine("------------Client connected------------");
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //Console.WriteLine("wait for message...");
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error on message received!");
                    Console.WriteLine(e.StackTrace);
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    Console.WriteLine("------------Client disconnected------------");
                    ClientConnected = false;
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                string JSONData = encoder.GetString(message, 0, bytesRead).Split('-').Last();
                //Console.WriteLine(JSONData);

                try
                {
                    this.player = JsonConvert.DeserializeObject<Player>(JSONData);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {


                }


                if(prevHealt != this.player.Healt)
                {
                    UpdateLights();
                    prevHealt = this.player.Healt;
                }
                


                //LFX_ColorStruct(255, 0, 255, 0);          groen
                //new LFX_ColorStruct(255, 255, 0, 0);      rood
                //LFX_ColorStruct(255, 255, 128, 0)

            }

            Console.WriteLine("Close connection...");
            tcpClient.Close();
            tcpListener.Stop();

            Program.lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, new LFX_ColorStruct(255, 0, 255, 0), new LFX_ColorStruct(255, 255, 0, 0));
            Program.lightFX.LFX_Update();

            Program.lightFX.LFX_Light(LFX_Position.LFX_All, new LFX_ColorStruct(255, 255, 0, 0));
            System.Threading.Thread.Sleep(Program.FadeTime);
            Program.lightFX.LFX_Update();

            Console.WriteLine("shutdown server...");

            Program.lightFX.LFX_Release();
        }

        private void UpdateLights()
        {
                if (this.player != null)
                {
                    if (this.player.Healt > 9 && prevState == LightState.Red)
                    {
                        Console.WriteLine("Red --> orange");

                        Program.lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, new LFX_ColorStruct(255, 255, 0, 0), new LFX_ColorStruct(255, 255, 128, 0));
                        Program.lightFX.LFX_Update();

                        Thread.Sleep(Program.FadeTime);
                        Program.lightFX.LFX_Light(LFX_Position.LFX_All, new LFX_ColorStruct(255, 255, 128, 0));
                        Program.lightFX.LFX_Update();
                        prevState = LightState.Orange;
                        UpdateLights();
                    }
                    else if (player.Healt <= 15 && prevState == LightState.Green)
                    {
                        Console.WriteLine("Green --> orange");

                        Program.lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, new LFX_ColorStruct(255, 0, 255, 0), new LFX_ColorStruct(255, 255, 128, 0));
                        Program.lightFX.LFX_Update();

                        Thread.Sleep(Program.FadeTime);
                        Program.lightFX.LFX_Light(LFX_Position.LFX_All, new LFX_ColorStruct(255, 255, 128, 0));
                        Program.lightFX.LFX_Update();
                        prevState = LightState.Orange;
                        UpdateLights();

                    }
                    else if (player.Healt > 15  && prevState == LightState.Orange)
                    {
                        Console.WriteLine("orange --> green");

                        Program.lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, new LFX_ColorStruct(255, 255, 128, 0), new LFX_ColorStruct(255, 0, 255, 0));
                        Program.lightFX.LFX_Update();

                        Thread.Sleep(Program.FadeTime);
                        Program.lightFX.LFX_Light(LFX_Position.LFX_All, new LFX_ColorStruct(255, 0, 255, 0));
                        Program.lightFX.LFX_Update();
                        prevState = LightState.Green;
                        UpdateLights();
                    }
                    else if (player.Healt <=9 && prevState == LightState.Orange)
                    {
                        Console.WriteLine("orange --> red");

                        Program.lightFX.LFX_ActionColorEx(LFX_Position.LFX_All, LFX_ActionEnum.Morph, new LFX_ColorStruct(255, 255, 128, 0), new LFX_ColorStruct(255, 255, 0, 0));
                        Program.lightFX.LFX_Update();

                        Thread.Sleep(Program.FadeTime);
                        Program.lightFX.LFX_Light(LFX_Position.LFX_All, new LFX_ColorStruct(255, 255, 0, 0));
                        Program.lightFX.LFX_Update();
                        prevState = LightState.Red;
                        UpdateLights();
                    }
                    else if (player.Healt > 3 && ((player.Healt <= 9 && prevState!=LightState.Red)|| prevState==LightState.Pulse))
                    {
                        // Red
                        Console.WriteLine("red");
                        Program.lightFX.LFX_Light(LFX_Position.LFX_All, new LFX_ColorStruct(255, 255, 0, 0));
                        Program.lightFX.LFX_Update();
                        prevState = LightState.Red;
                        UpdateLights();

                    }
                    else if (player.Healt <= 3 && prevState != LightState.Pulse)
                    {
                        Console.WriteLine("Pulse red");
                        // Pulse red

                        Program.lightFX.LFX_ActionColor(LFX_Position.LFX_All, LFX_ActionEnum.Pulse, new LFX_ColorStruct(255, 255, 0, 0));
                        Program.lightFX.LFX_Update();
                        prevState = LightState.Pulse;
                        UpdateLights();
                    }
                    //Console.WriteLine(this.player.Healt);
            }
        }
    }

    public class Player
    {
        public float Healt { get; set; }
        public int Hunger { get; set; }
    }
    public enum LightState
    {
        Green,
        Orange,
        Red,
        Pulse
    }
}
