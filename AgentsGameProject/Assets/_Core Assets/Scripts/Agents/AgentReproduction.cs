using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentReproduction
{
    public static void Reproduce(Agent argSelf, Agent mate)
    {
        // find a vacant location to instantiate the baby
        Vector3 birthLocation = FindVacantLocation(argSelf);

        // instantiate baby
        GameObject baby = argSelf.InstantiateBaby(birthLocation);

        // set baby's traits
        SetBabyTraits(argSelf, mate, baby);

        // set baby's color
        SetBabyColor(argSelf, mate, baby);

        // reset Parents Reproductive parameters
        ResetReproductiveParmeters(argSelf, mate);
    }




    private static Vector3 FindVacantLocation(Agent argSelf)
    {
        Vector3 location = new Vector3(0f,0f,0f);

        for (int j = 0; j < 30; j++)
        {
            location = argSelf.transform.position + AgentUtils.RandomLocation(argSelf.AgentsSharedParameters.SearchRadius);

            location.z = 0.0f;

            //check agents on location
            Collider2D[] objectColliders = Physics2D.OverlapCircleAll(location, 0.4f, LayerMask.GetMask("Agent")); 

            //check if location is on ground
            bool PositionValid = Physics2D.CircleCast(location, 0.55f, Camera.main.transform.forward, 100f, LayerMask.GetMask("Ground"));

            if (PositionValid && objectColliders.Length == 0)
            {
                break;
            }
        }

        return location;
    }

    private static void SetBabyTraits(Agent argSelf, Agent mate, GameObject baby)
    {
        float mutationPrecentage = argSelf.AgentsSharedParameters.MutaionPercentage;

        // get baby traits array
        float[] babyTraits = baby.GetComponent<Agent>().Traits; 


        //set mission in life to self mission
        baby.GetComponent<Agent>().AgentType = argSelf.AgentType; 


        // Decide which perent's trait to take by chance
        float randomHeritage = UnityEngine.Random.Range(0, 1); //get chance

        //Loop Traits & mutate
        for (int i = 0; i < babyTraits.Length; i++)
        {
            // baby get self trait
            if (randomHeritage <= 0.50)
            {
                babyTraits[i] = MutateTrait(mutationPrecentage, argSelf.Traits[i]);
            }

            // baby get mate trait
            else if (randomHeritage > 0.50)
            {
                babyTraits[i] = MutateTrait(mutationPrecentage, mate.Traits[i]);
            }

            if (i > 0) // dont clamp max life
            {
                babyTraits[i] = Mathf.Clamp(babyTraits[i], 1f, 10f); // limit trait value: from 1 to 10
            }
        }
    }

    private static float MutateTrait(float mutationPrecentage, float trait)
    {
        return trait * UnityEngine.Random.Range(-mutationPrecentage, mutationPrecentage) + trait;
    }

    private static void SetBabyColor(Agent argSelf, Agent mate, GameObject baby)
    {
        //Conver self Color To HSV
        Color.RGBToHSV(argSelf.AgentColor, out float h, out float s, out float v);

        // Conver Partners Color to HSV
        Color.RGBToHSV(mate.AgentColor, out float mateH, out float mateS, out float mateV);

        // Average Colors
        h = (h + mateH) / 2f;

        // Mutate Color by 5%
        h = MutateTrait(0.05f, h);

        // clamp to valid values
        h = Mathf.Clamp(h, 0f, 360f);

        // Assign Color to baby
        baby.GetComponent<Agent>().AgentColor = Color.HSVToRGB(h, s, v);
    }

    private static void ResetReproductiveParmeters(Agent argSelf, Agent mate)
    {
        mate.wantsToMate = false;
        mate.foundMate = false;
        mate.NeedsManager.NeedsValues[(int)AgentNeedsManager.Needs.Horny] = 0f;
        mate.ReproductiveMultiplier = 0.0f;

        argSelf.wantsToMate = false;
        argSelf.foundMate = false;
        argSelf.NeedsManager.NeedsValues[(int)AgentNeedsManager.Needs.Horny] = 0f;
        argSelf.ReproductiveMultiplier = 0.0f;
    }
}
