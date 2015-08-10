#I "../../../packages"
#r "../../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#I "../../../packages/FSharp.RDF/lib/"
#r "../../../packages/FSharp.RDF/lib/FSharp.RDF.dll"
#r "../../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../../../packages/VDS.Common/lib/net40-client/VDS.Common.dll"
#r "../../../packages/dotNetRDF/lib/net40/dotNetRDF.dll"

open VDS.Common
open FSharp.Data
open FSharp.RDF
open Assertion
open graph
open rdf

let (++) a b = System.IO.Path.Combine(a,b)


let rec depthTuple c xs =
  match xs with
    | ""::xs -> depthTuple (c+1) xs
    | x::_ -> (c,x)

type context = { parent: int * string
                 previous: int * string
                 grandparents: (int * string) list
}

let createOwlResource prefix label parentLabel =
    owl.cls !!(prefix + label) [
    !!(prefix + parentLabel)
    ] [
    dataProperty !!"rdfs:label" (label ^^xsd.string)
    ]

let moveParent context tuples  =
  match context, tuples with
    | _,[] -> context
    | { parent= (parentDepth,_); previous= previous; grandparents=grandparents}, (depth, label)::_ ->
      match depth-parentDepth with
        | 2 -> {context with
                    parent = previous
                    previous = (depth,label)
                    grandparents= context.parent::grandparents}
        | 0 -> { context with
                    parent= List.head grandparents
                    previous = (depth,label)
                    grandparents= List.tail grandparents}
        | _ -> {context with previous = (depth,label)}

let rec owlGen prefix context tuples =
  match tuples with
  | [] -> []
  | (depth, label)::tail ->
    let context' = moveParent context tuples
    createOwlResource prefix label (snd context'.parent) :: owlGen prefix (moveParent context tuples) tail

let newContext a = {
  parent=a
  previous=a
  grandparents= []
}


let typesFor file prefix ancestor =
    let csv = FSharp.Data.CsvFile.Load(__SOURCE_DIRECTORY__ ++ file)
    let g = Graph.empty !!"http://ld.nice.org.uk/ns/qualitystandards" []
    csv.Rows
    |> Seq.map (fun a -> a.Columns)
    |> Seq.map Array.toList
    |> Seq.map (depthTuple 1)
    |> Seq.toList
    |> owlGen prefix (newContext (0, ancestor))
    |> Assert.graph g
do

  let sb = System.Text.StringBuilder()
  let g' = Graph.loadTtl (fromString """
 @base <http://ld.nice.org.uk/ns/qualitystandards>.

@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.
@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.
@prefix base: <http://ld.nice.org.uk/ns/qualitystandards>.
@prefix owl: <http://www.w3.org/2002/07/owl#>.

<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Acne> a <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>,
                                                                        owl:Class;
                                                                      rdfs:label "Acne"^^xsd:string.
<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Acute%20coronary%20syndromes> a <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>,
                                                                                                owl:Class;
                                                                                              rdfs:label "Acute coronary syndromes"^^xsd:string.
<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Addiction> a <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>,
                                                                             owl:Class;
                                                                           rdfs:label "Addiction"^^xsd:string.
<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Age%20related%20macular%20degeneration> a <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>,
                                                                                                          owl:Class;
                                                                                                        rdfs:label "Age related macular degeneration"^^xsd:string .
""")
  ()

  let g = typesFor "./sample.csv" "http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#" "ConditionsAndDiseases"
  let d = Graph.diff g g'

  Graph.writeTtl (toString sb) g

  printfn "Graph is %s" (string sb)

  if not d.AreEqual then
    failwithf "Sample graph doesn't match  %s" ((string) d)
