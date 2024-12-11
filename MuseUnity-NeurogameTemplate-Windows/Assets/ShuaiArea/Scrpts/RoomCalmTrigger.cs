using UnityEngine;

public class RoomCalmTrigger : MonoBehaviour
{
    public bool playFirstin = false;
    public GrassCalmControl grassCalmControl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playFirstin = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (playFirstin)
        {
            if (other.CompareTag("Player"))
            {
              //  Debug.Log("playerinnnnn");
             // grassCalmControl.getedPlayerin = true;
                playFirstin = false;

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
