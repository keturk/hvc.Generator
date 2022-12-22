// MIT License
//
// Copyright (c) 2022 Kamil Ercan Turkarslan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace hvc.Generator;

public class PythonOutput : CodeOutput<PythonOutput>
{
    private static readonly HashSet<String> BuiltIns = new()
    {
        "ArithmeticError", "AssertionError", "AttributeError", "BaseException",
        "BufferError", "BytesWarning", "DeprecationWarning", "EOFError", "Ellipsis",
        "EnvironmentError", "Exception", "False", "FloatingPointError",
        "FutureWarning", "GeneratorExit", "IOError", "ImportError", "ImportWarning",
        "IndentationError", "IndexError", "KeyError", "KeyboardInterrupt",
        "LookupError", "MemoryError", "NameError", "None", "NotImplemented",
        "NotImplementedError", "OSError", "OverflowError", "PendingDeprecationWarning",
        "ReferenceError", "RuntimeError", "RuntimeWarning", "StandardError",
        "StopIteration", "SyntaxError", "SyntaxWarning", "SystemError", "SystemExit",
        "TabError", "True", "TypeError", "UnboundLocalError", "UnicodeDecodeError",
        "UnicodeEncodeError", "UnicodeError", "UnicodeTranslateError", "UnicodeWarning",
        "UserWarning", "ValueError", "Warning", "WindowsError", "ZeroDivisionError",
        "_", "__debug__", "__doc__", "__import__", "__name__", "__package__", "abs",
        "all", "any", "apply", "basestring", "bin", "bool", "buffer", "bytearray",
        "bytes", "callable", "chr", "classmethod", "cmp", "coerce", "compile",
        "complex", "copyright", "credits", "delattr", "dict", "dir", "divmod",
        "enumerate", "eval", "execfile", "exit", "file", "filter", "float", "format",
        "frozenset", "getattr", "globals", "hasattr", "hash", "help", "hex", "id",
        "input", "int", "intern", "isinstance", "issubclass", "iter", "len", "license",
        "list", "locals", "long", "map", "max", "memoryview", "min", "next", "object",
        "oct", "open", "ord", "pow", "print", "property", "quit", "range", "raw_input",
        "reduce", "reload", "repr", "reversed", "round", "set", "setattr", "slice",
        "sorted", "staticmethod", "str", "sum", "super", "tuple", "type", "unichr",
        "unicode", "vars", "xrange", "zip"
    };

    public override PythonOutput Comment(String commentLine)
    {
        return Line($"# {commentLine}");
    }

    public override Boolean IsBuiltIn(String value)
    {
        return BuiltIns.Contains(value);
    }
}