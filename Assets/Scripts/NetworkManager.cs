using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviour
{
    // Variables generales
    public int networkSize;
    Color colorLine = new Color(0.949f, 0.180f, 0.275f);
    public float height;
    public float width;
    private float area = 0;
    private float k;
    private float tamGrid;
    public float cooldownTime = 2f;
    private bool cooldown = false;
    private List<GameObject> vertexSet = new List<GameObject>();
    private List<GameObject> edgeSet = new List<GameObject>();
    // Variables actualizadas en la interfaz
    public float constante = 1f;
    public float temperature = 0.01f;
    public float vertexSize = 1f;
    public float lineThickness = 0.2f;
    public float lineAlpha = 0.5f;
    public float edgeFilter = 0f;
    // Asignaciones del editor
    public GameObject nodePrefab;
    public GameObject vertexPrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        area = height * width;
        k = constante * Mathf.Sqrt(area / Mathf.Max(1, vertexSet.Count));

        SpawnNode(networkSize);
    }

    // Update is called once per frame
    void Update()
    {
        k = Mathf.Max(constante,0.01f) * Mathf.Sqrt(area / Mathf.Max(1, vertexSet.Count));
        UpdateLayout();

    }

    private void FixedUpdate()
    {
        foreach (GameObject e in edgeSet)
        {
            if (e.activeInHierarchy)
            {
                DrawEdge(e);
            }
        }
    }


    void UpdateLayout()
    {
        // Se implementó el algoritmo de Fruchterman & Reingold (1990)
        if (vertexSet.Count < 2)
        {
            return;
        }
        Node v;
        Vector2 disp;
        Node u;
        Vector2 delta;
        float dist;
        Edge e;
        Vector2 vDisp;
        Vector2 uDisp;
        Vector2 pos;
        //Calculo de fuerzas de repulsión
        foreach (GameObject vertex in vertexSet)
        {
            v = vertex.GetComponent<Node>();
            disp = new Vector2(0f, 0f);
            foreach (GameObject uertex in vertexSet)
            {
                if (vertex != uertex)
                {
                    u = uertex.GetComponent<Node>();
                    delta = v.position() - u.position();
                    dist = Vector2.Distance(v.position(), u.position());
                    if (dist == 0f)
                    {
                        delta = new Vector2(Random.Range(0.1f, 1f), Random.Range(-1f, 1f));
                        dist = Vector2.Distance(new Vector2(0f,0f), delta);
                    }
                    disp = disp + (delta / dist) * RepulsionForce(dist);
                    
                }
            }
            v.SetDisplacement(disp);
        }

        //Calculo de fuerzas de atracción
        foreach (GameObject edge in edgeSet)
        {
            if (!edge.activeInHierarchy)
            {
                continue;
            }
            e = edge.GetComponent<Edge>();
            delta = e.v().position() - e.u().position();
            dist = Vector2.Distance(e.v().position(), e.u().position());
            if (dist == 0f)
            {
                delta = new Vector2(Random.Range(0.1f, 1f), Random.Range(-1f, 1f));
                dist = Vector2.Distance(new Vector2(0f, 0f), delta);
            }

            vDisp = e.v().displacement() - (delta / dist) * AttractionForce(dist);
            uDisp = e.u().displacement() + (delta / dist) * AttractionForce(dist);
            e.v().SetDisplacement(vDisp);
            e.u().SetDisplacement(uDisp);
        }

        //Aplicación del resultado
        foreach (GameObject vertex in vertexSet)
        {
            v = vertex.GetComponent<Node>();
            dist = Vector2.Distance(new Vector2(0f, 0f), v.displacement());
            if (dist == 0f)
            {
                dist = 1f;
            }
            pos = v.position() + (v.displacement() / dist) * Mathf.Min(dist, temperature);
            pos.x = Mathf.Min(width / 2, Mathf.Max(-width / 2, pos.x));
            pos.y = Mathf.Min(height/ 2, Mathf.Max(-height / 2, pos.y));
            v.SetPosition(pos);
        }
    }

    float AttractionForce(float distance)
    {
        return distance * distance / k;
    }

    float RepulsionForce(float distance)
    {
        if(distance > tamGrid)
        {
            return 0f;
        }
        return k * k / distance;
    }


    void SpawnEdge(GameObject node0, GameObject node1)
    {
        Node u = node0.GetComponent<Node>();
        Node v = node1.GetComponent<Node>();
        float w = Random.Range(0.1f, 1f);

        GameObject vertex = Instantiate(vertexPrefab, u.position(), Quaternion.identity);
        vertex.GetComponent<Edge>().AssignNodes(u, v, w);
        // Se asignan elementos gráficos
        LineRenderer lineRenderer = vertex.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        lineRenderer.sortingOrder = 1;

        edgeSet.Add(vertex);
    }

    void DrawEdge(GameObject vertex)
    {
        Edge e = vertex.GetComponent<Edge>();
        Node v = e.v();
        Node u = e.u();
        LineRenderer lineRenderer = vertex.GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = lineThickness;
        //lineRenderer.startColor = Color.yellow;
        //lineRenderer.endColor = Color.yellow;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(colorLine, 0.0f), new GradientColorKey(colorLine, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(lineAlpha, 0.0f), new GradientAlphaKey(lineAlpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        lineRenderer.SetPosition(0, v.position());
        lineRenderer.SetPosition(1, u.position());
    }


    void SpawnNode(int numberOfNodes)
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            Vector2 nodePosition = new Vector2(x, y);

            GameObject node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
            node.GetComponent<Node>().SetPosition(nodePosition);
            node.transform.localScale = Vector3.one * vertexSize;

            if (vertexSet.Count > 1)
            {
                int node1 = Random.Range(0, vertexSet.Count - 1);
                SpawnEdge(node, vertexSet[node1]);
                float prob = Random.Range(0f, 100f);
                if (prob > 0f)
                {
                    node1 = Random.Range(0, vertexSet.Count - 1);
                    SpawnEdge(node, vertexSet[node1]);
                }
                if (prob > 0f)
                {
                    node1 = Random.Range(0, vertexSet.Count - 1);
                    SpawnEdge(node, vertexSet[node1]);
                }


            }

            vertexSet.Add(node);
        }

        tamGrid = 2 * Mathf.Sqrt(width * height / vertexSet.Count);
        
    }

 
    public int VertexCount()
    {
        return vertexSet.Count;
    }

    public int EdgeCount()
    {
        return edgeSet.Count;
    }

    public void ChangeVertexScale(float scale)
    {
        vertexSize = scale;
        foreach(GameObject vertex in vertexSet)
        {
            vertex.transform.localScale = Vector3.one * vertexSize;
        }
    }

    public void FilterEdges(float filter)
    {
        edgeFilter = filter;
        foreach (GameObject edge in edgeSet)
        {
            float weight = edge.GetComponent<Edge>().Weight();
            if (weight < edgeFilter)
            {
                edge.SetActive(false);
                continue;
            }
            edge.SetActive(true);
        }
    }
}
