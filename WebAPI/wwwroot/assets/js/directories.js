PopulateWatchDirectories()
PopulateWatchedExtensionsList()

$("#refresh-list").on('click', async e => {
    let button = e.currentTarget;
    if (!button.classList.contains('disabled')) {
        button.classList.add('disabled')
        $("#watched-directories-section .list")[0].innerHTML = ""

        load(`<h1>Refreshing Cached Drives:</h1><p>This will take a few minutes</p>`);

        $(window).bind("beforeunload", e => {
            return "You are currently trying to refresh cached drives\nAre you sure you want to leave?"
        })
        let response = await fetch('/api/fs/refresh', { method: "POST" });
        if (response.ok) {
            PopulateWatchDirectories();
        }
        unload();
        button.classList.remove('disabled')
        $(window).unbind("beforeunload")
    }
})

async function PopulateWatchDirectories(cd = "") {
    let section = $("#watched-directories-section")[0];
    let list = section.querySelector(".list");
    list.innerHTML = ""

    let data = new FormData();
    data.append("path", cd);
    let response = await fetch(`/api/fs`, { method: "POST", body: data });
    let json = await response.json();

    if (cd == "") {
        for (let i = 0; i < json["drives"].length; i++) {
            let obj = json["drives"][i];
            let folder = makeFolder(obj["name"], `${obj["humanReadableTotalFreespace"]} / ${obj["humanReadableTotalSize"]}`, []);
            $(folder).on('click', () => {
                PopulateWatchDirectories(obj["name"])
            })
            list.append(folder);
        }
    } else {
        for (let i = 0; i < json["folders"].length; i++) {
            let obj = json["folders"][i];
            let folder;
            if (obj["name"] == "..") {
                folder = makeFolder(obj["name"], ``, []);
            } else {
                folder = makeFolder(obj["name"], `${obj["size"]}`, [
                    makeButton("Added to Watched Directories", "fas fa-plus", async () => {
                        let data = new FormData();
                        data.append("path", obj["path"])
                        await fetch("/api/fs/watch", { method: "POST", body: data });
                        folder.remove();
                    }),
                ]);
            }
            $(folder).on('click', e => {
                if (!e.target.classList.contains("btn"))
                    PopulateWatchDirectories(obj["path"])
            })
            list.append(folder);
        }
    }
}

async function PopulateWatchedExtensionsList() {
    let section = $("#watched-file-extensions")[0];
    let list = section.querySelector('.list');
    list.innerHTML = "";
    let response = await fetch('/api/config');
    let json = await response.json();
    let extensions = json["extensions"];
    Array.from(extensions).forEach(e => {
        list.append(makeFolder(e, "", [
            makeButton("Remove", "fas fa-minus", () => {
            })
        ]))
    })
}