using System;
using Gtk;
using Gdk;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Structures {
    static class Tests {
        public static int i = 0;
        public static Graphics.SystemView sys_view;
        public static readonly int DIGITS = 6;
        public static void PITest() {
            
            ObjectSystem sys = new ObjectSystem(new List<SimObject>() {
                new Ball() {
                    position = new Vector3(0,0,0),
                    velocity = new Vector3(0,0,0),
                    mass = 1e99,
                    radius = 1,
                    color = new Vector3(1,0,0)
                },
                new Ball() {
                    position = new Vector3(2.5,0,0),
                    velocity = new Vector3(0,0,0),
                    mass = 1,
                    color = new Vector3(0,1,0)
                },
                new Ball() {
                    position = new Vector3(5,0,0),
                    velocity = new Vector3(-1,0,0),
                    mass = Math.Pow(100,DIGITS),
                    color = new Vector3(0,0,1)
                }
            });
            int j = 0;
            int last = -1;
            Application.Init();
            var sysWindow = new SystemWindow("Mechanics Simulation",sys);
            sysWindow.PlayAsync(interval: 0);
            Console.WriteLine("hi");
            Task.Run(() => {
                while (true) {
                    sys.TimeStep(Math.Pow(10,-DIGITS-2));
                    if (j++%1e4 == 0 && last != Tests.i) {
                        Console.WriteLine($"{Tests.i} collisions");
                        last = Tests.i;
                    }
                }
            });
            Application.Run();
        }
    }
}