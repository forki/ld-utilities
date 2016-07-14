#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Formatting/lib/net40/FSharp.Markdown.dll"

open FSharp.Markdown
open System
open System.IO

let fs = System.IO.Directory.EnumerateFiles("/Users/Nate/_src/ld-statement-content", "Statement.md", SearchOption.AllDirectories)

let mutable count = 0
let paths = System.Collections.Generic.List<_>()

let codeBlock (parse: MarkdownDocument) (path : string) =
    for i in parse.Paragraphs do
        match i with
            | CodeBlock(x,_,_) ->
                count <- count + 1
                paths.Add(path) |> ignore
            | _ -> ()
            

for i in fs do
  let parse = Markdown.Parse (File.ReadAllText(i))
  codeBlock parse i

let uniqueWithAnnotations = paths |> Seq.distinct |> Seq.toList



let allSet = Set.ofList (fs |> Seq.toList)
let withSet = Set.ofList uniqueWithAnnotations


let result = (allSet - withSet) |> Set.toList

let statements =
    result
    |> List.map(fun path -> path.Replace("/Users/Nate/_src/ld-statement-content/qualitystandard/", ""))

printfn "Count : %d" count
printfn "Paths : %A" paths

for i in a do
    printfn "%s" i

