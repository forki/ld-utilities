module Owl =
    open org.semanticweb.owlapi.apibinding
    open org.semanticweb.owlapi.model
    open org.semanticweb.owlapi.util
    open org.semanticweb.owlapi.io
    open org.coode.owlapi.manchesterowlsyntax
    open uk.ac.manchester.cs.owl.owlapi
    open System.IO
    open ikvm.io
    open System
    open Nessos.UnionArgParser
    open org.coode.owlapi.turtle
    open java.io

    type Ontology =
        | Ontology of OWLOntology

    type LoadOntology =
        | Success of Ontology
        | Error of string

    type CommandArguments =
        | Uri of string
        interface IArgParserTemplate with
            member u.Usage =
                match u with
                | Uri _ -> "Specify a file"

    let oM = OWLManager.createOWLOntologyManager()

    let loadFromFile p =
        try
            let ont =
                oM.loadOntologyFromOntologyDocument
                    (new java.io.File(p : string))
            Success(Ontology ont)
        with
        | :? OWLOntologyAlreadyExistsException as e ->
            Success(Ontology(oM.getOntology(e.getDocumentIRI()))) //doesnt work at the moment
        | ex -> Error(sprintf "%s \n %s" ex.Message ex.StackTrace)

    let translate ont p fname =
        let fileToWrite = File(p + fname + ".ttl")
        let output = new FileOutputStream(fileToWrite)
        match ont with
        | Ontology.Ontology ont ->
            try
                oM.saveOntology(ont, TurtleOntologyFormat(), output)
                oM.removeOntology(ont) //remove, allows for future loading of same ontology
            with ex -> printfn "%s" ex.Message

open Nessos.UnionArgParser
[<EntryPoint>]
let main argv =
    let parser = UnionArgParser.Create<Owl.CommandArguments>()
    let argv = parser.Parse(argv)
    let fin = argv.GetResult(<@ Owl.CommandArguments.Uri @>)
    let startTrim = fin.LastIndexOf('/')+1
    let writeTo = fin.Substring(0, fin.LastIndexOf('/')+1)
    let fileName = fin.Substring(startTrim, fin.LastIndexOf('.')-startTrim)
    match Owl.loadFromFile fin with
    | Owl.Error e ->
        printfn "%s" e
        exit 1
    | Owl.Success ont -> Owl.translate ont writeTo fileName
    0 // return an integer exit code
