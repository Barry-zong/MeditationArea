using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class RstartGame : MonoBehaviour
{
    public float fadeOutTime = 1f; // 淡出时间
    public bool showLoadingScreen = false; // 是否显示加载画面

    private bool isReloading = false;

    void Update()
    {
        // 检测是否按下R键且当前没有在重载过程中
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadSceneWithTransition());
        }
    }

    IEnumerator ReloadSceneWithTransition()
    {
        isReloading = true;

        // 可以在这里添加淡出效果
        // 例如：渐变到黑屏、显示加载画面等

        if (showLoadingScreen)
        {
            // 异步加载场景
            Scene currentScene = SceneManager.GetActiveScene();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene.buildIndex);
            asyncLoad.allowSceneActivation = false;

            // 等待加载完成
            while (asyncLoad.progress < 0.9f)
            {
                float progress = asyncLoad.progress * 100;
                Debug.Log("Loading progress: " + progress + "%");
                yield return null;
            }

            // 完成加载
            asyncLoad.allowSceneActivation = true;
        }
        else
        {
            // 简单地重新加载当前场景
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        isReloading = false;
    }

    // 可选：添加保存游戏状态的方法
    void SaveGameState()
    {
        // 在这里添加保存游戏状态的代码
        // 例如：保存分数、等级等数据
        PlayerPrefs.Save();
    }

    // 可选：添加恢复游戏状态的方法
    void LoadGameState()
    {
        // 在这里添加加载游戏状态的代码
        // 例如：读取保存的数据
    }
}
