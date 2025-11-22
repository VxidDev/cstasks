# CSTasks
A simple command-line to-do list app written in C#. Add, view, and manage tasks directly from your terminal.

## Features
- Add tasks quickly
- View your to-do list
- Mark tasks as done
- Remove completed tasks
- Lightweight and easy to use from CLI

## Installation

```bash
git clone https://github.com/VxidDev/CStasks.git
dotnet new console -n cstasks
mv CStasks/Program.cs cstasks/Program.cs
cd cstasks 
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o .
chmod +x cstasks 
sudo mv cstasks /usr/local/bin/cstasks
```

and now cstasks is installed!

### Contributing

Feel free to fork this project and submit pull requests. Suggestions and bug reports are welcome!

### License

MIT License

