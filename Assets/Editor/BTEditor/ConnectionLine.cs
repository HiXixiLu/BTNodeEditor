using System;
using UnityEngine;
using UnityEditor;

public class ConnectionLine
{
    public BaseNode childNode;
    public BaseNode parentNode;
    public Action<ConnectionLine> OnRemoveConnection;

    public ConnectionLine(BaseNode childNode, BaseNode parentNode, Action<ConnectionLine> OnClickRemoveConnection) {
        this.childNode = childNode;
        this.parentNode = parentNode;
        this.OnRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw() {
        // Handles类，3D GUI 绘图接口
        Handles.DrawBezier(
            childNode.rect.center,
            parentNode.rect.center,
            childNode.rect.center,
            parentNode.rect.center,
            Color.white,
            null,
            MyDefined.lineWidth
            );

        // 绘制一个 3D Button
        // TODO: 使用右键菜单或者 Delete键删除一个 ConnectionLine, 修正选中方式
        if (Handles.Button((childNode.rect.center + parentNode.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
        {
            if (OnRemoveConnection != null)
            {
                OnRemoveConnection(this);
            }
        }
    }


}
