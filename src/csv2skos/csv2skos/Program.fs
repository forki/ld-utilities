// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module csvskos =
    open System
    open FSharp.Data
    open FSharp.RDF
    open Assertion
    open rdf
    open graph

    type csv = FSharp.Data.CsvProvider<Sample="First (string), Second (string), Third (string)">
    let loadCsv(p : string) = csv.Load(p)
        
    let rSpaces (x:(string*string*string)) =
        match x with
        | (x,y,z) -> (x.Replace(' ', '-'), y.Replace(' ', '-'), z.Replace(' ', '-'))

    let mkTuple(r : csv.Row) = rSpaces (r.First, r.Second, r.Third)

    let (|SeqEmpty|SeqCons|)(xs : 'a seq) =
        if Seq.isEmpty xs then SeqEmpty
        else SeqCons(Seq.head xs |> mkTuple, Seq.skip 1 xs)

    let (|E|NE|) s =
        if System.String.IsNullOrEmpty s then E
        else NE s

    let rec toPaths xs p = [
      let next p xs = p::(toPaths xs p)
      match xs,p with
      | SeqEmpty,_ -> ()
      | SeqCons((NE v,E, E),xs),[] -> yield! next [v] xs
      | SeqCons((NE v,E, E),xs),[a] -> yield! next [v] xs
      | SeqCons((E,NE v, E),xs),[a] -> yield! next [v;a] xs
      | SeqCons((E,NE v, E),xs),[a;b] -> yield! next [v;b] xs
      | SeqCons((E,E, NE v),xs),[a;b] -> yield! next [v;a;b] xs
      | SeqCons((E,E, NE v),xs),[a;b;c] -> yield! next [v;b;c] xs
      | SeqCons((E, NE v, E),xs),[a;b;c] -> yield! next [v;c] xs
      | SeqCons((NE v, E, E),xs), [a;b] -> yield! next [v] xs
      | SeqCons(x,_),p -> printfn "%A %A" x p
     ]



    let toConcept(ax : string list) = [
        match ax with
         | [x] ->
             yield owl.individual !(sprintf "qsc:%s" x) [!"skos:Concept"]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string)]
         | [y; x] ->
             yield owl.individual !(sprintf "qsc:%s" x) [!"skos:Concept"]
                       [objectProperty !"skos:broader" !(sprintf "qsc:%s" y)]
             yield owl.individual !(sprintf "qsc:%s" y) [!"skos:Concept"]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" !(sprintf "qsc:%s" x)]
         | [z; y; x] ->
             yield owl.individual !(sprintf "qsc:%s" x) [!"skos:Concept"]
                       [objectProperty !"skos:broader" !(sprintf "qsc:%s" y)]

             yield owl.individual !(sprintf "qsc:%s" z) [!"skos:Concept"]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" !(sprintf "qsc:%s" y)]

             yield owl.individual !(sprintf "qsc:%s" y) [!"skos:Concept"]
                       [objectProperty !"skos:broader" !(sprintf "qsc:%s" z)]

             yield owl.individual !(sprintf "qsc:%s" y) [!"skos:Concept"]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" !(sprintf "qsc:%s" x)]

             yield owl.individual !(sprintf "qsc:%s" z) [!"skos:Concept"]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" !(sprintf "qsc:%s" y)]]

open Nessos.UnionArgParser
open System
open FSharp.Data
open FSharp.RDF
open Assertion
open rdf
open graph

type CommandArguments =
    | Uri of string
    interface IArgParserTemplate with
        member u.Usage =
            match u with
            | Uri _ -> "Specify a file"

[<EntryPoint>]
let main argv =
    let parser = UnionArgParser.Create<CommandArguments>()
    let argv = parser.Parse(argv)
    let csvLocation = argv.GetResult(<@ CommandArguments.Uri @>)
    let g = graph.empty (!"http://nice.org.uk/ns/qualitystandard") []
    let csv = csvskos.loadCsv csvLocation
    let startTrim = csvLocation.LastIndexOf('/')+1
    let writeTo = csvLocation.Substring(0, csvLocation.LastIndexOf('/')+1)
    let fileName = csvLocation.Substring(startTrim, csvLocation.LastIndexOf('.')-startTrim)

    csvskos.toPaths csv.Rows []
    |> List.collect csvskos.toConcept
    |> Assert.resources g
    |> graph.format graph.write.ttl (graph.toFile (writeTo+fileName+".ttl"))
    |> ignore
    0 // return an integer exit code

    