using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesFactory
{
    private NodesFactory _instance;
    public NodesFactory Instance {
        get {
            if (_instance != null)
            {
                return _instance;
            }
            else {
                _instance = this;
                return _instance;
            }
        }
    }

    /* 委托参数的用法 —— 这里看来，委托类型在其所被声明的类以外的类引用时，引用语法和 Enum 的引用语法非常相似
     */
    public BaseNode CreateTheNode(Vector2 position, NodeType node, BaseNode.OnClickAsInPoint OnClickInPoint, BaseNode.OnClickAsOutPoint OnClickOutPoint, Action<BaseNode> OnClickRemoveNode) {
        BaseNode nodeCreated;

        switch (node) {
            //case BTNodes.Base:
            //    nodeCreated = new RootNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            //    break;
            case NodeType.Fallback:
                nodeCreated = new FallbackNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            case NodeType.Sequence:
                nodeCreated = new SequenceNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            case NodeType.Parallel:
                nodeCreated = new ParallelNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            case NodeType.Decorator:
                nodeCreated = new DecoratorNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            case NodeType.Selector:
                nodeCreated = new SelectorNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            case NodeType.Action:
                nodeCreated = new ActionNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            case NodeType.Condition:
                nodeCreated = new ConditionNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
            default:
                nodeCreated = new SequenceNode(position, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                break;
        }

        return nodeCreated;
    }
}

