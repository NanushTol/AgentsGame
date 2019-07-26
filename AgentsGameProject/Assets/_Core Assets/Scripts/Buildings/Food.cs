using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float MaxFood = 200;

    public int MaxFeeders = 6;

    public Color FeedingColor;
    public Color NotFeedingColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [HideInInspector]
    public int FeedingAgents = 0;

    [HideInInspector]
    public bool FeedingVacancy = true;

    public float FoodValue = 0.0f;

    Transform _feedingVacancyBar;
    Transform _foodBar;

    void Awake()
    {
        _feedingVacancyBar = transform.GetChild(2);
        _foodBar = transform.GetChild(3).transform.GetChild(1);
    }

    void Update()
    {
        UpdateFeedingVacancy();

        UpdateFeedingBar();

        UpdateFoodBar();
    }

    void UpdateFeedingBar()
    {
        if (FeedingAgents <= MaxFeeders)
        {
            for (int j = 0; j < MaxFeeders; j++)
            {
                _feedingVacancyBar.GetChild(j).gameObject.GetComponent<SpriteRenderer>().color = NotFeedingColor;
            }

            for (int i = 0; i < FeedingAgents; i++)
            {
                _feedingVacancyBar.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = FeedingColor;
            }
        }
    }

    void UpdateFeedingVacancy()
    {
        if (FeedingAgents < MaxFeeders)
            FeedingVacancy = true;
        else
            FeedingVacancy = false;
    }

    void UpdateFoodBar()
    {
        float mapedBar = AgentUtils.Remap(FoodValue, 0f, MaxFood, 0f, 1f);

        _foodBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapedBar);
    }
}
