using System;
using System.Collections.Generic;
using static Program.Constants;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace Structures
{
	[Serializable()]
	public class Vector3 {
		// Simple 3-vector class, used for positions, velocities, color, etc.
		// setters are required for deserialization but should not be used outside class
		public double x {get; set;}
		public double y {get; set;}
		public double z {get; set;}
		public Vector3() {} // paramaterless constructor for serialization
		public Vector3(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		// Immutable standard vectors
		public static Vector3 zero {get;} = new Vector3(0,0,0);
		public static Vector3 i {get;} = new Vector3(1,0,0);
		public static Vector3 j {get;} = new Vector3(0,1,0);
		public static Vector3 k {get;} = new Vector3(0,0,1);
		public override String ToString() {
			return $"Vector3({x},{y},{z})";
		}
		public static bool operator== (Vector3 a, Vector3 b) {
			// why reimplement null checks ourselves?
			if ((object)a == null || ((object)b == null)) return (object)a == null && (object)b == null;
			// otherwise return true if all components are within 10^-10
			bool[] eq = new bool[3];
			for (int i = 0; i < 3; i++) {
				double a1,b1;
				if (i == 0) {a1 = a.x; b1 = b.x;}
				else if (i == 1) {a1 = a.y; b1 = b.y;}
				else {a1 = a.z; b1 = b.z;}
				if (Math.Abs(a1) < 1e-2 || Math.Abs(b1) < 1e-2) {
					eq[i] = Math.Abs(a1 - b1) < 1e-10;
				} else {
					eq[i] = Math.Abs((a1-b1)/a1) < 1e-10 
					     && Math.Abs((a1-b1)/b1) < 1e-10;
				}
			}
			return eq[0] && eq[1] && eq[2];
		}
		public static bool operator!= (Vector3 a, Vector3 b) {
			// inverse of equality operator
			return !(a == b);
		}
		public static Vector3 operator- (Vector3 a, Vector3 b) {
			return new Vector3 (a.x-b.x,a.y-b.y,a.z-b.z);
		}
		public static Vector3 operator- (Vector3 a) {
			return new Vector3(-a.x,-a.y,-a.z);
		}
		public static Vector3 operator+ (Vector3 a, Vector3 b) {
			return new Vector3 (a.x+b.x,a.y+b.y,a.z+b.z);
		}
		public static Vector3 operator* (double a, Vector3 b) {
			return new Vector3 (a*b.x,a*b.y,a*b.z);
		}
		public static Vector3 operator/ (Vector3 a, double b) {
			return new Vector3 (a.x/b,a.y/b,a.z/b);
		}
		public static double dot(Vector3 a, Vector3 b) {
			// This could be overloaded to operator*, but an explicit function increases readibility.
			return a.x*b.x + a.y*b.y + a.z*b.z; 
		}
		public static Vector3 cross(Vector3 a, Vector3 b) {
			return new Vector3(
				a.y*b.z - a.z*b.y,
				a.z*b.x - a.x*b.z,
				a.x*b.y - a.y*b.x
			);
		}
		public static double Magnitude(Vector3 v) {
			// Pythagorean Theorem
			return Math.Sqrt(Math.Pow(v.x,2)+Math.Pow(v.y,2)+Math.Pow(v.z,2));
		}
		public static Vector3 Unit(Vector3 v) {
			if (v == Vector3.zero) {
				throw new DivideByZeroException("Cannot take unit of zero vector");
			}
			return v / Vector3.Magnitude(v);
		}
		public static double UnitDot(Vector3 a, Vector3 b) {
			// The dot of the unit vectors
			return Vector3.dot(Vector3.Unit(a),Vector3.Unit(b));
		}
		public static Vector3 Log(Vector3 v, double b = Math.E) {
			// Polar logarithm (radius is logged, direction is consistent)
			var polar = CartesianToPolar(v);
			var log_polar = new Vector3 (Math.Log(polar.x,b),polar.y,polar.z);
			var log = PolarToCartesian(log_polar);
			return log;
		}
		public static Vector3 LogByComponent(Vector3 v, double b = Math.E) {
			// Cartesian Logarithm, all components are logged
			var r = new Vector3(0,0,0); 
			// using Vector3.zero will modify it, since we are inside the Vector class,
			// where Vector3.zero is mutable
			if (v.x < 0) r.x = -Math.Log(-v.x,b);
			else if (v.x != 0) r.x = Math.Log(v.x,b);
			if (v.y < 0) r.y = -Math.Log(-v.y,b);
			else if (v.y != 0) r.y = Math.Log(v.y,b);
			if (v.z < 0) r.z = -Math.Log(-v.z,b);
			else if (v.z != 0) r.z = Math.Log(v.z,b);
			return r;
		}
		public static Vector3 CartesianToPolar(Vector3 v) {
			// ISO Convention
			var r = Vector3.Magnitude(v);
			var theta = Math.Acos(Vector3.UnitDot(v,Vector3.k));
			var phi = Math.Acos(Vector3.UnitDot(new Vector3(v.x,v.y,0),Vector3.i));
			if (v.y < 0) phi = -phi;
			return new Vector3(r,theta,phi);
		}
		public static Vector3 PolarToCartesian(Vector3 v) {
			// ISO Convention
			return Matrix3.ZRotation(v.z) * Matrix3.YRotation(v.y) * (v.x*Vector3.k);
		}
		
	}
	public class Matrix3 {
		// the fields describe the rows. Using Vector3s makes Matrix-Vector Multiplication
		// (which is the most useful operation) simpler, since then Vector3.dot can be used
		public Vector3 x {get;}
		public Vector3 y {get;}
		public Vector3 z {get;}
		public Matrix3(Vector3 x, Vector3 y, Vector3 z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public override String ToString() {
			return $"Matrix3( {x.x} {x.y} {x.z}\n         {y.x} {y.y} {y.z}\n         {z.x} {z.y} {z.z} )";
		}
		public static Matrix3 XRotation(double x) {
			return new Matrix3 (
				new Vector3(1,0,0),
				new Vector3(0,Math.Cos(x),Math.Sin(x)),
				new Vector3(0,-Math.Sin(x),Math.Cos(x))
			);
		}
		public static Matrix3 YRotation(double y) {
			return new Matrix3 (
				new Vector3(Math.Cos(y),0,Math.Sin(y)),
				new Vector3(0,1,0),
				new Vector3(-Math.Sin(y),0,Math.Cos(y))
			);
		}
		public static Matrix3 ZRotation(double z) {
			return new Matrix3 (
				new Vector3(Math.Cos(z),-Math.Sin(z),0),
				new Vector3(Math.Sin(z),Math.Cos(z),0),
				new Vector3(0,0,1)
			);
		}
		public static Matrix3 ExtrinsicZYXRotation(double x, double y, double z) {
			return XRotation(x)*YRotation(y)*ZRotation(z);
		}
		public static Matrix3 ExtrinsicZYXRotation(Vector3 v) {
			return XRotation(v.x)*YRotation(v.y)*ZRotation(v.z);
		}
		public static Matrix3 IntrinsicZYXRotation(double x, double y, double z) {
			return ZRotation(z)*YRotation(y)*XRotation(x);
		}
		public static Matrix3 IntrinsicZYXRotation(Vector3 v) {
			return ZRotation(v.z)*YRotation(v.y)*XRotation(v.x);
		}
		public static bool operator== (Matrix3 a, Matrix3 b) {
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}
		public static bool operator!= (Matrix3 a, Matrix3 b) {
			return !(a == b);
		}

		public static Matrix3 operator+ (Matrix3 a, Matrix3 b) {
			// Add component-wise
			return new Matrix3(
				a.x + b.x,
				a.y + b.y,
				a.z + b.z
			);
		}
		public static Vector3 operator* (Matrix3 m, Vector3 v) {
			return new Vector3(
				Vector3.dot(m.x,v),
				Vector3.dot(m.y,v),
				Vector3.dot(m.z,v)
			);
		}
		public static Matrix3 operator* (double d, Matrix3 m) {
			// multiply each component by d
			return new Matrix3(
				d * m.x,
				d * m.y,
				d * m.z
			);
		}
		public static Matrix3 operator/ (Matrix3 m, double d) {
			if (d == 0) throw new DivideByZeroException("Matrix Division By Zero");
			else return (1/d) * m;
		}
		public static Matrix3 operator* (Matrix3 l, Matrix3 r) {
			var r_t = Matrix3.Transpose(r);
			return new Matrix3 (
				new Vector3(
					Vector3.dot(l.x,r_t.x),
					Vector3.dot(l.x,r_t.y),
					Vector3.dot(l.x,r_t.z)
				),
				new Vector3(
					Vector3.dot(l.y,r_t.x),
					Vector3.dot(l.y,r_t.y),
					Vector3.dot(l.y,r_t.z)
				),
				new Vector3(
					Vector3.dot(l.z,r_t.x),
					Vector3.dot(l.z,r_t.y),
					Vector3.dot(l.z,r_t.z)
				)
			);
		}
		public static double Determinant(Matrix3 m) {
			return m.x.x * (m.y.y*m.z.z - m.y.z*m.z.y)
			      -m.x.y * (m.y.x*m.z.z - m.y.z*m.z.x)
				  +m.x.z * (m.y.x*m.z.y - m.y.y*m.z.x);
		}
		public static Matrix3 Transpose(Matrix3 m) {
			return new Matrix3(
				new Vector3(m.x.x,m.y.x,m.z.x),
				new Vector3(m.x.y,m.y.y,m.z.y),
				new Vector3(m.x.z,m.y.z,m.z.z)
			);
		}
		public static Matrix3 TransposeCofactor(Matrix3 m) {
			// We never need to do the cofactor without the transpose, so this is an optimisation
			return new Matrix3(
				new Vector3(m.x.x,-m.y.x,m.z.x),
				new Vector3(-m.x.y,m.y.y,-m.z.y),
				new Vector3(m.x.z,-m.y.z,m.z.z)
			);
		}
		public static Matrix3 Minor(Matrix3 m) {
			return new Matrix3(
				new Vector3(
					(m.y.y*m.z.z - m.y.z*m.z.y),
					(m.y.x*m.z.z - m.y.z*m.z.x),
					(m.y.x*m.z.y - m.y.y*m.z.x)
				),
				new Vector3(
					(m.x.y*m.z.z - m.x.z*m.z.y),
					(m.x.x*m.z.z - m.x.z*m.z.x),
					(m.x.x*m.z.y - m.x.y*m.z.x)
				),
				new Vector3(
					(m.x.y*m.y.z - m.x.z*m.y.y),
					(m.x.x*m.y.z - m.x.z*m.y.x),
					(m.x.x*m.y.y - m.x.y*m.y.x)
				)
			);
		}
		public static Matrix3 Inverse(Matrix3 m) {
			if (Matrix3.Determinant(m) == 0) throw new DivideByZeroException("Singular Matrix");
			Matrix3 C_T = Matrix3.TransposeCofactor(Matrix3.Minor(m));
			return (1/Matrix3.Determinant(m)) * C_T;
		}
	}
}