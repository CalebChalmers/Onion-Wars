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

    private NetworkManager manager;

    void Start()
    {
        manager = NetworkManager.singleton;

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
        manager.StartHost();
    }

    public void Join()
    {
        SavePrefs();
        manager.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetIPAddress(string val)
    {
        manager.networkAddress = val;
    }

    public void SetPort(string val)
    {
        int result;
        if (int.TryParse(val, out result))
        {
            manager.networkPort = result;
        }
    }

    private void LoadPrefs()
    {
        usernameInputField.text = PlayerPrefs.GetString("username", "");
        hostPortInputField.text = PlayerPrefs.GetString("host_port", manager.networkPort.ToString());
        joinPortInputField.text = PlayerPrefs.GetString("join_port", manager.networkPort.ToString());
        joinIpAddressInputField.text = PlayerPrefs.GetString("join_ip_address", manager.networkAddress);
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
