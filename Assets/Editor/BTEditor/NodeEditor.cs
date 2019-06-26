using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static UnityEditor.GenericMenu;

/* TODO LIST:
 * 1. 画布拖拽的卡顿问题要解决
 * 2. GUI元素要能多选
 * 3. 编辑行为可倒退 —— 增加缓存栈
 * 4. 能不能在编辑过程中就动态组织好树结构？想清楚要不要重构？
 * 5. bug: 节点删除了，但绘制的连线还在
 */
public class NodeEditorWindow : EditorWindow
{
    private List<BaseNode> nodes;
    private List<ConnectionLine> connections;
    public BaseNode treeRoot;

    public BaseNode selectedInPoint;
    public BaseNode selectedOutPoint;

    private Vector2 dragVector; // 控制整张画布的拖动
    private Vector2 gridOffset = Vector2.zero;

    // 节点工厂
    private NodesFactory _nodesFactory; // TODO： 这个请改成单例谢谢

    // 侧边栏固定菜单
    Rect _menuBar;

    [MenuItem("工具/行为树编辑器")]
    private static void OpenEditorWindsow() {
        NodeEditorWindow window = GetWindow<NodeEditorWindow>();
        window.titleContent = new GUIContent("BTs Nodes Editor");
    }

    private void OnEnable()
    {
        _nodesFactory = new NodesFactory().Instance;        
    }

    private void OnGUI()
    {
        // 绘制纵横网格
        DrawGrid(20, 0.2f, Color.gray, Vector2.zero);
        DrawGrid(100, 0.4f, Color.gray, Vector2.zero);

        DrawMenuBar();  // 绘制工具侧边栏

        DrawConnections();  //线条画在最下层
        DrawUnlinkedConnectionLine(Event.current);   //从outPoint到鼠标点的连线
        DrawNodes();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawNodes() {
        if (nodes != null)  // 使用列表来管理应绘制的节点
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }
    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }


