using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;


//src -- http://answers.unity3d.com/questions/820311/ugui-multi-image-button-transition.html

public class MultiImageButton : UnityEngine.UI.Button {
    public List<Graphic> m_graphics;
    /* protected Graphic[] Graphics {
         get {
             if(m_graphics == null) {
                 m_graphics = targetGraphic.transform.parent.GetComponentsInChildren<Graphic>();
             }
             return m_graphics;
         }
     } */

    new void Awake() {
        base.Awake();
        m_graphics.Clear();
        m_graphics.Add(targetGraphic);
    }
    
   /* public bool isHighlighted {        
        get { return EventSystem.current.currentSelectedGameObject == gameObject;  }
    }*/
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