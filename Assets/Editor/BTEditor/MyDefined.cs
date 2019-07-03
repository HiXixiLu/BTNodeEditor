using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyDefined
{
    public static int BorderOffset = 12;
    public static int controlNodeWidth = 80;
    public static int controlNodeHeight = 50;
    public static int nodeWidth = 180;
    public static int nodeHeight = 100;
    public static float ConnectionPointWidth = 10f;
    public static float ConnectionPointHeight = 20f;
    public static float lineWidth = 3f;

    public static float toolBarHeight = 30f;

    // 各种文件路径
    public static string BtnControlFlowPath = "Editor/Textures/ControlFlow.png";
    public static string BtnControlFlowOnPath = "Editor/Textures/ControlFlowOn.png";
    //public static string nodeSelectedBtnPath = "Editor/Textures/BtnSelected.png";
    //public static string actionNodeSelectedBtnPath = "Editor/Textures/ActionNodeSelected.png";
    public static string BtnLeafPath = "Editor/Textures/Leaf.png";
    public static string BtnLeafOnPath = "Editor/Textures/LeafOn.png";

    // 节点矩形
    public static Rect controlFlowRect = new Rect(0, 0, MyDefined.controlNodeWidth, MyDefined.controlNodeHeight);
    public static Rect LeafRect = new Rect(0, 0, nodeWidth, nodeHeight);
}
