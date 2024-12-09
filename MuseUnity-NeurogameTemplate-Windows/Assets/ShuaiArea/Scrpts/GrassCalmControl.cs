using UnityEngine;

public class GrassCalmControl : MonoBehaviour
{
    public GameObject GrassPlane;
    public float GrassIntensity = 0;
    public Material terrainGrassMaterial; // 可以在Inspector中指定材质
    private float calmValue = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GrassIntensity = InteraxonInterfacer.Instance.calm;
        if(GrassIntensity >1.5f)
        {
            GrassIntensity = 1.5f;
        }
         calmValue = Mathf.InverseLerp(0,1.5f, GrassIntensity);

        Vector3 currentScale = GrassPlane.transform.localScale;
        GrassPlane.transform.localScale = new Vector3(
            currentScale.x,
            1 + GrassIntensity/3, // 假设原始值为1，直接加上calmValue
            currentScale.z
        );

        UpdateWindStrength(1-calmValue);
    }
    void UpdateWindStrength(float strength)
    {
        // 直接设置材质的Wind Strength属性
        terrainGrassMaterial.SetFloat("_WindStrength", strength);
    }
}
