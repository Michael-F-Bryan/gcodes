# Gcodes

[![Build status](https://ci.appveyor.com/api/projects/status/b4t1cp42205pdqta?svg=true)](https://ci.appveyor.com/project/Michael-F-Bryan/gcodes)

A basic C# gcode parser and interpreter.

# Getting Started

Once you've downloaded a copy of the repository (e.g. as a git submodule), you
should be able to start using the `gcodes` library by asking Visual Studio to
add a reference to it.

Parsing the input text takes place in two steps:

- Tokenizing, using the `Lexer` class to convert source text into a stream of
  `Token`s
- Parsing, using a `Parser` to convert a stream of `Token`s into a bunch of
  `Gcode` objects.

```C#
public static void ParseGcodeFile(string filename) {
	string src = File.ReadAllText(filename);

	var lexer = new Lexer(src);
	List<Token> tokens = lexer.Tokenize().ToList();

	var parser = new Parser(tokens);
	List<Gcode> gcodes = parser.Parse().ToList();
}
```

> **Note:** For convenience, `Parser` also has a constructor which accepts a
> `string` and will tokenize everything for you.

There is no guarantee that every argument is provided to a gcode, therefore
if you want to fetch an argument's value (e.g. `X` or feed rate) you can use
the `ValueFor()` method. This will search the arguments provided to a particular
gcode for the specified `ArgumentKind`, and return its value. 