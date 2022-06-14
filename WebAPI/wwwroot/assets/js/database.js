PopulateSuccessfullyCompressed()
PopulateFailedCompress()
PopulateToBeCompress()
function PopulateSuccessfullyCompressed() {
    let section = $("#successfully-compressed-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""
    for (let i = 0; i < 36; i++) {
        let size = Math.ceil(Math.random() * (i + Math.random()) * 100)
        let newSize = Math.ceil(size*.6);
        list.append(makeFileItem(`File #${i}`, `${newSize} Gb / ${size} Gb`))
    }
}
function PopulateFailedCompress() {
    let section = $("#failed-compress-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""
    for (let i = 0; i < 36; i++) {
        list.append(makeFileItem(`File #${i}`, `Reason: Original File was smallest already!`))
    }
}
function PopulateToBeCompress() {
    let section = $("#tobe-compressed-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""
    for (let i = 0; i < 36; i++) {
        list.append(makeFileItem(`File #${i}`, `4.5 Gb`))
    }
}