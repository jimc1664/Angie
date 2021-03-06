﻿using UnityEngine;
using System.Collections.Generic;
using System;

/*
2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997
*/



public struct IVec3 {

    public IVec3(int a) {
        x = y = z = a;
    }
    public IVec3(int a, int b, int c) {
        x = a; y = b; z = c;
    }

    public IVec3(int[,] adj, int ai)  {
        x = adj[ai,0]; y = adj[ai, 1]; z = adj[ai, 2];
    }

    public int x, y, z;

    static public IVec3 operator +(IVec3 a, IVec3 b) {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;
        return a;
    }
    public bool allLower(IVec3 o) {
        return x < o.x && y < o.y && z < o.z;
    }
    public bool allGreaterOE(IVec3 o) {
        return x >= o.x && y >= o.y && z >= o.z;
    }
    public bool contained(IVec3 mn, IVec3 mx) {
        return (x >= mn.x && x < mx.x) && (y >= mn.y && y < mx.y) && (z >= mn.z && z < mx.z);
    }

    public static explicit operator Vector3(IVec3 a) {
        return new Vector3(a.x, a.y, a.z);
    }

    public static implicit operator string(IVec3 a) {
        return "X: " + a.x + "  Y: " + a.y + "  Z: " + a.z;
    }




}
public class DyArray<T> {

    public T get(IVec3 i) {
        var ai = i + C;
        validate(ref ai);
        return Data[i.x, i.y, i.z];
    }

    public T this[IVec3 i] {
        get {
            var ai = i + C;
            validate(ref ai);
            return Data[ai.x, ai.y, ai.z];
        }
        set {
            var ai = i + C;
            validate(ref ai);
            Data[ai.x, ai.y, ai.z] = value;
        }
    }

    public delegate void Foo(ref T t);
    public delegate bool FooB(ref T t);
    public void op(IVec3 i, Foo f) {
        var ai = i + C;
        validate(ref ai);
        f(ref Data[ai.x, ai.y, ai.z]);
    }
    public bool op(IVec3 i, FooB f) {
        var ai = i + C;
        validate(ref ai);
        return f(ref Data[ai.x, ai.y, ai.z]);
    }
    bool boundFix(ref int i, ref int c, ref int s) {
        if(i < 0) {
            c -= i;
            s -= i;
            i = 0;
            return true;
        } else if(i >= s) {
            s = i + 1;
            return true;
        }
        return false;
    }
    void validate(ref IVec3 i) {
        IVec3 oc = C, os = S;
        var oi = i;
        //Debug.Log("validate  " + i);
        if(boundFix(ref i.x, ref C.x, ref S.x)
            | boundFix(ref i.y, ref C.y, ref S.y)
            | boundFix(ref i.z, ref C.z, ref S.z)) {

            // Debug.Log(" fix  oi "+oi+ " -> i "+ i + " oc " + oc + "->C " + C + " os " + os + "->S " + S);
            var od = Data;
            Data = new T[S.x, S.y, S.z];
            for(int a = os.x; a-- > 0;)
                for(int b = os.y; b-- > 0;)
                    for(int c = os.z; c-- > 0;) {
                        //    Debug.Log("  cpy a " + a +" -> " + (a - oc.x + C.x) + " b " + b + " -> " + (b - oc.y + C.y) + " c " + c + " -> " + (c - oc.z + C.z));
                        Data[a - oc.x + C.x, b - oc.y + C.y, c - oc.z + C.z] = od[a, b, c];
                    }
        }
    }



    T[,,] Data;
    IVec3 C, S;
};
public struct Callback<T> {
    public T Data;
    public delegate void CB_T(T o);
    public CB_T CB;
    public void proc() { CB(Data); }
};

public class MonoBehaviourComparer<T> : IComparer<T> where T : MonoBehaviour {
    public int Compare(T a, T b) {
        return a.GetInstanceID().CompareTo(b.GetInstanceID());
    }
};

public class DuplicateKeyComparer<TKey>
                   : IComparer<TKey> where TKey : System.IComparable {
    public int Compare(TKey x, TKey y) {
        int result = x.CompareTo(y);
        if(result == 0)
            return 1;   // Handle equality as beeing greater
        else
            return result;
    }

}

[System.Serializable]
public struct SimpleBitField {
    public int Data;
    public bool this[int indx] {
        get {
            return (Data & (1 << indx)) != 0;
        }
        set {
            var mask = (1 << indx);
            if(value)
                Data |= mask;
            else
                Data &= ~mask;
        }
    }
};
public static class Math_JC {

    public static float Au_Km = 149597870.700f;
    public static float Km_Au = 1.0f / Au_Km;
    public static void resetTransformation(this Transform trans) {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }


    public static T getComponentInParentAct<T>(this Component t ) where T : Component {
        T ret = null;
        var p = t.transform.parent;
        if(p != null)
            return p.GetComponentInParent<T>();
        return ret;
    }

