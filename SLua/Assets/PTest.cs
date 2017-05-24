using UnityEngine;
using SLua;
using System.Collections;

public class PTest : MonoBehaviour {
    public LuaSvr l;
    public string logText = "";
    public WaitForSeconds ws = new WaitForSeconds(2);
    public int runCount;

    TestItem[] testItems;

    void Start()
    {
        Application.logMessageReceived += this.log;

        l = new LuaSvr();
        l.init(null, () =>
        {
            l.start("perf");
        });

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
            l.luaState.getFunction("Version").call();
        }

        if (GUI.Button(new Rect(10, 80, 100, 50), "Jit Switch"))
        {
            l.luaState.getFunction("JitSwitch").call();
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
        l.luaState.getFunction("GC").call();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        l.luaState.getFunction("GC").call();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        print("GC Done!");
    }
}
