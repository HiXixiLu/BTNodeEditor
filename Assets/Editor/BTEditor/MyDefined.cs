using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyDefined
{
    public static int BorderOffset = 12;
    public static int controlNodeWidth = 80;
    public static int controlNodeHeight = 50;
    public static float ConnectionPointWidth = 10f;
    public static float ConnectionPointHeight = 20f;
    public static float lineWidth = 2f;

    public static float toolBarHeight = 30f;

    // 各种文件路径
    public static string nodeBtnPath = "Editor/Textures/BtnNormal.png";
    public static string nodeSelectedBtnPath = "Editor/Textures/BtnSelected.png";
    public static string actionNodeSelectedBtnPath = "Editor/Textures/ActionNodeSelected.png";
}

public enum BTNodes {
    Base,
    Action,
    Condition,
    Fallback,
    Parallel,
    Sequence,
    Decorator
}
