using UnityEngine;
using System.Collections.Generic;

public class MapLayerManager : MonoBehaviour
{
    [System.Serializable]
    public class MapLayerGroup
    {
        public string Name;

        public GameObject Foreground;
        public GameObject PanelBG;
        public GameObject Background;
        public GameObject PanelBBG;
        public GameObject BackBackground;
        public GameObject Lamp;

        public List<GameObject> GetAllLayers()
        {
            return new List<GameObject> { Foreground, PanelBG, Background, PanelBBG, BackBackground, Lamp };
        }
    }

    [Header("Map Settings")]
    public List<MapLayerGroup> layers = new List<MapLayerGroup>();
    private int currentLayerIndex = 0;

    void Start()
    {
        InitializeLayers();
    }

    private void InitializeLayers()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            bool isActive = (i == currentLayerIndex);
            SetLayerGroupActive(layers[i], isActive);
        }
    }

    private void SetLayerGroupActive(MapLayerGroup group, bool active)
    {
        foreach (GameObject layer in group.GetAllLayers())
        {
            if (layer != null)
            {
                layer.SetActive(active);
            }
        }
    }

    public void ResetToOriginalLayer()
    {
        SetLayerGroupActive(layers[currentLayerIndex], false);

        currentLayerIndex = 0;

        SetLayerGroupActive(layers[currentLayerIndex], true);
    }

    public void SwitchToNextLayer()
    {
        SetLayerGroupActive(layers[currentLayerIndex], false);

        currentLayerIndex = (currentLayerIndex + 1) % layers.Count;

        SetLayerGroupActive(layers[currentLayerIndex], true);

        Debug.Log("Current Layer: " + layers[currentLayerIndex].Name);
    }
}
