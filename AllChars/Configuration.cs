using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace AllCharsNS {
    public enum VersionDisposition {
        Unversioned,
        Invalid,
        OlderUnconvertable,
        OlderConvertable,
        Correct,
        Newer
    }

    public class ConfigurationVersionException : Exception {
        private VersionDisposition disposition;

        public ConfigurationVersionException(VersionDisposition disposition) {
            this.disposition = disposition;
        }

        public VersionDisposition Disposition {
            get { return disposition; }
        }
    }


    /// <summary>
    /// Summary description for Configuration.
    /// </summary>
    public class Configuration {
        private const int CONFIG_VERSION = 2;

        private string fileName;
        private XmlDocument document;
        private XPathNavigator lookup;

        public Configuration(string configFileName) {
            fileName = configFileName;

            BinaryReader resConfig =
                new BinaryReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AllCharsNS.AllChars.xml"));

            if (!File.Exists(fileName)) {
                BinaryWriter newConfig = new BinaryWriter(File.Create(fileName));
                newConfig.Write(resConfig.ReadBytes((int)resConfig.BaseStream.Length));
                newConfig.Flush();
                newConfig.Close();
            }

            try {
                string exampleFileName = Path.Combine(Path.GetDirectoryName(configFileName), "AllChars-example.xml");
                BinaryWriter exampleConfig = new BinaryWriter(File.Create(exampleFileName));
                exampleConfig.Write(resConfig.ReadBytes((int)resConfig.BaseStream.Length));
                exampleConfig.Flush();
                exampleConfig.Close();
            }
            catch (Exception) {} // could not write example config, this is non-fatal

            document = new XmlDocument();

            document.Load(fileName);
            int versionNumber;
            try {
                Debug.WriteLine("Got config version string: " + document.SelectSingleNode("/AllChars/Version").InnerText);
                versionNumber = int.Parse(document.SelectSingleNode("/AllChars/Version").InnerText);
            }
            catch (NullReferenceException) {
                throw new ConfigurationVersionException(VersionDisposition.Unversioned);
            }
            catch {
                throw new ConfigurationVersionException(VersionDisposition.Invalid);
            }

            if (versionNumber < CONFIG_VERSION)
                throw new ConfigurationVersionException(VersionDisposition.OlderUnconvertable);
            else if (versionNumber > CONFIG_VERSION)
                throw new ConfigurationVersionException(VersionDisposition.Newer);

            lookup = document.CreateNavigator();
        }

        private bool getBoolSetting(string settingName) {
            return
                Boolean.Parse(
                    (string)lookup.Evaluate("string(//Configuration/Setting[@name='" + settingName + "']/@bool)"));
        }

        private void setBoolSetting(string settingName, bool value) {
            XmlElement setting =
                (XmlElement)document.SelectSingleNode("//Configuration/Setting[@name='" + settingName + "']");
            setting.SetAttribute("bool", value.ToString());
        }

        private int getIntSetting(string settingName) {
            return
                int.Parse((string)lookup.Evaluate("string(//Configuration/Setting[@name='" + settingName + "']/@int)"));
        }

        private void setIntSetting(string settingName, int value) {
            XmlElement setting =
                (XmlElement)document.SelectSingleNode("//Configuration/Setting[@name='" + settingName + "']");
            setting.SetAttribute("int", value.ToString());
        }

        public void Save() {
            document.Save(fileName);
        }

        public string FileName {
            get { return fileName; }
        }

        #region Configuration parameters

        public bool HideTaskbarIcon {
            get { return getBoolSetting("HideTaskbarIcon"); }
            set { setBoolSetting("HideTaskbarIcon", value); }
        }

        public Keys HotKey {
            get {
                return (Keys)Enum.Parse(typeof (Keys),
                                        (string)lookup.Evaluate("string(//Configuration/Setting[@name='HotKey']/@key)"),
                                        true);
            }
            set {
                XmlElement setting = (XmlElement)document.SelectSingleNode("//Configuration/Setting[@name='HotKey']");
                setting.SetAttribute("key", value.ToString());
            }
        }

        public bool SendUndefined {
            get { return getBoolSetting("SendUndefined"); }
            set { setBoolSetting("SendUndefined", value); }
        }

        public bool TranslateDecimal {
            get { return getBoolSetting("TranslDecPoint"); }
            set { setBoolSetting("TranslDecPoint", value); }
        }

        public bool TryCaseInsensitive {
            get { return getBoolSetting("TryToggleCase"); }
            set { setBoolSetting("TryToggleCase", value); }
        }

        public bool TryReverse {
            get { return getBoolSetting("TrySwapped"); }
            set { setBoolSetting("TrySwapped", value); }
        }

        public bool UnixSemantics {
            get { return getBoolSetting("UnixSemantics"); }
            set { setBoolSetting("UnixSemantics", value); }
        }

        public bool UseBeeps {
            get { return getBoolSetting("UseBeeps"); }
            set { setBoolSetting("UseBeeps", value); }
        }

        public bool UseSpeaker {
            get { return getBoolSetting("UseSpeaker"); }
            set { setBoolSetting("UseSpeaker", value); }
        }

        private TimeSpan typingTimeout = TimeSpan.MinValue;

        public TimeSpan TypingTimeout {
            get {
                if (typingTimeout == TimeSpan.MinValue)
                    typingTimeout = TimeSpan.FromMilliseconds(getIntSetting("MacroTimeout"));
                return typingTimeout;
            }
            set {
                typingTimeout = value;
                setIntSetting("MacroTimeout", typingTimeout.Milliseconds);
            }
        }

        public char MacroChar {
            get { return ((string)lookup.Evaluate("string(//Configuration/Setting[@name='MacroChar']/@char)"))[0]; }
            /*
            set
            {
                XmlElement setting = (XmlElement)document.SelectSingleNode("//Configuration/Setting[@name='MacroChar']");
                setting.SetAttribute("char", value.ToString());
            }
            */
        }

        #endregion

        #region Business data

        public XmlNode ComposeTable {
            get { return document.GetElementsByTagName("ComposeTable")[0]; }
        }

        private List<string> ignoredWindowClasses;
        public List<string> IgnoredWindowClasses {
            get {
                if (ignoredWindowClasses == null) {
                    XmlNodeList xmlIgnores = document.SelectNodes("//Ignore/WindowClass");
                    if (xmlIgnores != null) {
                        ignoredWindowClasses = new List<string>(xmlIgnores.Count);
                        foreach (XmlNode xmlIgnore in xmlIgnores)
                            ignoredWindowClasses.Add(xmlIgnore.InnerText);
                    }
                    else
                        ignoredWindowClasses = new List<string>(0);
                }
                return ignoredWindowClasses;
            }
        }

        public string[] ComposeCategories {
            get {
                XmlNodeList xmlCategories = document.SelectNodes("//ComposeTable/Section");
                string[] composeCategories = new string[xmlCategories.Count];
                for (int i = 0; i < xmlCategories.Count; i++)
                    composeCategories[i] = xmlCategories[i].Attributes["name"].Value;
                return composeCategories;
            }
        }

        #endregion

        Dictionary<string, ComposeSequence> myAllComposeSequences;
        internal Dictionary<string, ComposeSequence> AllComposeSequences
        {
            get
            {
                if (myAllComposeSequences == null)
                {
                    myAllComposeSequences = new Dictionary<string, ComposeSequence>();
                    foreach (var c in ComposeCategories.SelectMany(cat => GetSequencesByCategory(cat)))
                    {
                        if (myAllComposeSequences.ContainsKey(c.Sequence))
                            Debug.WriteLine("Duplicate compose for sequence {0}", c.Sequence);
                        myAllComposeSequences[c.Sequence] = c;
                    }
                }
                return myAllComposeSequences;
            }
        }

        HashSet<string> myAllComposePrefixes;
        internal HashSet<string> AllComposePrefixes
        {
            get
            {
                if (myAllComposePrefixes == null)
                {
                    myAllComposePrefixes = new HashSet<string>();
                    myAllComposePrefixes.Add("");
                    var sequences = AllComposeSequences;
                    foreach (var s in sequences.Keys)
                    {
                        var ss = s.ToLower();
                        for (int i = 0; i < ss.Length; i++)
                        {
                            myAllComposePrefixes.Add(ss.Substring(0, i + 1));
                        }
                    }
                }
                return myAllComposePrefixes;
            }
        }

        internal int MaxSequenceLength
        {
            get
            {
                return myAllComposeSequences.Select(c => c.Key.Length).Max(); // todo: optimize?
            }
        }

        internal IList<ComposeSequence> GetSequencesByCategory(string category) {
            XmlNodeList xmlSequences = document.SelectNodes("//ComposeTable/Section[@name='" + category + "']/Compose");
            ComposeSequence[] composeSequences = new ComposeSequence[xmlSequences.Count];

            for (int i = 0; i < xmlSequences.Count; i++)
                composeSequences[i] =
                    new ComposeSequence(xmlSequences[i].InnerText, xmlSequences[i].Attributes["sequence"].Value);

            return composeSequences;
        }
    }
}