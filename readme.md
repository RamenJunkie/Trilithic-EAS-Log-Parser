# Parser for Trilithic EAS (Emergency Alert System) Logs

Parses through log files produced by Trilithic EAS systems and outputs them in an easily readable format for parsing through logs on a monthly basis.

There are two binaries, EASLog.exe is a console app used at the terminal using:

```
EASLog.exe <filename>
```
Without replacing <filename> with the file name, no braces.  This will output a file <filename>-Processed.txt with a simple table of dates and sources and alert types separated by weeks.

EASLogWindows.exe is a Windows app that will open a window, and allow the user to open a file, which will then be parsed and the result displayed in a window.  It will also produce a <filename>-Processed.txt file in the background, in the same folder as the original file.

---

A sample log file and output file is included, it's all public information, but for the sake of my sanity, I've scrubbed it with fake radio stations and data, but it is based on a real log file.

---

It is strongly recommended to verify the log accuracy after running the tool.

---

Known issues:

It's probably fixable very easily, but if you run a log through the reader a second time, it appends the results to the previous results file, instead of creating a new file.

