using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private Node node0;
    private Node node1;
    private float weight = 1f;

    public void AssignNodes(Node _node0, Node _node1, float _weight)
    {
        node0 = _node0;
        node1 = _node1;
        weight = _weight;
    }

    public Node v()
    {
        return node0;
    }
    public Node u()
    {
        return node1;
    }

    public float Weight()
    {
        return weight;
    }
}
