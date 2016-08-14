using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

/*
IPointerEnterHandler - OnPointerEnter - Called when a pointer enters the object
IPointerExitHandler - OnPointerExit - Called when a pointer exits the object
IPointerDownHandler - OnPointerDown - Called when a pointer is pressed on the object
IPointerUpHandler - OnPointerUp - Called when a pointer is released (called on the original the pressed object)
IPointerClickHandler - OnPointerClick - Called when a pointer is pressed and released on the same object
IInitializePotentialDragHandler - OnInitializePotentialDrag - Called when a drag target is found, can be used to initialise values
IBeginDragHandler - OnBeginDrag - Called on the drag object when dragging is about to begin
IDragHandler - OnDrag - Called on the drag object when a drag is happening
IEndDragHandler - OnEndDrag - Called on the drag object when a drag finishes
IDropHandler - OnDrop - Called on the object where a drag finishes
IScrollHandler - OnScroll - Called when a mouse wheel scrolls
IUpdateSelectedHandler - OnUpdateSelected - Called on the selected object each tick
ISelectHandler - OnSelect - Called when the object becomes the selected object
IDeselectHandler - OnDeselect - Called on the selected object becomes deselected
IMoveHandler - OnMove - Called when a move event occurs (left, right, up, down, ect)
ISubmitHandler - OnSubmit - Called when the submit button is pressed
ICancelHandler - OnCancel - Called when the cancel button is pressed*/

public class UIMain : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject Popup_Panel, Popup_Button, Popup_ButtonEx;

    public GameObject Icon_Frame, Icon_Ship, Icon_Station;

    public RectTransform SolSys;

    public GameObject UI_Hl = null;

    static UIMain _Singleton = null;
    public static UIMain Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<UIMain>();
            return _Singleton;
        }
        private set { }
    }

    void Awake() {
        if(_Singleton != null) Debug.LogError("UIMain:: Singleton violation");

        _Singleton = this;

        LMPos = MPos = Input.mousePosition;


    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        UI_Hl = eventData.pointerEnter;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        UI_Hl = null;
    }


    //public Vector2 MouseSensitivity = new Vector2(15f,15f);

    [HideInInspector]
    public Vector2 MPos;
    protected Vector2 LMPos;//, CMPos;

   // [HideInInspector]  public bool Return;
    public Vector2 MMove { get; private set; }
    public Vector2 RawMMove { get; private set; }
    public Vector3 MPoint { get; private set; }
    public Vector3 MNorm { get; private set; }
    public Ray MRay { get; private set; }

    public SimpleBitField MouseB, MouseUp, MouseDown; //private set

    public bool grabMouseDown(int mi) {
        if(!MouseDown[mi]) return false;
        MouseDown[mi] = false;
        return true;
    }
    public bool grabMouseUp(int mi) {
        if(!MouseUp[mi]) return false;
        MouseUp[mi] = false;
        return true;
    }

    public UIEle Highlight { get; private set; }
    [HideInInspector]
    public UIEle LockHighlight = null;

    /*
    void FixedUpdate() {
        if(!Screen.lockCursor) return;
        float mmx = Input.GetAxis("Mouse X"), mmy = Input.GetAxis("Mouse Y");
        mmx *= 1 + Mathf.Abs(mmx); mmy *= 1 + Mathf.Abs(mmy);

        MPos = new Vector2(MPos.x + mmx * MouseSensitivity.x, MPos.y + mmy * MouseSensitivity.y);
        if(MPos.x < 0) { CMPos.x += MPos.x; MPos.x = 0; } else if(MPos.x > Screen.width) { CMPos.x += MPos.x - Screen.width; MPos.x = Screen.width; }
        if(MPos.y < 0) { CMPos.y += MPos.y; MPos.y = 0; } else if(MPos.y > Screen.height) { CMPos.y += MPos.y - Screen.height; MPos.y = Screen.height; }

        //        MPos.x = Mathf.Clamp(MPos.x + mmx * MouseSensitivity.x, 0, Screen.width);
        //      MPos.y = Mathf.Clamp(MPos.y + mmy * MouseSensitivity.y, 0, Screen.height);
    }*/

  //  public delegate void MouseCB(ref Vector2 mp, Vector2 cmp);
   // [HideInInspector]  public MouseCB MCB = null;



    protected void Update() {
        MPos = Input.mousePosition;

        RawMMove = MPos - LMPos;
      //  if(MCB != null) MCB(ref MPos, CMPos);
        MMove = MPos - LMPos;
        LMPos = MPos; //CMPos = Vector2.zero;

       // UICamera.mMouse[0].pos = MPos;

        Vector3 mp3 = new Vector3(MPos.x, MPos.y, 0);

        MRay = Camera.main.ScreenPointToRay(mp3);

        for(int i = 3; i-- > 0;) {
            var old = MouseB[i];
            MouseB[i] = Input.GetMouseButton(i);
            MouseDown[i] = UI_Hl == null && Input.GetMouseButtonDown(i);
            MouseUp[i] = old && !MouseB[i];
        }

        int mi = 1;
       // Debug.Log("  b " + MouseB[mi] + "  d " + MouseDown[mi] + "  u " + MouseUp[mi]);

        if(LockHighlight == null ) {
            UIEle nHl = null;
            if(UI_Hl == null) {

                for(int i = 0; i < 2; i++) {
                    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if(Physics.Raycast(MRay, out hit, float.MaxValue, 1 << (30 + i))) {
                        nHl = hit.collider.GetComponent<UIEle>();

                        Debug.Assert(nHl != null);
                        if(nHl != null)
                            nHl.highlight_Cast(this, hit);
                        break;
                    }
                }
            }
            if(Highlight != nHl) {
                if(Highlight) Highlight.lostHighlight(this);
                if(nHl) nHl.gotHighlight(this);
                Highlight = nHl;
            } else if(Highlight)
                Highlight.keptHighlight(this);

        } else {
            LockHighlight.keptHighlight(this);
 
        }

        if(CtorMain.Singleton.isActiveAndEnabled ) //todo -- better
            CtorMain.Singleton.update(this);


    }
}