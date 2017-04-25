using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public InputField usernameInputField;
    public InputField hostPortInputField;
    public InputField joinPortInputField;
    public InputField joinIpAddressInputField;

    void Start()
    {
        CursorHelper.CursorLocked = false;

        LoadPrefs();
    }

    void Update()
    {
        Camera cam = Camera.main;
        Vector3 rot = cam.transform.eulerAngles;
        rot.y = 101.44f - Mathf.Sin(Time.time / 40.0f) * 50f;
        cam.transform.eulerAngles = rot;
    }

    public void Host()
    {
        SavePrefs();
        NetworkManager.singleton.StartHost();
    }

    public void Join()
    {
        SavePrefs();
        NetworkManager.singleton.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetIPAddress(string val)
    {
        NetworkManager.singleton.networkAddress = val;
    }

    public void SetPort(string val)
    {
        int result;
        if (int.TryParse(val, out result))
        {
            NetworkManager.singleton.networkPort = result;
        }
    }

    private void LoadPrefs()
    {
        usernameInputField.text = PlayerPrefs.GetString("username", "");
        hostPortInputField.text = PlayerPrefs.GetString("host_port", NetworkManager.singleton.networkPort.ToString());
        joinPortInputField.text = PlayerPrefs.GetString("join_port", NetworkManager.singleton.networkPort.ToString());
        joinIpAddressInputField.text = PlayerPrefs.GetString("join_ip_address", NetworkManager.singleton.networkAddress);
    }

    private void SavePrefs()
    {
        PlayerPrefs.SetString("username", usernameInputField.text);
        PlayerPrefs.SetString("host_port", hostPortInputField.text);
        PlayerPrefs.SetString("join_port", joinPortInputField.text);
        PlayerPrefs.SetString("join_ip_address", joinIpAddressInputField.text);
        PlayerPrefs.Save();
    }
}
