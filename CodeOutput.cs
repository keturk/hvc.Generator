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

using System.Collections;
using System.Text;
using Antlr4.StringTemplate;
using hvc.Extensions;

namespace hvc.Generator;

public abstract class CodeOutput<T> where T : CodeOutput<T>
{
    private const Int32 IndentSize = 4;

    protected readonly StringBuilder CodeContent = new();
    public Int32 IndentLevel { get; protected set; }
    protected TemplateStack TemplateStack;

    protected CodeOutput()
    {
        TemplateStack = TemplateStack.Create(GetType(), typeof(CodeOutput<T>));
    }

    public String Content
    {
        get
        {
            FinalizeContent();

            return CodeContent.ToString();
        }
    }

    public T Append(params String[] lines)
    {
        if (lines == null || !lines.Any())
            throw new ArgumentNullException(nameof(lines));

        foreach (var line in lines)
            CodeContent.Append(line);

        return (T) this;
    }

    public T AppendIf(Boolean condition, params String[] lines)
    {
        if (condition)
            Append(lines);

        return (T) this;
    }

    public T AppendIn()
    {
        Append(GetIndent());

        return (T) this;
    }

    public Template Template(String templateName)
    {
        return TemplateStack[templateName];
    }

    public T Block(Action action)
    {
        action.Invoke();

        return (T) this;
    }

    public T ForEach(IEnumerable items, Action<Object> action)
    {
        foreach (var item in items)
            action.Invoke(item);

        return (T)this;
    }

    public T Line(params String[] values)
    {
        if (!values.Any())
            CodeContent.AppendLine();
        else
            foreach (var lines in values)
            foreach (var line in lines.Lines())
                CodeContent.AppendLine(IndentLevel == 0 ? $"{line}" : $"{GetIndent()}{line}");

        return (T) this;
    }

    public T LineIf(Boolean condition, params String[] lines)
    {
        if (condition)
            Line(lines);

        return (T) this;
    }

    public virtual T Begin()
    {
        return Line("{").In();
    }

    public virtual T Comment(String commentLine)
    {
        return Line($"// {commentLine}");
    }

    public T CommentIf(Boolean condition, String commentLine)
    {
        return condition ? Comment(commentLine) : (T)this;
    }

    public virtual T Comment(params String[] commentLines)
    {
        foreach (var commentLine in commentLines)
            Line($"// {commentLine}");

        return (T) this;
    }

    public T CommentIf(Boolean condition, params String[] commentLines)
    {
        return condition ? Comment(commentLines) : (T)this;
    }

    public virtual T End(String? prefix = null)
    {
        return Out().Line($"}}{prefix ?? String.Empty}");
    }

    public virtual void FinalizeContent()
    {
    }

    public T In()
    {
        IndentLevel++;

        return (T) this;
    }

    public T Out()
    {
        if (IndentLevel > 0)
            IndentLevel--;

        return (T) this;
    }

    public T Reset()
    {
        CodeContent.Clear();
        IndentLevel = 0;

        return (T) this;
    }

    public T SetIndentLevel(Int32 indentLevel)
    {

        if(indentLevel < 0)
            throw new ArgumentException($"Value '{indentLevel}' is not a valid indent level!");

        IndentLevel = indentLevel;

        return (T) this;
    }

    public override String ToString()
    {
        return CodeContent.ToString();
    }

    public void WriteToFile(String outputFilename, Boolean skipIfExists = false)
    {
        Content.WriteToFile(outputFilename, skipIfExists);
    }

    protected String GetIndent()
    {
        return new String(' ', IndentLevel * IndentSize);
    }

    public T Public(String value)
    {
        return Line($"public {value}");
    }

    public T Private(String value)
    {
        return Line($"private {value}");
    }
}