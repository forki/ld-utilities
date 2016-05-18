# ld-utilities

A collection of utilities tool -

* omn2ttl - Converting Manchester OWL Syntax to Turtle format.
* ComLLODE - A fork of [LODE](https://github.com/essepuntato/LODE) which has been altered to work via command line. Generating documentation from ontologies.
* csv2skos - CSV document into a SKOS ontology

#Steps to do a scrape

* run QSImport.fsx with appropriate QS statement No.
* find . -name "*.html" | xargs -n 1 ../../html2md.sh
* find . -name "Statement.md" -exec sed -i -e 's/{.title}//g' {} \;
* find . -iname "*.html" -delete
* find . -iname "*.md-e" -delete

#Steps to put in YAML

Make sure you have a folder structure setup like so:
  /ld-content-update
    /with
      /qualitystandard/qs1/st1/
      ...
    /without
      /qualitystandard/qs1/st1/
* run YamlSwap.fsx File.MoveAnnotation line
