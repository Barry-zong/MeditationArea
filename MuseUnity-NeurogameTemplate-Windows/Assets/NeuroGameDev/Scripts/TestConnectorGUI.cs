using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Interaxon.Libmuse;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// This class is recycled from Interaxon's own SampleApp script.
/// Excuse the unconventional style and heavy use of comments and Debug.Logs.
/// </summary>
public class TestConnectorGUI : MonoBehaviour
{
    //--------------------------------------
    // Public members that connects to UI components

    [Header("GUI Elements")] public Button startScanButton;
    public Button connectButton;
    public Button disconnectButton;
    public Dropdown museList;
    public Text connectionText;
    public Text bluetoothText;
    public Text batteryText;
    public Text fitTP9Text;
    public Text fitAF7Text;
    public Text fitAF8Text;
    public Text fitTP10Text;
    public Text calmText;
    public Text focusText;
    public Text flowText;
    public Text ppgValue;
    public Image ppgDot;
    public Text headbandOnText;
    public Text blinkText;
    public Text jawText;
    public Text forwardText;
    public Text rightText;
    public Text downText;
    public Text rollText;
    public Text pitchText;
    public Text yawText;

    //--------------------------------------
    // Public methods that gets called on UI events.

    public void startScanning()
    {
        //Debug.Log("startScanning");
        // Must register at least MuseListeners before scanning for headbands.
        // Otherwise no callbacks will be triggered to get a notification.

        InteraxonInterfacer.Instance.startScanning();
        StartCoroutine(WaitBeforeFillingMuseList());
    }

    private IEnumerator WaitBeforeFillingMuseList()
    {
        float elapsedTime = 0f;

        Debug.Log("Waiting for connection");
        do
        {
            yield return new WaitForSeconds(0.25f);
            elapsedTime += 0.25f;

            receiveMuseList(InteraxonInterfacer.Instance.museList);
        } while (string.IsNullOrEmpty(InteraxonInterfacer.Instance.museList) && elapsedTime < 5f);

        if (elapsedTime >= 5f)
        {
            Debug.Log("No Muse could be found");
        }
        else
        {
            Debug.Log("Connection established");
        }
    }

    public void userSelectedMuse()
    {
        //this.userPickedMuse = this.museList.options[this.museList.value].text;
        //Debug.Log("Selected muse = " + this.userPickedMuse);

        InteraxonInterfacer.Instance.userSelectedMuse(this.museList.options[this.museList.value].text);
    }

    public void connect()
    {
        Debug.Log("Connect");
        // If user just clicks connect without selecting a muse from the
        // dropdown menu, then connect to the one displayed in the dropdown.

        if (InteraxonInterfacer.Instance.userMuse == "")
        {
            InteraxonInterfacer.Instance.userMuse = this.museList.options[0].text;
        }

        //Debug.Log("Connecting to " + InteraxonInterface.userMuse);
        InteraxonInterfacer.Instance.connect();
    }

    public void disconnect()
    {
        Debug.Log("Disconnected");
        InteraxonInterfacer.Instance.disconnect();
    }

    private void OnApplicationQuit()
    {
#if PLATFORM_STANDALONE_WIN
        if (InteraxonInterfacer.Instance.connected)
        {
            InteraxonInterfacer.Instance.disconnect();
        }
#endif
    }
    //--------------------------------------
    // Private Variables

    private bool initialized;
    private RectTransform heartBounceTransform;

    //--------------------------------------
    // Private Methods

    private void OnEnable()
    {
        heartBounceTransform = ppgDot.rectTransform;
    }

    //--------------------------------------
    // These listener methods update the buffer
    // The Update() per frame will display the data.

    private void receiveMuseList(string data)
    {
        // Convert string to list of muses and populate the dropdown menu.
        var muses = data.Split(' ').ToList();
        this.museList.ClearOptions();
        this.museList.AddOptions(muses);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!this.initialized && !string.IsNullOrEmpty(InteraxonInterfacer.Instance.museList))
        {
            receiveMuseList(InteraxonInterfacer.Instance.museList);
            initialized = true;
        }
#if PLATFORM_STANDALONE_WIN
        LibmuseBridgeWindows.InvokeDispatchQueue();
#endif

