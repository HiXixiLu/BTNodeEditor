using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;


public class BTEditorRightPanel : EditorWindow
{
    [MenuItem("Window/UIElements/BTEditorRightPanel")]
    public static void ShowExample()
    {
        BTEditorRightPanel wnd = GetWindow<BTEditorRightPanel>();
        wnd.titleContent = new GUIContent("BTEditorRightPanel");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = this.GetRootVisualContainer();

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath("Assets/Editor/BTEditor/rightPanel/BTEditorRightPanel.uxml", typeof(VisualTreeAsset)) as VisualTreeAsset;
        VisualElement labelFromUXML = visualTree.CloneTree(null);
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.AddStyleSheetPath("Assets/Editor/BTEditor/rightPanel/BTEditorRightPanel_style.uss");
        root.Add(labelWithStyle);
    }
}