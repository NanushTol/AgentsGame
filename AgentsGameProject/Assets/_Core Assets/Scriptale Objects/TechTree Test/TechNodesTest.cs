using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechNodesTest : MonoBehaviour
{
    public TechNode T1;
    public TechNode T2;

    void Awake()
    {
        T2.PreNode = T1;
    }
}
