using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AgentUtils
{
    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    public static Vector3 RandomLocation(float radius)
    {
        return new Vector3(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius), 0);
    }


    public static void SetChildColor(GameObject gameObject, int childIndex, Color color)
    {
        gameObject.transform.GetChild(childIndex).GetComponent<SpriteRenderer>().material.color = color;
    }


    public static GameObject FindClosestObject(GameObject source, float searchRadius, string layerMask)
    {
        float closestObjectDistance = 1000f;
        GameObject closestObject = null;

        //get all object in a given radius
        Collider2D[] objectColliders = Physics2D.OverlapCircleAll(source.transform.position, searchRadius, LayerMask.GetMask(layerMask));

        //Loop over the given object found
        foreach (Collider2D obj in objectColliders)
        {
            // find distance to object
            float distanceToObject = Vector3.Distance(obj.transform.position, source.transform.position);

            //check if distance is smaller the the closest one yet
            if (distanceToObject < closestObjectDistance)
            {
                if (obj.gameObject.name != source.name)
                {
                    closestObject = obj.gameObject; //current object is closest else continue
                    closestObjectDistance = distanceToObject;
                }
            }
        }

        return closestObject;
    }

    public static GameObject FindClosestObject(GameObject source, float searchRadius, string layerMask, string tag)
    {
        float closestObjectDistance = 1000f;
        GameObject closestObject = null;

        //get all object in a given radius
        Collider2D[] objectColliders = Physics2D.OverlapCircleAll(source.transform.position, searchRadius, LayerMask.GetMask(layerMask));

        //Loop over the given object found
        foreach (Collider2D obj in objectColliders)
        {
            if (closestObject.tag == tag)
            {
                // find distance to object
                float distanceToObject = Vector3.Distance(obj.transform.position, source.transform.position);

                //check if distance is smaller the the closest one yet
                if (distanceToObject < closestObjectDistance)
                {
                    if (obj.gameObject.name != source.name)
                    {
                        closestObject = obj.gameObject; //current object is closest else continue
                        closestObjectDistance = distanceToObject;
                    }
                }
            }
        }

        return closestObject;
    }

}
