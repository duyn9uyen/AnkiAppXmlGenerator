# AnkiAppXmlGenerator
Console app to construct an xml file that is compatible to import a new deck into AnkiApp

# VS Code Debugger configurqtion
Add this file: (.vscode/launch.json)

```
{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/bin/Debug/net6.0/AnkiAppXmlGenerator.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "console": "integratedTerminal"
        }
    ]
}```