#r "../packages/dotNetRDF/lib/net40/dotNetRDF.dll"
#r "../packages/FSharp.RDF/lib/net40/FSharp.RDF.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data
open System
open FSharp.RDF
open Assertion
open rdf

type skosv = CsvProvider<"/Users/Nate/MEGA/Development/FSharp/ld-ontologies/ontologies/ns/Sample Settings vocabulary to go in SKOS format.csv">
let csv = skosv.Load("/Users/Nate/MEGA/Development/FSharp/ld-ontologies/ontologies/ns/Sample Settings vocabulary to go in SKOS format.csv")

let mkTuple (r:skosv.Row) = (r.First,r.Second,r.Third)

let (|SeqEmpty|SeqCons|) (xs: 'a seq) =
  if Seq.isEmpty xs then SeqEmpty
  else SeqCons( Seq.head xs |> mkTuple, Seq.skip 1 xs)

let (|E|NE|) s =
  if String.IsNullOrEmpty s then E
  else NE s

(**
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


let r = toPaths csv.Rows []
*)
let toConcept (ax:string list) = [
  match ax with
  | [x] ->
     yield owl.cls !(sprintf "qsc:%A"x) [!"skos:Concept"] [dataProperty !"skos:prefLabel" (x^^xsd.string)]
  | [y;x] ->
     yield owl.cls !(sprintf "qsc:%A"x) [!"skos:Concept"] [objectProperty !"skos:broader" !(sprintf "qsc:%A"y)]
     yield owl.cls !(sprintf "qsc:%A"y) [!"skos:Concept" ] [
               dataProperty !"skos:prefLabel" (x^^xsd.string)
               objectProperty !"skos:narrower" !(sprintf "qsc:%A"x)]
  | [z;y;x] ->
     yield owl.cls !(sprintf "qsc:%A"x) [!"skos:Concept"] [objectProperty !"skos:broader" !(sprintf "qsc:%A"z)]
     yield owl.cls !(sprintf "qsc:%A"z) [!"skos:Concept"] [
                 dataProperty !"skos:prefLabel" (x^^xsd.string)
                 objectProperty !"skos:narrower" !(sprintf "qsc:%A"y)
                ]
     yield owl.cls !(sprintf "qsc:%A"x) [!"skos:Concept"] [objectProperty !"skos:broader" !(sprintf "qsc:%A"y)]
     yield owl.cls !(sprintf "qsc:%A"y) [!"skos:Concept"] [
                 dataProperty !"skos:prefLabel" (x^^xsd.string)
                 objectProperty !"skos:narrower" !(sprintf "qsc:%A"x)
                ]
     yield owl.cls !(sprintf "qsc:%A"z) [!"skos:Concept"] [
                 dataProperty !"skos:prefLabel" (x^^xsd.string)
                 objectProperty !"skos:narrower" !(sprintf "qsc:%A"y)
                ]
  ]

let g = graph.empty (!"http://nice.org.uk/ns/qualitystandard" ) []
toPaths csv.Rows []
|> List.collect toConcept
|> output.toGraph g
|> output.format graph.write.ttl (graph.toFile "")
