using UnityEngine;

public class ReadyLinkMusic : MonoBehaviour
{
    private AudioSource audioSource;
    public bool testBug = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!(InteraxonInterfacer.Instance.flow==0)|| !(InteraxonInterfacer.Instance.focus == 0)|| !(InteraxonInterfacer.Instance.calm == 0))
        {
            testBug = true;
        }
        else
        {
            testBug= false;
        }
       
        if (testBug)
        {
            Debug.Log(true);
            audioSource.Play();
        }
    }
}
