using UnityEngine;
using XLua;
using System.Collections;
using System;

public class PTest : MonoBehaviour
{
    public string logText = "";
    public WaitForSeconds ws = new WaitForSeconds(2);
    public int runCount;

    public TextAsset luaScript;
    public LuaEnv luaEnv;
    public LuaTable scriptEnv;

    TestItem[] testItems;
    Action mGC;
    Action mJitSwitch;
    Action mVersion;

    void Start()
    {
        Application.logMessageReceived += this.log;

        luaEnv = new LuaEnv();
        scriptEnv = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        luaEnv.DoString(luaScript.text, "LuaBehaviour", scriptEnv);

        scriptEnv.Get("GC", out mGC);
        scriptEnv.Get("JitSwitch", out mJitSwitch);
        scriptEnv.Get("Version", out mVersion);

        runCount = 10;

        testItems = new TestItem[17];
        for (int i = 0; i <= 10; ++i)
        {
            testItems[i] = new TestLua(this, i, transform);
        }
        testItems[11] = new TestEmptyFunc(this, 11);
        testItems[12] = new TestGetLuaValue(this, 12, "_V0");
        testItems[13] = new TestGetLuaValue(this, 13, "_V1");
        testItems[14] = new TestGetLuaValue(this, 14, "_V2");
        testItems[15] = new TestGetLuaValue(this, 15, "_V3");
        testItems[16] = new TestGetLuaValue(this, 16, "_V4");
    }

    void log(string cond, string trace, LogType lt)
    {
        logText += cond;
        logText += "\n";
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "GC"))
        {
            GC();
        }

        if (GUI.Button(new Rect(130, 10, 100, 50), "Version"))
        {
            mVersion();
        }

        if (GUI.Button(new Rect(10, 80, 100, 50), "Jit Switch"))
        {
            mJitSwitch();
        }

        if (GUI.Button(new Rect(130, 80, 100, 50), "Clear Screen"))
        {
            logText = "";
        }

        int[] rows = { 10, 110, 210 };
        int[] cols = { 0, 0, 70 };
        int col = 150;
        for (int i = 0; i < testItems.Length; ++i)
        {
            if (GUI.Button(new Rect(rows[i % 3], col, 100, 50), "Test" + i))
            {
                StartCoroutine(testItems[i].Test());
            }

            col += cols[i % 3];
        }




        GUI.Label(new Rect(400, 0, 500, 1000), logText);
    }

    public void GC()
    {
        mGC();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        mGC();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        print("GC Done!");
    }
}
