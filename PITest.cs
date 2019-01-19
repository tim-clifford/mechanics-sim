using System;
using Gtk;
using Gdk;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimObjects;
using static Program.Globals;
namespace Structures {
    static class Tests {
        public static int i = 0;
        public static Graphics.SystemView sys_view;
        public static readonly int DIGITS = 4;
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
                new Block() {
                    position = new Vector3(2.5,0,0),
                    velocity = new Vector3(0,0,0),
                    mass = 1,
                    color = new Vector3(0,1,0),
                    dimensions = new Vector3(1,1,1),
                },
                new Block() {
                    position = new Vector3(5,0,0),
                    velocity = new Vector3(-1,0,0),
                    mass = Math.Pow(100,DIGITS),
                    color = new Vector3(0,0,1),
                    dimensions = new Vector3(3,3,3),
                },
            });
            int j = 0;
            int last = -1;
            Application.Init();
            var sysWindow = new Graphics.SystemWindow("Mechanics Simulation",sys);
            sysWindow.PlayAsync(interval: 0);
            activesyswindow = sysWindow;
            Console.WriteLine("hi");
            Task.Run(() => {
                while (true) {
                    sys.TimeStep(Math.Pow(10,-DIGITS-1));
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