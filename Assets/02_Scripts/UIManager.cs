using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    void OnEnable()
    {
        // 이벤트 연결
        startButton.onClick.AddListener(() => OnStartButtonClick());
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Level01");
        SceneManager.LoadScene("Logic", LoadSceneMode.Additive);
    }
}
