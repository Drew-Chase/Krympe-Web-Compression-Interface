Array.from($(".toggle-field")).forEach(e => {
    $(e).on('click', () => {
        e.setAttribute("value", e.getAttribute("value") == "false")
    })
});
Array.from($(".folder-item")).forEach(e => {
    $(e.querySelector(".detail-section")).on('click', () => {
        e.classList.toggle("expanded")
    })
})

function setProgress(element, value) {
    element.querySelector(".handle").style.width = `${value <= 1 ? value * 100 : value}%`
}

function getToggle(element) {
    return element.getAttribute("value") == "true";
}

function makeFileItem(name, details) {
    let item = document.createElement('div')
    item.id = `item-${name.replaceAll(" ", "-").replaceAll(".", "-").replaceAll("(", "-").replaceAll(")", "-")}`
    item.classList.add("file-item");
    let detailsSection = document.createElement('div')
    detailsSection.classList.add('detail-section');
    let nameText = document.createElement('p');
    nameText.classList.add('name')
    nameText.innerText = name;
    let detailsText = document.createElement('p')
    detailsText.classList.add('details');
    detailsText.innerText = details;

    detailsSection.append(nameText)
    detailsSection.append(detailsText)
    item.appendChild(detailsSection)

    return item;
}
function makeFolder(name, details, buttons) {
    let item = document.createElement('div');
    item.classList.add('folder-item')
    let quickAccess = document.createElement('div')
    quickAccess.classList.add('quick-access')
    let detailsSection = document.createElement('div');
    detailsSection.classList.add('detail-section')
    let nameText = document.createElement('p');
    nameText.classList.add('name')
    nameText.innerText = name;
    let detailsText = document.createElement('p');
    detailsText.classList.add('details')
    detailsText.innerText = details;

    detailsSection.appendChild(nameText)
    detailsSection.appendChild(detailsText)
    buttons.forEach(e => detailsSection.appendChild(e));
    quickAccess.appendChild(detailsSection);
    item.appendChild(quickAccess);
    return item;
}

function makeButton(title, icon, action) {
    let btn = document.createElement('div');
    btn.classList.add('btn');
    Array.from(icon.split(' ')).forEach(e => btn.classList.add(e))
    btn.title = title;
    $(btn).on('click', action);
    return btn;
}