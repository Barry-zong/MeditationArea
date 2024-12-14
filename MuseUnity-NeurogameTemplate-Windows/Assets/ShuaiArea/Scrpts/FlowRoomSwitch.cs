using UnityEngine;
using System.Collections;
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
    public MaterialEmissionController materialEmissionController;
    public float holdTime = 5f;

    public CircularMotion circularMotion;
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
        if(!isDebug)
        {
            mindvalue = InteraxonInterfacer.Instance.focus * 5;
        }
       
           
        
        if (flowStartBool)
        {
            if (-5 + addValue < 3)
            {
                addIntensity();
            }
            else
            {
                //switch
                //FadeInAudio();
              //  audioSource.Play();
              //  Debug.Log("musicShouldPlay_tree");
                StartCoroutine(DisableColliderAfterDelay());
                materialEmissionController.LighttheTree = true;
            }
            
        }
        circularMotion.height = addValue * 2.65f / 8f;
    }
    private IEnumerator DisableColliderAfterDelay()
    {
        yield return new WaitForSeconds(holdTime);  // 等待x秒
       // FadeOutAudio();
        roomclider.enabled = false;  // 禁用碰撞器
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
       
        if (mindvalue > 0.5f)
        {
            circularMotion.isEnhanced = true;
            addValue+=0.010f;
            addValue2 += 0.04f;
            if(addValue2>3)
            {
                addValue2 = 3;
            }

        }
        else
        {
            circularMotion.isEnhanced = false;
        }
        FlowDoorMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, -5+addValue));
        FlowStarMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, addValue2));

    }

    

   

}
