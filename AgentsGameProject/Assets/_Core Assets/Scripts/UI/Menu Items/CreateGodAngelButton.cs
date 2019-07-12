using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGodAngelButton : MonoBehaviour
{
    public void CallCreateAngel()
    {
        GetComponent<CreateAngel>().CreateAngelFunc();
    }
}
