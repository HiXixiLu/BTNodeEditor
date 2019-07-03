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
    static XmlSerializationUtil _instance = new XmlSerializationUtil();
    static XmlSerializationUtil() { }
    public static XmlSerializationUtil Instance {
        get { return _instance; }
    }

    int selectedLangIndex = 0;


    private void StartSerializationWithTreeRoot(ref BaseNode root, string filePath)
    {
        Debug.Log("开始写入：");

        // 创建一个 encoding = UTF-8 的 writer
        FileStream targetFile = File.Create(filePath);
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
        w.Flush();
    }

    // TODO: ball ball 你赶紧理解理解什么是 优雅的多叉树！
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

    public void startSerialization(ref List<ConnectionLine> cls, string filePath) {
        BaseNode root = buildBehaviorTree(ref cls);
        StartSerializationWithTreeRoot(ref root, filePath);
    }

    //public BaseNode DeserializeXmlToTree(string filePath)
    //{
    //    Debug.Log("文件读取：");
    //    BaseNode root;

    //    // 创建 reader
    //    XmlReaderSettings settings = new XmlReaderSettings();
    //    settings.IgnoreComments = true; //忽略文档里面的注释
    //    XmlReader reader = XmlReader.Create(filePath, settings);
    //    doc.Load(reader);

    //    XmlNode xns = doc.SelectSingleNode("BehaviorTree");
    //    XmlElement xe = (XmlElement)xns.FirstChild;   // 根节点
    //    Debug.Log(xe.Name);
        

    //    Debug.Log("反序列化完成");
    //}

    public NodeType IdentifyNodeType(string name) {
        switch (name) {
            case "Sequence":
                return NodeType.Sequence;
            case "Fallback":
                return NodeType.Fallback;
            case "Selector":
                return NodeType.Selector;
            case "Parallel":
                return NodeType.Parallel;
            case "Decorator":
                return NodeType.Decorator;
            case "Action":
                return NodeType.Action;
            case "Condition":
                return NodeType.Condition;
            default:
                return NodeType.AbstractNode;
        }
    }

}
