using System;
using UnityEngine;

public class PTest
{
    public static int _V0 = 1;
    public static string _V1 = "12345";
    public static GameObject _V2;
    public static Vector3 _V3;
    public static System.ArgumentException _V4;

    public static void Init()
    {
        _V2 = new GameObject();
        _V3 = new Vector3(1, 2, 3);
        _V4 = new ArgumentException();
    }

    public static void EmptyFunc()
    {
        _V0 = _V0 + 1;
    }

    public static double Test0(Transform trans)
    {
        long ts = DateTime.Now.Ticks;

        for (int i=0; i<200000; ++i)
        {
            trans.position = trans.position;
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test1(Transform trans)
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 200000; ++i)
        {
            trans.Rotate(Vector3.up, 1);
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test2()
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 2000000; ++i)
        {
            Vector3 v = new Vector3(i, i, i);
            float x = v.x;
            float y = v.y;
            float z = v.z; 
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test3()
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 20000; ++i)
        {
            GameObject go = new GameObject();
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test4()
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 20000; ++i)
        {
            GameObject go = new GameObject();
            go.AddComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer c = go.GetComponent<SkinnedMeshRenderer>();
            c.receiveShadows = false;
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test5()
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 200000; ++i)
        {
            Vector3 p = Input.mousePosition;
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test6()
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 200000; ++i)
        {
            Vector3 v = new Vector3(i, i, i);
            Vector3.Normalize(v);
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test7()
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 200000; ++i)
        {
            Quaternion q1 = Quaternion.Euler(i, i, i);
            Quaternion q2 = Quaternion.Euler(i * 2, i * 2, i * 2);
            Quaternion.Slerp(Quaternion.identity, q1, 0.5f);
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }


    public static double Test8()
    {
        int total = 0;
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 1000000; ++i)
        {
            total = total + i - (i / 2) * (i + 3) / (i + 5);
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test9()
    {
        int[] array = new int[1024];
        for (int i = 0; i < 1024; ++i)
        {
            array[i] = i;
        }

        int total = 0;
        long ts = DateTime.Now.Ticks;

        for (int j=0; j< 100000; ++j)
        {
            for (int i = 0; i < 1024; ++i)
            {
                total = total + array[i];
            }
        }
        

        return (DateTime.Now.Ticks - ts) / 10000;
    }

    public static double Test10(Transform trans)
    {
        long ts = DateTime.Now.Ticks;

        for (int i = 0; i < 200000; ++i)
        {
            UserClass.TestFunc1(1, "123", trans.position, trans);
        }

        return (DateTime.Now.Ticks - ts) / 10000;
    }
}
