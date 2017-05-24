using System.Collections;
using UnityEngine;
using LuaInterface;

public class PTest : MonoBehaviour {
    public LuaState state = null;
    public string logText = "";
    public WaitForSeconds ws = new WaitForSeconds(2);
    public int runCount;

    TestItem[] testItems;

    LuaFunction gcFunc;


    void Awake()
    {
        new LuaResLoader();
        state = new LuaState();
        state.Start();
        LuaBinder.Bind(state);

        Application.logMessageReceived += ShowTips;
    }

    void Start()
    {
        runCount = 10;

        testItems = new TestItem[17];
        for (int i=0; i<=10; ++i)
        {
            testItems[i] = new TestLua(this, i, transform);
        }
        testItems[11] = new TestEmptyFunc(this, 11);
        testItems[12] = new TestGetLuaValue(this, 12, "_V0");
        testItems[13] = new TestGetLuaValue(this, 13, "_V1");
        testItems[14] = new TestGetLuaValue(this, 14, "_V2");
        testItems[15] = new TestGetLuaValue(this, 15, "_V3");
        testItems[16] = new TestGetLuaValue(this, 16, "_V4");


        state.DoFile("TestPerf.lua");
        gcFunc = state.GetFunction("GC");
    }

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        logText += msg;
        logText += "\r\n";
    }

    IEnumerator TestLua(string luaFuncName, params object[] args)
    {
        logText += luaFuncName + " Begin:\n";
        double totalMS = 0;
        LuaFunction f = state.GetFunction(luaFuncName);
        int count = 1;
        for (int i = 1; i <= count; ++i)
        {
            GC();
            yield return ws;

            object[] rets = f.Call(args);
            double t = (double)rets[0] * 1000;
            totalMS += t;
            logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        logText += string.Format("{0} complete average ms: {1}\n", luaFuncName, totalMS / count);
    }

    IEnumerator TestEmptyFunc()
    {
        LuaFunction f = state.GetFunction("EmptyFunc");
        int count = 10;
        double totalMS = 0;
        for (int i = 1; i <= count; ++i)
        {
            GC();
            yield return ws;

            long ts = System.DateTime.Now.Ticks;
            for (int j=0; j<200000; ++j)
            {
                f.Call();
            }
            double t = (double)((System.DateTime.Now.Ticks - ts) / 10000.0);

            totalMS += t;
            logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        logText += string.Format("TestEmptyFunc complete average ms: {0}\n", totalMS / count);
    }


    public void GC()
    {
        gcFunc.Call();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        gcFunc.Call();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        print("GC Done!");
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "GC"))
        {
            GC();
        }

        if (GUI.Button(new Rect(130, 10, 100, 50), "Version"))
        {
            state.GetFunction("Version").Call();
        }

        if (GUI.Button(new Rect(10, 80, 100, 50), "Jit Switch"))
        {
            state.GetFunction("JitSwitch").Call();
        }

        if (GUI.Button(new Rect(130, 80, 100, 50), "Clear Screen"))
        {
            logText = "";
        }


        int[] rows = { 10, 110, 210 };
        int[] cols = { 0, 0, 70 };
        int col = 150;
        for (int i=0; i<testItems.Length; ++i)
        {
            if (GUI.Button(new Rect(rows[i % 3], col, 100, 50), "Test" + i))
            {
                StartCoroutine(testItems[i].Test());
            }

            col += cols[i % 3];
        }

        


        GUI.Label(new Rect(400, 0, 500, 1000), logText);
    }
}
