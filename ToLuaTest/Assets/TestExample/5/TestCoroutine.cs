using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class TestCoroutine : MonoBehaviour {

	public TextAsset luafile = null;
	private LuaState lua = null;
	private LuaLooper looper = null;
	private void Awake() {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif	
        new LuaResLoader();
		lua = new LuaState();
		lua.Start();
		LuaBinder.Bind(lua);
		DelegateFactory.Init();
		looper = gameObject.AddComponent<LuaLooper>();
		looper.luaState = lua;

		lua.DoString(luafile.text ,"TestLuaCoroutine.lua");
		LuaFunction f = lua.GetFunction("TestCortinue");
		f.Call();
		f.Dispose();
		f = null;
	}
    
	private void OnApplicationQuit() {
	    looper.Destroy();
		lua.Dispose();
		lua = null;	
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif	
	}

	string tips = null;
    private void ShowTips(string condition, string stackTrace, LogType type)
    {
        tips += condition;
		tips += "\r\n";
    }

    private void OnGUI() {
	    GUI.Label(new Rect(Screen.width / 2 - 300,Screen.height / 2  - 200 ,600,400),tips);

		if(GUI.Button(new Rect(50,50,120,45),"Start Counter"))
		{
			tips = null;
			LuaFunction func = lua.GetFunction("StartDelay");
			func.Call();
			func.Dispose();
		}
		else if (GUI.Button(new Rect(50,150,120,45),"Stop Counter"))
		{
			LuaFunction func = lua.GetFunction("StopDelay");
			func.Call();
			func.Dispose();
		}
		else if (GUI.Button(new Rect(50,250,120,45),"GC"))
		{
			lua.DoString("collectgarbage('collect')","TestCoroutine.cs");
			Resources.UnloadUnusedAssets();
		}
	}
}
