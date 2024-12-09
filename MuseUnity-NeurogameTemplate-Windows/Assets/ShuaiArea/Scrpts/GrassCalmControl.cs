using UnityEngine;

public class GrassCalmControl : MonoBehaviour
{
    public GameObject GrassPlane;
    public float GrassIntensity = 0;
    public Material terrainGrassMaterial; // ������Inspector��ָ������
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
            1 + GrassIntensity/3, // ����ԭʼֵΪ1��ֱ�Ӽ���calmValue
            currentScale.z
        );

        UpdateWindStrength(1-calmValue);
    }
    void UpdateWindStrength(float strength)
    {
        // ֱ�����ò��ʵ�Wind Strength����
        terrainGrassMaterial.SetFloat("_WindStrength", strength);
    }
}
