using System;
using System.Linq;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Cairo;
using Structures;
using SimObjects;
using System.Threading;
using System.Threading.Tasks;
using static Program.Constants;
namespace Graphics {
	class Camera {
		public Vector3 position {get; protected set;}
		public Vector3 angle {get; protected set;}
		public Camera(double distance, Vector3 angle) {
			// the camera always "points" to the origin
			this.angle = angle;
			position = Matrix3.IntrinsicZYXRotation(angle)*new Vector3(0,0,distance);

		}
		public Vector3 Transform(Vector3 position) {
			return Matrix3.ExtrinsicZYXRotation(this.angle)*(position - this.position);
		}
	}
	class SystemWindow : Gtk.Window {
		protected SystemView sys_view;
		public Camera camera { 
			get {
				return sys_view.camera;
			} set {
				sys_view.camera = value;
			}
		}
		public double radius_multiplier { 
			get {
				return sys_view.radius_multiplier;
			} set {
				sys_view.radius_multiplier = value;
			}
		}
		public double bounds_multiplier { 
			get {
				return sys_view.bounds_multiplier;
			} set {
				sys_view.bounds_multiplier = value;
			}
		}
		public SystemWindow(String s, ObjectSystem sys) : base(s) {
			sys_view = new SystemView(sys);
            this.Add(sys_view);
            this.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask;
            this.DeleteEvent += delegate { Application.Quit (); };
            this.KeyPressEvent += Program.Input.KeyPress;
            this.MotionNotifyEvent += Program.Input.MouseMovement;
            this.ScrollEvent += Program.Input.Scroll;
			this.ShowAll();
		}
		public void Play(int interval) { sys_view.Play(interval); }
		public void PlayAsync(int interval) { sys_view.PlayAsync(interval); }
		public void Stop() { sys_view.Stop(); }
	}
	class SystemView : DrawingArea {
		
		public Camera camera {get; set;} = new Camera(100,Vector3.zero);
		public double radius_multiplier {get; set;} = 1;
		public double bounds_multiplier {get; set;} = 0.25;
		protected ObjectSystem sys;
		protected bool playing = false;
		protected double max = 0;
		protected List<int> order;
		public SystemView(ObjectSystem sys) {
			this.sys = sys;
			order = new List<int>();
			for (int i = 0; i < sys.Count; i++) { order.Add(i); }
			Redraw();
		}
		public void Redraw() {
			max = 0;
			foreach (SimObject b in sys) {
				var p = Vector3.Magnitude(camera.Transform(b.position));
				if (p > max) {
					max = p;
				}
			} 
		}
		public void Play(int interval) {
			playing = true;
			while (playing) {
				this.QueueDraw();
				Thread.Sleep(interval);
			}
		}
		public void PlayAsync(int interval) {
			Task.Run(() => Play(interval));
		}
		public void Stop() {
			playing = false;
		}
		protected override bool OnDrawn (Cairo.Context ctx) {
			// color the screen black
			ctx.SetSourceRGB(0,0,0);
			ctx.Paint();
			// Normally (0,0) is in the corner, but we want it in the middle, so we must translate:
			ctx.Translate(AllocatedWidth/2,AllocatedHeight/2);
			var bounds = bounds_multiplier * max * new Vector3(1,1,1);
			var scale = Math.Min(AllocatedWidth/bounds.x,AllocatedHeight/bounds.y);
			ctx.Scale(scale,scale);
			var origin = this.sys.origin;
			// draw closest objects last
			order = order.OrderByDescending(x => Vector3.Magnitude(sys[x].position - camera.position)).ToList();
			foreach (int i in order) {
				sys[i].Draw(ctx, origin, camera);
			}
			return true;
		}
	}
}