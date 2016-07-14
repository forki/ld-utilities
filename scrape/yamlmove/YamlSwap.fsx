#I "../../packages/NICE.Freya/tools/"
#r "UnionArgParser.dll"
#r "FSharp.Markdown.dll"
#r "IKVM.OpenJDK.Core.dll"
#r "Newtonsoft.Json.dll"
#r "owlapi.dll"
#r "JsonLd.dll"
#r "dotNetRDF.dll"
#r "FSharp.RDF.dll"
#r "FSharp.Compiler.Service.dll"
#r "freya.exe"
#load "Extensions.fsx"

open System
open System.IO
open Freya.YamlParser
open FSharp.Markdown
open Extensions
open System.Text

let mismatch = System.Collections.Generic.List<string>()
let placeholder = System.Collections.Generic.List<string>()
type Files =
    // string (without) * string (with)
    | Exists of string * string * string
with
    static member WithoutAnnotations =
        Directory.GetFiles((root ++ "without"), "Statement.md", SearchOption.AllDirectories)
        |> Array.toList
    static member Exist =
        let withAnnon =
            Files.WithoutAnnotations
            |> List.map(fun file -> file.Replace("without", "with"))
            // |> List.map(fun file -> fileExists file )
            // |> List.choose id
        withAnnon |> List.map(fun path -> Exists(path.Replace("with", "without"), path, path.Replace("with", "new")))
    static member MoveAnnotation =
        for file in Files.Exist do
            match file with
                | Exists(withoutAnn, withAnn, newPath) ->
                    let fileWith = File.ReadAllText(withAnn, Encoding.UTF8)
                    let fileWithout = File.ReadAllText(withoutAnn, Encoding.UTF8)
                    let mdWith = Markdown.Parse(fileWith)
                    let mdWithout = Markdown.Parse(fileWithout)
                    let titleWith = getTrimmedTitle mdWith.Paragraphs
                    let titleWithout = getTrimmedTitle mdWithout.Paragraphs
                    if(titleWith = titleWithout) then
                        let mutable yaml = ""
                        let annotations = getYaml mdWith.Paragraphs
                        if (Option.isSome annotations) then
                            yaml <- sprintf """```%s%s```%s""" Environment.NewLine (Option.get annotations) Environment.NewLine
                            writeToFile (yaml + fileWithout) newPath
                        else
                            placeholder.Add(withAnn) |> ignore
                    else
                        mismatch.Add(sprintf "Non matching title or couldnt retrieve: %s" withAnn) |> ignore


Files.MoveAnnotation
Files.Exist
printfn "%d" (mismatch |> Seq.toList |> List.length)
for i in mismatch do
    printfn "%s" i

let out = Markdown.Parse (File.ReadAllText("/Users/Nate/_src/yamltest/without/qs47/st2/Statement.md", Encoding.UTF8))

let yaml = getTitle out.Paragraphs


