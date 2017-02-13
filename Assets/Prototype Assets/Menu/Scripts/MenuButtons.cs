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

    public InputField ipAddressText;
    public Dropdown dropDownList;

    private void Start()
    {
        subButton.onClick.AddListener(SubButtonClicked);
        controlsButton.onClick.AddListener(ControlsButtonClicked);
        qrScannerButton.onClick.AddListener(QRButtonClicked);

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

    public void IPAddressChanged(string ipaddress)
    {
        GlobalVariables.ipAddress = ipaddress;
    }

    public void PlayerSelectChanged(int val)
    {
        GlobalVariables.playerNumber = val;
    }
}
