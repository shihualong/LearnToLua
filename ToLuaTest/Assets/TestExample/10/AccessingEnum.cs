using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class AccessingEnum : MonoBehaviour {

	string script = 
	@"
	   space = nil
	   function TestEnum(e)
	      print('Enum is:'..tostring(e))
		  
		  if space:ToInt() == 0 then
		      print('enum ToInt() is Ok')
		  end

		  if not space:Equals(0) then
		      print('enum compare int is ok ')
		  end

		  if space == e then
		      print('enum compare enum is ok')
		  end
           
		  local s = UnityEngine.Space.IntToEnum(0)
		  if space == s then
		      print('IntToEnum change type is ok')
		  end
	   end

	   function ChangeLightType(light,type)
	      print('change light type to ' .. tostring(type))
		  light.type = type
	   end

	";

	LuaState state = null;
	void Start () {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
		state = new LuaState();
		state.Start();
		LuaBinder.Bind(state);
		state.DoString(script);
        state["space"] = Space.World;

		LuaFunction func = state.GetFunction("TestEnum");
		func.BeginPCall();
		func.Push(Space.World);
		func.PCall();
		func.EndPCall();
		func.Dispose();
		func = null;
	}

	private void OnApplicationQuit() {
		state.CheckTop();
		state.Dispose();
		state = null;
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif 
	}
    string tips = null;
	int count = 1;
    private void ShowTips(string condition, string stackTrace, LogType type)
    {
        tips += condition;
		tips += "\r\n";
    }
    private void OnGUI() {
		 GUI.Label(new Rect(Screen.width / 2 - 300 ,Screen.height / 2 - 200,600,400),tips);

		 if(GUI.Button(new Rect(0,60,120,50),"ChangeType"))
		 {
			 GameObject go = GameObject.Find("Light");
			 Light l = go.GetComponent<Light>();
			 LuaFunction func  = state.GetFunction("ChangeLightType");
			 func.BeginPCall();
			 func.Push(l);
			 LightType type = (LightType)(count++ % 4);
			 func.Push(type);
			 func.PCall();
			 func.EndPCall();
			 func.Dispose();

		 }
	}
    // Update is called once per frame
    void Update () {
	
	}
}
