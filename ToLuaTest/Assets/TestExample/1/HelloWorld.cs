using UnityEngine;
using System.Collections;
using LuaInterface;
public class HelloWorld : MonoBehaviour
{

    /*
	1.使用Tolua的相关类及方法都需要调用命名空间LuaInterface
	2.调用lua脚本必须要先创建一个lua的虚拟机，而创建的步骤就是LuaState lua = new LuaState();，
	创建一个LuaState型的对象，这个就是lua的虚拟机，后面的lua的与C#的交互全部依仗这个东西。
    3.lua中直接运行一段lua脚本最简单的方法就是 lua.DoString
	4.使用完lua虚拟机之后记得要销毁，具体操作如下：先进行lua虚拟机栈的判空，具体对应的就是lua.CheckTop ，
	  然后就是析构掉lua虚拟机，具体方法为lua.Dispose
	*/ 
    private void Awake()
    {
	   LuaState  lua = new LuaState();
	   lua.Start();
	   string hello = @"print('hello tolua#')";
	   string hello1 = "print('what')";
	   lua.DoString(hello1,"HelloWorld.cs");
	   lua.CheckTop();
	   lua.Dispose();
	   lua = null;
    }
}
