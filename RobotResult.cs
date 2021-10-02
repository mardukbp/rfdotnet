using System;
using Horizon.XmlRpc.Core;

namespace RobotDotNet {
    public class RobotResult {
        const string None = "";
        public const string PASS = "PASS";
        public const string FAIL = "FAIL";
        public string status = "";
        public string output = "";
        public string returnValue = None;
        public string error = "";
        public string stacktrace = "";

        public XmlRpcStruct asXmlRpcStruct() {
            var result = new XmlRpcStruct();
            result.Add("status", status);
            result.Add("output", output);
            result.Add("return", returnValue);
            result.Add("error", error);
            result.Add("traceback", stacktrace);
            return result;
        }
    }

    public sealed class Success : RobotResult {
        public Success(string returnValue, string message) {
            this.returnValue = returnValue;
            this.output = $"*INFO* {message}";
            this.status = PASS;
        }

        public Success(string message) {
            this.output = $"*INFO* {message}";
            this.status = PASS;
        }
    }

    public sealed class ExceptionError : RobotResult {
        const string errorDEBUG = "For more details execute: robot --loglevel DEBUG file.robot and take a look at log.html.";
        public ExceptionError(Exception e, string errorMessage) {
            this.output = $"*ERROR* {errorMessage}\n{errorDEBUG}";
            this.error = $"{e.Message}";
            this.stacktrace = $"{e.StackTrace}";
            this.status = FAIL;
        }
    }
}
