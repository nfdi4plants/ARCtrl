import { Validator, ValidatorResult } from 'jsonschema';
import * as https from 'https';
import * as url from 'url'

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
    try {
        let parts = '';
        parts = baseurl.split('/');
        parts.pop();
        baseurl = parts.join('/');
        baseurl = baseurl + relative
        let newUrl = new URL(baseurl)
        return newUrl;
    } catch (error) {
        console.error("Unable to resolve reference schema url: ", error)
    }
}

function getSchema(nextSchema, callback) {
    let rawData = '';
    // console.log("Get next ref:", nextSchema)
    https.get(nextSchema, res => {
        res.on('data', chunk => { rawData += chunk }) 
        res.on('end', () => { 
            try {
                const schemaParsed = JSON.parse(rawData);
                callback(schemaParsed)
                // console.log("Finish:", nextSchema)
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
function validateAgainstSchema(instance, schemaUrl) {
    const instanceParsed = JSON.parse(instance)
    var v = new Validator();
    return new Promise((resolve, reject) => {
        https.get(schemaUrl, res => {
            let data = '';
            // console.log("[JS] Start")
            // A chunk of data has been received
            res.on('data', chunk => { data += chunk });
            
            // The whole response has been received
            res.on('end', () => {
                try {
                    // console.log("[JS] parse schema")
                    const schemaParsed = JSON.parse(data);
                    // console.log("[JS] add main parse schema")
                    v.addSchema(schemaParsed);
                    // console.log("[JS] start import ref schema")
                    function importNextSchema(){
                        let nextSchema = v.unresolvedRefs.shift();
                        // console.log("[JS] start resolving ref schema")
                        if(!nextSchema){ 
                            // console.log("[JS] start validate")
                            let validation = v.validate(instanceParsed, schemaParsed);
                            // console.log("[JS] finish validate")
                            resolve(validation);
                        } else {
                            const nextSchemaUrl = resolveUrl(schemaUrl, nextSchema).href
                            getSchema(nextSchemaUrl, function(schema){
                                // console.log("[JS] get ref schema:", schema)
                                v.addSchema(schema, nextSchema)
                                importNextSchema();
                            });
                        }
                    }
                    importNextSchema()
                } catch (e) {
                    reject(e);
                }
            });
        }).on('error', err => {
            reject(err);
        });
    });
}

function helloWorld () {
    return "Hello World"
} 

// validateAgainstSchema(commentInstance, commentSchemaURL);
// validateAgainstSchema(personInstance, personSchemaURL);
export { validateAgainstSchema, helloWorld };