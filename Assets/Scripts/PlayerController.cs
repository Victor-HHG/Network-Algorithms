using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI VertexCounterText;
    [SerializeField] TextMeshProUGUI EdgeCounterText;
    [SerializeField] TextMeshProUGUI EdgeFilterValueText;

    public NetworkManager network;
    private bool useLogFilter = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        VertexCounterText.text = network.VertexCount().ToString();
        EdgeCounterText.text = network.EdgeCount().ToString();
    }

    public void UpdateForceScale(float value)
    {
        network.constante = value;
    }
    public void UpdateTemperature(float value)
    {
        network.temperature = value;
    }
    public void UpdateLineThickness(float value)
    {
        network.lineThickness = value;
    }

    public void UpdateLineAlpha(float value)
    {
        network.lineAlpha = value;
    }

    public void UpdateVertexSize(float value)
    {
        network.ChangeVertexScale(value);
    }

    public void UpdateEdgeFilter(float value)
    {
        float filter;
        if (useLogFilter)
        {
            filter = Mathf.Round(Mathf.Exp(value) * 1000f) / 1000f;
        }
        else
        {
            float value2 = (value + 50f) / 50f;
            filter = Mathf.Round(value2 * 1000f) / 1000f;
        }
        network.FilterEdges(filter);
        EdgeFilterValueText.text = filter.ToString();
    }

    public void ChangeFilterScale(bool check)
    {
        useLogFilter = check;
    }
}
