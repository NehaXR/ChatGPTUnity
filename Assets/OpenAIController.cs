 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using OpenAI_API.Models;
using OpenAI_API.Images;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textfield;
    public TMP_InputField inputfield;
    public Button ok;
    public Image image;

    private OpenAIAPI api;
    private List<ChatMessage> messages;
    // Start is called before the first frame update
    void Start()
    {
        api = new OpenAIAPI("your api key");

        StartConversation();
        ok.onClick.AddListener(() => GetResponse());
    }

    

    private void StartConversation()
    {
        messages = new List<ChatMessage> { new ChatMessage(ChatMessageRole.System, "Hello! ask jokes") 
        };

        inputfield.text = "";
        string startString = "you are awesome";
        textfield.text = startString;
        // Debug.Log(startString);
        
    }

    private async void GetResponse()
    {
        if (inputfield.text.Length < 1)
        {
            return;
        }

        //Disable the ok button

        ok.enabled = false;

        //fill the user msg from input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputfield.text;

        if (userMessage.Content.Length > 100)
        {
            //Limit responses to 100 charater

            userMessage.Content = userMessage.Content.Substring(0, 100);
        }
        //Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        //add msg to the list
        messages.Add(userMessage);
        //update 
        textfield.text = string.Format("you: {0}", userMessage.Content);
        
       

        //clear
        inputfield.text = "";

        //send the entire chat

        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature=0.1,
            MaxTokens=50,
            Messages=messages
        });

        //Get the response msg

        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        //Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        //add response

        messages.Add(responseMessage);

        //update the text field
        textfield.text = string.Format("you: {0}\n\nBoy: {1}", userMessage.Content, responseMessage.Content);

        //renable

        ok.enabled = true;

    }


}
