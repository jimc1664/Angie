using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class StarField : MonoBehaviour {


    public int cubemapSize = 128;
    public bool oneFacePerFrame = false;
    public Camera cam  ;
	public RenderTexture rtex;
    //  public Renderer Dis;
    //  public Material Mat;
    public bool GenNebula = false;

    public int CloudCnt = 20, CountLow = 5;

    public GameObject Dust, Cloud;

    public float DustQn = 0.65f, RotRn = 0.1f, SclRnd = 0.1f, DustSclZMod = 0.7f, Disperse = 0.1f;
    public bool RandomRot = true;
    void Update() {
        if(!GenNebula) return;
        GenNebula = false;
        var bs = new GameObject("neb").transform;
        bs.transform.parent = transform;
        bs.resetTransformation();

        var disperse = (1.0f - Random.value * 0.5f) * Disperse;

        for(int i = Random.Range(CountLow, CloudCnt); i-- >0;) {

            bool dust = Random.value > DustQn;

            var t = Instantiate(dust ? Dust : Cloud).transform;
            t.parent = bs;
            t.resetTransformation();

            //t.localRotation = Quaternion.Slerp(Quaternion.identity, Random.rotation, RotRn);
            var up = Random.insideUnitCircle .normalized;
            var rotOff = Random.insideUnitSphere * RotRn;
            rotOff.x *= (1.0f + Random.value )* disperse;
            t.localRotation = Quaternion.LookRotation( up, Vector3.forward+ rotOff );

            var sclAdd = (Vector3.one * -0.5f + new Vector3(Random.value, Random.value, Random.value)) * (0.5f + Random.value) * SclRnd;
            if(dust) sclAdd.y *= DustSclZMod;
            t.localScale = Vector3.one + sclAdd;

        }

        if(RandomRot)
            bs.localRotation = Random.rotationUniform;
        else
            bs.localEulerAngles = new Vector3(90, 0, 0);

        var sclY = Random.Range(-1.0f, 1.0f) *1.3f;
        bs.localScale.Scale( new Vector3(1, sclY,1) );
        bs.position += bs.forward * ((2 + Random.value)*2 + 1 - sclY) * 75 ;

    }

    void LateUpdate() {
        if(oneFacePerFrame) {
            var faceToRender = Time.frameCount % 6;
            var faceMask = 1 << faceToRender;
            UpdateCubemap(faceMask);
        } else {
            UpdateCubemap(63); // all six faces
        }
    }

    void UpdateCubemap(int faceMask) {
        if(!cam) {
            cam = GetComponent< Camera > ();
        }

        if(!rtex) {
            rtex = new RenderTexture(cubemapSize, cubemapSize, 16);

            //rtex.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rtex.isCubemap = true;
            rtex.hideFlags = HideFlags.HideAndDontSave;
          //  rtex.hideFlags = HideFlags.DontSave;          
        }
        cam.RenderToCubemap(rtex, faceMask);
        RenderSettings.skybox.SetTexture("_Tex", rtex);

    }
    void OnEnable() {
        //  DestroyImmediate(cam);
        if( rtex) DestroyImmediate(rtex);
    }
    void OnDisable() {
      //  DestroyImmediate(cam);
      //  DestroyImmediate(rtex);
    }



}
