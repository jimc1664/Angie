using System.Collections.Generic;
using UnityEngine;

/*
Quadtree by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
//Any object that you insert into the tree must implement this interface
public interface IQuadTreeObject {
    Vector2 GetPosition();
    float Rad { get; }
}
public class QuadTree<T> where T : IQuadTreeObject {
    private int MaxObjectCount {
        get {
            return 
                StoredObjects.Capacity;
        }
    }
    private List<T> StoredObjects;
    private Rect Bounds;
    private Vector2 Div;
    private QuadTree<T> CellA, CellB, CellC, CellD;

    public QuadTree<T> cell(int i) {
        switch(i) {
            case 0: return CellA;
            case 1: return CellB;
            case 2: return CellC;
            case 3: return CellD;
        }
        Debug.LogError("err");
        return null;
    }
    public QuadTree(int maxSize) {
        Bounds.max = Bounds.min = new Vector2(float.MinValue, float.MinValue );
        // Div = d;
        // m_maxObjectCount = maxSize;
        //cells = new QuadTree<T>[4];
       // maxSize = 9999;
        StoredObjects = new List<T>(maxSize);
    }
    public void Insert(T objectToInsert) {

        if(CellA != null) {
            int iCell = GetCellToInsertObject(objectToInsert.GetPosition());
           // if(iCell > -1) {
                cell(iCell).Insert(objectToInsert);
            //}
            Bounds = Bounds.extended(objectToInsert.GetPosition());
            return;
        }
        //
        //Objects exceed the maximum count
        if(StoredObjects.Count + 1 > MaxObjectCount) {
            //Split the quad into 4 sections
            Debug.Assert(CellA == null);
            //if(cells[0] == null) {
            float subWidth = (Bounds.width / 2f);
            float subHeight = (Bounds.height / 2f);
            float x = Bounds.x;
            float y = Bounds.y;
            Div = Bounds.center;
            CellA = new QuadTree<T>(MaxObjectCount);//, new Rect(x + subWidth, y, subWidth, subHeight));
            CellB = new QuadTree<T>(MaxObjectCount);//, new Rect(x, y, subWidth, subHeight));
            CellC = new QuadTree<T>(MaxObjectCount);//, new Rect(x, y + subHeight, subWidth, subHeight));
            CellD = new QuadTree<T>(MaxObjectCount);//, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
            //}
            //Reallocate this quads objects into its children
            //int i = m_storedObjects.Count - 1; ;
            for(int i = StoredObjects.Count; ;) {
                //T storedObj = m_storedObjects[i];
                int iCell = GetCellToInsertObject(objectToInsert.GetPosition());
               // if(iCell > -1) {
                    cell(iCell).Insert(objectToInsert);
               // }
                //m_storedObjects.RemoveAt(i);
                // i--;
                if(i-- <= 0) break;
                objectToInsert = StoredObjects[i];
            }
            /*Debug.Log(" div   " + Div + "    bounds   " + Bounds + "    StoredObjects.count   " + StoredObjects.Count);
            for(int i = 0; i <4; i++) {
                Debug.Log("  c" + i + "   " + cell(i).StoredObjects.Count + "    b  " + cell(i).Bounds);               
            }*/
            StoredObjects = null;

            Bounds = Bounds.extended(objectToInsert.GetPosition());
        } else {
            if(StoredObjects.Count == 0) {
                Bounds.max = Bounds.min = objectToInsert.GetPosition();
            } else
                Bounds = Bounds.extended(objectToInsert.GetPosition());
            StoredObjects.Add(objectToInsert);
            
        }
    }
    /*
    public void Remove(T objectToRemove) {
        if(ContainsLocation(objectToRemove.GetPosition())) {
            StoredObjects.Remove(objectToRemove);
            if(cells[0] != null) {
                for(int i = 0; i < 4; i++) {
                    cells[i].Remove(objectToRemove);
                }
            }
        }
    } */
    /*
    public List<T> RetrieveObjectsInArea(Rect area) {
        if(rectOverlap(m_bounds, area)) {
            List<T> returnedObjects = new List<T>();
            for(int i = 0; i < m_storedObjects.Count; i++) {
                if(area.Contains(m_storedObjects[i].GetPosition())) {
                    returnedObjects.Add(m_storedObjects[i]);
                }
            }
            if(cells[0] != null) {
                for(int i = 0; i < 4; i++) {
                    List<T> cellObjects = cells[i].RetrieveObjectsInArea(area);
                    if(cellObjects != null) {
                        returnedObjects.AddRange(cellObjects);
                    }
                }
            }
            return returnedObjects;
        }
        return null;
    } */

    bool intersects( Vector2 cp, float cr, Rect rect) {
        Vector2 circleDistance = (cp - rect.center).abs();
      //  circleDistance.y = abs(circle.y - rect.y);

        if(circleDistance.x > (rect.width / 2 + cr)) { return false; }
        if(circleDistance.y > (rect.height / 2 + cr)) { return false; }

        if(circleDistance.x <= (rect.width / 2)) { return true; }
        if(circleDistance.y <= (rect.height / 2)) { return true; }

        var cornerDistance_sq = Math_JC.pow2(circleDistance.x - rect.width / 2) +
                             Math_JC.pow2(circleDistance.y - rect.height / 2);

        return (cornerDistance_sq <= (cr*cr));
    }
    bool contains(Vector2 cp, float cr, Vector2 p) {
        return (cp - p).sqrMagnitude < cr * cr;
    }
    public void RetrieveObjectsInArea(Vector2 p, float r, List<T> ret) {

        if(intersects(p, r, Bounds)) {
            if(CellA != null) {
                for(int i = 0; i < 4; i++) {
                    cell(i).RetrieveObjectsInArea(p, r, ret);
                }
            } else for(int i = 0; i < StoredObjects.Count; i++) {
                    if(contains(p, r, StoredObjects[i].GetPosition())) {
                        ret.Add(StoredObjects[i]);
                    }
                }
        }
    }

    public List<T> RetrieveObjectsInArea(Vector2 p, float r ) {
        List<T> ret = new List<T>();
        RetrieveObjectsInArea(p, r, ret);
        return ret;
    }

    // Clear quadtree
    public void Clear() {
        StoredObjects.Clear();

        if(CellA == null) return;
        for(int i = 0; i < 4; i++) 
            cell(i).Clear();

        CellA = CellB = CellC = CellD = null;
    }
    public bool ContainsLocation(Vector2 location) {
        return Bounds.Contains(location);
    }
    private int GetCellToInsertObject(Vector2 location) {
        int r = 0;
        if(location.x > Div.x) r += 1;
        if(location.y > Div.y) r += 2;
        return r;
    }
    /*
   bool valueInRange(float value, float min, float max) { return (value >= min) && (value <= max); }


   bool rectOverlap(Rect A, Rect B) {
       bool xOverlap = valueInRange(A.x, B.x, B.x + B.width) ||
                       valueInRange(B.x, A.x, A.x + A.width);

       bool yOverlap = valueInRange(A.y, B.y, B.y + B.height) ||
                       valueInRange(B.y, A.y, A.y + A.height);

       return xOverlap && yOverlap;
   } */
    /*
    public void DrawDebug() {
        Gizmos.DrawLine(new Vector3(m_bounds.x, 0, m_bounds.y), new Vector3(m_bounds.x, 0, m_bounds.y + m_bounds.height));
        Gizmos.DrawLine(new Vector3(m_bounds.x, 0, m_bounds.y), new Vector3(m_bounds.x + m_bounds.width, 0, m_bounds.y));
        Gizmos.DrawLine(new Vector3(m_bounds.x + m_bounds.width, 0, m_bounds.y), new Vector3(m_bounds.x + m_bounds.width, 0, m_bounds.y + m_bounds.height));
        Gizmos.DrawLine(new Vector3(m_bounds.x, 0, m_bounds.y + m_bounds.height), new Vector3(m_bounds.x + m_bounds.width, 0, m_bounds.y + m_bounds.height));
        if(cells[0] != null) {
            for(int i = 0; i < cells.Length; i++) {
                if(cells[i] != null) {
                    cells[i].DrawDebug();
                }
            }
        }
    }*/
}