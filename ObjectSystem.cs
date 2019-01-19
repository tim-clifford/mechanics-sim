using System;
using Structures;
using System.Collections;
using System.Collections.Generic;
namespace SimObjects {
sealed class ObjectSystem : IEnumerable<SimObject> {
    private List<SimObject> objects;
    public Vector3 origin {get; set;} = Vector3.zero;
    public IEnumerator<SimObject> GetEnumerator() {
        return (IEnumerator<SimObject>)objects.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
        return (IEnumerator)objects.GetEnumerator();
    }
    public SimObject this[int i] {
        get {
            return objects[i];
        } set {
            objects[i] = value;
        }
    }
    public int Count {
        get {
            return this.objects.Count;
        }
    }
    public ObjectSystem(List<SimObject> objects) {
        this.objects = objects;
    }
    public void TimeStep(double timestep) {
        for (int i = 0; i < objects.Count; i++) {
            var obj = objects[i];
            for (int j = i+1; j < objects.Count; j++) {
                var obj2 = objects[j];
                if (SimObject.Collided(obj,obj2)) {
                    SimObject.Collide(obj,obj2);
                    //soundPlayer.Play();
                    // Log collisions in the test class
                    Tests.i++;
                }
            }
        }
        foreach (SimObject obj in objects) {
            obj.position += timestep * obj.velocity;
        }
    }
}
}