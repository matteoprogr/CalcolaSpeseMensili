
document.getElementById("form").addEventListener("submit", function (e) {
    e.preventDefault();
    let myform = new FormData(this);

    fetch("/Home/calcolaspese", {
        method: "post",
        body: myform
    }).then(risposta => risposta.json())
        .then(data => {
            console.log(data);
            
            creaTabella(data);
        })
})

function unisci() {

    // ottengo la tabella presente nel div
    const tabella = document.getElementById("tab");

    // ottengo tutti gli elementi tr presenti nella tabella
    let righe = tabella.querySelectorAll("tr");

    // ottengo il testo inserito in nomeRiga
    const nome = document.getElementById("nome").value;
    
   
    let dataUniti = {};
    // itero sulle righe 
    righe.forEach(riga => {
        //ottengo tutti gli elementi td presenti nella riga
        let celle = riga.querySelectorAll("td");
        // ottengo la checkbox presente nella riga
        let check = riga.querySelector("input[type='checkbox']");

        //se la checkbox è selezionata unisco i valori presenti in quelle righe tra loro
        // assegno nomeRiga come nome della proprietà dell'oggetto
        if (check != null && check.checked) {
            if (nome in dataUniti) {
                let valore = dataUniti[nome] + Number(celle[1].innerHTML);
                dataUniti[nome] = valore;
            } else {
                dataUniti[nome] = Number(celle[1].innerHTML);
            }

        } else if (check != null && !check.checked) {
            dataUniti[celle[0].innerHTML] = Number(celle[1].innerHTML);
        }
        
    })
    console.log(dataUniti)
    // rimuovo la tabella precedente 
    tabella.remove();
    // creo la nuova tabella con i dati presenti nell'oggetto dataUniti
    creaTabella(dataUniti);
    
}

function creaTabella(dataUniti) {

    // creo la tabella che conterra i valori di dataUniti
    let tabellaUnita = "<table id='tab'><tr><th>Tipo spesa</th><th>Spesa</th></tr>";

   

    // creo una nuova riga per ogni proprietà presente in dataUniti
    for (let key in dataUniti) {
        tabellaUnita += "<tr><td>" + key + "</td><td>" + Number(dataUniti[key]).toFixed(1) + "</td>" + "<td><input type='checkbox'></td></tr>";
    }
    tabellaUnita += "</table>";

    // reecupero il div che conterrà la tabella e inserisco la tabella creata nel div
    let divtab = document.getElementById("tabella");
    divtab.innerHTML = tabellaUnita;

    //creo un bottone(unisciRighe) e un elemento di input text(nomeRiga)
    // a unisciRighe addiziono un evento che permette di sommare le righe della tabella tra loro
    // nomeRiga permette di dare un nome alla riga creata
    let unisciRighe = document.createElement("button");
    unisciRighe.textContent = "Unisci righe selezionate";
    unisciRighe.classList = "marginAll";
    unisciRighe.addEventListener("click", function (e) {
        e.preventDefault();
        unisci();
    });

    const nomeRiga = document.createElement("input");
    nomeRiga.classList = "marginAll";
    nomeRiga.type = "text";
    nomeRiga.id = "nome";
    nomeRiga.placeholder = "Nome nuova riga";
    nomeRiga.required = true;


    divtab.appendChild(unisciRighe);
    divtab.appendChild(nomeRiga);
}

