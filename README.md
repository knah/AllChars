# AllChars
AllChars is a tool that emulates compose key on Windows, allowing the user to input Unicode characters such as "?", "?" or "e" with relative ease.

This is a version with my changes, which are mostly there to allow for multi-key sequences, including sequences that output multiple characters.
This includes changes to config format, and lack of the "try reordered" feature.
Longer sequences also won't try different case, and if one sequence is a prefix of another (like "ab" and "abc"), you won't be able to use the longer one.
### BACKUP YOUR OLD CONFIG, NEW CONFIG FORMAT IS INCOMPATIBLE AND THERE IS NO MIGRATION CODE
Configuration files can be found in %AppData% folder.

I've also removed all things I deemed unnecessary, like expiration date, macros (their functionality is replaced by plain macros), and probably something else I've missed.

It requires .NET Framework 4 to run, and in my experience works fine on all Windows versions from 7 to 10.

Also with 90% less stuck shift key now.

Original website for AllChars is http://allchars.zwolnet.com/, and SourceForge project is located at https://sourceforge.net/projects/allchars/.
