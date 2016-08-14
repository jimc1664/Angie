using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonRmb : Button {

    //public bool RightC = false;
    public ButtonClickedEvent onRClick;
    public new ButtonClickedEvent onClick;

    new void Awake() {
        base.Awake();
        if(onClick == null)
            onClick = base.onClick;
        base.onClick = null;

        m_graphics.Clear();
        m_graphics.Add(targetGraphic);
    }

    public override void OnPointerDown(PointerEventData eventData) {

        if(IsPressed()) return;

        if( eventData.button == PointerEventData.InputButton.Right) {
            eventData.button = PointerEventData.InputButton.Left;
            base.onClick = onRClick;
        } else
            base.onClick = onClick;

        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData) {

        if(eventData.button == PointerEventData.InputButton.Right) 
            eventData.button = PointerEventData.InputButton.Left;

          base.OnPointerUp(eventData);
    }
    public List<Graphic> m_graphics;
    /* protected Graphic[] Graphics {
         get {
             if(m_graphics == null) {
                 m_graphics = targetGraphic.transform.parent.GetComponentsInChildren<Graphic>();
             }
             return m_graphics;
         }
     } */


    public bool isHighlighted {        
         get { return currentSelectionState == SelectionState.Highlighted;  }
    }
    protected override void DoStateTransition(SelectionState state, bool instant) {
        Color color;
        switch(state) {
            case Selectable.SelectionState.Normal:
                color = this.colors.normalColor;
                break;
            case Selectable.SelectionState.Highlighted:
                color = this.colors.highlightedColor;
                break;
            case Selectable.SelectionState.Pressed:
                color = this.colors.pressedColor;
                break;
            case Selectable.SelectionState.Disabled:
                color = this.colors.disabledColor;
                break;
            default:
                color = Color.black;
                break;
        }
        if(base.gameObject.activeInHierarchy) {
            switch(this.transition) {
                case Selectable.Transition.ColorTint:
                    ColorTween(color * this.colors.colorMultiplier, instant);
                    break;
                default:
                    throw new System.NotSupportedException();
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant) {
        if(this.targetGraphic == null) {
            return;
        }

        foreach(Graphic g in m_graphics) {
            g.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
        }
    }
}
