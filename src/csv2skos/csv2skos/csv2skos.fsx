﻿#r "../../../packages/dotNetRDF/lib/net40/dotNetRDF.dll"
#r "../../../packages/FSharp.RDF/lib/net40/FSharp.RDF.dll"
#r "../../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../../../packages/VDS.Common/lib/net40-client/VDS.Common.dll"

open VDS.Common
open FSharp.Data
open FSharp.RDF
open Assertion
open graph
open rdf


type csv = FSharp.Data.CsvProvider< Sample="First (string), Second (string), Third (string)" >

let loadCsv(p : string) = csv.Load(p)

let rSpaces(x : string * string * string) =
    match x with
    | (x, y, z) ->
        (x.Replace(' ', '-'), y.Replace(' ', '-'), z.Replace(' ', '-'))

let mkTuple(r : csv.Row) = rSpaces(r.First, r.Second, r.Third)

let (|SeqEmpty|SeqCons|)(xs : 'a seq) =
    if Seq.isEmpty xs then SeqEmpty
    else SeqCons(Seq.head xs |> mkTuple, Seq.skip 1 xs)

let (|E|NE|) s =
    if System.String.IsNullOrEmpty s then E
    else NE s

let rec toPaths xs p =
    [let next p xs = p :: (toPaths xs p)
     match xs, p with
     | SeqEmpty, _ -> ()
     | SeqCons((NE v, E, E), xs), [] -> yield! next [v] xs
     | SeqCons((NE v, E, E), xs), [a] -> yield! next [v] xs
     | SeqCons((E, NE v, E), xs), [a] -> yield! next [v; a] xs
     | SeqCons((E, NE v, E), xs), [a; b] -> yield! next [v; b] xs
     | SeqCons((E, E, NE v), xs), [a; b] -> yield! next [v; a; b] xs
     | SeqCons((E, E, NE v), xs), [a; b; c] -> yield! next [v; b; c] xs
     | SeqCons((E, NE v, E), xs), [a; b; c] -> yield! next [v; c] xs
     | SeqCons((NE v, E, E), xs), [a; b] -> yield! next [v] xs
     | SeqCons(x, _), p -> printfn "%A %A" x p]

let toConcept(ax : string list) =
    [match ax with
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

let g = graph.empty (!"http://nice.org.uk/ns/qualitystandard") []
let csv = loadCsv ""
toPaths csv.Rows []
|> List.collect toConcept
|> Assert.resources g
|> graph.format graph.write.ttl (graph.toFile "")
|> ignore
