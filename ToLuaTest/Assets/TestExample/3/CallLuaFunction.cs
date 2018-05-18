using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
public class CallLuaFunction : MonoBehaviour
{

    private string script =
    @"function luaFunc(num)
	    return num + 1
	  end
	  
	  test = {}
	  test.luaFunc = luaFunc";
    LuaFunction luaFunc = null;
    LuaState lua = null;
    string tips = null;
    void Start()
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        lua = new LuaState();
        lua.Start();
        DelegateFactory.Init();
        lua.DoString(script);

        //Get the Function object
        luaFunc = lua.GetFunction("test.luaFunc");
        if (luaFunc != null)
        {
            int num = luaFunc.Invoke<int, int>(123456);
            Debugger.Log("generic call return : {0}", num);
            num = CallFunc();
            Debugger.Log("expansion call return: {0}", num);
            num = lua.Invoke<int, int>("test.luaFunc", 123456, true);
            Debugger.Log("luastate call return : {0}", num);
        }
        lua.CheckTop();
    }
    private void ShowTips(string meg, string stackTrace, LogType type)
    {
        tips += meg;
        tips += "\r\n";
    }
#if !TEST_GC
    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300), tips);

    }
#endif
    private void OnDestroy()
    {
        if(luaFunc != null)
		{
			luaFunc.Dispose();
			luaFunc = null;
		}
		lua.Dispose();
		luaFunc = null;
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }
    private int CallFunc()
    {
        luaFunc.BeginPCall();
		luaFunc.Push(123456);
		luaFunc.PCall();
		int num = (int)luaFunc.CheckNumber();
		luaFunc.EndPCall();
		return num;
    }
}
