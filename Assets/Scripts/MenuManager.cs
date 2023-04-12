using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button beginBtn;

    private void Start()
    {
        beginBtn.onClick.AddListener(BeginClicked);
    }

    private void BeginClicked()
    {
        LoadMainScene();
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
