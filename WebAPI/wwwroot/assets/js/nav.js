(() => {
    $("#menu-button").on('click', () => {
        $("#nav-bar")[0].classList.toggle("active");
    })
    Array.from($(".menu-item")).forEach(e => {
        let page = `${e.querySelector('p').innerText}`
        $(e).on('click', () => {
            $('main').load(`/assets/pages/${page}.html`, () => {
                $("#nav-bar")[0].classList.remove("active");
                window.location.hash = page;
                Array.from($(".menu-item")).forEach(n => { n.classList.remove('active') });
                e.classList.add('active')
                init();
            });
        })
        let hash = window.location.hash;
        if (hash == `#${page}`) {
            e.click();
        }
    })
    if (window.location.hash == "") {
        $(".menu-item")[0].click()
    }
}).call();