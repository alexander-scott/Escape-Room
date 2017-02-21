using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Button subButton;
    public Button controlsButton;
    public Button qrScannerButton;
    public Button sonarButton;

    public InputField ipAddressText;
    public Dropdown dropDownList;

    private void Start()
    {
        subButton.onClick.AddListener(SubButtonClicked);
        controlsButton.onClick.AddListener(ControlsButtonClicked);
        qrScannerButton.onClick.AddListener(QRButtonClicked);
        sonarButton.onClick.AddListener(SonarButtonClicked);

        ipAddressText.onValueChanged.AddListener(IPAddressChanged);
        dropDownList.onValueChanged.AddListener(PlayerSelectChanged);

        ipAddressText.text = GlobalVariables.ipAddress;
        dropDownList.value = GlobalVariables.playerNumber;
    }

    private void SubButtonClicked()
    {
        SceneManager.LoadScene("Test");
    }

    private void ControlsButtonClicked()
    {
        SceneManager.LoadScene("Controls");
    }

    private void QRButtonClicked()
    {
        SceneManager.LoadScene("QRScanner");
    }

    private void SonarButtonClicked()
    {
        SceneManager.LoadScene("Sonar");
    }

    public void IPAddressChanged(string ipaddress)
    {
        GlobalVariables.ipAddress = ipaddress;
        Debug.Log("IPAddress is now set to " + ipaddress);
    }

    public void PlayerSelectChanged(int val)
    {
        GlobalVariables.playerNumber = val;
        Debug.Log("This players number is now set to " + val);
    }
}
