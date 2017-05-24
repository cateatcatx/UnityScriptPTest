using System.Collections;
using System.IO;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;

public class PTest : MonoBehaviour {

    public static int _V0;

    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    //public AppDomain appdomain;
    bool inited = false;
    TestItem[] testItems;

    public WaitForSeconds ws = new WaitForSeconds(2);
    public string logText = "";
    public int runCount;

#if ILRuntime
    public ILRuntime.Runtime.Enviorment.AppDomain appdomain;
#else
    public System.Reflection.Assembly assembly;
#endif


    void Start()
    {
        Application.logMessageReceived += this.log;

        runCount = 10;

        testItems = new TestItem[17];
        for (int i = 0; i <= 10; ++i)
        {
            if (i == 0 || i == 1 || i == 10)
            {
                testItems[i] = new TestDllWithTrans(this, i, transform);
            }
            else
            {
                testItems[i] = new TestDll(this, i);
            }
        }
        testItems[11] = new TestEmptyFunc(this, 11);
        testItems[12] = new TestGetValue(this, 12, "_V0");
        testItems[13] = new TestGetValue(this, 13, "_V1");
        testItems[14] = new TestGetValue(this, 14, "_V2");
        testItems[15] = new TestGetValue(this, 15, "_V3");
        testItems[16] = new TestGetValue(this, 16, "_V4");


        StartCoroutine(Init());
    }

#if ILRuntime
    IEnumerator Init()
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译PTestDll.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/PTestDll.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/PTestDll.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/PTestDll.pdb");
#else
        www = new WWW("file:///" + Application.streamingAssetsPath + "/PTestDll.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        using (System.IO.MemoryStream fs = new MemoryStream(dll))
        {
            using (System.IO.MemoryStream p = new MemoryStream(pdb))
            {
                appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }

        ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);

        OnHotFixLoaded();
    }
#else
    IEnumerator Init()
    {
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/PTestDll.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/PTestDll.dll");
#endif

        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);

        assembly = System.Reflection.Assembly.Load(www.bytes);

        OnHotFixLoaded();
    }
#endif

    void log(string cond, string trace, LogType lt)
    {
        logText += cond;
        logText += "\n";
    }

    void OnHotFixLoaded()
    {
        inited = true;

#if ILRuntime
        appdomain.Invoke("PTest", "Init", null, null);
#else
        InvokeStatic("PTest", "Init", null);
#endif
    }

#if !ILRuntime
    public object InvokeStatic(string type, string name, params object[] p)
    {
        System.Type t = assembly.GetType(type);
        var m = t.GetMethod(name);
        return m.Invoke(null, p);
    }
#endif

#if !ILRuntime
    public System.Reflection.MethodBase GetStaticMethod(string type, string name)
    {
        System.Type t = assembly.GetType(type);
        return t.GetMethod(name);
    }
#endif

    public void GC()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        print("GC Done!");
    }


    void OnGUI()
    {
        if (!inited)
        {
            return;
        }

        GUI.Label(new Rect(400, 0, 500, 1000), logText);

        if (GUI.Button(new Rect(10, 10, 100, 50), "GC"))
        {
            GC();
        }

        if (GUI.Button(new Rect(130, 10, 100, 50), "Clear Screen"))
        {
            logText = "";
        }


        int[] rows = { 10, 110, 210 };
        int[] cols = { 0, 0, 70 };
        int col = 80;
        for (int i = 0; i < testItems.Length; ++i)
        {
            if (GUI.Button(new Rect(rows[i % 3], col, 100, 50), "Test" + i))
            {
                StartCoroutine(testItems[i].Test());
            }

            col += cols[i % 3];
        }
    }
}
