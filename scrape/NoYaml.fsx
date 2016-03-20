#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Formatting/lib/net40/FSharp.Markdown.dll"

open FSharp.Markdown
open System
open System.IO

let fs = System.IO.Directory.EnumerateFiles("/Users/Nate/_src/ld-content", "Statement.md", SearchOption.AllDirectories)


let d = """
```
Setting:
    - Test
``
#Hello
"""

let md = Markdown.Parse d



let matchCodeBlock (para:MarkdownParagraph) =
    match para with
        | CodeBlock (x,_,_) -> Some true
        | _ -> None


for path in fs do
    let md = Markdown.Parse(System.IO.File.ReadAllText(path))
    let ret = seq { for i in md.Paragraphs do yield (matchCodeBlock i)}
    let hasCodeBlock = ret |> Seq.choose id
    if(Seq.isEmpty hasCodeBlock) then
        printfn "%s" path

