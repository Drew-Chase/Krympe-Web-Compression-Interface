PopulateSuccessfullyCompressed()
PopulateFailedCompress()
PopulateToBeCompress()
async function PopulateSuccessfullyCompressed() {
    let section = $("#successfully-compressed-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""
    for (let i = 0; i < 0; i++) {
        let size = Math.ceil(Math.random() * (i + Math.random()) * 100)
        let newSize = Math.ceil(size*.6);
        list.append(makeFileItem(`File #${i}`, `${newSize} Gb / ${size} Gb`))
    }

    let response = await fetch('/api/process/successful');
    let json = await response.json();
    let size = json["size"];
    let count = json["count"];
    $("#successfully-compressed-total-size")[0].innerHTML = `${size} - ${count} Files`
    Array.from(json["processes"]).forEach(e => {
        list.append(makeFileItem(e["name"], `${e["size"]} / ${e["original"]}`))
    })
}
async function PopulateFailedCompress() {
    let section = $("#failed-compress-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""

    let response = await fetch('/api/process/failed');
    let json = await response.json();
    let count = json["count"];
    $("#failed-compress-extra")[0].innerHTML = `${count} Files`
    Array.from(json["processes"]).forEach(e => {
        list.append(makeFileItem(e["name"], `Reason: ${e["reason"]}`))
    })

    for (let i = 0; i < 0; i++) {
        list.append(makeFileItem(`File #${i}`, `Reason: Original File was smallest already!`))
    }
}
async function PopulateToBeCompress() {
    let section = $("#tobe-compressed-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""
    let response = await fetch('/api/process/todo');
    let json = await response.json();
    let size = json["size"];
    let count = json["count"];
    $("#tobe-compressed-extra")[0].innerHTML = `${size} - ${count} Files`
    Array.from(json["processes"]).forEach(e => {
        list.append(makeFileItem(e["name"], e["size"]))
    })
}