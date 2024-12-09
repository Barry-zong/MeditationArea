using UnityEngine;

public class FlowRoomSwitch : MonoBehaviour
{
    public bool isDebug = false;
    public Material FlowDoorMat;
    public Material FlowStarMat;
    public float mindvalue = 0 ;
    private float addValue = 0;
    private float addValue2 = 0;
    private Collider roomclider;
    public bool flowStartBool = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomclider = GetComponent<Collider>();
        FlowDoorMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, -10));
        FlowStarMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDebug)
        {
            mindvalue = InteraxonInterfacer.Instance.focus;
        }
        if (flowStartBool)
        {
            if (-5 + addValue < 3)
            {
                addIntensity();
            }
            else
            {
                roomclider.enabled = false;
            }
        }
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家开始接触平台");
            flowStartBool = true;
        }
    }

    void addIntensity()
    {
       
        if (mindvalue > 1.2)
        {
            addValue+=0.01f;
            addValue2 += 0.03f;
            if(addValue2>3)
            {
                addValue2 = 3;
            }

        }
        FlowDoorMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, -5+addValue));
        FlowStarMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, addValue2));

    }
}
