using UnityEngine;
using System.Collections;

public struct URLParam
{
    public string paramID;
    public string paramValue;
}

public class HTTPHelper : MonoBehaviour
{
    public static HTTPHelper _instance;

    public const string DEFAULT_HOST = "ec2-34-248-183-89.eu-west-1.compute.amazonaws.com";
    public const int DEFAULT_PORT = 8090;

    public const string COMMAND_RESET_RIDDLE_PROGRESS = "riddleProgressReset";
    public const string COMMAND_GET_RIDDLE_PROGRESS = "riddleProgress";
    public const string COMMAND_UPDATE_RIDDLE_PROGRESS = "updateRiddleProgress";

    public const string PARAM_ID_UPDATE_RIDDLE_PROGRESS = "stage";

    public static bool responseSuccessful = false;
    public static string responseError = "";
    public static string responseData = "";

    public static HTTPHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("HTTP Helper Singleton");
                _instance = go.AddComponent<HTTPHelper>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private static string ConstructURL(string host, int port, string command, URLParam[] parameters)
    {
        string url = host + ":" + port + "/" + command;

        if (parameters != null)
        {
            if (parameters.Length > 0)
            {
                url += "?";

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i != 0)
                    {
                        url += "&";
                    }

                    url += parameters[i].paramID + "=" + parameters[i].paramValue;
                }
            }
        }

        return url;
    }

    public static void GetRequest(string host, int port, string command, URLParam[] parameters)
    {
        string url = ConstructURL(host, port, command, parameters);

        Debug.Log(url);

        WWW www = new WWW(url);
        Instance.StartCoroutine(WaitForRequest(www));
    }

    static IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            responseSuccessful = true;
            responseError = "";
            responseData = www.text;
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
            responseSuccessful = false;
            responseError = www.error;
            responseData = "";
        }
    }
}