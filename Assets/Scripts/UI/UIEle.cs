using UnityEngine;
using System.Collections;

public class UIEle : MonoBehaviour {

    public virtual void gotHighlight( UIMain um ) { }
    public virtual void keptHighlight(UIMain um) { }
    public virtual void lostHighlight(UIMain um) { }

    public virtual void highlight_Cast(UIMain um, RaycastHit hit ) { }

    //   public virtual void lClick(InputMan um) { }

    /*
void OnEnable() {
    InputMan.Ins.addEle(gameObject.layer);
}
void OnDisable() {
    InputMan.Ins.remEle(gameObject.layer);
}*/

    public class Draggable : UIEle {

        public override void keptHighlight(UIMain um) {
            if(um.LockHighlight == this ) {
                drag(um);
                if(!um.MouseB[0]) {
                    um.LockHighlight = null;
                    um.MouseUp[0] = false;
                }
            } else if( um.grabMouseDown(0) ) {
                startDrag( um );
                um.LockHighlight = this;
                Debug.Assert(this == um.Highlight);
            }
        }
        public override void gotHighlight(UIMain um) { keptHighlight(um); }
        public virtual void drag(UIMain um) { }
        public virtual void startDrag(UIMain um) { }
    }
}
