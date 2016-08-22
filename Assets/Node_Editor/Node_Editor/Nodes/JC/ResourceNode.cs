﻿using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;


public class ResourceType : IConnectionTypeDeclaration {

    enum ResE {
        Energy, 
        Minerals,
    };
    static public string Ident { get { return "Resource"; } }

    public string Identifier { get { return Ident; } }
    public System.Type Type { get { return typeof(ResE); } }
    public Color Color { get { return Color.yellow; } }
    public string InKnobTex { get { return "Textures/In_Knob.png"; } }
    public string OutKnobTex { get { return "Textures/Out_Knob.png"; } }
}


[Node(false, "JC/Resource Node")]
public class ResourceNode : Node {
    public const string ID = "ResourceNode";
    public override string GetID { get { return ID; } }


    public override bool AllowRecursion { get { return true; } }


    public override Node Create(Vector2 pos) {
        var node = CreateInstance<ResourceNode>();

        node.rect = new Rect(pos.x, pos.y, 200, 60);
        node.name = "Resource Node";

        //node.CreateInput("Value", "Float");
        node.CreateOutput("Output val", ResourceType.Ident );

        node.regen();

        return node;
    }

    void regen() {
        bool hasEmpty = false;
        for(int i = Inputs.Count; i-- > 0;) {
            var a = Inputs[i];

            if(a.connection == null) {
                if(hasEmpty) {
                    Inputs.RemoveAt(i);
                    nodeKnobs.Remove(a);
                    DestroyImmediate(a, true);
                } else {
                    hasEmpty = true;
                    if(Inputs.Count - 1 != i) {
                        Inputs.RemoveAt(i);
                        Inputs.Add(a);
                    }
                }
            }
        }
        if(!hasEmpty) {
            CreateInput("Value", ResourceType.Ident );
        }
    }

    protected internal override void OnAddInputConnection(NodeInput input) {
        regen();
    }


    protected internal override void NodeGUI() {

        float yMax = 0;


        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        foreach(var i in Inputs)
            i.DisplayLayout();

        yMax = Mathf.Max(yMax, GUILayoutUtility.GetLastRect().yMax);

        GUILayout.EndVertical();
        GUILayout.BeginVertical();

        Outputs[0].DisplayLayout();

        yMax = Mathf.Max(yMax, GUILayoutUtility.GetLastRect().yMax);

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        yMax += 2;
        if(Event.current.type != EventType.Repaint)
            return;
        if(rect.size.y != yMax) {
            rect.yMax = rect.yMin + contentOffset.y + yMax;
            GUI.changed = true;
        }

    }


    public override bool Calculate() {

        if(!allInputsReady())
            return false;

        Outputs[0].SetValue<float>(Inputs[0].GetValue<float>() * 5);
        return true;
    }
}
