var Validator = require('jsonschema').Validator;
const https = require('https');
const url = require('url');

const personSchemaURL = "https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/person_schema.json"
const commentSchemaURL = "https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/comment_schema.json"
const commentInstance = `{
    "name": "velit amet",
    "value": "minim ut reprehenderit cillum commodo"
  }`;
const personInstance = `{
    "phone": "",
    "firstName": "Juan",
    "address": "Oxford Road, Manchester M13 9PT, UK",
    "lastName": "Castrillo",
    "midInitials": "I",
    "@id": "#person/Castrillo",
    "fax": "",
    "email": "lol@lal.das",
    "comments": [
      {
        "value": "",
        "name": "Investigation Person REF"
      }
    ],
    "roles": [
      {
        "annotationValue": "author"
      }
    ],
    "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
  }`

function resolveUrl(baseurl, relative) {
    parts = baseurl.split('/');
    parts.pop();
    baseurl = parts.join('/');
    baseurl = baseurl + relative
    let newUrl = new URL(baseurl)
    return newUrl;
}

function getSchema(nextSchema, callback) {
    let rawData = '';
    console.log("Get next ref:", nextSchema)
    https.get(nextSchema, res => {
        res.on('data', chunk => { rawData += chunk }) 
        res.on('end', () => { 
            try {
                const schemaParsed = JSON.parse(rawData);
                callback(schemaParsed)
                console.log("Finish:", nextSchema)
            } catch (e) {
                console.error(e.message);
            }
        })
    })
}

/**
 * 
 * @param {string} instance json string to be checked for validation against schema
 * @param {url} schemaUrl url to raw json schema
 * @returns 
 */
export async function validateAgainstSchema(instance, schemaUrl) {
    const instanceParsed = JSON.parse(instance)
    var v = new Validator();
    let rawData = '';
    return new Promise((resolve) => {
        https.get(schemaUrl, res => {

            res.on('data', chunk => { rawData += chunk }) 

            res.on('end', () => {
                try {
                    const schemaParsed = JSON.parse(rawData);
                    v.addSchema(schemaParsed);
                    console.log("[Init Schema]")
                    function importNextSchema(){
                        let nextSchema = v.unresolvedRefs.shift();
                        if(!nextSchema){ 
                            console.log("[Start Validation]")
                            let validation = v.validate(instanceParsed, schemaParsed)
                            console.log("[Result]", validation)
                        } else {
                            const nextSchemaUrl = resolveUrl(schemaUrl, nextSchema).href
                            getSchema(nextSchemaUrl, function(schema){
                                v.addSchema(schema, nextSchema)
                                importNextSchema();
                            });
                        }
                    }
                    importNextSchema()
                } catch (e) {
                    console.error(e.message);
                }
            })
        }) 
    })
}

// validateAgainstSchema(commentInstance, commentSchemaURL);
validateAgainstSchema(personInstance, personSchemaURL);