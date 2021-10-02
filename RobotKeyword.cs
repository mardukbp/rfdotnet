using System;

namespace RobotDotNet {
    public class Keyword : Attribute {
        public string Name;
        
        public Keyword(string name) {
            Name = name;
        }
    }

    public class Doc : Attribute {
        public string DocString;

        public Doc(string docString) {
            DocString = docString;
        }
    }

    public record RobotKeyword(string name, string method, string[] args, string doc) {}
}
