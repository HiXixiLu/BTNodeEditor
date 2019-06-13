using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
using UnityEditorInternal;


public class BTEditorMainWindow : EditorWindow
{
    [MenuItem("工具/BTEditor_UXML")]
    public static void ShowBTEditorMainWindow()
    {
        BTEditorMainWindow wnd = GetWindow<BTEditorMainWindow>();
        wnd.titleContent = new GUIContent("BTEditor");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = this.GetRootVisualContainer();

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath("Assets/Editor/BTEditor/BTEditorMainWindow.uxml", typeof(VisualTreeAsset)) as VisualTreeAsset;
        VisualElement elementsFromUXML = visualTree.CloneTree(null);
        root.Add(elementsFromUXML);
        //RegisterButtonsCallback(elementsFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        root.AddStyleSheetPath("Assets/Editor/BTEditor/BTEditorMainWindow_style.uss");
    }

    private void OnGUI()
    {

        if (GUILayout.Button("导入行为树")) {
            ButtonImportCallback();
        }
        if (GUILayout.Button("导出行为树"))
        {

        }
        if (GUILayout.Button("行为树检查"))
        {

        }
    }

    void ButtonImportCallback() {
        Debug.Log("Import");
    }

    void OnInspectorUpdate()
    {
        Debug.Log("窗口面板的更新");       
        this.Repaint(); //这里开启窗口的重绘，不然窗口信息不会刷新
    }

   
}