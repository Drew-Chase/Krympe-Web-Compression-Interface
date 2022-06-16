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

function load(header) {
    let page = document.createElement("div");
    page.id = 'loading-page';
    let head = document.createElement('div')
    head.innerHTML = header;
    let throbber = document.createElement('span')
    throbber.classList.add('loading');

    page.appendChild(head)
    page.appendChild(throbber)
    $("main")[0].appendChild(page);
}

function unload() {
    if ($("#loading-page").length > 0) {
        Array.from($("#loading-page")).forEach(e => e.remove())
    }
}