using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyCanvas : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button SubmitBtn;


    public void LoadMainScene()
    {
        Client.instance.Login(nameInput.text);
        SceneManager.LoadScene("Main");
    }
}