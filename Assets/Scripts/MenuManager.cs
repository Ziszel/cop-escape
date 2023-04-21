using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button beginBtn;
    public Image logoImage;
    public Image backgroundImage;
    public Image backgroundImage2;

    private AudioSource _flavour;
    private readonly float _fadeRate = 0.2f; // Determines the rate at which the logo fades in
    private readonly float _scrollSpeed = 20.0f; // Determines how quickly the parallax background scrolls
    private Vector3 _backgroundImageStartPos; // store the start positions for resetting later
    private Vector3 _backgroundImage2StartPos;
    private float _backgroundSize; // Getting the background size allows for easy resetting

    private void Start()
    {
        _flavour = GetComponent<AudioSource>(); // sound that plays when the button BEGIN is pressed
        beginBtn.onClick.AddListener(BeginClicked);
        StartCoroutine(FadeIn()); // begin fading in the logo
        // https://stackoverflow.com/questions/60487667/unity-2d-get-the-x-axis-and-width-of-the-rendered-ui-image-and-not-the-values-o
        // I need the width of the RectTransform NOT the width of the texture
        _backgroundSize = backgroundImage.transform.GetComponent<RectTransform>().rect.width;
        _backgroundImageStartPos = backgroundImage.transform.position;
        _backgroundImage2StartPos = backgroundImage2.transform.position;
    }

    private void Update()
    {
        ParallaxScroll();
        if (backgroundImage.transform.position.x < -(_backgroundSize - 200))
        {
            ResetBackgroundPosition();
        }
    }

    private void ParallaxScroll()
    {
        // Move background one
        backgroundImage.transform.position = new Vector3(
            backgroundImage.transform.position.x - _scrollSpeed * Time.deltaTime,
            backgroundImage.transform.position.y,
            backgroundImage.transform.position.z);
        
        // Move background two
        backgroundImage2.transform.position = new Vector3(
            backgroundImage2.transform.position.x - _scrollSpeed * Time.deltaTime,
            backgroundImage2.transform.position.y,
            backgroundImage2.transform.position.z);
    }

    private void ResetBackgroundPosition()
    {
        backgroundImage.transform.position = _backgroundImageStartPos;
        backgroundImage2.transform.position = _backgroundImage2StartPos;
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
    // This co-routine will fade the logo in over time (dictated by _fadeRate)
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
