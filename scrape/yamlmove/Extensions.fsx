module Extensions

#I "/Users/Nate/_src/ld-utilities/packages/NICE.Freya/tools/"
#r "FSharp.Markdown.dll"

open System
open System.IO
open System.Text
open FSharp.Markdown

let root = "/Users/Nate/_src/yamltest/"

let (++) x y = Path.Combine(x,y)

let fileExists path = if(File.Exists(path)) then Some(path) else None

let createDirectory (path:string) = 
    let path = path.Replace("/Statement.md", String.Empty)
    if(Directory.Exists path) then () else Directory.CreateDirectory(path) |> ignore

let writeToFile (contents:string) (path:string) =
    createDirectory path
    let bytesOfFile = Encoding.UTF8.GetBytes(contents)
    use fs = File.Create(path,1024, FileOptions.None)
    fs.Write(bytesOfFile, 0, bytesOfFile.Length)


let returnSpans (spans:MarkdownSpan list) = [
    for span in spans do
        match span with
            | Literal text -> yield text
            | Strong spans ->
               for span in spans do
                 match span with
                   | Literal text -> yield text
            | Emphasis spans ->
               for span in spans do
                 match span with
                   | Literal text -> yield text
]

//Codeblock retrieveal
let private yieldCodeBlock (paragraphs: MarkdownParagraphs) =[
    for i in paragraphs do
        match i with
            | CodeBlock(x,_,_) -> yield Some x
            | _ -> yield None
    ]

//title retrieval
let private yieldTitle (paragraphs: MarkdownParagraphs) = [
    for i in paragraphs do
        match i with
            | Heading(2, text) -> yield Some text
            | _ -> yield None
    ]

let getYaml x = x |> yieldCodeBlock |> List.choose id |> List.tryHead

let getTitle x = x |> yieldTitle |> List.choose id |> List.concat |> returnSpans |> String.Concat

let getTrimmedTitle x = (getTitle x).Replace(" ", String.Empty).ToLowerInvariant()

