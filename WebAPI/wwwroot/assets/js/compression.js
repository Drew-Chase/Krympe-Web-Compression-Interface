(() => {
    Array.from($("input[cmd-arg]")).forEach(e => {
        $(e).on('keyup', f => {
            BuildCommand();
        })
    });
    Array.from($("input[pre-cmd-arg]")).forEach(e => {
        $(e).on('keyup', f => {
            BuildCommand();
        })
    })
    $("#test-ffmpeg-command-btn").on('click', async e => {
        let button = e.currentTarget;
        if (!button.classList.contains('disabled')) {
            let success = `<i class="fa-solid fa-circle-check"></i>`;
            let failed = `<i class="fa-solid fa-circle-xmark"></i>`;
            let unset = `<i class="fa-solid fa-vial"></i>`;
            let loading = `<i class="loading"></i>`;
            button.innerHTML = "";
            button.classList.add('disabled')
            button.innerHTML = loading;

            let response = await fetch('/api/process/test');
            let json = await response.json();

            if (json["working"] == true) {
                button.innerHTML = success
                button.style.backgroundColor = "green"
            } else {
                button.innerHTML = failed
                button.style.backgroundColor = "red"
            }
            button.classList.remove("loading");

            setTimeout(() => {
                button.innerHTML = "";
                button.innerHTML = unset
                button.classList.remove("loading");
                button.style.backgroundColor = ""
                button.classList.remove("disabled");
            }, 5 * 1000);
        }
    })

    BuildCommand();
    function BuildCommand() {
        let cmd = `ffmpeg`
        Array.from($("input[pre-cmd-arg]")).forEach(e => {
            let value = e.value;
            if (value != "") {
                let argument = $(e).attr("pre-cmd-arg");
                cmd += ` ${argument} ${value} `
            }
        });
        cmd += "-y -i {INPUT} -loglevel quiet";
        Array.from($("input[cmd-arg]")).forEach(e => {
            let value = e.value;
            if (value != "") {
                let argument = $(e).attr("cmd-arg");
                if ($(e).attr("sub-arg") != null && $(e).attr("sub-arg") != "") {
                    cmd += ` ${argument} "${$(e).attr("sub-arg")}=${value}"`
                } else {
                    cmd += ` ${argument} ${value}`
                }
            }
        })
        cmd += " {OUTPUT}"
        $("#ffmpeg-command").attr('value', cmd);
        Save();
    }
}).call();