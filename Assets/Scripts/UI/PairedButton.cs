using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PairedButton : Button {

    public PairedButton Other;

    public void setOther( PairedButton o )  {
        Other = o;
        o.Other = this;
    }

    public void baseOnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
    }
    public void baseOnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if(Other != null) Other.baseOnPointerEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if(Other != null) Other.baseOnPointerExit(eventData);
    }

}
