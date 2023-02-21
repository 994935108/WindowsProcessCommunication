using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{

    public InputField receviceInputField;
    public InputField msgInputField;

    public Text receiveMsgText;

    public Button sendMsg;

    private void Start()
    {
        sendMsg.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(msgInputField.text)) {
               
                WindowsProcessCommunication.SendMessage(msgInputField.text, WindowsProcessCommunication.FindWindow(null, receviceInputField.text));
            }
        });
        WindowsProcessCommunication.HookLoad(ReciveMsgCallback);
    }

    public void ReciveMsgCallback(string msg) {
        Debug.LogError("�յ���Ϣ:" + msg);

        receiveMsgText.text ="�յ���Ϣ��"+ msg;


    }

    private void OnDestroy()
    {
        WindowsProcessCommunication.UnhookWindowsHookEx();
    }

}
