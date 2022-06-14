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