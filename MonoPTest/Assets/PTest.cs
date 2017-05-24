using System;
using UnityEngine;

public class PTest : MonoBehaviour {
    public string logText;


    void Start()
    {
        Application.logMessageReceived += this.log;
    }


    void log(string cond, string trace, LogType lt)
    {
        logText += cond;
        logText += "\n";
    }


    public static void Test9()
    {
        int[] array = new int[1024];
        for (int i = 0; i < 1024; ++i)
        {
            array[i] = i;
        }

        int total = 0;
        long ts = DateTime.Now.Ticks;

        for (int j = 0; j < 100000; ++j)
        {
            for (int i = 0; i < 1024; ++i)
            {
                total = total + array[i];
            }
        }


        print((DateTime.Now.Ticks - ts) / 10000);
    }


    void OnGUI()
    {
        GUI.Label(new Rect(400, 0, 500, 1000), logText);

        if (GUI.Button(new Rect(10, 10, 100, 50), "GC"))
        {
            GC();
        }

        if (GUI.Button(new Rect(130, 10, 100, 50), "Clear Screen"))
        {
            logText = "";
        }

        if (GUI.Button(new Rect(10, 80, 100, 50), "Test9"))
        {
            Test9();
        }
    }

    public void GC()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        print("GC Done!");
    }
}
