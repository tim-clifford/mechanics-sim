using System;
using System.Linq;
using Gtk;
using Cairo;
using Structures;
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
	class SystemView : DrawingArea {
		
		public Camera camera {get; set;} = new Camera(100,Vector3.zero);
		public double radius_multiplier {get; set;} = 1;
		public int line_max {get; set;} = 100;
		public double bounds_multiplier {get; set;} = 0.25;
		protected ObjectSystem sys;
		protected bool playing = false;
		protected int[] order;
		protected double max = 0;
		public SystemView(ObjectSystem sys) {
			this.sys = sys;
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
			// we care about the limiting factor, since most orbits will be bounded roughly by a square
			// but screens are rectangular
			var scale = Math.Min(AllocatedWidth/bounds.x,AllocatedHeight/bounds.y);
			ctx.Scale(scale,scale);
			var origin = this.sys.origin;
			foreach (SimObject obj in sys) {
				obj.Draw(ctx, origin, camera);
			}
			return true;
		}
	}
}