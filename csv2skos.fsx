#I @"packages/FSharp.RDF/lib"
#r @"packages/FSharp.RDF/lib/FSharp.RDF.dll"
#r @"packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r @"packages/dotNetRDF/lib/net40/dotNetRDF.dll"
#r @"packages/VDS.Common/lib/net40-client/VDS.Common.dll"

open VDS.Common
open FSharp.Data
open FSharp.RDF
open Assertion
open graph
open rdf

let root = fsi.CommandLineArgs.[1]

let (++) a b = System.IO.Path.Combine(a, b)
let fPath = root ++ "../skoscsv"

let rec depthTuple c xs =
  match xs with
  | "" :: xs -> depthTuple (c + 1) xs
  | x :: _ -> (c, x)

type context =
  { parent : int * string
    previous : int * string
    grandparents : (int * string) list }

let createOwlResource prefix label parentLabel =
  owl.cls !!(prefix + label) []
    [ dataProperty !!"rdfs:label" (label ^^ xsd.string)
      objectProperty !!"rdfs:subClassOf" !!(prefix + parentLabel) ]

let moveParent context tuples =
  match context, tuples with
  | _, [] -> context
  | { parent = (parentDepth, _); previous = previous;
      grandparents = grandparents }, (depth, label) :: _ ->
    match depth - parentDepth with
    | 2 ->
      { context with parent = previous
                     previous = (depth, label)
                     grandparents = context.parent :: grandparents }
    | n when n <= 0 ->
      let grandparents = grandparents |> Seq.skip (abs n) |> Seq.toList
      { context with parent = List.head grandparents
                     previous = (depth, label)
                     grandparents = List.tail grandparents }
    | _ -> { context with previous = (depth, label) }

let rec owlGen prefix context tuples =
  match tuples with
  | [] -> []
  | (depth, label) :: tail ->
    let context' = moveParent context tuples
    createOwlResource prefix label (snd context'.parent)
    :: owlGen prefix (moveParent context tuples) tail

let newContext a =
  { parent = a
    previous = a
    grandparents = [] }

let typesFor (file:string) prefix ancestor =
  let csv = FSharp.Data.CsvFile.Load(file)
  csv.Rows
  |> Seq.map (fun a -> a.Columns)
  |> Seq.map Array.toList
  |> Seq.map (depthTuple 1)
  |> Seq.toList
  |> owlGen prefix (newContext (0, ancestor))

let mapSnomed (file:string) prefix =
  let skosDefs = CsvFile.Load(file)
  skosDefs.Rows
  |> Seq.map
       (fun r ->
       rdf.resource !!(prefix + r.Columns.[0])
         [ objectProperty !!"http://www.w3.org/2004/02/skos/core#closeMatch"
             !!("http://bioportal.bioontology.org/ontologies/SNOMEDCT/"
                + r.Columns.[1]) ])
  |> Seq.toList

let mapSynonyms (file:string) prefix =
  let syn = CsvFile.Load(file)
  syn.Rows
  |> Seq.map
       (fun r ->
       rdf.resource !!(prefix + r.Columns.[0])
         [ dataProperty !!"http://www.w3.org/2004/02/skos/core#altLabel"
             (r.Columns.[1] ^^ xsd.string) ])
  |> Seq.toList

do let g = Graph.empty !!"http://ld.nice.org.uk/ns/qualitystandard" []
   let sb = System.Text.StringBuilder()
   let g' = Graph.loadTtl (fromString """
@base <http://ld.nice.org.uk/ns/qualitystandard>.

@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.
@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.
@prefix base: <http://ld.nice.org.uk/ns/qualitystandard>.
@prefix owl: <http://www.w3.org/2002/07/owl#>.

<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Acne> a owl:Class;
                                                                      rdfs:label "Acne"^^xsd:string;
                                                                      rdfs:subClassOf <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>;
                                                                      <http://www.w3.org/2004/02/skos/core#closeMatch> <http://bioportal.bioontology.org/ontologies/SNOMEDCT/88616000>;
                                                                      <http://www.w3.org/2004/02/skos/core#altLabel> "Acne vulgaris"^^xsd:string,
                                                                                                                     "Common acne"^^xsd:string.
<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Acute%20coronary%20syndromes> a owl:Class;
                                                                                              rdfs:label "Acute coronary syndromes"^^xsd:string;
                                                                                              rdfs:subClassOf <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>;
                                                                                              <http://www.w3.org/2004/02/skos/core#closeMatch> <http://bioportal.bioontology.org/ontologies/SNOMEDCT/394659003>.
<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Addiction> a owl:Class;
                                                                           rdfs:label "Addiction"^^xsd:string;
                                                                           rdfs:subClassOf <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>.
<http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#Age%20related%20macular%20degeneration> a owl:Class;
                                                                                                        rdfs:label "Age related macular degeneration"^^xsd:string;
                                                                                                        rdfs:subClassOf <http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#ConditionsAndDiseases>.
""")
   ()
   let g =
     typesFor (root ++ "sample.csv")
       "http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#"
       "ConditionsAndDiseases" |> Assert.graph g
   let g =
     mapSnomed (root ++ "SampleSkosDef.csv" )
       "http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#"
     |> Assert.graph g
   let g =
     mapSynonyms (root ++"SampleSynonyms.csv" )
       "http://ld.nice.org.uk/ns/qualitystandard/conditionsanddiseases#"
     |> Assert.graph g
   let d = Graph.diff g g'
   Graph.writeTtl (toString sb) g
   if not d.AreEqual then
     failwithf "Sample graph doesn't match %s \n ------- \n %s" ((string) d) (string sb)

let appendFile (p) = (System.IO.File.AppendText p) :> System.IO.TextWriter


[ Some "http://ld.nice.org.uk/ns/qualitystandard/agegroup#", Some "AgeGroup",
  Some "Age groups.csv", Some "Age groups synonyms.csv", None

  Some "http://ld.nice.org.uk/ns/qualitystandard/conditiondisease#",
  Some "ConditionDisease", Some "Conditions and diseases.csv",
  Some "Conditions and diseases synonyms.csv",
  Some "Conditions and diseases to snomed mapping.csv"

  Some "http://ld.nice.org.uk/ns/qualitystandard/lifestylecondition#",
  Some "LifestyleCondition", Some "Lifestyle conditions.csv",
  Some "Lifestyle conditions synonyms.csv", None

  Some "http://ld.nice.org.uk/ns/qualitystandard/servicearea#", Some "ServiceArea",
  Some "Service areas.csv", Some "Service area synonyms.csv", None

  Some "http://ld.nice.org.uk/ns/qualitystandard/setting#", Some "Setting",
  Some "Settings.csv", Some "Settings synonyms.csv", None ]
|> List.map
     (fun ((Some prefix), (Some root), (Some types), (Some synonyms), snomed) ->
     (root.ToLower(), List.concat [ typesFor (fPath ++ types) prefix root
                                    mapSynonyms (fPath ++ synonyms) prefix

                                    Option.toList snomed
                                    |> List.collect (fun snomed -> mapSnomed (fPath ++ snomed) prefix) ]))
|> List.iter (fun (name,xr) ->
   let g = Graph.empty !!("http://ld.nice.org.uk/ns/qualitystandard/" + name) []
   Assert.graph g xr |> ignore

   let fout = root ++ "../ns/qualitystandard" ++ (sprintf "%s.ttl" name)
   printfn "Write csv derived ontology to %s" fout

   Graph.writeTtl ( appendFile (fout) ) g
)

