using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button beginBtn;
    private AudioSource _flavour;

    private void Start()
    {
        _flavour = GetComponent<AudioSource>();
        beginBtn.onClick.AddListener(BeginClicked);
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

}
