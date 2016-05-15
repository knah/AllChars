using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Text;

namespace AllCharsNS
{
    public enum TranslationMode {
        Unicode,
        HTMLEntity
    }

	/// <summary>
	/// Summary description for TranslateCompose.
	/// </summary>
	public class Translator
	{
		private Configuration config;

	    internal TranslationMode Mode = TranslationMode.Unicode;

		internal Translator(Configuration config)
		{
			this.config = config;
		}

		private void Beep()
		{
			if (config.UseSpeaker)
				Native.Beep(Native.MessageBeepType.Default);
			else
				Native.Beep(Native.MessageBeepType.Ok);				
		}

        private ComposeSequence getComposeEntry(string sequence) {

            ComposeSequence compose = null;

            if (config.AllComposeSequences.TryGetValue(sequence, out compose))
                return compose;
            
            /*if (config.TryReverse) {
                compose = lookupCompose(secondQuoted, firstQuoted);
                if (compose != null)
                    return ((IHasXmlNode)compose).GetNode();
            }*/

            if (config.TryCaseInsensitive && sequence.Length < 16)
            {
                StringBuilder ns = new StringBuilder();
                for (int charstates = 0; charstates < 1 << sequence.Length; charstates++)
                {
                    ns.Clear();
                    for (int i = 0; i < sequence.Length; i++)
                        ns.Append((charstates & (1 << i)) > 0 ? char.ToUpper(sequence[i]) : char.ToLower(sequence[i]));
                    if(config.AllComposeSequences.TryGetValue(ns.ToString(), out compose))
                        return compose;
                }
            }

            return null;
        }

	    private string getComposedUnicode(string sequence)
		{
		    var compose = getComposeEntry(sequence);

            if (compose != null)
                return compose.Character;
			else return "";
		}
		
        // return true to mean end composing
		internal bool Compose(string sequence)
		{
            switch (Mode) {
                case TranslationMode.Unicode:
                    string composed = getComposedUnicode(sequence);
                    if (composed != "") {
                        Native.SendChars(composed.ToCharArray());
                        return true;
                    }
                    break;
            }

            if (config.AllComposePrefixes.Contains(sequence.ToLower()))
                return false;

            if (config.UseBeeps)
                Beep();
			
			if (config.SendUndefined)
				Native.SendChars(sequence.ToCharArray());

            return true;
		}
	}
}
