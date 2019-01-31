using System;
using Gtk;
using Gdk;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimObjects;
using static Program.Globals;
namespace Structures {
    static class Tests {
        public static readonly int DIGITS = 3;
        public static readonly double PRECISION = -DIGITS-2; 
        public static void PITest() {
            
            ObjectSystem sys = new ObjectSystem(new List<SimObject>() {
                new Block() {
                    position = new Vector3(0,0,0),
                    velocity = new Vector3(0,0,0),
                    mass = 1e300,
                    dimensions = new Vector3(2,8,8),
                    color = new Vector3(1,0,0),
                    orientation = new Vector3(0,0,0),
                },
                new Ball() {
                    position = new Vector3(2.5,0,0),
                    velocity = new Vector3(0,0,0),
                    mass = 1,
                    color = new Vector3(0,1,0),
                },
                new Ball() {
                    position = new Vector3(5,0,0),
                    velocity = new Vector3(-1,0,0),
                    mass = Math.Pow(100,DIGITS),
                    color = new Vector3(0,0,1),
                },
            });
            int j = 0;
            int last = -1;
            Application.Init();
            var sysWindow = new Graphics.SystemWindow("Mechanics Simulation",sys);
            sysWindow.PlayAsync(interval: 0);
            active_syswindow = sysWindow;
            Console.WriteLine("hi");
            Task.Run(() => {
                while (true) {
                    sys.TimeStep(Math.Pow(10,PRECISION));
                    // Don't output every frame, and only if the number of collisions has changed
                    if (j++%1e4 == 0 && last != collision_counter) {
                        Console.WriteLine($"{collision_counter} collisions");
                        last = collision_counter;
                    }
                }
            });
            Application.Run();
        }
    }
}