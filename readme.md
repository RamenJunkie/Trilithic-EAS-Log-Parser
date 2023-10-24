# Parser for Trilithic EAS (Emergency Slert System) Logs

Parses through log files produced by Trilithic EAS systems and outputs them in an easily readable format for parsing through logs on a monthly basis.

There are two binaries, EASLog.exe is a console app used at the terminal using:

```
EASLog.exe <filename>
```
Without replacing <filename> witht he file name, no braces.  This will output a file <filename>-Processed.txt with a simple table fo dates and sources and alert types seperated by weeks.

EASLogWindows.exe is a Windows app which will open a window, allow the user to opena a file, which will then be parsed and the result displayed in a window.  It will also produce a <filename>-Processed.txt file in the background, in the same folder as the original file.

---
It is strongly recomended to verify the log accuracy after running the tool.