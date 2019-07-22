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


    public static void SetColor(GameObject argGameObject, int childIndex, Color color)
    {
        argGameObject.transform.GetChild(childIndex).GetComponent<SpriteRenderer>().material.color = color;
    }


    public static void InitializeTraits(Agent agent, float[] traits)
    {
        traits = new float[6];

        traits[0] = agent.MaxAge;
        traits[1] = agent.ReproductiveUrge;
        traits[2] = agent.FoodConsumption;
        traits[3] = agent.EnergyConsumption;
        traits[4] = agent.WorkingSpeed;
        traits[5] = agent.Size;
    }


    public static GameObject FindClosestMate(GameObject source, float searchRadius)
    {
        float closestObjectDistance = 1000f;
        GameObject closestObject = null;

        //get all object in a given radius
        Collider2D[] objectColliders = Physics2D.OverlapCircleAll(source.transform.position, searchRadius, LayerMask.GetMask("Agent"));

        // no others found
        if (objectColliders.Length <= 0)
        {
            closestObject = null;
        }

        // found others
        else if (objectColliders.Length > 0)
        {
            // Loop over the given object found
            foreach (Collider2D obj in objectColliders)
            {
                if (obj.gameObject.name != source.name)// If it is not this object
                {
                    // find distance to object
                    float distanceToObject = Vector3.Distance(obj.transform.position, source.transform.position);

                    if (obj.GetComponent<Agent>().wantsToMate) // check if other wants to mate ****** Will break when "Agent" is re-writen to have a proper state machine ******
                    {
                        // Check if distance is smaller the the closest one yet
                        if (distanceToObject < closestObjectDistance)
                        {
                            closestObject = obj.gameObject;
                            closestObjectDistance = distanceToObject;
                        }
                    }
                    else
                    {
                        closestObject = null;
                    }
                }
            }
        }

        return closestObject;
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
