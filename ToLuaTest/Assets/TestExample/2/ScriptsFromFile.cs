using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
using System.IO;

//展示SearchPath 使用，require 与 dofile 区别
/*
1：调用lua.Start方法完成lua虚拟机的一些基础初始化，
里面的内容主要包括一些环境的配置和一些lua的基本库的加载，
默认一般工程中创建该虚拟机时都要初始化

2：重要方法lua.AddSearchPath ，通过此方法添加lua文件的路径，
只有添加了文件路径之后，在该路径上的lua文件才可以被读取

3：lua文件的2个加载方法，lua.DoFile ， lua.Require  ,
参数为lua文件名，推荐使用Require，
因为Require 读取文件是会检查该文件是否被加载过，
如果被加载过，则直接返回一个索引，否则则加载并返回一个索引，
而Dofile则是每次调用都会重新加载使用，
相对来说对性能等的消耗都会大一些，而且感觉不利于一些后面代码的书写
 */
public class ScriptsFromFile : MonoBehaviour
{
    LuaState lua = null;
    private string strLog = null;

    void Start()
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += Log;
#else
    Application.RegisterLogCallback(Log);
#endif
        lua = new LuaState();
        lua.Start();
        string fullPath = Application.dataPath +
        "\\TestExample/2";
        lua.AddSearchPath(fullPath);
    }


    private void Log(string meg, string stackTrace, LogType type)
    {
        strLog = meg;
        strLog = "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(100, Screen.height / 2 - 100, 600, 400), "Console:"+ strLog);
        if (GUI.Button(new Rect(50, 50, 120, 45), "DoFile"))
        {
            strLog = "";
            lua.DoFile("ScriptsFromFile.lua");
        }
        else if (GUI.Button(new Rect(50, 150, 120, 45), "Require"))
        {
            strLog = "";
            lua.Require("ScriptsFromFile");
        }
        lua.Collect();
        lua.CheckTop();

    }
    private void OnApplicationQuit()
    {
       lua.Dispose();
       lua = null;
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= Log;
#else
        Application.RegisterLogCallback(null);
#endif
    }
}







