using System;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Horizon.XmlRpc.Server;
using Horizon.XmlRpc.Core;

namespace RobotRemote {
  class _ {
    
    public static void Main(string[] args) {
      var httpListener = new HttpListener();
      var serverAddress = "http://127.0.0.1:8270/";
      httpListener.Prefixes.Add(serverAddress);
      httpListener.Start();
      Console.WriteLine($"Server listening on {serverAddress}. Press Ctrl+C to stop it.");
      
      while (true) {
        HttpListenerContext httplistenerContext = httpListener.GetContext();
        XmlRpcListenerService xmlrpcServer = new XMLRPCServer();
        xmlrpcServer.ProcessRequest(httplistenerContext);
      }
    }
  }

  public class Keyword : Attribute {
    public string Name;

    public Keyword(string name) {
      Name = name;
    }
  }
  public static class Keywords {
    [Keyword("Say Hello To")]
    public static void salute(string name) {
      System.Console.WriteLine($"Hello {name}");
    }
  }

  public class XMLRPCServer : XmlRpcListenerService {
    Dictionary<string, string> methodFromKeyword;

    public XMLRPCServer () {
      methodFromKeyword = new Dictionary<string, string>();
      MethodInfo[] methodsInfo = typeof(Keywords).GetMethods(BindingFlags.Public | BindingFlags.Static);
      foreach (MethodInfo methodInfo in methodsInfo) {
        var methodName = methodInfo.Name;
        var keywordName = ((Keyword)methodInfo.GetCustomAttribute(typeof(Keyword))).Name;
        methodFromKeyword.Add(keywordName, methodName);
      }
    }

    [XmlRpcMethod("get_keyword_names")]
    public string[] getKeywordNames() {
      return methodFromKeyword.Keys.ToArray();
    }

    [XmlRpcMethod("run_keyword")]
    public XmlRpcStruct runKeyword(string keyword, object[] args) {
      Console.WriteLine($"Robot Framework requested the keyword '{keyword}'");
      Console.WriteLine("Arguments: " + string.Join(",", args));
      typeof(Keywords).GetMethod(methodFromKeyword[keyword]).Invoke(null, args);

      var result = new XmlRpcStruct();
      result.Add("status", "PASS");
      result.Add("output", $"Keyword '{keyword}' was executed successfully");
      result.Add("return", "");
      result.Add("error", "");
      return result;
    }
  }
}