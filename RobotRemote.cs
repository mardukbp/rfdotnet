using System;
using Horizon.XmlRpc.Server;
using Horizon.XmlRpc.Core;

namespace RobotDotNet {
    public sealed class RobotRemote : XmlRpcListenerService {
        KeywordLibrary keywordLibrary;
        int port;
        bool debug;

        public RobotRemote (Options options) {
            this.debug = options.debug;
            this.port = options.port;
            this.keywordLibrary = new KeywordLibrary();
        }

        [XmlRpcMethod("get_keyword_names")]
        public string[] getKeywordNames() {
            keywordLibrary = new KeywordLibrary();
            var keywordNames = keywordLibrary.getKeywordNames();

            if (debug) {
                Console.WriteLine(string.Join(Environment.NewLine, 
                                  "", "Available Keywords:", new String('-', "Available Keywords:".Length), "", 
                                  string.Join(Environment.NewLine, keywordNames), ""));
            }
            
            return keywordNames;
        }

        [XmlRpcMethod("run_keyword")]
        public XmlRpcStruct runKeyword(string keywordName, object[] args) {
            if (debug) {
                Console.WriteLine($"Keyword: {keywordName}");
                Console.WriteLine("Arguments: " + string.Join(", ", args));
            }

            var result = new RobotResult();
            var keyword = keywordLibrary.getKeyword(keywordName);
            var method = typeof(KeywordLibrary).GetMethod(keyword.method);
            
            try {
                // To Do: Verify that the type of the arguments sent by Robot Framework
                // corresponds to the type expected by the method behind the keyword
                var returnValue = method?.Invoke(keywordLibrary, args);
                if (returnValue != null) {
                    result = (RobotResult)returnValue;
                }
            } catch (Exception e) {
                result = new ExceptionError(e, $"The execution of the keyword '{keywordName}' failed.");
            }

            return result.asXmlRpcStruct();
        }

        [XmlRpcMethod("get_keyword_arguments")]
        public string[] getKeywordArguments(string keyword) {
            return keywordLibrary.getKeyword(keyword).args;
        }

        [XmlRpcMethod("get_keyword_documentation")]
        public string getKeywordDocumentation(string keyword) {
            return keyword switch {
                "__intro__" => "RobotDotNet: RobotFramework meets C#",
                
                "__init__" => String.Join(Environment.NewLine, 
                    $"In order to use this library add the following entry to the ``Settings`` table in a .robot file:",
                    $"| ``Library   Remote   http://127.0.0.1:{port}/   WITH NAME   RobotDotNet``"),
                    
                _ => keywordLibrary.getKeyword(keyword).doc
            };
        }
    }
}
