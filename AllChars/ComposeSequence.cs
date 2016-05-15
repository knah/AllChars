using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Linq;

namespace AllCharsNS
{
    internal class UnicodeNames {
        private static UnicodeNames instance = new UnicodeNames();

        public static UnicodeNames Instance {
            get { return instance; }
        }

        private XmlDocument unDoc;
        private XPathNavigator lookup;

        private UnicodeNames()
        {
            unDoc = new XmlDocument();
            unDoc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("AllCharsNS.UnicodeNames.xml"));
            lookup = unDoc.CreateNavigator();
        }

        public string NameOf(char character) {
            string charCode = string.Format("{0:X4}", (int)character);
            return (string)lookup.Evaluate("string(//UnicodeNames/n[@c='"+charCode+"'])");
        }
    }

    internal class ComposeSequence
    {
        private string result;
        private string sequence;

        public ComposeSequence(string result, string sequence)
        {
            this.sequence = sequence;
            this.result = result;
        }

        public string Character
        {
            get { return result.ToString(); }
        }

        public string Sequence
        {
            get { return sequence; }
        }

        public string FancySequence
        {
            get { return sequence.Select(c => ("<" + c + ">")).Aggregate("", (a, s) => a + " " + s); }
        }

        public string Name {
            get
            { 
                if(result.Length == 1)
                    return UnicodeNames.Instance.NameOf(result[0]);
                return result;
            }
        }
    }
}