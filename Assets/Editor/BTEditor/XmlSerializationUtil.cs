using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;   // 基于文档结构模型的方式来读取 XML 文件
using System.IO;

/* 单例工具类: 由静态初始化实现
 */
public class XmlSerializationUtil
{
    static XmlDocument doc = new XmlDocument();
    private static XmlSerializationUtil _instance = new XmlSerializationUtil();
    static XmlSerializationUtil() { }
    public static XmlSerializationUtil Instance {
        get { return _instance; }
    }

    string xml_path = Application.dataPath; //TODO: 行为树保存路径可以选择
    int selectedLangIndex = 0;


    private void StartSerializationWithTreeRoot(ref BaseNode root)
    {
        string targetXmlFullPathName = xml_path + Path.DirectorySeparatorChar + "target-test.xml";


        Debug.Log("开始写入：");

        // 创建一个 encoding = UTF-8 的 writer
        FileStream targetFile = File.Create(targetXmlFullPathName);
        XmlTextWriter writer = new XmlTextWriter(targetFile, System.Text.Encoding.UTF8);
        writer.Formatting = Formatting.Indented; //设置缩进
        writer.WriteStartDocument();
        writer.WriteStartElement("BehaviorTree");

        // 开始遍历
        TraverseTheTreeInDepthFirstOrder(ref root, ref writer);

        writer.WriteEndElement();
        writer.Flush();
        writer.Close();
        targetFile.Close();

        Debug.Log("序列化完成");
    }

    private void TraverseTheTreeInDepthFirstOrder(ref BaseNode node, ref XmlTextWriter w) {
        if (node == null)
            return;

        w.WriteStartElement(node.nodeType.ToString());
        for (int i = 0; i < node.children.Count; i++) {
            BaseNode child = node.children[i];
            TraverseTheTreeInDepthFirstOrder(ref child, ref w);
        }
        w.WriteEndElement();
    }

    private BaseNode buildBehaviorTree(ref List<ConnectionLine> cls) {
        HashSet<BaseNode> nodeSet = new HashSet<BaseNode>();
        BaseNode treeNode = null;
        foreach (ConnectionLine cl in cls)
        {
            cl.parentNode.children.Add(cl.childNode);
            cl.childNode.parent = cl.parentNode;
            nodeSet.Add(cl.parentNode);
            nodeSet.Add(cl.childNode);
        }

        IEnumerator<BaseNode> itr = nodeSet.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.WriteoutNodeType(); // test only
            if (itr.Current.parent == null)
            {
                treeNode = itr.Current;
            }
        }
        Debug.Log("根节点： " + treeNode.nodeType.ToString());

        return treeNode;
    }

    public void startSerialization(ref List<ConnectionLine> cls) {
        BaseNode root = buildBehaviorTree(ref cls);
        StartSerializationWithTreeRoot(ref root);
    }

}
