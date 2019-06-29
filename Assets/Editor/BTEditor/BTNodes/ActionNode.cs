using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActionNode : BaseNode
{
    private string label = "Action";
    public GameObject obj;

    // 节点不同状态下的特性
    //public GUIStyle style;
    //private GUIStyle defaultNodeStyle;
    //private GUIStyle selectedNodeStyle;
    private Texture2D defaultBoxImage;
    private Texture2D selectedBoxImage;
    private Texture2D backgroundImage;

    public override void InitiateStyle()
    {
        //defaultNodeStyle = new GUIStyle();
        //defaultNodeStyle.normal.background = EditorUtility.LoadTextureByIO(MyDefined.BtnLeafPath, nodeWidth, nodeHeight); // 自定义的图片

        //this.style = defaultNodeStyle;

        //selectedNodeStyle = new GUIStyle();
        //selectedNodeStyle.normal.background = EditorUtility.LoadTextureByIO(MyDefined.BtnLeafOnPath, nodeWidth, nodeHeight);  // 自定义的图片

        defaultBoxImage = MyEditorUtility.LoadTextureByIO(MyDefined.BtnLeafPath, MyDefined.nodeWidth, MyDefined.nodeHeight);
        selectedBoxImage = MyEditorUtility.LoadTextureByIO(MyDefined.BtnLeafOnPath, MyDefined.nodeWidth, MyDefined.nodeHeight);
        backgroundImage = defaultBoxImage;
    }

    public ActionNode(Vector2 position, OnClickAsInPoint OnClickInPoint, OnClickAsOutPoint OnClickOutPoint, Action<BaseNode> OnClickRemoveNode) {
        InitiateStyle();
        rect = new Rect(position.x, position.y, MyDefined.nodeWidth, MyDefined.nodeHeight);
        this.OnRemoveNode = OnClickRemoveNode;
        this.inPointClicked = OnClickInPoint;
        this.outPointClicked = OnClickOutPoint;

        children = new List<BaseNode>();
        nodeType = NodeType.Action;
    }

    public override void Draw()
    {
        
        GUI.BeginGroup(rect);
        //GUI.Box(new Rect(0, 0, nodeWidth, nodeHeight), new GUIContent(label), style);
        GUI.DrawTexture(MyDefined.LeafRect, backgroundImage, ScaleMode.StretchToFill);
        GUI.Label(new Rect(10, 10, 80, 20), label);
        EditorGUI.ObjectField(new Rect(10, 60, 160, 16), obj, typeof(GameObject), true);    // EditorGUI 类下包含了各种 Unity开放的编辑组件
        GUI.EndGroup();
    }

    public override void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public override bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        //style = selectedNodeStyle;
                        backgroundImage = selectedBoxImage;
                        inPointClicked(this);
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        //style = defaultNodeStyle;
                        backgroundImage = defaultBoxImage;
                    }
                }
                if (e.button == 1 && rect.Contains(e.mousePosition) && isSelected)
                {
                    ProcessContextMenu();
                    e.Use();    // 防止事件继续传播
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }
}
