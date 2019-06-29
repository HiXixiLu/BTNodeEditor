using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParallelNode : BaseNode
{
    private string label = "Parallel";

    // 节点不同状态下的特性
    //public GUIStyle style;
    //private GUIStyle defaultNodeStyle;
    //private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
    private Texture2D defaultBoxImage;
    private Texture2D selectedBoxImage;
    private Texture2D backgroundImage;


    public override void InitiateStyle()
    {
        //defaultNodeStyle = new GUIStyle();
        //defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        //defaultNodeStyle.border = new RectOffset(MyDefined.BorderOffset, MyDefined.BorderOffset, MyDefined.BorderOffset, MyDefined.BorderOffset);
        //defaultNodeStyle.fontStyle = FontStyle.Bold;



        //style = defaultNodeStyle;

        //selectedNodeStyle = new GUIStyle();
        //selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        //selectedNodeStyle.border = new RectOffset(MyDefined.BorderOffset, MyDefined.BorderOffset, MyDefined.BorderOffset, MyDefined.BorderOffset);
        //selectedNodeStyle.fontStyle = FontStyle.Bold;

        defaultBoxImage = MyEditorUtility.LoadTextureByIO(MyDefined.BtnControlFlowPath, MyDefined.controlNodeWidth, MyDefined.controlNodeHeight);
        selectedBoxImage = MyEditorUtility.LoadTextureByIO(MyDefined.BtnControlFlowOnPath, MyDefined.controlNodeWidth, MyDefined.controlNodeHeight);
        backgroundImage = defaultBoxImage;

    }

    public ParallelNode(Vector2 position)
    {
        InitiateStyle();
        rect = new Rect(position.x, position.y, MyDefined.controlNodeWidth, MyDefined.controlNodeHeight);

        children = new List<BaseNode>();
        nodeType = NodeType.Parallel;
    }
    // 重载的 RootNode 构造器 —— 加入了in out回调,加入删除回调
    public ParallelNode(Vector2 position, OnClickAsInPoint OnClickInPoint, OnClickAsOutPoint OnClickOutPoint, Action<BaseNode> OnClickRemoveNode)
    {
        InitiateStyle();
        this.rect = new Rect(position.x, position.y, MyDefined.controlNodeWidth, MyDefined.controlNodeHeight);
        this.OnRemoveNode = OnClickRemoveNode;
        this.inPointClicked = OnClickInPoint;
        this.outPointClicked = OnClickOutPoint;

        children = new List<BaseNode>();
        nodeType = NodeType.Parallel;
    }

    public override void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public override void Draw()
    {
        //GUI.Box(rect, label, style);
        GUI.BeginGroup(rect);
        GUI.DrawTexture(MyDefined.controlFlowRect, backgroundImage, ScaleMode.StretchToFill);
        GUI.Label(new Rect(10, 10, 60, 20), label);
        GUI.EndGroup();
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
                    e.Use();    // TODO: 啥意思，防止事件冒泡？
                    return true;
                }
                break;
        }
        return false;
    }
}
