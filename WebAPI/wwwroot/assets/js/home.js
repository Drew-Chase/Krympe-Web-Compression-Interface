(() => {
    $("#refresh-watched-directories").on('click', () => {
        PopulateWatchDirectories();
    })

    var updateInteraval = setInterval(() => {
        UpdateActiveProcesses()
    }, 500)

    PopulateWatchDirectories()
    PopulateActiveProcesses()
    async function PopulateActiveProcesses() {
        clearInterval(updateInteraval);
        let section = $("#active-processes-section")[0];
        let list = section.querySelector(".list");
        list.innerHTML = ""
        let response = await fetch(`/api/process/active`, { method: "GET" });
        let json = (await response.json())["active"];
        for (let i = 0; i < json.length; i++) {
            let folder = json[i];
            list.append(makeFileItem(folder["name"], `${folder["currentSize"]} / ${folder["totalSize"]} ${folder["percentage"]}%`))
        }
        if (json.length != 0) {
            updateInteraval = setInterval(() => {
                UpdateActiveProcesses()
            }, 500)
        }
    }

    async function UpdateActiveProcesses() {
        let response = await fetch(`/api/process/active`, { method: "GET" });
        let json = (await response.json())["active"];
        for (let i = 0; i < json.length; i++) {
            let folder = json[i];
            $(`#item-${folder["name"].replaceAll(" ", "-").replaceAll(".", "-").replaceAll("(", "-").replaceAll(")", "-")} .details`)[0].innerText = `${folder["currentSize"]} / ${folder["totalSize"]} ${folder["percentage"]}%`;
        }
    }

    async function PopulateWatchDirectories() {
        let section = $("#watch-directories-section")[0];
        let list = section.querySelector(".list");
        list.innerHTML = ""
        let response = await fetch(`/api/fs/watch`, { method: "GET" });
        let json = await response.json();
        for (let i = 0; i < json.length; i++) {
            let folder = json[i];
            list.append(makeFolder(folder["path"], folder["size"], [
                makeButton("Remove Watched Directory", "fas fa-minus", async () => {
                    let data = new FormData();
                    data.append('path', folder["path"]);
                    await fetch('/api/fs/watch', { method: "DELETE", body: data })
                    PopulateWatchDirectories();
                }),
                makeButton("Add to Active Process", "fas fa-plus", async () => {
                    let data = new FormData();
                    data.append('path', folder["path"]);
                    await fetch('/api/process/active', { method: "POST", body: data })
                    setTimeout(() => PopulateActiveProcesses(), 1000)
                }),
            ]))
        }
    }
}).call()