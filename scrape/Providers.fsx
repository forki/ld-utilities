module Providers
#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.DesignTime.dll"
#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data

let workingDir = "/Users/Nate/_src/2016/qualitystandards/"

//List all Standards
let listOfQStandards = "https://api.import.io/store/data/d5c4f2ed-2def-40aa-976b-9101273a8158/_query?input/webpage/url=https://www.nice.org.uk/guidance/published?type=QS&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

type QualityStandardList = JsonProvider<"https://api.import.io/store/data/d5c4f2ed-2def-40aa-976b-9101273a8158/_query?input/webpage/url=https://www.nice.org.uk/guidance/published?type=QS&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">


//List all statements within a standard
type ListOfStatements = JsonProvider<"jsonsamples/ListOfStatements.json", SampleIsList = true >

let ListOfStatementsUrl qsNo = sprintf "https://api.import.io/store/connector/e0038a2e-5320-45d6-bc92-2501132a2261/_query?input=webpage/url:https://www.nice.org.uk/guidance/%s/chapter/list-of-quality-statements&&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6" qsNo


//Get content from within a statement
type jp = JsonProvider<"https://api.import.io/store/connector/df5b2807-aeb0-412c-a253-1d923a27a367/_query?input=webpage/url:https://www.nice.org.uk/guidance/qs36/chapter/quality-statement-1-presentation-with-unexplained-fever-of-38c-or-higher&&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6">


let StatementExtractorBuilder uri = sprintf "https://api.import.io/store/connector/df5b2807-aeb0-412c-a253-1d923a27a367/_query?input=webpage/url:%s&&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6" uri

