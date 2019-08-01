using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public static class GameUtils 
{
    public static bool IsMouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static RaycastHit2D CastRayFromMouse(LayerMask mask)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Physics2D.Raycast(mousePos, Camera.main.transform.forward, 15f, mask);
    }
}