    // 用于传递画布事件
    private void ProcessEvents(Event evt) {
        dragVector = Vector2.zero;
        switch (evt.type)
        {
            case EventType.MouseDown:
                if (evt.button == 1)
                {
                    ProcessContextMenu(evt.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (evt.button == 0) {  //TODO: 修改为组合键的监听/使网格绘制也可以随之拖动
                    OnDrag(evt.delta);
                }
                break;
        }
    }
    // 用于传递Node事件
    private void ProcessNodeEvents(Event e) {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)  //从后往前遍历，是因为最上的节点理应最先处理 —— 这里如果组织为树的话就是另外的遍历方式了
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    // 添加右键列表
    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();

        // 勘误，Root这种节点是不存在的，所谓的 Root 节点也只不过是 Control Flow节点中的一种
        //genericMenu.AddItem(new GUIContent("Add Root"), false, ()=> OnClickAddNode(mousePosition, BTNodes.Base));

        genericMenu.AddItem(new GUIContent("Add Fallback"), false, () => OnClickAddNode(mousePosition, BTNodes.Fallback));
        genericMenu.AddItem(new GUIContent("Add Sequence"), false, () => OnClickAddNode(mousePosition, BTNodes.Sequence));
        genericMenu.AddItem(new GUIContent("Add Parallel"), false, () => OnClickAddNode(mousePosition, BTNodes.Parallel));
        genericMenu.AddItem(new GUIContent("Add Decorator"), false, () => OnClickAddNode(mousePosition, BTNodes.Decorator));
        genericMenu.AddItem(new GUIContent("Add Selector"), false, () => OnClickAddNode(mousePosition, BTNodes.Selector));
        genericMenu.AddItem(new GUIContent("Add Action"), false, () => OnClickAddNode(mousePosition, BTNodes.Action));
        genericMenu.AddItem(new GUIContent("Add Condition"), false, () => OnClickAddNode(mousePosition, BTNodes.Condition));
        genericMenu.ShowAsContext();
    }

    // 右键列表的事件传递

    private void OnClickAddNode(Vector2 mousePosition, BTNodes nodeType)
    {
        if (nodes == null)
        {
            nodes = new List<BaseNode>();
        }
        Debug.Log(nodeType.ToString());
        nodes.Add(_nodesFactory.CreateTheNode(mousePosition, nodeType, OnClickAsInPoint, OnClickAsOutPoint, OnClickRemoveNode));
    }

    //private void OnClickInPoint(ConnectionPoint childNode)
    //{
    //    selectedInPoint = childNode;

    //    if (selectedOutPoint != null)
    //    {
    //        if (selectedOutPoint.node != selectedInPoint.node)
    //        {
    //            CreateConnection();
    //            ClearConnectionSelection();
    //        }
    //        else
    //        {
    //            ClearConnectionSelection();
    //        }
    //    }
    //}

    //private void OnClickOutPoint(ConnectionPoint parentNode)
    //{
    //    selectedOutPoint = parentNode;

    //    if (selectedInPoint != null)
    //    {
    //        if (selectedOutPoint.node != selectedInPoint.node)
    //        {
    //            CreateConnection();
    //            ClearConnectionSelection();
    //        }
    //        else
    //        {
    //            ClearConnectionSelection();
    //        }
    //    }
    //}

    // 删除一段连线
    private void OnClickRemoveConnection(ConnectionLine connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<ConnectionLine>();
        }

        connections.Add(new ConnectionLine(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
        // TODO: 删除连线
    }

    // 删除一个节点
    private void OnClickRemoveNode(BaseNode node) {
        if (connections != null)
        {
            List<ConnectionLine> connectionsToRemove = new List<ConnectionLine>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].childNode == node || connections[i].parentNode == node)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    // 画布拖动事件的回调
    private void OnDrag(Vector2 delta) {
        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);    //只要nodes动了，曲线也会重绘
            }
        }
    }

    // 从选中 parentNode 到鼠标位置的连线绘制
    private void DrawUnlinkedConnectionLine(Event e) {

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
            selectedOutPoint.rect.center,
            e.mousePosition,
            selectedOutPoint.rect.center,
            e.mousePosition,
            Color.red,
            null,
            MyDefined.lineWidth
            );

            GUI.changed = true;
        }
    }

    // 网格背景绘制
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, Vector2 dragVector)
    {
        if (dragVector == null) {
            dragVector = Vector2.zero;
        }
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);  // position: 父类 EditorWindow 的属性
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        gridOffset += dragVector * 0.5f;
        Vector3 newOffset = new Vector3(gridOffset.x % gridSpacing, gridOffset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
    // 侧边栏窗口 ToolBar绘制
    private void DrawMenuBar()
    {
        _menuBar = new Rect(0, 0, position.width, MyDefined.toolBarHeight);

        GUILayout.BeginArea(_menuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(80));
        GUILayout.Space(5);
        GUILayout.Button(new GUIContent("Load"), EditorStyles.toolbarButton, GUILayout.Width(80));
        GUILayout.Space(5);
        GUILayout.Button(new GUIContent("Check"), EditorStyles.toolbarButton, GUILayout.Width(80));
        GUILayout.Space(5);
        GUILayout.Button(new GUIContent("Advise"), EditorStyles.toolbarButton, GUILayout.Width(80));
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("TestBtn"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
            ReconstructBehaviorTree();
        };

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    //接收被点击的out节点
    public void OnClickAsOutPoint(BaseNode node) {
        this.selectedOutPoint = node;
    }

    //接收被点击的in节点
    public void OnClickAsInPoint(BaseNode node)
    {
        if (this.selectedOutPoint != null && this.selectedOutPoint != node)
        {
            this.selectedInPoint = node;
            CreateConnection();
            ClearConnectionSelection();
        }
        else {
            ClearConnectionSelection();
        }
        
    }

    void ReconstructBehaviorTree() {
        HashSet<BaseNode> nodeSet = new HashSet<BaseNode>();
        foreach(ConnectionLine cl in connections) {
            cl.parentNode.children.Add(cl.childNode);
            cl.childNode.parent = cl.parentNode;
            nodeSet.Add(cl.parentNode);
            nodeSet.Add(cl.childNode);
        }

        IEnumerator<BaseNode> itr = nodeSet.GetEnumerator();
        while (itr.MoveNext()) {
            itr.Current.WriteoutNodeType(); // test only
            if (itr.Current.parent == null) {
                treeRoot = itr.Current;
            }
        }
        Debug.Log("根节点： " + treeRoot.nodeType.ToString());

        // test only: Traverse the tree by level
        // 求你赶紧不择手段理解下多叉树及其相关高效的算法吧！！！！
        List<BaseNode> nodeQueue = new List<BaseNode>();
        nodeQueue.Add(treeRoot);
        //itr = nodeQueue.GetEnumerator();
        //while (itr.MoveNext()) {
        //    if (itr.Current.children.Count > 0) {
        //        foreach (BaseNode node in itr.Current.children) {
        //            nodeQueue.Add(node);
        //        }
        //    }
        //}
        //itr.Reset();
        //while (itr.Current != null)
        //{
        //    itr.Current.WriteoutNodeType();
        //    itr.MoveNext();
        //}
        for (int i = 0; i < nodeQueue.Count; i++) {
            if (nodeQueue[i].children.Count > 0) {
                foreach (BaseNode node in nodeQueue[i].children) {
                    nodeQueue.Add(node);
                }
            }
        }
        for (int i = 0; i < nodeQueue.Count; i++)
        {
            Debug.Log("层次遍历： index = " + i + " 节点：" + nodeQueue[i].nodeType.ToString());
        }
    }
}

    
