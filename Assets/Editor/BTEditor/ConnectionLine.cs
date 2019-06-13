using System;
using UnityEngine;
using UnityEditor;

public class ConnectionLine
{
    public BaseNode inPoint;
    public BaseNode outPoint;
    public Action<ConnectionLine> OnRemoveConnection;

    public ConnectionLine(BaseNode inPoint, BaseNode outPoint, Action<ConnectionLine> OnClickRemoveConnection) {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw() {
        // Handles类，3D GUI 绘图接口
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center,
            outPoint.rect.center,
            Color.red,
            null,
            MyDefined.lineWidth
            );

        // 绘制一个 3D Button
        // TODO: 使用右键菜单或者 Delete键删除一个 ConnectionLine, 修正选中方式
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
        {
            if (OnRemoveConnection != null)
            {
                OnRemoveConnection(this);
            }
        }
    }


}
