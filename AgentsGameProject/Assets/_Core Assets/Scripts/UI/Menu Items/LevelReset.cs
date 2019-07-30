using UnityEngine;

public class LevelReset : MonoBehaviour
{
    ResourcesDataController ResourcesDataControllerRef;
    public GameObject GameOverPanel;
    public IntVariable AgentsBorn;

    void Awake()
    {
        ResourcesDataControllerRef = GetComponent<ResourcesDataController>();
        AgentsBorn.SetValue(2);
    }

    void Update()
    {
       if (ResourcesDataControllerRef.Population.Value <= 0)
        {
            GameOverPanel.SetActive(true);
        }
    }
}
