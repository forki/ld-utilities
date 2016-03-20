module Providers
#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.DesignTime.dll"
#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data

let workingDir = "/Users/Nate/_src/2016/qualitystandards/"

let listOfQStandards = "https://api.import.io/store/data/d5c4f2ed-2def-40aa-976b-9101273a8158/_query?input/webpage/url=https://www.nice.org.uk/guidance/published?type=QS&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

type QualityStandardList = JsonProvider<"https://api.import.io/store/data/d5c4f2ed-2def-40aa-976b-9101273a8158/_query?input/webpage/url=https://www.nice.org.uk/guidance/published?type=QS&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">
//qs

type ListOfStatements = JsonProvider<"https://api.import.io/store/connector/e0038a2e-5320-45d6-bc92-2501132a2261/_query?input=webpage/url:https://www.nice.org.uk/guidance/qs68/chapter/list-of-quality-statements&&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6">

let ListOfStatementsUrl qsNo = sprintf "https://api.import.io/store/connector/e0038a2e-5320-45d6-bc92-2501132a2261/_query?input=webpage/url:https://www.nice.org.uk/guidance/%s/chapter/list-of-quality-statements&&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6" qsNo






let listOfQStatements = "https://api.import.io/store/data/50414792-2b81-40a1-aa8b-bff104926c6f/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/list-of-quality-statements&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let QStatement = "https://api.import.io/store/data/7a17c2ca-2960-4181-90a3-ed998a28d184/_query?input/webpage/url=guidance/qs68/chapter/quality-statement-1-diagnosis-of-acute-myocardial-infarction&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

//e.g. qs22 st7
let replacedQStatements = "https://api.import.io/store/data/483f329a-1f24-405f-88aa-7bc44bdd32b7/_query?input/webpage/url=https://www.nice.org.uk//guidance/qs22/chapter/quality-statement-7-risk-assessment-pre-eclampsia&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6"


type html = HtmlProvider<"""<p><a class="link" href="/guidance/qs68/chapter/quality-statement-1-diagnosis-of-acute-myocardial-infarction" target="_top" nice-original-href="http://publications.nice.org.uk/acute-coronary-syndromes-including-myocardial-infarction-qs68/quality-statement-1-diagnosis-of-acute-myocardial-infarction">Statement 1</a>. Adults with a suspected acute coronary syndrome are assessed for acute myocardial infarction using the criteria in the universal definition of myocardial infarction.</p>""">



type QStatementProv = JsonProvider<"https://api.import.io/store/data/7a17c2ca-2960-4181-90a3-ed998a28d184/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/quality-statement-1-diagnosis-of-acute-myocardial-infarction&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type ReplacedQStatement = JsonProvider<"https://api.import.io/store/data/483f329a-1f24-405f-88aa-7bc44bdd32b7/_query?input/webpage/url=https://www.nice.org.uk//guidance/qs22/chapter/quality-statement-7-risk-assessment-pre-eclampsia&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6">

