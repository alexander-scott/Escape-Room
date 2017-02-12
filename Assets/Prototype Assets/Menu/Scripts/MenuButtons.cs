using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    public Button subButton;
    public Button controlsButton;
    //public Button subControls;

    private void Start()
    {
        subButton.onClick.AddListener(SubButtonClicked);
        controlsButton.onClick.AddListener(ControlsButtonClicked);
    }

    private void SubButtonClicked()
    {
        SceneManager.LoadScene("Test");
    }

    private void ControlsButtonClicked()
    {
        SceneManager.LoadScene("Controls");
    }
}
