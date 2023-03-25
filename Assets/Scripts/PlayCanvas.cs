using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayCanvas : MonoBehaviour
{
    public TMP_InputField chatField;
    public TextMeshProUGUI chatLog;

    public PlayerMove player;

    void Awake()
    {
        Client.instance.chatBox = chatLog;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            player.ToggleStop();
            if(chatField.gameObject.activeSelf)
            {
                EnterChat();
                chatField.gameObject.SetActive(false);
            }
            else
            {
                chatField.gameObject.SetActive(true);
                chatField.ActivateInputField();
            }
        }
    }

    public void EnterChat()
    {
        if (chatField.text.Length == 0)
        {
            return;
        }
        Client.instance.Chat(chatField.text);
        chatField.text = "";
        chatField.Select();
    }
}
