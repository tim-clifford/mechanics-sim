using System;
using Gtk;
using Gdk;
using System.Threading.Tasks;
using System.Collections.Generic;
using static Program.Input;
namespace Structures {
    static class Tests {
        public static int i = 0;
        public static Graphics.SystemView sys_view;

        public static void PITest() {
            readonly int DIGITS = 5;
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
            var mainWindow = new Gtk.Window("Mechanics Simulation");
            //mainWindow.SetDefaultSize(1280,720);
            sys_view = new Graphics.SystemView(sys);
            mainWindow.Add(sys_view);
            mainWindow.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask;
            mainWindow.DeleteEvent += delegate { Application.Quit (); };
            mainWindow.KeyPressEvent += Program.Input.KeyPress;
            mainWindow.MotionNotifyEvent += Program.Input.MouseMovement;
            mainWindow.ScrollEvent += Program.Input.Scroll;
            sys_view.PlayAsync(interval: 0);
            mainWindow.ShowAll();

            Task.Run(() => {
                while (true) {
                    sys.TimeStep(Math.Pow(10,-DIGITS - 2));
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