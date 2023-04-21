using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button beginBtn;
    public Image logoImage;
    
    private AudioSource _flavour;
    private readonly float _fadeRate = 0.02f;

    private void Start()
    {
        _flavour = GetComponent<AudioSource>();
        beginBtn.onClick.AddListener(BeginClicked);
        StartCoroutine(FadeIn());
    }

    private void BeginClicked()
    {
        // Once shorter audio track is sourced this will be re-enabled
        //StartCoroutine(PlayFlavourTrack());
        LoadMainScene();
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    IEnumerator PlayFlavourTrack()
    {
        // Begin playing the track
        _flavour.Play();
        // Wait until the track is finished before loading the main scene.
        // Since the track is very short this will look more like a sound effect but with no cutting off.
        while (_flavour.isPlaying)
        {
            // In here, an animation could be ran at the same time (perhaps fading to black?)
            yield return null;
        }

        // wait until track has finished to load the main game
        LoadMainScene();
    }

    // https://stackoverflow.com/questions/56516299/how-to-fade-in-ui-image-in-unity
    IEnumerator FadeIn()
    {
        // the targetAlpha is 1.0f meaning 255 in the alpha channel of the image
        float targetAlpha = 1.0f;
        Color currentColor = logoImage.color; // You cannot set a colour directly, thus a secondly variable is used
        while(currentColor.a < targetAlpha) {
            // lerp the colour towards the targetAlpha by the fadeRate for a smooth transition
            currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, _fadeRate * Time.deltaTime);
            // Update the logo colour by the now updated colour value
            logoImage.color = currentColor;
            yield return null;
        }
    }
}
