using System;
using System.Net;

namespace RobotDotNet {
    class _ {
        static void error(params string[] messages) {
            Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static void info(params string[] messages) {
            Console.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static void startServer(HttpListener listener, string serverAddress) {
            listener.Prefixes.Add($"{serverAddress}/");

            try {
                listener.Start();
                info($"The RobotDotNet keyword server is running on {serverAddress}. To stop it press Ctrl+C.",
                    "- To use another port pass the option -p [PORT].");
            } catch (Exception e) {
                error("The RobotDotNet keyword server could not be started.",
                      $"Error: {e.Message}");
                Environment.Exit(1);
            }
        }

        public static void Main(string[] args) {
            const string host = "http://127.0.0.1";
            var options = new Options(args);
            var serverAddress = $"{host}:{options.port}";
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options);
            startServer(httpListener, serverAddress);

            if (options.debug) {
                info("========== DEBUG-Mode active ==========");
            } else {
                info("- To activate the debug mode pass the option --debug.");
            }

            while (httpListener.IsListening) {
                var context = httpListener.GetContext();
                var url = context.Request.Url;

                if (url == null) {
                    if (options.debug) {
                        error("The HTTP request contains no URL.");
                    }
                    continue;
                }

                var endpoint = String.Join("", url.Segments);
                robotRemote.ProcessRequest(context);
            }
        }
    }
}
