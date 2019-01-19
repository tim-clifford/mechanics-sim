using Structures;
using System;
namespace SimObjects {
    class Block : SimObject {
        public Vector3 dimensions {get; set;}
        public Vector3 orientation {get; set;} = Vector3.zero;
        public readonly double line_multiplier = 0.1;
        public double line_width {
            get { return line_multiplier*Math.Min(dimensions.z,Math.Min(dimensions.x,dimensions.y));}
        }
        public override double MomentOfInertia {
            get { return 1; }
        }
        public Vector3 ToInternalPosition(Vector3 v) {
            return Matrix3.Inverse(Matrix3.IntrinsicZYXRotation(orientation))*(v - this.position);
        }
        public Vector3 ToExternalPosition(Vector3 v) {
            return Matrix3.IntrinsicZYXRotation(orientation)*(v) + this.position;
        }
        protected override Vector3 Normal(Vector3 v) {
            var r = ToInternalPosition(v);
            var r_polar = Vector3.CartesianToPolar(r);
            bool yx; // true for x, false for y
            bool yxpos; // true in the positive direction, false in the negative
            bool z; // true for z, false for other
            bool zpos;
            if ((r_polar.z < Math.Atan(dimensions.y/dimensions.x) 
                && r_polar.z > -Math.Atan(dimensions.y/dimensions.x))
            ) {
                yx = true; yxpos = true;
            } else if (
                r_polar.z > Math.PI-Math.Atan(dimensions.y/dimensions.x) // always less than 180
                || r_polar.z < -Math.PI+Math.Atan(dimensions.y/dimensions.x) // always greater than -180
            ) {
                yx = true; yxpos = false;
            } else {
                yx = false;
                if (r_polar.z > 0) yxpos = true;
                else yxpos = false;
            }
            Vector3 r_plane;
            if (r.z > 0) {
                r_plane = (dimensions.z/(2*r.z))*r;
                zpos = true;
                
            } else {
                r_plane = (-dimensions.z/(2*r.z))*r;
                zpos = false;
            }
            if (r_plane.y <= dimensions.y/2 && r_plane.y >= -dimensions.y/2 
                && r_plane.x <= dimensions.x/2 && r_plane.x >= -dimensions.x/2) {
                z = true;
            } else {
                z = false;
            }
            if (z) {
                if (zpos) return new Vector3(0,0,1);
                else return new Vector3(0,0,-1);
            } else {
                if (yx) {
                    if (yxpos) return new Vector3(1,0,0);
                    else return new Vector3(-1,0,0); 
                } else {
                    if (yxpos) return new Vector3(0,1,0);
                    else return new Vector3(0,-1,0);
                }
            }
        }
        protected override bool IsCollided(Vector3 v) {
            var r = ToInternalPosition(v);
            if (Math.Abs(r.x) < dimensions.x/2
                && Math.Abs(r.y) < dimensions.y/2
                && Math.Abs(r.z) < dimensions.z/2
            ) {
                return true;
            } else return false;
        }
        protected override Vector3 Closest(Vector3 r) {
            if (IsCollided(r)) return r;
            else {
                var p = ToInternalPosition(r);
                var n = Normal(r);
                Vector3 c = Vector3.zero;
                if (p.x > 0) c += new Vector3(Math.Min(p.x,dimensions.x/2),0,0);
                else c += new Vector3(Math.Max(p.x,-dimensions.x/2),0,0);
                if (p.y > 0) c += new Vector3(0,Math.Min(p.y,dimensions.y/2),0);
                else c += new Vector3(0,Math.Max(p.y,-dimensions.y/2),0);
                if (p.z > 0) c += new Vector3(0,0,Math.Min(p.z,dimensions.z/2));
                else c += new Vector3(0,0,Math.Max(p.z,-dimensions.z/2));
                return ToExternalPosition(c);
                /*var n_inv = new Vector3(Math.Abs(Math.Abs(n.x)-1),Math.Abs(Math.Abs(n.y)-1),Math.Abs(Math.Abs(n.z)-1));
                var p_1 = new Vector3(n_inv.x*p.x,n_inv.y*p.y,n_inv.z*p.z);
                var n_dim = new Vector3(n.x*dimensions.x,n.y*dimensions.y,n.z*dimensions.z);
                if (Math.Abs(p_1.x) <= dimensions.x/2
                    && Math.Abs(p_1.y) <= dimensions.y/2
                    && Math.Abs(p_1.z) <= dimensions.z/2
                ) {
                    return ToExternalPosition(p_1 + n_dim/2);
                } else {
                    // closest will be on an edge or corner

                }*/

            }
        }
        public override void Draw(Cairo.Context ctx, Vector3 origin, Graphics.Camera camera) {
            Vector3[] vertices = new Vector3[] {
                ToExternalPosition(new Vector3((dimensions.x - line_width)/2,(dimensions.y - line_width)/2,(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3((dimensions.x - line_width)/2,(dimensions.y - line_width)/2,-(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3((dimensions.x - line_width)/2,-(dimensions.y - line_width)/2,(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3((dimensions.x - line_width)/2,-(dimensions.y - line_width)/2,-(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3(-(dimensions.x - line_width)/2,(dimensions.y - line_width)/2,(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3(-(dimensions.x - line_width)/2,(dimensions.y - line_width)/2,-(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3(-(dimensions.x - line_width)/2,-(dimensions.y - line_width)/2,(dimensions.z - line_width)/2)),
                ToExternalPosition(new Vector3(-(dimensions.x - line_width)/2,-(dimensions.y - line_width)/2,-(dimensions.z - line_width)/2)),
            };
            int[][] adjacency = new int[][] {
                new int[] {1,2,4},
                new int[] {3,5},
                new int[] {3,6},
                new int[] {7},
                new int[] {5,6},
                new int[] {7},
                new int[] {7}
            };
            var cl = this.color;
            ctx.SetSourceRGB (cl.x,cl.y,cl.z);
            ctx.LineWidth = this.line_width;
            for (int i = 0; i < 7; i++) {
                foreach (int j in adjacency[i]) {
                    var p1 = camera.Transform(vertices[i]) - camera.Transform(origin);
                    var p2 = camera.Transform(vertices[j]) - camera.Transform(origin);
                    ctx.MoveTo(p1.x,p1.y);
                    ctx.LineTo(p2.x,p2.y);
                    ctx.Stroke();
                }
            }
            
        }
    }
}