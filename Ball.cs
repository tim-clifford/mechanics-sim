using System;
using Structures;
namespace SimObjects {
    class Ball : SimObject {
        public double radius {get; set;} = 1;
        public override double MomentOfInertia {
            get {
                return 2*mass*Math.Pow(radius,2)/5;
            }
        }
        protected override Vector3 Normal(Vector3 p) {
            // normal vector is just the vector from the center to the surface
            return Vector3.Unit(p-this.position);
        }
        protected override bool IsCollided(Vector3 p) {
            if (Vector3.Magnitude(p-this.position) <= radius) return true;
            else return false;
        }
        protected override Vector3 Closest(Vector3 p) {
            if (IsCollided(p)) return p;
            else return radius*Vector3.Unit(p-this.position) + this.position;
        }
        public override void Draw(Cairo.Context ctx, Vector3 origin, Graphics.Camera camera) {
            // just draw a circle
            var pos = camera.Transform(this.position) - camera.Transform(origin);
		    var cl = this.color;
			ctx.SetSourceRGB (cl.x,cl.y,cl.z);
			ctx.Arc(pos.x,pos.y,this.radius,0,2*Math.PI);
			ctx.Fill();
        }
    }
}