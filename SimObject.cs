using System;
using Structures;
namespace SimObjects {
    abstract class SimObject {
        public Vector3 color {get; set;} = new Vector3(1,1,1);
        // The position of the center of the object
        public Vector3 position {get; set;}
        public Vector3 velocity {get; set;}
        public double mass {get; set;} = 1;
        public double restitution {get; set;} = 1;
        public Vector3 Momentum {
            get {
                return mass*velocity;
            }
        }
        public double KineticEnergy {
            get {
                return Vector3.dot(velocity,velocity)*mass/2;
            }
        }
        public abstract double MomentOfInertia {get;}
        // returns the closest point within the object to p
        protected abstract Vector3 Closest(Vector3 p);
        // The unit normal to the surface at position vector r (for direction of impulse)
        protected abstract Vector3 Normal(Vector3 r);
        // returns whether p is inside the object
        protected abstract bool IsCollided(Vector3 p);
        protected virtual bool IsCollided(SimObject obj) {
            // if the closest point is inside the object, they have collided
            return obj.IsCollided(Closest(obj.position));
        }
        public static bool Collided(SimObject obj1, SimObject obj2) {
            // both objects must think they have collided, if not there is an error in the implementation
            if (obj1.IsCollided(obj2)) {
                if (obj2.IsCollided(obj1)) {
                    return true;
                } else {
                    Console.WriteLine("WARNING: Collision equality 1");
                    return true;
                }
            } else if (obj2.IsCollided(obj1)) {
                Console.WriteLine("WARNING: Collision equality 2");
                return true;
            } else {
                return false;
            }
        }
        public static void Collide(SimObject obj1, SimObject obj2) {
            
            Vector3 collisionPoint = (obj1.Closest(obj2.position) + obj2.Closest(obj1.position))/2;
            Vector3 normal = (obj1.Normal(collisionPoint) - obj2.Normal(collisionPoint))/2;
            double e = obj1.restitution*obj2.restitution;
            //transform to relative to obj2 for simplicity
            Vector3 u = obj1.velocity - obj2.velocity;
            double ucostheta = Vector3.dot(normal,u);
            // Done on paper, quadratic gives 2 solutions but only 1 is correct
            Vector3 v2_1 = (ucostheta*(1 + e)/(1 + obj2.mass/obj1.mass))*Vector3.Unit(u);
            Vector3 v2_2 = (ucostheta*(1 - e)/(1 + obj2.mass/obj1.mass))*Vector3.Unit(u);
            Vector3 v1_1 = (ucostheta - Vector3.Magnitude(v2_1)*obj2.mass/obj1.mass)*Vector3.Unit(u);
            Vector3 v1_2 = (ucostheta - Vector3.Magnitude(v2_2)*obj2.mass/obj1.mass)*Vector3.Unit(u);
            double check_1 = Vector3.dot(normal,v1_1 - v2_1);
            double check_2 = Vector3.dot(normal,v1_2 - v2_2);
            Vector3 v1;
            Vector3 v2;
            if (ucostheta < 0) {
                if (check_1 > 0) {
                    if (check_2 > 0) {
                        throw new Exception("Mechanics incorrect");
                    } else {
                        v1 = v1_1;
                        v2 = v2_1;
                    }
                } else if (check_2 > 0) {
                    v1 = v1_2;
                    v2 = v2_2;
                } else {
                    throw new Exception("Mechanics incorrect");
                }
            } else {
                if (check_1 < 0) {
                    if (check_2 < 0) {
                        throw new Exception("Mechanics incorrect");
                    } else {
                        v1 = v1_1;
                        v2 = v2_1;
                    }
                } else if (check_2 < 0) {
                    v1 = v1_2;
                    v2 = v2_2;
                } else {
                    throw new Exception("Mechanics incorrect");
                }
            }
            // transform back to the real world
            obj1.velocity = obj2.velocity + v1;
            obj2.velocity = obj2.velocity + v2;
            
        }
        public abstract void Draw(Cairo.Context ctx, Vector3 origin, Graphics.Camera camera);
    }
}