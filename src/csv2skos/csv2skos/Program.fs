module csvskos =
    open System
    open FSharp.Data
    open FSharp.RDF
    open Assertion
    open rdf
    open graph

    let loadCsv(p : string) = CsvFile.Load(p)
  
    let rSpaces (x:(string*string*string)) =
        match x with
        | (x,y,z) -> (x.Replace(' ', '-'), y.Replace(' ', '-'), z.Replace(' ', '-'))

    let mkTuple (r:CsvRow) =
        let c = r.Columns.Length
        match c with
        | 1 -> rSpaces (r.[0], "", "")
        | 3 -> rSpaces (r.[0], r.[1], r.[2])
        | _ -> ("","","")

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

    let idOf (x:string) =
      !(sprintf "qsc:%s" (x.Replace("&", "and")))

    let toConcept b ax = [
        match ax with
         | [x] ->
             yield owl.individual (idOf x) [b]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string)]
         | [y; x] ->
             yield owl.individual (idOf x) [b]
                       [objectProperty !"skos:broader" (idOf y)]
             yield owl.individual (idOf y) [b]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" (idOf x)]
         | [z; y; x] ->
             yield owl.individual (idOf x) [b]
                       [objectProperty !"skos:broader" (idOf y)]

             yield owl.individual (idOf z) [b]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" (idOf y)]

             yield owl.individual (idOf y) [b]
                       [objectProperty !"skos:broader" (idOf z)]

             yield owl.individual (idOf y) [b]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" (idOf x)]

             yield owl.individual (idOf z) [b]
                       [dataProperty !"skos:prefLabel" (x ^^ xsd.string);
                        objectProperty !"skos:narrower" (idOf y)]]

open Nessos.UnionArgParser
open System
open FSharp.Data
open FSharp.RDF
open Assertion
open rdf
open graph

type CommandArguments =
    | Uri of string
    | BaseConcept of string
    interface IArgParserTemplate with
        member u.Usage =
            match u with
            | Uri _ -> "Specify a file"
            | BaseConcept _ -> "The base class for this skos"


[<EntryPoint>]
let main argv =
    let parser = UnionArgParser.Create<CommandArguments>()
    let argv = parser.Parse(argv)
    let csvLocation = argv.GetResult(<@ CommandArguments.Uri @>)
    let baseConcept = argv.GetResult(<@ CommandArguments.BaseConcept @>)
    let g = graph.empty (!"http://nice.org.uk/ns/qualitystandard")
                        [("qsc",!"http://nice.org.uk/ns/qualitystandard/skos#")
                         ("qs",!"http://nice.org.uk/ns/qualitystandard#")
                         ("skos",!"http://www.w3.org/2004/02/skos/core#")
                        ]

    let csv = csvskos.loadCsv csvLocation
    let startTrim = csvLocation.LastIndexOf('/')+1
    let writeTo = csvLocation.Substring(0, csvLocation.LastIndexOf('/')+1)
    let fileName = csvLocation.Substring(startTrim, csvLocation.LastIndexOf('.')-startTrim)

    csvskos.toPaths csv.Rows []
    |> List.collect (csvskos.toConcept !baseConcept)
    |> Assert.resources g
    |> graph.format graph.write.ttl (graph.toFile (writeTo+fileName+".ttl"))
    |> ignore
    0 // return an integer exit code
