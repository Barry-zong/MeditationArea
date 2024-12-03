using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class RstartGame : MonoBehaviour
{
    public float fadeOutTime = 1f; // ����ʱ��
    public bool showLoadingScreen = false; // �Ƿ���ʾ���ػ���

    private bool isReloading = false;

    void Update()
    {
        // ����Ƿ���R���ҵ�ǰû�������ع�����
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadSceneWithTransition());
        }
    }

    IEnumerator ReloadSceneWithTransition()
    {
        isReloading = true;

        // ������������ӵ���Ч��
        // ���磺���䵽��������ʾ���ػ����

        if (showLoadingScreen)
        {
            // �첽���س���
            Scene currentScene = SceneManager.GetActiveScene();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene.buildIndex);
            asyncLoad.allowSceneActivation = false;

            // �ȴ��������
            while (asyncLoad.progress < 0.9f)
            {
                float progress = asyncLoad.progress * 100;
                Debug.Log("Loading progress: " + progress + "%");
                yield return null;
            }

            // ��ɼ���
            asyncLoad.allowSceneActivation = true;
        }
        else
        {
            // �򵥵����¼��ص�ǰ����
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        isReloading = false;
    }

    // ��ѡ����ӱ�����Ϸ״̬�ķ���
    void SaveGameState()
    {
        // ��������ӱ�����Ϸ״̬�Ĵ���
        // ���磺����������ȼ�������
        PlayerPrefs.Save();
    }

    // ��ѡ����ӻָ���Ϸ״̬�ķ���
    void LoadGameState()
    {
        // ��������Ӽ�����Ϸ״̬�Ĵ���
        // ���磺��ȡ���������
    }
}