    public static float sqrMag(this Quaternion q) {
        return Quaternion.Dot(q, q);
        //return q.x * q.x + q.y + q.y + q.z * q.z + q.w + q.w;
    }
    public static Quaternion scaled(this Quaternion q, float s) {
        q.x *= s;
        q.y *= s;
        q.z *= s;
        q.w *= s;
        return q;
    }
    public static Quaternion negated(this Quaternion q) {
        q.x = -q.x;
        q.y = -q.y;
        q.z = -q.z;
        q.w = -q.w;
        return q;
    }


    //http://stackoverflow.com/a/12934750
    public static Quaternion normalised(this Quaternion q) {
       
        float qmagsq = q.sqrMag();
        if(Mathf.Abs(1.0f - qmagsq) < 2.107342e-08) {
            return q.scaled(2.0f / (1.0f + qmagsq));
        } else {
            return q.scaled(1.0f / Mathf.Sqrt(qmagsq));
        }
    }
    public static Vector2 abs(this Vector2 v) {

        return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }
    public static float pow2(float a ) { return a * a; }

    //create a vector of direction "vector" with length "size"
    public static Vector3 SetVectorLength(Vector3 vector, float size) {

        //normalize the vector
        Vector3 vectorNormalized = Vector3.Normalize(vector);

        //scale the vector
        return vectorNormalized *= size;
    }
    public static Vector3 abs(this Vector3 v) {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
    public static float maxAbsD(this Vector3 v) {
        var abs = v.abs();
        return Mathf.Max(abs.x, Math.Max(abs.y, abs.z ) );
    }

    public static Rect extended(this Rect r, Vector2 p) {
        r.min = Vector2.Min(r.min, p);
        r.max = Vector2.Max(r.max, p);
        return r;
    }

    public static Rect lerp( this Rect a, Rect b, float d ) {

        return new Rect(Vector2.LerpUnclamped(a.position, b.position, d), Vector2.LerpUnclamped(a.size, b.size, d) );
    }

    //Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
    //The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
    //the function outputs true, otherwise false.
    public static bool planePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position) {

        linePoint = Vector3.zero;
        lineVec = Vector3.zero;

        //We can get the direction of the line of intersection of the two planes by calculating the 
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet. We need a point for that to go with the line vector.
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.		
        Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

        float denominator = Vector3.Dot(plane1Normal, ldir);

        //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
        if(Mathf.Abs(denominator) > 0.006f) {

            Vector3 plane1ToPlane2 = plane1Position - plane2Position;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
            linePoint = plane2Position + t * ldir;

            return true;
        }

        //output not valid
        else {
            return false;
        }
    }
    //Get the intersection between a line and a plane. 
    //If the line and plane are not parallel, the function outputs true, otherwise false.
    public static bool linePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint) {

        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if(dotDenominator != 0.0f) {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            vector = SetVectorLength(lineVec, length);

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;

            return true;
        }

        //output not valid
        else {
            return false;
        }
    }

    public static void swap<T>(ref T a, ref T b) {
        T x = a; a = b; b = x;
    }



    /// <summary>
    /// Calculates the intersection line segment between 2 lines (not segments).
    /// Returns false if no solution can be found.
    /// </summary>
    /// http://paulbourke.net/geometry/pointlineplane/
    /// <returns></returns>
    public static bool calculateLineLineIntersection(Vector3 line1Point1, Vector3 line1Point2,
        Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2) {
        // Algorithm is ported from the C algorithm of 
        // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
        resultSegmentPoint1 = Vector3.zero;
        resultSegmentPoint2 = Vector3.zero;

        Vector3 p1 = line1Point1;
        Vector3 p2 = line1Point2;
        Vector3 p3 = line2Point1;
        Vector3 p4 = line2Point2;
        Vector3 p13 = p1 - p3;
        Vector3 p43 = p4 - p3;

        if(p43.sqrMagnitude < Mathf.Epsilon) {
            return false;
        }
        Vector3 p21 = p2 - p1;
        if(p21.sqrMagnitude < Mathf.Epsilon) {
            return false;
        }

        double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
        double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
        double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
        double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
        double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

        double denom = d2121 * d4343 - d4321 * d4321;
        if(Math.Abs(denom) < Mathf.Epsilon) {
            return false;
        }
        double numer = d1343 * d4321 - d1321 * d4343;

        double mua = numer / denom;
        double mub = (d1343 + d4321 * (mua)) / d4343;

        resultSegmentPoint1.x = (float)(p1.x + mua * p21.x);
        resultSegmentPoint1.y = (float)(p1.y + mua * p21.y);
        resultSegmentPoint1.z = (float)(p1.z + mua * p21.z);
        resultSegmentPoint2.x = (float)(p3.x + mub * p43.x);
        resultSegmentPoint2.y = (float)(p3.y + mub * p43.y);
        resultSegmentPoint2.z = (float)(p3.z + mub * p43.z);

        return true;
    }

}
