module Gubbins
#r "../FSharp/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data

let workingDir = "/Users/Nate/_src/ld-content-test/qualitystandards/"

//qs
let listOfQStatements = "https://api.import.io/store/data/50414792-2b81-40a1-aa8b-bff104926c6f/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/list-of-quality-statements&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let listOfQStandards = "https://api.import.io/store/data/d5c4f2ed-2def-40aa-976b-9101273a8158/_query?input/webpage/url=https://www.nice.org.uk/guidance/published?type=QS&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let QStatement = "https://api.import.io/store/data/7a17c2ca-2960-4181-90a3-ed998a28d184/_query?input/webpage/url=guidance/qs68/chapter/quality-statement-1-diagnosis-of-acute-myocardial-infarction&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

//e.g. qs22 st7
let replacedQStatements = https://api.import.io/store/data/483f329a-1f24-405f-88aa-7bc44bdd32b7/_query?input/webpage/url=https://www.nice.org.uk//guidance/qs22/chapter/quality-statement-7-risk-assessment-pre-eclampsia&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6
//gubbins
let intro = "https://api.import.io/store/data/4251357d-0feb-479f-9cf9-eebec3e07481/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/introduction&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let using = "https://api.import.io/store/data/a50dbe3d-9f08-45bb-b49f-13e716232ba2/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/using-the-quality-standard&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let del = "https://api.import.io/store/data/c8bc04f9-bbe9-46a4-9487-08ced3a7b398/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/diversity-equality-and-language&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g="

let devSources = "https://api.import.io/store/data/4c4aa030-7a1f-45ee-9ac2-d226843d5b66/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/development-sources&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let related = "https://api.import.io/store/data/eefae6b2-5bfc-432d-8e89-d65f59819d27/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/related-nice-quality-standards&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let advisory = "https://api.import.io/store/data/008f0baf-bffa-408b-81c6-f8b2b283cb83/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/quality-standards-advisory-committee-and-nice-project-team&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

let about = "https://api.import.io/store/data/c90fd4ad-3618-4cfc-899e-96436f0ae8b6/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/about-this-quality-standard&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g=="

//qs
type html = HtmlProvider<"""<p><a class="link" href="/guidance/qs68/chapter/quality-statement-1-diagnosis-of-acute-myocardial-infarction" target="_top" nice-original-href="http://publications.nice.org.uk/acute-coronary-syndromes-including-myocardial-infarction-qs68/quality-statement-1-diagnosis-of-acute-myocardial-infarction">Statement 1</a>. Adults with a suspected acute coronary syndrome are assessed for acute myocardial infarction using the criteria in the universal definition of myocardial infarction.</p>""">

type allQS = JsonProvider<"https://api.import.io/store/data/d5c4f2ed-2def-40aa-976b-9101273a8158/_query?input/webpage/url=https://www.nice.org.uk/guidance/published?type=QS&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type QStatements = JsonProvider<"https://api.import.io/store/data/e0038a2e-5320-45d6-bc92-2501132a2261/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/list-of-quality-statements&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type QStatementProv = JsonProvider<"https://api.import.io/store/data/7a17c2ca-2960-4181-90a3-ed998a28d184/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/quality-statement-1-diagnosis-of-acute-myocardial-infarction&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type ReplacedQStatement = JsonProvider<"https://api.import.io/store/data/483f329a-1f24-405f-88aa-7bc44bdd32b7/_query?input/webpage/url=https://www.nice.org.uk//guidance/qs22/chapter/quality-statement-7-risk-assessment-pre-eclampsia&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf3657662253418e9458ab6538072d48f224ab47080be847e8b6031358fb467ab6b4ba663456b7a14f09df680ed3f229d9a166831e5727617a345520d51abd1da25803960471b8943bddd01fae495fe6">

//gubbins
type Intro = JsonProvider<"https://api.import.io/store/data/4251357d-0feb-479f-9cf9-eebec3e07481/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/introduction&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type Using = JsonProvider<"https://api.import.io/store/data/a50dbe3d-9f08-45bb-b49f-13e716232ba2/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/using-the-quality-standard&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type Del = JsonProvider<"https://api.import.io/store/data/c8bc04f9-bbe9-46a4-9487-08ced3a7b398/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/diversity-equality-and-language&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type DevSources = JsonProvider<"https://api.import.io/store/data/4c4aa030-7a1f-45ee-9ac2-d226843d5b66/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/development-sources&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type Related = JsonProvider<"https://api.import.io/store/data/eefae6b2-5bfc-432d-8e89-d65f59819d27/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/related-nice-quality-standards&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type Advisory = JsonProvider<"https://api.import.io/store/data/008f0baf-bffa-408b-81c6-f8b2b283cb83/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/quality-standards-advisory-committee-and-nice-project-team&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">

type About = JsonProvider<"https://api.import.io/store/data/c90fd4ad-3618-4cfc-899e-96436f0ae8b6/_query?input/webpage/url=https://www.nice.org.uk/guidance/qs68/chapter/about-this-quality-standard&_user=cf365766-2253-418e-9458-ab6538072d48&_apikey=cf365766-2253-418e-9458-ab6538072d48:8iSrRwgL6EfotgMTWPtGera0umY0VrehTwnfaA7T8inZoWaDHlcnYXo0VSDVGr0dolgDlgRxuJQ73dAfrklf5g==">
