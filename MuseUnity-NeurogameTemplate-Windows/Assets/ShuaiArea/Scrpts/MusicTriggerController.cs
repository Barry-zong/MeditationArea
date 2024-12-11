using UnityEngine;
using System.Collections;
public class MusicTriggerController : MonoBehaviour
{
    AudioSource audioSource;
    public bool flow = false;
    public bool calm = false;
    public bool focus = false;
    public float musicMax = 1.0f;
    public float musicMin = 0.0f;
    public float MusicIntensity;
    private float currentVolume = 0f;
    private float targetVolume = 0f;
    private float currentIntensityVolume = 0.2f;
    private float targetIntensityVolume = 0.2f;
    private bool isPlaying = false;
    private bool isTransitioning = false;
    private float lastTriggerTime = -5f;
    private const float COOLDOWN_TIME = 5f;
    private const float TRANSITION_TIME = 2f;
    private const float VOLUME_SMOOTH_SPEED = 5f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;  // 确保音乐循环播放
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTriggerTime < COOLDOWN_TIME) return;
        lastTriggerTime = Time.time;
        StartCoroutine(StartMusic());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTriggerTime < COOLDOWN_TIME) return;
        lastTriggerTime = Time.time;
        StartCoroutine(StopMusic());
    }

    private IEnumerator StartMusic()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            isTransitioning = true;
            audioSource.Play();

            // 计算目标intensity音量
            targetIntensityVolume = Mathf.Lerp(0.2f, 1f,
                Mathf.InverseLerp(musicMin, musicMax, MusicIntensity));

            float elapsedTime = 0f;
            float startVolume = 0f;  // 从0开始

            while (elapsedTime < TRANSITION_TIME)
            {
                elapsedTime += Time.deltaTime;
                currentVolume = Mathf.Lerp(startVolume, targetIntensityVolume, elapsedTime / TRANSITION_TIME);
                audioSource.volume = currentVolume;
                yield return null;
            }

            currentIntensityVolume = targetIntensityVolume;  // 确保结束时intensity音量同步
            isTransitioning = false;
        }
    }

    private IEnumerator StopMusic()
    {
        if (isPlaying)
        {
            isTransitioning = true;
            targetVolume = 0f;
            float elapsedTime = 0f;
            float startVolume = currentIntensityVolume;

            while (elapsedTime < TRANSITION_TIME)
            {
                elapsedTime += Time.deltaTime;
                currentVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / TRANSITION_TIME);
                audioSource.volume = currentVolume;
                yield return null;
            }

            audioSource.Stop();
            isPlaying = false;
            isTransitioning = false;
        }
    }

    private void Update()
    {
        if (focus)
        {
            MusicIntensity = InteraxonInterfacer.Instance.focus;
        }
        if (calm)
        {
            MusicIntensity = InteraxonInterfacer.Instance.calm;
        }
        if (flow)
        {
            MusicIntensity = InteraxonInterfacer.Instance.flow;
        }

        if (isPlaying && !isTransitioning)
        {
            // Map MusicIntensity from [musicMin, musicMax] to [0.2, 1.0]
            targetIntensityVolume = Mathf.Lerp(0.2f, 1f,
                Mathf.InverseLerp(musicMin, musicMax, MusicIntensity));

            // Smooth transition for intensity volume
            currentIntensityVolume = Mathf.Lerp(currentIntensityVolume,
                targetIntensityVolume, Time.deltaTime * VOLUME_SMOOTH_SPEED);

            // Apply the intensity volume
            audioSource.volume = currentIntensityVolume;
            Debug.Log(currentIntensityVolume);
        }
    }
}