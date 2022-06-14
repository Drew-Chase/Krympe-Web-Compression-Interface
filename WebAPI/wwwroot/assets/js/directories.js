PopulateWatchDirectories()
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