using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UiSyncShow : MonoBehaviour
{
    public ArduinoIOtoUnity arduinotounity;
    public MuseReceiver museReciver;
    public TextMeshProUGUI textMeshProUGUI1;
    public TextMeshProUGUI textMeshProUGUI2;
    private string messageA;
    private string messageB;
    // Start is called before the first frame update
    void Start()
    {
        messageA = arduinotounity.ArduinoMessage;
        messageB = museReciver.EegData;
    }

    // Update is called once per frame
    void Update()
    {
        if (arduinotounity.ArduinoMessage != null)
        { messageA = arduinotounity.ArduinoMessage; }
        if (museReciver.EegData != null)
        { messageB = museReciver.EegData; }
        textMeshProUGUI1.text = "Arduino Data: " + messageA;
        textMeshProUGUI2.text = "Muse Data: " + messageB;
    }
}
