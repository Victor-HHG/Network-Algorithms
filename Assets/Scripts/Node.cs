using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Vector2 disp = new Vector2(0f, 0f);
    private Vector2 pos = new Vector2(0f, 0f);

    public Vector2 position()
    {
        return pos;
    }

    public Vector2 displacement()
    {
        return disp;
    }

    public void SetPosition(Vector2 newPos)
    {
        pos = newPos;
        transform.position = pos;
    }
    
    public void SetDisplacement(Vector2 newDisp)
    {
        disp = newDisp;
    }
}
