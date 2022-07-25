const fs = require("fs");

const baseDir = process.argv.length > 3 ? `${process.argv[3]}` : ".";

const configDir = `${baseDir}/config`;
const env = process.argv[2];

let data = JSON.parse(fs.readFileSync(`${configDir}/config.json`));

data = { ...data, ...JSON.parse(fs.readFileSync(`${configDir}/config.${env}.json`)) };

fs.writeFile(`${baseDir}/public/config.json`, JSON.stringify(data), (err) => {
    if (err) throw err;
    console.log(data);
});
