const fs = require('fs');


const categories = ['Water','Heat','Parking','Gas','Smthing']

function getRandomFloat(min, max, decimals) {
    const str = (Math.random() * (max - min) + min).toFixed(decimals);
  
    return parseFloat(str);
}

function createLine() {
    let payment = getRandomFloat(0.1, 10000, 3);
    let year = getRandomFloat(20, 24, 0);
    let day = getRandomFloat(1, 27, 0);
    let month = getRandomFloat(1, 12, 0);
    let id = getRandomFloat(1,100_000,0);
    let city_id = getRandomFloat(1,40,0);
    let category = categories[Math.floor(Math.random() * categories.length)];
    return `FirstName${id},
    LastName, 
    "City${city_id}, Kleparivska 35, 4",
    ${payment},20${year}-${day<10 ? '0' : ''}${day}-${month<10 ? '0' : ''}${month},
    ${getRandomFloat(1000000,9000000)}, 
    ${category}`.replaceAll("\n","");
}

const maxSize = 4;
const baseSize = 2;
const repeats = 10;
let writeCsv = false;
for(let repeat=0;repeat<repeats;repeat++) {
    for (let multiplicator = 10; multiplicator < Math.pow(10, maxSize); multiplicator = multiplicator * 10) {
        let content = '';
        if (writeCsv) {
            content += 'col1,col2,col3,col4,col5,col6,col7\n';
        }
        for (let fileIndex = 0; fileIndex < baseSize * multiplicator; fileIndex++) {
            content += createLine() + '\n';
        }
        const path = `./watch1/${multiplicator * baseSize}_${getRandomFloat(0, 2000, 0)}.${writeCsv ? 'csv' : 'txt'}`;
        fs.writeFile(path, content, err => console.log(err));
        writeCsv = !writeCsv;
    }
}