        GUIRefresh();
    }

    private void GUIRefresh()
    {
        ButtonCheck();

        connectionText.text = InteraxonInterfacer.Instance.currentConnectionState.ToString();
        bluetoothText.text = InteraxonInterfacer.Instance.bluetoothMac;
        batteryText.text = $"{(InteraxonInterfacer.Instance.Battery.level > 0 ? InteraxonInterfacer.Instance.Battery.level.ToString() + "%" : "FETCHING DATA")}";

        // Headband Fit

        fitTP9Text.text = HSIFormat(InteraxonInterfacer.Instance.HeadbandFit.fitTP9);
        fitAF7Text.text = HSIFormat(InteraxonInterfacer.Instance.HeadbandFit.fitAF7);
        fitAF8Text.text = HSIFormat(InteraxonInterfacer.Instance.HeadbandFit.fitAF8);
        fitTP10Text.text = HSIFormat(InteraxonInterfacer.Instance.HeadbandFit.fitTP10);

        // Interpreted Values

        calmText.text = InteraxonInterfacer.Instance.calm.ToString();
        focusText.text = InteraxonInterfacer.Instance.focus.ToString();
        flowText.text = InteraxonInterfacer.Instance.flow.ToString();
        ppgValue.text = InteraxonInterfacer.Instance.heartMonitor.ToString();

        var transformRate = Mathf.Clamp(InteraxonInterfacer.Instance.heartMonitor / 100000f, 0f, 1f);

        // Try to make it beat like an EKG

        ppgDot.transform.transform.localScale = Vector2.Lerp(heartBounceTransform.transform.localScale, new Vector2((1 + (transformRate * 10)), (1 + (transformRate * 10))), Time.deltaTime * 500f);
        //heartBounce.transform.transform.localScale = new Vector2((1 + (transformRate * 10)), (1 + (transformRate * 10)));

        // Try to make it bounce like an EKG

        //heartBounce.transform.position = Vector2.Lerp(heartBounce.transform.position, new Vector2(heartBounceInitialPlace.x, heartBounceInitialPlace.y + (InteraxonInterfacer.Instance.heartMonitor / 100f)), Time.deltaTime * 10f);
        //heartBounce.transform.position = new Vector2(heartBounceInitialPlace.x, heartBounceInitialPlace.y + (InteraxonInterfacer.Instance.heartMonitor / 1000f));
        //Vector3.Lerp(transform.position, new Vector3(_startPosition.x, _startPosition.y + InteraxonInterfacer.Instance.calm, _startPosition.z), Time.deltaTime * 1f);

        // Artifacts

        headbandOnText.text = InteraxonInterfacer.Instance.Artifacts.headbandOn.ToString();
        blinkText.text = InteraxonInterfacer.Instance.Artifacts.blink.ToString();
        jawText.text = InteraxonInterfacer.Instance.Artifacts.jawClench.ToString();

        // Accelerometer

        forwardText.text = InteraxonInterfacer.Instance.Accelerometer.forward.ToString();
        rightText.text = InteraxonInterfacer.Instance.Accelerometer.right.ToString();
        downText.text = InteraxonInterfacer.Instance.Accelerometer.down.ToString();

        // Gyroscope

        rollText.text = InteraxonInterfacer.Instance.Gyro.roll.ToString();
        pitchText.text = InteraxonInterfacer.Instance.Gyro.pitch.ToString();
        yawText.text = InteraxonInterfacer.Instance.Gyro.yaw.ToString();
    }

    private void ButtonCheck()
    {
        var connectButtonText = this.connectButton.GetComponentInChildren<Text>();

        if (InteraxonInterfacer.Instance.currentConnectionState == ConnectionState.CONNECTED &&
            museList.options[museList.value].text == InteraxonInterfacer.Instance.userMuse)
        {
            connectButtonText.text = "Connected";

            this.connectButton.interactable = false;
            this.disconnectButton.interactable = true;
        }
        else if (InteraxonInterfacer.Instance.currentConnectionState == ConnectionState.CONNECTING)
        {
            connectButtonText.text = "Connecting...";

            this.connectButton.interactable = false;
            this.disconnectButton.interactable = false;
        }
        else
        {
            connectButtonText.text = "Connect";

            this.connectButton.interactable = true;
            this.disconnectButton.interactable = false;
        }
    }

    private string HSIFormat(int hsiValue)
    {
        string result = "";

        switch (hsiValue)
        {
            case 1:
                result = "[1] : Good fit";
                return result;
            case 2:
                result = "[2] : Mediocre fit";
                return result;
            case 4:
                result = "[4] : Bad fit";
                return result;
            default:
                result = "NO DATA";
                return result;
        }
    }
}