using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Guardem la dificultat seleccionada
    public static int selectedDifficulty = 0;  
    // 0=Easy, 1=Medium, 2=Hard

    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI labelBtn;
    [SerializeField] private AudioClip audioClip;

    void Start()
    {
        
    }

    public void SelectDifficulty(int d)
    {
        selectedDifficulty = d;
        Debug.Log("Dificultat seleccionada: " + d);
    }

    public void onClickButton()
    {
        Debug.Log("Iniciant partida...");

        AudioSource source = GetComponent<AudioSource>();
        source.PlayOneShot(audioClip);

        SceneManager.LoadScene("GameScene");
    }
}