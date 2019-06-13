using System;
using UnityEngine;
using UnityEditor;

/*
 * 所绘制节点的基类
 * 由于该类并不继承或拓展 IMGUI 模块内的各种 Controls，因此判断其选中与否等，需要依靠状态位
*/
public abstract class BaseNode
{
    public Rect rect;
    public bool isDragged;  //拖动控制状态 
    public bool isSelected; //是否选中的控制状态位

    //节点上回调
    public Action<BaseNode> OnRemoveNode;
    public delegate void OnClickAsInPoint(BaseNode node);
    public delegate void OnClickAsOutPoint(BaseNode node);  //委托类型声明：返回值类型 void，委托类型 OnClickAsOutPoint，接受的参数类型 BaseNode
    public OnClickAsOutPoint outPointClicked;  //该委托类型的变量作为一个属性声明
    public OnClickAsInPoint inPointClicked;

    public virtual void InitiateStyle() { }

    // 基类仅仅管理 Connection的绘制
    public abstract void Draw();
    public abstract void Drag(Vector2 v);
    public abstract bool ProcessEvents(Event e);

    public virtual void ProcessContextMenu() {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        genericMenu.AddItem(new GUIContent("Make Connection"), false, OnClickMakeConnection);
        genericMenu.ShowAsContext();
    }

    public void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
    
    // 连接线条绘制回调
    public void OnClickMakeConnection() {
        if (outPointClicked != null) {
            outPointClicked(this);
        }
    }
}
