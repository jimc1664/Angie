using UnityEngine;
using System.Collections;

public class CmpCast : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnDrawGizmosSelected() {

        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        var ol = gameObject.layer;
        gameObject.layer = 26;
        int lm = 1 << 26;
        RaycastHit hit;
        Vector3 p = transform.position;
        Gizmos.color = Color.red;

        for(int maxa = 8, a = maxa; a-- > 0;) {
            float ang = a*360.0f  / (float)maxa ;
            Vector3 dir = new Vector3(Mathf.Sin(ang * Mathf.Deg2Rad), Mathf.Cos(ang * Mathf.Deg2Rad));
            var r = new Ray(p + dir * 30, -dir);
            if(Physics.SphereCast( r, 0.5f, out hit, float.MaxValue, lm)) {

                Gizmos.DrawWireSphere( r.GetPoint(hit.distance), 0.5f);
            }
        }

        gameObject.layer = ol;
       DestroyImmediate(mc);
    }

    
}
