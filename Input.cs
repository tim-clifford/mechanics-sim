using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Structures;
using static Program.Constants;
using static Program.Globals;
using Gtk;
using Gdk;
using Cairo;
using Graphics;
namespace Program {
    static class Input {
        private static bool canMove = false;
        private static Vector3 rootPos = null;
        private static Vector3 rootAngle = null;
        public static readonly double mouse_sensitivity = 1;
        public static readonly double scroll_sensitivity = 1.1;
        [GLib.ConnectBefore]
    	public static void KeyPress(object sender, KeyPressEventArgs args) {
	    	if (args.Event.Key == Gdk.Key.r) {
                double d = Vector3.Magnitude(active_syswindow.camera.position);
                active_syswindow.camera = new Camera(d,Vector3.zero);
            } else if (args.Event.Key == Gdk.Key.l) {
                canMove = !canMove;
                if (!canMove) {
                    rootPos = null;
                }
            }

	    }
        [GLib.ConnectBefore]
        public static void MouseMovement(Object sender, MotionNotifyEventArgs args) {
            if (canMove) {
                if (rootPos == null || rootAngle == null ) {
                    rootPos = new Vector3(args.Event.X,args.Event.Y,0);
                    rootAngle = active_syswindow.camera.angle;
                } else {
                    double d = Vector3.Magnitude(active_syswindow.camera.position);
                    active_syswindow.camera = new Camera(d,rootAngle + deg*mouse_sensitivity* new Vector3(rootPos.y - args.Event.Y,args.Event.X - rootPos.x, 0));
                } args.RetVal = true;
            }
        }
        [GLib.ConnectBefore]
        public static void Scroll(Object sender, ScrollEventArgs args) {
            if (args.Event.Direction == Gdk.ScrollDirection.Up) {
                active_syswindow.bounds_multiplier /= scroll_sensitivity;
            } else if (args.Event.Direction == Gdk.ScrollDirection.Down) {
                active_syswindow.bounds_multiplier *= scroll_sensitivity;
            }
        }
    }
}