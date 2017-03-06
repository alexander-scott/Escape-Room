using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour {

    [SerializeField]
    private int[] numbers = new int[4] { 1, 2, 3, 4 };
    //private int[] enteredNumbers = new int[4];
    List<int> enteredNumbers = new List<int>();

    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button btn5;
    public Button btn6;
    public Button btn7;
    public Button btn8;
    public Button btn9;
    public Button btn0;
    public Button btnEnt;
    public Button btnEsc;

    public Text display1;
    public Text display2;
    public Text display3;
    public Text display4;
    public Text result;

    // Use this for initialization
    void Start () {
        btn1.onClick.AddListener(delegate { ButtonPressed(btn1); });
        btn2.onClick.AddListener(delegate { ButtonPressed(btn2); });
        btn3.onClick.AddListener(delegate { ButtonPressed(btn3); });
        btn4.onClick.AddListener(delegate { ButtonPressed(btn4); });
        btn5.onClick.AddListener(delegate { ButtonPressed(btn5); });
        btn6.onClick.AddListener(delegate { ButtonPressed(btn6); });
        btn7.onClick.AddListener(delegate { ButtonPressed(btn7); });
        btn8.onClick.AddListener(delegate { ButtonPressed(btn8); });
        btn9.onClick.AddListener(delegate { ButtonPressed(btn9); });
        btn0.onClick.AddListener(delegate { ButtonPressed(btn0); });
        btnEnt.onClick.AddListener(delegate { ButtonPressed(btnEnt); });
        btnEsc.onClick.AddListener(delegate { ButtonPressed(btnEsc); });
    }


    void ButtonPressed(Button button)
    {
        string name = button.name;

        switch(name)
        {
            case "1":
                enteredNumbers.Add(1);
                break;
            case "2":
                enteredNumbers.Add(2);
                break;
            case "3":
                enteredNumbers.Add(3);
                break;
            case "4":
                enteredNumbers.Add(4);
                break;
            case "5":
                enteredNumbers.Add(5);
                break;
            case "6":
                enteredNumbers.Add(6);
                break;
            case "7":
                enteredNumbers.Add(7);
                break;
            case "8":
                enteredNumbers.Add(8);
                break;
            case "9":
                enteredNumbers.Add(9);
                break;
            case "0":
                enteredNumbers.Add(0);
                break;
            case "ent":
                checkNumbers();
                break;
            case "esc":
                enteredNumbers.Clear();
                break;
        }

        if (enteredNumbers.Count == 4)
        {
            if(checkNumbers())
            {
                result.text = "Access Granted!";
                result.color = Color.green;
                result.enabled = true;
                //Send message to Alex's stuff               
            }
            else
            {
                result.text = "Access Denied!";
                result.color = Color.red;
                result.enabled = true;
                enteredNumbers.Clear();
            }
        }
    }

    bool checkNumbers()
    {
        if (enteredNumbers.Count != 4)
        {
            return false;
        }

        for (int i = 0; i < enteredNumbers.Count; i++)
        {
            if (numbers[i] != enteredNumbers[i])
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update () {

        int size = enteredNumbers.Count;

        switch (size)
        {
            case 0:
                display1.text = "-";
                display2.text = "-";
                display3.text = "-";
                display4.text = "-";
                break;
            case 1:
                display1.text = enteredNumbers[0].ToString();
                display2.text = "-";
                display3.text = "-";
                display4.text = "-";
                break;
            case 2:
                display1.text = enteredNumbers[0].ToString();
                display2.text = enteredNumbers[1].ToString();
                display3.text = "-";
                display4.text = "-";
                break;
            case 3:
                display1.text = enteredNumbers[0].ToString();
                display2.text = enteredNumbers[1].ToString();
                display3.text = enteredNumbers[2].ToString();
                display4.text = "-";
                break;
            case 4:
                display1.text = enteredNumbers[0].ToString();
                display2.text = enteredNumbers[1].ToString();
                display3.text = enteredNumbers[2].ToString();
                display4.text = enteredNumbers[3].ToString();
                break;
        }

    }
}
