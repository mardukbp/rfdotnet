using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RobotDotNet {
    public class KeywordLibrary {
        List<RobotKeyword> keywords;

        public KeywordLibrary() {
            keywords = new List<RobotKeyword>();
            typeof(KeywordLibrary)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .ToList()
                .ForEach(methodInfo => {
                    var attrKeyword = methodInfo.GetCustomAttribute(typeof(Keyword));
                    var attrDoc = methodInfo.GetCustomAttribute(typeof(Doc));
                    if (attrKeyword != null && attrDoc != null) {
                        var name = ((Keyword)attrKeyword).Name;
                        var method = methodInfo.Name;
                        var args = methodInfo.GetParameters()
                                             .Select(param => param.Name ?? "").ToArray();
                        var doc = ((Doc)attrDoc).DocString;
                        keywords.Add(new RobotKeyword(name, method, args, doc));
                    }
                });
        }

        public RobotKeyword getKeyword(string name) {
            return keywords.Single(keyword => keyword.name == name);
        }

        public string[] getKeywordNames() {
            return keywords.Select(keyword => keyword.name).ToArray();
        }

        [Keyword("Say Hello To"),
         Doc("Salute the person with the specified name.\n\n" + 
             "| ``Say Hello To    name``")]
        public RobotResult say_hello_to(string name) {
            Console.WriteLine($"Hello {name}");
            return new Success($"Said hello to {name}");
        }
    }
}
