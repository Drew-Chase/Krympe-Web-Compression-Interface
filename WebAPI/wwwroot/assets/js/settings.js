const token = "4a36cd4c-5e3c-4bc4-8deb-ec9a416a5c84"
function init() {
    Array.from($("[setting]")).forEach(async e => {
        let response = await fetch(`/api/config?token=${token}`)
        let json = await response.json();
        $(e).attr("value", json[$(e).attr("setting")]);
        $(e).on('keyup', async () => {
            $(e).attr("value", e.value);
            Save()
        })
        $(e).on('click', async () => {
            $(e).attr("value", e.value);
            Save()
        })
    })
}

function Save() {
    Array.from($("[setting]")).forEach(async e => {
        let setting = $(e).attr("setting");
        let value = $(e).attr("value");

        let data = new FormData();
        data.append("name", setting);
        data.append("value", value);
        await fetch(`/api/config/set`, { method: "POST", body: data })
    })
}